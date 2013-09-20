using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using System.Windows.Data;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Collections;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using Microsoft.Support.Workflow.Authoring.Security;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.AssetStore;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.UnitTest
{
    public class TestWorkflowItemGenerator
    {
        public static WorkflowItem GetWorkflowItemNoDesigner()
        {
            string wfName = "ChangeAuthorTest";
            WorkflowItem wf = new WorkflowItem(wfName, wfName, string.Empty, "MetaData");
            wf.CreatedBy = "v-test";
            wf.Env = Authoring.AddIns.Data.Env.Dev;
            return wf;
        }
    }

    [TestClass]
    public class CopyCurrentProjectViewModelUnitTest
    {
        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void CopyCurrentProjectViewModel_TestPropertyChanged()
        {
            TestUtilities.RegistLoginUserRole(Role.Admin);
            WorkflowItem wf = TestWorkflowItemGenerator.GetWorkflowItemNoDesigner();
            var vm = new CopyCurrentProjectViewModel(wf);

            Assert.IsTrue(vm.AvaliableCopyToEnvs.Count > 0);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "WorkflowTemplates", () => vm.WorkflowTemplates = null);
            Assert.AreEqual(vm.WorkflowTemplates, null);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "CopyTo", () => vm.CopyTo = null);
            Assert.AreEqual(vm.CopyTo, null);
            //TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "NewName", () => vm.NewName = "Test");
            //Assert.AreEqual(vm.NewName, "Test");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "AvaliableCopyToEnvs", () => vm.AvaliableCopyToEnvs = null);
            Assert.AreEqual(vm.AvaliableCopyToEnvs, null);

            Assert.AreEqual(vm.CopyFrom, Authoring.AddIns.Data.Env.Dev);
            Assert.AreEqual(vm.ProjectName, wf.Name);
            Assert.AreEqual(vm.CreatedBy, wf.CreatedBy);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "WorkflowTemplate", () => vm.WorkflowTemplate = null);
            Assert.AreEqual(vm.WorkflowTemplate, null);
        }

        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void CopyCurrentProjectViewModel_TestCopyProjectCommandExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                TestUtilities.RegistLoginUserRole(Role.Admin);
                WorkflowItem wf = TestWorkflowItemGenerator.GetWorkflowItemNoDesigner();
                wf.Env = Authoring.AddIns.Data.Env.Dev;
                var vm = new CopyCurrentProjectViewModel(wf);

                //test CopyWorkflowCommand canexecute
                Assert.IsFalse(vm.CopyProjectCommand.CanExecute());

                using (var client = new Implementation<WorkflowsQueryServiceClient>())
                {
                    WorkflowTypeGetReplyDC reply = new WorkflowTypeGetReplyDC();

                    reply.WorkflowActivityType = new List<WorkflowTypesGetBase>() 
                    {
                       new WorkflowTypesGetBase(){Id = 1},
                    };
                    StoreActivitiesDC replyActivity = new StoreActivitiesDC();
                    client.Register(inst => inst.ActivityCopy(Argument<ActivityCopyRequest>.Any)).Return(replyActivity);

                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                    
                  ImplementationOfType imp = null;
                    TestUtilities.MockAssetStoreProxy(ref imp, () =>
                    {
                        imp.Register(() => AssetStoreProxy.WorkflowTypes)
                            .Return(new ObservableCollection<WorkflowTypesGetBase>(reply.WorkflowActivityType));
                        using (var wfUploader = new ImplementationOfType(typeof(WorkflowUploader)))
                        {
                            wfUploader.Register(() => WorkflowUploader.CheckActivityExist(client.Instance, Argument<StoreActivitiesDC>.Any))
                                .Return(false);

                            vm.CopyTo = vm.AvaliableCopyToEnvs.FirstOrDefault();
                            Assert.IsTrue(vm.WorkflowTemplates.Count == 1);

                            vm.WorkflowTemplate = vm.WorkflowTemplates.First();
                            //vm.NewName = "newname";
                            Assert.IsTrue(vm.CopyProjectCommand.CanExecute());
                            replyActivity.StatusReply = new StatusReplyDC();
                            vm.CopyProjectCommand.Execute();
                            Assert.AreEqual(vm.CopiedActivity, replyActivity);
                        }
                    });
                    //reset client
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                }
            });
        }
    }
}
