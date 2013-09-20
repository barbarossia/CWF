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
using Microsoft.Support.Workflow.Authoring.Security;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.UnitTest
{
    [TestClass]
    public class EnvSecurityOptionsViewModelUnitTest
    {
        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void EnvironmentSecurityOptionsViewModel_TestPropertyChanged()
        {
            ImplementationOfType imp = null;
            TestUtilities.MockAssetStoreProxy(ref imp, () =>
            {
                var vm = new EnvironmentSecurityOptionsViewModel();
                Assert.IsFalse(vm.CanSave);

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "HasChanged", () => vm.HasChanged = true);
                Assert.IsTrue(vm.HasChanged);

                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "TestButtonTitle", () => vm.TestButtonTitle = "test");
                Assert.AreEqual(vm.TestButtonTitle, "test");
                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "IsTesting", () => vm.IsTesting = true);
                Assert.IsTrue(vm.IsTesting);
                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SGType", () => vm.SGType = new List<string>());
                Assert.IsNotNull(vm.SGType);
                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedSGType", () => vm.SelectedSGType = "Admin");
                Assert.AreEqual(vm.SelectedSGType, "Admin");
                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "ViewerGroups", () => vm.ViewerGroups = null);
                Assert.IsNull(vm.ViewerGroups);
                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "AuthorGroups", () => vm.AuthorGroups = null);
                Assert.IsNull(vm.AuthorGroups);
                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "AdminGroups", () => vm.AdminGroups = null);
                Assert.IsNull(vm.AdminGroups);
                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "TenantEndpoint", () => vm.TenantEndpoint = "url");
                Assert.AreEqual(vm.TenantEndpoint, "url");
                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "TenantAdminGroup", () => vm.TenantAdminGroup = "admin");
                Assert.AreEqual(vm.TenantAdminGroup, "admin");
                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SGName", () => vm.SGName = "admin");
                Assert.AreEqual(vm.SGName, "admin");
            });
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        public void EnvironmentSecurityOptionsViewModel_TestLoadData()
        {
            ImplementationOfType imp = null;
            TestUtilities.MockAssetStoreProxy(ref imp, () =>
            {
                var vm = new EnvironmentSecurityOptionsViewModel();
                TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                {
                    using (var client = new Implementation<WorkflowsQueryServiceClient>())
                    {
                        AuthorizationGroupGetReplyDC reply = new AuthorizationGroupGetReplyDC();
                        reply.StatusReply = new StatusReplyDC();
                        reply.AuthorizationGroups = GetFakeAuthorGroupDCs();
                        client.Register(inst => inst.GetAuthorizationGroups(Argument<AuthorizationGroupGetRequestDC>.Any))
                            .Return(reply);
                        WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                        vm.LoadLiveData();
                        Assert.IsTrue(vm.AdminGroups.Count > 0);
                        Assert.IsTrue(vm.AuthorGroups.Count > 0);
                        Assert.IsTrue(vm.ViewerGroups.Count > 0);
                        Assert.IsNotNull(vm.TenantAdminGroup);
                        TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                    }
                });
            });
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        public void EnvironmentSecurityOptionsViewModel_TestSaveCommandExecute()
        {
            ImplementationOfType imp = null;
            TestUtilities.MockAssetStoreProxy(ref imp, () =>
            {
                var vm = new EnvironmentSecurityOptionsViewModel();
                TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                {
                    //test savecommand can execute
                    vm.IsTesting = true;
                    Assert.IsFalse(vm.CanSave);

                    vm.IsTesting = false;
                    vm.TenantAdminGroup = "test";
                    Assert.IsTrue(vm.CanSave);

                    //test tenantadmingroup exist
                    using (var impAuthorizationService = new ImplementationOfType(typeof(AuthorizationService)))
                    {
                        using (var impDialog = new ImplementationOfType(typeof(MessageBoxService)))
                        {
                            bool isShowError = false;
                            bool isShowInfo = false;

                            impDialog.Register(() => MessageBoxService.ShowInfo(Argument<string>.Any)).Execute(() => { isShowInfo = true; return MessageBoxResult.OK; });
                            impDialog.Register(() => MessageBoxService.ShowError(Argument<string>.Any)).Execute(() => { isShowError = true; return MessageBoxResult.OK; });

                            //the tanant admin group is same as one author group
                            vm.AuthorGroups = new System.Collections.ObjectModel.ObservableCollection<AuthorizationGroupDC>() 
                            {
                                new AuthorizationGroupDC(){AuthGroupName="test",RoleId = (int)Role.Author},
                            };
                            vm.AdminGroups = new ObservableCollection<AuthorizationGroupDC>();
                            vm.ViewerGroups = new ObservableCollection<AuthorizationGroupDC>();

                            vm.SaveCommand.Execute();
                            Assert.IsTrue(isShowInfo);


                            //the admintenantgroup not exist
                            isShowError = false;
                            impAuthorizationService.Register(() => AuthorizationService.GroupExists(Argument<string>.Any))
                            .Return(false);
                            vm.AuthorGroups = new System.Collections.ObjectModel.ObservableCollection<AuthorizationGroupDC>();
                            vm.SaveCommand.Execute();
                            Assert.IsTrue(isShowError);

                            impAuthorizationService.Register(() => AuthorizationService.GroupExists(Argument<string>.Any))
                            .Return(true);
                            //savecommand execute
                            using (var client = new Implementation<WorkflowsQueryServiceClient>())
                            {
                                AuthGroupsCreateOrUpdateReplyDC reply = null;
                                reply = new AuthGroupsCreateOrUpdateReplyDC();
                                reply.StatusReply = new StatusReplyDC();
                                client.Register(inst => inst.AuthorizationGroupCreateOrUpdate(Argument<AuthGroupsCreateOrUpdateRequestDC>.Any))
                                    .Return(reply);

                                AuthGroupsEnableOrDisableReplyDC replyEnable = new AuthGroupsEnableOrDisableReplyDC();
                                replyEnable.StatusReply = new StatusReplyDC();
                                client.Register(inst => inst.AuthorizationGroupEnableOrDisable(Argument<AuthGroupsEnableOrDisableRequestDC>.Any))
                                    .Return(replyEnable);

                                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                                isShowInfo = false;
                                isShowError = false;
                                //save successfully
                                vm.SaveCommand.Execute();
                                Assert.IsTrue(isShowInfo);
                                Assert.IsTrue(vm.HasSaved);
                                Assert.IsFalse(vm.HasChanged);
                                TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                            }
                        }
                    }
                });
            });
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        public void EnvironmentSecurityOptionsViewModel_TestRemoveCommandExecute()
        {
            ImplementationOfType imp = null;
            TestUtilities.MockAssetStoreProxy(ref imp, () =>
            {
                var vm = new EnvironmentSecurityOptionsViewModel();
                var groups = this.GetFakeAuthorGroupDCs();
                vm.AuthorGroups = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Author));
                vm.AdminGroups = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Admin));
                vm.ViewerGroups = new ObservableCollection<AuthorizationGroupDC>(groups.Where(a => a.RoleId == (int)Role.Viewer));
                var firstElement = vm.AuthorGroups.FirstOrDefault();
                if (firstElement != null)
                    vm.RemoveSGGroupCommand.Execute(firstElement);
                Assert.IsFalse(vm.AuthorGroups.Contains(firstElement));

                firstElement = vm.AdminGroups.FirstOrDefault();
                if (firstElement != null)
                    vm.RemoveSGGroupCommand.Execute(firstElement);
                Assert.IsFalse(vm.AdminGroups.Contains(firstElement));

                firstElement = vm.ViewerGroups.FirstOrDefault();
                if (firstElement != null)
                    vm.RemoveSGGroupCommand.Execute(firstElement);
                Assert.IsFalse(vm.ViewerGroups.Contains(firstElement));

            });
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        public void EnvironmentSecurityOptionsViewModel_TestAddCommandExecute()
        {
            ImplementationOfType imp = null;
            TestUtilities.MockAssetStoreProxy(ref imp, () =>
            {
                using (var impAuthorizationService = new ImplementationOfType(typeof(AuthorizationService)))
                {
                    using (var impDialog = new ImplementationOfType(typeof(MessageBoxService)))
                    {
                        bool isShowError = false;
                        bool isShowInfo = false;

                        impDialog.Register(() => MessageBoxService.ShowInfo(Argument<string>.Any)).Execute(() =>
                        {
                            isShowInfo = true;
                            return MessageBoxResult.OK;
                        });
                        impDialog.Register(() => MessageBoxService.ShowError(Argument<string>.Any)).Execute(() =>
                        {
                            isShowError = true;
                            return MessageBoxResult.OK;
                        });
                        impAuthorizationService.Register(() => AuthorizationService.GroupExists(Argument<string>.Any))
                                .Return(false);
                        var vm = new EnvironmentSecurityOptionsViewModel();
                        //initial authgroups
                        vm.ViewerGroups = new ObservableCollection<AuthorizationGroupDC>();
                        vm.AuthorGroups = new ObservableCollection<AuthorizationGroupDC>();
                        vm.AdminGroups = new ObservableCollection<AuthorizationGroupDC>();

                        //Test addcommand can execute
                        vm.IsTesting = true;
                        Assert.IsFalse(vm.AddSGGroupCommand.CanExecute());
                        vm.IsTesting = false;
                        vm.SGName = "admingroup";
                        vm.SelectedSGType = "Admin";
                        Assert.IsTrue(vm.AddSGGroupCommand.CanExecute());
                        
                        vm.AddSGGroupCommand.Execute();
                        Assert.IsTrue(isShowError);//group not exist.

                        impAuthorizationService.Register(() => AuthorizationService.GroupExists(Argument<string>.Any))
                               .Return(true);
                        vm.ViewerGroups = new ObservableCollection<AuthorizationGroupDC>() { 
                            new AuthorizationGroupDC()
                            {
                                AuthGroupName = "admingroup",
                                RoleId = (int)Role.Viewer
                            }};
                        vm.AddSGGroupCommand.Execute();
                        Assert.IsTrue(isShowInfo);//duplicate name with view groups

                        vm.ViewerGroups.Clear();
                        vm.AdminGroups.Clear();
                        vm.AddSGGroupCommand.Execute();
                        Assert.IsTrue(vm.AdminGroups.Count > 0);

                        vm.AdminGroups.Clear();
                        vm.SelectedSGType = "Viewer";
                        vm.AddSGGroupCommand.Execute();
                        Assert.IsTrue(vm.ViewerGroups.Count > 0);

                        vm.ViewerGroups.Clear();
                        vm.SelectedSGType = "Author";
                        vm.AddSGGroupCommand.Execute();
                        Assert.IsTrue(vm.AuthorGroups.Count > 0);
                    }
                }
            });
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        public void EnvironmentSecurityOptionsViewModel_TestVerifyGroupMethod()
        {
            ImplementationOfType imp = null;
            TestUtilities.MockAssetStoreProxy(ref imp, () =>
            {
                using (var impAuthorizationService = new ImplementationOfType(typeof(AuthorizationService)))
                {
                    using (var impDialog = new ImplementationOfType(typeof(MessageBoxService)))
                    {
                        bool isShowError = false;
                        bool isShowInfo = false;
                        impDialog.Register(() => MessageBoxService.ShowInfo(Argument<string>.Any)).Execute(() => { isShowInfo = true; return MessageBoxResult.OK; });
                        impDialog.Register(() => MessageBoxService.ShowError(Argument<string>.Any)).Execute(() => { isShowError = true; return MessageBoxResult.OK; });

                        var vm = new EnvironmentSecurityOptionsViewModel();
                        vm.ViewerGroups = new ObservableCollection<AuthorizationGroupDC>();
                        vm.AuthorGroups = new ObservableCollection<AuthorizationGroupDC>();
                        vm.AdminGroups = new ObservableCollection<AuthorizationGroupDC>();
                        Assert.IsFalse(vm.TestCommand.CanExecute());
                        vm.TenantAdminGroup = "test";
                        Assert.IsTrue(vm.TestCommand.CanExecute());
                        
                        //group exists
                        impAuthorizationService.Register(() => AuthorizationService.GroupExists(Argument<string>.Any))
                              .Return(true);
                        vm.TestCommand.Execute();
                        while (vm.IsTesting) { Thread.Sleep(500); }
                        Assert.IsTrue(isShowInfo);
                        

                        //group not exists
                        impAuthorizationService.Register(() => AuthorizationService.GroupExists(Argument<string>.Any))
                              .Return(false);

                        vm.TestCommand.Execute();
                        while (vm.IsTesting) { Thread.Sleep(500); } 
                        Assert.IsTrue(isShowError);
                    }
                }
            });
        }

        private List<AuthorizationGroupDC> GetFakeAuthorGroupDCs()
        {
            List<AuthorizationGroupDC> authorizationGroups = new List<AuthorizationGroupDC>()
            {
                new AuthorizationGroupDC(){RoleId=(int)Role.Admin,AuthGroupName="admin1"},
                new AuthorizationGroupDC(){RoleId=(int)Role.Admin,AuthGroupName="admin2"},
                new AuthorizationGroupDC(){RoleId=(int)Role.Viewer,AuthGroupName="view1"},
                new AuthorizationGroupDC(){RoleId=(int)Role.Viewer,AuthGroupName="view2"},
                new AuthorizationGroupDC(){RoleId=(int)Role.Author,AuthGroupName="author1"},
                new AuthorizationGroupDC(){RoleId=(int)Role.Author,AuthGroupName="author2"},
                new AuthorizationGroupDC(){RoleId=(int)Role.Author,AuthGroupName="author3"},
                new AuthorizationGroupDC(){RoleId=(int)Role.CWFEnvAdmin,AuthGroupName="envAdmin"},
            };
            return authorizationGroups;
        }
    }
}
