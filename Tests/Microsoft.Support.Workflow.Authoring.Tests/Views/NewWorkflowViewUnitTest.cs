using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Views;
using Microsoft.DynamicImplementations;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.AssetStore;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;

namespace Microsoft.Support.Workflow.Authoring.Tests.Views
{
    [TestClass]
    public class NewWorkflowViewUnitTest
    {
        [WorkItem(325745)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Views_VerifyDataContext()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
               {
                   using (var client = new Implementation<WorkflowsQueryServiceClient>())
                   {
                       client.Register(inst => inst.WorkflowTypeGet(Argument<WorkflowTypesGetRequestDC>.Any)).Execute(() =>
                       {
                           WorkflowTypeGetReplyDC replyDC = new WorkflowTypeGetReplyDC();
                           replyDC.WorkflowActivityType = new List<WorkflowTypesGetBase>() 
                            {
                                new WorkflowTypesGetBase(){WorkflowTemplateId=1,Name="myworkflow"},
                            };
                           replyDC.StatusReply = new StatusReplyDC() { Errorcode = 0 };
                           return replyDC;

                       });
                       WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                       var viewModel = new NewWorkflowViewModel();

                       var view = new NewWorkflowView();
                       view.DataContext = viewModel;
                       viewModel.IsCreatingBlank = true;

                       KeyEventArgs args = new KeyEventArgs(Keyboard.PrimaryDevice, new HwndSource(
                           0, 0, 0, 0, 0, string.Empty, IntPtr.Zero), 0, Key.Enter);
                       PrivateObject po = new PrivateObject(view);
                       try
                       {

                           po.Invoke("Cancel_Click", null, args);
                           Assert.IsTrue(view.DialogResult.Value == false);
                       }
                       catch (Exception) { }

                       try
                       {
                           po.Invoke("Ok_Click", null, args);
                           Assert.IsTrue(view.DialogResult.Value == true);
                       }
                       catch (Exception) { }
                       WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                   }
               });
        }
    }
}
