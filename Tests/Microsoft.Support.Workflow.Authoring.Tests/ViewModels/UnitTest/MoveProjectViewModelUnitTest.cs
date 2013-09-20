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
using System.Collections.ObjectModel;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.AssetStore;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.UnitTest
{
    [TestClass]
    public class MoveProjectViewModelUnitTest
    {
        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void MoveProjectViewModel_TestPropertyChanged()
        {
            WorkflowItem wf = TestWorkflowItemGenerator.GetWorkflowItemNoDesigner();
            var vm = new MoveProjectViewModel(wf);
            wf.Env = Authoring.AddIns.Data.Env.Dev;

            //Assert.IsTrue(vm.AvaliableNextStatus.Count > 0);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "WorkflowTemplates", () => vm.WorkflowTemplates = null);
            Assert.AreEqual(vm.WorkflowTemplates, null);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "AvaliableNextStatus", () => vm.AvaliableNextStatus = null);
            Assert.AreEqual(vm.AvaliableNextStatus, null);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "NextStatus", () => vm.NextStatus = null);
            Assert.AreEqual(vm.NextStatus, null);

            Assert.AreEqual(vm.CurrentStatus, Authoring.AddIns.Data.Env.Dev);
            Assert.AreEqual(vm.ProjectName, wf.Name);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "WorkflowTemplate", () => vm.WorkflowTemplate = null);
            Assert.AreEqual(vm.WorkflowTemplate, null);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void MoveProjectViewModel_TestStatus()
        {
            WorkflowItem wf = TestWorkflowItemGenerator.GetWorkflowItemNoDesigner();
            var vm = new MoveProjectViewModel(wf);
            PrivateObject pv = new PrivateObject(vm);

            wf.Env = Authoring.AddIns.Data.Env.Dev;
            pv.Invoke("InitializeNextStatus");
            CollectionAssert.AreEquivalent(vm.AvaliableNextStatus, new ObservableCollection<Env>() { Env.Test });

            wf.Env = Env.Test;
            pv.Invoke("InitializeNextStatus");
            CollectionAssert.AreEquivalent(vm.AvaliableNextStatus, new ObservableCollection<Env>() { Env.Dev});

            //wf.Env = Env.Stage;
            //pv.Invoke("InitializeNextStatus");
            //CollectionAssert.AreEquivalent(vm.AvaliableNextStatus, new ObservableCollection<Env>() { Env.Test, Env.Prod });

            //wf.Env = Env.Prod;
            //pv.Invoke("InitializeNextStatus");
            //CollectionAssert.AreEquivalent(vm.AvaliableNextStatus, new ObservableCollection<Env>() { Env.Stage });
        }

        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MoveProjectViewModel_TestMoveProjectCommandExecute()
        {
            WorkflowItem wf = TestWorkflowItemGenerator.GetWorkflowItemNoDesigner();
            wf.Env = Authoring.AddIns.Data.Env.Dev;
            var vm = new MoveProjectViewModel(wf);
            Assert.IsFalse(vm.MoveProjectCommand.CanExecute());

            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var client = new Implementation<WorkflowsQueryServiceClient>())
                {
                    WorkflowTypeGetReplyDC reply = new WorkflowTypeGetReplyDC();

                    reply.WorkflowActivityType = new List<WorkflowTypesGetBase>() 
                    {
                       new WorkflowTypesGetBase(){Id = 1},
                    };

                    ActivityMoveReply replyActivity = new ActivityMoveReply();
                    client.Register(inst => inst.ActivityMove(Argument<ActivityMoveRequest>.Any)).Return(replyActivity);

                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                    ImplementationOfType imp = null;
                    TestUtilities.MockAssetStoreProxy(ref imp, () =>
                    {
                        imp.Register(() => AssetStoreProxy.WorkflowTypes).Return(new ObservableCollection<WorkflowTypesGetBase>(reply.WorkflowActivityType));

                        using (var wfUploader = new ImplementationOfType(typeof(WorkflowUploader)))
                        {
                            wfUploader.Register(() => WorkflowUploader.CheckActivityExist(client.Instance, Argument<StoreActivitiesDC>.Any))
                                .Return(false);

                            vm.NextStatus = vm.AvaliableNextStatus.FirstOrDefault();
                            
                            Assert.IsTrue(vm.WorkflowTemplates.Count == 1);
                            vm.WorkflowTemplate = vm.WorkflowTemplates.FirstOrDefault();
                            Assert.IsTrue(vm.MoveProjectCommand.CanExecute());

                            replyActivity.StatusReply = new StatusReplyDC();
                            vm.MoveProjectCommand.Execute();
                            Assert.AreEqual(wf.Env, vm.NextStatus);
                        }
                        //reset client
                        WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                    });
                }
            });
        }
    }
}
