using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CWF.DataContracts;
using System.Collections.ObjectModel;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Microsoft.Support.Workflow.Authoring.Tests
{
    /// <summary>
    /// This Class Dynamically extract latest data from tables and 
    /// verify if data is inserted as expected
    /// </summary>
    public class DataVerifier
    {
        SortedDictionary<string, StoreActivitiesDC> _storeActivitesTable;
        SortedDictionary<string, ActivityAssemblyItem> _activityLibrariesTable;
        string ActivityName;

        public static string NotFoundInStoreActicityTable = "WorkFlow is not found in Store Acticity Table";
        public static string NotFoundInActicityLibraryTable = "WorkFlow is not found in ActicityLibrary Table";

        /// <summary>
        /// constructor
        /// </summary>
        public DataVerifier(string activityName)
        {
            //Load Data
            ActivityName = activityName;
            WorkflowsQueryServiceUtility.UsingClient(client => _storeActivitesTable = StoreActivitesTable(client, ActivityName));
        }

        /// <summary>
        /// Is used to refresh data if change is made
        /// </summary>
        public bool DataRefresh
        {
            get
            {
                //Load Data
                WorkflowsQueryServiceUtility.UsingClient(client => _storeActivitesTable = StoreActivitesTable(client, ActivityName));
                return true;
            }
        }

        /// <summary>
        /// Checks workflow expected properties in the Store Activities tables
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="workFlowItem"></param>
        /// <param name="isItemFound"></param>
        public void VerifyWorkflowPropertiesInStoreActivitesTable(WorkFlowProperties properties, WorkflowItem workFlowItem, MainWindowViewModel viewModel)
        {
            var key = string.Format("{0}", workFlowItem.Name.ToLower());

            if (!_storeActivitesTable.ContainsKey(key))
            {
                string errorMessage = NotFoundInStoreActicityTable;
                if (viewModel != null && !string.IsNullOrEmpty(viewModel.ErrorMessage))
                {
                    errorMessage = viewModel.ErrorMessage;
                }
                throw new InvalidOperationException(errorMessage);
            }
            var storeActivitesTable = _storeActivitesTable[key];

            //Verification In Store Activities table
            if (!string.IsNullOrEmpty(properties.Name))
            {
                Assert.IsTrue(storeActivitesTable.Name == properties.Name,
                             string.Format("WorkFlow Name Expected:{0}  but not found in Store Activities Table.", properties.Name));
            }

            if (!string.IsNullOrEmpty(properties.Version))
            {
                Assert.IsTrue(storeActivitesTable.Version == properties.Version,
                                 string.Format(
                                     "WorkFlow Version Expected:{0}  but not found in Store Activities Table.", properties.Version));
            }

            if (!string.IsNullOrEmpty(properties.CreatedBy))
            {
                Assert.IsTrue(storeActivitesTable.InInsertedByUserAlias.Contains(properties.CreatedBy),
                                 string.Format("WorkFlow CreatedBy Expected:{0}  but not found in Store Activites Table.", properties.CreatedBy));
            }

            //if (properties.CreateDateTime != null)
            //{
            //    Assert.IsTrue(storeActivitesTable.InsertedDateTime.Equals(properties.CreateDateTime),
            //                     string.Format("WorkFlow CreateDateTime Expected:{0}  but not found in Store Activites Table.", properties.CreateDateTime));
            //}

            //if (properties.UpdateDateTime != null)
            //{
            //    Assert.IsTrue(storeActivitesTable.UpdatedDateTime.Equals(properties.UpdateDateTime),
            //                     string.Format("WorkFlow CreateDateTime Expected:{0}  but not found in Store ActivitesTable.", properties.UpdateDateTime));
            //}

            if (!string.IsNullOrEmpty(properties.Description))
            {
                Assert.IsTrue(storeActivitesTable.Description.Contains(properties.Description),
                                 string.Format("WorkFlow Description Expected:{0}  but not found in Store Activites Table.", properties.Description));
            }

            if (!string.IsNullOrEmpty(properties.DeveloperNote))
            {
                Assert.IsTrue(storeActivitesTable.DeveloperNotes.Contains(properties.DeveloperNote),
                                 string.Format("WorkFlow DeveloperNote Expected:{0}  but not found in Store Activites Table.", properties.DeveloperNote));
            }
        }

        /// <summary>
        /// Checks workflow expected properties in the Activity Libraries tables
        /// </summary>
        /// <param name="properties"> </param>
        /// <param name="workFlowItem"> </param>
        /// <param name="isItemFound"></param>
        public void VerifyWorkflowPropertiesInActivityLibrariesTable(WorkFlowProperties properties, WorkflowItem workFlowItem, MainWindowViewModel viewModel)
        {
            var key = string.Format("{0}", workFlowItem.Name.ToLower());

            ActivityLibraryDC dc = new ActivityLibraryDC
          {
              Incaller = Environment.UserName,
              IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
              Name =key
          };
         
            List<ActivityLibraryDC> replyDC = null;
            WorkflowsQueryServiceUtility.UsingClient(client => replyDC = client.ActivityLibraryGet(dc));
            // Require a pre-check since the list is not reliably instantiated 
            // by the WCF service
            //if not found in table, throw exception
            if ( replyDC==null || replyDC.Count<=0)
            {
                string errorMessage = NotFoundInActicityLibraryTable;
                if (viewModel != null && !string.IsNullOrEmpty(viewModel.ErrorMessage))
                {
                    errorMessage = viewModel.ErrorMessage;
                }
                throw new InvalidOperationException(errorMessage);
            }
            var activityLibrariesTable = replyDC;
          
            //Verification In Activity Libraries table
            if (!string.IsNullOrEmpty(properties.Name))
            {
                Assert.IsTrue(activityLibrariesTable.Select(l=>l.Name).ToList().Contains(properties.Name),
                                 string.Format("WorkFlow Name Expected:{0}  but not found in Activity Libraries Table.", properties.Name));
            }

            if (!string.IsNullOrEmpty(properties.Version))
            {
                Assert.IsTrue(activityLibrariesTable.Select(l=>l.VersionNumber).ToList().Contains(properties.Version),
                                 string.Format("WorkFlow Version Expected:{0}  but not found in Activity Libraries Table.", properties.Version));
            }

            if (!string.IsNullOrEmpty(properties.Status))
            {
               
                Assert.IsTrue(activityLibrariesTable.Select(l=>l.StatusName).ToList().Contains(properties.Status),
                                 string.Format("WorkFlow Status Expected:{0}  but not found in Activity Libraries Table.", properties.Status));
            }

            //if (properties.CreateDateTime != null)
            //{
            //    Assert.IsTrue(activityLibrariesTable.CreationDateTime.Equals(properties.CreateDateTime),
            //                     string.Format("WorkFlow CreateDateTime Expected:{0}  but not found in Activity Libraries Table.", properties.CreateDateTime));
            //}

            //if (properties.UpdateDateTime != null)
            //{
            //    Assert.IsTrue(activityLibrariesTable.UpdateDateTime.Equals(properties.UpdateDateTime),
            //                     string.Format("WorkFlow CreateDateTime Expected:{0}  but not found in Activity Libraries Table.", properties.UpdateDateTime));
            //}

            if (!string.IsNullOrEmpty(properties.Description))
            {
                Assert.IsTrue(activityLibrariesTable.Select(l=>l.Description).ToList().Contains(properties.Description),
                                 string.Format("WorkFlow Description Expected:{0}  but not found in Activity Libraries Table.", properties.Description));
            }
        }

        #region DataBase Actions
        /// <summary>
        /// Fill ExistingWorkflows collection with data
        /// </summary>
        private SortedDictionary<string, StoreActivitiesDC> StoreActivitesTable(IWorkflowsQueryService client, string activityName)
        {
            if (activityName == null || activityName.Trim() == "")
            {
                return null;
            }
            var collectedStoreActivitesTable = new SortedDictionary<string, StoreActivitiesDC>();
            var storeActivitesTable = new ObservableCollection<StoreActivitiesDC>(
                                 (from activity in
                                      client.StoreActivitiesGetByName(
                                          new StoreActivitiesDC
                                          {
                                              Name = activityName,
                                              Incaller = Assembly.GetExecutingAssembly().GetName().Name,
                                              IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString()
                                          })
                                  where !string.IsNullOrWhiteSpace(activity.Xaml)
                                  select activity));
            IList<StoreActivitiesDC> storeActivities = WorkflowsQueryServiceUtility.UsingClientReturn<IList<StoreActivitiesDC>>(c => c.StoreActivitiesGetByName(new StoreActivitiesDC
            {
                Name = activityName,
                Incaller = Environment.UserName,
                IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            }));


            var key = string.Format("{0}", storeActivities[0].Name.ToLower());
            collectedStoreActivitesTable[key] = storeActivities[0];
         
            return collectedStoreActivitesTable;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        private SortedDictionary<string, ActivityAssemblyItem> ActivityLibrariesTable(IWorkflowsQueryService client)
        {
            var activityLibrariesTable = new SortedDictionary<string, ActivityAssemblyItem>();
            // Load from database
            GetAllActivityLibrariesReplyDC replyDC;

            GetAllActivityLibrariesRequestDC dc = new GetAllActivityLibrariesRequestDC
            {
                Incaller = Environment.UserName,
                IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            };

            // Get the list of libraries that are on the server            
            replyDC = client.GetAllActivityLibraries(dc);
         

            // Require a pre-check since the list is not reliably instantiated 
            // by the WCF service
            if (null != replyDC && 0 == replyDC.Errorcode && null != replyDC.List)
            {
                // Each library which isn't already locally cached
                // should show up on the Select screen as a library
                // available on the server
                foreach (var dbAsm in replyDC.List)
                {
                    var key = string.Format("{0}", dbAsm.Name.ToLower());
                    activityLibrariesTable[key] = DataContractTranslator.ActivityLibraryDCToActivityAssemblyItem(dbAsm);
                }
            }
            return activityLibrariesTable;
        }

        private string ConvertStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 1000:
                    return "Private";
                case 1010:
                    return "Public";
                case 1020:
                    return "Retired";
                default:
                    return null;
            }
        }

        #endregion
    }
}
