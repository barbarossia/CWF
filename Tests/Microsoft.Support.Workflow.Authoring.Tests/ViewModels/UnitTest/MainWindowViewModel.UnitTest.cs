using System;
using System.Activities.Presentation;
using System.Activities.Statements;
using System.Collections.ObjectModel;
using System.Reflection;
using System.ServiceModel.Activities;
using System.Text;
using System.Windows;
using System.Xaml;
using AuthoringToolTests.Services;
using CWF.DataContracts;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Security.Principal;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.Security;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media.Imaging;
using Microsoft.Support.Workflow.Authoring;
using System.Windows.Input;
using System.Activities.Presentation.View;
using System.Windows.Media;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using System.Windows.Threading;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;

namespace Authoring.Tests.Unit
{
    [TestClass]
    public partial class MainWindowViewModel_UnitTests
    {
        public MainWindowViewModel_UnitTests()
        {
            // Test setup: prevent MessageBoxes from popping up anywhere during a test, just in case a test fails with an error message
            MessageBoxService.ShowFunc = (a, b, c, d, e) => MessageBoxResult.OK;
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
                    validWorkflowItem.Env = Env.Dev;
                    Assert.IsTrue(validWorkflowItem.IsValid, "Setup error! validWorkflowItem should be valid XAML for workflow.");
                }
                return validWorkflowItem;
            }
        }


        private WorkflowItem invalidWorkflowItem;
        private WorkflowItem InvalidWorkflowItem
        {
            get
            {
                if (invalidWorkflowItem == null)
                {
                    invalidWorkflowItem = new WorkflowItem("MyWorkflow", "WF", (new Assign().ToXaml()), string.Empty);
                    invalidWorkflowItem.Env = Env.Dev;
                }
                return invalidWorkflowItem;
            }
        }


        [Description("Verify that INPC notifications are raised for properties that are supposed to have it")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_PropertyChangedNotificationsAreRaised()
        {
            var vm = new MainWindowViewModel();
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "FocusedActivityItem", () => vm.FocusedActivityItem = null);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "FocusedWorkflowItem", () => vm.FocusedWorkflowItem = null);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "IsBusy", () => vm.IsBusy = true);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "BusyCaption", () => vm.BusyCaption = "Hello");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Title", () => vm.Title = null);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "ErrorMessage", () => vm.ErrorMessage = "error");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "ErrorMessageType", () => vm.ErrorMessageType = "errortype");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "VersionFault", () => vm.VersionFault = null);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "UserInfo", () => vm.UserInfo = null);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Version", () => vm.Version = "1.0.0.1");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "AssemblyToImportFile", () => vm.AssemblyToImportFile = null);

            Assert.AreEqual(vm.AssemblyToImportFile, null);
            Assert.AreEqual(vm.ErrorMessage, "error");
            Assert.AreEqual(vm.ErrorMessageType, "errortype");
            Assert.AreEqual(vm.Version, "1.0.0.1");
            Assert.AreEqual(vm.FocusedActivityItem, null);
            Assert.AreEqual(vm.FocusedWorkflowItem, null);
            Assert.AreEqual(vm.IsBusy, true);
            Assert.AreEqual(vm.BusyCaption, "Hello");
            Assert.AreEqual(vm.Title, null);
            Assert.AreEqual(vm.VersionFault, null);
            Assert.AreEqual(vm.UserInfo, null);
        }

        //[WorkItem(325768)]
        //[Owner("v-kason")]
        //[TestCategory("Unit-Dif")]
        //[TestMethod]
        //public void VerifySaveWorkflowToBitmap()
        //{
        //    using (var viewModel = new Implementation<MainWindowViewModel>())
        //    {
        //        using (var command = new Implementation<RoutedCommand>())
        //        {
        //            this.validWorkflowItem = null;
        //            var wf = this.ValidWorkflowItem;
        //            //var designView = wf.WorkflowDesigner.Context.Services.GetService<DesignerView>();

        //            //command.Register(inst => inst.Execute(null, designView)).Execute(() => { });
        //            viewModel.Instance.GetRoutedCommand();
        //            viewModel.Register(inst => inst.GetRoutedCommand()).Return(command.Instance);

        //            var vm = viewModel.Instance;
        //            try { vm.SaveWorkflowToBitmap(null); }
        //            catch (ArgumentNullException expect)
        //            {
        //                Assert.AreEqual(expect.ParamName, "workflow");
        //            }

        //            using (var helper = new ImplementationOfType(typeof(VisualTreeHelper)))
        //            {
        //                helper.Register(() => VisualTreeHelper.GetDescendantBounds(Argument<Visual>.Any)).Return(new Rect(1, 1, 1, 1));
        //                //designView.Height = 100;
        //                //designView.Width = 100;
        //                using (var bitmap = new ImplementationOfType(typeof(BitmapFrame)))
        //                {
        //                    bool isCreated = false;
        //                    bitmap.Register(() => BitmapFrame.Create(Argument<BitmapSource>.Any))
        //                        .Execute(() =>
        //                        {
        //                            isCreated = true;
        //                            return BitmapFrame.Create(new Uri("http://www.test.com"));
        //                        });
        //                    vm.SaveWorkflowToBitmap(wf);
        //                    Assert.IsTrue(isCreated);
        //                }
        //            }
        //        }
        //    }
        //}

        [WorkItem(325742)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_VerifyCompileWorkflowWithError()
        {
            using (var compile = new ImplementationOfType(typeof(Compiler)))
            {
                using (var service = new ImplementationOfType(typeof(MessageBoxService)))
                {
                    bool isError = false;
                    service.Register(() => MessageBoxService.Show(Argument<string>.Any, Argument<string>.Any, MessageBoxButton.OK, MessageBoxImage.Error)).Execute(() =>
                    {
                        isError = true;
                        return MessageBoxResult.OK;
                    });

                    CompileResult result = new CompileResult(Microsoft.Build.Execution.BuildResultCode.Failure, string.Empty, new Exception("expect excption"));

                    compile.Register(() => Compiler.Compile(Argument<CompileProject>.Any)).Return(result);
                    var vm = new MainWindowViewModel();

                    //mock addin
                    using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                    {
                        var wf = ValidWorkflowItem;
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

                        wf.WorkflowDesigner = workflowDesigner.Instance;
                        vm.FocusedWorkflowItem = wf;
                        PrivateObject po = new PrivateObject(vm);
                        po.Invoke("CompileFocusedWorkflow");
                        Assert.IsTrue(isError);
                    }
                }
            }
        }

        [WorkItem(325749)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_VerifyGetActivityLibraries()
        {
            using (new CachingIsolator())
            using (var cach = new ImplementationOfType(typeof(Caching)))
            {
                using (var client = new WorkflowsQueryServiceClient())
                {
                    bool isGot = false;
                    cach.Register(() => Caching.ComputeDependencies(client, Argument<ActivityAssemblyItem>.Any)).Return(new List<ActivityAssemblyItem>());
                    cach.Register(() => Caching.DownloadAndCacheAssembly(client, Argument<List<ActivityAssemblyItem>>.Any)).Execute(() =>
                    {
                        isGot = true;
                    });
                    var vm = new MainWindowViewModel();
                    var dc = new StoreActivitiesDC();
                    dc.ActivityLibraryVersion = "0.1.1.1";
                    dc.ActivityLibraryName = "lib";
                    PrivateObject po = new PrivateObject(vm);
                    po.Invoke("GetActivityLibraries", client, dc);
                    Assert.IsTrue(isGot);
                }
            }

        }

        [Description("Verify that setting the focused workflow item updates the right commands")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_Check_Setting_FocusedWorkflowItem_UpdatesCanExecutes()
        {
            //mock addin
            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
            {
                var wf = ValidWorkflowItem;

                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                workflowDesigner.Register(inst => inst.XamlCode).Return(string.Empty);
                workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                wf.WorkflowDesigner = workflowDesigner.Instance;

                var vm = new MainWindowViewModel();
                TestUtilities.Assert_EventuallyShouldRaiseCanExecuteChanged(() => vm.FocusedWorkflowItem = wf,
                    vm.SaveFocusedWorkflowCommand,
                    vm.PublishCommand,
                    vm.CompileCommand
                );
            }
        }

        [WorkItem(321619)]
        [Description("Verify that the MainWindow can be closed only when there are no dirty items or the user explicitly okays it")]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_ShouldCancelExit()
        {
            // No workflow items
            using (var messageBox = new ImplementationOfType(typeof(MessageBoxService)))
            {
                var vm = new MainWindowViewModel();
                Assert.AreEqual(false, vm.CheckShouldCancelExit());

                vm = new MainWindowViewModel();
                messageBox.Register(() => MessageBoxService.ShoudExitWithMarketplaceOpened()).Return(false);
                messageBox.Register(() => MessageBoxService.ShoudExitWithMarketplaceDownloading()).Return(false);
                Assert.AreEqual(false, vm.CheckShouldCancelExit());
            }
            //mock addin
            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
            {
                var wf = ValidWorkflowItem;

                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                workflowDesigner.Register(inst => inst.XamlCode).Return(string.Empty);
                workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                wf.WorkflowDesigner = workflowDesigner.Instance;

                // Readonly workflow item
                using (var vmImpl = new Implementation<MainWindowViewModel>())
                {
                    vmImpl.Register(v => v.CloseWorkflowItem(Argument<int>.Any, Argument<WorkflowItem>.Any)).Execute(() => { });
                    var vm = vmImpl.Instance;
                    wf.IsReadOnly = true;
                    vm.WorkflowItems.Add(wf);
                    Assert.AreEqual(false, vm.CheckShouldCancelExit());
                }

                // Non-dirty workflow item from server
                using (var messageBox = new ImplementationOfType(typeof(MessageBoxService)))
                {
                    using (var vmImpl = new Implementation<MainWindowViewModel>())
                    {
                        vmImpl.Register(v => v.CloseWorkflowItem(Argument<int>.Any, Argument<WorkflowItem>.Any)).Execute(() => { });
                        var vm = vmImpl.Instance;
                        wf.IsReadOnly = false;
                        wf.IsOpenFromServer = true;
                        wf.IsDataDirty = false;
                        vm.WorkflowItems.Add(wf);

                        bool locked = true;
                        vm.OnStoreActivitesUnlockWithBusy = (a, b) => { locked = false; };

                        // cancel
                        messageBox.Register(() => MessageBoxService.ShowKeepLockedConfirmation(Argument<string>.Any)).Return(null);
                        Assert.AreEqual(true, vm.CheckShouldCancelExit());
                        Assert.IsTrue(locked);
                        // keep lock
                        messageBox.Register(() => MessageBoxService.ShowKeepLockedConfirmation(Argument<string>.Any)).Return(SavingResult.DoNothing);
                        Assert.AreEqual(false, vm.CheckShouldCancelExit());
                        Assert.IsTrue(locked);
                        // unlock
                        vm.WorkflowItems.Add(wf);
                        messageBox.Register(() => MessageBoxService.ShowKeepLockedConfirmation(Argument<string>.Any)).Return(SavingResult.Unlock);
                        Assert.AreEqual(false, vm.CheckShouldCancelExit());
                        Assert.IsFalse(locked);
                    }
                }

                // Dirty workflow item from server
                using (var messageBox = new ImplementationOfType(typeof(MessageBoxService)))
                {
                    using (var vmImpl = new Implementation<MainWindowViewModel>())
                    {
                        vmImpl.Register(v => v.CloseWorkflowItem(Argument<int>.Any, Argument<WorkflowItem>.Any)).Execute(() => { });
                        var vm = vmImpl.Instance;
                        wf.IsReadOnly = false;
                        wf.IsOpenFromServer = true;
                        wf.IsDataDirty = true;
                        vm.WorkflowItems.Add(wf);

                        bool isSaved = false;
                        vm.OnSaveToServer = (a) => { isSaved = true; return isSaved; };

                        // cancel
                        messageBox.Register(() => MessageBoxService.ShowClosingConfirmation(Argument<string>.Any)).Return(null);
                        Assert.AreEqual(true, vm.CheckShouldCancelExit());
                        Assert.IsFalse(isSaved);
                        // don't save
                        messageBox.Register(() => MessageBoxService.ShowClosingConfirmation(Argument<string>.Any)).Return(SavingResult.DoNothing);
                        Assert.AreEqual(false, vm.CheckShouldCancelExit());
                        Assert.IsFalse(isSaved);
                        // save
                        vm.WorkflowItems.Add(wf);
                        messageBox.Register(() => MessageBoxService.ShowClosingConfirmation(Argument<string>.Any)).Return(SavingResult.Save);
                        Assert.AreEqual(false, vm.CheckShouldCancelExit());
                        Assert.IsTrue(isSaved);
                    }
                }

                // Non-dirty workflow item from local
                using (var vmImpl = new Implementation<MainWindowViewModel>())
                {
                    vmImpl.Register(v => v.CloseWorkflowItem(Argument<int>.Any, Argument<WorkflowItem>.Any)).Execute(() => { });
                    var vm = vmImpl.Instance;
                    wf.IsReadOnly = false;
                    wf.IsOpenFromServer = false;
                    wf.IsDataDirty = false;
                    vm.WorkflowItems.Add(wf);

                    Assert.AreEqual(false, vm.CheckShouldCancelExit());
                }

                // Dirty workflow item from local
                using (var messageBox = new ImplementationOfType(typeof(MessageBoxService)))
                {
                    using (var vmImpl = new Implementation<MainWindowViewModel>())
                    {
                        vmImpl.Register(v => v.CloseWorkflowItem(Argument<int>.Any, Argument<WorkflowItem>.Any)).Execute(() => { });
                        var vm = vmImpl.Instance;
                        wf.IsReadOnly = false;
                        wf.IsOpenFromServer = false;
                        wf.IsDataDirty = true;
                        vm.WorkflowItems.Add(wf);

                        bool isSaved = false;
                        vm.OnSaveToLocal = (a, b) => { isSaved = true; return isSaved; };

                        // cancel
                        messageBox.Register(() => MessageBoxService.ShowLocalSavingConfirmation(Argument<string>.Any)).Return(null);
                        Assert.AreEqual(true, vm.CheckShouldCancelExit());
                        Assert.IsFalse(isSaved);
                        // don't save
                        messageBox.Register(() => MessageBoxService.ShowLocalSavingConfirmation(Argument<string>.Any)).Return(SavingResult.DoNothing);
                        Assert.AreEqual(false, vm.CheckShouldCancelExit());
                        Assert.IsFalse(isSaved);
                        // save
                        vm.WorkflowItems.Add(wf);
                        messageBox.Register(() => MessageBoxService.ShowLocalSavingConfirmation(Argument<string>.Any)).Return(SavingResult.Save);
                        Assert.AreEqual(false, vm.CheckShouldCancelExit());
                        Assert.IsTrue(isSaved);
                    }
                }
            }
        }



        [Description("Verify that Compile is enabled only when workflow is not dirty and not a service, and has no validation errors")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_CompileCanExecute()
        {
            //mock addin
            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
            {
                var wf = ValidWorkflowItem;
                TestUtilities.RegistLoginUserRole(Role.Admin);
                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                workflowDesigner.Register(inst => inst.XamlCode).Return(string.Empty);
                workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                wf.WorkflowDesigner = workflowDesigner.Instance;

                // Requires: focused item is not dirty and is not a service and has no validation errors
                var vm = new MainWindowViewModel();
                var compile = vm.CompileCommand;
                
                Assert.AreEqual(false, compile.CanExecute()); // FocusedWorkflowItem is null

                vm.FocusedWorkflowItem = wf;
                Assert.AreEqual(true, compile.CanExecute()); // dirty is irrelevant to compile
            }
        }

        [Description("Verify that increments version of the just-compiled workflow, and imports the assembly that was created from the workflow")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void aaMainWindow_CompileForSuccessfulCompile()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                //mock addin
                using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                {
                    var wf = ValidWorkflowItem;
                    List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                    workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                    workflowDesigner.Register(inst => inst.XamlCode).Return(string.Empty);
                    workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                    workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                    workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.CompileProject).Return(new CompileProject() { });
                    workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);

                    wf.WorkflowDesigner = workflowDesigner.Instance;
                    wf.Version = "1.4.0.0";

                    // Arrange
                    var vm = new MainWindowViewModel();
                    var errors = new StringBuilder();
                    MessageBoxService.ShowFunc = (msg, b, c, d, e) => { errors.Append(msg); return MessageBoxResult.OK; };

                    // ImportAssemblies requires a valid assembly file for PreviewImportAssembly but it doesn't matter which
                    // so we can use a test input library
                    var validAssemblyPath = TestInputs.Assemblies.TestInput_Lib1.Location;

                    // Act
                    using (new CachingIsolator()) // to prevent "This assembly already loaded"
                    {
                        vm.FocusedWorkflowItem = wf;
                        var pvm = new PrivateObject(vm);
                        try
                        {
                            using (var compile = new ImplementationOfType(typeof(Compiler)))
                            {
                                CompileResult result = new CompileResult(Microsoft.Build.Execution.BuildResultCode.Success, validAssemblyPath, null);
                                compile.Register(() => Compiler.Compile(Argument<CompileProject>.Any)).Return(result);
                                compile.Register(() => Compiler.AddToCaching(Argument<string>.Any)).Execute(() => { });
                                pvm.Invoke("CompileCommandExecute");
                                // Assert
                                Assert.AreEqual("1.4.1.0", wf.Version);
                            }
                        }
                        catch (System.Reflection.TargetInvocationException tie) //Obviously target invocation fails, but we just want to prove compile works
                        {
                            Console.WriteLine("Captured obvious exception: " + tie.Source);
                        }
                    }

                }
            });
        }

        [Description("Verify that Compile button does nothing after failure")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_CompileForUnsuccessfulCompile()
        {
            //mock addin
            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
            {
                var wf = ValidWorkflowItem;

                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                workflowDesigner.Register(inst => inst.XamlCode).Return(string.Empty);
                workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                wf.WorkflowDesigner = workflowDesigner.Instance;

                // Arrange
                var vm = new MainWindowViewModel();
                wf.Version = "1.4.0.0";
                var validAssemblyPath = TestInputs.Assemblies.TestInput_Lib1.Location;
                // Act
                using (new CachingIsolator()) // to prevent "This assembly already loaded"
                {
                    CompileResult result = null;
                    var pvm = new PrivateObject(vm);
                    try
                    {
                        pvm.Invoke("CompileCommandExecutePostCompile", wf, result); // null or empty signifies unsuccessful compile
                    }
                    catch (ArgumentNullException ane) //if null, wont compile
                    {
                        Console.WriteLine("Captured obvious exception: " + ane.Source);
                    }
                }

                // Assert
                Assert.AreEqual("1.4.0.0", wf.Version); // does not change on unsuccessful compile   
            }
        }

        [Description("Verify that NewWorkflowCommand adds created workflow to WorkflowItems collection")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_NewWorkflowCommand()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
             {
                 using (var push = new ImplementationOfType(typeof(Dispatcher)))
                 {
                     bool ispush = false;
                     push.Register(() => Dispatcher.PushFrame(Argument<DispatcherFrame>.Any)).Execute(() => { ispush = true; });

                     using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                     {
                         var wf = ValidWorkflowItem;
                         TestUtilities.RegistLoginUserRole(Role.Admin);
                         List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                         workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                         workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                         workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                         workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                         workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                         workflowDesigner.Register(inst => inst.XamlCode).Return(string.Empty);
                         workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                         workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                         workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                         workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                         workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                         wf.WorkflowDesigner = workflowDesigner.Instance;

                         using (var client = new Implementation<WorkflowsQueryServiceClient>())
                         {
                             client.Register(inst => inst.WorkflowTypeGet(Argument<WorkflowTypesGetRequestDC>.Any)).Execute(() =>
                             {
                                 WorkflowTypeGetReplyDC replyDC = new WorkflowTypeGetReplyDC();
                                 replyDC.WorkflowActivityType = new List<WorkflowTypesGetBase>() 
                                 {
                                    new WorkflowTypesGetBase(){WorkflowTemplateId=1,Name="myworkflow"},
                                 };
                                 return replyDC;
                             });
                             WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                             // For some reason, NewWorkflowCommand does not use CheckIsInListOrAdd and does not do the normal copy thing,
                             // it just adds directly to the WorkflowItems collection.
                             var vm = new MainWindowViewModel();
                             DialogService.ShowDialogFunc = (viewModel) =>
                             {
                                 //suppressing dialog display and assing new workflow    
                                 return true;
                             };
                             Assert.AreEqual(0, vm.WorkflowItems.Count);

                             // Act
                             vm.NewWorkflowCommand.Execute();

                             // Assert
                             Assert.AreEqual(0, vm.WorkflowItems.Count);

                             // Act
                             wf.IsDataDirty = true;
                             DialogService.ShowDialogFunc = (viewModel) =>
                             {
                                 var pvm = new PrivateObject(viewModel);
                                 pvm.SetFieldOrProperty("CreatedItem", wf);
                                 return true;
                             };

                             vm.NewWorkflowCommand.Execute();
                             // Assert
                             Assert.AreEqual(true, wf.IsDataDirty); // should still be true
                             Assert.AreEqual(1, vm.WorkflowItems.Count);
                             Assert.IsTrue(vm.WorkflowItems.Contains(wf));
                             WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                         }
                     }
                 }
             });
        }

        [Description("Verify that CheckIsAlreadyInListOrAdd copies and adds workflow items which are not already in collection")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_CheckIsAlreadyInListOrAdd()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                //mock addin
                using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                {
                    var wf1 = ValidWorkflowItem;
                    var wf2 = InvalidWorkflowItem;

                    List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                    workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                    workflowDesigner.Register(inst => inst.XamlCode).Return(string.Empty);
                    workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                    workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                    workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                    wf1.WorkflowDesigner = workflowDesigner.Instance;
                    wf2.WorkflowDesigner = workflowDesigner.Instance;

                    // Arrange
                    var vm = new MainWindowViewModel();

                    wf1.Version = "6.6";
                    wf2.Name = wf1.Name;
                    wf2.Version = wf1.Version;

                    vm.WorkflowItems.Add(wf1);

                    // Act
                    vm.CheckIsAlreadyInListOrAdd(wf2); // won't add because something w/ same Name/Version is already in the list

                    // Assert
                    Assert.AreEqual(1, vm.WorkflowItems.Count);
                    Assert.AreSame(wf1, vm.WorkflowItems[0]);

                    // Act
                    wf2.Name = wf2.Name + "1";
                    vm.CheckIsAlreadyInListOrAdd(wf2); // will add a copy because Name/Version is not in the list already

                    // Assert
                    Assert.AreEqual(2, vm.WorkflowItems.Count);
                    Assert.AreEqual(wf2.Name, vm.WorkflowItems[1].Name);
                    Assert.AreEqual(wf2.Version, vm.WorkflowItems[1].Version);
                    Assert.AreEqual(wf2.DisplayName, vm.WorkflowItems[1].DisplayName);
                    Assert.AreEqual(wf2.XamlCode, vm.WorkflowItems[1].XamlCode);
                    Assert.AreEqual(wf2.WorkflowType, vm.WorkflowItems[1].WorkflowType);
                    Assert.AreEqual(wf2.LocalFileFullName, vm.WorkflowItems[1].LocalFileFullName);
                    Assert.AreEqual(wf2.IsSavedToServer, vm.WorkflowItems[1].IsSavedToServer);
                    Assert.AreEqual(wf2.IsDataDirty, vm.WorkflowItems[1].IsDataDirty);
                }
            });
        }

        [Description("Verify that Publish is enabled only for valid services that are already saved to server")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_PublishCommandCanExecute()
        {  //mock addin
            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
            {
                var wf = ValidWorkflowItem;
                TestUtilities.RegistLoginUserRole(Role.Admin);
                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                workflowDesigner.Register(inst => inst.XamlCode).Return(string.Empty);
                workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                wf.WorkflowDesigner = workflowDesigner.Instance;

                var vm = new MainWindowViewModel();

                // Requires: focused item is saved to server and is a service and has no validation errors
                var publish = vm.PublishCommand;
                Assert.AreEqual(false, publish.CanExecute()); // FocusedWorkflowItem is null

                vm.FocusedWorkflowItem = wf;
                vm.FocusedWorkflowItem.WorkflowName = "23wirok";//not valid workflow
                Assert.AreEqual(false, publish.CanExecute()); // not a service

                vm.FocusedWorkflowItem.Category = "unassigned";
                vm.FocusedWorkflowItem.WorkflowName = "TestName";
                Assert.AreEqual(true, publish.CanExecute()); // not saved
            }
        }

        [Description("Verify that Publish saves a workflow if it hasn't been saved already")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        [Ignore]
        public void MainWindow_PublishSavesIfNecessary()
        {

            //mock addin
            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
            {
                var wf = ValidWorkflowItem;
                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                wf.WorkflowDesigner = workflowDesigner.Instance;

                using (var mock = new Mock<IWorkflowsQueryService>())
                {
                    wf.IsSavedToServer = false;
                    var msgs = new StringBuilder();
                    MessageBoxService.ShowClickableFunc = (msg, caption, url) =>
                    {
                        msgs.Append(msg);
                        msgs.Append(url);
                        return MessageBoxResult.OK;
                    };

                    mock.Expect(inst => inst.GetNextVersion(Argument<StoreActivitiesDC>.Any, Argument<string>.Any)).Return(new Version());
                    mock.Expect(inst => inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any))
                        .Return(new GetAllActivityLibrariesReplyDC());
                    mock.Expect(inst => inst.UploadActivityLibraryAndDependentActivities(Argument<StoreLibraryAndActivitiesRequestDC>.Any))
                       .Return(new StatusReplyDC());
                    mock.Expect(inst => inst.PublishWorkflow(Argument<PublishingRequest>.Any))
                       .Execute((PublishingRequest dc) =>
                       {
                           Assert.AreEqual(wf.Name, dc.WorkflowName);
                           Assert.AreEqual(wf.Version, dc.WorkflowVersion);
                           return new PublishingReply { StatusReply = new StatusReplyDC(), PublishedVersion = "1.0", PublishedLocation = "somewhere" };
                       });

                    MainWindowViewModel vm = new MainWindowViewModel();
                    vm.PublishCommandExecute_Implementation(mock.Instance, wf);
                    Assert.AreEqual(string.Format("Workflow: {0} Version 1.0 was published to:somewhere", wf.Name), msgs.ToString());
                }
            }
        }

        [Description("Verify the info message from a successful Publish")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_PublishOnSuccess()
        {
            //mock addin
            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
            {
                var wf = ValidWorkflowItem;
                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                wf.WorkflowDesigner = workflowDesigner.Instance;
                using (var mock = new Mock<IWorkflowsQueryService>())
                {
                    using (var viewModel = new Implementation<MainWindowViewModel>())
                    {
                        viewModel.Register(inst => inst.UploadWorkflowWithBusy(Argument<WorkflowItem>.Any))
                        .Execute(() =>
                        {
                            return true;
                        });

                        wf.IsSavedToServer = true; // skip Save for this test
                        var msgs = new StringBuilder();
                        MessageBoxService.ShowClickableFunc = (msg, caption, url) =>
                        {
                            msgs.Append(msg);
                            msgs.Append(url);
                            return MessageBoxResult.OK;
                        };
                        mock.Expect(inst => inst.PublishWorkflow(Argument<PublishingRequest>.Any))
                            .Execute((PublishingRequest dc) =>
                                {
                                    Assert.AreEqual(wf.Name, dc.WorkflowName);
                                    Assert.AreEqual(wf.Version, dc.WorkflowVersion);
                                    return new PublishingReply { StatusReply = new StatusReplyDC(), PublishedVersion = "1.0", PublishedLocation = "somewhere" };
                                });

                        MainWindowViewModel vm = viewModel.Instance;
                        vm.PublishCommandExecute_Implementation(mock.Instance, wf);
                        Assert.AreNotEqual(string.Format("Workflow: {0} Version 1.0 was published to:somewhere", wf.Name), msgs.ToString());
                    }
                }
            }
        }

        [Description("Verify the error message from a failed Publish that has a 0 errorcode")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_PublishOnSoftFailure()
        {
            // For some reason, Publish will sometimes return 0 but still have errors that need to be shown to the user.
            // This is incoherent and should be fixed, but for now it is the behavior and we need to display the result to the user
            using (var mock = new Mock<IWorkflowsQueryService>())
            {
                using (var viewModel = new Implementation<MainWindowViewModel>())
                {
                    viewModel.Register(inst => inst.UploadWorkflowWithBusy(Argument<WorkflowItem>.Any))
                    .Execute(() =>
                    {
                        return true;
                    });

                    //mock addin
                    using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                    {
                        var wf = ValidWorkflowItem;
                        List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                        workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                        workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                        workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                        workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                        workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                        workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                        workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                        workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                        workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                        workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                        workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                        wf.WorkflowDesigner = workflowDesigner.Instance;

                        wf.IsSavedToServer = true; // skip Save for this test
                        var msgs = new StringBuilder();
                        MessageBoxService.ShowClickableFunc = (msg, c, url) =>
                        {
                            msgs.Append(msg);
                            return MessageBoxResult.OK;
                        };
                        mock.Expect(inst => inst.PublishWorkflow(Argument<PublishingRequest>.Any))
                            .Execute((PublishingRequest dc) =>
                                {
                                    Assert.AreEqual(wf.Name, dc.WorkflowName);
                                    Assert.AreEqual(wf.Version, dc.WorkflowVersion);
                                    return new PublishingReply { StatusReply = new StatusReplyDC(), PublishedVersion = "1.0", PublishedLocation = "somewhere", PublishErrors = "ERRORS" };
                                });
                        MainWindowViewModel vm = viewModel.Instance;
                        vm.PublishCommandExecute_Implementation(mock.Instance, wf);
                        Assert.AreNotEqual(string.Format("Workflow: {0} Version 1.0 was published with errors reported during publish: ERRORS", wf.Name), msgs.ToString());
                    }
                }
            }
        }

        [Description("Verify the error message from a failed Publish")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_PublishOnHardFailure()
        {
            using (var mock = new Mock<IWorkflowsQueryService>())
            {
                using (var viewModel = new Implementation<MainWindowViewModel>())
                {
                    viewModel.Register(inst => inst.UploadWorkflowWithBusy(Argument<WorkflowItem>.Any))
                    .Execute(() =>
                    {
                        return true;
                    });

                    //mock addin
                    using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                    {
                        var wf = ValidWorkflowItem;
                        List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                        workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                        workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                        workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                        workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                        workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                        workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                        workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                        workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                        workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                        workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                        workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                        wf.WorkflowDesigner = workflowDesigner.Instance;

                        wf.IsSavedToServer = true; // skip Save for this test
                        var msgs = new StringBuilder();
                        MessageBoxService.ShowFunc = (msg, a, b, c, d) =>
                        {
                            msgs.Append(msg);
                            return MessageBoxResult.OK;
                        };
                        mock.Expect(inst => inst.PublishWorkflow(Argument<PublishingRequest>.Any))
                            .Execute((PublishingRequest dc) =>
                                {
                                    Assert.AreEqual(wf.Name, dc.WorkflowName);
                                    Assert.AreEqual(wf.Version, dc.WorkflowVersion);
                                    return new PublishingReply { StatusReply = new StatusReplyDC { Errorcode = 1, ErrorMessage = "Error message" } };
                                });
                        var vm = viewModel.Instance;
                        vm.PublishCommandExecute_Implementation(mock.Instance, wf);
                        Assert.AreNotEqual("Error message", msgs.ToString());
                    }
                }
            }
        }

        [Description("Verify that adding to WorkflowItems sets up the right notifications")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_WorkflowItemPropertyChanged_And_ItemAdded()
        {
            //mock addin
            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
            {
                var wf = ValidWorkflowItem;
                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                wf.WorkflowDesigner = workflowDesigner.Instance;
                // Unit test these two together because they both do basically one thing, working together: trigger CanExecute updates
                var vm = new MainWindowViewModel();

                // Check only the Compile command for simplicity, since they are all linked
                TestUtilities.Assert_EventuallyShouldRaiseCanExecuteChanged(() => vm.WorkflowItems.Add(wf), vm.CompileCommand); // adding to WorkflowItems causes re-evaluation of CanExecute
                TestUtilities.Assert_EventuallyShouldRaiseCanExecuteChanged(() => wf.IsDataDirty = false, vm.CompileCommand); // and sets up notifications on the thing added
            }
        }

        [WorkItem(321607)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_CheckUnlockCommandCanExecute()
        {
            //mock addin
            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
            {
                var wf = ValidWorkflowItem;
                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                wf.WorkflowDesigner = workflowDesigner.Instance;
                TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                {
                    var vm = new MainWindowViewModel();
                    Assert.IsFalse(vm.UnlockCommand.CanExecute());

                    vm.FocusedWorkflowItem = wf;
                    vm.FocusedWorkflowItem.IsOpenFromServer = true;
                    vm.FocusedWorkflowItem.IsReadOnly = false;
                    Assert.IsTrue(vm.UnlockCommand.CanExecute());
                });
            }
        }

        [WorkItem(322356)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_VerifySaveToServer()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
               {
                   using (var uploader = new Implementation<MainWindowViewModel>())
                   {
                       bool isUploaded = false;
                       uploader.Register(inst => inst.UploadWorkflowWithBusy(Argument<WorkflowItem>.Any)).Execute(() =>
                       {
                           isUploaded = true;
                           return isUploaded;
                       });

                       //mock addin
                       using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                       {
                           var wf = ValidWorkflowItem;
                           List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                           workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                           workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                           workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                           workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                           workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                           workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                           workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                           workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                           workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                           workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                           workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                           wf.WorkflowDesigner = workflowDesigner.Instance;
                           var vm = uploader.Instance;
                           vm.FocusedWorkflowItem = wf;
                           vm.SaveToServer(vm.FocusedWorkflowItem);
                           Assert.IsTrue(isUploaded);
                       }
                   }
               });
        }
        [WorkItem(321608)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_VerifyUnlockExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
               {
                   using (var client = new Implementation<WorkflowsQueryServiceClient>())
                   {
                       using (var viewModel = new Implementation<MainWindowViewModel>())
                       {
                           viewModel.Register(inst => inst.SaveToServer(Argument<WorkflowItem>.Any)).Execute(() =>
                           {
                               return true;
                           });

                           var vm = viewModel.Instance;

                           //mock addin
                           using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                           {
                               var wf = ValidWorkflowItem;
                               List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                               workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                               workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                               workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                               workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                               workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                               workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                               workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                               workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                               workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                               workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                               workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                               wf.WorkflowDesigner = workflowDesigner.Instance;

                               vm.FocusedWorkflowItem = this.ValidWorkflowItem;
                               vm.FocusedWorkflowItem.IsOpenFromServer = false;
                               //do nothing
                               using (var messageBoxService = new ImplementationOfType(typeof(MessageBoxService)))
                               {
                                   bool needToSave = true;
                                   messageBoxService.Register(() => MessageBoxService.ShowUnlockConfirmation(Argument<string>.Any)).Execute(() =>
                                   {
                                       SavingResult? result = null;
                                       needToSave = false;
                                       return result;
                                   });
                                   vm.UnlockCommand.Execute();
                                   Assert.IsFalse(needToSave);
                               }

                               using (var principal = new Implementation<WindowsPrincipal>())
                               {
                                   //principal.Register(p => p.IsInRole(AuthorizationService.AdminAuthorizationGroupName))
                                   //    .Return(true);
                                   //principal.Register(p => p.IsInRole(AuthorizationService.AuthorAuthorizationGroupName))
                                   //    .Return(false);

                                   Thread.CurrentPrincipal = principal.Instance;

                                   //Save lock
                                   using (var messageBoxService = new ImplementationOfType(typeof(MessageBoxService)))
                                   {
                                       bool isSetLock = false;
                                       List<StoreActivitiesDC> dcs = new List<StoreActivitiesDC>();
                                       dcs.Add(new StoreActivitiesDC()
                                       {
                                           Incaller = Assembly.GetExecutingAssembly().GetName().Name,
                                           IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                                           Name = vm.FocusedWorkflowItem.Name,
                                           Version = vm.FocusedWorkflowItem.Version,
                                           Locked = true,
                                           LockedBy = Environment.UserName,
                                           StatusReply = new StatusReplyDC() { Errorcode = 0, },
                                       });

                                       client.Register(inst => inst.StoreActivitiesGet(Argument<StoreActivitiesDC>.Any)).Execute(() =>
                                       {
                                           return dcs;
                                       });

                                       client.Register(inst => inst.StoreActivitiesUpdateLock(Argument<StoreActivitiesDC>.Any, Argument<DateTime>.Any)).Execute(() =>
                                       {
                                           StatusReplyDC reply = new StatusReplyDC();
                                           reply.Errorcode = 0;
                                           isSetLock = true;
                                           return reply;
                                       });
                                       WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                                       SavingResult? result = null;
                                       messageBoxService.Register(() => MessageBoxService.ShowUnlockConfirmation(Argument<string>.Any)).Execute(() =>
                                       {
                                           result = SavingResult.Save;
                                           return result;
                                       });

                                       bool isUploaded = false;
                                       viewModel.Register(inst => inst.UploadWorkflow(WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient(), Argument<WorkflowItem>.Any, true)).Execute(() =>
                                       {
                                           isUploaded = true;
                                           return isUploaded;
                                       });

                                       vm.OnStoreActivitesUnlock = new Action<WorkflowItem, bool>((f, e) => { isSetLock = true; });
                                       vm.FocusedWorkflowItem.WorkflowDesigner = null;
                                       vm.UnlockCommand.Execute();
                                       Assert.IsTrue(isSetLock);
                                   }
                               }
                           }
                       }
                   }
               });
        }

        [Description("Verify that save is enabled when workflow is loaded and is dirty")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_CheckSaveFocusedWorkflowCanExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                //mock addin
                using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                {
                    var wf = ValidWorkflowItem;
                    TestUtilities.RegistLoginUserRole(Role.Admin);
                    List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                    workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                    workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                    workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                    workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.Tasks).Return(new List<TaskAssignment>());
                    workflowDesigner.Register(inst => inst.FinishTaskAssigned()).Execute(() => { });
                        
                    wf.WorkflowDesigner = workflowDesigner.Instance;
                    
                    var vm = new MainWindowViewModel();
                    var command = vm.SaveFocusedWorkflowCommand;
                    Assert.IsFalse(command.CanExecute("ToLocal")); // FocusedWorkflowItem is null
                    vm.WorkflowItems.Add(wf);
                    vm.FocusedWorkflowItem = wf;
                    wf.IsDataDirty = true;
                    Assert.IsTrue(command.CanExecute("ToLocal")); // dirty is irrelevant to compile
                    //Assert.IsTrue(command.CanExecute("SaveAsToLocal"));
                    Assert.IsTrue(command.CanExecute("ToMarketplace"));
                    vm.FocusedWorkflowItem.IsReadOnly = false;
                    Assert.IsTrue(command.CanExecute("ToServer"));
                    vm.FocusedWorkflowItem.IsSavedToServer = true;
                    //Assert.IsTrue(command.CanExecute("SaveAsTemplate"));
                    Assert.IsTrue(command.CanExecute("ToImage"));
                }
            });
        }

        [Description("Verify that close focused command can execute only when a workflow is loaded")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_CheckCloseCanExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                //mock addin
                using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                {
                    var wf = ValidWorkflowItem;
                    List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                    workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                    workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                    workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                    workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                    wf.WorkflowDesigner = workflowDesigner.Instance;
                    var vm = new MainWindowViewModel();
                    var command = vm.CloseFocusedWorkflowCommand;
                    Assert.AreEqual(false, command.CanExecute()); // FocusedWorkflowItem is null

                    vm.FocusedWorkflowItem = wf;
                    wf.IsDataDirty = true;
                    Assert.AreEqual(true, command.CanExecute()); // dirty is irrelevant to compile
                }
            });
        }

        //[WorkItem(325767)]
        //[TestCategory("Unit-Dif")]
        //[Owner("v-kason1")]
        //[TestMethod]
        //public void VerifySaveToImageFile()
        //{
        //    using (var viewModel = new Implementation<MainWindowViewModel>())
        //    {
        //        viewModel.Register(inst => inst.SaveWorkflowToBitmap(Argument<WorkflowItem>.Any)).Execute(() =>
        //        {
        //            return BitmapFrame.Create(new Uri("http://www.test.com"));
        //        });
        //        var vm = viewModel.Instance;

        //        try { vm.SaveToImageFile(null); }
        //        catch (Exception ex) { Assert.IsTrue(ex is ArgumentNullException); }

        //        using (var service = new ImplementationOfType(typeof(DialogService)))
        //        {
        //            string fileName = string.Empty;
        //            service.Register(() => DialogService.ShowSaveDialogAndReturnResult(Argument<string>.Any, Argument<string>.Any))
        //                .Execute(() =>
        //                {
        //                    fileName = "file";
        //                    return fileName;
        //                });

        //            using (var serviceFile = new ImplementationOfType(typeof(FileService)))
        //            {
        //                //print wf with no error
        //                bool isSaved = false;
        //                serviceFile.Register(() => FileService.SaveImageToDisk(Argument<string>.Any, Argument<BitmapSource>.Any))
        //                    .Execute(() =>
        //                    {
        //                        isSaved = true;
        //                        return true;
        //                    });
        //                vm.SaveToImageFile(ValidWorkflowItem);
        //                Assert.IsTrue(isSaved);
        //            }
        //        }
        //    }
        //}

        //[WorkItem(321144)]
        //[TestCategory("Unit-Dif")]
        //[Owner("v-kason")]
        //[TestMethod]
        //public void CheckPrintCanExecute()
        //{
        //    TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
        //    {
        //        using (var viewModel = new Implementation<MainWindowViewModel>())
        //        {
        //            viewModel.Register(inst => inst.SaveWorkflowToBitmap(Argument<WorkflowItem>.Any)).Execute(() =>
        //            {
        //                return BitmapFrame.Create(new Uri("http://www.test.com"));
        //            });
        //            var vm = viewModel.Instance;
        //            var command = vm.PrintCommand;
        //            Assert.AreEqual(false, command.CanExecute()); // FocusedWorkflowItem is null

        //            vm.FocusedWorkflowItem = ValidWorkflowItem;
        //            if (!vm.WorkflowItems.Contains(vm.FocusedWorkflowItem))
        //                vm.WorkflowItems.Add(vm.FocusedWorkflowItem);
        //            ValidWorkflowItem.IsDataDirty = true;
        //            Assert.AreEqual(true, command.CanExecute()); // dirty is irrelevant to compile
        //        }
        //    });
        //}

        [Description("Verify that refresh is enabled only when workflow is loaded in the designer")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_CheckRefreshCanExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                //mock addin
                using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                {
                    var wf = ValidWorkflowItem;
                    List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                    workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                    workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                    workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                    workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.RefreshDesignerFromXamlCode()).Execute(() => { });

                    wf.WorkflowDesigner = workflowDesigner.Instance;
                    var vm = new MainWindowViewModel();
                    var command = vm.RefreshCommand;
                    Assert.AreEqual(false, command.CanExecute()); // FocusedWorkflowItem is null

                    vm.FocusedWorkflowItem = wf;
                    if (!vm.WorkflowItems.Contains(vm.FocusedWorkflowItem))
                        vm.WorkflowItems.Add(vm.FocusedWorkflowItem);
                    wf.IsDataDirty = true;

                    Assert.AreEqual(true, command.CanExecute()); // dirty is irrelevant to compile
                    vm.RefreshCommand.Execute();
                }
            });
        }

        [Description("Verify that refresh is enabled only when workflow is loaded in the designer")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_CheckUndoCanExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                //mock addin
                using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                {
                    var wf = ValidWorkflowItem;
                    List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                    workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                    workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                    workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                    workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.CanUndo()).Return(true);
                    workflowDesigner.Register(inst => inst.Undo()).Return();
                    
                    wf.WorkflowDesigner = workflowDesigner.Instance;

                    var vm = new MainWindowViewModel();
                    var command = vm.UndoCommand;
                    Assert.AreEqual(false, command.CanExecute()); // FocusedWorkflowItem is null

                    vm.FocusedWorkflowItem = wf;
                    if (!vm.WorkflowItems.Contains(vm.FocusedWorkflowItem))
                        vm.WorkflowItems.Add(vm.FocusedWorkflowItem);
                    vm.FocusedWorkflowItem.Name = "TestName";
                    wf.IsDataDirty = true;
                    Assert.AreEqual(true, command.CanExecute()); // dirty is irrelevant to compile
                    vm.UndoCommand.Execute();
                }
            });
        }

        [Description("Verify that refresh is enabled only when workflow is loaded in the designer")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_CheckRedoCanExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                //mock addin
                using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                {
                    var wf = ValidWorkflowItem;
                    List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                    workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                    workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                    workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                    workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.CanRedo()).Return(true);
                    workflowDesigner.Register(inst => inst.Redo()).Return();
                    
                    wf.WorkflowDesigner = workflowDesigner.Instance;
                    
                    var vm = new MainWindowViewModel();
                    var command = vm.RedoCommand;
                    Assert.AreEqual(false, command.CanExecute()); // FocusedWorkflowItem is null

                    vm.FocusedWorkflowItem = wf;
                    if (!vm.WorkflowItems.Contains(vm.FocusedWorkflowItem))
                        vm.WorkflowItems.Add(vm.FocusedWorkflowItem);
                    wf.IsDataDirty = true;
                    vm.FocusedWorkflowItem.Name = "test_name";
                    Assert.AreEqual(true, command.CanExecute()); // dirty is irrelevant to compile
                    vm.RedoCommand.Execute();
                }
            });
        }

        [Description("Verify that about command is always true")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_CheckAboutCommandCanExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                var vm = new MainWindowViewModel();
                var command = vm.ShowAboutViewCommand;
                bool isShow = false;
                DialogService.ShowDialogFunc = (view) =>
                {
                    isShow = true;
                    return true;
                };
                command.Execute();
                Assert.IsTrue(isShow);
            });
        }

        [Description("Verify that import is always enabled")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_VerifyImportCanExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                //mock addin
                using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                {
                    var wf = ValidWorkflowItem;
                    List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                    workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                    workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                    workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                    workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                    wf.WorkflowDesigner = workflowDesigner.Instance;
                    var vm = new MainWindowViewModel();
                    var command = vm.ImportAssemblyCommand;
                    Assert.AreEqual(false, command.CanExecute()); // FocusedWorkflowItem is null

                    vm.FocusedWorkflowItem = wf;
                    Assert.AreEqual(true, command.CanExecute()); // dirty is irrelevant to compile
                }
            });
        }


        [Description("Verify that cut is enabled only if some workflow is loaded")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_CheckCutCanExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                //mock addin
                using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                {
                    var wf = ValidWorkflowItem;
                    List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                    workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                    workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                    workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                    workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.CanCut()).Return(true);
                    workflowDesigner.Register(inst => inst.Cut()).Return();
                    
                    wf.WorkflowDesigner = workflowDesigner.Instance;
                    var vm = new MainWindowViewModel();
                    var command = vm.CutCommand;
                    Assert.AreEqual(false, command.CanExecute()); // FocusedWorkflowItem is null

                    vm.FocusedWorkflowItem = wf;
                    vm.WorkflowItems.Add(vm.FocusedWorkflowItem);
                    wf.IsDataDirty = true;
                    Assert.IsTrue(command.CanExecute());
                }
            });
        }

        [Description("Verify that copy is enabled only if some workflow is loaded")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_CheckCopyCanExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                //mock addin
                using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                {
                    var wf = ValidWorkflowItem;
                    List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                    workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                    workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                    workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                    workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.CanCopy()).Return(true);
                    workflowDesigner.Register(inst => inst.Copy()).Return();
                    
                    wf.WorkflowDesigner = workflowDesigner.Instance;
                    
                    var vm = new MainWindowViewModel();
                    var command = vm.CopyCommand;
                    Assert.AreEqual(false, command.CanExecute()); // FocusedWorkflowItem is null

                    vm.FocusedWorkflowItem = wf;
                    vm.WorkflowItems.Add(vm.FocusedWorkflowItem);
                    wf.IsDataDirty = true;
                    bool canExecute = command.CanExecute();
                    Assert.IsTrue(canExecute);
                    command.Execute();
                }
            });
        }

        [Description("Verify that paste is enabled only if some workflow is loaded")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_CheckPasteCanExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                //mock addin
                using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                {
                    var wf = ValidWorkflowItem;
                    List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                    workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                    workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                    workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                    workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                    workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                    workflowDesigner.Register(inst => inst.CanPaste()).Return(true);
                    workflowDesigner.Register(inst => inst.Paste()).Return();
                    wf.WorkflowDesigner = workflowDesigner.Instance;
                    
                    var vm = new MainWindowViewModel();
                    var command = vm.PasteCommand;
                    Assert.AreEqual(false, command.CanExecute()); // FocusedWorkflowItem is null

                    vm.FocusedWorkflowItem = wf;
                    vm.WorkflowItems.Add(vm.FocusedWorkflowItem);
                    wf.IsDataDirty = true;
                    bool canExecute = command.CanExecute();
                    Assert.IsTrue(canExecute);
                    command.Execute();
                }
            });
        }

        [WorkItem(322359)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_VerifySelectAssemblyAndActivityCommandExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
             {
                 WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                 var vm = new MainWindowViewModel();
                 bool isCommandExecute = false;
                 using (new CachingIsolator())
                 {
                     using (var client = new Implementation<WorkflowsQueryServiceClient>())
                     {
                         client.Register(inst => inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any)).Execute(() =>
                         {
                             GetAllActivityLibrariesReplyDC reply = new GetAllActivityLibrariesReplyDC();
                             reply.List = new List<ActivityLibraryDC>();
                             return reply;
                         });
                         WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                         using (var service = new ImplementationOfType(typeof(DialogService)))
                         {
                             service.Register(() => DialogService.ShowDialog(Argument<SelectAssemblyAndActivityViewModel>.Any)).Execute(() =>
                             {
                                 bool? result = true;
                                 isCommandExecute = true;
                                 return result;
                             });
                             vm.SelectAssemblyAndActivityCommand.Execute();
                             Assert.IsTrue(isCommandExecute);
                         }
                         WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                     }
                 }
             });
        }

        [WorkItem(322369)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_VerifyUploadAssemblyCommandExecute()
        {
            var vm = new MainWindowViewModel();
            bool isCommandExecute = false;
            using (new CachingIsolator(TestInputs.ActivityAssemblyItems.TestInput_Lib1))
            {
                using (var service = new ImplementationOfType(typeof(DialogService)))
                {
                    service.Register(() => DialogService.ShowDialog(Argument<UploadAssemblyViewModel>.Any)).Execute(() =>
                    {
                        bool? result = true;
                        isCommandExecute = true;
                        return result;
                    });
                    vm.UploadAssemblyCommand.Execute();
                    Assert.IsTrue(isCommandExecute);
                }
            }
        }

        [WorkItem(322365)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_VerifyShowClickableMessageCommandExecute()
        {
            var vm = new MainWindowViewModel();
            bool isCommandExecute = false;
            using (var service = new ImplementationOfType(typeof(DialogService)))
            {
                service.Register(() => DialogService.ShowDialog(Argument<ClickableMessageViewModel>.Any)).Execute(() =>
                {
                    bool? result = true;
                    isCommandExecute = true;
                    return result;
                });
                vm.ShowClickableMessageCommand.Execute();
                Assert.IsTrue(isCommandExecute);
            }
        }

        [WorkItem(322355)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_VerifySaveToLocal()
        {
            //mock addin
            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
            {
                var wf = ValidWorkflowItem;
                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                workflowDesigner.Register(inst => inst.SaveWorkflowToBitmap(Argument<string>.Any)).Return();
                workflowDesigner.Register(inst => inst.Save(Argument<string>.Any)).Return();
                wf.WorkflowDesigner = workflowDesigner.Instance;

                bool forceSaveAs = true;

                var vm = new MainWindowViewModel();
                using (var service = new ImplementationOfType(typeof(DialogService)))
                {
                    //Save as .wf
                    bool isSaveAswf = false;
                    string wfName = ".\\wf";

                    service.Register(() => DialogService.ShowSaveDialogAndReturnResult(Argument<string>.Any, Argument<string>.Any)).Execute(() =>
                    {
                        return wfName;
                    });
                    using (var file = new Implementation<BinaryFormatter>())
                    {
                        file.Register(inst => inst.Serialize(Argument<Stream>.Any, Argument<object>.Any)).Execute(() =>
                        {
                            isSaveAswf = true;
                        });
                        vm.SaveToLocal(wf, forceSaveAs);
                        if (File.Exists(wfName + "..wf"))
                            isSaveAswf = true;
                        Assert.IsTrue(isSaveAswf);
                    }

                    //Save as .jpg
                    service.Register(() => DialogService.ShowSaveDialogAndReturnResult(Argument<string>.Any, Argument<string>.Any)).Execute(() =>
                    {
                        return ".\\wf.jpg";
                    });

                    bool isSaveAsJpg = false;
                    using (var fileService = new ImplementationOfType(typeof(FileService)))
                    {
                        fileService.Register(() => FileService.SaveImageToDisk(Argument<string>.Any, Argument<BitmapSource>.Any)).Execute(() =>
                        {
                            isSaveAsJpg = true;
                            return true;
                        });
                        vm.SaveToLocal(wf, forceSaveAs);
                        Assert.IsFalse(isSaveAsJpg);
                    }

                    //Save as Xaml
                    bool isSaveXaml = false;
                    service.Register(() => DialogService.ShowSaveDialogAndReturnResult(Argument<string>.Any, Argument<string>.Any)).Execute(() =>
                    {
                        return ".\\wf.xaml";
                    });

                    using (var designer = new Implementation<WorkflowDesigner>())
                    {
                        designer.Register(inst => inst.Save(Argument<string>.Any)).Execute(() =>
                        {
                            isSaveXaml = true;
                        });
                        //wf.WorkflowDesigner = designer.Instance;
                        vm.SaveToLocal(wf, forceSaveAs);
                        Assert.IsFalse(isSaveXaml);
                    }
                }
            }
        }

        [WorkItem(322329)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_VerifyCloseWorkflowExecute()
        {
            //mock addin
            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
            {
                var wf = ValidWorkflowItem;
                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                bool isUnload = false;
                workflowDesigner.Register(inst => inst.UnloadAddIn()).Execute(() => { isUnload = true; });

                wf.WorkflowDesigner = workflowDesigner.Instance;

                TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                {
                     using (var viewModel = new Implementation<MainWindowViewModel>())
                     {
                         var isSaveToLocal = false;
                         var isSaveToServer = false;
                         var IsUnlock = false;
                         viewModel.Register(inst => inst.SaveToLocal(Argument<WorkflowItem>.Any, Argument<bool>.Any)).Execute(() =>
                         {
                             isSaveToLocal = true;
                             return true;
                         });
                         viewModel.Register(inst => inst.SaveToServer(Argument<WorkflowItem>.Any)).Execute(() =>
                         {
                             isSaveToServer = true;
                             return true;
                         });

                         viewModel.Register(inst => inst.StoreActivitesUnlock(Argument<WorkflowItem>.Any, Argument<bool>.Any)).Execute(() =>
                         {
                             IsUnlock = true;
                         });

                         var vm = viewModel.Instance;

                         vm.FocusedWorkflowItem = wf;
                         vm.WorkflowItems.Add(wf);

                         vm.FocusedWorkflowItem.IsReadOnly = true;
                         vm.CloseFocusedWorkflowCommand.Execute();
                         Assert.IsTrue(vm.WorkflowItems.Count == 0);
                         Assert.IsTrue(isUnload);

                         SavingResult? closeResult = null;
                         using (var messageBoxService = new ImplementationOfType(typeof(MessageBoxService)))
                         {
                             messageBoxService.Register(() => MessageBoxService.ShowClosingConfirmation(Argument<string>.Any)).Execute(() =>
                             {
                                 closeResult = SavingResult.Save;
                                 return closeResult;
                             });

                             messageBoxService.Register(() => MessageBoxService.ShowKeepLockedConfirmation(Argument<string>.Any)).Execute(() =>
                             {
                                 closeResult = SavingResult.Unlock;
                                 return closeResult;
                             });

                             messageBoxService.Register(() => MessageBoxService.ShowLocalSavingConfirmation(Argument<string>.Any)).Execute(() =>
                             {
                                 closeResult = SavingResult.Save;
                                 return closeResult;
                             });

                             wf.WorkflowDesigner = workflowDesigner.Instance;
                             vm.FocusedWorkflowItem = wf;
                             vm.FocusedWorkflowItem.IsReadOnly = false;
                             vm.FocusedWorkflowItem.IsOpenFromServer = true;
                             vm.FocusedWorkflowItem.IsDataDirty = true;
                             vm.WorkflowItems.Add(vm.FocusedWorkflowItem);
                             vm.CloseFocusedWorkflowCommand.Execute();
                             Assert.AreEqual(closeResult.Value, SavingResult.Save);
                             Assert.IsTrue(isSaveToServer);

                             wf.WorkflowDesigner = workflowDesigner.Instance;
                             vm.FocusedWorkflowItem = wf;
                             vm.WorkflowItems.Clear();
                             vm.WorkflowItems.Add(vm.FocusedWorkflowItem);
                             vm.FocusedWorkflowItem.IsOpenFromServer = true;
                             vm.FocusedWorkflowItem.IsDataDirty = false;
                             vm.CloseFocusedWorkflowCommand.Execute();
                             Assert.AreEqual(closeResult.Value, SavingResult.Unlock);
                             Assert.IsTrue(IsUnlock);

                             wf.WorkflowDesigner = workflowDesigner.Instance;
                             vm.FocusedWorkflowItem = wf;
                             vm.WorkflowItems.Clear();
                             vm.WorkflowItems.Add(vm.FocusedWorkflowItem);
                             vm.FocusedWorkflowItem.IsOpenFromServer = false;
                             vm.FocusedWorkflowItem.IsDataDirty = true;
                             vm.CloseFocusedWorkflowCommand.Execute();
                             Assert.AreEqual(closeResult.Value, SavingResult.Save);
                             Assert.IsTrue(isSaveToLocal);
                         }
                     }
                 });
            }
        }


        [WorkItem(322342)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_VerifyImportAssemblyExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
             {
                   //mock addin
                 using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                 {
                     var wf = ValidWorkflowItem;
                     List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                     workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                     workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                     workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                     workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                     workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                     workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                     workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                     workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                     workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                     workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                     workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                     wf.WorkflowDesigner = workflowDesigner.Instance;
                     
                     using (var dialogService = new ImplementationOfType(typeof(DialogService)))
                     {
                         bool? isexecute = false;
                         dialogService.Register(() => DialogService.ShowDialog(Argument<object>.Any)).Execute(() =>
                         {
                             isexecute = true;
                             return isexecute;
                         });
                         var vm = new MainWindowViewModel();
                         vm.FocusedWorkflowItem = wf;
                         var command = vm.ImportAssemblyCommand;
                         command.Execute();
                         Assert.IsTrue(isexecute.Value);
                     }
                 }
             });
        }

        [WorkItem(322348)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_VerifyOpenWorkflowCommandExecute()
        {
            TestUtilities.RegistLoginUserRole(Role.Author);
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {

                    TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                    using (var client = new Implementation<WorkflowsQueryServiceClient>())
                    {
                        ActivitySearchReplyDC dc = new ActivitySearchReplyDC();
                        dc.SearchResults = new List<StoreActivitiesDC>();
                        dc.ServerResultsLength = 0;
                        client.Register(inst => inst.SearchActivities(Argument<ActivitySearchRequestDC>.Any)).Return(dc);
                        WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                        using (var cache = new ImplementationOfType(typeof(Caching)))
                        {
                            List<ActivityAssemblyItem> list = new List<ActivityAssemblyItem>();
                            cache.Register(() => Caching.ComputeDependencies(client.Instance, Argument<ActivityAssemblyItem>.Any)).Execute(() =>
                            {
                                return list;
                            });

                            cache.Register(() => Caching.CacheAndDownloadAssembly(client.Instance, list)).Return(list);

                            cache.Register(() => Caching.Match(list)).Return(null);
                              //mock addin
                            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                            {
                                var wf = ValidWorkflowItem;
                                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                                workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                                workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                                workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                                workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                                workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                                workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                                workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                                workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                                workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                                workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                                wf.WorkflowDesigner = workflowDesigner.Instance;
                                
                                using (var viewModel = new Implementation<MainWindowViewModel>())
                                {
                                    var sdc = DataContractTranslator.ActivityItemToStoreActivitiyDC(wf);
                                    sdc.ActivityLibraryName = wf.Name;
                                    sdc.ActivityLibraryVersion = wf.Version;

                                    WorkflowItem workflow = null;
                                    viewModel.Register(inst => inst.OpenStoreActivitiesDC(Argument<StoreActivitiesDC>.Any, Argument<ActivityAssemblyItem>.Any, null, false, true)).Execute(() =>
                                    {
                                        workflow = DataContractTranslator.StoreActivitiyDCToWorkflowItem(sdc, null);
                                        workflow.IsOpenFromServer = true;
                                    });

                                    var vm = viewModel.Instance;
                                    var command = vm.OpenWorkflowCommand;

                                    DialogService.ShowDialogFunc = (view) =>
                                    {
                                        var pvm = new PrivateObject(view);
                                        pvm.SetFieldOrProperty("SelectedWorkflow", sdc);
                                        pvm.SetFieldOrProperty("ShouldDownloadDependencies", false);
                                        return true;
                                    };

                                    client.Register(c => c.StoreActivitiesUpdateLock(Argument<StoreActivitiesDC>.Any, Argument<DateTime>.Any)).Return(new StatusReplyDC());
                                    command.Execute("FromServer");
                                    Assert.IsTrue(vm.FocusedWorkflowItem.IsOpenFromServer);
                                    Assert.AreEqual(vm.FocusedWorkflowItem.Name, wf.Name);
                                    Assert.AreEqual(vm.FocusedWorkflowItem.Version, wf.Version);

                                    vm.WorkflowItems.Clear();
                                    using (var dialogService = new ImplementationOfType(typeof(DialogService)))
                                    {
                                        string fileName = string.Empty;
                                        dialogService.Register(() => DialogService.ShowOpenFileDialogAndReturnResult(Argument<string>.Any, Argument<string>.Any)).Execute(() =>
                                        {
                                            fileName = "D:\\open";
                                            return fileName;
                                        });
                                        using (var utility = new ImplementationOfType(typeof(Utility)))
                                        {
                                            utility.Register(() => Utility.DeserializeSavedContent(Argument<string>.Any)).Execute(() =>
                                            {
                                                var content = wf as object;
                                                return content;
                                            });

                                            command.Execute("FromLocal");
                                            Assert.IsFalse(vm.FocusedWorkflowItem.IsReadOnly);
                                            Assert.IsFalse(vm.FocusedWorkflowItem.IsDataDirty);
                                            Assert.IsFalse(vm.FocusedWorkflowItem.IsOpenFromServer);
                                        }
                                    }

                                    //Verify Open Workflow With Exception
                                    PrivateObject po = new PrivateObject(vm);
                                    try { po.Invoke("OpenWorkflowFromLocal", string.Empty); }
                                    catch (ArgumentNullException expect)
                                    {
                                        Assert.AreEqual(expect.ParamName, "fileName");
                                    }
                                }
                            }
                        }
                    }
                TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
            });
        }

        [WorkItem(322354)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_VerifySaveFocusedWorkflowCommandExecute()
        {
            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
            {
                var wf = ValidWorkflowItem;
                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                wf.WorkflowDesigner = workflowDesigner.Instance;
                using (var viewModel = new Implementation<MainWindowViewModel>())
                {
                    bool isSaveToLocal = false;
                    viewModel.Register(inst => inst.SaveToLocal(Argument<WorkflowItem>.Any, Argument<bool>.Any)).Execute(() =>
                    {
                        isSaveToLocal = true;
                        return true;
                    });

                    bool isSaveToServer = false;
                    viewModel.Register(inst => inst.SaveToServer(Argument<WorkflowItem>.Any)).Execute(() =>
                    {
                        isSaveToServer = true;
                        return true;
                    });

                    bool isSaveToImage = false;
                    viewModel.Register(inst => inst.SaveToImageFile(Argument<WorkflowItem>.Any)).Execute(() =>
                    {
                        isSaveToImage = true;
                    });
                    var vm = viewModel.Instance;
                    vm.FocusedWorkflowItem = wf;
                    vm.SaveFocusedWorkflowCommandExecute("ToLocal");
                    Assert.IsTrue(isSaveToLocal);

                    vm.SaveFocusedWorkflowCommandExecute("ToServer");
                    Assert.IsTrue(isSaveToServer);

                    vm.SaveFocusedWorkflowCommandExecute("ToMarketplace");
                    Assert.AreEqual(vm.FocusedWorkflowItem.Status, MarketplaceStatus.Public.ToString());

                    vm.SaveFocusedWorkflowCommandExecute("ToImage");
                    Assert.IsTrue(isSaveToImage);
                }
            }
        }

        [Description("Verify that the MainWindow can be closed only when there are no dirty items or the user explicitly okays it")]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MainWindow_CheckShouldCancelExit()
        {
            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
            {
                var wf = ValidWorkflowItem;
                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                workflowDesigner.Register(inst => inst.UnloadAddIn()).Return();
                wf.WorkflowDesigner = workflowDesigner.Instance;
                // No workflow items (ok to close)
                var vm = new MainWindowViewModel();
                Assert.AreEqual(false, vm.CheckShouldCancelExit());

                wf.IsReadOnly = false;
                wf.IsOpenFromServer = true;
                wf.IsDataDirty = false;
                vm.WorkflowItems.Add(wf);
                MessageBoxService.ShowSavingConfirmationFunc = (a, b, c, d, e) => SavingResult.DoNothing;
                Assert.AreEqual(false, vm.CheckShouldCancelExit());

                // Dirty item (no to close)
                wf.IsReadOnly = true;
                wf.IsOpenFromServer = false;
                wf.IsDataDirty = true;
                Assert.AreEqual(false, vm.CheckShouldCancelExit());

                // Dirty item (cancel not to close)
                wf.IsReadOnly = false;
                wf.IsOpenFromServer = true;
                wf.IsDataDirty = true;
                vm.WorkflowItems.Add(wf);
                MessageBoxService.ShowSavingConfirmationFunc = (a, b, c, d, e) => null;
                Assert.AreEqual(true, vm.CheckShouldCancelExit());
            }
        }

        [WorkItem(321674)]
        [Description("Verify open a StoreActivitiesDC from server.")]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_CheckOpenStoreActivitiesDC()
        {
            TestUtilities.RegistLoginUserRole(Role.Author);
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
               using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
               {
                   var wf = ValidWorkflowItem;
                   List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                   workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                   workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                   workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                   workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                   workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                   workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                   workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                   workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                   workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                   workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                   workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                   wf.WorkflowDesigner = workflowDesigner.Instance;
                   // Arrange
                   var vm = new MainWindowViewModel();
                   vm.FocusedWorkflowItem = wf;
                   
                   StoreActivitiesDC activities = DataContractTranslator.ActivityItemToStoreActivitiyDC(vm.FocusedWorkflowItem);
                   using (var push = new ImplementationOfType(typeof(Dispatcher)))
                   {
                       push.Register(() => Dispatcher.PushFrame(Argument<DispatcherFrame>.Any)).Execute(() => { });

                       //test by author
                       using (var principal = new Implementation<WindowsPrincipal>())
                       {
                           Thread.CurrentPrincipal = principal.Instance;

                           using (var messageBox = new ImplementationOfType(typeof(MessageBoxService)))
                           {
                               MessageBoxResult result = MessageBoxResult.None;
                               messageBox.Register(() => MessageBoxService.OpenLockedActivityByNonAdmin(Argument<string>.Any))
                                   .Execute(() =>
                                   {
                                       result = MessageBoxResult.OK;
                                   });
                               activities.Locked = true;
                               activities.LockedBy = Environment.UserName + "1";

                               vm.OpenStoreActivitiesDC(activities, null);
                               Assert.AreEqual(result, MessageBoxResult.OK);
                           }
                       }
                   }
               }
           });
        }

        [WorkItem(368688)]
        [Owner("v-toy")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MainWindow_PublishCommandExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var client = new Implementation<WorkflowsQueryServiceClient>())
                {
                    using (var viewModel = new Implementation<MainWindowViewModel>())
                    {
                        var vm = viewModel.Instance;

                        //mock addin
                        using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                        {
                            var wf = ValidWorkflowItem;
                            List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                            workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                            workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                            workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                            workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                            workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                            workflowDesigner.Register(inst => inst.XamlCode).Return(wf.XamlCode);
                            workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                            workflowDesigner.Register(inst => inst.IsValid()).Return(true);
                            workflowDesigner.Register(inst => inst.CompiledXaml).Return(wf.XamlCode);
                            workflowDesigner.Register(inst => inst.SetReadOnly(Argument<bool>.Any)).Execute(() => { });
                            workflowDesigner.Register(inst => inst.SetWorkflowName(Argument<string>.Any)).Execute(() => { });
                            workflowDesigner.Register(inst => inst.Tasks).Return(new List<TaskAssignment>());
                            workflowDesigner.Register(inst => inst.FinishTaskAssigned()).Execute(() => { });
                            CompileProject cp = new CompileProject();
                            cp.ProjectXaml = wf.XamlCode;
                            workflowDesigner.Register(inst => inst.CompileProject).Return(cp);
                            wf.WorkflowDesigner = workflowDesigner.Instance;

                            WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                            viewModel.Register(inst => inst.UploadWorkflow(client.Instance, Argument<WorkflowItem>.Any, false)).Return(true);
                            viewModel.Register(inst => inst.PublishCommandExecute_Implementation(client.Instance, Argument<WorkflowItem>.Any));
                            vm.FocusedWorkflowItem = wf;

                            MessageBoxResult messageBoxResult = new MessageBoxResult();
                            PrivateObject privateInstance = new PrivateObject(vm);
                            MainWindowViewModel_Accessor accessor = new MainWindowViewModel_Accessor(privateInstance);

                            using (var messageBoxService = new ImplementationOfType(typeof(MessageBoxService)))
                            {
                                messageBoxService.Register(() => MessageBoxService.Show(
                                Argument<string>.Any,
                                Argument<string>.Any,
                                Argument<MessageBoxButton>.Any,
                                Argument<MessageBoxImage>.Any)).Execute(() =>
                                {
                                    messageBoxResult = MessageBoxResult.OK;
                                    return messageBoxResult;
                                });

                                client.Register(c => c.PublishWorkflow(Argument<PublishingRequest>.Any)).
                                    Return(new PublishingReply() { StatusReply = new StatusReplyDC() { Errorcode = 1 } });
                                accessor.PublishCommandExecute();

                                Assert.AreEqual(MessageBoxResult.OK, messageBoxResult);
                            }

                            using (var messageBoxService = new ImplementationOfType(typeof(MessageBoxService)))
                            {
                                messageBoxService.Register(() => MessageBoxService.ShowClickable(Argument<string>.Any,Argument<string>.Any,Argument<string>.Any)).Execute(() =>
                                {
                                    messageBoxResult = MessageBoxResult.OK;
                                    return messageBoxResult;
                                });

                                client.Register(c => c.PublishWorkflow(Argument<PublishingRequest>.Any)).
                                    Return(new PublishingReply() { StatusReply = new StatusReplyDC() { Errorcode = 0 }, PublishErrors = "PublishErrors" });
                                accessor.PublishCommandExecute();

                                Assert.AreEqual(MessageBoxResult.OK, messageBoxResult);
                            }

                            using (var messageBoxService = new ImplementationOfType(typeof(MessageBoxService)))
                            {
                                messageBoxService.Register(() => MessageBoxService.ShowClickable(Argument<string>.Any, Argument<string>.Any, Argument<string>.Any)).Execute(() =>
                                {
                                    messageBoxResult = MessageBoxResult.OK;
                                    return messageBoxResult;
                                });

                                client.Register(c => c.PublishWorkflow(Argument<PublishingRequest>.Any)).
                                    Return(new PublishingReply() { StatusReply = new StatusReplyDC() { Errorcode = 0 }});
                                accessor.PublishCommandExecute();

                                Assert.AreEqual(MessageBoxResult.OK, messageBoxResult);
                            }

                        }
                    }
                }
            });
        }


    }
}
