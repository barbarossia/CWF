using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Activities.Statements;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using System.Threading;
namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.UnitTest
{
    [TestClass]
    public class ProjectExplorerViewModel_UnitTest
    {
        [WorkItem(348268)]
        [TestCategory("Unit-NoDif")]
        [Owner("v-jillhu")]
        [TestMethod]
        public void aaProjectExplorer_ExecuteSearch()
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            Sequence mySequence = new Sequence();
            WriteLine myWriteLine = new WriteLine();
            myWriteLine.Text = "myWrite";
            mySequence.Activities.Add(myWriteLine);
            //changed
            
            WorkflowEditorViewModel vm = new WorkflowEditorViewModel(cancellationToken);
            vm.Init("MyWorkflow",mySequence.ToXaml(), false);
            var wf = new ProjectExplorerViewModel(vm);
            wf.SearchText = "Sequence";
            wf.IsSearchWholeWorkflow = true;

            wf.ExecuteSearch();
            Assert.IsNotNull(wf.SelectedWorkflowOutlineNode);
            Assert.IsTrue(wf.SelectedWorkflowOutlineNode.NodeName == "Sequence");
            wf.SelectedWorkflowOutlineNode = null;

            wf.SearchText = "Activity";
            wf.ExecuteSearch();
            Assert.IsFalse(wf.SelectedWorkflowOutlineNode.NodeName == "MyWF");

            wf.ExecutePreviousSearch();
            Assert.IsNotNull(wf.SelectedWorkflowOutlineNode);

            wf.ExecuteNextSearch();
            Assert.IsNotNull(wf.SelectedWorkflowOutlineNode);
            //execute the else path
            wf.SearchText = "test";
            wf.ExecuteSearch();
            wf.SearchText = "Activity";
            wf.ExecutePreviousSearch();
            Assert.IsNotNull(wf.SelectedWorkflowOutlineNode);

            wf.ExecuteNextSearch();
            Assert.IsNotNull(wf.SelectedWorkflowOutlineNode);

            wf.SearchText = "WriteLine";
            wf.ExecuteSearch();
            Assert.IsTrue(wf.SelectedWorkflowOutlineNode.NodeName == "WriteLine");

            wf.SearchText = "myWrite";
            wf.ExecuteSearch();
            Assert.IsTrue(wf.SelectedWorkflowOutlineNode.NodeName == "WriteLine");
            wf.SelectedWorkflowOutlineNode = null;

            wf.IsSearchWholeWorkflow = false;
            wf.ExecuteSearch();
            Assert.IsNotNull(wf.SelectedWorkflowOutlineNode);
            //test of workflowItem Properties
            //TestUtilities.Assert_ShouldRaiseINPCNotification(wf,"ViewIsHidden",()=>wf.ViewIsHidden=true);            
        }   
    }
}
