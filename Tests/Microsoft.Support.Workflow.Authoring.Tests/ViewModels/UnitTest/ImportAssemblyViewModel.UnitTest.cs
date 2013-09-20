using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Text;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Windows;
using System;
using AuthoringToolTests.Services;
using System.Collections.ObjectModel;
using Microsoft.Support.Workflow.Authoring.Common;
using System.Collections.Generic;
using Microsoft.Support.Workflow.Authoring;
using System.Activities;
using System.Linq;
using Microsoft.Support.Workflow.Authoring.Tests.Services;
using System.Diagnostics;
using System.IO;
using Microsoft.DynamicImplementations;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;
using System.Windows.Threading;
using Microsoft.Support.Workflow.Authoring.AssetStore;
using System.Windows.Data;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
namespace Authoring.Tests.Unit
{
    [TestClass]
    public class ImportAssemblyViewModelUnitTests
    {

        private const string ANY_TEST_STRING = "any test string";
        private ActivityAssemblyItem testLib = TestInputs.ActivityAssemblyItems.TestInput_Lib1;
        private ActivityAssemblyItem testLib1 = TestInputs.ActivityAssemblyItems.TestInput_Lib2;
        public ImportAssemblyViewModelUnitTests()
        {
            // suppress message boxes from popping up
            MessageBoxService.ShowFunc = (msg, b, c, d, e) => { throw new AssertFailedException(msg); };
        }

        [WorkItem(325753)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason1")]
        [TestMethod()]
        public void Import_VerifyInspectAssembly()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
             {
                 using (new CachingIsolator())
                 {

                     using (var service1 = new Implementation<AssemblyInspectionService>())
                     {
                         service1.Register(inst => inst.Inspect(Argument<string>.Any)).Return(true);
                         service1.Register(inst => inst.SourceAssembly).Return(this.testLib);
                         service1.Instance.OperationException = null;

                         Utility.GetAssemblyInspectionService = () => service1.Instance;
                         using (var mock = new ImplementationOfType(typeof(Assembly)))
                         {
                             Assembly assembly1 = this.testLib.Assembly;
                             mock.Register(() => Assembly.ReflectionOnlyLoadFrom(this.testLib.Location)).Execute<string, Assembly>((s) =>
                             {
                                 return assembly1;
                             });
                             ImportAssemblyViewModel vm = new ImportAssemblyViewModel(this.testLib.Location);

                             //test location is empty
                             var mainAssembly = vm.AssembliesToImport[0];
                             try
                             {
                                 vm.UpdateReferenceLocation(vm.AssembliesToImport[0], string.Empty);
                             }
                             catch (ArgumentException expectException)
                             {
                                 Assert.AreEqual(expectException.ParamName, "assemblyPath");
                             }

                             try
                             {
                                 vm.InspectAssembly(string.Empty);
                             }
                             catch (ArgumentException expectException)
                             {
                                 Assert.AreEqual(expectException.ParamName, "assemblyPath");
                             }

                             using (var service = new ImplementationOfType(typeof(AssemblyInspectionService)))
                             {
                                 //check assembly path
                                 bool isCheck = false;
                                 service.Register(() => AssemblyInspectionService.CheckAssemblyPath(Argument<string>.Any)).Execute(() =>
                                 {
                                     isCheck = true;
                                 });

                                 //using (var boxService = new ImplementationOfType(typeof(MessageBoxService)))
                                 //{
                                 //    bool isAlreadyContains = false;
                                 //    boxService.Register(() => MessageBoxService.Show(Argument<string>.Any, Argument<string>.Any, MessageBoxButton.OK, MessageBoxImage.Information)).Execute(() =>
                                 //    {
                                 //        isAlreadyContains = true;
                                 //        return MessageBoxResult.None;
                                 //    });
                                 //    vm.InspectAssembly(vm.SelectedActivityAssemblyItem.Location);
                                 //    Assert.IsTrue(isCheck);
                                 //    Assert.IsTrue(isAlreadyContains);
                                 //}


                                 service1.Register(inst => inst.Inspect(Argument<string>.Any)).Return(false);
                                 service1.Instance.OperationException = new Exception("Expect Exception");
                                 Utility.GetAssemblyInspectionService = () => service1.Instance;
                                 try { vm.InspectAssembly(this.testLib1.Location); }
                                 catch (UserFacingException ex)
                                 {
                                     Assert.AreEqual(ex.Message, "Expect Exception");
                                 }

                                 service1.Register(inst => inst.Inspect(Argument<string>.Any)).Return(true);
                                 service1.Register(inst => inst.SourceAssembly).Return(this.testLib1);
                                 service1.Instance.OperationException = null;
                                 Utility.GetAssemblyInspectionService = () => service1.Instance;
                                 vm.InspectAssembly(this.testLib1.Location);

                                 vm.AssembliesToImport.Clear();
                                 mainAssembly.Location = string.Empty;
                                 vm.AssembliesToImport.Add(mainAssembly);

                                 HashSet<ActivityAssemblyItem> referencedAssemblies = new HashSet<ActivityAssemblyItem>();
                                 referencedAssemblies.Add(this.testLib);

                                 service1.Register(inst => inst.Inspect(Argument<string>.Any)).Return(true);
                                 service1.Register(inst => inst.SourceAssembly).Return(this.testLib1);
                                 service1.Register(inst => inst.ReferencedAssemblies).Return(referencedAssemblies.AsEnumerable());
                                 service1.Instance.OperationException = null;
                                 Utility.GetAssemblyInspectionService = () => service1.Instance;


                                 vm.InspectAssembly(this.testLib1.Location);
                                 Assert.AreEqual(vm.AssembliesToImport.Count(), 2);
                             }
                         }
                     }
                 }
                 Utility.GetAssemblyInspectionService = () => new AssemblyInspectionService();
             });
        }

