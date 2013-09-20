using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using CWF.DataContracts;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using Microsoft.Support.Workflow.Authoring.Security;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Principal;

namespace Microsoft.Support.Workflow.Authoring.Tests.Services {
    [TestClass]
    public class AuthorizationServiceUnitTest {
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void TestEnvPermissionMaps() {
            CleanupMaps();
            using (var client = new Implementation<WorkflowsQueryServiceClient>()) {
                client.Register(inst => inst.PermissionGetList(Argument<RequestHeader>.Any)).Execute(() => {
                    return new PermissionGetListReply() { List = GetPermissionList() };
                });
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                var maps = AuthorizationService.EnvPermissionMaps;
                Thread.Sleep(60000); // delay to make task finish
                CollectionAssert.AreEquivalent(GetEnvPermissionMaps(), maps);
            }
        }

        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void TestGetAuthorizedPrincipals() {
            List<PermissionGetReplyDC> permissionList = GetPermissionList();
            FieldInfo fi = typeof(AuthorizationService).GetField("permissionList", BindingFlags.NonPublic | BindingFlags.Static);
            fi.SetValue(null, permissionList);

            List<Principal> expected = AuthorizationService.ListGroupsUsers("cwf_eng_wsp").OrderBy(p => p.DisplayName).ToList();
            CollectionAssert.AreEqual(expected, AuthorizationService.GetAuthorizedPrincipals(Permission.SaveWorkflow, Env.Dev));
            CollectionAssert.AreEqual(expected, AuthorizationService.GetAuthorizedPrincipals(Permission.SaveWorkflow, Env.Test));
            expected = new List<Principal>();
            CollectionAssert.AreEqual(expected, AuthorizationService.GetAuthorizedPrincipals(Permission.SaveWorkflow, Env.Stage));
            CollectionAssert.AreEqual(expected, AuthorizationService.GetAuthorizedPrincipals(Permission.SaveWorkflow, Env.Prod));
        }

        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void TestValidate() {
            RegistSimpleMaps();

            TestUtilities.Assert_ShouldThrow<ArgumentException>(() => {
                AuthorizationService.Validate((Env)0, Permission.SaveWorkflow);
            });
            Assert.IsTrue(AuthorizationService.Validate(Env.Any, Permission.OpenWorkflow));
            Assert.IsTrue(AuthorizationService.Validate(Env.All, Permission.OpenWorkflow));
            Assert.IsTrue(AuthorizationService.Validate(Env.Dev, Permission.OpenWorkflow));
            Assert.IsTrue(AuthorizationService.Validate(Env.Test, Permission.OpenWorkflow));
            Assert.IsTrue(AuthorizationService.Validate(Env.Stage, Permission.OpenWorkflow));
            Assert.IsTrue(AuthorizationService.Validate(Env.Prod, Permission.OpenWorkflow));

            Assert.IsTrue(AuthorizationService.Validate(Env.Any, Permission.SaveWorkflow));
            Assert.IsFalse(AuthorizationService.Validate(Env.All, Permission.SaveWorkflow));
            Assert.IsTrue(AuthorizationService.Validate(Env.Dev, Permission.SaveWorkflow));
            Assert.IsTrue(AuthorizationService.Validate(Env.Test, Permission.SaveWorkflow));
            Assert.IsFalse(AuthorizationService.Validate(Env.Stage, Permission.SaveWorkflow));
            Assert.IsFalse(AuthorizationService.Validate(Env.Prod, Permission.SaveWorkflow));

            RegistEmptyMaps();

            Assert.IsFalse(AuthorizationService.Validate(Env.Any, Permission.OpenWorkflow));
            Assert.IsFalse(AuthorizationService.Validate(Env.All, Permission.OpenWorkflow));
        }

        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void TestToEnv() {
            Assert.AreEqual(Env.Dev, "Dev".ToEnv());
            Assert.AreEqual(Env.Test, "TEST".ToEnv());
            Assert.AreEqual(Env.Stage, "stage".ToEnv());
            Assert.AreEqual(Env.Prod, "PrOd".ToEnv());
        }

        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void TestGetAuthorizedEnvs() {
            RegistSimpleMaps();

            foreach (Permission permission in Enum.GetValues(typeof(Permission))) {
                Env[] envs;
                switch (permission) {
                    case Permission.None:
                        envs = new Env[] { Env.Dev, Env.Test, Env.Stage, Env.Prod };
                        break;
                    case Permission.OpenWorkflow:
                        envs = new Env[] { Env.Dev, Env.Test, Env.Stage, Env.Prod };
                        break;
                    case Permission.SaveWorkflow:
                        envs = new Env[] { Env.Dev, Env.Test };
                        break;
                    default:
                        envs = new Env[0];
                        break;
                }
                CollectionAssert.AreEquivalent(envs, AuthorizationService.GetAuthorizedEnvs(permission));
            }
        }

        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void TestGroupExists() {
            Assert.IsFalse(AuthorizationService.GroupExists("GroupNotExist"));
            Assert.IsTrue(AuthorizationService.GroupExists("cwf_eng_wsp"));
        }

        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void TestListGroupsUsers() {
            RegistEmptyMaps();

            var list = AuthorizationService.ListGroupsUsers("pqocwfauthors");
            Assert.IsTrue(list.Any());

            var list2 = AuthorizationService.ListGroupsUsers("pqocwfauthors");
            Assert.AreEqual(list, list2);
        }

        private void CleanupMaps() {
            SetEnvPermissionMaps(null);
        }

        private void RegistEmptyMaps() {
            SetEnvPermissionMaps(new Dictionary<Env, Permission>());
        }

        private void RegistSimpleMaps() {
            SetEnvPermissionMaps(GetEnvPermissionMaps());
        }

        private Dictionary<Env, Permission> GetEnvPermissionMaps() {
            return GetPermissionList().ToDictionary(dc => dc.EnvironmentName.ToEnv(), dc => (Permission)dc.Permission);
        }

        private void SetEnvPermissionMaps(Dictionary<Env, Permission> envPermissionMaps) {
            FieldInfo fi = typeof(AuthorizationService).GetField("envPermissionMaps", BindingFlags.NonPublic | BindingFlags.Static);
            fi.SetValue(null, envPermissionMaps);
        }

        private List<PermissionGetReplyDC> GetPermissionList() {
            return new List<PermissionGetReplyDC>() {
                new PermissionGetReplyDC() { 
                    AuthorGroupName = "cwf_eng_wsp", 
                    EnvironmentName = Env.Dev.ToString(), 
                    Permission = (long)(Permission.OpenWorkflow | Permission.SaveWorkflow)
                },
                new PermissionGetReplyDC() { 
                    AuthorGroupName = "cwf_eng_wsp", 
                    EnvironmentName = Env.Test.ToString(), 
                    Permission = (long)(Permission.OpenWorkflow | Permission.SaveWorkflow)
                },
                new PermissionGetReplyDC() { 
                    AuthorGroupName = "cwf_eng_wsp", 
                    EnvironmentName = Env.Stage.ToString(), 
                    Permission = (long)(Permission.OpenWorkflow)
                },
                new PermissionGetReplyDC() { 
                    AuthorGroupName = "cwf_eng_wsp", 
                    EnvironmentName = Env.Prod.ToString(), 
                    Permission = (long)(Permission.OpenWorkflow)
                },
            };
        }
    }
}
