using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using CWF.DataContracts;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Services;
using System.ServiceModel;
using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.UnitTest
{
    [TestClass]
    public class SearchTaskActivityViewModelUnitTest
    {
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void SearchTaskActivityViewModel_PropertyChangedNotificationsAreRaised()
        {
            SearchTaskActivityViewModel vm = new SearchTaskActivityViewModel();

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedActivity", () => vm.SelectedActivity = null);
            Assert.AreEqual(vm.SelectedActivity, null);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Activities", () => vm.Activities = null);
            Assert.AreEqual(vm.Activities, null);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "DataPagingVM", () => vm.DataPagingVM = null);
            Assert.AreEqual(vm.DataPagingVM, null);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SortColumn", () => vm.SortColumn = "Name");
            Assert.AreEqual(vm.SortColumn, "Name");

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SortByAsceinding", () => vm.SortByAsceinding = true);
            Assert.AreEqual(vm.SortByAsceinding, true);

            vm.DataPagingVM = new DataPagingViewModel();

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SearchFilter", () => vm.SearchFilter = "filter");
            Assert.AreEqual(vm.SearchFilter, "filter");
        }

        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void SearchTaskActivityViewModel_GetSelectedActivity()
        {
            SearchTaskActivityViewModel vm = new SearchTaskActivityViewModel();
            vm.SelectedActivity = new TaskActivityDC() { StatusReply = new StatusReplyDC() };
            PrivateObject pv = new PrivateObject(vm);
            using (var client = new Implementation<WorkflowsQueryServiceClient>())
            {
                TaskActivityDC reply = new TaskActivityDC()
                {
                    StatusReply = new StatusReplyDC(),
                    Id = 1
                };

                client.Register(inst => inst.TaskActivityGet(Argument<TaskActivityDC>.Any))
                    .Return(reply);

                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                pv.Invoke("GetSelectedActivity");
                Assert.AreEqual(vm.SelectedActivity, reply);
                //reset client
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
            }
        }

        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void SearchTaskActivityViewModel_SearchCommandExecuted()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var client = new Implementation<WorkflowsQueryServiceClient>())
                {
                    TaskActivityGetReplyDC reply = new TaskActivityGetReplyDC();
                    reply.List = new List<TaskActivityDC>();
                    reply.ServerResultsLength = 0;
                    reply.StatusReply = new StatusReplyDC();

                    client.Register(inst => inst.SearchTaskActivities(Argument<TaskActivityGetRequestDC>.Any))
                        .Return(reply);

                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                    SearchTaskActivityViewModel vm = new SearchTaskActivityViewModel();
                    vm.FilterOldVersions = false;
                    Assert.IsFalse(vm.FilterOldVersions);

                    vm.SearchCommand.Execute();
                    Assert.AreEqual(vm.DataPagingVM.ResultsLength, 0);

                    //FaultException<ValidationFault>
                    client.Register(inst => inst.SearchTaskActivities(Argument<TaskActivityGetRequestDC>.Any)).Execute(() =>
                    {
                        TaskActivityGetReplyDC dcs = null;
                        if (dcs == null)
                            throw new FaultException<ValidationFault>(new ValidationFault(), "reason");
                        return dcs;
                    });

                    try { vm.SearchCommand.Execute(); }
                    catch (Exception ex)
                    {
                        Assert.IsTrue(ex is BusinessValidationException);
                    }

                    //FaultException<ServiceFault>
                    client.Register(inst => inst.SearchTaskActivities(Argument<TaskActivityGetRequestDC>.Any)).Execute(() =>
                    {
                        TaskActivityGetReplyDC dcs = null;
                        if (dcs == null)
                            throw new FaultException<ServiceFault>(new ServiceFault(), "reason");
                        return dcs;
                    });

                    try { vm.SearchCommand.Execute(); }
                    catch (Exception ex)
                    {
                        Assert.IsFalse(ex is CommunicationException);
                    }

                    //common excption as communication exception
                    client.Register(inst => inst.SearchTaskActivities(Argument<TaskActivityGetRequestDC>.Any)).Execute(() =>
                    {
                        TaskActivityGetReplyDC dcs = null;
                        if (dcs == null)
                            throw new Exception("reason");
                        return dcs;
                    });
                    try { vm.SearchCommand.Execute(); }
                    catch (Exception ex)
                    {
                        Assert.IsFalse(ex is CommunicationException);
                    }

                    //reset client
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                }
            });
        }
    }
}