        [WorkItem(325771)]
        [TestCategory("Unit-NoDif")]
        [Owner("v-kason1")]
        [TestMethod()]
        public void Import_VerifyValidateLocation()
        {
            bool result = ImportAssemblyViewModel.ValidateLocation(null, string.Empty, false);
            Assert.IsFalse(result);

            try { result = ImportAssemblyViewModel.ValidateLocation(null, string.Empty, true); }
            catch (ArgumentNullException ex)
            { Assert.AreEqual(ex.ParamName, "activityAssemblyItem"); }
        }

        [WorkItem(325727)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [TestMethod()]
        public void Import_MarkResolvedAssembly()
        {
            using (new CachingIsolator())
            {
                TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                {
                    using (var service1 = new Implementation<AssemblyInspectionService>())
                    {
                        service1.Register(inst => inst.Inspect(Argument<string>.Any)).Return(true);
                        service1.Register(inst => inst.SourceAssembly).Return(testLib);
                        service1.Instance.OperationException = null;

                        Utility.GetAssemblyInspectionService = () => service1.Instance;

                        using (var mock = new ImplementationOfType(typeof(Assembly)))
                        {
                            Assembly assembly1 = this.testLib.Assembly;
                            mock.Register(() => Assembly.ReflectionOnlyLoadFrom(this.testLib.Location)).Execute<string, Assembly>((s) =>
                            {
                                return assembly1;
                            });
                            ImportAssemblyViewModel vm = new ImportAssemblyViewModel(this.testLib.Location);
                            ActivityAssemblyItemViewModel newAssembly = new ActivityAssemblyItemViewModel(this.testLib1);
                            vm.AssembliesToImport.Add(newAssembly);
                            PrivateObject po = new PrivateObject(vm);
                            Assert.IsFalse(newAssembly.IsResolved);
                            po.Invoke("MarkResolvedAssembly", newAssembly);
                            Assert.IsTrue(newAssembly.IsResolved);
                        }
                        Utility.GetAssemblyInspectionService = () => new AssemblyInspectionService();
                    }
                });
            }
        }

