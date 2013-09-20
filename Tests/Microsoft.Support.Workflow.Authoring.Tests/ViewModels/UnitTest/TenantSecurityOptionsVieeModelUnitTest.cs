using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using System.Windows.Data;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Collections;
using Microsoft.DynamicImplementations;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using System.Collections.ObjectModel;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.UnitTest
{
    [TestClass]
    public class TenantSecurityOptionsViewModelUnitTest
    {
        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void TenantSecurityOptionsViewModel_TestPropertyChanged()
        {
            var vm = new TenantSecurityOptionsViewModel();
            Assert.IsFalse(vm.CanSave);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "HasChanged", () => vm.HasChanged = true);
            Assert.IsTrue(vm.HasChanged);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedDisenableViewerGroup", ()=>vm.SelectedDisenableViewerGroup = null);
            Assert.IsNull(vm.SelectedDisenableViewerGroup);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedDisenableAuthorGroup", () => vm.SelectedDisenableAuthorGroup = null);
            Assert.IsNull(vm.SelectedDisenableAuthorGroup);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedDisenableAdminGroup", () => vm.SelectedDisenableAdminGroup = null);
            Assert.IsNull(vm.SelectedDisenableAdminGroup);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "ViewerGroupsDisEnabled", () => vm.ViewerGroupsDisEnabled = null);
            Assert.IsNull(vm.ViewerGroupsDisEnabled);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "AuthorGroupsDisEnabled", () => vm.AuthorGroupsDisEnabled = null);
            Assert.IsNull(vm.AuthorGroupsDisEnabled);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "AdminGroupsDisEnabled", () => vm.AdminGroupsDisEnabled = null);
            Assert.IsNull(vm.AdminGroupsDisEnabled);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "ViewerGroupsEnabled", () => vm.ViewerGroupsEnabled = null);
            Assert.IsNull(vm.ViewerGroupsEnabled);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "AuthorGroupsEnabled", () => vm.AuthorGroupsEnabled = null);
            Assert.IsNull(vm.AuthorGroupsEnabled);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "AdminGroupsEnabled", () => vm.AdminGroupsEnabled = null);
            Assert.IsNull(vm.AdminGroupsEnabled);

        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        public void TenantSecurityOptionsViewModel_TestLoadData()
        {
            var vm = new TenantSecurityOptionsViewModel();
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {

                using (var client = new Implementation<WorkflowsQueryServiceClient>())
                {
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                    AuthorizationGroupGetReplyDC reply = new AuthorizationGroupGetReplyDC();
                    reply.StatusReply = new StatusReplyDC();
                    reply.AuthorizationGroups = GetFakeAuthorGroupDCs();
                    client.Register(inst => inst.GetAuthorizationGroups(Argument<AuthorizationGroupGetRequestDC>.Any))
                        .Return(reply);
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                    vm.LoadLiveData();
                    Assert.IsTrue(vm.AdminGroupsDisEnabled.Count > 0);
                    Assert.IsTrue(vm.AuthorGroupsDisEnabled.Count > 0);
                    Assert.IsTrue(vm.ViewerGroupsDisEnabled.Count > 0);
                    Assert.IsTrue(vm.AdminGroupsEnabled.Count > 0);
                    Assert.IsTrue(vm.AuthorGroupsEnabled.Count > 0);
                    Assert.IsTrue(vm.ViewerGroupsEnabled.Count > 0);
                    TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                }
            });
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        public void TenantSecurityOptionsViewModel_TestSaveCommandExecute()
        {
            var vm = new TenantSecurityOptionsViewModel();
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {

                using (var client = new Implementation<WorkflowsQueryServiceClient>())
                {
                    AuthGroupsEnableOrDisableReplyDC reply = new AuthGroupsEnableOrDisableReplyDC();
                    reply.StatusReply = new StatusReplyDC();

                    client.Register(inst => inst.AuthorizationGroupEnableOrDisable(Argument<AuthGroupsEnableOrDisableRequestDC>.Any))
                        .Return(reply);

                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                    using (var impDialog = new ImplementationOfType(typeof(MessageBoxService)))
                    {
                        bool isShowInfo = false;
                        impDialog.Register(() => MessageBoxService.ShowInfo(Argument<string>.Any)).Execute(() => { isShowInfo = true; return MessageBoxResult.OK; });

                        var groups = this.GetFakeAuthorGroupDCs();
                        vm.AdminGroupsEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Admin && a.Enabled));
                        vm.AuthorGroupsEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Author && a.Enabled));
                        vm.ViewerGroupsEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Viewer && a.Enabled));
                        vm.AdminGroupsDisEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Admin && !a.Enabled));
                        vm.AuthorGroupsDisEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Author && !a.Enabled));
                        vm.ViewerGroupsDisEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Viewer && !a.Enabled));
                        vm.SaveCommand.Execute();
                        Assert.IsTrue(isShowInfo);
                        Assert.IsFalse(vm.HasChanged);
                        Assert.IsTrue(vm.HasSaved);
                    }
                    TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                }
            });
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void TenantSecurityOptionsViewModel_TestRemoveCommandExecute()
        {
            var vm = new TenantSecurityOptionsViewModel();
            var groups = this.GetFakeAuthorGroupDCs();
            vm.AdminGroupsEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Admin && a.Enabled));
            vm.AuthorGroupsEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Author && a.Enabled));
            vm.ViewerGroupsEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Viewer && a.Enabled));
            vm.AdminGroupsDisEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Admin && !a.Enabled));
            vm.AuthorGroupsDisEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Author && !a.Enabled));
            vm.ViewerGroupsDisEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Viewer && !a.Enabled));

            //remove from admin groups
            var firstElement = vm.AdminGroupsEnabled.FirstOrDefault();
            if (firstElement != null)
                vm.RemoveSGGroupCommand.Execute(firstElement);
            Assert.IsTrue(vm.AdminGroupsDisEnabled.Contains(firstElement));
            Assert.IsFalse(vm.AdminGroupsEnabled.Contains(firstElement));

            //remove from author group
            firstElement = vm.AuthorGroupsEnabled.FirstOrDefault();
            if (firstElement != null)
                vm.RemoveSGGroupCommand.Execute(firstElement);
            Assert.IsTrue(vm.AuthorGroupsDisEnabled.Contains(firstElement));
            Assert.IsFalse(vm.AuthorGroupsEnabled.Contains(firstElement));

            //remove from viewgroups
            firstElement = vm.ViewerGroupsEnabled.FirstOrDefault();
            if (firstElement != null)
                vm.RemoveSGGroupCommand.Execute(firstElement);
            Assert.IsTrue(vm.ViewerGroupsDisEnabled.Contains(firstElement));
            Assert.IsFalse(vm.ViewerGroupsEnabled.Contains(firstElement));
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        public void TenantSecurityOptionsViewModel_TestAddCommandExecute()
        {
            var vm = new TenantSecurityOptionsViewModel();
            var groups = this.GetFakeAuthorGroupDCs();
            vm.AdminGroupsEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Admin && a.Enabled));
            vm.AuthorGroupsEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Author && a.Enabled));
            vm.ViewerGroupsEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Viewer && a.Enabled));
            vm.AdminGroupsDisEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Admin && !a.Enabled));
            vm.AuthorGroupsDisEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Author && !a.Enabled));
            vm.ViewerGroupsDisEnabled = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Viewer && !a.Enabled));

            Assert.IsFalse(vm.AddSGGroupCommand.CanExecute(""));

            //add from admin groups
            var firstElement = vm.AdminGroupsDisEnabled.FirstOrDefault();
            if (firstElement != null)
            {
                vm.SelectedDisenableAdminGroup = firstElement;
                Assert.IsTrue(vm.AddSGGroupCommand.CanExecute("Admin"));
                vm.AddSGGroupCommand.Execute("Admin");
                Assert.IsFalse(vm.AdminGroupsDisEnabled.Contains(firstElement));
                Assert.IsTrue(vm.AdminGroupsEnabled.Contains(firstElement));
            }

            //add from author group
            firstElement = vm.AuthorGroupsEnabled.FirstOrDefault();
            if (firstElement != null)
            {
                vm.SelectedDisenableAuthorGroup = firstElement;
                Assert.IsTrue(vm.AddSGGroupCommand.CanExecute("Author"));
                vm.AddSGGroupCommand.Execute("Author");
                Assert.IsFalse(vm.AuthorGroupsDisEnabled.Contains(firstElement));
                Assert.IsTrue(vm.AuthorGroupsEnabled.Contains(firstElement));
            }

            //add from viewgroups
            firstElement = vm.ViewerGroupsEnabled.FirstOrDefault();
            if (firstElement != null)
            {
                vm.SelectedDisenableViewerGroup = firstElement;
                Assert.IsTrue(vm.AddSGGroupCommand.CanExecute("Viewer"));
                vm.AddSGGroupCommand.Execute("Viewer");
                Assert.IsFalse(vm.ViewerGroupsDisEnabled.Contains(firstElement));
                Assert.IsTrue(vm.ViewerGroupsEnabled.Contains(firstElement));
            }
        }

        private List<AuthorizationGroupDC> GetFakeAuthorGroupDCs()
        {
            List<AuthorizationGroupDC> authorizationGroups = new List<AuthorizationGroupDC>()
            {
                new AuthorizationGroupDC(){RoleId=(int)Role.Admin,AuthGroupName="admin1",Enabled = true},
                new AuthorizationGroupDC(){RoleId=(int)Role.Admin,AuthGroupName="admin2",Enabled = false},
                new AuthorizationGroupDC(){RoleId=(int)Role.Viewer,AuthGroupName="view1",Enabled = true},
                new AuthorizationGroupDC(){RoleId=(int)Role.Viewer,AuthGroupName="view2", Enabled = false},
                new AuthorizationGroupDC(){RoleId=(int)Role.Author,AuthGroupName="author1",Enabled = true},
                new AuthorizationGroupDC(){RoleId=(int)Role.Author,AuthGroupName="author2",Enabled = false},
                new AuthorizationGroupDC(){RoleId=(int)Role.Author,AuthGroupName="author3",Enabled = true},
                new AuthorizationGroupDC(){RoleId=(int)Role.CWFEnvAdmin,AuthGroupName="envAdmin",Enabled = true},
            };
            return authorizationGroups;
        }

    }
}
