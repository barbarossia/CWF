using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.DataAccess;

namespace Authoring.Tests.Functional
{
    [TestClass]
    public class ManageWorkflowTypeViewModelTest
    {
        private const string TESTOWNER = "v-allhe";
        private TestContext testContextInstance;

        private string UniqueWorkflowDisplayName
        {
            get
            {
                return "TestAutomation" + DateTime.Now.ToString("mmddyyyyhhMMss");
            }
        }

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [WorkItem(349418)]
        [Description("Verify admin can serach for workflow type what they want")]
        [Owner("v-allhe")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void VerifyAdminCanSearchWorkFlowType()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            ManageWorkflowTypeViewModel mv = new ManageWorkflowTypeViewModel();
            mv.LoadData();
            Assert.IsTrue(mv.WorkflowTypes.Count > 1);
            mv.SearchText = "Workflow";
            mv.SearchWorkflowTypeCommand.Execute();
            Assert.AreEqual(1, mv.WorkflowTypes.Count);
            Assert.AreEqual("Workflow", mv.WorkflowTypes.FirstOrDefault().Name);
        }

        [WorkItem(349446)]
        [Description("Verify admin can edit information of workflow type")]
        [Owner("v-allhe")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void VerifyAdminCanEditWorkFlowType()
        {
            string oldtestname = TestUtilities.GenerateRandomString(15);
            string newtestname = TestUtilities.GenerateRandomString(10);
            ManageWorkflowTypeViewModel mv = new ManageWorkflowTypeViewModel();
            mv.SearchText = oldtestname;
            mv.SearchWorkflowTypeCommand.Execute();
            Assert.AreEqual(0, mv.WorkflowTypes.Count);
            mv.AddWorkTypeflowTypeCommand.Execute();
            mv.EditWorkflowTypeVM.Name = oldtestname;
            mv.EditWorkflowTypeVM.SelectedAuthGroup = mv.EditWorkflowTypeVM.AuthGroups.FirstOrDefault();
            SelectWorkflowViewModel viewModel = new SelectWorkflowViewModel("dev");
            viewModel.LoadData();
            mv.EditWorkflowTypeVM.PublishingWorkflowId = viewModel.Activities.FirstOrDefault().Id;
            mv.EditWorkflowTypeVM.TemplateId = viewModel.Activities.FirstOrDefault().Id;

            mv.UploadWorkflowCommand.Execute();
            mv.SearchText = oldtestname;
            mv.SearchWorkflowTypeCommand.Execute();
            Assert.AreEqual(1, mv.WorkflowTypes.Count);
            Assert.AreEqual(oldtestname, mv.WorkflowTypes.FirstOrDefault().Name);
            mv.SelectedWorkflowType = mv.WorkflowTypes.FirstOrDefault();
            mv.EditWorkflowTypeCommand.Execute();
            mv.EditWorkflowTypeVM.Name = newtestname;
            mv.EditWorkflowTypeVM.PublishingWorkflowId = viewModel.Activities.LastOrDefault().Id;
            mv.EditWorkflowTypeVM.TemplateId = viewModel.Activities.LastOrDefault().Id;
            int newPublishingWorkflowId = viewModel.Activities.LastOrDefault().Id;
            mv.UploadWorkflowCommand.Execute();

            mv.SearchText = oldtestname;
            mv.SearchWorkflowTypeCommand.Execute();
            Assert.AreEqual(0, mv.WorkflowTypes.Count);
            mv.SearchText = newtestname;
            mv.SearchWorkflowTypeCommand.Execute();
            Assert.AreEqual(1, mv.WorkflowTypes.Count);
            Assert.AreEqual(newPublishingWorkflowId, mv.WorkflowTypes.FirstOrDefault().PublishingWorkflowId);
            Assert.AreEqual(newPublishingWorkflowId, mv.WorkflowTypes.FirstOrDefault().WorkflowTemplateId);
        }


        [WorkItem(349441)]
        [Description("Verify admin can delete workflow type")]
        [Owner("v-allhe")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void VerifyAdminCanDeleteWorkFlowType()
        {
            string testname = TestUtilities.GenerateRandomString(10);
            ManageWorkflowTypeViewModel mv = new ManageWorkflowTypeViewModel();
            mv.SearchText = testname;
            mv.SearchWorkflowTypeCommand.Execute();
            Assert.AreEqual(0, mv.WorkflowTypes.Count);
            mv.AddWorkTypeflowTypeCommand.Execute();
            mv.EditWorkflowTypeVM.Name = testname;
            mv.EditWorkflowTypeVM.SelectedAuthGroup = mv.EditWorkflowTypeVM.AuthGroups.FirstOrDefault();
            SelectWorkflowViewModel viewModel = new SelectWorkflowViewModel("dev");
            viewModel.LoadData();
            mv.EditWorkflowTypeVM.PublishingWorkflowId = viewModel.Activities.FirstOrDefault().Id;
            mv.EditWorkflowTypeVM.TemplateId = viewModel.Activities.FirstOrDefault().Id;

            mv.UploadWorkflowCommand.Execute();
            mv.SearchText = testname;
            mv.SearchWorkflowTypeCommand.Execute();
            Assert.AreEqual(1, mv.WorkflowTypes.Count);
            Assert.AreEqual(testname, mv.WorkflowTypes.FirstOrDefault().Name);
            mv.SelectedWorkflowType = mv.WorkflowTypes.FirstOrDefault();
            mv.DeleteWorkflowTypeCommand.Execute();
            mv.SearchText = testname;
            mv.SearchWorkflowTypeCommand.Execute();
            Assert.AreEqual(0, mv.WorkflowTypes.Count);
        }

        [WorkItem(349449)]
        [Description("Verify admin can add a new workflow type")]
        [Owner("v-allhe")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void VerifyAdminCanAddWorkFlowType()
        {
           string testname= TestUtilities.GenerateRandomString(10);
            ManageWorkflowTypeViewModel mv = new ManageWorkflowTypeViewModel();
            mv.SearchText = testname;
            mv.SearchWorkflowTypeCommand.Execute();
            Assert.AreEqual(0, mv.WorkflowTypes.Count);
            mv.AddWorkTypeflowTypeCommand.Execute();
            mv.EditWorkflowTypeVM.Name = testname;
            mv.EditWorkflowTypeVM.SelectedAuthGroup = mv.EditWorkflowTypeVM.AuthGroups.FirstOrDefault();
            SelectWorkflowViewModel viewModel = new SelectWorkflowViewModel("dev");
            viewModel.LoadData();
            mv.EditWorkflowTypeVM.PublishingWorkflowId = viewModel.Activities.FirstOrDefault().Id;
            mv.EditWorkflowTypeVM.TemplateId = viewModel.Activities.FirstOrDefault().Id;

            mv.UploadWorkflowCommand.Execute();
            mv.SearchText = testname;
            mv.SearchWorkflowTypeCommand.Execute();
            Assert.AreEqual(1, mv.WorkflowTypes.Count);
            Assert.AreEqual(testname, mv.WorkflowTypes.FirstOrDefault().Name);
        }

        [WorkItem(349448)]
        [Description("Verify admin add a new workflow type with not name,authorize,and template")]
        [Owner("v-allhe")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void VerifyAdminAddWorkFlowTypeWithNull()
        {
            string testname = TestUtilities.GenerateRandomString(10);
            ManageWorkflowTypeViewModel mv = new ManageWorkflowTypeViewModel();
            mv.SearchText = testname;
            mv.SearchWorkflowTypeCommand.Execute();
            Assert.AreEqual(0, mv.WorkflowTypes.Count);
            mv.AddWorkTypeflowTypeCommand.Execute();
            try
            {
                mv.UploadWorkflowCommand.Execute();
            }
            catch (UserFacingException e)
            {
                mv.SearchText = testname;
                mv.SearchWorkflowTypeCommand.Execute();
                Assert.AreEqual(0, mv.WorkflowTypes.Count);
            }

        }

        [WorkItem(349455)]
        [Description("Verify admin add a new workflow type with same name")]
        [Owner("v-allhe")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void VerifyAdminAddWorkFlowTypeWithSameName()
        {
            string testname = TestUtilities.GenerateRandomString(10);
            ManageWorkflowTypeViewModel mv = new ManageWorkflowTypeViewModel();
            mv.SearchText = testname;
            mv.SearchWorkflowTypeCommand.Execute();
            Assert.AreEqual(0, mv.WorkflowTypes.Count);
            mv.AddWorkTypeflowTypeCommand.Execute();
            mv.EditWorkflowTypeVM.Name = testname;
            mv.EditWorkflowTypeVM.SelectedAuthGroup = mv.EditWorkflowTypeVM.AuthGroups.FirstOrDefault();
            SelectWorkflowViewModel viewModel = new SelectWorkflowViewModel("dev");
            viewModel.LoadData();
            mv.EditWorkflowTypeVM.PublishingWorkflowId = viewModel.Activities.FirstOrDefault().Id;
            mv.EditWorkflowTypeVM.TemplateId = viewModel.Activities.FirstOrDefault().Id;

            mv.UploadWorkflowCommand.Execute();
            mv.SearchText = testname;
            mv.SearchWorkflowTypeCommand.Execute();
            Assert.AreEqual(1, mv.WorkflowTypes.Count);
            Assert.AreEqual(testname, mv.WorkflowTypes.FirstOrDefault().Name);

            mv.AddWorkTypeflowTypeCommand.Execute();
            mv.EditWorkflowTypeVM.Name = testname;
            mv.EditWorkflowTypeVM.SelectedAuthGroup = mv.EditWorkflowTypeVM.AuthGroups.FirstOrDefault();
            viewModel.LoadData();
            mv.EditWorkflowTypeVM.PublishingWorkflowId = viewModel.Activities.FirstOrDefault().Id;
            mv.EditWorkflowTypeVM.TemplateId = viewModel.Activities.FirstOrDefault().Id;
            try
            {
                mv.UploadWorkflowCommand.Execute();
            }
            catch (UserFacingException e)
            {
                mv.SearchText = testname;
                mv.SearchWorkflowTypeCommand.Execute();
                Assert.AreEqual(1, mv.WorkflowTypes.Count);
            }

        }

        [WorkItem(349454)]
        [Description("Verify admin can search for workflow what they want on browse workflows window")]
        [Owner("v-allhe")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void VerifyAdminCanSearchWorkFlowInWorkflowsWindow()
        {
            var workFlowItem = TestDataUtility.CreateWorkFlowItemTestData();
            SelectWorkflowViewModel viewModel = new SelectWorkflowViewModel("dev");
            viewModel.LoadData();
            viewModel.SearchFilter = workFlowItem;
            viewModel.SearchCommand.Execute();
            Assert.AreEqual(1, viewModel.Activities.Count);
            Assert.AreEqual(workFlowItem, viewModel.Activities.FirstOrDefault().Name);
        }

    }
}
