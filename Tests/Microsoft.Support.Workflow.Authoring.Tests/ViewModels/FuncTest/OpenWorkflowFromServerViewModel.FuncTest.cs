using System;
using System.Activities.Statements;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using ActivityLibrary1;
using AuthoringToolTests.Services;
using CWF.DataContracts;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Security;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.DataAccess;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using Testinput_Lib1;
using TestInput_Lib2;
using Testinput_Lib3;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using System.Configuration;

namespace Authoring.Tests.Functional
{
    [TestClass]
    public class OpenWorkflowFromServerViewModelFunctionalTest
    {

        string projectLockedByOther = "The workflow was locked by {0}. You will open it in read-only mode.";
        bool isPopuped = false;

        [Description("Needed to use the MainWindowViewModel class")]
        [ClassInitialize]
        public static void TestClassSetup(TestContext context)
        {
            TestUtilities.LoadSystemActivitesIntoCurrentAppDomain();
            //CreateWorkFlow();
        }

        [ClassCleanup]
        public static void TestClassEnd()
        {

        }

        //[TestInitialize]
        //public  void TestSetUp()
        //{ }

        //[TestCleanup]
        //public  void TestEnd()
        //{

        //}

        [WorkItem(28027)]
        [Description("Verify opening a workflow from the server downloads all needed dependencies")]
        [Owner("v-billmi")]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyOpenFromServerWithDependenciesDownloaded()
        {
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.RegistLoginUserRole(Role.Author);
                TestUtilities.RegistCreateIntellisenseList();

                MainWindowViewModel mw = new MainWindowViewModel();
                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();
                // Get collection from StoreActivities table
                StoreActivitiesDC workflowToOpen =
                TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsNotNull(mw);
                mw.WorkflowItems.FirstOrDefault().Close();

            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);
            }

        }

        [WorkItem(28026)]
        [Description("Verify all workflows in the database are opened")]
        [Owner("v-billmi")]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyOpenFromServer()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                Utility.Resolve(args.Name);
            TestUtilities.RegistCreateIntellisenseList();
            using (new CachingIsolator())
            {
                int workflowCount = 0;
                int timeOut = 10;
                int templateIds = 3;

                ObservableCollection<StoreActivitiesDC> storeActivitiesDC = TestUtilities.GetCollectionFromStoreActivitiesTable();
                MainWindowViewModel mw = new MainWindowViewModel();

                // Want to break out of loop if there are a large number of workflows in the DB
                DateTime dtTimeOut = DateTime.Now.AddSeconds(timeOut);
                foreach (StoreActivitiesDC saDC in storeActivitiesDC)
                {
                    Console.WriteLine(string.Format("activity name: {0} : id: {1}", saDC.Name, saDC.ActivityLibraryId));

                    // The first 3 Ids are the templates and don't want to mess with those during the test
                    if (saDC.Id > templateIds && saDC.Xaml != null)
                    {
                        workflowCount++;

                        mw = TestUtilities.OpenWorkflowFromServer(saDC, mw);
                    }

                    if (dtTimeOut.Ticks < DateTime.Now.Ticks)
                    {
                        break;
                    }
                }

                Assert.AreEqual(workflowCount, mw.WorkflowItems.Count);
                //mw.WorkflowItems.FirstOrDefault().Close();
            }
        }

        [WorkItem(16137)]
        [Description("Test the Open Workflow function")]
        [Owner("v-ertang")]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyOpenWorkflow()
        {
            try
            {
                // Tests open workflow from server
                var vmOpenWorkflow = new OpenWorkflowFromServerViewModel();
                vmOpenWorkflow.EnvFilters.Where(e => e.Env == Env.Dev).FirstOrDefault().IsFilted = true;
                // loads data from the database
                WorkflowsQueryServiceUtility.UsingClient(client =>
                    vmOpenWorkflow.LoadLiveData(client));
                ObservableCollection<StoreActivitiesDC> storeActivitiesDC = vmOpenWorkflow.ExistingWorkflows; // workflows from the database
                Assert.AreNotEqual(0, storeActivitiesDC.Count, TestUtilities.ListOfTemplateErrorMessage);
                // select the first workflow in the list and then open it
                vmOpenWorkflow.SelectedWorkflow = storeActivitiesDC[0];
                vmOpenWorkflow.OpenSelectedWorkflowCommand.Execute();
            }
            catch (Exception ex)
            {
                Assert.Fail("Open workflow failed with this exception: {0}", ex.Message);
            }
        }

        [WorkItem(282246)]
        [Description("Save the modification for an older activity by an administrator.")]
        [Owner("v-jerzha")]
        [TestCategory("Func-NoDif3-Full")]
        [TestMethod()]
        public void SaveOlderActivityWithUpdate()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            string createdWorkflowItemName = string.Empty;
            try
            {

                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.RegistLoginUserRole(Role.Author);
                TestUtilities.RegistCreateIntellisenseList();

                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();
                MainWindowViewModel mw = new MainWindowViewModel();
                TestUtilities.VerifyWorkflowNameAndVersionInDB("1.0.0.0", createdWorkflowItemName);

                //open expect workflow and save it again to increase version number
                StoreActivitiesDC workflowToOpen =
                    TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).Where(sa => sa.Version == "1.0.0.0").FirstOrDefault();

                Assert.IsNotNull(workflowToOpen, "Did not find workflowToOpen in Store Activities table.");

                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                mw.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());
                Assert.IsTrue(mw.FocusedWorkflowItem.IsSavedToServer, "IsSavedToServer should be true");
                TestUtilities.VerifyWorkflowNameAndVersionInDB("1.0.0.1", createdWorkflowItemName);

                //open previous version workflow
                StoreActivitiesDC workflowToOpenPre =
                    TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).Where(sa => sa.Version == "1.0.0.0").FirstOrDefault();
                Assert.IsNotNull(workflowToOpenPre, "Did not find Previous Version Work Flow in Store Activities table.");

                mw.CloseFocusedWorkflowCommand.Execute();
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpenPre, mw);
                string newXAML = TestUtilities.GenerateASequenceXmalCodeWithoutActivity(); ;
                mw.FocusedWorkflowItem.XamlCode = newXAML;
                mw.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());

                //check work flow version
                TestUtilities.VerifyWorkflowNameAndVersionInDB("1.0.0.2", createdWorkflowItemName);
                mw.WorkflowItems.FirstOrDefault().Close();

            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);
            }
        }

        [WorkItem(282301)]
        [Description("Open a project locked by another admin from server.")]
        [Owner("v-jerzha")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void OpenLockedProject()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.RegistLoginUserRole(Role.Author);
                TestUtilities.RegistCreateIntellisenseList();

                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();
                MainWindowViewModel mw = new MainWindowViewModel();
                //Open one project from server
                StoreActivitiesDC workflowToOpen =
                    TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();
                Assert.IsNotNull(workflowToOpen, "Did not find workflowToOpen in Store Activities table.");
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsTrue(workflowToOpen.Locked);
                mw.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());

                string temp = string.Format(projectLockedByOther, Environment.UserName);
                TestUtilities.RegistLoginUserRole(Role.Admin);
                StoreActivitiesDC workflowToOpen2 =
                    TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();
                MainWindowViewModel mw2 = new MainWindowViewModel();
                mw2 = TestUtilities.OpenWorkflowFromServer(workflowToOpen2, mw2);
                mw.WorkflowItems.FirstOrDefault().Close();
                mw2.WorkflowItems.FirstOrDefault().Close();
            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);
            }
        }

        [WorkItem(282304)]
        [Description("Save the modification for an older activity by the author who is the same registered user that created the latest version.")]
        [Owner("v-jerzha")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void SaveModificationActivityBySameUser()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            TestUtilities.RegistCreateIntellisenseList();
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.RegistLoginUserRole(Role.Author);
                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();
                MainWindowViewModel mw = new MainWindowViewModel();
                //open expect workflow and save it
                StoreActivitiesDC workflowToOpen01 =
                    TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();
                MainWindowViewModel mw01 = new MainWindowViewModel();
                mw01 = TestUtilities.OpenWorkflowFromServer(workflowToOpen01, mw01);
                mw01.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());

                //open previous workflow version and save it
                StoreActivitiesDC workflowToOpen =
                    TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault(ac => ac.Version == "1.0.0.0");

                Assert.IsNotNull(workflowToOpen, "Did not find workflowToOpen in Store Activities table.");
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);

                mw.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());
                Assert.IsTrue(mw.FocusedWorkflowItem.IsSavedToServer, "IsSavedToServer should be true");
                TestUtilities.VerifyWorkflowNameAndVersionInDB("1.0.0.0", createdWorkflowItemName);
                mw.WorkflowItems.FirstOrDefault().Close();
                mw01.WorkflowItems.FirstOrDefault().Close();
            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);
            }
        }

        [WorkItem(282302)]
        [Description("Save the modification for an older activity by the author who is different person that the one has the record locked in the server.")]
        [Owner("v-jerzha")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void SaveModificationActivityByDiffUser()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();

            string createdWorkflowItemName = string.Empty;
            try
            {
                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.RegistCreateIntellisenseList();

                string temp = string.Format(projectLockedByOther, Environment.UserName);
                StoreActivitiesDC workflowToOpen =
                    TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();

                TestUtilities.RegistLoginUserRole(Role.Author);
                MainWindowViewModel mw01 = new MainWindowViewModel();
                mw01 = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw01);
                mw01.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());
                //Check the project status
                Assert.IsTrue(workflowToOpen.Locked);

                TestUtilities.RegistLoginUserRole(Role.Admin);

                StoreActivitiesDC workflowToOpen02 =
                    TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();

                //open same project from server
                MainWindowViewModel mw02 = new MainWindowViewModel();
                mw02 = TestUtilities.OpenWorkflowFromServer(workflowToOpen02, mw02);
                mw02.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());

                Assert.IsTrue(mw02.FocusedWorkflowItem.IsSavedToServer, "IsSavedToServer should be true");
                mw01.WorkflowItems.FirstOrDefault().Close();
                mw02.WorkflowItems.FirstOrDefault().Close();
            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);
            }

        }

        [WorkItem(282300)]
        [Description("Check out project which is not locked by anyone from server.")]
        [Owner("v-jerzha")]
        [TestCategory("Func-NoDif2-Smoke")]
        [TestMethod()]
        public void CheckOutNotLockedProject()
        {
            string createdWorkflowItemName = string.Empty;
            TestUtilities.RegistCreateIntellisenseList();

            try
            {
                //using (var helper = new ImplementationOfType(typeof(ExpressionEditorHelper)))
                //{
                //    helper.Register(() => ExpressionEditorHelper.CreateIntellisenseList()).Return(null);

                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();

                TestUtilities.RegistMessageBoxServiceOfCommonOperate();

                TestUtilities.RegistLoginUserRole(Role.Author);

                MainWindowViewModel mw = new MainWindowViewModel();
                StoreActivitiesDC workflowOpen = TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();
                //Microsoft.Support.Workflow.Authoring.AddIns.AddinPreloader.Init();
                mw = TestUtilities.OpenWorkflowFromServer(workflowOpen, mw);
                mw.WorkflowItems.FirstOrDefault().Close();
                // Check the workflow has been locked.
                Assert.IsTrue(workflowOpen.Locked);
                mw.WorkflowItems.FirstOrDefault().Close();
                //}
            }
            catch (Exception ex)
            {
                Assert.Fail("Open workflow failed with this exception: {0}", ex.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);
            }
        }

        [WorkItem(282298)]
        [Description("Check out project which is not locked by anyone from server.")]
        [Owner("v-jerzha")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void CheckOutLockedProjectByCurrUser()
        {
            // Microsoft.Support.Workflow.Authoring.AddIns.AddinPreloader.Current.IsEnabled = false;
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.RegistLoginUserRole(Role.Author);
                TestUtilities.RegistCreateIntellisenseList();

                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();
                StoreActivitiesDC workflowOpen = TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();
                MainWindowViewModel mw = new MainWindowViewModel();

                try
                {
                    mw = TestUtilities.OpenWorkflowFromServer(workflowOpen, mw);
                }
                catch (Exception ex)
                {
                    string str = ex.Message;
                }
                // Check the workflow has been locked.
                Assert.IsTrue(workflowOpen.Locked);
                mw.WorkflowItems.FirstOrDefault().Close();
            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);
            }
        }

        [WorkItem(282299)]
        [Description("Check out project locked by other user from server (Current user is not Admin)")]
        [Owner("v-jerzha")]
        [TestCategory("Func-Dif-Full")]
        [TestMethod()]
        public void CheckOutLockedProjectByOtherUser()
        {
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.RegistLoginUserRole(Role.Author);
                TestUtilities.RegistCreateIntellisenseList();

                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();

                StoreActivitiesDC workflowOpen = TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();
                string temp;
                //first user open the project
                using (Implementation impl = new Implementation())
                {
                    MainWindowViewModel mw = new MainWindowViewModel();
                    impl.Register(typeof(Environment), "get_UserName").Return("v-toy");
                    TestUtilities.OpenWorkflowFromServer(workflowOpen, mw);
                    temp = string.Format(projectLockedByOther, Environment.UserName);
                }

                MainWindowViewModel mw2 = new MainWindowViewModel();
                mw2 = TestUtilities.OpenWorkflowFromServer(workflowOpen, mw2);
                Assert.IsTrue(workflowOpen.Locked);
                //mw2.WorkflowItems.FirstOrDefault().Close();
            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);
            }

        }

        [WorkItem(282297)]
        [Description("Check out project locked by other user from server (Current user is an Admin)")]
        [Owner("v-jerzha")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void CheckOutLockedProjectByAdmin()
        {
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistCreateIntellisenseList();
                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();

                MainWindowViewModel viewModel = new MainWindowViewModel();
                StoreActivitiesDC workflowToOpen =
                TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();

                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                //Open project by someone:not admin
                viewModel = TestUtilities.OpenWorkflowFromServer(workflowToOpen, viewModel);
                Assert.IsTrue(workflowToOpen.Locked);

                //open workflow by admin
                TestUtilities.RegistLoginUserRole(Role.Admin);
                MainWindowViewModel mw = new MainWindowViewModel();

                //open workflow for edit mode
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsFalse(mw.FocusedWorkflowItem.IsReadOnly);

                //open workflow for readonly mode
                string message;
                MessageBoxService.ShowOpenActivityConfirmationFunc = (msg, caption) =>
                {
                    message = msg;
                    return MessageBoxResult.No;
                };
                mw.CloseFocusedWorkflowCommand.Execute();
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsTrue(mw.FocusedWorkflowItem.IsReadOnly);

                //cancel open workflow
                MessageBoxService.ShowOpenActivityConfirmationFunc = (msg, caption) =>
                {
                    message = msg;
                    return MessageBoxResult.Cancel;
                };
                mw.CloseFocusedWorkflowCommand.Execute();
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.AreEqual(0, mw.WorkflowItems.Count);
                if (mw.WorkflowItems.Count > 0)
                    mw.WorkflowItems.FirstOrDefault().Close();
                viewModel.WorkflowItems.FirstOrDefault().Close();
            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);
            }
        }


        [WorkItem(282296)]
        [Description("Check in new project or project saved on local disk")]
        [Owner("v-jerzha")]
        [TestCategory("Func-Dif-Full")]
        [TestMethod()]
        public void CheckInProjectOnLocal()
        {
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistUtilityGetCurrentWindow();
                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();
                TestUtilities.RegistLoginUserRole(Role.Author);
                TestUtilities.RegistCreateIntellisenseList();

                TestUtilities.MockMessageBoxServiceOfCommonOperate(() =>
                {
                    MainWindowViewModel mw = new MainWindowViewModel();
                    TestUtilities.MockDialogService(createdWorkflowItemName, () =>
                    {
                        StoreActivitiesDC workflowToOpen =
          TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();
                        mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                        mw.SaveFocusedWorkflowCommand.Execute(Locations.ToLocal.ToString());
                        mw.CloseFocusedWorkflowCommand.Execute();
                        //System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); 
                        TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);

                        MainWindowViewModel_Accessor mw2 = new MainWindowViewModel_Accessor();
                        //mw.OpenWorkflowCommand.Execute(Locations.FromLocal.ToString());
                        mw2.OpenWorkflowFromLocal(createdWorkflowItemName + ".wf");
                        mw2.WorkflowItems[0].XamlCode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
                        mw2.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());
                        Assert.IsTrue(mw2.FocusedWorkflowItem.IsSavedToServer);
                        mw2.WorkflowItems.FirstOrDefault().Close();
                    });
                });

            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);
            }
        }

        [WorkItem(282294)]
        [Description("Check in project locked by current user to server.")]
        [Owner("v-jerzha")]
        [TestCategory("Func-Dif-Full")]
        [TestMethod()]
        public void CheckInProjectLockedByCurrentUser()
        {
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.Author);
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.RegistCreateIntellisenseList();

                MainWindowViewModel mw = new MainWindowViewModel();
                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();

                StoreActivitiesDC workflowToOpen =
                                        TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);

                //Open project on other machine
                using (Implementation impl = new Implementation())
                {
                    impl.Register(typeof(Environment), "get_MachineName").Return("Test1");
                    MainWindowViewModel mw2 = new MainWindowViewModel();
                    mw2 = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw2);

                    Assert.IsTrue(workflowToOpen.Locked);
                    mw2.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());
                }

                //Try to save same project 
                mw.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());
                Assert.IsTrue(mw.FocusedWorkflowItem.IsSavedToServer);
                mw.WorkflowItems.FirstOrDefault().Close();
            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);
            }
        }

        [WorkItem(282295)]
        [Description("Check in project locked by other user to server (current user is an Admin)")]
        [Owner("v-jerzha")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void CheckInProjectLockedByOther()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            string createWorkFlowItemName = string.Empty;
            try
            {
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.RegistLoginUserRole(Role.Author);
                TestUtilities.RegistCreateIntellisenseList();

                MainWindowViewModel mw = new MainWindowViewModel();
                createWorkFlowItemName = TestDataUtility.CreateWorkFlowItemTestData();
                StoreActivitiesDC workflowToOpen =
                                           TestUtilities.GetStoreActivitiesFromeServerByName(createWorkFlowItemName).FirstOrDefault();

                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsTrue(workflowToOpen.Locked);

                TestUtilities.RegistLoginUserRole(Role.Admin);
                MainWindowViewModel mw2 = new MainWindowViewModel();
                mw2 = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw2);

                Assert.IsTrue(workflowToOpen.Locked);
                mw2.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());
                Assert.IsTrue(mw2.FocusedWorkflowItem.IsSavedToServer, "SavedToServer should be true");
                mw.WorkflowItems.FirstOrDefault().Close();
                mw2.WorkflowItems.FirstOrDefault().Close();
            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createWorkFlowItemName);
            }
        }

        [WorkItem(282293)]
        [Description("Check in project locked by current user to server.")]
        [Owner("v-jerzha")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void CheckInProjectLockedByAdmin()
        {
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.RegistCreateIntellisenseList();

                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();
                StoreActivitiesDC workflowToOpen =
                        TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();


                TestUtilities.RegistLoginUserRole(Role.Admin);
                MainWindowViewModel mw = new MainWindowViewModel();
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsTrue(workflowToOpen.Locked);

                TestUtilities.RegistLoginUserRole(Role.Author);
                MainWindowViewModel mw2 = new MainWindowViewModel();
                mw2 = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw2);

                Assert.IsTrue(workflowToOpen.Locked);

                // mw2.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());
                // Assert.IsTrue(mw2.FocusedWorkflowItem.IsSavedToServer, "SavedToServer should be true");
                mw.WorkflowItems.FirstOrDefault().Close();
                mw2.WorkflowItems.FirstOrDefault().Close();
            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);
            }
        }

        [WorkItem(283537)]
        [Description("Clear the lock after user close workflow in AT.")]
        [Owner("v-jerzha")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void UnlockedClosedProject()
        {
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.Author);
                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();

                MainWindowViewModel mw = new MainWindowViewModel();
                StoreActivitiesDC workflowToOpen =
                   TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();


                TestUtilities.RegistMessageBoxServiceOfCommonOperate();

                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsTrue(workflowToOpen.Locked);

                //unlocked when the project closed
                mw.CloseFocusedWorkflowCommandExecute();

                workflowToOpen =
                   TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();
                Assert.IsTrue(workflowToOpen.Locked);

                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsTrue(workflowToOpen.Locked);

                string message;
                MessageBoxService.ShowSavingConfirmationFunc = ((msg, caption, canKeepLocked, shouldUnlock, unlockVisibility) =>
                {
                    message = msg;
                    return SavingResult.Unlock;
                });

                //unlocked when the project closed
                mw.CloseFocusedWorkflowCommandExecute();
                workflowToOpen =
                   TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();
                Assert.IsFalse(workflowToOpen.Locked);

            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);
            }
        }


        [WorkItem(405151)]
        [Description("verify the user:viewer/env admin/cwf admin only can open workflow in readonly model")]
        [Owner("v-allhe")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void VerifyOpenWorkflowOnlyHaveReadonlyModel()
        {
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.Viewer);
                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();
                MainWindowViewModel mw = new MainWindowViewModel();
                StoreActivitiesDC workflowToOpen =
                   TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();

                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsTrue(mw.WorkflowItems.FirstOrDefault().IsReadOnly);
                mw.CloseFocusedWorkflowCommandExecute();

                TestUtilities.RegistLoginUserRole(Role.CWFEnvAdmin);
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsTrue(mw.WorkflowItems.FirstOrDefault().IsReadOnly);
                mw.CloseFocusedWorkflowCommandExecute();

                TestUtilities.RegistLoginUserRole(Role.CWFAdmin);
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsTrue(mw.WorkflowItems.FirstOrDefault().IsReadOnly);
                mw.CloseFocusedWorkflowCommandExecute();
            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);
            }
        }

        [WorkItem(405152)]
        [Description("verify the user:author/admin can open workflow with two model readonly model/edit in Dev/Test")]
        [Owner("v-allhe")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void VerifyOpenWorkflowHaveTwoModel()
        {
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.Author);
                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();
                MainWindowViewModel mw = new MainWindowViewModel();
                StoreActivitiesDC workflowToOpen =
                   TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();

                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsFalse(mw.WorkflowItems.FirstOrDefault().IsReadOnly);
                mw.CloseFocusedWorkflowCommandExecute();

                string message = string.Empty;
                MessageBoxService.ShowOpenActivityConfirmationFunc = (msg, caption) =>
                {
                    message = msg;
                    return MessageBoxResult.No;
                };
                TestUtilities.RegistLoginUserRole(Role.Admin);
                MainWindowViewModel mwvm = new MainWindowViewModel();
                TestUtilities.OpenWorkflowFromServer(workflowToOpen, mwvm);
                Assert.IsTrue(mwvm.WorkflowItems.FirstOrDefault().IsReadOnly);
                mwvm.CloseFocusedWorkflowCommandExecute();

            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(createdWorkflowItemName);
            }
        }


    }
}
