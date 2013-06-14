// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetStoreProxy.xaml.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



namespace Microsoft.Support.Workflow.Authoring.AssetStore
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Data;
    using CWF.DataContracts;
    using Models;
    using Security;
    using Services;
    using System.Windows.Threading;

    /// <summary>
    /// Class to centralize all operations that call the QueryService
    /// </summary>
    public class AssetStoreProxy
    {
        private readonly static ObservableCollection<string> Categories;

        /// <summary>
        /// Types of workflows
        /// </summary>
        public static ObservableCollection<WorkflowTypesGetBase> WorkflowTypes
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of default activity categories
        /// </summary>
        public static CollectionViewSource ActivityCategories
        {
            get;
            set;
        }


        /// <summary>
        /// Default static constructor for the class
        /// </summary>
        static AssetStoreProxy()
        {
            Categories = new ObservableCollection<string>();
            GetActivityCategories();
            GetWorkflowTypes();
        }

        /// <summary>
        /// Get a list of activity categories from the asset store.
        /// </summary>
        public static bool GetActivityCategories()
        {
            using (var client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
            {
                var request = new ActivityCategoryByNameGetRequestDC
                {
                    Incaller = Assembly.GetExecutingAssembly().GetName().Name,
                    IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    InInsertedByUserAlias = Environment.UserName,
                    InUpdatedByUserAlias = Environment.UserName
                };

                var categoriesCollection = client.ActivityCategoryGet(request);

                categoriesCollection.Add(new ActivityCategoryByNameGetReplyDC { Name = String.Empty }); // we need a blank entry in the import wizard on step #2

                Categories.Assign(from category in categoriesCollection select category.Name);

                ActivityCategories = new CollectionViewSource { Source = Categories };
                ActivityCategories.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
                return true;
            }
        }

        /// <summary>
        /// Get a list of workflow types from the asset store.
        /// </summary>
        public static bool GetWorkflowTypes()
        {
            using (var client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
            {
                var workflowTypes = client.WorkflowTypeGet().WorkflowActivityType;

                if (null != workflowTypes)
                    WorkflowTypes = new ObservableCollection<WorkflowTypesGetBase>(workflowTypes);
                return true;
            }
        }


        /// <summary>
        /// Method to create or update one activity category
        /// </summary>
        /// <param name="categoryName">Name of the category to create or update</param>
        /// <returns>True if the operation was successful</returns>
        public static bool ActivityCategoryCreateOrUpdate(string categoryName)
        {
            using (var client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
            {
                if (String.IsNullOrEmpty(categoryName))
                {
                    throw new ArgumentNullException("categoryName");
                }

                bool result = false;

                var request = new ActivityCategoryCreateOrUpdateRequestDC
                                       {
                                           Incaller = Assembly.GetExecutingAssembly().GetName().Name,
                                           IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                                           InGuid = Guid.NewGuid(),
                                           InAuthGroupName = AuthorizationService.AdminAuthorizationGroupName,
                                           InName = categoryName,
                                           InDescription = categoryName,
                                           InMetaTags = categoryName,
                                           InUpdatedByUserAlias = Environment.UserName,
                                           InInsertedByUserAlias = Environment.UserName
                                       };

                var reply = client.ActivityCategoryCreateOrUpdate(request);
                if (reply.StatusReply.Errorcode == 0)
                {
                    Categories.Add(categoryName);
                    result = true;
                }
                return result;
            }
        }
    }
}
