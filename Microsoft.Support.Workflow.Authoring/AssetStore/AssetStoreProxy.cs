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
    using Data = Microsoft.Support.Workflow.Authoring.AddIns.Data;
    using System.Configuration;
    using System.ServiceModel.Configuration;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;

    /// <summary>
    /// Class to centralize all operations that call the QueryService
    /// </summary>
    public class AssetStoreProxy
    {
        public readonly static ObservableCollection<string> Categories;

        /// <summary>
        /// Types of workflows
        /// </summary>
        public static ObservableCollection<WorkflowTypesGetBase> WorkflowTypes
        {
            get;
            private set;
        }

        public static string TenantName
        {
            get;
            private set;
        }

        public static string ClientEndPoint
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

        public CollectionViewSource NewActivityCategories
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
            GetTenantName();
            GetClientEndpoint();
        }

        public AssetStoreProxy()
        {
            if (ActivityCategories != null)
            {
                NewActivityCategories = new CollectionViewSource { Source = Categories };
                NewActivityCategories.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
            }
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
                Categories.Assign(from category in categoriesCollection orderby category.Name select category.Name);
                ActivityCategories = new CollectionViewSource { Source = Categories };
                ActivityCategories.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
                return true;
            }
        }

        /// <summary>
        /// Get a list of workflow types from the asset store.
        /// </summary>
        public static bool GetWorkflowTypes(Env env)
        {
            using (var client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
            {
                WorkflowTypesGetRequestDC request = new WorkflowTypesGetRequestDC();
                request.SetIncaller();
                request.Environment = env.ToString();
                var workflowTypes = client.WorkflowTypeGet(request).WorkflowActivityType;
                if (null != workflowTypes)
                    WorkflowTypes = new ObservableCollection<WorkflowTypesGetBase>(workflowTypes);
                return true;
            }
        }

        public static void GetTenantName()
        {
            using (var client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
            {
                TenantName = client.TenantGet();
            }
        }

        public static void GetClientEndpoint()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); ServiceModelSectionGroup serviceModelSectionGroup =
            ServiceModelSectionGroup.GetSectionGroup(
            ConfigurationManager.OpenExeConfiguration(
            ConfigurationUserLevel.None));
            ClientSection clientSection = configuration.GetSection("system.serviceModel/client") as ClientSection; //serviceModelSectionGroup.Client;
            ChannelEndpointElement e = serviceModelSectionGroup.Client.Endpoints[0];
            ClientEndPoint = e.Address.AbsoluteUri;
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
