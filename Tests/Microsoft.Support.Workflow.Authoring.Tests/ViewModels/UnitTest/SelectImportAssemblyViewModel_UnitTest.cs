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
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using System.Activities.Statements;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using System.Collections.ObjectModel;
using AuthoringToolTests.Services;
using System.Windows;
namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.UnitTest
{
    [TestClass]
    public class SelectImportAssemblyViewModel_UnitTest
    {
        private ActivityAssemblyItem testLib1
        {
            get
            {
                var lib1 = TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1;
                lib1.ReferencedAssemblies.Clear();
                return lib1;
            }
        }

        private ActivityAssemblyItem testLib2
        {
            get
            {
                var lib1 = TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib2;
                lib1.AssemblyName = lib1.Assembly.GetName();
                lib1.Name = lib1.AssemblyName.Name;
                lib1.Version = lib1.AssemblyName.Version;
                lib1.ReferencedAssemblies.Clear();
                return lib1;
            }
        }

        // Create WorkflowItems lazily to avoid creating lots of WorkflowDesigners. 
        private WorkflowItem validWorkflowItem;
        private WorkflowItem ValidWorkflowItem
        {
            get
            {
                if (validWorkflowItem == null)
                {
                    validWorkflowItem = new WorkflowItem("MyWorkflow", "WF", (new Sequence()).ToXaml(), string.Empty);
                    Assert.IsTrue(validWorkflowItem.IsValid, "Setup error! validWorkflowItem should be valid XAML for workflow.");
                }
                return validWorkflowItem;
            }
        }


        [TestCategory("Unit")]
        [Owner("v-kason")]
        [TestMethod()]
        public void SelectImport_VerifyPropertyChanged()
        {
            //mock addin
            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
            {
                var wf = ValidWorkflowItem;
                IEnumerable<ActivityAssemblyItem> dependencies = new List<ActivityAssemblyItem>();

                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                workflowDesigner.Register(inst => inst.XamlCode).Return(string.Empty);
                workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                workflowDesigner.Register(inst => inst.CompileProject).Return(new CompileProject());
                workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependencies);
                wf.WorkflowDesigner = workflowDesigner.Instance;

                TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                {
                    SelectImportAssemblyViewModel vm = new SelectImportAssemblyViewModel(wf);
                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Title", () => vm.Title = "title");
                    Assert.AreEqual(vm.Title, "title");

                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "TabNames", () => vm.TabNames = null);
                    Assert.IsNull(vm.TabNames);

                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Assemblies", () => vm.Assemblies = new ObservableCollection<ActivityAssemblyItem>());
                    Assert.IsNotNull(vm.Assemblies);

                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedActivityAssemblyItem", () => vm.SelectedActivityAssemblyItem = new ActivityAssemblyItem());
                    Assert.IsNotNull(vm.SelectedActivityAssemblyItem);
                });
            }
        }

        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [TestMethod()]
        public void SelectImport_BrowseAssembly()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var service1 = new Implementation<AssemblyInspectionService>())
                {
                    var testLib = testLib1;
                    service1.Register(inst => inst.Inspect(Argument<string>.Any)).Return(true);
                    service1.Register(inst => inst.SourceAssembly).Return(testLib);
                    service1.Instance.OperationException = null;
                    Utility.GetAssemblyInspectionService = () => service1.Instance;

                    var wf = ValidWorkflowItem;

                    SelectImportAssemblyViewModel vm = new SelectImportAssemblyViewModel(wf);
                    vm.Assemblies.Clear();
                    bool isDialogOpen = false;
                    string assemblyFile = "test_file";
                    using (var dialogService = new ImplementationOfType(typeof(DialogService)))
                    {
                        dialogService.Register(() => DialogService.ShowOpenFileDialogAndReturnMultiResult(Argument<string>.Any, Argument<string>.Any))
                            .Execute(() =>
                            {
                                isDialogOpen = true;
                                assemblyFile = testLib.Location;
                                return new string[] { assemblyFile };
                            });

                        bool importAssemblyViewCreated = false;
                        ImplementationFunc<object, bool?> show = delegate(object o)
                        {
                            ImportAssemblyViewModel import = o as ImportAssemblyViewModel;
                            if (import != null)
                            {
                                PrivateObject po = new PrivateObject(import);
                                po.SetFieldOrProperty("ImportResult", true);
                            }
                            importAssemblyViewCreated = true;
                            return true;
                        };
                        dialogService.Register(() => DialogService.ShowDialog(Argument<object>.Any)).Execute(show);
                        vm.BrowseCommand.Execute();

                        Assert.IsTrue(isDialogOpen);
                        Assert.IsTrue(importAssemblyViewCreated);
                        Assert.IsTrue(vm.Assemblies.Count > 0);

                        Utility.GetAssemblyInspectionService = () => new AssemblyInspectionService();
                    }
                }
            });
        }

        [TestCategory("Unit")]
        [Owner("v-kason")]
        [TestMethod()]
        public void SelectImport_ImportAssembly()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                var a1 = this.testLib1;
                var a2 = this.testLib2;

                WorkflowItem myItem = this.ValidWorkflowItem;
                SelectImportAssemblyViewModel vm = new SelectImportAssemblyViewModel(myItem);
                vm.Assemblies.Clear();
                try
                {
                    Assert.IsTrue(myItem.References.Count == 0);
                    vm.Assemblies = new System.Collections.ObjectModel.ObservableCollection<ActivityAssemblyItem>() { a1, a2 };

                    myItem.WorkflowDesigner = null;
                    vm.ImportCommand.Execute();
                    Assert.IsTrue(vm.ImportResult);
                }
                catch (Exception ex)
                {
                    Assert.IsFalse(vm.ImportResult);
                }
            });
        }

        [TestCategory("Unit")]
        [Owner("v-kason")]
        [TestMethod()]
        public void SelectImport_CheckIfAnyConflict()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                SelectImportAssemblyViewModel vm = new SelectImportAssemblyViewModel(ValidWorkflowItem);
                vm.Assemblies.Clear();
                var a1 = this.testLib1;
                var a2 = this.testLib2;

                using (var messageBoxService = new ImplementationOfType(typeof(MessageBoxService)))
                {
                    bool isConflict = false;
                    messageBoxService.Register(() => MessageBoxService.CannotCheckAssemblyForAnotherVersionSelected(Argument<string>.Any, Argument<string>.Any, Argument<string>.Any))
                        .Execute(() => { isConflict = true; });

                    //check version conflict
                    a2.Name = a1.Name;
                    a2.Version = new Version("0.2.0.1");
                    vm.Assemblies.Add(a1);
                    vm.Assemblies.Add(a2);

                    a1.UserSelected = false;
                    a2.UserSelected = true;

                    PrivateObject po = new PrivateObject(vm);
                    po.Invoke("CheckIfAnyConflict", a1);
                    Assert.IsTrue(isConflict);

                    //check references conflict
                    vm.Assemblies.Clear();
                    a2.AssemblyName = a2.Assembly.GetName();
                    a2.Name = a2.AssemblyName.Name;
                    a2.Version = a2.AssemblyName.Version;

                    a1.ReferencedAssemblies = new System.Collections.ObjectModel.ObservableCollection<System.Reflection.AssemblyName>() { a2.AssemblyName };
                    a2.UserSelected = false;
                    a2.ReferencedAssemblies.Clear();
                    vm.Assemblies.Add(a1);
                    vm.Assemblies.Add(a2);

                    object result = po.Invoke("CheckIfAnyConflict", a1);
                    bool hasConflict = Convert.ToBoolean(result);
                    Assert.IsFalse(hasConflict);
                }
            });
        }

        [TestCategory("Unit")]
        [Owner("v-kason")]
        [TestMethod()]
        public void SelectImport_CheckAssemblyDependencies()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                SelectImportAssemblyViewModel_Accessor vm = new SelectImportAssemblyViewModel_Accessor(ValidWorkflowItem);
                vm.Assemblies.Clear();

                //test Argument null
                try
                {
                    vm.CheckAssemblyDependencies(null);
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex is ArgumentException);
                }

                ActivityAssemblyItem a1 = testLib1;
                ActivityAssemblyItem a2 = testLib2;
                //check version conflict
                using (var messageBoxService = new ImplementationOfType(typeof(MessageBoxService)))
                {
                    bool isConflict = false;
                    messageBoxService.Register(() => MessageBoxService.CannotCheckAssemblyForAnotherVersionSelected(Argument<string>.Any, Argument<string>.Any, Argument<string>.Any))
                        .Execute(() => { isConflict = true; });
                    a1.UserSelected = true;
                    a2.UserSelected = true;
                    a2.Name = a1.Name;
                    a2.Version = new Version("2.0.0.1");
                    vm.Assemblies.Add(a1);
                    vm.Assemblies.Add(a2);
                    vm.CheckAssemblyDependencies(a1);
                    Assert.IsFalse(a1.UserSelected);
                    Assert.IsTrue(isConflict);
                }

                //test if main assembly is selected, all referenced assembly shuold be selected.
                a1.UserSelected = true;
                a2.UserSelected = false;
                a2.AssemblyName = a2.Assembly.GetName();
                a2.Name = a2.AssemblyName.Name;
                a2.Version = a2.AssemblyName.Version;
                a1.ReferencedAssemblies =
                    new System.Collections.ObjectModel.ObservableCollection<System.Reflection.AssemblyName>() { a2.AssemblyName };

                vm.CheckAssemblyDependencies(a1);
                Assert.IsTrue(a2.UserSelected);
                a2.UserSelected = false;

                vm.CheckAssemblyDependencies(a1);
                Assert.IsTrue(a2.UserSelected);

                using (var messageBoxService = new ImplementationOfType(typeof(MessageBoxService)))
                {
                    //test if main assembly is referenced by onather assembly, it can't be unselected
                    bool isConflict = false;
                    messageBoxService.Register(() => MessageBoxService.CannotUncheckAssemblyForReferenced(Argument<AssemblyName>.Any, Argument<AssemblyName[]>.Any))
                        .Execute(() => { isConflict = true; });
                    a2.UserSelected = false;
                    a1.UserSelected = true;
                    vm.CheckAssemblyDependencies(a2);
                    Assert.IsTrue(a2.UserSelected);
                    Assert.IsTrue(isConflict);

                    //test if main assembly is unselected, its referencedAssemblies should be removed
                    a2.UserSelected = false;
                    PrivateObject po = new PrivateObject(vm);
                    a1.ReferencedAssemblies.Clear();
                    Dictionary<ActivityAssemblyItem, List<ActivityAssemblyItem>> referencedAssemblies = new Dictionary<ActivityAssemblyItem, List<ActivityAssemblyItem>>();
                    referencedAssemblies.Add(a1, new List<ActivityAssemblyItem>() { a2 });

                    po.SetFieldOrProperty("referencedAssemblies",
                        referencedAssemblies);
                    a2.ReferencedAssemblies = new System.Collections.ObjectModel.ObservableCollection<System.Reflection.AssemblyName>() { a1.AssemblyName }; ;

                    vm.CheckAssemblyDependencies(a2);
                    Assert.IsTrue(referencedAssemblies[a1].Count == 0);
                }
            });
        }
    }
}
