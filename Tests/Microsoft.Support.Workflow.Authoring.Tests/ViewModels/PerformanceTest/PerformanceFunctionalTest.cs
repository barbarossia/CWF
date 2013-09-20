using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Security;
using Microsoft.Support.Workflow.Authoring.Tests.DataAccess;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using CWF.DataContracts;
using System.Reflection;
using System.Activities.Statements;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Build.Execution;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.PerformanceTest
{
    [TestClass]
    public class PerformanceFunctionalTest
    {
        [WorkItem(367327)]
        [Description("Open a project from server.")]
        [Owner("v-allhe")]
        [TestCategory("Func-Performance-Full")]
        [TestMethod()]
        public void OpenLockedProject()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.Author);
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
  
                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();
                MainWindowViewModel mw = new MainWindowViewModel();
                //Open one project from server
                StoreActivitiesDC workflowToOpen =
                    TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();

                Assert.IsNotNull(workflowToOpen, "Did not find workflowToOpen in Store Activities table.");
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsTrue(workflowToOpen.Locked);
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

        [WorkItem(367329)]
        [Description("Open a project from server.")]
        [Owner("v-allhe")]
        [TestCategory("Func-Performance-Full")]
        [TestMethod()]
        public void SaveProject()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.Author);
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();

                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();
                MainWindowViewModel mw = new MainWindowViewModel();
                //Open one project from server
                StoreActivitiesDC workflowToOpen =
                    TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();

                Assert.IsNotNull(workflowToOpen, "Did not find workflowToOpen in Store Activities table.");
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsTrue(workflowToOpen.Locked);
                mw.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());
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


        [WorkItem(367326)]
        [Description("Open a project from server.")]
        [Owner("v-allhe")]
        [TestCategory("Func-Performance-Full")]
        [TestMethod()]
        public void CompileProject()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.Author);
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();

                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();
                MainWindowViewModel mw = new MainWindowViewModel();
                //Open one project from server
                StoreActivitiesDC workflowToOpen =
                    TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();

                Assert.IsNotNull(workflowToOpen, "Did not find workflowToOpen in Store Activities table.");
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsTrue(workflowToOpen.Locked);

                CompileProject compileProject = mw.FocusedWorkflowItem.WorkflowDesigner.CompileProject;
                compileProject.ProjectName = mw.FocusedWorkflowItem.Name;
                compileProject.ProjectVersion = mw.FocusedWorkflowItem.Version;
                compileProject.ProjectXaml = mw.FocusedWorkflowItem.WorkflowDesigner.CompiledXaml;

                CompileResult result = Compiler.Compile(compileProject);
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

        [WorkItem(367328)]
        [Description("Open a project from server.")]
        [Owner("v-allhe")]
        [TestCategory("Func-Performance-Full")]
        [TestMethod()]
        public void PublishProject()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            string createdWorkflowItemName = string.Empty;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.Author);
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();

                createdWorkflowItemName = TestDataUtility.CreateWorkFlowItemTestData();
                MainWindowViewModel mw = new MainWindowViewModel();
                //Open one project from server
                StoreActivitiesDC workflowToOpen =
                    TestUtilities.GetStoreActivitiesFromeServerByName(createdWorkflowItemName).FirstOrDefault();

                Assert.IsNotNull(workflowToOpen, "Did not find workflowToOpen in Store Activities table.");
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
                Assert.IsTrue(workflowToOpen.Locked);
                mw.FocusedWorkflowItem.Tags = "Main\\2.07\\2.07.7.0";
                mw.FocusedWorkflowItem.Status = MarketplaceStatus.Public.ToString();
                mw.FocusedWorkflowItem.XamlCode = mw.FocusedWorkflowItem.WorkflowDesigner.CompiledXaml;

                // Stop publishing if saving failed.
                //if (!WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient(client => UploadWorkflow(client, this.FocusedWorkflowItem, shouldBeLoose: false)))
                //   return;
                WorkflowsQueryServiceUtility.UsingClient(client => mw.PublishCommandExecute_Implementation(client, mw.FocusedWorkflowItem));
                mw.FocusedWorkflowItem.Close();
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

        [WorkItem(367323)]
        [Description("Open a project from server.")]
        [Owner("v-allhe")]
        [TestCategory("Func-Performance-Full")]
        [TestMethod()]
        [Ignore]
        public void OpenAndSaveSmallProjectTimeCount()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            string createdWorkflowItemName = string.Empty;
            string smallWorkflow = "small";
            DateTime start;
            DateTime end;
            TimeSpan span;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.Author);
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                MainWindowViewModel mw = new MainWindowViewModel();
                //Open smallworkflow
                start = DateTime.Now;
                StoreActivitiesDC workflowToOpenSmall =
                    TestUtilities.GetStoreActivitiesFromeServerByName(smallWorkflow).FirstOrDefault();
                end = DateTime.Now;
                span = end - start;
                Console.WriteLine("get the smallworkflow data from QueryService time is:" + span.TotalSeconds + "s");

                Assert.IsNotNull(workflowToOpenSmall, "Did not find workflowToOpen in Store Activities table.");

                start = DateTime.Now;
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpenSmall, mw);
                end = DateTime.Now;
                span = end - start;
                Console.WriteLine("open the smallwprkflow data from QueryService time is:" + span.TotalSeconds + "s");

                start = DateTime.Now;
                mw.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());
                end = DateTime.Now;
                span = end - start;
                Console.WriteLine("save the smallworkflow data to QueryService time is:" + span.TotalSeconds + "s");
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

        [WorkItem(367324)]
        [Description("Open a project from server.")]
        [Owner("v-allhe")]
        [TestCategory("Func-Performance-Full")]
        [TestMethod()]
        [Ignore]
        public void OpenAndSaveMiddleProjectTimeCount()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            string createdWorkflowItemName = string.Empty;
            string middleWorkflow = "middle";
            DateTime start;
            DateTime end;
            TimeSpan span;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.Author);
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();

                MainWindowViewModel mw = new MainWindowViewModel();
                //Open middleworkflow
                start = DateTime.Now;
                StoreActivitiesDC workflowToOpenMiddle =
                    TestUtilities.GetStoreActivitiesFromeServerByName(middleWorkflow).FirstOrDefault();
                end = DateTime.Now;
                span = end - start;
                Console.WriteLine("get the middleworkflow data from QueryService time is:" + span.TotalSeconds + "s");

                Assert.IsNotNull(workflowToOpenMiddle, "Did not find workflowToOpen in Store Activities table.");

                start = DateTime.Now;
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpenMiddle, mw);
                end = DateTime.Now;
                span = end - start;
                Console.WriteLine("open the middleworkflow data from QueryService time is:" + span.TotalSeconds + "s");

                start = DateTime.Now;
                mw.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());
                end = DateTime.Now;
                span = end - start;
                Console.WriteLine("save the middleworkflow data to QueryService time is:" + span.TotalSeconds + "s");
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

        [WorkItem(367325)]
        [Description("Open a project from server.")]
        [Owner("v-allhe")]
        [TestCategory("Func-Performance-Full")]
        [TestMethod()]
        [Ignore]
        public void OpenAndSaveLargeProjectTimeCount()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            string createdWorkflowItemName = string.Empty;
            string largeWorkflow = "large";
            DateTime start;
            DateTime end;
            TimeSpan span;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.Author);
                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                MainWindowViewModel mw = new MainWindowViewModel();
                //Open largeworkflow
                start = DateTime.Now;
                StoreActivitiesDC workflowToOpenlarge =
                    TestUtilities.GetStoreActivitiesFromeServerByName(largeWorkflow).FirstOrDefault();
                end = DateTime.Now;
                span = end - start;
                Console.WriteLine("get the largeworkflow data from QueryService time is:" + span.TotalSeconds + "s");

                Assert.IsNotNull(workflowToOpenlarge, "Did not find workflowToOpen in Store Activities table.");

                start = DateTime.Now;
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpenlarge, mw);
                end = DateTime.Now;
                span = end - start;
                Console.WriteLine("open the largeworkflow data from QueryService time is:" + span.TotalSeconds + "s");

                // Assert.IsTrue(workflowToOpenlarge.Locked);
                start = DateTime.Now;
                mw.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());
                end = DateTime.Now;
                span = end - start;
                Console.WriteLine("save the largeworkflow data to QueryService time is:" + span.TotalSeconds + "s");
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

    }
}
