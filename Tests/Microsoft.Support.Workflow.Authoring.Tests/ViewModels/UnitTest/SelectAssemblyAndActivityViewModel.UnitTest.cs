using System.Collections.Generic;
using CWF.DataContracts;
using System.Linq;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AuthoringToolTests.Services;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using System;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.HelpClass;
using System.ServiceModel;
using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using Microsoft.Support.Workflow.Authoring;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
namespace Authoring.Tests.Unit
{
    /// <summary>
    /// Summary description for SelectAssemblyAndActivityViewModel
    /// </summary>
    [TestClass]
    public class SelectAssemblyAndActivityViewModelUnitTests
    {

        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [WorkItem(325762)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [TestMethod()]
        public void SelectAssembly_VerifyPropertyChanged()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
               {
                   using (new CachingIsolator())
                   {
                       var dataHelper = new MarketplaceDataHelper(TestContext);
                       using (var viewModel = new Implementation<SelectAssemblyAndActivityViewModel>())
                       {
                           var client = new WorkflowsQueryServiceClient();
                           WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client;
                           viewModel.Register(inst => inst.LoadLiveData(client)).Execute(() => { });

                           var vm = viewModel.Instance;
                           ActivityAssemblyItem testLib = new ActivityAssemblyItem();//TestInputs.ActivityAssemblyItems.TestInput_Lib1;
                           TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "CurrentActivityAssemblyItem", () => vm.CurrentActivityAssemblyItem = testLib);
                           Assert.AreEqual(vm.CurrentActivityAssemblyItem, testLib);

                           ActivityItem item = new ActivityItem();
                           TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "CurrentActivityItem", () => vm.CurrentActivityItem = item);
                           Assert.AreEqual(vm.CurrentActivityItem, item);

                           //test ActivityItem UserSelected set to true  when ActivityAssemblyItem UserSelected set to true
                           testLib = dataHelper.GetTestActivities()[0];
                           vm.CurrentActivityAssemblyItem = testLib;

                           Assert.IsTrue(vm.CurrentActivityAssemblyItem.ActivityItems.Count > 0);

                           vm.CurrentActivityAssemblyItem.UserSelected = true;
                           var items = vm.CurrentActivityAssemblyItem.ActivityItems;

                           Assert.IsTrue(items.ToList().All(i => i.UserSelected == true));
                       }
                   }
               });
        }

        [WorkItem(325748)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [TestMethod()]
        public void SelectAssembly_VerifyGetActivityItemsByActivityAssemblyItemWithNullParameter()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (new CachingIsolator())
                {
                    //var dataHelper = new MarketplaceDataHelper(TestContext);
                    using (var viewModel = new Implementation<SelectAssemblyAndActivityViewModel>())
                    {
                        var client = new WorkflowsQueryServiceClient();
                        WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client;
                        viewModel.Register(inst => inst.LoadLiveData(client)).Execute(() => { });

                        var vm = viewModel.Instance;

                        try { vm.GetActivityItemsByActivityAssemblyItem(null, null); }
                        catch (ArgumentNullException expectException)
                        {
                            Assert.AreEqual(expectException.ParamName, "client");
                        }

                        try { vm.GetActivityItemsByActivityAssemblyItem(client, null); }
                        catch (ArgumentNullException expectException)
                        {
                            Assert.AreEqual(expectException.ParamName, "activityAssemblyItem");
                        }
                        WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                    }
                }
            });
        }

        [WorkItem(325758)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [TestMethod()]
        public void SelectAssembly_VerifyOkCommandExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (new CachingIsolator())
                {
                    using (var viewModel = new Implementation<SelectAssemblyAndActivityViewModel>())
                    {
                        SelectAssemblyAndActivityViewModel vm = null;
                        using (var client1 = new Implementation<WorkflowsQueryServiceClient>())
                        {
                            client1.Register(inst => inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any)).Execute(() =>
                            {
                                GetAllActivityLibrariesReplyDC dc = null;
                                return dc;
                            });
                            WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client1.Instance;
                            vm = new SelectAssemblyAndActivityViewModel();

                            using (var client = new Implementation<WorkflowsQueryServiceClient>())
                            {
                                client.Register(inst => inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any)).Execute(() =>
                                {
                                    GetAllActivityLibrariesReplyDC dc = null;
                                    return dc;
                                });
                                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                                var dataHelper = new MarketplaceDataHelper(TestContext);
                                var testLib = dataHelper.GetTestActivities()[0];
                                vm.ActivityAssemblyItemCollection = new System.Collections.ObjectModel.ObservableCollection<ActivityAssemblyItem>() { testLib };
                                vm.CurrentActivityAssemblyItem = vm.ActivityAssemblyItemCollection[0];
                                vm.CurrentActivityAssemblyItem.UserSelected = true;
                                vm.CurrentActivityAssemblyItem.CachingStatus = CachingStatus.Server;

                                using (var cach = new ImplementationOfType(typeof(Caching)))
                                {
                                    bool isOk = false;
                                    cach.Register(() => Caching.CacheAssembly(Argument<List<ActivityAssemblyItem>>.Any, Argument<bool>.Any)).Execute(() => { isOk = true; });
                                    cach.Register(() => Caching.ComputeDependencies(client.Instance, Argument<List<ActivityAssemblyItem>>.Any)).Return(new List<ActivityAssemblyItem>());
                                    cach.Register(() => Caching.DownloadAssemblies(client.Instance, Argument<List<ActivityAssemblyItem>>.Any)).Return(new List<ActivityAssemblyItem>());
                                    try
                                    {
                                        vm.OkCommand.Execute();
                                        Assert.IsTrue(isOk);
                                    }
                                    catch (Exception ex)
                                    {
                                        Assert.IsTrue(ex is CommunicationException);
                                    }
                                }
                            }
                        }
                        TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                    }
                }
            });
        }

        [WorkItem(325755)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [TestMethod()]
        public void SelectAssembly_VerifyLoadLiveDataWithException()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
           {
               using (var client = new Implementation<WorkflowsQueryServiceClient>())
               {
                   client.Register(inst => inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any)).Execute(() =>
                   {
                       GetAllActivityLibrariesReplyDC dc = null;
                       if (dc == null)
                           throw new FaultException<ServiceFault>(new ServiceFault(), "expect reason");
                       return dc;
                   });
                   WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                   try { var vm = new SelectAssemblyAndActivityViewModel(); }
                   catch (Exception expect)
                   {
                       Assert.IsTrue(expect is UserFacingException);
                       Assert.IsTrue(expect.InnerException is CommunicationException);
                   }

                   client.Register(inst => inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any)).Execute(() =>
                   {
                       GetAllActivityLibrariesReplyDC dc = null;
                       if (dc == null)
                           throw new FaultException<ValidationFault>(new ValidationFault(), "expect reason");
                       return dc;
                   });
                   WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                   try { var vm = new SelectAssemblyAndActivityViewModel(); }
                   catch (Exception expect)
                   {
                       Assert.IsTrue(expect is BusinessValidationException);
                   }

                   client.Register(inst => inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any)).Execute(() =>
                   {
                       GetAllActivityLibrariesReplyDC dc = null;
                       if (dc == null)
                           throw new Exception("expect reason");
                       return dc;
                   });
                   WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                   try { var vm = new SelectAssemblyAndActivityViewModel(); }
                   catch (Exception expect)
                   {
                       Assert.IsTrue(expect is UserFacingException);
                       Assert.IsTrue(expect.InnerException is CommunicationException);
                   }
                   TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
               }
           });
        }

        [Description("Verify that Select gracefully handles ActivityLibraries from the DB with garbage or null Executables")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void SelectAssembly_TestBadExecutable()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();

                // Arrange: stub out database, create viewmodel-under-test
                using (new CachingIsolator())
                using (var clientStub = new Implementation<WorkflowsQueryServiceClient>())
                {
                    // Library with bad bytes
                    var fakeLib =
                        new ActivityLibraryDC { Name = "MyLib", VersionNumber = "1.0.0.0", Executable = new byte[] { } };

                    // The DB will return a list of [fakeLib] to populate the Select. Executable is ignored on this call.
                    clientStub.Register(inst =>
                        inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any))
                        .Return(new GetAllActivityLibrariesReplyDC
                            {
                                Errorcode = 0,
                                List = new List<ActivityLibraryDC>
                            {
                                fakeLib
                            }
                            });

                    // Fakelib has no dependencies
                    clientStub.Register(inst => inst.StoreActivityLibraryDependenciesTreeGet(Argument<StoreActivityLibrariesDependenciesDC>.Any))
                        .Return(new List<StoreActivityLibrariesDependenciesDC>());

                    // When Select asks for the full version of fakeLib, including the Executable, we just
                    // return fakeLib again--but this time it will see the bad executable
                    clientStub.Register(inst => inst.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any))
                        .Return(new List<ActivityLibraryDC> { fakeLib });

                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => clientStub.Instance;

                    // ViewModel-under-test
                    var select = new SelectAssemblyAndActivityViewModel();

                    // Mark fakeLib in the virtual UI as something that we want to download
                    foreach (var item in select.ActivityAssemblyItemCollection)
                        item.UserSelected = true;

                    select.OkCommandExecute(clientStub.Instance);
                    TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                }
            });
        }

        [Description("Verify that LoadLiveData method ignores any dummy libraries returned from data service.")]
        [Owner("v-sanja")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void SelectAssembly_DiscardDummyLibrariesReturnedFromDataServiceWhenLoadLiveDataCalled()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                ActivityAssemblyItem[] assemblyItems = 
                {   
                    TestInputs.ActivityAssemblyItems.TestInput_Lib3,
                    TestInputs.ActivityAssemblyItems.TestInput_Lib2,
                    TestInputs.ActivityAssemblyItems.TestInput_Lib1
                };

                using (var cache = new CachingIsolator(assemblyItems))
                using (var clientStub = new Implementation<WorkflowsQueryServiceClient>()) // Stub out service proxy.
                {
                    // Create fake reply.
                    var fakeReply = new GetAllActivityLibrariesReplyDC() { Errorcode = 0, ErrorGuid = null, ErrorMessage = string.Empty };
                    fakeReply.List = new List<ActivityLibraryDC>();

                    clientStub.Register(inst => inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any))
                        .Return(fakeReply);

                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => clientStub.Instance;
                    var vm = new SelectAssemblyAndActivityViewModel();
                    // Assert that only the 3 cached assemblies defined in CachingIsolator are included in ActivityAssemblyItemCollection.
                    // Dummy libraries are to be ignored. 
                    foreach (ActivityAssemblyItem item in assemblyItems)
                    {
                        Assert.IsTrue(vm.ActivityAssemblyItemCollection.Contains(item));
                    }

                    // None of the assemblies returned from the service should be in vm.ActivityAssemblyItemCollection.  
                    // Assert whether the available assemblies are really the assemblies defined with CachingIsolator.
                    List<ActivityAssemblyItem> items = new List<ActivityAssemblyItem>(vm.ActivityAssemblyItemCollection);
                    AssertPresenceOfCachedAssemblies(items);
                    TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                }
            });
        }

        [Description("Verify that LoadLiveData method ignores any libraries returned from data service if they are already in cache.")]
        [Owner("v-sanja")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void SelectAssembly_DiscardAlreadyCachedLibrariesReturnedFromDataServiceWhenLoadLiveDataCalled()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                ActivityAssemblyItem[] assemblyItems = 
                {   
                    TestInputs.ActivityAssemblyItems.TestInput_Lib3,
                    TestInputs.ActivityAssemblyItems.TestInput_Lib2,
                    TestInputs.ActivityAssemblyItems.TestInput_Lib1
                };
                using (var cache = new CachingIsolator(assemblyItems))

                using (var clientStub = new Implementation<WorkflowsQueryServiceClient>()) // Stub out service proxy.
                {
                    // Create fake reply.
                    var fakeReply = new GetAllActivityLibrariesReplyDC() { Errorcode = 0, ErrorGuid = null, ErrorMessage = string.Empty };
                    fakeReply.List = new List<ActivityLibraryDC>();

                    // Add already cached assemblies to the fake reply.
                    AssemblyName assemblyName = new AssemblyName(TestInputs.Assemblies.TestInput_Lib1.FullName);
                    var library = new ActivityLibraryDC { Name = assemblyName.Name, VersionNumber = assemblyName.Version.ToString(), Executable = new byte[10] };
                    fakeReply.List.Add(library);
                    assemblyName = new AssemblyName(TestInputs.Assemblies.TestInput_Lib2.FullName);
                    library = new ActivityLibraryDC { Name = assemblyName.Name, VersionNumber = "None", Executable = new byte[20] };
                    fakeReply.List.Add(library);
                    assemblyName = new AssemblyName(TestInputs.Assemblies.TestInput_Lib3.FullName);
                    library = new ActivityLibraryDC { Name = assemblyName.Name, VersionNumber = "None", Executable = new byte[5] };
                    fakeReply.List.Add(library);

                    clientStub.Register(inst => inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any))
                        .Return(fakeReply);
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => clientStub.Instance;
                    var vm = new SelectAssemblyAndActivityViewModel();

                    // Assert that only the 3 cached assemblies defined in CachingIsolator are included in ActivityAssemblyItemCollection.
                    // Already cached assemblies should not show up twice on the Select screen.
                    foreach (ActivityAssemblyItem item in assemblyItems)
                    {
                        Assert.IsTrue(vm.ActivityAssemblyItemCollection.Contains(item));
                    }
                    // Assert whether the available assemblies are really the assemblies defined with CachingIsolator.
                    List<ActivityAssemblyItem> items = new List<ActivityAssemblyItem>(vm.ActivityAssemblyItemCollection);
                    AssertPresenceOfCachedAssemblies(items);
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                }
            });
        }

        [Description("Verify that LoadLiveData method considers non-cached assemblies into ActivityAssemblyItemCollection.")]
        [Owner("v-sanja")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void SelectAssembly_IncludeNonCachedLibrariesReturnedFromDataServiceWhenLoadLiveDataCalled()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                //Create New WorkFlow
                WorkflowItem newWorkflow = WorkFlowActions.AutoGeneratedWorkFlowItem;
                ActivityAssemblyItem aai = new ActivityAssemblyItem
                {
                    Name = newWorkflow.Name,
                    Version = Version.Parse(newWorkflow.Version)
                };

                //Create project or workflow as public activities
                var properties = new WorkFlowProperties
                {
                    Name = TestUtilities.GetRandomStringOfLength(15),
                    Status = "Private",
                    Version = "0.0.0.1"
                };
                //WorkFlowActions.CreateSaveAndValidateWorkFlow(properties, newWorkflow, false);

                ActivityAssemblyItem[] assemblyItems = 
            {   
                    TestInputs.ActivityAssemblyItems.TestInput_Lib3,
                    TestInputs.ActivityAssemblyItems.TestInput_Lib2,
                    TestInputs.ActivityAssemblyItems.TestInput_Lib1,
                    
            };
                using (var cache = new CachingIsolator(assemblyItems))

                using (var clientStub = new Implementation<WorkflowsQueryServiceClient>()) // Stub out service proxy.            
                {
                    // Create fake reply.
                    var fakeReply = new GetAllActivityLibrariesReplyDC() { Errorcode = 10, ErrorGuid = null, ErrorMessage = "Something failed" };
                    fakeReply.List = new List<ActivityLibraryDC>();

                    // Add non cached assemblies to fake reply.
                    var assembly1Name = newWorkflow.Name;
                    var assembly1Version = newWorkflow.Version;
                    var library = new ActivityLibraryDC { Name = assembly1Name, VersionNumber = assembly1Version, Executable = new byte[5] };
                    fakeReply.List.Add(library);

                    clientStub.Register(inst => inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any))
                        .Return(fakeReply);

                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => clientStub.Instance;

                    //Add non cached assemblies to fake reply.
                    var vm = new SelectAssemblyAndActivityViewModel();

                    //3 cached assemblies + 2 non-cached assemblies returned from service should make a total of 5.
                    foreach (ActivityAssemblyItem aitem in assemblyItems)
                    {
                        Assert.IsTrue(vm.ActivityAssemblyItemCollection.Contains(aitem));
                    }

                    // The ActivityAssemblyItemCollection should contain the assemblies returned from the service.
                    List<ActivityAssemblyItem> items = new List<ActivityAssemblyItem>(vm.ActivityAssemblyItemCollection);

                    // Assert that the remaining assemblies in ActivityAssemblyItemCollection are the cached assemblies.
                    AssertPresenceOfCachedAssemblies(items);
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                }
            });
        }


        /// <summary>
        /// Asserts whether the cached assemblies defined in the CachingIsolator are included in 
        /// a given list of ActivityAssemblyItem.
        /// </summary>
        /// <param name="items">List of ActivityAssemblyItem.</param>
        private static void AssertPresenceOfCachedAssemblies(List<ActivityAssemblyItem> items)
        {
            ActivityAssemblyItem item = items.Find(match =>
                match.Matches(TestInputs.Assemblies.TestInput_Lib1.GetName()));
            Assert.IsNotNull(item, "Cached assembly TestInput_Library1 should appear in the returned collection.");

            item = items.Find(match =>
                match.Matches(TestInputs.Assemblies.TestInput_Lib2.GetName()));
            Assert.IsNotNull(item, "Cached assembly TestInput_Library2 should appear in the returned collection.");

            item = items.Find(match =>
                match.Matches(TestInputs.Assemblies.TestInput_Lib3.GetName()));
            Assert.IsNotNull(item, "Cached assembly TestInput_Library3 should appear in the returned collection.");
        }

        [Description("Verify that when GetActivityItemsByActivityAssemblyItem method is called, the ActivityAssemblyItem gets added with the new activity returned from the service. ")]
        [Owner("v-sanja")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void SelectAssembly_ActivityItemIsAddedToActivityAssemblyItemWhenGetActivityItemsByActivityAssemblyItemCalled()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (new CachingIsolator()) // No assemblies in cache.
                using (var clientStub = new Implementation<WorkflowsQueryServiceClient>()) // Stub out service proxy.
                {

                    // Fake the service to return one assembly for GetAllActivityLibraries call.
                    clientStub.Register(inst => inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any))
                        .Return(new GetAllActivityLibrariesReplyDC
                        {
                            Errorcode = 0,
                            ErrorGuid = null,
                            ErrorMessage = string.Empty,
                            List = new List<ActivityLibraryDC>() 
                        { 
                            new ActivityLibraryDC { 
                                Name = "New_Assembly1", 
                                VersionNumber = "2.0.0.0", 
                                Executable = new byte[5], 
                                HasExecutable=true,
                                Environment= "dev",
                            } 
                        }
                        });

                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => clientStub.Instance;

                    var vm = new SelectAssemblyAndActivityViewModel();
                    var fakeActivitiesReply = new List<StoreActivitiesDC>();
                    var activityName = "Activity 1";
                    var activityVersion = "1.0.0.0";
                    fakeActivitiesReply.Add(new StoreActivitiesDC
                    {
                        ActivityLibraryName = "TestInput_Lib1",
                        ActivityLibraryVersion = "1.0.0.1",
                        ActivityCategoryName = "Custom Activities",
                        AuthGroupName = "Auth Group 1",
                        Name = activityName,
                        Version = activityVersion,
                        Environment = "dev",
                    });

                    clientStub.Register(inst =>
                       inst.GetActivitiesByActivityLibraryNameAndVersion(Argument<GetActivitiesByActivityLibraryNameAndVersionRequestDC>.Any))
                       .Return(new GetActivitiesByActivityLibraryNameAndVersionReplyDC
                       {
                           StatusReply = new StatusReplyDC { Errorcode = 0, ErrorGuid = null, ErrorMessage = string.Empty },
                           List = fakeActivitiesReply
                       });

                    Assert.IsTrue(vm.ActivityAssemblyItemCollection[0].ActivityItems.Count == 0, "No activities should be available before calling GetActivityItemsByActivityAssemblyItem().");

                    vm.GetActivityItemsByActivityAssemblyItem(clientStub.Instance, vm.ActivityAssemblyItemCollection[0]);

                    Assert.IsTrue(vm.ActivityAssemblyItemCollection[0].ActivityItems.Count == 1, "Only one activity should be available after calling GetActivityItemsByActivityAssemblyItem() since the GetActivitiesByActivityLibraryNameAndVersion service call returns only one activity.");
                    Assert.AreEqual(activityName, vm.ActivityAssemblyItemCollection[0].ActivityItems[0].Name);
                    Assert.AreEqual(activityVersion, vm.ActivityAssemblyItemCollection[0].ActivityItems[0].Version);

                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                }
            });
        }
    }
}