        [WorkItem(325765)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason1")]
        [TestMethod()]
        public void Import_VerifyPropertyChanged()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
             {
                 using (new CachingIsolator())
                 {
                     using (var service1 = new Implementation<AssemblyInspectionService>())
                     {
                         service1.Register(inst => inst.Inspect(Argument<string>.Any)).Return(true);
                         service1.Register(inst => inst.SourceAssembly).Return(testLib);
                         service1.Instance.OperationException = null;

                         Utility.GetAssemblyInspectionService = () => service1.Instance;
                         using (var mock = new ImplementationOfType(typeof(Assembly)))
                         {
                             Assembly assembly1 = this.testLib.Assembly;
                             mock.Register(() => Assembly.ReflectionOnlyLoadFrom(this.testLib.Location)).Execute<string, Assembly>((s) =>
                             {
                                 return assembly1;
                             });
                             ImportAssemblyViewModel vm = new ImportAssemblyViewModel(this.testLib.Location);
                             TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "NewCategoryName", () => vm.NewCategoryName = "category");
                             Assert.AreEqual(vm.NewCategoryName, "category");
                             TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "IsAddingCategory", () => vm.IsAddingCategory = false);
                             Assert.AreEqual(vm.IsAddingCategory, false);
                             Assert.AreEqual(vm.SelectedActivityAssemblyItem.Name, this.testLib.Name);
                             Utility.GetAssemblyInspectionService = () => new AssemblyInspectionService();
                         }
                     }
                 }
             });
        }

        [WorkItem(325728)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [TestMethod()]
        public void Import_VerifyAddNewCategoryCommandExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (new CachingIsolator())
                {
                    using (var store = new ImplementationOfType(typeof(AssetStoreProxy)))
                    {
                        store.Register(() => AssetStoreProxy.GetActivityCategories()).Return(true);
                        store.Register(() => AssetStoreProxy.GetWorkflowTypes(Env.Test)).Return(true);
                        store.Register(() => AssetStoreProxy.ActivityCategoryCreateOrUpdate(Argument<string>.Any)).Return(true);
                        store.Register(() => AssetStoreProxy.ActivityCategories).Return(new CollectionViewSource());

                        store.Register(() => AssetStoreProxy.TenantName).Return("test");
                        using (var service1 = new Implementation<AssemblyInspectionService>())
                        {
                            service1.Register(inst => inst.Inspect(Argument<string>.Any)).Return(true);
                            service1.Register(inst => inst.SourceAssembly).Return(testLib);
                            service1.Instance.OperationException = null;

                            Utility.GetAssemblyInspectionService = () => service1.Instance;

                            ImportAssemblyViewModel vm = new ImportAssemblyViewModel(this.testLib.Location);

                            vm.NewCategoryName = string.Empty;
                            Assert.IsFalse(vm.CanAddNewCategoryCommandExecute());
                            vm.NewCategoryName = "a12345678901234567890123456789012345678901234567890";
                            Assert.IsFalse(vm.CanAddNewCategoryCommandExecute());
                            vm.NewCategoryName = "  ";
                            Assert.IsFalse(vm.CanAddNewCategoryCommandExecute());
                            vm.NewCategoryName = "???";
                            Assert.IsFalse(vm.CanAddNewCategoryCommandExecute());
                            vm.NewCategoryName = "tool";
                            Assert.IsTrue(vm.CanAddNewCategoryCommandExecute());
                            vm.IsAddingCategory = true;
                            vm.AddNewCategoryCommand.Execute();
                            Assert.IsFalse(vm.IsAddingCategory);
                            Utility.GetAssemblyInspectionService = () => new AssemblyInspectionService();
                        }
                    }
                }
            });
        }

        [WorkItem(325770)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason1")]
        [TestMethod()]
        public void Import_VerifyStartAndStopAddingCategoryCommand()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
             {
                 using (new CachingIsolator())
                 {
                     ImportAssemblyViewModel vm = new ImportAssemblyViewModel(this.testLib.Location);
                     vm.StartAddingCategoryCommand.Execute();
                     Assert.IsTrue(vm.IsAddingCategory);
                     vm.StopAddingCategoryCommand.Execute();
                     Assert.IsFalse(vm.IsAddingCategory);
                 }
             });
        }

        [WorkItem(325739)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [TestMethod()]
        public void Import_VerifyCanImportAssemblyCommandExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
             {
                 using (new CachingIsolator())
                 {
                     ImportAssemblyViewModel vm = new ImportAssemblyViewModel(this.testLib.Location);
                     vm.AssembliesToImport.ToList().ForEach(item => item.IsReviewed = true);
                     Assert.IsTrue(vm.CanImportCommandExecute());
                 }
             });
        }

        [WorkItem(325752)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [TestMethod()]
        public void Import_VerifyImportAssemblyCommandExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
             {
                 using (var mock = new ImplementationOfType(typeof(Assembly)))
                 {
                     Assembly assembly1 = this.testLib.Assembly;
                     mock.Register(() => Assembly.ReflectionOnlyLoadFrom(this.testLib.Location)).Execute<string, Assembly>((s) =>
                     {
                         return assembly1;
                     });
                     using (new CachingIsolator())
                     {
                         ImportAssemblyViewModel vm = new ImportAssemblyViewModel(this.testLib.Location);
                         string location = this.testLib.Location;

                         var sourceList = new ObservableCollection<ActivityAssemblyItemViewModel>(vm.AssembliesToImport);
                         vm.AssembliesToImport.Clear();
                         try { vm.ImportCommandExecute(); }
                         catch (ArgumentException ex)
                         {
                             Assert.AreEqual(ex.Message, "The list of assemblies to import is null.");
                         }

                         vm.SelectedActivityAssemblyItem = null;
                         sourceList.ToList().ForEach(item => vm.AssembliesToImport.Add(item));
                         vm.AssembliesToImport.ToList().ForEach(item => item.Location = null);
                         try { vm.ImportCommandExecute(); }
                         catch (ArgumentException ex)
                         {
                             Assert.AreEqual(ex.Message, "Assembly Location cannot be null.");
                             vm.AssembliesToImport.ToList().ForEach(item => item.Location = location);
                         }

                         using (var file = new ImplementationOfType(typeof(File)))
                         {
                             file.Register(() => File.Exists(Argument<string>.Any)).Return(false);
                             try { vm.ImportCommandExecute(); }
                             catch (ArgumentException ex)
                             {
                                 Assert.AreEqual(ex.Message, "Assembly Location cannot be null.");
                             }
                         }

                         using (var helper = new ImplementationOfType(typeof(ExpressionEditorHelper)))
                         {
                             bool isThrow = false;
                             helper.Register(() => ExpressionEditorHelper.CreateIntellisenseList()).Return(null);
                             vm.ImportCommandExecute();
                             Assert.IsTrue(vm.ImportResult);

                             helper.Register(() => ExpressionEditorHelper.CreateIntellisenseList()).Execute(() =>
                             {
                                 TreeNode nd = null;
                                 if (!isThrow)
                                     throw new Exception("Expect Exception");
                                 return nd;
                             });

                             try { vm.ImportCommandExecute(); }
                             catch (Exception ex)
                             {
                                 Assert.IsFalse(vm.ImportResult);
                                 Assert.AreEqual(ex.Message, "Expect Exception");
                             }
                         }
                     }
                 }
             });
        }

        [Description("Verify SetLocation error paths display the right messages")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void Import_VerifySetLocation_ErrorPaths()
        {
            var lib2 = TestInputs.ActivityAssemblyItems.TestInput_Lib2;
            MessageBoxService.ShowFunc = (msg, b, c, d, e) => { throw new AssertFailedException(msg); };

            // bad assembly path
            var exn = TestUtilities.Assert_ShouldThrow<UserFacingException>(() =>
                ImportAssemblyViewModel.ValidateLocation(new ActivityAssemblyItemViewModel(lib2), "$s..\n/ndif"));
            Assert.IsTrue(exn.Message.Contains("is not a .NET assembly and cannot be imported"));

            // not the requested assembly
            exn = TestUtilities.Assert_ShouldThrow<UserFacingException>(
                () => ImportAssemblyViewModel.ValidateLocation(new ActivityAssemblyItemViewModel(lib2), TestInputs.Assemblies.TestInput_Lib1.Location));
            Assert.IsTrue(exn.Message.Contains("does not contain the required assembly"));

            // not the requested version (unsigned vs 99.98)
            lib2.AssemblyName.Version = new Version(99, 98);
            exn = TestUtilities.Assert_ShouldThrow<UserFacingException>(
                () => ImportAssemblyViewModel.ValidateLocation(new ActivityAssemblyItemViewModel(lib2), TestInputs.Assemblies.TestInput_Lib2.Location));
            Assert.IsTrue(exn.Message.Contains("is v1.0.0.0, but v99.98 is required"));

            // not the requested version (4.0.0.0 vs. unsigned)
            lib2 = new ActivityAssemblyItem(typeof(Activity).Assembly);
            lib2.AssemblyName.Version = null;
            exn = TestUtilities.Assert_ShouldThrow<UserFacingException>(
                () => ImportAssemblyViewModel.ValidateLocation(new ActivityAssemblyItemViewModel(lib2), lib2.Location));
            Assert.IsTrue(exn.Message.Contains("but an unsigned assembly is required"));
        }

        [Description("Verify ImportAssembly recursively loads only non-cached dependencies")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Import_VerifyLoadsNonCachedDependencies()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                // Arrange
                var lib1 = new ActivityAssemblyItem(TestInputs.Assemblies.TestInput_Lib1.GetName());
                lib1.Location = TestInputs.Assemblies.TestInput_Lib1.Location; // Precondition for loading from cache

                var lib2 = new ActivityAssemblyItem(TestInputs.Assemblies.TestInput_Lib2.GetName());

                Assert.IsNull(lib1.Assembly);
                using (new CachingIsolator(lib1))
                {
                    using (var mock = new Implementation<AssemblyInspectionService>())
                    {

                        // Act
                        var lib3 = new ActivityAssemblyItem(TestInputs.Assemblies.TestInput_Lib3.GetName());
                        lib3.Location = TestInputs.Assemblies.TestInput_Lib3.Location;

                        mock.Register(inst => inst.Inspect(Argument<string>.Any)).Execute(() =>
                        {
                            return true;
                        });
                        mock.Register(inst => inst.SourceAssembly).Return(lib3);

                        HashSet<ActivityAssemblyItem> referencedAssemblies = new HashSet<ActivityAssemblyItem>();
                        referencedAssemblies.Add(lib1);
                        referencedAssemblies.Add(lib2);

                        mock.Register(inst => inst.ReferencedAssemblies).Return(referencedAssemblies.AsEnumerable());

                        Utility.GetAssemblyInspectionService = () => mock.Instance;
                        var vm = new ImportAssemblyViewModel(lib3.Location);

                        // Assert
                        var imports = vm.AssembliesToImport;
                        Assert.AreEqual(2 ,imports.Count); // lib3-without-location and ref to 3

                        // lib3
                        Assert.AreEqual(lib3.Name, imports[1].Name);

                        Assert.IsTrue(imports[0].Source.Matches(TestInputs.Assemblies.TestInput_Lib2.GetName()), "Should have ref to TestInput_Library2");
                        Assert.IsFalse(imports[0].IsReviewed);
                        Assert.AreEqual(CachingStatus.None, imports[0].CachingStatus, "TestInput_Lib2 should not have been autolocated");
                        Utility.GetAssemblyInspectionService = () => new AssemblyInspectionService();
                    }
                }
            });
        }

        [Description("Test that Location box will be empty so the user can fill it in, if AutoLocate fails")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Import_LeavesLocationEmptyWhenDependencyNotFound()
        {
            using (new CachingIsolator())
            {
                TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                 {
                     var hasUnknownReference = new ActivityAssemblyItem(TestInputs.Assemblies.TestInput_Lib1.GetName());
                     hasUnknownReference.ReferencedAssemblies.Add(new AssemblyName("unknownDll"));

                     try
                     {
                         var vm = new ImportAssemblyViewModel(hasUnknownReference.Location);
                     }
                     catch (ArgumentNullException ex)
                     {
                         Assert.IsNotNull(ex);
                     }
                 });
            }
        }

        [Description("Test that we don't load un-imported dlls into the current AppDomain, even reflection-only. We only load the cached versions after import completes.")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Import_AssemblyAlreadyInTheList()
        {
            using (new CachingIsolator())
            {
                TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                 {
                     // Arrange: lib1 with lib2 ref
                     var lib1 = TestInputs.ActivityAssemblyItems.TestInput_Lib1;
                     var vm = new ImportAssemblyViewModel(lib1.Location);
                     var lib2 = new ActivityAssemblyItem(TestInputs.Assemblies.TestInput_Lib2.GetName()); // no location, just a ref
                     lib1.ReferencedAssemblies.Add(lib2.AssemblyName);
                     vm.AssembliesToImport.Add(new ActivityAssemblyItemViewModel(lib2)); // Add it directly to the import list to simulate a non-autolocated assembly, without having to do the copy-file dance during test setup
                     lib2.ReferencedAssemblies.Add(lib1.AssemblyName);
                     // Act
                     lib2.Location = TestInputs.Assemblies.TestInput_Lib2.Location;
                     try
                     {
                         vm.UpdateReferenceLocation(new ActivityAssemblyItemViewModel(lib2), lib2.Location);
                     }
                     catch (Exception exn)
                     {
                         Assert.AreEqual("The selected assembly is already in the list of assemblies to import",
                             exn.Message);
                     }
                 });
            }
        }

        [Description("When a user tries to import an assembly with XAML dependencies, and those XAML dependencies can't be auto-located by the CLR (e.g. in the GAC or the current directory), we can't get them all at once. Verify that locating one makes the next one pop up.")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Import_DetectsDependenciesWhenLoading()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (new CachingIsolator())
                {
                    using (var mock = new ImplementationOfType(typeof(Assembly)))
                    {
                        // Arrange
                        var start1 = Path.GetTempFileName().Replace(".tmp", ".dll");
                        var start2 = Path.GetTempFileName().Replace(".tmp", ".dll");
                        var start3 = Path.GetTempFileName().Replace(".tmp", ".dll");

                        File.Copy(TestInputs.Assemblies.TestInput_Lib1.Location, start1, overwrite: true);
                        File.Copy(TestInputs.Assemblies.TestInput_Lib2.Location, start2, overwrite: true);
                        File.Copy(TestInputs.Assemblies.TestInput_Lib3.Location, start3, overwrite: true);

                        mock.Register(() => Assembly.ReflectionOnlyLoadFrom(start3)).Execute<string, Assembly>((s) =>
                        {
                            return TestInputs.Assemblies.TestInput_Lib3;
                        });

                        // Act and Assert
                        var vm = new ImportAssemblyViewModel(start3);
                        Assert.AreEqual(3, vm.AssembliesToImport.Count);
                    }
                }
            });
        }

        [Description("Test that we don't load un-imported dlls into the current AppDomain, even reflection-only. We only load the cached versions after import completes.")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void Import_DoesntLoadIntoCurrentAppDomain()
        {
            using (var appdomain = new DisposableAppDomain())
            {
                var loader = (Verify_Import_DoesntLoadIntoCurrentAppDomain_Implementation)appdomain.AppDomain.CreateInstanceFromAndUnwrap(typeof(Verify_Import_DoesntLoadIntoCurrentAppDomain_Implementation).Assembly.Location, typeof(Verify_Import_DoesntLoadIntoCurrentAppDomain_Implementation).FullName);

                // Act/Assert
                loader.ExecuteTest();
            }
        }

        [TestMethod()]
        [Description("Check to see if we can instantiate the target class correctly (Bug #86473)")]
        [TestCategory("Unit-Dif")]
        public void Import_ConstructorTest()
        {
            using (new CachingIsolator())
            {
                TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                {
                    using (var mock = new ImplementationOfType(typeof(Assembly)))
                    {
                        Assembly assembly1 = TestInputs.Assemblies.TestInput_Lib1;
                        mock.Register(() => Assembly.ReflectionOnlyLoadFrom(Argument<string>.Any)).Execute<string, Assembly>((s) =>
                        {
                            return assembly1;
                        });
                        ImportAssemblyViewModel target = new ImportAssemblyViewModel(assembly1.Location);
                        Assert.IsNotNull(target, "An instance of ImportAssemblyViewModel could not be instantiated.");
                    }
                });
            }
        }

        [TestMethod()]
        [Description("verify that we get the same value back out from the property that we put in (Bug #86473)")]
        [TestCategory("Unit-Dif")]
        public void Import_SelectedCategoryTest()
        {
            using (new CachingIsolator())
            {
                TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                 {
                     using (var mock = new ImplementationOfType(typeof(Assembly)))
                     {
                         Assembly assembly1 = TestInputs.Assemblies.TestInput_Lib1;
                         mock.Register(() => Assembly.ReflectionOnlyLoadFrom(Argument<string>.Any)).Execute<string, Assembly>((s) =>
                         {
                             return assembly1;
                         });
                         ImportAssemblyViewModel target = new ImportAssemblyViewModel(TestInputs.Assemblies.TestInput_Lib1.Location);
                         string expected = ANY_TEST_STRING;
                         string actual;

                         target.SelectedCategory = expected;
                         actual = target.SelectedCategory;

                         Assert.AreEqual(expected, actual);
                     }
                 });
            }
        }

        /// <summary>
        /// AppDomain doesn't play well with callbacks. Test code for Verify_Import_DoesntLoadIntoCurrentAppDomain that we will run in the other AppDomain.
        /// </summary>
        public class Verify_Import_DoesntLoadIntoCurrentAppDomain_Implementation : MarshalByRefObject
        {
            public Verify_Import_DoesntLoadIntoCurrentAppDomain_Implementation()
            {
                AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                    Utility.Resolve(args.Name);
                var c = Caching.ActivityAssemblyItems;
                Assert.AreEqual(0, c.Count);
            }

            public void ExecuteTest()
            {
                var vm = new ImportAssemblyViewModel("TestInput_Lib3.dll");
                Assert.AreEqual(3, vm.AssembliesToImport.Count);
                Assert.IsTrue(!string.IsNullOrEmpty(vm.AssembliesToImport[0].Location), "TestInput_Lib3 should have been located");
                Assert.IsTrue(!string.IsNullOrEmpty(vm.AssembliesToImport[1].Location), "TestInput_Lib2 should have been auto-located");
                Assert.IsTrue(!string.IsNullOrEmpty(vm.AssembliesToImport[2].Location), "TestInput_Lib1 should have been auto-located");
                var testlibs = AppDomain.CurrentDomain.GetAssemblies().Where(asm => asm.GetName().Name.Contains("TestInput_Lib"));
                Assert.AreEqual(0, testlibs.Count());
            }
        }
    }
}
