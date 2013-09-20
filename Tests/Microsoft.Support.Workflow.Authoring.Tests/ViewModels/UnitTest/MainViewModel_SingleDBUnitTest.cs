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
using Microsoft.Support.Workflow.Authoring.AssetStore;
using Microsoft.Support.Workflow.Authoring.Tests.ViewModels.UnitTest;
using System.DirectoryServices.AccountManagement;

namespace Authoring.Tests.Unit
{
    public partial class MainWindowViewModel_UnitTests
    {
        private void MockWorkflowDesigner(Action action, WorkflowItem workflowItem)
        {
            using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
            {
                var wf = workflowItem;
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
                workflowDesigner.Register(inst => inst.HasTask).Return(false);
                workflowDesigner.Register(inst => inst.UnloadAddIn()).Execute(() => { });
                wf.WorkflowDesigner = workflowDesigner.Instance;
                action();
            }
        }

        [TestMethod]
        [TestCategory("Unit_Dif")]
        [Owner("v-kason")]
        public void MainWindowViewModel_TestOpenTetantSecurityOptionsCommand()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var dialogService = new ImplementationOfType(typeof(DialogService)))
                {
                    bool? isexecute = false;
                    dialogService.Register(() => DialogService.ShowDialog(Argument<object>.Any)).Execute(() =>
                    {
                        isexecute = true;
                        return isexecute;
                    });
                    ChangeAuthorViewModelUnitTest.SetAuthorizationServicePermessionList();
                    ImplementationOfType imp = null;
                    TestUtilities.MockAssetStoreProxy(ref imp, () =>
                    {
                        TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                        {
                            using (var client = new Implementation<WorkflowsQueryServiceClient>())
                            {
                                AuthorizationGroupGetReplyDC reply = new AuthorizationGroupGetReplyDC();
                                reply.StatusReply = new StatusReplyDC();
                                reply.AuthorizationGroups = new List<AuthorizationGroupDC>();
                                client.Register(inst => inst.GetAuthorizationGroups(Argument<AuthorizationGroupGetRequestDC>.Any))
                                    .Return(reply);
                                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                                var vm = new MainWindowViewModel();
                                TestUtilities.RegistLoginUserRole(Role.Admin);
                                Assert.IsFalse(vm.OpenTenantSecurityOptionsCommand.CanExecute());
                                TestUtilities.RegistLoginUserRole(Role.CWFEnvAdmin);
                                Assert.IsTrue(vm.OpenTenantSecurityOptionsCommand.CanExecute());
                                vm.OpenTenantSecurityOptionsCommand.Execute();
                                Assert.IsTrue(isexecute.Value);
                                TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                            }
                        });
                    });
                }
            });
        }

        [TestMethod]
        [TestCategory("Unit_Dif")]
        [Owner("v-kason")]
        public void MainWindowViewModel_TestOpenEnvSecurityOptionsCommand()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                //var wf = this.ValidWorkflowItem;
                ChangeAuthorViewModelUnitTest.SetAuthorizationServicePermessionList();
                using (var dialogService = new ImplementationOfType(typeof(DialogService)))
                {
                    bool? isexecute = false;
                    dialogService.Register(() => DialogService.ShowDialog(Argument<object>.Any)).Execute(() =>
                    {
                        isexecute = true;
                        return isexecute;
                    });
                    ImplementationOfType imp = null;
                    TestUtilities.MockAssetStoreProxy(ref imp, () =>
                    {
                        TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                        {
                            using (var client = new Implementation<WorkflowsQueryServiceClient>())
                            {
                                AuthorizationGroupGetReplyDC reply = new AuthorizationGroupGetReplyDC();
                                reply.StatusReply = new StatusReplyDC();
                                reply.AuthorizationGroups = new List<AuthorizationGroupDC>();
                                client.Register(inst => inst.GetAuthorizationGroups(Argument<AuthorizationGroupGetRequestDC>.Any))
                                    .Return(reply);
                                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                                var vm = new MainWindowViewModel();
                                TestUtilities.RegistLoginUserRole(Role.Admin);
                                Assert.IsFalse(vm.OpenEnvSecurityOptionsCommand.CanExecute());
                                TestUtilities.RegistLoginUserRole(Role.CWFAdmin);
                                Assert.IsTrue(vm.OpenEnvSecurityOptionsCommand.CanExecute());
                                vm.OpenEnvSecurityOptionsCommand.Execute();
                                Assert.IsTrue(isexecute.Value);
                                TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                            }
                        });
                    });
                }
            });
        }

        [TestMethod]
        [TestCategory("Unit_Dif")]
        [Owner("v-kason")]
        public void MainWindowViewModel_TestCopyProjectCommandExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                var wf = this.ValidWorkflowItem;
                this.MockWorkflowDesigner(() =>
                {
                    using (var dialogService = new ImplementationOfType(typeof(DialogService)))
                    {
                        bool? isexecute = false;
                        dialogService.Register(() => DialogService.ShowDialog(Argument<object>.Any))
                            .Execute<object, bool?>((c) =>
                            {
                                var v = c as CopyCurrentProjectViewModel;
                                v.CopiedActivity = null;
                                isexecute = true;
                                return isexecute;
                            });

                        var vm = new MainWindowViewModel();
                        //workflow = null
                        Assert.IsFalse(vm.CopyProjectCommand.CanExecute());
                        vm.FocusedWorkflowItem = wf;
                        vm.FocusedWorkflowItem.IsDataDirty = false;
                        Assert.IsFalse(vm.CopyProjectCommand.CanExecute());
                        vm.FocusedWorkflowItem.TaskActivityGuid = Guid.NewGuid();
                        Assert.IsFalse(vm.CopyProjectCommand.CanExecute());
                        vm.FocusedWorkflowItem.TaskActivityGuid = new Nullable<Guid>();
                        vm.FocusedWorkflowItem.IsOpenFromServer = true;
                        TestUtilities.RegistLoginUserRole(Role.Admin);
                        vm.FocusedWorkflowItem.Env = Env.Dev;
                        Assert.IsTrue(vm.CopyProjectCommand.CanExecute());
                        vm.CopyProjectCommand.Execute();
                        Assert.IsTrue(isexecute.Value);
                    }
                }, wf);
            });
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void MainWindowViewModel_TestMoveProjectCommandExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                var wf = this.ValidWorkflowItem;
                this.MockWorkflowDesigner(() =>
                {
                    ImplementationOfType imp = null;
                    TestUtilities.MockAssetStoreProxy(ref imp, () =>
                    {
                        imp.Register(() => AssetStoreProxy.WorkflowTypes)
                            .Return(new ObservableCollection<WorkflowTypesGetBase>());

                        using (var dialogService = new ImplementationOfType(typeof(DialogService)))
                        {
                            bool? isexecute = false;
                            dialogService.Register(() => DialogService.ShowDialog(Argument<object>.Any))
                                .Execute<object, bool?>((c) =>
                                {
                                    var v = c as MoveProjectViewModel;
                                    v.NextStatus = Env.Test;
                                    isexecute = true;
                                    return isexecute;
                                });

                            var vm = new MainWindowViewModel();
                            //workflow = null
                            Assert.IsFalse(vm.MoveProjectCommand.CanExecute());
                            vm.FocusedWorkflowItem = wf;
                            vm.FocusedWorkflowItem.IsDataDirty = false;
                            Assert.IsFalse(vm.MoveProjectCommand.CanExecute());
                            vm.FocusedWorkflowItem.TaskActivityGuid = Guid.NewGuid();
                            Assert.IsFalse(vm.MoveProjectCommand.CanExecute());
                            vm.FocusedWorkflowItem.TaskActivityGuid = new Nullable<Guid>();
                            vm.FocusedWorkflowItem.IsOpenFromServer = true;
                            TestUtilities.RegistLoginUserRole(Role.Admin);
                            vm.FocusedWorkflowItem.Env = Env.Dev;
                            Assert.IsTrue(vm.MoveProjectCommand.CanExecute());
                            vm.MoveProjectCommand.Execute();
                            Assert.IsTrue(isexecute.Value);
                        }
                    });
                }, wf);
            });
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void MainWindowViewModel_TestChangeProjectAuthorCommandExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                var wf = this.ValidWorkflowItem;
                ChangeAuthorViewModelUnitTest.SetAuthorizationServicePermessionList();
                this.MockWorkflowDesigner(() =>
                {
                    using (var dialogService = new ImplementationOfType(typeof(DialogService)))
                    {

                        bool? isexecute = false;
                        dialogService.Register(() => DialogService.ShowDialog(Argument<object>.Any))
                            .Execute<object, bool?>((v) =>
                            {
                                ChangeAuthorViewModel c = v as ChangeAuthorViewModel;
                                Principal principal = System.DirectoryServices.AccountManagement.UserPrincipal.Current;
                                c.TargetAuthor = principal;
                                isexecute = true;
                                return isexecute;
                            });

                        var vm = new MainWindowViewModel();
                        //workflow = null
                        Assert.IsFalse(vm.ChangeAuthorCommand.CanExecute());
                        vm.FocusedWorkflowItem = wf;
                        vm.FocusedWorkflowItem.IsDataDirty = false;
                        Assert.IsFalse(vm.ChangeAuthorCommand.CanExecute());
                        vm.FocusedWorkflowItem.TaskActivityGuid = Guid.NewGuid();
                        Assert.IsFalse(vm.ChangeAuthorCommand.CanExecute());
                        vm.FocusedWorkflowItem.TaskActivityGuid = new Nullable<Guid>();
                        vm.FocusedWorkflowItem.IsOpenFromServer = true;
                        TestUtilities.RegistLoginUserRole(Role.Admin);
                        vm.FocusedWorkflowItem.Env = Env.Dev;
                        Assert.IsTrue(vm.ChangeAuthorCommand.CanExecute());
                        vm.ChangeAuthorCommand.Execute();
                        Assert.IsTrue(isexecute.Value);
                    }
                }, wf);
            });
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void MainWindowViewModel_TestDeleteProjectCommandExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                var wf = this.ValidWorkflowItem;
                wf.Env = Env.Dev;
                using (var messageBox = new ImplementationOfType(typeof(MessageBoxService)))
                {
                    bool shouldDelete = false;
                    messageBox.Register(() => MessageBoxService.ShouldDeleteWorkflow())
                                    .Execute(() =>
                                    {
                                        shouldDelete = true;
                                        return shouldDelete;
                                    });
                    TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                   {
                       using (var client = new Implementation<WorkflowsQueryServiceClient>())
                       {
                           StoreActivitiesDC reply = new StoreActivitiesDC();
                           reply.StatusReply = new StatusReplyDC();
                           client.Register(inst => inst.ActivityDelete(Argument<StoreActivitiesDC>.Any))
                               .Return(reply);
                           WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                           var vm = new MainWindowViewModel();
                           //workflow = null
                           Assert.IsFalse(vm.DeleteProjectCommand.CanExecute());
                           vm.FocusedWorkflowItem = wf;
                           vm.FocusedWorkflowItem.IsDataDirty = false;
                           Assert.IsFalse(vm.DeleteProjectCommand.CanExecute());
                           vm.FocusedWorkflowItem.TaskActivityGuid = Guid.NewGuid();
                           Assert.IsFalse(vm.DeleteProjectCommand.CanExecute());
                           vm.FocusedWorkflowItem.TaskActivityGuid = new Nullable<Guid>();
                           vm.FocusedWorkflowItem.IsOpenFromServer = true;
                           TestUtilities.RegistLoginUserRole(Role.Admin);
                           vm.FocusedWorkflowItem.Env = Env.Dev;
                           Assert.IsTrue(vm.DeleteProjectCommand.CanExecute());
                           vm.DeleteProjectCommand.Execute();
                           Assert.IsNull(vm.FocusedWorkflowItem);
                           WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                       }
                   });
                }
            });
        }
    }
}
