// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyDownloader.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace Microsoft.Support.Workflow.Authoring.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using Models;
    using CWF.DataContracts;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

    /// <summary>
    /// The assembly downloader.
    /// </summary>
    public class AssemblyDownloader
    {
        /// <summary>
        /// Convert a byte array to file.
        /// </summary>
        /// <param name="byteArray">
        /// The byte array.
        /// </param>
        /// <returns>
        /// The byte array of file content.
        /// </returns>
        public static string ByteArrayToFile(byte[] byteArray)
        {
            string destFileName = string.Format(@"{0}\{1}.dll", FileService.GetTempDirectoryPath(), Guid.NewGuid());

            File.WriteAllBytes(destFileName, byteArray);

            return destFileName;
        }

        /// <summary>
        /// The get activity items by activity assembly item.
        /// </summary>
        /// <param name="activityAssemblyItem">
        /// The activity assembly item.
        /// </param>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <returns>
        /// ActivityItems contained in ActivityAssembly
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public static List<ActivityItem> GetActivityItemsByActivityAssemblyItem(ActivityAssemblyItem activityAssemblyItem, IWorkflowsQueryService client)
        {
            var request = new GetActivitiesByActivityLibraryNameAndVersionRequestDC
                {
                    Name = activityAssemblyItem.Name,
                    VersionNumber = activityAssemblyItem.Version.ToString(),
                    Environment = activityAssemblyItem.Env.ToString(),
                    Incaller = Environment.UserName,
                    IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                };
            var activityItems = client.GetActivitiesByActivityLibraryNameAndVersion(request).List;

            var activityItemCollection = new List<ActivityItem>();

            foreach (StoreActivitiesDC item in activityItems)
            {
                activityItemCollection.Add(DataContractTranslator.StoreActivitiyDCToActivityItem(item, activityAssemblyItem));
            }

            activityAssemblyItem.ActivityItems = new ObservableCollection<ActivityItem>(activityItemCollection);

            foreach (ActivityItem ai in activityItemCollection)
            {
                ai.ParentAssemblyItem = activityAssemblyItem;
            }

            return activityItemCollection;
        }
    }
}