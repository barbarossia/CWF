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
    public class SelectWorkflowsViewModel_UnitTest
    {
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void SelectWorkflows_PropertyChangedNotificationsAreRaised()
        {
            SelectWorkflowViewModel vm = new SelectWorkflowViewModel("dev");

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedActivity", () => vm.SelectedActivity = null);
            Assert.AreEqual(vm.SelectedActivity, null);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Activities", () => vm.Activities = null);
            Assert.AreEqual(vm.Activities, null);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "DataPagingVM", () => vm.DataPagingVM = null);
            Assert.AreEqual(vm.DataPagingVM, null);

            vm.DataPagingVM = new DataPagingViewModel();

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SearchFilter", () => vm.SearchFilter = "filter");
            Assert.AreEqual(vm.SearchFilter, "filter");
        }

        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void SelectWorkflows_SearchCommandExecuted()
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

                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                    SelectWorkflowViewModel vm = new SelectWorkflowViewModel("dev");
                    vm.FilterOldVersions = false;
                    Assert.IsFalse(vm.FilterOldVersions);

                    vm.SearchCommand.Execute();
                    Assert.AreEqual(vm.DataPagingVM.ResultsLength, 0);

                    //FaultException<ValidationFault>
                    client.Register(inst => inst.SearchActivities(Argument<ActivitySearchRequestDC>.Any)).Execute(() =>
                    {
                        ActivitySearchReplyDC dcs = null;
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
                    client.Register(inst => inst.SearchActivities(Argument<ActivitySearchRequestDC>.Any)).Execute(() =>
                    {
                        ActivitySearchReplyDC dcs = null;
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
                    client.Register(inst => inst.SearchActivities(Argument<ActivitySearchRequestDC>.Any)).Execute(() =>
                    {
                        ActivitySearchReplyDC dcs = null;
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
