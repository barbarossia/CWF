using System;
using System.Activities.Presentation;
using System.Activities.Presentation.Services;
using System.Activities.Statements;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CWF.DataContracts;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Services;

namespace Authoring.Tests.Unit
{
    /// <summary>
    /// Summary description for NewWorkflowFromTemplate
    /// </summary>
    [TestClass]
    public class NewWorkflowFromTemplateUnitTests
    {
        [Description("Verify that method to get the list of templates works as expected.")]
        [Owner("sbora")]
        [TestCategory("Unit")]
        [TestMethod]
        public void VerifyGetListOfTemplatesSuccessfulGet()
        {
            //Arrange: stub out database, create viewmodel-under-test
            using (var clientStub = new Implementation<IWorkflowsQueryService>())
            {
                var fakeReply = new WorkflowTypeGetReplyDC();
                var status = new StatusReplyDC { Errorcode = 0, ErrorGuid = null, ErrorMessage = string.Empty };
                fakeReply.StatusReply = status;
                var fakeList = new List<WorkflowTypesGetBase>();

                var wf = new WorkflowTypesGetBase { WorkflowTemplateId = 1, Name = "OASPage" };
                fakeList.Add(wf);
                fakeReply.WorkflowActivityType = fakeList;
                clientStub.Register(inst =>
                    inst.WorkflowTypeGet())
                    .Return(fakeReply);

                List<WorkflowTemplateItem> returnedData = NewWorkflowFromTemplateViewModel.GetListOfTemplates(clientStub.Instance);
                Assert.IsTrue(returnedData.Count() == 1);
                Assert.IsTrue(returnedData[0].WorkflowTypeName == wf.Name && returnedData[0].WorkflowTemplateId == 1);
            }
        }

        [Description("Verify that method to get the list of templates fails when error is returned from query service.")]
        [Owner("sbora")]
        [TestCategory("Unit")]
        [TestMethod]
        public void VerifyGetListOfTemplatesReturnsNoDataWhenErroredInQS()
        {
            //Arrange: stub out database, create viewmodel-under-test
            using (var clientStub = new Implementation<IWorkflowsQueryService>())
            {
                var fakeReply = new WorkflowTypeGetReplyDC();
                var status = new StatusReplyDC { Errorcode = 10, ErrorGuid = null, ErrorMessage = "Something failed" };
                fakeReply.StatusReply = status;
                var fakeList = new List<WorkflowTypesGetBase>();

                var wf = new WorkflowTypesGetBase { WorkflowTemplateId = 1, Name = "OASPage" };
                fakeList.Add(wf);
                fakeReply.WorkflowActivityType = fakeList;
                clientStub.Register(inst =>
                    inst.WorkflowTypeGet())
                    .Return(fakeReply);

                List<WorkflowTemplateItem> returnedData = NewWorkflowFromTemplateViewModel.GetListOfTemplates(clientStub.Instance);
                Assert.IsTrue(returnedData.Count() == 0);
            }
        }

        [Description("Verify that CreateWorkflowItemExecute creates the WorkflowItem successfully if QS returns data without errors.")]
        [Owner("sbora")]
        [TestCategory("Unit")]
        [TestMethod]
        public void VerifyCreateWorkflowItemExecute()
        {
            Utility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
            var vm = CreateTestViewModel();
            NewWorkflowFromTemplateViewModel_Accessor accessor =
                new NewWorkflowFromTemplateViewModel_Accessor(new PrivateObject(vm));
            vm.SelectedWorkflowTemplateItem = new WorkflowTemplateItem(3, "TestWorkflow");
            var wkItem = new WorkflowItem("TestWorkflow", "Test Workflow", new Sequence().ToXaml(), "Test");
            accessor.GetWorkflowTemplateActivity = client => wkItem;
            vm.WorkflowClassName = "TestWorkflow";
            vm.WorkflowName = "Test Workflow";
            accessor.CreateWorkflowItemExecute();
            Assert.AreEqual(vm.CreatedItem, wkItem);
            Assert.AreEqual(true, wkItem.IsDataDirty);
        }

