using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AuthoringToolTests.Services;
using CWF.DataContracts;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Authoring.Tests.Functional
{
    [TestClass]
    public class SelectAssemblyAndActivityViewModelFunctionalTest
    {
        #region constants

        private const string ACTIVITYLIBNAME1 = "activityLib1";
        private const string ACTIVITYLIBNAME2 = "activityLib2";
        private const string VERSION1 = "1.0.0.0";
        private const string VERSION2 = "1.0.0.0";

        #endregion

        #region test methods

        [WorkItem(24445)]
        [Description("Test the Select function with mocked data")]
        [Owner("v-ertang")]
        [TestCategory("Func-Dif-Full")]
        [TestMethod()]
        [Ignore]
        public void VerifySelectForMockedData()
        {
            // This test method uses the Dynamic Implementation Framework to mock data and then loads it with SelectAssemblyAndActivityViewModel
            using (new CachingIsolator())
            using (var clientStub = new Implementation<WorkflowsQueryServiceClient>())
            {
                string executingAssemblyPath = TestUtilities.GetExecutingAssemblyPath();
                ActivityAssemblyItem executingActivityAssemblyItem = new ActivityAssemblyItem(Assembly.LoadFrom((executingAssemblyPath)));

                List<ActivityLibraryDC> activityLibraryList = new List<ActivityLibraryDC>
                        {
                                new ActivityLibraryDC{ Name = ACTIVITYLIBNAME1, VersionNumber = VERSION1, Executable = new byte[] { } },
                                new ActivityLibraryDC{ Name = ACTIVITYLIBNAME2, VersionNumber = VERSION2, Executable = null },
                                DataContractTranslator.AssemblyItemToActivityLibraryDataContract(executingActivityAssemblyItem)
                        };

                clientStub.Register(inst =>
                    inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any))
                    .Return(new GetAllActivityLibrariesReplyDC
                    {
                        Errorcode = 0,
                        List = activityLibraryList
                    });

                clientStub.Register(inst => inst.GetActivitiesByActivityLibraryNameAndVersion(Argument<GetActivitiesByActivityLibraryNameAndVersionRequestDC>.Any))
                    .Return(new GetActivitiesByActivityLibraryNameAndVersionReplyDC { List = new List<StoreActivitiesDC>() });

                // For OKCommand.Execute
                clientStub.Register(db => db.StoreActivityLibraryDependenciesTreeGet(Argument<StoreActivityLibrariesDependenciesDC>.Any))
                   .Return(new List<StoreActivityLibrariesDependenciesDC>());

                // For OKCommand.Execute
                clientStub.Register(db => db.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any))
                    .Return(new List<ActivityLibraryDC>());

                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => clientStub.Instance;
                var vmSelect = new SelectAssemblyAndActivityViewModel();
               
                Assert.AreEqual(activityLibraryList.Count, vmSelect.ActivityAssemblyItemCollection.Count);
                // check that the names in the select list are in the correct order
                Assert.AreEqual(ACTIVITYLIBNAME1, vmSelect.ActivityAssemblyItemCollection[0].DisplayName);
                Assert.AreEqual(ACTIVITYLIBNAME2, vmSelect.ActivityAssemblyItemCollection[1].DisplayName);
                string assemblyname = Path.GetFileNameWithoutExtension(executingAssemblyPath);
                Assert.AreEqual(assemblyname, vmSelect.ActivityAssemblyItemCollection[2].Name);
                Assert.AreEqual(VERSION1, vmSelect.ActivityAssemblyItemCollection[0].Version);
                Assert.AreEqual(VERSION2, vmSelect.ActivityAssemblyItemCollection[1].Version);
                Assert.AreEqual(executingActivityAssemblyItem.Version, vmSelect.ActivityAssemblyItemCollection[2].Version);

                // on the select window, the following lines will click get, check the select box, and click ok
                vmSelect.GetActivityItemsByActivityAssemblyItem(clientStub.Instance, vmSelect.ActivityAssemblyItemCollection[0]); // get
                vmSelect.GetActivityItemsByActivityAssemblyItem(clientStub.Instance, vmSelect.ActivityAssemblyItemCollection[1]);
                vmSelect.ActivityAssemblyItemCollection[0].UserSelected = true;
                vmSelect.OkCommandExecute(clientStub.Instance);
            }
            WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
        }

        [WorkItem(16289)]
        [Description("Test the Select function with data from database")]
        [Owner("v-ertang")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void VerifySelectForDatabase()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmSelect = new SelectAssemblyAndActivityViewModel();
                WorkflowsQueryServiceUtility.UsingClient(client =>
                    vmSelect.LoadLiveData(client)); // select from real database
                Assert.AreNotEqual(0, vmSelect.ActivityAssemblyItemCollection.Count);
                // on the select window, the following lines will check the select box and click ok
                vmSelect.ActivityAssemblyItemCollection[0].UserSelected = true;
                vmSelect.OkCommand.Execute();
            }
        }

        [WorkItem(24530)]
        [Description("VerifyThatSelectPullsDownDependencies")]
        [Owner("v-ertang")]
        [TestCategory("Func-Dif-Full")]
        [TestMethod()]
        [Ignore]
        public void VerifySelectPullsDownDependencies()
        {
            // We will select activityLibrary3, which is dependent on activityLibrary2, which in turn is dependent on activityLibrary1.
            // This test confirms that the caching contains all 3.
            var activityLibrary1 = TestInputs.ActivityAssemblyItems.TestInput_Lib1;
            var activityLibrary2 = TestInputs.ActivityAssemblyItems.TestInput_Lib2;
            var activityLibrary3 = TestInputs.ActivityAssemblyItems.TestInput_Lib3;

            var activityLibrary1Actual = new ActivityLibraryDC { Id = 1, Name = activityLibrary1.Name, VersionNumber = activityLibrary1.Version.ToString(), Executable = File.ReadAllBytes(activityLibrary1.Location) };
            var activityLibrary2Actual = new ActivityLibraryDC { Id = 2, Name = activityLibrary2.Name, VersionNumber = activityLibrary2.Version.ToString(), Executable = File.ReadAllBytes(activityLibrary2.Location) };
            var activityLibrary3Actual = new ActivityLibraryDC { Id = 3, Name = activityLibrary3.Name, VersionNumber = activityLibrary3.Version.ToString(), Executable = File.ReadAllBytes(activityLibrary3.Location) };

            using (var cache = new CachingIsolator())
            using (var mock = new Implementation<WorkflowsQueryServiceClient>())
            {
                // upload workflows in order of dependency: activityLibrary1 activityLibrary2 activityLibrary3
                mock.Register(db => db.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any))
                    .Return(new GetAllActivityLibrariesReplyDC()
                    {
                        List = new List<ActivityLibraryDC>
                        {
                            activityLibrary1Actual,
                            activityLibrary2Actual,
                            activityLibrary3Actual,
                        }
                    });

                mock.Register(db => db.StoreActivityLibraryDependenciesTreeGet(Argument<StoreActivityLibrariesDependenciesDC>.Any))
                    .Execute((StoreActivityLibrariesDependenciesDC arg) =>
                        {
                            if (arg.StoreDependenciesRootActiveLibrary.activityLibraryName == activityLibrary3.Name)
                                return new List<StoreActivityLibrariesDependenciesDC> 
                                    {
                                        new StoreActivityLibrariesDependenciesDC
                                        {
                                            StoreDependenciesDependentActiveLibraryList = new List<StoreDependenciesDependentActiveLibrary> 
                                            {
                                                new StoreDependenciesDependentActiveLibrary
                                                {
                                                    activityLibraryDependentId = 2,
                                                    activityLibraryParentId = 3
                                                },
                                                new StoreDependenciesDependentActiveLibrary
                                                {
                                                    activityLibraryDependentId = 1,
                                                    activityLibraryParentId = 2
                                                }
                                            }
                                        }
                                    };
                            else
                                return new List<StoreActivityLibrariesDependenciesDC>();
                        });

                mock.Register(db => db.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any))
                    .Execute((ActivityLibraryDC request) =>
                        {
                            if (request.Name == activityLibrary1.Name)
                                return new List<ActivityLibraryDC> { activityLibrary1Actual };
                            else if (request.Name == activityLibrary2.Name)
                                return new List<ActivityLibraryDC> { activityLibrary2Actual };
                            else if (request.Name == activityLibrary3.Name)
                                return new List<ActivityLibraryDC> { activityLibrary3Actual };
                            else return null;
                        });

                //  Assume right now that no libraries have activities
                mock.Register(db => db.GetActivitiesByActivityLibraryNameAndVersion(Argument<GetActivitiesByActivityLibraryNameAndVersionRequestDC>.Any))
                    .Execute((GetActivitiesByActivityLibraryNameAndVersionRequestDC request) =>
                        new GetActivitiesByActivityLibraryNameAndVersionReplyDC { List = new List<StoreActivitiesDC>() });

                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => mock.Instance;
                var vmSelect = new SelectAssemblyAndActivityViewModel();
                
                Assert.AreEqual(3, vmSelect.ActivityAssemblyItemCollection.Count);
                // Make sure that the 3rd item is the one with dependencies
                Assert.AreEqual(activityLibrary3.Name, vmSelect.ActivityAssemblyItemCollection[2].Name);
                // Select this item and click ok
                vmSelect.ActivityAssemblyItemCollection[2].UserSelected = true;
                vmSelect.OkCommandExecute(mock.Instance);

                // Confirming that just these 3 libraries were pulled down
                Assert.AreEqual(3, Caching.ActivityAssemblyItems.Count);

                // Confirming that the cache contains these 3 libraries
                Assert.IsTrue(Caching.ActivityAssemblyItems.Any(a => a.Name == activityLibrary1.Name));
                Assert.IsTrue(Caching.ActivityAssemblyItems.Any(a => a.Name == activityLibrary2.Name));
                Assert.IsTrue(Caching.ActivityAssemblyItems.Any(a => a.Name == activityLibrary3.Name));
            }
            WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
        }

        [WorkItem(26066)]
        [Description("Test the Select function with mocked data that unselecting will take it out of the cache")]
        [Owner("v-ertang")]
        [TestCategory("Func-Dif-Full")]
        [TestMethod()]
        [Ignore]
        public void VerifyUnselectingWillRemoveFromCache()
        {
            // This test method uses the Dynamic Implementation Framework to mock data and then loads it with SelectAssemblyAndActivityViewModel.
            // It will check that unselecting will take it out of the cache
            try
            {
                var activityLibrary1 = TestInputs.ActivityAssemblyItems.TestInput_Lib1;

                var activityLibrary1Actual = new ActivityLibraryDC { Id = 1, Name = activityLibrary1.Name, VersionNumber = activityLibrary1.Version.ToString(), Executable = File.ReadAllBytes(activityLibrary1.Location) };

                using (var cache = new CachingIsolator())
                using (var mock = new Implementation<WorkflowsQueryServiceClient>())
                {
                    mock.Register(db => db.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any))
                        .Return(new GetAllActivityLibrariesReplyDC()
                        {
                            List = new List<ActivityLibraryDC>
                        {
                            activityLibrary1Actual
                        }
                        });

                    mock.Register(db => db.StoreActivityLibraryDependenciesTreeGet(Argument<StoreActivityLibrariesDependenciesDC>.Any))
                        .Execute((StoreActivityLibrariesDependenciesDC arg) =>
                        {
                            return new List<StoreActivityLibrariesDependenciesDC>();
                        });

                    mock.Register(db => db.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any))
                        .Execute((ActivityLibraryDC request) =>
                        {
                            if (request.Name == activityLibrary1.Name)
                                return new List<ActivityLibraryDC> { activityLibrary1Actual };
                            else return null;
                        });

                    //  Assume right now that no libraries have activities
                    mock.Register(db => db.GetActivitiesByActivityLibraryNameAndVersion(Argument<GetActivitiesByActivityLibraryNameAndVersionRequestDC>.Any))
                        .Execute((GetActivitiesByActivityLibraryNameAndVersionRequestDC request) =>
                            new GetActivitiesByActivityLibraryNameAndVersionReplyDC { List = new List<StoreActivitiesDC>() });

                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => mock.Instance;
                    var vmSelect = new SelectAssemblyAndActivityViewModel();
                   
                    Assert.AreEqual(1, vmSelect.ActivityAssemblyItemCollection.Count);
                    // Unselect this item and click ok
                    vmSelect.ActivityAssemblyItemCollection[0].UserSelected = false;
                    vmSelect.OkCommandExecute(mock.Instance);
                    // Confirm that nothing was pulled down
                    Assert.AreEqual(0, Caching.ActivityAssemblyItems.Count);
                }
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
            }
            catch (Exception ex)
            {
                Assert.Fail("Verifying Select for mocked data failed with this exception: {0}", ex.Message);
            }
        }
        
        #endregion
    }
}
