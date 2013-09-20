using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.UnitTest
{
    [TestClass]
    public class EditWorkflowTypeViewModel_UnitTest
    {
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void EditWorkflowType_PropertyChangedNotificationsAreRaised()
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

                EditWorkflowTypeViewModel vm = new EditWorkflowTypeViewModel(WorkflowTypeOperations.Add, null, null);
                Assert.AreEqual(vm.WindowTitle, "Add Workflow Type");

                Assert.IsTrue(vm.AuthGroups != null && vm.AuthGroups.Count == 1);

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedAuthGroup", () => vm.SelectedAuthGroup = agdc);
                Assert.AreEqual(vm.SelectedAuthGroup, agdc);

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Name", () => vm.Name = "NewWFType");
                Assert.AreEqual(vm.Name, "NewWFType");

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "PublishingWorkflowId", () => vm.PublishingWorkflowId = 1);
                Assert.AreEqual(vm.PublishingWorkflowId, 1);

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "PublishingWorkflow", () => vm.PublishingWorkflow = "PbWF");
                Assert.AreEqual(vm.PublishingWorkflow, "PbWF");

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "TemplateId", () => vm.TemplateId = 1);
                Assert.AreEqual(vm.TemplateId, 1);

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Template", () => vm.Template = "Template");
                Assert.AreEqual(vm.Template, "Template");

                //reset client
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
            }
        }

        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void EditWorkflowType_UploadWorkflowType()
        {
            using (var client = new Implementation<WorkflowsQueryServiceClient>())
            {
                WorkFlowTypeCreateOrUpdateReplyDC reply = new WorkFlowTypeCreateOrUpdateReplyDC();
                client.Register(inst => inst.WorkflowTypeCreateOrUpdate(Argument<WorkFlowTypeCreateOrUpdateRequestDC>.Any))
                    .Return(reply);

                AuthorizationGroupGetReplyDC auReply = new AuthorizationGroupGetReplyDC();
                AuthorizationGroupDC auDc = new AuthorizationGroupDC()
                {
                    AuthGroupId = 1,
                    AuthGroupName = "admin"
                };
                auReply.AuthorizationGroups = new List<AuthorizationGroupDC>() { auDc };
                client.Register(Inst => Inst.GetAuthorizationGroups(Argument<AuthorizationGroupGetRequestDC>.Any))
                    .Return(auReply);
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                //verify Name too long
                EditWorkflowTypeViewModel vm = new EditWorkflowTypeViewModel(WorkflowTypeOperations.Add, null, null);
                vm.Name = "qwertyuioplkjhgfdsazxcvbnm,./123456789123456789123456";
                try
                {
                    vm.UploadWorkflowType(client.Instance);
                }
                catch (Exception ex)
                {
                    Assert.AreEqual(ex.Message, "The Name is too long.");
                }

                //Verify has error
                reply.StatusReply = new StatusReplyDC();
                reply.StatusReply.Errorcode = 15001;
                reply.StatusReply.ErrorMessage = "Request Error";
                try
                {
                    vm.Name = "validName";
                    vm.UploadWorkflowType(client.Instance);
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex is UserFacingException);
                    Assert.AreEqual(ex.Message, "Request Error");
                }
            }
        }


        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void EditWorkflowType_BrowserPublishingWorkflows()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var client = new Implementation<WorkflowsQueryServiceClient>())
                {
                    ActivitySearchReplyDC reply = new ActivitySearchReplyDC();
                    reply.SearchResults = new List<StoreActivitiesDC>();
                    reply.ServerResultsLength = 0;

                    client.Register(inst => inst.SearchActivities(Argument<ActivitySearchRequestDC>.Any))
                        .Return(reply);

                    AuthorizationGroupGetReplyDC auReply = new AuthorizationGroupGetReplyDC();
                    AuthorizationGroupDC auDc = new AuthorizationGroupDC()
                    {
                        AuthGroupId = 1,
                        AuthGroupName = "admin"
                    };
                    auReply.AuthorizationGroups = new List<AuthorizationGroupDC>() { auDc };
                    client.Register(Inst => Inst.GetAuthorizationGroups(Argument<AuthorizationGroupGetRequestDC>.Any))
                        .Return(auReply);

                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                    using (var service = new ImplementationOfType(typeof(DialogService)))
                    {
                        bool isInvoked = false;
                        service.Register(() => DialogService.ShowDialog(Argument<SelectWorkflowViewModel>.Any)).Execute(() =>
                        {
                            bool? result = true;
                            isInvoked = true;
                            return result;
                        });

                        EditWorkflowTypeViewModel vm = new EditWorkflowTypeViewModel(WorkflowTypeOperations.Add, null, null);
                        PrivateObject po = new PrivateObject(vm);
                        po.Invoke("BrowserPublishingWorkflows");
                        Assert.IsTrue(isInvoked);
                    }

                    //reset client
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                }
            });
        }

        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void EditWorkflowType_BrowserTemplateWorkflows()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
             {
                 using (var client = new Implementation<WorkflowsQueryServiceClient>())
                 {
                     ActivitySearchReplyDC reply = new ActivitySearchReplyDC();
                     reply.SearchResults = new List<StoreActivitiesDC>();
                     reply.ServerResultsLength = 0;

                     client.Register(inst => inst.SearchActivities(Argument<ActivitySearchRequestDC>.Any))
                         .Return(reply);
                     AuthorizationGroupGetReplyDC auReply = new AuthorizationGroupGetReplyDC();
                     AuthorizationGroupDC auDc = new AuthorizationGroupDC()
                     {
                         AuthGroupId = 1,
                         AuthGroupName = "admin"
                     };
                     auReply.AuthorizationGroups = new List<AuthorizationGroupDC>() { auDc };
                     client.Register(Inst => Inst.GetAuthorizationGroups(Argument<AuthorizationGroupGetRequestDC>.Any))
                         .Return(auReply);

                     WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                     using (var service = new ImplementationOfType(typeof(DialogService)))
                     {
                         bool isInvoked = false;
                         service.Register(() => DialogService.ShowDialog(Argument<SelectWorkflowViewModel>.Any)).Execute(() =>
                         {
                             bool? result = true;
                             isInvoked = true;
                             return result;
                         });

                         EditWorkflowTypeViewModel vm = new EditWorkflowTypeViewModel(WorkflowTypeOperations.Add, null, null);
                         PrivateObject po = new PrivateObject(vm);
                         po.Invoke("BrowserTemplateWorkflows");
                         Assert.IsTrue(isInvoked);
                     }

                     //reset client
                     WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                 }
             });
        }

        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void EditWorkflowType_GetAuthGroups()
        {
            using (var client = new Implementation<WorkflowsQueryServiceClient>())
            {
                AuthorizationGroupGetReplyDC auReply = new AuthorizationGroupGetReplyDC();
                AuthorizationGroupDC auDc = new AuthorizationGroupDC()
                {
                    AuthGroupId = 1,
                    AuthGroupName = "admin"
                };
                auReply.AuthorizationGroups = new List<AuthorizationGroupDC>() { auDc };
                client.Register(inst => inst.GetAuthorizationGroups(Argument<AuthorizationGroupGetRequestDC>.Any))
                    .Return(auReply);
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                WorkflowTypeSearchDC dc = new WorkflowTypeSearchDC();
                dc.Name = "validName";
                dc.AuthGroupId = 1;
                EditWorkflowTypeViewModel vm = new EditWorkflowTypeViewModel(WorkflowTypeOperations.Add, dc, null);
                PrivateObject po = new PrivateObject(vm);
                po.Invoke("GetAuthGroups", client.Instance);
                Assert.AreEqual(auDc.AuthGroupId, vm.SelectedAuthGroup.AuthGroupId);
                //reset client
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
            }
        }
    }
}
