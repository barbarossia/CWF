using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.UnitTest
{
    [TestClass]
    public class ManageWorkflowTypeViewModel_UnitTest
    {
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void ManageWorkflowType_PropertyChangedNotificationsAreRaised()
        {
            ManageWorkflowTypeViewModel vm = new ManageWorkflowTypeViewModel();

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "DataPagingVM", () => vm.DataPagingVM = null);
            Assert.AreEqual(vm.DataPagingVM, null);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SortColumn", () => vm.SortColumn = "Id");
            Assert.AreEqual(vm.SortColumn, "Id");

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SortByAsceinding", () => vm.SortByAsceinding = true);
            Assert.AreEqual(vm.SortByAsceinding, true);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SearchText", () => vm.SearchText = "PbWF");
            Assert.AreEqual(vm.SearchText, "PbWF");

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "EditWorkflowTypeVM", () => vm.EditWorkflowTypeVM = null);
            Assert.AreEqual(vm.EditWorkflowTypeVM, null);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "IsEditing", () => vm.IsEditing = true);
            Assert.AreEqual(vm.IsEditing, true);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedWorkflowType", () => vm.SelectedWorkflowType = null);
            Assert.AreEqual(vm.SelectedWorkflowType, null);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "WorkflowTypes", () => vm.WorkflowTypes = null);
            Assert.AreEqual(vm.WorkflowTypes, null);
        }

        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void ManageWorkflowType_CanUploadWorkflowCommandExecute()
        {
            AuthorizationGroupDC agdc = new AuthorizationGroupDC();
            List<AuthorizationGroupDC> agdcs = new List<AuthorizationGroupDC>() { agdc };
            AuthorizationGroupGetReplyDC reply = new AuthorizationGroupGetReplyDC();
            reply.AuthorizationGroups = agdcs;

            using (var client = new Implementation<WorkflowsQueryServiceClient>())
            {
                client.Register(Inst => Inst.GetAuthorizationGroups(Argument<AuthorizationGroupGetRequestDC>.Any))
                    .Return(reply);

                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                ManageWorkflowTypeViewModel vm = new ManageWorkflowTypeViewModel();
                vm.EditWorkflowTypeVM = null;
                Assert.IsFalse(vm.UploadWorkflowCommand.CanExecute());

                vm.EditWorkflowTypeVM = new EditWorkflowTypeViewModel(WorkflowTypeOperations.Add, null, null);
                vm.EditWorkflowTypeVM.Name = null;

                Assert.IsFalse(vm.UploadWorkflowCommand.CanExecute());
                vm.EditWorkflowTypeVM.Name = "  ";
                Assert.IsFalse(vm.UploadWorkflowCommand.CanExecute());

                vm.EditWorkflowTypeVM.Name = "TypeTest";
                vm.EditWorkflowTypeVM.SelectedAuthGroup = null;
                Assert.IsFalse(vm.UploadWorkflowCommand.CanExecute());

                vm.EditWorkflowTypeVM.SelectedAuthGroup = new AuthorizationGroupDC();
                Assert.IsTrue(vm.UploadWorkflowCommand.CanExecute());
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
            }
        }

        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void ManageWorkflowType_SearchCommandExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
             {
                 using (var client = new Implementation<WorkflowsQueryServiceClient>())
                 {
                     WorkflowTypeSearchReply reply = new WorkflowTypeSearchReply();
                     reply.SearchResults = new List<WorkflowTypeSearchDC>() { new WorkflowTypeSearchDC() };
                     reply.ServerResultsLength = 1;
                     reply.StatusReply = new StatusReplyDC();

                     client.Register(inst => inst.SearchWorkflowTypes(Argument<WorkflowTypeSearchRequest>.Any))
                         .Return(reply);
                     WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                     ManageWorkflowTypeViewModel vm = new ManageWorkflowTypeViewModel();
                     vm.SearchWorkflowTypeCommand.Execute();

                     Assert.AreEqual(vm.DataPagingVM.ResultsLength, 1);

                     WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                 }
             });
        }

        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void ManageWorkflowType_DeleteWorkflowType()
        {
            using (var client = new Implementation<WorkflowsQueryServiceClient>())
            {
                WorkflowTypeSearchReply reply = new WorkflowTypeSearchReply();
                reply.SearchResults = new List<WorkflowTypeSearchDC>() { new WorkflowTypeSearchDC() };
                reply.ServerResultsLength = 1;
                bool isDeleted = false;

                client.Register(inst => inst.SearchWorkflowTypes(Argument<WorkflowTypeSearchRequest>.Any))
                    .Return(reply);

                client.Register(inst => inst.WorkflowTypeCreateOrUpdate(Argument<WorkFlowTypeCreateOrUpdateRequestDC>.Any))
                    .Execute(() =>
                    {
                        isDeleted = true;
                        return new WorkFlowTypeCreateOrUpdateReplyDC();
                    });
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                WorkflowTypeSearchDC type = new WorkflowTypeSearchDC();
                ManageWorkflowTypeViewModel vm = new ManageWorkflowTypeViewModel();
                vm.SelectedWorkflowType = type;
                vm.DeleteWorkflowTypeCommand.Execute();
                Assert.IsTrue(isDeleted);
                //reset client
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
            }
        }

        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void ManageWorkflowType_OnAddWorkflowType()
        {
            AuthorizationGroupDC agdc = new AuthorizationGroupDC();
            List<AuthorizationGroupDC> agdcs = new List<AuthorizationGroupDC>() { agdc };
            AuthorizationGroupGetReplyDC reply = new AuthorizationGroupGetReplyDC();
            reply.AuthorizationGroups = agdcs;

            using (var client = new Implementation<WorkflowsQueryServiceClient>())
            {
                client.Register(Inst => Inst.GetAuthorizationGroups(Argument<AuthorizationGroupGetRequestDC>.Any))
                    .Return(reply);

                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                ManageWorkflowTypeViewModel vm = new ManageWorkflowTypeViewModel();
                vm.IsEditing = false;
                vm.AddWorkTypeflowTypeCommand.Execute();
                Assert.IsTrue(vm.IsEditing);
                //reset client
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
            }
        }

        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void ManageWorkflowType_OnEditWorkflowType()
        {
            AuthorizationGroupDC agdc = new AuthorizationGroupDC();
            List<AuthorizationGroupDC> agdcs = new List<AuthorizationGroupDC>() { agdc };
            AuthorizationGroupGetReplyDC reply = new AuthorizationGroupGetReplyDC();
            reply.AuthorizationGroups = agdcs;

            using (var client = new Implementation<WorkflowsQueryServiceClient>())
            {
                client.Register(Inst => Inst.GetAuthorizationGroups(Argument<AuthorizationGroupGetRequestDC>.Any))
                    .Return(reply);

                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                ManageWorkflowTypeViewModel vm = new ManageWorkflowTypeViewModel();
                vm.IsEditing = false;
                vm.EditWorkflowTypeCommand.Execute();
                Assert.IsFalse(vm.IsEditing);

                WorkflowTypeSearchDC type = new WorkflowTypeSearchDC();
                vm.SelectedWorkflowType = type;
                vm.EditWorkflowTypeCommand.Execute();
                Assert.IsTrue(vm.IsEditing);
                //reset client
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
            }

        }

        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void ManageWorkflowType_UploadWorkflowType()
        {
            using (var client = new Implementation<WorkflowsQueryServiceClient>())
            {
                WorkflowTypeSearchReply reply = new WorkflowTypeSearchReply();
                reply.SearchResults = new List<WorkflowTypeSearchDC>() { new WorkflowTypeSearchDC() };
                reply.ServerResultsLength = 1;
                bool isUploaded = false;

                client.Register(inst => inst.SearchWorkflowTypes(Argument<WorkflowTypeSearchRequest>.Any))
                    .Return(reply);

                client.Register(inst => inst.WorkflowTypeCreateOrUpdate(Argument<WorkFlowTypeCreateOrUpdateRequestDC>.Any))
                    .Execute(() =>
                    {
                        isUploaded = true;
                        return new WorkFlowTypeCreateOrUpdateReplyDC();
                    });
                
                AuthorizationGroupDC agdc = new AuthorizationGroupDC();
                List<AuthorizationGroupDC> agdcs = new List<AuthorizationGroupDC>() { agdc };
                AuthorizationGroupGetReplyDC reply1 = new AuthorizationGroupGetReplyDC();
                reply1.AuthorizationGroups = agdcs;
                client.Register(Inst => Inst.GetAuthorizationGroups(Argument<AuthorizationGroupGetRequestDC>.Any))
                    .Return(reply1);

                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                ManageWorkflowTypeViewModel vm = new ManageWorkflowTypeViewModel();
                vm.IsEditing = true;

                WorkflowTypeSearchDC type = new WorkflowTypeSearchDC();
                type.Name = "test";
                vm.EditWorkflowTypeVM = new EditWorkflowTypeViewModel(WorkflowTypeOperations.Edit, type, null);
                vm.UploadWorkflowCommand.Execute();
                Assert.IsTrue(isUploaded);
                Assert.IsFalse(vm.IsEditing);
                Assert.IsNull(vm.EditWorkflowTypeVM);
                
                //reset client
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
            }
        }

    }
}
