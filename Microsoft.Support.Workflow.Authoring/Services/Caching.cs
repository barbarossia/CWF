// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Caching.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.ServiceModel;
    using Common;
    using CWF.DataContracts;
    using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
    using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;
    using Models;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    using Microsoft.Support.Workflow.Authoring.AddIns;

    /// <summary>
    /// The caching class. Represent the local caching.
    /// This class helps you to read and write local caching.
    /// There are 2 categories in local cashing folder -- All activity assemblies with their activity type children; All assemblies with their local location.
    /// </summary>
    public class Caching
    {
        /// <summary>
        /// Initializes static members of the <see cref="Caching"/> class.
        /// </summary>
        static Caching()
        {
            ActivityAssemblyItems = new ObservableCollection<ActivityAssemblyItem>();
            ActivityItems = new ObservableCollection<ActivityItem>();
        }

        /// <summary>
        /// Gets or sets all cached ActivityAssemblyItems.
        /// </summary>
        public static ObservableCollection<ActivityAssemblyItem> ActivityAssemblyItems { get; set; }

        /// <summary>
        /// Gets or sets all cached ActivityItems.
        /// </summary>
        public static ObservableCollection<ActivityItem> ActivityItems { get; set; }

        /// <summary>
        /// The cache assembly.
        /// </summary>
        /// <param name="activityAssemblyItems">
        /// The activity assembly items.
        /// </param>
        public static void CacheAssembly(List<ActivityAssemblyItem> activityAssemblyItems, bool isFromServer = false)
        {
            if (activityAssemblyItems == null)
            {
                throw new ArgumentNullException("activityAssemblyItems");
            }

            if (activityAssemblyItems.Any(item => item == null))
            {
                throw new ArgumentNullException("activityAssemblyItems");
            }

            foreach (ActivityAssemblyItem assemblyItem in activityAssemblyItems)
            {
                // Skip cached item
                if (assemblyItem.CachingStatus == CachingStatus.Latest)
                {
                    continue;
                }

                // Check if a location is already in location catalog. If true, remove it first.
                ActivityAssemblyItem cachedAssembly;
                if (Utility.LoadCachedAssembly(ActivityAssemblyItems, assemblyItem.AssemblyName, out cachedAssembly))
                {
                    ActivityAssemblyItems.Remove(cachedAssembly);
                }

                // Copy assemblies to local caching directory
                string destFileName = Utility.CopyAssemblyToLocalCachingDirectory(assemblyItem.AssemblyName, assemblyItem.Location, false);
                // break link to original location by resetting Location and AssemblyName.CodeBase
                assemblyItem.Location = destFileName;
                assemblyItem.AssemblyName.CodeBase = null;
                assemblyItem.UpdateDateTime = DateTime.Now;
                assemblyItem.CachingStatus = CachingStatus.Latest;

                if (isFromServer)
                {
                    var inspection = Utility.GetAssemblyInspectionService();
                    inspection.Inspect(destFileName);
                    assemblyItem.ActivityItems = inspection.SourceAssembly.ActivityItems;
                    assemblyItem.UserSelected = true;
                    assemblyItem.ActivityItems.ToList().ForEach(i =>
                    {
                        i.UserSelected = true;
                        i.Category = "Unassigned";
                    });
                }

                // Make ActivityItem read only. Note: ActivityItem's metadata can be edited only when imported.
                foreach (var activityItem in assemblyItem.ActivityItems)
                {
                    activityItem.IsReadOnly = true;
                    activityItem.CachingStatus = CachingStatus.Latest;
                }

                ActivityAssemblyItems.Add(assemblyItem);
            }
        }

        /// <summary>
        /// Reload data from local caching categories.
        /// </summary>
        public static void Refresh()
        {
            // Serialize and save
            using (var stream = File.Open(Utility.GetActivityAssemblyCatalogFileName(), FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, ActivityAssemblyItems.ToList());
            }

            LoadFromLocal();   // Reload from the local cache.
        }

        /// <summary>
        /// Load caching from local hard drive.
        /// </summary>
        public static void LoadFromLocal()
        {
            ActivityAssemblyItems.Clear();
            ActivityItems.Clear();

            if (File.Exists(Utility.GetActivityAssemblyCatalogFileName()))
            {
                var deserialized = Utility.DeserializeSavedContent(Utility.GetActivityAssemblyCatalogFileName()) as IEnumerable<ActivityAssemblyItem>;
                ActivityAssemblyItems = new ObservableCollection<ActivityAssemblyItem>(deserialized);
                ActivityAssemblyItems
                    .SelectMany(activityAssemblyItem => activityAssemblyItem.ActivityItems)
                    .ToList()
                    .ForEach(item => ActivityItems.Add(item));
            }
        }

        /// <summary>
        /// Compute ActivityAssemblyItem dependencies
        /// </summary>
        /// <param name="client"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static List<ActivityAssemblyItem> ComputeDependencies(IWorkflowsQueryService client, ActivityAssemblyItem assembly)
        {
            return ComputeDependencies(client, new[] { assembly }.ToList());
        }

        ///<summary>
        /// Compute ActivityAssemblyItem dependencies
        /// </summary>
        /// <param name="client"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static List<ActivityAssemblyItem> ComputeDependencies(IWorkflowsQueryService client, List<ActivityAssemblyItem> assemblies)
        {
            List<ActivityLibraryDC> assembliesOnServer;
            var dependencies = new MultiMap<int, int>();
            var neededIDs = new HashSet<int>();

            try
            {
                assembliesOnServer = client.GetAllActivityLibraries(new GetAllActivityLibrariesRequestDC().SetIncaller()).List;
            }
            catch (FaultException<ServiceFault> ex)
            {
                throw new CommunicationException(ex.Detail.ErrorMessage);
            }
            catch (FaultException<ValidationFault> ex)
            {
                throw new BusinessValidationException(ex.Detail.ErrorMessage);
            }
            catch (Exception ex)
            {
                throw new CommunicationException(ex.Message);
            }

            // get dependencies for each assembly
            foreach (var assembly in assemblies)
            {
                var dependenciesByID = client.StoreActivityLibraryDependenciesTreeGet(DataContractTranslator.ActivityAssemblyItemToStoreActivityLibrariesDependenciesDC(assembly));
                // for some reason, QueryService sends back list of lists instead of flattened list
                foreach (var dependencyContainer in dependenciesByID)
                {
                    foreach (var dependency in dependencyContainer.StoreDependenciesDependentActiveLibraryList)
                    {
                        // If A needs B, then A is the "parent" according to the data contract
                        var needs = dependency.activityLibraryParentId;
                        var needed = dependency.activityLibraryDependentId;
                        dependencies.AddValue(needs, needed);
                        neededIDs.Add(needed);
                    }
                }
            }

            var lookupById = new Dictionary<int, AssemblyName>();
            foreach (var serverAssembly in assembliesOnServer)
            {
                // determine the version number (can be nullable)
                Version serverAssemblyVersion = Version.TryParse(serverAssembly.VersionNumber, out serverAssemblyVersion) ? serverAssemblyVersion : null;

                // Don't use assembliesOnServer.ToDictionary() because test code can have key conflicts, e.g. all Id = 0 for some tests
                // that don't care about dependencies.
                lookupById[serverAssembly.Id] = new AssemblyName { Name = serverAssembly.Name, Version = serverAssemblyVersion };
            }

            var assembliesAndDependencies = new List<ActivityAssemblyItem>(
                from dbAsm in assembliesOnServer
                where neededIDs.Contains(dbAsm.Id)
                let cached = ActivityAssemblyItems.FirstOrDefault(
                         cached => string.Equals(cached.Name, dbAsm.Name) && string.Equals(cached.Version, dbAsm.VersionNumber)
                       )
                // prefer cached version over DB version to prevent re-download
                select cached ?? DataContractTranslator.ActivityLibraryDCToActivityAssemblyItem(dbAsm, from depId in dependencies.GetValues(dbAsm.Id).Distinct() let dependency = lookupById[depId] orderby dependency.Name, dependency.Version select dependency)
                );

            return assembliesAndDependencies; // pull it out into a separate variable for debuggability, vs. returning immediately
        }

        /// <summary>
        /// Download and cache assembly.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="assembliesToCache">
        /// The activity assembly items.
        /// </param>
        public static void DownloadAndCacheAssembly(IWorkflowsQueryService client, List<ActivityAssemblyItem> assembliesToCache)
        {
            // Check for any new assemblies that we need to get from the server, and cache them
            List<ActivityAssemblyItem> newlyDownloadedAssemblies = DownloadAssemblies(client, assembliesToCache);
            if (newlyDownloadedAssemblies.Any())
            {
                Caching.CacheAssembly(newlyDownloadedAssemblies, true);
                Caching.Refresh();
            }
        }

        public static List<ActivityAssemblyItem> CacheAndDownloadAssembly(IWorkflowsQueryService client, List<ActivityAssemblyItem> assembliesToCache)
        {
            DownloadAndCacheAssembly(client, assembliesToCache);
            return Match(assembliesToCache);
        }

        public static List<ActivityAssemblyItem> Match(List<ActivityAssemblyItem> assembliesToCache)
        {
            var result = (from cache in ActivityAssemblyItems
                          from download in assembliesToCache
                          where cache.Matches(download)
                          select cache).ToList();

            return result;
        }

        public static List<ActivityAssemblyItem> DownloadAssemblies(IWorkflowsQueryService client, List<ActivityAssemblyItem> assemblies)
        {
            // Ignore any assembly which exists in the cache already
            IEnumerable<ActivityAssemblyItem> assembliesToDownload =
                assemblies.Where(
                    item => !ActivityAssemblyItems.Any(assemblyItem => assemblyItem.Name == item.Name &&
                        assemblyItem.Version == item.Version));


            var downloadedAssemblies = new List<ActivityAssemblyItem>();
            foreach (ActivityAssemblyItem assembly in assembliesToDownload)
            {
                byte[] byteArray = GetExecutableBytes(client, assembly);
                // Only if we get a byte array do we try and process it
                if (null != byteArray)
                {
                    // try to create an ActivityAssemblyItem from the bytes
                    try
                    {
                        // The hosted VB compiler that we use isn't compatible with Assembly.Load(byte[])
                        // so we write the bytes to a temp file and load that
                        string sourceFileName = AssemblyDownloader.ByteArrayToFile(byteArray);

                        // We also cache the location for subsequent runs of the authoring tool
                        assembly.Location = sourceFileName;

                        assembly.AssemblyName = AssemblyName.GetAssemblyName(sourceFileName);

                        // Hit the database again in order to get metadata for the activities in the assembly

                        downloadedAssemblies.Add(assembly);
                    }

                    catch (BadImageFormatException)
                    {
                        // Treat garbage .dlls as if they had been null Executables, i.e. they are silently skipped
                    }
                }
            }
            return downloadedAssemblies;
        }

        /// <summary>
        /// Method gets the Executable that this Activity Library represents
        /// </summary>
        /// <param name="client">The WCF Service</param>
        /// <param name="aai">The ActivityAssemblyItem</param>
        /// <returns>a Byte[]</returns>
        public static byte[] GetExecutableBytes(IWorkflowsQueryService client, ActivityAssemblyItem aai)
        {
            List<ActivityLibraryDC> replyList;

            if (aai == null)
            {
                throw new Exception("aai");
            }

            byte[] sourceFileBytes = null;

            var request = new ActivityLibraryDC
            {
                Name = aai.Name,
                VersionNumber = aai.Version.ToString(),
                Incaller = Environment.UserName,
                IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            };

            try
            {
                replyList = client.ActivityLibraryGet(request);
            }
            catch (FaultException<ServiceFault> ex)
            {
                throw new CommunicationException(ex.Detail.ErrorMessage);
            }
            catch (FaultException<ValidationFault> ex)
            {
                throw new BusinessValidationException(ex.Detail.ErrorMessage);
            }
            catch (Exception ex)
            {
                throw new CommunicationException(ex.Message);
            }

            if ((null != replyList) && (0 < replyList.Count))
            {
                sourceFileBytes = replyList[0].Executable;
            }

            return sourceFileBytes;
        }

        /// <summary>
        /// Try to retrieve a cached assembly
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="cachedAssembly"></param>
        /// <returns></returns>
        public static bool LoadCachedAssembly(AssemblyName assemblyName,out ActivityAssemblyItem cachedAssembly)
        {
            return Utility.LoadCachedAssembly(ActivityAssemblyItems, assemblyName, out cachedAssembly);
        }
    }
}
