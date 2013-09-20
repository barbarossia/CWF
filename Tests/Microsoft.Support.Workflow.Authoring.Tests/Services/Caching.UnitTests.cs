using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using System.Reflection;
using CWF.DataContracts;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.Support.Workflow.Authoring.Common;
using System.IO;
using System.Activities;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.Services;
using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;
using System.ServiceModel;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns;

namespace AuthoringToolTests.Services
{
    [TestClass]
    public class CachingUnitTests
    {

        [TestInitialize()]
        public void Initialize()
        {
            Caching.ActivityAssemblyItems.Clear();
            Caching.ActivityItems.Clear();
        }

        [WorkItem(323454)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Caching_TestDownloadAssemblies()
        {
            using (new CachingIsolator())
            {
                List<ActivityAssemblyItem> items = new List<ActivityAssemblyItem>() { TestInputs.ActivityAssemblyItems.TestInput_Lib1 };
                Caching.CacheAssembly(items);
                Assert.AreEqual(0, Caching.DownloadAssemblies(null, items).Count);
            }
        }

        [WorkItem(323454)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Caching_TestCacheAssembliesExceptions()
        {
            List<ActivityAssemblyItem> items;
            using (new CachingIsolator())
            {
                items = null;
                TestUtilities.Assert_ShouldThrow<ArgumentNullException>(() =>
                {
                    Caching.CacheAssembly(items);
                });

                items = new List<ActivityAssemblyItem>() { null };
                TestUtilities.Assert_ShouldThrow<ArgumentNullException>(() =>
                {
                    Caching.CacheAssembly(items);
                });
            }
        }

        [WorkItem(323454)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        public void Caching_TestComputeDependenciesExceptions()
        {
            using (var client = new Mock<IWorkflowsQueryService>())
            {
                client.Expect(c => c.GetAllActivityLibraries(null))
                    .Where(request => request != null)
                    .Throw(new FaultException<ServiceFault>(new ServiceFault()));
                client.Expect(c => c.GetAllActivityLibraries(null))
                    .Where(request => request != null)
                    .Throw(new FaultException<ValidationFault>(new ValidationFault()));
                client.Expect(c => c.GetAllActivityLibraries(null))
                    .Where(request => request != null)
                    .Throw(new Exception());

                TestUtilities.Assert_ShouldThrow<CommunicationException>(() =>
                {
                    Caching.ComputeDependencies(client.Instance, new List<ActivityAssemblyItem>());
                });
                TestUtilities.Assert_ShouldThrow<BusinessValidationException>(() =>
                {
                    Caching.ComputeDependencies(client.Instance, new List<ActivityAssemblyItem>());
                });
                TestUtilities.Assert_ShouldThrow<CommunicationException>(() =>
                {
                    Caching.ComputeDependencies(client.Instance, new List<ActivityAssemblyItem>());
                });
                client.Verify();
            }
        }

        [WorkItem(323454)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        public void Caching_TestGetExecutableBytesExceptions()
        {
            using (var client = new Mock<IWorkflowsQueryService>())
            {
                client.Expect(c => c.ActivityLibraryGet(null))
                    .Where(request => request != null)
                    .Throw(new FaultException<ServiceFault>(new ServiceFault()));
                client.Expect(c => c.ActivityLibraryGet(null))
                    .Where(request => request != null)
                    .Throw(new FaultException<ValidationFault>(new ValidationFault()));
                client.Expect(c => c.ActivityLibraryGet(null))
                    .Where(request => request != null)
                    .Throw(new Exception());

                TestUtilities.Assert_ShouldThrow<Exception>(() =>
                {
                    Caching.GetExecutableBytes(client.Instance, null);
                });
                TestUtilities.Assert_ShouldThrow<CommunicationException>(() =>
                {
                    Caching.GetExecutableBytes(client.Instance, TestInputs.ActivityAssemblyItems.TestInput_Lib1);
                });
                TestUtilities.Assert_ShouldThrow<BusinessValidationException>(() =>
                {
                    Caching.GetExecutableBytes(client.Instance, TestInputs.ActivityAssemblyItems.TestInput_Lib1);
                });
                TestUtilities.Assert_ShouldThrow<CommunicationException>(() =>
                {
                    Caching.GetExecutableBytes(client.Instance, TestInputs.ActivityAssemblyItems.TestInput_Lib1);
                });
                client.Verify();
            }
        }

        [Description("Verify branches of TestCacheAssemblies")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void Caching_CacheAssemblies_ReplacesNoncachedVersions()
        {
            var cachedVersion = TestInputs.ActivityAssemblyItems.TestInput_Lib1;
            var cachedSentinel = "arbitraryString"; // CacheAssembly flushes through serialization so we can't compare equality directly--need to recognize sentinel flag
            cachedVersion.Status = cachedSentinel;
            // Arrange initial cache
            using (new CachingIsolator(cachedVersion))
            {
                var newVersion = TestInputs.ActivityAssemblyItems.TestInput_Lib1;
                var newSentinel = "anotherArbitraryString";
                newVersion.Status = newSentinel;
                Assert.AreNotSame(newVersion, Caching.ActivityAssemblyItems[0]);
                Assert.AreEqual(1, Caching.ActivityAssemblyItems.Count);
                Assert.AreEqual(cachedSentinel, Caching.ActivityAssemblyItems[0].Status);

                // Act
                newVersion.CachingStatus = CachingStatus.Latest; // Cached: should not overwrite existing version
                Caching.CacheAssembly(new[] { newVersion }.ToList());
                // Assert
                Assert.AreEqual(1, Caching.ActivityAssemblyItems.Count);
                Assert.AreEqual(cachedSentinel, Caching.ActivityAssemblyItems[0].Status);

                // Act
                newVersion.CachingStatus = CachingStatus.None; // Not cached: should overwrite existing version
                Caching.CacheAssembly(new[] { newVersion }.ToList());
                // Assert
                Assert.AreEqual(1, Caching.ActivityAssemblyItems.Count);
                Assert.AreEqual(newSentinel, Caching.ActivityAssemblyItems[0].Status);
            }
        }

        [Description("Verify that cached assemblies' Location and AssemblyName do not point to the original location")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void Caching_CacheAssemblies_UpdatesLocationAndAssemblyName()
        {
            // Arrange
            using (new CachingIsolator())
            {
                var precache = TestInputs.ActivityAssemblyItems.TestInput_Lib1;
                Assert.AreEqual(TestInputs.Assemblies.TestInput_Lib1.Location, Path.GetFullPath(precache.Location));

                // Act
                Caching.CacheAssembly(new[] { TestInputs.ActivityAssemblyItems.TestInput_Lib1 }.ToList());
                Assert.AreEqual(1, Caching.ActivityAssemblyItems.Count);

                // Assert
                Assert.AreEqual(1, Caching.ActivityAssemblyItems.Count);
                var cached = Caching.ActivityAssemblyItems[0];
                // AssemblyName and Location should NOT point to original location of imported .dll or we won't be isolated from changes to it
                Assert.AreNotEqual(TestInputs.Assemblies.TestInput_Lib1.Location, Path.GetFullPath(cached.Location));
                Assert.IsNull(cached.AssemblyName.CodeBase, "AssemblyName.CodeBase should have been reset to null when library was cached");
            }
        }

        [Description("Verify that Caching.Load() resets Caching.ActivityItems based on Caching.ActivityAssemblyItems")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void Caching_ReloadsActivities()
        {
            using (new CachingIsolator())
            {
                // Arrange
                Caching.CacheAssembly(new[] { // Cache and serialize out to disk 
                    TestInputs.ActivityAssemblyItems.TestInput_Lib1, 
                    TestInputs.ActivityAssemblyItems.TestInput_Lib2 
                }.ToList());
                Caching.Refresh();
                // Check test pre-condition--if this fails then the problem is elsewhere (i.e. arrange step failed)
                Assert.AreEqual(2, Caching.ActivityAssemblyItems.Count);
                Assert.AreEqual(2, Caching.ActivityItems.Count);

                // Act
                Caching.ActivityAssemblyItems.Clear();
                Caching.ActivityItems.Clear();
                Caching.LoadFromLocal(); // restore from disk

                // Assert that it got restored from disk
                Assert.AreEqual(2, Caching.ActivityAssemblyItems.Count);
                Assert.AreEqual(2, Caching.ActivityItems.Count);
            }
        }

        [Description("Verify Caching.TryGet() keys off of assembly Name and Version-iff-signed")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void Caching_TryGet_IsBasedOnNameAndVersion()
        {
            var lib1 = TestInputs.ActivityAssemblyItems.TestInput_Lib1;
            lib1.Version = null; // pretend it's unsigned
            using (new CachingIsolator(lib1))
            {
                ActivityAssemblyItem ignore;
                // trivially wrong, wrong name
                Assert.IsFalse(Caching.LoadCachedAssembly(new AssemblyName(), out ignore));
                // has name and no version = match
                Assert.IsTrue(Caching.LoadCachedAssembly(new AssemblyName(TestInputs.Assemblies.TestInput_Lib1.GetName().Name), out ignore));
                // has name and unsigned version = match
                Assert.IsTrue(
                    Caching.LoadCachedAssembly(
                        new AssemblyName()
                        {
                            Name = TestInputs.Assemblies.TestInput_Lib1.GetName().Name,
                            Version = TestInputs.Assemblies.TestInput_Lib1.GetName().Version
                        },
                        out ignore));
                // wrong name = no match
                Assert.IsFalse(Caching.LoadCachedAssembly(TestInputs.Assemblies.TestInput_Lib2.GetName(), out ignore));
                // right name, signed version = no match since we are pretending the cached version is unsigned
                Assert.IsFalse(Caching.LoadCachedAssembly(TestInputs.Assemblies.TestInput_Lib1.GetName(), out ignore));
            }
        }

        [Description("Verify Caching's ComputeDependencies() functionality.")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Caching_TestComputeDependencies()
        {
            // Arrange
            var lib3 = TestInputs.ActivityAssemblyItems.TestInput_Lib3;
            var lib2 = TestInputs.ActivityAssemblyItems.TestInput_Lib2;
            var lib1 = TestInputs.ActivityAssemblyItems.TestInput_Lib1;
            lib1.ReferencedAssemblies.Add(new AssemblyName()); // references should be computed based on StoreActivityLibraryDependenciesTreeGet
            lib2.ReferencedAssemblies.Add(new AssemblyName()); // so we clear this, to make sure we're not cheating anywhere
            lib3.ReferencedAssemblies.Add(new AssemblyName());
            lib1.Env = Microsoft.Support.Workflow.Authoring.AddIns.Data.Env.Dev;
            lib2.Env = Microsoft.Support.Workflow.Authoring.AddIns.Data.Env.Dev;
            lib3.Env = Microsoft.Support.Workflow.Authoring.AddIns.Data.Env.Dev;

            var stubClient = new Implementation<IWorkflowsQueryService>();
            stubClient.Register(inst => inst.StoreActivityLibraryDependenciesTreeGet(Argument<StoreActivityLibrariesDependenciesDC>.Any))
                .Return(new List<StoreActivityLibrariesDependenciesDC>
                {
                    new StoreActivityLibrariesDependenciesDC
                    {
                        StoreDependenciesDependentActiveLibraryList =
                            new List<StoreDependenciesDependentActiveLibrary>
                            {
                                new StoreDependenciesDependentActiveLibrary
                                {
                                    activityLibraryParentId = 3,
                                    activityLibraryDependentId = 2 // TestInput_Library3 depends on TestInput_Library2 
                                },
                                new StoreDependenciesDependentActiveLibrary
                                {
                                    activityLibraryParentId = 2,
                                    activityLibraryDependentId = 1 // TestInput_Library2 depends on TestInput_Library1
                                }
                            }
                    }
                });

            stubClient.Register(inst => inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any))
                .Return(new GetAllActivityLibrariesReplyDC
                {
                    Errorcode = 0,
                    List = new List<ActivityLibraryDC> {
                        // Dependencies have no runtime representation, only a DB representation in terms of IDs. 
                        // We need to give each library an ID in order to decode dependencies.                        
                        new ActivityLibraryDC {
                            Id = 3, Name = lib3.Name, VersionNumber = lib3.Version.ToString(),Environment="dev"
                        },
                        new ActivityLibraryDC {
                            Id = 2, Name = lib2.Name, VersionNumber = lib2.Version.ToString(),Environment="dev"
                        },
                        new ActivityLibraryDC {
                            Id = 1, Name = lib1.Name, VersionNumber = lib1.Version.ToString(),Environment="dev"
                        },
                        // Something else just to make sure it doesn't get downloaded
                        new ActivityLibraryDC {
                            Id = 4, Name = "SomeOtherLibrary", VersionNumber = "1.0.0.0",Environment="dev"
                        }
                    }
                });
            var client = stubClient.Instance;

            using (new CachingIsolator(lib1))
            using (stubClient)
            {
                // Act
                var deps = Caching.ComputeDependencies(client, TestInputs.ActivityAssemblyItems.TestInput_Lib3);

                // Assert
                ActivityAssemblyItem[] assemblyItems = 
                {   
                    TestInputs.ActivityAssemblyItems.TestInput_Lib3,
                    TestInputs.ActivityAssemblyItems.TestInput_Lib2,
                    TestInputs.ActivityAssemblyItems.TestInput_Lib1
                };

                foreach (ActivityAssemblyItem item in deps)
                {
                    Assert.IsTrue(deps.Contains(item));
                }
            };
        }

        [Description("Verify that GetExecutableBytes() doesn't choke if the web service returns null")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Caching_GetExecutableBytes_ReturnsNullForNullDataContract()
        {
            using (var mock = new Mock<IWorkflowsQueryService>())
            {
                mock.Register(inst => inst.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any))
                    .Return(null);
                Assert.IsNull(Caching.GetExecutableBytes(mock.Instance, TestInputs.ActivityAssemblyItems.TestInput_Lib1));
            }
        }

        [Description("Verify that GetExecutableBytes() returns whatever bytes are supplied by web service")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Caching_GetExecutableBytes_GetsBytesIfTheyExist()
        {
            var bytes = new byte[10];
            using (var mock = new Mock<IWorkflowsQueryService>())
            {
                mock.Register(inst => inst.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any))
                    .Return(new List<ActivityLibraryDC> 
                    { 
                        new ActivityLibraryDC { Executable = bytes },
                        new ActivityLibraryDC { Executable = new byte[10] }
                    });
                Assert.AreSame(bytes, Caching.GetExecutableBytes(mock.Instance, TestInputs.ActivityAssemblyItems.TestInput_Lib1));
            }
        }

        [Description("Verify that GetExecutableBytes() doesn't throw on null executable")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Caching_GetExecutableBytes_ReturnsNullForNullExecutable()
        {
            var bytes = new byte[10];
            using (var mock = new Mock<IWorkflowsQueryService>())
            {
                mock.Register(inst => inst.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any))
                    .Return(new List<ActivityLibraryDC> 
                    { 
                        new ActivityLibraryDC { Executable = null }, // same as new ActivityLibraryDC(), we're just being explicit for test readability
                        new ActivityLibraryDC { Executable = new byte[10] }
                    });
                Assert.IsNull(Caching.GetExecutableBytes(mock.Instance, TestInputs.ActivityAssemblyItems.TestInput_Lib1));
            }
        }

        [Description("Verify that GetExecutableBytes() doesn't throw on empty list")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Caching_GetExecutableBytes_TranslatesEmptyListToNull()
        {
            using (var mock = new Mock<IWorkflowsQueryService>())
            {
                mock.Register(inst => inst.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any))
                    .Return(new List<ActivityLibraryDC>());
                Assert.IsNull(Caching.GetExecutableBytes(mock.Instance, TestInputs.ActivityAssemblyItems.TestInput_Lib1));
            }
        }

        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Caching_TestDownloadAssembliesNotNull()
        {
            var lib1 = TestInputs.ActivityAssemblyItems.TestInput_Lib1;
            var download = new List<ActivityAssemblyItem>(){lib1};
            var bytes = File.ReadAllBytes(lib1.Location);
            using (var client = new Mock<IWorkflowsQueryService>())
            {
                client.Register(inst => inst.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any))
                    .Return(new List<ActivityLibraryDC> 
                    { 
                        new ActivityLibraryDC { Executable = bytes }
                    });

                var result = Caching.DownloadAssemblies(client.Instance, download);

                Assert.AreEqual(1, result.Count);

            }
        }

        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Caching_TestDownloadAndCacheAssembly()
        {
            var lib1 = TestInputs.ActivityAssemblyItems.TestInput_Lib1;
            var download = new List<ActivityAssemblyItem>() { lib1 };
            var bytes = File.ReadAllBytes(lib1.Location);
            Caching.ActivityAssemblyItems.Clear();
            Caching.ActivityItems.Clear();

            using (var client = new Mock<IWorkflowsQueryService>())
            {
                client.Register(inst => inst.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any))
                    .Return(new List<ActivityLibraryDC> 
                    { 
                        new ActivityLibraryDC { Executable = bytes }
                    });

                Caching.DownloadAndCacheAssembly(client.Instance, download);

                Assert.AreEqual(1, Caching.ActivityAssemblyItems.Count);
            }
        }

        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Caching_TestCacheAndDownloadAssembly()
        {
            var lib1 = TestInputs.ActivityAssemblyItems.TestInput_Lib1;
            var download = new List<ActivityAssemblyItem>() { lib1 };
            var bytes = File.ReadAllBytes(lib1.Location);
            Caching.ActivityAssemblyItems.Clear();
            Caching.ActivityItems.Clear();

            using (var client = new Mock<IWorkflowsQueryService>())
            {
                client.Register(inst => inst.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any))
                    .Return(new List<ActivityLibraryDC> 
                    { 
                        new ActivityLibraryDC { Executable = bytes }
                    });

                var result = Caching.CacheAndDownloadAssembly(client.Instance, download);

                Assert.AreEqual(1, result.Count);
            }
        }

        [TestCleanup]
        public void TestCleanup() { System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); }
    }
}
