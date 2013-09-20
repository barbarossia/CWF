using System;
using System.Collections.Generic;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.DynamicImplementations;
using System.Security.Principal;
using Microsoft.Support.Workflow.Authoring.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Authoring.Tests.Functional
{
    [TestClass]
    public class NewWorkflowFromTemplateViewModelFunctionalTest
    {
        private const string workflowDisplayName = "Workflow1";

        [WorkItem(16030)]
        [Description("Test the New Workflow function")]
        [TestCategory("Smoke")]
        [Owner("DiffRequired")]
        [TestMethod()]
        public void VerifyNewWorkflow()
        {
            try
            {              
                using (var principal = new Implementation<WindowsPrincipal>())
                {
                    principal.Register(p => p.IsInRole(AuthorizationService.AdminAuthorizationGroupName))
                                      .Return(false);
                    principal.Register(p => p.IsInRole(AuthorizationService.AuthorAuthorizationGroupName))
                        .Return(true);
                    Thread.CurrentPrincipal = principal.Instance;
                    Dispatch(() =>
                    {
                        var vmNewWorkflow = Utility.UsingClientReturn(client => new NewWorkflowFromTemplateViewModel(client));
                        // get the list of templates from the database
                        List<WorkflowTemplateItem> lstWorkflowTemplateItem =
                            Utility.UsingClientReturn(client => NewWorkflowFromTemplateViewModel.GetListOfTemplates(client));

                        Assert.AreNotEqual(0, lstWorkflowTemplateItem.Count, TestUtilities.ListOfTemplateErrorMessage);
                        // initialize the workflow item
                        vmNewWorkflow.SelectedWorkflowTemplateItem = lstWorkflowTemplateItem[0];
                        vmNewWorkflow.WorkflowName = workflowDisplayName;
                        vmNewWorkflow.WorkflowClassName = workflowDisplayName;
                        // create this workflow item
                        vmNewWorkflow.CreateWorkflowItem.Execute();
                    });

                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Create new workflow failed with this exception: {0}", ex.Message);
            }
        }


        protected void Dispatch(Action action)
        {
            //Getting called from main thread, execute on backgroud Dispatcher
            if (Dispatcher.CurrentDispatcher != null)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, action);
            }
            //Getting called from unit test, execute on main thread
            else
            {
                action.Invoke();
            }
        }

    }
}
