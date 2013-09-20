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
using Microsoft.Support.Workflow.Authoring.Security;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using System.Threading;
using System.DirectoryServices.AccountManagement;
using CWF.DataContracts;
using System.Reflection;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.UnitTest
{
    [TestClass]
    public class ChangeAuthorViewModelUnitTest
    {
        public static void SetAuthorizationServicePermessionList() 
        {
           List<PermissionGetReplyDC> permissionList = new List<PermissionGetReplyDC>();
           System.Reflection.FieldInfo fi = typeof(AuthorizationService).GetField("permissionList", BindingFlags.NonPublic | BindingFlags.Static);
           fi.SetValue(null, permissionList);
        }

        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        [TestMethod]
        public void ChangeAuthorViewModel_TestProjectChanged()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
             {
                 SetAuthorizationServicePermessionList();
                 WorkflowItem wf = TestWorkflowItemGenerator.GetWorkflowItemNoDesigner();
                 var vm = new ChangeAuthorViewModel(wf);
                 Assert.AreEqual(vm.ProjectName, wf.Name);
                 Assert.AreEqual(vm.CreatedBy, "v-test");
                 Assert.AreEqual(vm.Environment, Authoring.AddIns.Data.Env.Dev);

                 TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "AvaliableAuthors", () => vm.AvaliableAuthors = new List<System.DirectoryServices.AccountManagement.Principal>());
                 Assert.IsTrue(vm.AvaliableAuthors.Count == 0);

                 TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "TargetAuthor", () => vm.TargetAuthor = null);
                 Assert.AreEqual(vm.TargetAuthor, null);

                 TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "CreatedBy", () => vm.CreatedBy = "test");
                 Assert.AreEqual(vm.CreatedBy, "test");
                 //}
             });
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        public void ChangeAuthorViewModel_TestChangeAuthorCommandExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                {
                    SetAuthorizationServicePermessionList();
                    WorkflowItem wf = TestWorkflowItemGenerator.GetWorkflowItemNoDesigner();
                    //test ChangeAuthorCommand CanExexute
                    var vm = new ChangeAuthorViewModel(wf);
                    Assert.IsFalse(vm.ChangeAuthorCommand.CanExecute());
                    
                    Principal principal = System.DirectoryServices.AccountManagement.UserPrincipal.Current;
                    vm.TargetAuthor = principal;
                    Assert.IsTrue(vm.ChangeAuthorCommand.CanExecute());

                    //Test ChangeAuthorCommand Execute
                    ChangeAuthorReply reply = new ChangeAuthorReply();
                    reply.StatusReply = new StatusReplyDC();
                    using (var client = new Implementation<WorkflowsQueryServiceClient>())
                    {
                        client.Register(inst => inst.ChangeAuthor(Argument<ChangeAuthorRequest>.Any))
                            .Return(reply);
                        
                        WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                        using (var service = new ImplementationOfType(typeof(DialogService)))
                        {
                            bool isInvoked = false;
                            service.Register(() => DialogService.ShowDialog(Argument<object>.Any)).Execute(() =>
                            {
                                bool? result = true;
                                isInvoked = true;
                                return result;
                            });
                            vm.ChangeAuthorCommand.Execute();

                            Assert.IsTrue(isInvoked);
                            //CreatedBy is changed
                            Assert.AreEqual(wf.CreatedBy, principal.SamAccountName);
                        }
                        //reset client
                        WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                    }

                    //}
                });
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        public void ChangeAuthorViewModel_TestChangeAuthorSummary()
        {
            var vm = new ChangeAuthorSummaryViewModel("TestName", "TestCreated", "TestAuthor");
            Assert.AreEqual(vm.ProjectName, "TestName");
            Assert.AreEqual(vm.OriginalAuthor, "TestCreated");
            Assert.AreEqual(vm.CurrentAuthor, "TestAuthor");
        }
    }
}
