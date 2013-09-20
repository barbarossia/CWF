// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContractTranslator.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Common;
    using CWF.DataContracts;
    using Models;
    using Security;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    using Microsoft.Support.Workflow.Authoring.AddIns;
    using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
    using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
    using Microsoft.Support.Workflow.Authoring.AddIns.Data;
    using Microsoft.Support.Workflow.Authoring.Security;

    /// <summary>
    /// The data contract translator.
    /// </summary>
    public static class DataContractTranslator
    {
        /// <summary>
        /// The activity item to store activity dc.
        /// </summary>
        /// <param name="activityItem">
        /// The activity item.
        /// </param>
        /// <returns>
        /// Converted StoreActivitiesDC instance
        /// </returns>
        public static StoreActivitiesDC ActivityItemToStoreActivitiyDC(ActivityItem activityItem)
        {
            var storeActivity = new StoreActivitiesDC();

            storeActivity.ActivityCategoryName = activityItem.Category;
            storeActivity.Description = activityItem.Description;
            storeActivity.Name = activityItem.FullName;
            storeActivity.ShortName = activityItem.Name;
            storeActivity.IsCodeBeside = activityItem.HasCodeBehind;
            storeActivity.Version = activityItem.Version;
            storeActivity.Xaml = activityItem.XamlCode;
            storeActivity.MetaTags = activityItem.Tags;
            storeActivity.ActivityCategoryName = activityItem.Category;

            storeActivity.Incaller = Utility.GetCallerName();
            storeActivity.IncallerVersion = Utility.GetCallerVersion();

            storeActivity.DeveloperNotes = activityItem.DeveloperNote;

            storeActivity.InsertedDateTime = activityItem.CreateDateTime;
            storeActivity.InInsertedByUserAlias = !String.IsNullOrEmpty(activityItem.CreatedBy) ? activityItem.CreatedBy : Environment.UserName;

            storeActivity.IsCodeBeside = true;
            storeActivity.StatusCodeName = activityItem.Status;
            storeActivity.Guid = Guid.NewGuid();
            storeActivity.InUpdatedByUserAlias = Utility.GetCurrentUserName();

            storeActivity.Locked = false;
            storeActivity.LockedBy = Environment.UserName;
            storeActivity.InUpdatedByUserAlias = activityItem.UpdatedBy;
            storeActivity.UpdatedDateTime = activityItem.UpdateDateTime;

            storeActivity.OldVersion = activityItem.OldVersion;

            if (activityItem is WorkflowItem)
            {
                storeActivity.WorkflowTypeName = ((WorkflowItem)activityItem).WorkflowType;
                storeActivity.IsService = ((WorkflowItem)activityItem).IfNotNull(wfi => wfi.IsService); 
            }
            else
            {
                //Save default values to the store. When saving the different activities, only the root activity will be a 
                //workflow item, the rest sub-activities will be saved with WorkflowType "metadata" until we remove WorkflowTypeID
                //From the Store Activities table. There is already an ActivityLibraryID to check the library that contains the activity,
                //so this workflowType is not being used at all and should be removed.
                storeActivity.WorkflowTypeName = "Metadata";
                storeActivity.IsService = false;
            }

            storeActivity.Environment = activityItem.Env.ToString();

            return storeActivity;
        }

        public static StoreActivitiesDC ActivityItemToStoreActivitiyDC(ActivityItem activityItem, TaskAssignment task)
        {
            StoreActivitiesDC dc = ActivityItemToStoreActivitiyDC(activityItem);
            dc.Name = dc.ShortName = task.GetFriendlyName(activityItem.Name);
            dc.Version = task.Version;
            dc.Xaml = task.Xaml;
            dc.InInsertedByUserAlias = dc.InUpdatedByUserAlias = task.AssignTo;
            return dc;
        }

        /// <summary>
        /// Prepare to query for an assembly's dependencies
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static StoreActivityLibrariesDependenciesDC ActivityAssemblyItemToStoreActivityLibrariesDependenciesDC(ActivityAssemblyItem assembly)
        {
            // Create the request
            return new StoreActivityLibrariesDependenciesDC()
            {
                Incaller = Utility.GetCallerName(),
                IncallerVersion = Utility.GetCallerVersion(),
                StoreDependenciesRootActiveLibrary = new StoreDependenciesRootActiveLibrary
                {
                    activityLibraryName = assembly.Name,
                    activityLibraryVersionNumber = assembly.Version.ToString(),
                    Environment = assembly.Env.ToString()
                }
            };
        }

        /// <summary>
        /// The activity library dc to activity assembly item.
        /// </summary>
        /// <param name="activityLibraryDC">
        /// The activity library dc.
        /// </param>
        /// <param name="assemblyReferences">
        /// Referenced assemblies for this ActivityLibraryDC. Use this parameter when you are actually downloading the library
        /// to cache and use, vs. just displaying it in a Select screen. It would be great if this info were actually part of
        /// ActivityLibraryDC but it isn't so we have to collect it separately.
        /// </param>
        /// <returns>
        /// Converted ActivityAssemblyItem instance
        /// </returns>
        public static ActivityAssemblyItem ActivityLibraryDCToActivityAssemblyItem(ActivityLibraryDC activityLibraryDC,
                                                                                   IEnumerable<AssemblyName> assemblyReferences = null)
        {
            var activityAssemblyItem = new ActivityAssemblyItem();
            Version version;

            activityAssemblyItem.ActivityItems = new ObservableCollection<ActivityItem>();
            activityAssemblyItem.Assembly = null;
            activityAssemblyItem.AssemblyName = new AssemblyName(activityLibraryDC.Name);
            System.Version.TryParse(activityLibraryDC.VersionNumber, out version);
            activityAssemblyItem.Version = version;
            activityAssemblyItem.AuthorityGroup = activityLibraryDC.AuthGroupName;
            activityAssemblyItem.CachingStatus = CachingStatus.None;
            activityAssemblyItem.Category = activityLibraryDC.CategoryName;

            activityAssemblyItem.Description = activityLibraryDC.Description;
            activityAssemblyItem.DisplayName = activityLibraryDC.Name;
            activityAssemblyItem.Status = activityLibraryDC.StatusName;
            activityAssemblyItem.Tags = activityLibraryDC.MetaTags;

            if (assemblyReferences != null)
            {
                activityAssemblyItem.SetDatabaseReferences(assemblyReferences);
            }

            activityAssemblyItem.UserSelected = false;
            activityAssemblyItem.ReleaseNotes = activityLibraryDC.ReleaseNotes;
            activityAssemblyItem.FriendlyName = activityLibraryDC.FriendlyName;

            activityAssemblyItem.Env = activityLibraryDC.Environment.ToEnv();

            return activityAssemblyItem;
        }

        /// <summary>
        /// The assembly name local location location to activity library dc.
        /// </summary>
        /// <param name="aai">
        /// The  assemblyLocationItems.
        /// </param>
        /// <returns>
        /// Converted ActivityLibraryDC instance
        /// </returns>
        public static ActivityLibraryDC AssemblyItemToActivityLibraryDataContract(ActivityAssemblyItem aai)
        {
            var activityLibrary = new ActivityLibraryDC();
            activityLibrary.Name = aai.AssemblyName.Name;
            activityLibrary.VersionNumber = aai.Version.ToString();

            // TODO: remove these
            activityLibrary.Incaller = Utility.GetCallerName();
            activityLibrary.IncallerVersion = Utility.GetCallerVersion();
            activityLibrary.Guid = Guid.NewGuid();
            activityLibrary.AuthGroupName = aai.AuthorityGroup;
            activityLibrary.CategoryName = aai.Category ?? "OAS Basic Controls";
            activityLibrary.Category = Guid.Empty;
            activityLibrary.Executable = new byte[4];
            activityLibrary.HasActivities = false;
            activityLibrary.ImportedBy = Utility.GetCurrentUserName();
            activityLibrary.Description = aai.Description;
            activityLibrary.Status = 1;

            activityLibrary.Environment = aai.Env.ToString();

            return activityLibrary;
        }

        /// <summary>
        /// The store activity dc to activity item.
        /// </summary>
        /// <param name="dc">
        /// The StoreActivitiesDC data contract instance.
        /// </param>
        /// <param name="parentAssemblyItem">
        /// The parent assembly item.
        /// </param>
        /// <returns>
        /// Converted ActivityItem instance
        /// </returns>
        public static ActivityItem StoreActivitiyDCToActivityItem(
            StoreActivitiesDC dc, ActivityAssemblyItem parentAssemblyItem)
        {
            var activityItem = new ActivityItem();

            activityItem.CachingStatus = CachingStatus.None;
            activityItem.Category = dc.ActivityCategoryName;

            activityItem.CreateDateTime = dc.InsertedDateTime;
            activityItem.CreatedBy = dc.InInsertedByUserAlias;
            activityItem.Description = dc.Description;
            activityItem.FullName = dc.Name;
            activityItem.Name = dc.ShortName ?? dc.Name;
            activityItem.DisplayName = dc.ShortName ?? dc.Name;
            activityItem.HasCodeBehind = dc.IsCodeBeside;
            activityItem.IsReadOnly = true; // TODO: Need to find out where this is in the Model
            activityItem.IsUserFavorite = false; // TODO: This needs to be added to the Database, DAL, BAL and/or DataContract reply 
            activityItem.ParentAssemblyItem = parentAssemblyItem;

            activityItem.Status = dc.StatusCodeName;
            activityItem.UpdatedBy = dc.InUpdatedByUserAlias;
            activityItem.UpdateDateTime = dc.UpdatedDateTime;
            activityItem.UserSelected = true;  // TODO: Why is this set to true?
            activityItem.Version = activityItem.OldVersion = dc.Version;
            activityItem.XamlCode = dc.Xaml;

            activityItem.Env = dc.Environment.ToEnv();

            return activityItem;
        }

        public static WorkflowItem StoreActivitiyDCToWorkflowItem(
            StoreActivitiesDC dc, 
            ActivityAssemblyItem parentAssemblyItem, 
            List<ActivityAssemblyItem> references = null,
            bool isTask = false)
        {
            var workflowItem = new WorkflowItem(dc.Name, dc.Name, dc.Xaml, dc.WorkflowTypeName, references);

            workflowItem.CachingStatus = CachingStatus.None;
            workflowItem.Category = dc.ActivityCategoryName;

            workflowItem.CreateDateTime = dc.InsertedDateTime;
            workflowItem.CreatedBy = dc.InInsertedByUserAlias;
            workflowItem.Description = dc.Description;
            workflowItem.FullName = dc.Name;
            workflowItem.Name = dc.Name;
            workflowItem.DisplayName = dc.ShortName ?? dc.Name;
            workflowItem.HasCodeBehind = dc.IsCodeBeside;
            workflowItem.ParentAssemblyItem = parentAssemblyItem;
            workflowItem.Status = dc.StatusCodeName;
            workflowItem.UpdatedBy = dc.InUpdatedByUserAlias;
            workflowItem.UpdateDateTime = dc.UpdatedDateTime;
            workflowItem.IsSavedToServer = true;
            workflowItem.Version = workflowItem.OldVersion = dc.Version;
            workflowItem.IsDataDirty = false;
            workflowItem.Tags = dc.MetaTags;
            workflowItem.XamlCode = dc.Xaml;
            workflowItem.WorkflowType = dc.WorkflowTypeName;
            workflowItem.IsOpenFromServer = true;
            workflowItem.IsTask = isTask;

            workflowItem.Env = dc.Environment.ToEnv();

            return workflowItem;
        }

        /// <summary>
        /// Take a WorkflowItem and the computed list of ActivityAssemblyItems it depends on and translate them into DC form for an upload
        /// </summary>
        /// <param name="workflow"></param>
        /// <param name="assemblyItemsUsed">Assemblies list (computed by WorkflowUploader.ComputeDependencies())</param>
        /// <returns></returns>
        public static StoreLibraryAndActivitiesRequestDC WorkflowToStoreLibraryAndActivitiesRequestDC(WorkflowItem workflow, IEnumerable<ActivityAssemblyItem> assemblyItemsUsed, List<TaskAssignment> tasks)
        {
            string libraryName = workflow.Name;
            var library = GetActivityLibraryDC(libraryName, workflow.Category, workflow.Description, workflow.CreatedBy, workflow.Version, workflow.Status, workflow.Env);
            var dependencyList = new List<StoreActivityLibraryDependenciesGroupsRequestDC>(
                assemblyItemsUsed.Select(asm => new StoreActivityLibraryDependenciesGroupsRequestDC
                {
                    Name = asm.Name,
                    Version = asm.Version.ToString(),
                }.SetIncaller())
            );

            return new StoreLibraryAndActivitiesRequestDC
            {
                Incaller = Utility.GetCallerName(),
                IncallerVersion = Utility.GetCallerVersion(),
                InInsertedByUserAlias = !String.IsNullOrEmpty(workflow.CreatedBy) ? workflow.CreatedBy : Utility.GetCurrentUserName(),
                InUpdatedByUserAlias = Utility.GetCurrentUserName(),
                EnforceVersionRules = true,
                ActivityLibrary = library,
                StoreActivitiesList = new List<StoreActivitiesDC> {
                    // There is only one activity in this "library", = the workflow itself
                    ActivityItemToStoreActivitiyDC(workflow)
                },
                StoreActivityLibraryDependenciesGroupsRequestDC = new StoreActivityLibraryDependenciesGroupsRequestDC()
                {
                    Name = libraryName,
                    Version = workflow.Version,
                    List = dependencyList
                }.SetIncaller(),
                TaskActivitiesList = tasks.Select(t => new StoreLibraryAndTaskActivityRequestDC()
                {
                    EnforceVersionRules = true,
                    ActivityLibrary = GetActivityLibraryDC(t.GetFriendlyName(workflow.Name), workflow.Category, workflow.Description, workflow.CreatedBy, t.Version, workflow.Status, workflow.Env),
                    StoreActivityLibraryDependenciesGroupsRequestDC = new StoreActivityLibraryDependenciesGroupsRequestDC()
                    {
                        Name = t.GetFriendlyName(workflow.Name),
                        Version = t.Version,
                        List = dependencyList
                    },
                    TaskActivitiesList = new List<TaskActivityDC>()
                    {
                        new TaskActivityDC()
                        {
                            Guid = t.TaskId,
                            AssignedTo = t.AssignTo,
                            Activity = ActivityItemToStoreActivitiyDC(workflow, t),
                            Status = t.TaskStatus == TaskActivityStatus.Unassigned ? TaskActivityStatus.Unassigned : TaskActivityStatus.Assigned,
                        }
                    }
                }.SetIncaller()).ToList()
            };
        }

        public static StoreLibraryAndTaskActivityRequestDC WorkflowToStoreLibraryAndTaskActivityRequestDC(WorkflowItem workflow, IEnumerable<ActivityAssemblyItem> assemblyItemsUsed)
        {
            string libraryName = workflow.Name;
            var library = GetActivityLibraryDC(libraryName, workflow.Category, workflow.Description, workflow.CreatedBy, workflow.Version, workflow.Status, workflow.Env);
            var dependencyList = new List<StoreActivityLibraryDependenciesGroupsRequestDC>(
                assemblyItemsUsed.Select(asm => new StoreActivityLibraryDependenciesGroupsRequestDC
                {
                    Name = asm.Name,
                    Version = asm.Version.ToString(),
                }.SetIncaller())
            );

            return new StoreLibraryAndTaskActivityRequestDC
            {
                Incaller = Utility.GetCallerName(),
                IncallerVersion = Utility.GetCallerVersion(),
                InInsertedByUserAlias = !String.IsNullOrEmpty(workflow.CreatedBy) ? workflow.CreatedBy : Utility.GetCurrentUserName(),
                InUpdatedByUserAlias = Utility.GetCurrentUserName(),
                EnforceVersionRules = true,
                ActivityLibrary = library,
                Environment = workflow.Env.ToString(),
                StoreActivityLibraryDependenciesGroupsRequestDC = new StoreActivityLibraryDependenciesGroupsRequestDC()
                {
                    Name = libraryName,
                    Version = workflow.Version,
                    List = dependencyList
                }.SetIncaller(),
                TaskActivitiesList = new List<TaskActivityDC>()
                {
                    new TaskActivityDC()
                    {
                        Guid = workflow.TaskActivityGuid.Value,
                        AssignedTo = Utility.GetCurrentUserName(),
                        Activity = ActivityItemToStoreActivitiyDC(workflow),
                        Status = workflow.TaskActivityStatus.Value,
                        Environment = workflow.Env.ToString()
                    }
                }
            };
        }

        private static ActivityLibraryDC GetActivityLibraryDC(
            string libraryName,
            string category,
            string description,
            string insertBy,
            string version,
            string status,
            Env env)
        {
            var library = new ActivityLibraryDC()
            {
                Name = libraryName,
                Executable = null,
                CategoryName = category,
                ImportedBy = Utility.GetCurrentUserName(),
                Description = description,
                InInsertedByUserAlias =
                    !String.IsNullOrEmpty(insertBy)
                        ? insertBy
                        : Utility.GetCurrentUserName(),
                VersionNumber = version,
                InUpdatedByUserAlias = Utility.GetCurrentUserName(),
                Guid = Guid.NewGuid(),
                StatusName = status,
                Environment = env.ToString()
            };
            return library;
        }

        /// <summary>
        /// Take a WorkflowItem and the computed list of ActivityAssemblyItems it depends on and translate them into DC form for an upload
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="assemblyItemsUsed">Assemblies list (computed by WorkflowUploader.ComputeDependencies())</param>
        /// <returns></returns>
        public static StoreLibraryAndActivitiesRequestDC ToStoreLibraryAndActivitiesRequestDC(this ActivityAssemblyItem assembly, IEnumerable<ActivityAssemblyItem> assemblyItemsUsed = null)
        {
            return new StoreLibraryAndActivitiesRequestDC
            {
                Incaller = Utility.GetCallerName(),
                IncallerVersion = Utility.GetCallerVersion(),
                InInsertedByUserAlias = !String.IsNullOrEmpty(assembly.CreatedBy) ? assembly.CreatedBy : Utility.GetCurrentUserName(),
                InUpdatedByUserAlias = Utility.GetCurrentUserName(),
                // TODO, bug #21381, Remove hardcoded values from WorkflowUploader

                ActivityLibrary = new ActivityLibraryDC()
                {
                    Name = assembly.Name,
                    VersionNumber = assembly.Version.ToString(),
                    Executable = File.ReadAllBytes(assembly.Location),
                    Description = assembly.Description,
                    CategoryName = assembly.Category,
                    ImportedBy = Utility.GetCurrentUserName(),
                    InInsertedByUserAlias = !String.IsNullOrEmpty(assembly.CreatedBy) ? assembly.CreatedBy : Utility.GetCurrentUserName(),
                    InUpdatedByUserAlias = Utility.GetCurrentUserName(),
                    Guid = Guid.NewGuid(),
                    Environment = assembly.Env.ToString()
                }, // library for this workflow
                StoreActivitiesList = new List<StoreActivitiesDC>(assembly.ActivityItems.Select(asm => DataContractTranslator.ActivityItemToStoreActivitiyDC(asm))),
                StoreActivityLibraryDependenciesGroupsRequestDC = assemblyItemsUsed.IfNotNull(used =>
                    new StoreActivityLibraryDependenciesGroupsRequestDC()
                    {
                        Name = assembly.Name,
                        Version = assembly.Version.ToString(),
                        List = new List<StoreActivityLibraryDependenciesGroupsRequestDC>(
                            used.Select(asm =>
                                new StoreActivityLibraryDependenciesGroupsRequestDC
                                {
                                    Name = asm.Name,
                                    Version = asm.Version.ToString(),
                                }.SetIncaller())
                        )
                    }.SetIncaller())
            };
        }
    }
}
