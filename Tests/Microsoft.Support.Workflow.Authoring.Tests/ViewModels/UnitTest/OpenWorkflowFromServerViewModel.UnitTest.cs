using System.Collections.Generic;
using CWF.DataContracts;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Xml.Linq;
using AuthoringToolTests.Services;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using System;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.Models;
using System.ServiceModel;
using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels
{
    [TestClass]
    public class OpenWorkflowFromServerViewModelUnitTests
    {
        [WorkItem(325754)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason1")]
        [TestMethod()]
        public void OpenWorkflow_VerifyLoadDataWithException()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
           {
               TestUtilities.RegistLoginUserRole(Role.Admin);
               using (var viewModel = new Implementation<OpenWorkflowFromServerViewModel>())
               {
                   viewModel.Register(inst => inst.LoadData()).Execute(() => { });
                   var vm = viewModel.Instance;

                   using (var client = new Implementation<WorkflowsQueryServiceClient>())
                   {
                       client.Register(inst => inst.SearchActivities(Argument<ActivitySearchRequestDC>.Any)).Execute(() =>
                       {
                           ActivitySearchReplyDC dcs = null;
                           if (dcs == null)
                               throw new FaultException<ServiceFault>(new ServiceFault(), "reason");
                           return dcs;
                       });
                       WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                       UIHelper.IsInTesting = true;

                       try { vm.LoadLiveData(client.Instance); }
                       catch (Exception ex)
                       {
                           Assert.IsTrue(ex is CommunicationException);
                       }

                       client.Register(inst => inst.SearchActivities(Argument<ActivitySearchRequestDC>.Any)).Execute(() =>
                       {
                           ActivitySearchReplyDC dcs = null;
                           if (dcs == null)
                               throw new FaultException<ValidationFault>(new ValidationFault(), "reason");
                           return dcs;
                       });
                       //WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                       try { vm.LoadLiveData(client.Instance); }
                       catch (Exception ex)
                       {
                           Assert.IsTrue(ex is BusinessValidationException);
                       }

                       client.Register(inst => inst.SearchActivities(Argument<ActivitySearchRequestDC>.Any)).Execute(() =>
                       {
                           ActivitySearchReplyDC dcs = null;
                           if (dcs == null)
                               throw new Exception("reason");
                           return dcs;
                       });
                       try { vm.LoadLiveData(client.Instance); }
                       catch (Exception ex)
                       {
                           Assert.IsTrue(ex is CommunicationException);
                       }
                       TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                   }
               }
           });
        }

        [WorkItem(325764)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason1")]
        [TestMethod()]
        public void OpenWorkflow_VerifyPropertyChanged()
        {
            TestUtilities.RegistLoginUserRole(Role.Admin);
            using (var viewModel = new Implementation<OpenWorkflowFromServerViewModel>())
            {
                viewModel.Register(inst => inst.LoadData()).Execute(() => { });
                var vm = viewModel.Instance;

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "FilterByName", () => vm.FilterByName = false);
                Assert.AreEqual(vm.FilterByName, false);

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "FilterByDescription", () => vm.FilterByDescription = false);
                Assert.AreEqual(vm.FilterByDescription, false);

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "FilterByType", () => vm.FilterByType = false);
                Assert.AreEqual(vm.FilterByType, false);

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "FilterByTags", () => vm.FilterByTags = false);
                Assert.AreEqual(vm.FilterByTags, false);

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "FilterByVersion", () => vm.FilterByVersion = false);
                Assert.AreEqual(vm.FilterByVersion, false);

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "FilterByCreatedBy", () => vm.FilterByCreatedBy = false);
                Assert.AreEqual(vm.FilterByCreatedBy, false);

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "FilterOldVersions", () => vm.FilterOldVersions = false);
                Assert.AreEqual(vm.FilterOldVersions, false);

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "ShouldDownloadDependencies", () => vm.ShouldDownloadDependencies = false);
                Assert.AreEqual(vm.ShouldDownloadDependencies, false);

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SearchFilter", () => vm.SearchFilter = "");
                Assert.AreEqual(vm.SearchFilter, "");

                //verify commands execute
                var dc = new StoreActivitiesDC()
                {
                    Environment = Env.Dev.ToString()
                };
                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedWorkflow", () => vm.SelectedWorkflow = dc);
                Assert.AreEqual(vm.SelectedWorkflow, dc);
                Assert.IsTrue(vm.OpenSelectedWorkflowCommand.CanExecute());

                var exists = new System.Collections.ObjectModel.ObservableCollection<StoreActivitiesDC>()
                {
                    dc,
                };
                vm.ExistingWorkflows = exists;
            }
        }

        [WorkItem(325740)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [TestMethod()]
        public void OpenWorkflow_VerifyCommandsExecute()
        {
            TestUtilities.RegistLoginUserRole(Role.Admin);
            using (var viewModel = new Implementation<OpenWorkflowFromServerViewModel>())
            {
                using (var client = new Implementation<WorkflowsQueryServiceClient>())
                {
                    bool isLoaded = false;
                    viewModel.Register(inst => inst.LoadLiveData(client.Instance)).Execute(() =>
                    {
                        isLoaded = true;
                    });
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                    //search command
                    var vm = viewModel.Instance;
                    vm.EnvFilters = new List<Authoring.Common.EnvFilter>()
                    {
                        new EnvFilter(){Env = Authoring.AddIns.Data.Env.Dev,IsFilted= true},
                        new EnvFilter(){Env = Authoring.AddIns.Data.Env.Test,IsFilted= false},
                    };
                    vm.SearchCommand.Execute();
                    Assert.IsTrue(isLoaded);

                    //sort command
                    isLoaded = false;
                    vm.SortCommand.Execute("");
                    Assert.IsTrue(isLoaded);

                    //open command
                    isLoaded = false;

                    StoreActivitiesDC dc = new StoreActivitiesDC()
                    {
                        Environment = Env.Dev.ToString()
                    };
                    client.Register(inst => inst.StoreActivitiesGet(Argument<StoreActivitiesDC>.Any)).Execute(() =>
                    {
                        List<StoreActivitiesDC> list = new List<StoreActivitiesDC>();
                        list.Add(dc);
                        isLoaded = true;
                        return list;
                    });
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                    vm.SelectedWorkflow = dc;

                    vm.OpenSelectedWorkflowCommand.Execute();
                    Assert.IsTrue(isLoaded);
                    Assert.AreEqual(vm.SelectedWorkflow, dc);
                    TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                }
            }
        }


        [Description("Verify that workflows list gets populated with the objects returned from the service.")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void OpenWorkflow_WorkflowsListContainsDataReturnedFromServiceWhenLoadLiveDataCalled()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
               using (var clientStub = new Implementation<WorkflowsQueryServiceClient>()) // Stub out service proxy.
               {
                   TestUtilities.RegistLoginUserRole(Role.Admin);
                   OpenWorkflowFromServerViewModel vm = new OpenWorkflowFromServerViewModel();
                   Assert.AreEqual(0, vm.ExistingWorkflows.Count,
                       "No items are expected on this variable before invoking LoadLiveData method.");

                   string activityName = "Activity 1";
                   string activityVersion = "1.0.0.0";
                   // Create fake response to return some valid workflow activites.
                   clientStub.Register(inst =>
                      inst.SearchActivities(Argument<ActivitySearchRequestDC>.Any))
                      .Return(new ActivitySearchReplyDC()
                      {
                          ServerResultsLength = 1,
                          SearchResults = new List<StoreActivitiesDC>()
                            {
                                new StoreActivitiesDC 
                                   {  
                                       ActivityLibraryName = "TestInput_Lib1",
                                       ActivityLibraryVersion = "1.0.0.1",
                                       ActivityCategoryName = "Custom Activities",
                                       AuthGroupName = "Auth Group 1",
                                       Name = activityName,
                                       Version = activityVersion,
                                       Xaml = @"<SequentialWorkflowActivity x:Class=\""FirstXAMLWFApplication. MyWorkflow\"" Name=\""MyWorkflow \"" xmlns=\""http://schemas.microsoft.com/winfx/2006/xaml/workflow\"" xmlns:x=\""http://schemas.microsoft.com/winfx/2006/xaml\""><WhileActivity x:Name=\""whileActivity1\""></WhileActivity></SequentialWorkflowActivity>"
                                   }
                            },
                            StatusReply = new StatusReplyDC(),

                      });
                   
                   // Create fake response to return no data for other service calls.
                   CreateEmptyFakeResponseForApplicationsGetMethod(clientStub);
                   CreateEmptyFakeResponseForWorkflowTypeGetMethod(clientStub);
                   CreateEmptyFakeResponseForStatusCodeGetMethod(clientStub);

                   WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = ()=>clientStub.Instance;

                   vm.EnvFilters = new List<Authoring.Common.EnvFilter>()
                    {
                        new EnvFilter(){Env = Authoring.AddIns.Data.Env.Dev,IsFilted= true},
                        new EnvFilter(){Env = Authoring.AddIns.Data.Env.Test,IsFilted= false},
                    };
                   vm.LoadLiveData(clientStub.Instance);

                   Assert.AreEqual(1, vm.ExistingWorkflows.Count,
                       "One workflow activity returned from the fake service call is expected here..");
                   Assert.AreEqual(activityName, vm.ExistingWorkflows[0].Name);
                   Assert.AreEqual(activityVersion, vm.ExistingWorkflows[0].Version);
                   TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
               }
           });
        }

        [Description("Verify that workflows list gets populated with the objects returned from the service.")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void OpenWorkflow_VerifyOpenWorkflowViewWorks()
        {
            int capacity = 5;
            TestUtilities.RegistLoginUserRole(Role.Admin);
            using (var clientStub = new Implementation<IWorkflowsQueryService>()) // Stub out service proxy.
            {
                OpenWorkflowFromServerViewModel_Accessor vm = new OpenWorkflowFromServerViewModel_Accessor();
                Assert.AreEqual(0, vm.ExistingWorkflows.Count,
                    "No items are expected on this variable before invoking LoadLiveData method.");

                var activities = GenerateDummyStoreActivities(capacity);

                foreach (var item in activities)
                {
                    vm.ExistingWorkflows.Add(item);
                }
                Assert.AreEqual(5, vm.ExistingWorkflows.Count,
                    "Five items expected in the collection after the fake data is inserted.");
            }
        }

        [Description("Verify that paging is enabled/disabled correctly according to the state of the viewmodel properties.")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void OpenWorkflow_VerifyPaging()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var clientStub = new Implementation<WorkflowsQueryServiceClient>()) // Stub out service proxy.
                {
                    TestUtilities.RegistLoginUserRole(Role.Admin);
                    OpenWorkflowFromServerViewModel_Accessor vm = new OpenWorkflowFromServerViewModel_Accessor();
                    Assert.AreEqual(0, vm.ExistingWorkflows.Count,
                        "No items are expected on this variable before invoking LoadLiveData method.");

                    // Create fake response to return some valid workflow activites.
                    clientStub.Register(inst =>
                       inst.SearchActivities(Argument<ActivitySearchRequestDC>.Any))
                       .Return(new ActivitySearchReplyDC()
                       {
                           ServerResultsLength = 22,
                           SearchResults = GenerateDummyStoreActivities(10),
                           StatusReply = new StatusReplyDC(),
                       });

                    // Create fake response to return no data for other service calls.
                    CreateEmptyFakeResponseForApplicationsGetMethod(clientStub);
                    CreateEmptyFakeResponseForWorkflowTypeGetMethod(clientStub);
                    CreateEmptyFakeResponseForStatusCodeGetMethod(clientStub);
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => clientStub.Instance;
                    
                    vm.EnvFilters = new List<Authoring.Common.EnvFilter>()
                    {
                        new EnvFilter(){Env = Authoring.AddIns.Data.Env.Dev,IsFilted= true},
                        new EnvFilter(){Env = Authoring.AddIns.Data.Env.Test,IsFilted= false},
                    };
                    vm.pageSize = 10;
                    vm.LoadData();

                    Assert.AreEqual(10, vm.ExistingWorkflows.Count,
                        "Ten workflow activities returned from the fake service call are expected here..");
                    TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                }
            });
        }

        [Description("Verify that paging is enabled/disabled correctly according to the state of the viewmodel properties.")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void OpenWorkflow_VerifyPagingToPreviousPage()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var clientStub = new Implementation<WorkflowsQueryServiceClient>()) // Stub out service proxy.
                {
                    TestUtilities.RegistLoginUserRole(Role.Admin);
                    OpenWorkflowFromServerViewModel_Accessor vm = new OpenWorkflowFromServerViewModel_Accessor();
                    Assert.AreEqual(0, vm.ExistingWorkflows.Count,
                        "No items are expected on this variable before invoking LoadLiveData method.");

                    // Create fake response to return some valid workflow activites.
                    clientStub.Register(inst =>
                       inst.SearchActivities(Argument<ActivitySearchRequestDC>.Any))
                       .Return(new ActivitySearchReplyDC()
                       {
                           ServerResultsLength = 22,
                           SearchResults = GenerateDummyStoreActivities(10),
                           StatusReply = new StatusReplyDC(),
                       });

                    // Create fake response to return no data for other service calls.
                    CreateEmptyFakeResponseForApplicationsGetMethod(clientStub);
                    CreateEmptyFakeResponseForWorkflowTypeGetMethod(clientStub);
                    CreateEmptyFakeResponseForStatusCodeGetMethod(clientStub);
                    vm.EnvFilters = new List<Authoring.Common.EnvFilter>()
                    {
                        new EnvFilter(){Env = Authoring.AddIns.Data.Env.Dev,IsFilted= true},
                        new EnvFilter(){Env = Authoring.AddIns.Data.Env.Test,IsFilted= false},
                    };
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => clientStub.Instance;
                    vm.pageSize = 10;
                    vm.LoadData();

                    Assert.AreEqual(10, vm.ExistingWorkflows.Count,
                        "Ten workflow activities returned from the fake service call are expected here..");
                    TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                }
            });
        }

        [Description("Verify that the search command is enabled/disabled correctly according to the state of the vm properties.")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void OpenWorkflow_VerifySearchBehaviour()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var clientStub = new Implementation<WorkflowsQueryServiceClient>()) // Stub out service proxy.
                {
                    TestUtilities.RegistLoginUserRole(Role.Admin);
                    OpenWorkflowFromServerViewModel vm = new OpenWorkflowFromServerViewModel();
                    vm.EnvFilters = new List<Authoring.Common.EnvFilter>()
                    {
                        new EnvFilter(){Env = Authoring.AddIns.Data.Env.Dev,IsFilted= true},
                        new EnvFilter(){Env = Authoring.AddIns.Data.Env.Test,IsFilted= false},
                    };
                    Assert.AreEqual(0, vm.ExistingWorkflows.Count,
                        "No items are expected on this variable before invoking LoadLiveData method.");
                    // Create fake response to return some valid workflow activites.
                    clientStub.Register(inst =>
                       inst.SearchActivities(Argument<ActivitySearchRequestDC>.Any))
                       .Return(new ActivitySearchReplyDC()
                       {
                           ServerResultsLength = 22,
                           SearchResults = GenerateDummyStoreActivities(10),
                           StatusReply = new StatusReplyDC(),
                       });

                    // Create fake response to return no data for other service calls.
                    CreateEmptyFakeResponseForApplicationsGetMethod(clientStub);
                    CreateEmptyFakeResponseForWorkflowTypeGetMethod(clientStub);
                    CreateEmptyFakeResponseForStatusCodeGetMethod(clientStub);
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => clientStub.Instance;
                    vm.LoadData();
                    vm.SearchFilter = "test";

                    Assert.AreEqual(true, vm.SearchCommand.CanExecute(),
                       "Search should be enabled here after the view is initialized and search filter was provided.");
                    TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                }
            });
        }

        private List<StoreActivitiesDC> GenerateDummyStoreActivities(int activityCount)
        {
            List<StoreActivitiesDC> result = new List<StoreActivitiesDC>();

            for (int count = 0; count < activityCount; count++)
            {
                StoreActivitiesDC dummy = new StoreActivitiesDC()
                {
                    ActivityLibraryName = "TestInput_Lib" + Convert.ToString(count),
                    ActivityLibraryVersion = "1.0.0." + Convert.ToString(count % 2),
                    ActivityCategoryName = "Custom Activities",
                    AuthGroupName = "Auth Group 1",
                    Name = "Activity " + Convert.ToString(count),
                    Version = "1.0.0." + Convert.ToString(count % 2),
                    Xaml = @"<SequentialWorkflowActivity x:Class=\""FirstXAMLWFApplication. MyWorkflow\"" Name=\""MyWorkflow \"" xmlns=\""http://schemas.microsoft.com/winfx/2006/xaml/workflow\"" xmlns:x=\""http://schemas.microsoft.com/winfx/2006/xaml\""><WhileActivity x:Name=\""whileActivity1\""></WhileActivity></SequentialWorkflowActivity>",
                    Environment = "dev"
                };
                result.Add(dummy);
            }
            return result;
        }



        private static void CreateEmptyFakeResponseForApplicationsGetMethod(Implementation<WorkflowsQueryServiceClient> clientStub)
        {
            clientStub.Register(inst =>
                inst.ApplicationsGet(Argument<ApplicationsGetRequestDC>.Any))
                .Return(new ApplicationsGetReplyDC
                {
                    StatusReply = new StatusReplyDC { Errorcode = 0, ErrorGuid = null, ErrorMessage = string.Empty },
                    List = new List<ApplicationGetBase>()
                });
        }

        private static void CreateEmptyFakeResponseForStoreActivitiesGetMethod(Implementation<WorkflowsQueryServiceClient> clientStub)
        {
            clientStub.Register(inst =>
               inst.StoreActivitiesGet(Argument<StoreActivitiesDC>.Any))
               .Return(new List<StoreActivitiesDC>());
        }

        private static void CreateEmptyFakeResponseForStoreActivitiesSearchMethod(Implementation<WorkflowsQueryServiceClient> clientStub)
        {
            clientStub.Register(inst =>
               inst.SearchActivities(Argument<ActivitySearchRequestDC>.Any))
               .Return(new ActivitySearchReplyDC() { ServerResultsLength = 0, SearchResults = new List<StoreActivitiesDC>() });
        }

        private static void CreateEmptyFakeResponseForStatusCodeGetMethod(Implementation<WorkflowsQueryServiceClient> clientStub)
        {
            clientStub.Register(inst =>
                inst.StatusCodeGet(Argument<StatusCodeGetRequestDC>.Any))
                .Return(new StatusCodeGetReplyDC
                {
                    StatusReply = new StatusReplyDC { Errorcode = 0, ErrorGuid = null, ErrorMessage = string.Empty },
                    List = new List<StatusCodeAttributes>()
                });
        }

        private static void CreateEmptyFakeResponseForWorkflowTypeGetMethod(Implementation<WorkflowsQueryServiceClient> clientStub)
        {
            clientStub.Register(inst =>
                inst.WorkflowTypeGet(Argument<WorkflowTypesGetRequestDC>.Any))
                .Return(new WorkflowTypeGetReplyDC
                {
                    StatusReply = new StatusReplyDC { Errorcode = 0, ErrorGuid = null, ErrorMessage = string.Empty },
                    WorkflowActivityType = new List<WorkflowTypesGetBase> { new WorkflowTypesGetBase() }
                });
        }
    }
}