        [Description("Verify that CreateWorkflowItemExecute raises exception if SelectedWorkflowTemplateItem is null i.e. user has not selected a workflow item yet.")]
        [Owner("sbora")]
        [TestCategory("Unit")]
        [TestMethod]
        public void CreateWorkflowItemExecuteRaiseExceptionwhenSelectedWorkflowTemplateItemisNull()
        {
            var vm = CreateTestViewModel();
            var accessor =
                new NewWorkflowFromTemplateViewModel_Accessor(new PrivateObject(vm));
            vm.SelectedWorkflowTemplateItem = null;
            try
            {
                accessor.CreateWorkflowItemExecute();
                Assert.Fail("Argument null exception should be thrown.");
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsNotNull(ex);
            }
        }

        [Description("Verify that GetWorkflowTemplateActivity raises exception if SelectedWorkflowTemplateItem is null i.e. user has not selected a workflow item yet.")]
        [Owner("sbora")]
        [TestCategory("Unit")]
        [TestMethod]
        public void GetWorkflowTemplateActivityRaiseExceptionwhenSelectedWorkflowTemplateItemisNull()
        {
            using (var clientStub = new Implementation<IWorkflowsQueryService>())
            {
                var vm = CreateTestViewModel();
                var accessor =
                    new NewWorkflowFromTemplateViewModel_Accessor(new PrivateObject(vm));
                vm.SelectedWorkflowTemplateItem = null;
                try
                {
                    accessor.GetWorkflowTemplateActivity(clientStub.Instance);
                    Assert.Fail("Argument null exception should be thrown.");
                }
                catch (ArgumentNullException ex)
                {
                    Assert.IsNotNull(ex);
                }
            }

        }

        [Description("Verify that CreateWorkflowItemExecute.")]
        [Owner("sbora")]
        [TestCategory("Unit")]
        [TestMethod]
        public void VerifyGetWorkflowTemplateActivity()
        {
            var vm = CreateTestViewModel();
            var accessor =
                new NewWorkflowFromTemplateViewModel_Accessor(new PrivateObject(vm));
            vm.SelectedWorkflowTemplateItem = new WorkflowTemplateItem(2, "OAS Page");

            using (var clientStub = new Implementation<IWorkflowsQueryService>())
            {
                var fakeSaReplyList = new List<StoreActivitiesDC>();
                var fakeSaReply = new StoreActivitiesDC() { Description = "Test", Guid = Guid.NewGuid(), IconsId = 1, Id = 50, InsertedDateTime = DateTime.Now, Name = "Service", Namespace = "", ShortName = "Test", ToolBoxtab = 1, Version = "1.0.0.0", WorkflowTypeName = "Metadata", ActivityLibraryId = 50, Xaml = new Sequence().ToXaml() };
                fakeSaReplyList.Add(fakeSaReply);
                clientStub.Register(inst =>
                                    inst.StoreActivitiesGet(Argument<StoreActivitiesDC>.Any))
                    .Return(fakeSaReplyList);

                var fakeAlReplyList = new List<ActivityLibraryDC>();
                var fakeAlReply = new ActivityLibraryDC() { Name = "Test Library", VersionNumber = "1.0.0.0" };

                fakeAlReplyList.Add(fakeAlReply);
                clientStub.Register(inst => inst.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any))
                    .Return(fakeAlReplyList);

                clientStub.Register(inst => inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any))
                    .Return(new GetAllActivityLibrariesReplyDC { List = new List<ActivityLibraryDC>() });

                clientStub.Register(inst => inst.StoreActivityLibraryDependenciesTreeGet(Argument<StoreActivityLibrariesDependenciesDC>.Any))
                    .Return(new List<StoreActivityLibrariesDependenciesDC>());

                var result = accessor.GetWorkflowTemplateActivity(clientStub.Instance);

                Assert.IsNotNull(result);
                Assert.AreEqual("OAS Page", result.WorkflowType); // NOT "Metadata"
            }
        }

        /// <summary>
        /// Factored out: for tests that need a blank viewModel. Replaces the DIf profiler hack.
        /// </summary>
        /// <returns></returns>
        private NewWorkflowFromTemplateViewModel CreateTestViewModel()
        {
            using (var mock = new Implementation<IWorkflowsQueryService>())
            {
                mock.Register(inst => inst.WorkflowTypeGet()).Return(new WorkflowTypeGetReplyDC());
                return new NewWorkflowFromTemplateViewModel(mock.Instance);
            }
        }
    }
}
