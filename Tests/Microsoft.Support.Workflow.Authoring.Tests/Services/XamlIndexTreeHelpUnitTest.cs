using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Activities.Presentation;
using System.Activities.Presentation.Services;
using System.Activities.Statements;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Microsoft.Support.Workflow.Authoring.Tests.Services
{
    [TestClass]
    public class XamlIndexTreeHelpUnitTest
    {
        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobzh")]
        public void Create()
        {
            Sequence mySequence = new Sequence();
            WriteLine myWriteLine = new WriteLine();
            myWriteLine.Text = "myWrite";
            mySequence.Activities.Add(myWriteLine);

            XamlIndexTreeHelper.CreateIndexTree(mySequence.ToXaml());
            Assert.IsNotNull(XamlIndexTreeHelper.Nodes);
        }

        [TestCategory("Unit-NoDif")]
        [Owner("v-bobzh")]
        [TestMethod]
        public void Refresh()
        {
            Sequence mySequence = new Sequence();
            WriteLine myWriteLine = new WriteLine();
            myWriteLine.Text = "myWrite";
            mySequence.Activities.Add(myWriteLine);

            XamlIndexTreeHelper.Refresh(mySequence.ToXaml());
            Assert.IsNotNull(XamlIndexTreeHelper.Nodes);
        }

        [TestCategory("Unit-NoDif")]
        [Owner("v-bobzh")]
        [TestMethod]
        public void Search()
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            Sequence mySequence = new Sequence();
            WriteLine myWriteLine = new WriteLine();
            myWriteLine.Text = "myWrite";
            mySequence.Activities.Add(myWriteLine);
            XamlIndexTreeHelper.CreateIndexTree(mySequence.ToXaml());

            WorkflowEditorViewModel vm = new WorkflowEditorViewModel(cancellationToken);
            vm.Init("MyWorkflow", mySequence.ToXaml(), false);

            var wf = new ProjectExplorerViewModel(vm);
            wf.SearchText = "Sequence";
            wf.IsSearchWholeWorkflow = true;

            wf.ExecuteSearch();
            Assert.IsNotNull(wf.SelectedWorkflowOutlineNode);

            var index = XamlIndexTreeHelper.Search(wf.SelectedWorkflowOutlineNode);

            Assert.IsNotNull(index);
        }

        [TestCategory("Unit-NoDif")]
        [Owner("v-bobzh")]
        [TestMethod]
        public void SearchAll()
        {
            Sequence mySequence = new Sequence();
            WriteLine myWriteLine = new WriteLine();
            myWriteLine.Text = "myWrite";
            mySequence.Activities.Add(myWriteLine);
            XamlIndexTreeHelper.CreateIndexTree(mySequence.ToXaml());

            WorkflowDesigner workflowDesigner = new WorkflowDesigner();
            workflowDesigner.Load(mySequence);

            var root = workflowDesigner.Context.Services.GetService<ModelService>().Root;
            var WorkflowOutlineNodes = new ObservableCollection<WorkflowOutlineNode>() 
            { 
                new WorkflowOutlineNode(root) 
            };

            RecursiveWorkflowOutlineNode(WorkflowOutlineNodes.First(), (node) =>
                {
                    var index = XamlIndexTreeHelper.Search(node);
                    Assert.IsNotNull(index);
                });            
        }

        private void RecursiveWorkflowOutlineNode(WorkflowOutlineNode parent, Action<WorkflowOutlineNode> action)
        {
            if (parent != null)
            {
                action(parent);
            }
            foreach (var child in parent.Children)
            {
                RecursiveWorkflowOutlineNode(child, action);
            }
        }

    }
}
