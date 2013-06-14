using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using System.Activities.Presentation;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using System.Collections.ObjectModel;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.View;
using System.Activities;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

namespace Microsoft.Support.Workflow.Authoring.AddIns.ViewModels
{
    public class ProjectExplorerViewModel : NotificationObject
    {


        private const string FindEnd = "Search reached the end point.";
        private const string FindStart = "Search reached the starting point.";
        private const string NoResult = "No items match your search.";

        private const string DisplayNamePropertyName = "DisplayName";
        private const string InArgumentTypeName = "InArgument";
        private const string OutArgumentTypeName = "OutArgument";
        private const string InArgumentGenericTypeName = "InArgument`1";
        private const string OutArgumentGenericTypeName = "OutArgument`1";

        [NonSerialized]
        private List<WorkflowOutlineNode> searchResult = new List<WorkflowOutlineNode>();

        [NonSerialized]
        private ObservableCollection<WorkflowOutlineNode> workflowOutlineNode;

        [NonSerialized]
        private bool isInitialized = false;

        [NonSerialized]
        private bool isSearchWholeWorkflow;

        [NonSerialized]
        private bool isSearchCurrentActivity;

        [NonSerialized]
        private bool isSearchTitle;

        [NonSerialized]
        private string searchText;

        [NonSerialized]
        private bool isSearchType;

        [NonSerialized]
        private bool isSearchParameter;

        [NonSerialized]
        private WorkflowOutlineNode selectedWorkflowOutlineNode;

        [NonSerialized]
        private string searchPositionNotify;

        [NonSerialized]
        private WorkflowEditorViewModel workflowEditor;

        public ProjectExplorerViewModel(WorkflowEditorViewModel paramEditor)
        {
            if (paramEditor != null)
            {
                this.WorkflowEditor = paramEditor;
                this.WorkflowEditor.PropertyChanged += this.WorkflowDesignerChanged;
                this.Designer.Context.Items.Subscribe<Selection>(ActivityFocusedChanged);
            }
            else
                throw new Exception("Argument WorkflowEditor is null.");
            this.RefreshProjectExplorer();
        }


        public WorkflowDesigner Designer
        {
            get { return this.workflowEditor.WorkflowDesigner; }
        }

        public WorkflowEditorViewModel WorkflowEditor
        {
            get { return this.workflowEditor; }
            set
            {
                this.workflowEditor = value;
                RaisePropertyChanged(() => this.WorkflowEditor);
                if (WorkflowEditor.WorkflowDesigner != null)
                    WorkflowEditor.WorkflowDesigner.ModelChanged += this.Designer_ModelChanged;
            }
        }

        /// <summary>
        /// The project explorer tree nodes generated according the workflow designer
        /// </summary>
        public ObservableCollection<WorkflowOutlineNode> WorkflowOutlineNodes
        {
            get { return this.workflowOutlineNode; }
            set
            {
                this.workflowOutlineNode = value;
                RaisePropertyChanged(() => this.WorkflowOutlineNodes);
            }
        }


        /// <summary>
        /// Gets or sets the indicator if search activities in the whole workflow
        /// </summary>
        public bool IsSearchWholeWorkflow
        {
            get { return this.isSearchWholeWorkflow; }
            set
            {
                this.isSearchWholeWorkflow = value;
                RaisePropertyChanged(() => this.IsSearchWholeWorkflow);
                if (searchResult != null)
                    searchResult.Clear();
            }
        }

        /// <summary>
        /// Gets or sets the indicator if seach activities in the current activity
        /// </summary>
        public bool IsSearchCurrentActivity
        {
            get { return this.isSearchCurrentActivity; }
            set
            {
                this.isSearchCurrentActivity = value;
                RaisePropertyChanged(() => this.IsSearchCurrentActivity);
                if (searchResult != null)
                    searchResult.Clear();
            }
        }


        /// <summary>
        /// Gets or sets the indicator if search the Activity title(DisplayName)
        /// </summary>
        public bool IsSearchTitle
        {
            get { return this.isSearchTitle; }
            set
            {
                this.isSearchTitle = value;
                RaisePropertyChanged(() => this.IsSearchTitle);
                if (searchResult != null)
                    searchResult.Clear();
            }
        }


        /// <summary>
        ///  Gets or sets the indicator if search the Activity Type
        /// </summary>
        public bool IsSearchType
        {
            get { return this.isSearchType; }
            set
            {
                this.isSearchType = value;
                RaisePropertyChanged(() => this.IsSearchType);
                if (searchResult != null)
                    searchResult.Clear();
            }
        }


        /// <summary>
        ///  Gets or sets the indicator if search the Activity parameters
        /// </summary>
        public bool IsSearchParameter
        {
            get { return this.isSearchParameter; }
            set
            {
                this.isSearchParameter = value;
                RaisePropertyChanged(() => this.IsSearchParameter);
                if (searchResult != null)
                    searchResult.Clear();
            }
        }


        /// <summary>
        /// Gets or sets the search text that the user input
        /// </summary>
        public string SearchText
        {
            get { return this.searchText; }
            set
            {
                this.searchText = value;
                this.SearchPositionNotify = string.Empty;
                RaisePropertyChanged(() => this.SearchText);
            }
        }


        /// <summary>
        /// Gets or sets the current searching position if the seach reached the starting point or the end point
        /// </summary>
        public string SearchPositionNotify
        {
            get { return this.searchPositionNotify; }
            set
            {
                this.searchPositionNotify = value;
                RaisePropertyChanged(() => this.SearchPositionNotify);
            }
        }


        /// <summary>
        /// Gets or sets the selected WorkflowOutlineNode in Project Explorer tree
        /// </summary>
        public WorkflowOutlineNode SelectedWorkflowOutlineNode
        {
            get { return this.selectedWorkflowOutlineNode; }
            set
            {
                this.selectedWorkflowOutlineNode = value;
                RaisePropertyChanged(() => this.SelectedWorkflowOutlineNode);
                if (this.selectedWorkflowOutlineNode != null)
                {
                    this.WorkflowEditor.WorkflowDesigner.Context.Items.SetValue(new Selection(this.SelectedWorkflowOutlineNode.Model));
                    this.SelectedWorkflowOutlineNode.Model.Focus();
                    this.SearchPositionNotify = string.Empty;
                }
            }
        }

        #region public method
        /// <summary>
        /// Execute the search action
        /// </summary>
        public void ExecuteSearch()
        {
            this.Search();
            if (this.searchResult != null && this.searchResult.Any())
            {
                this.SelectedWorkflowOutlineNode = searchResult.First();
            }
        }

        /// <summary>
        /// Execute the previous search action
        /// </summary>
        public void ExecuteNextSearch()
        {
            if (this.searchResult != null && this.searchResult.Any())
            {
                WorkflowOutlineNode next = Next(this.searchResult, i => i == this.SelectedWorkflowOutlineNode);
                if (next != null)
                {
                    this.SelectedWorkflowOutlineNode = next;
                }
                else
                {
                    this.SearchPositionNotify = FindEnd;
                }
            }
            else
            {
                this.ExecuteSearch();
            }
        }

        /// <summary>
        /// Execute the next search action
        /// </summary>
        public void ExecutePreviousSearch()
        {
            if (this.searchResult != null && this.searchResult.Any())
            {
                var pre = this.Previous(this.searchResult, i => i == this.SelectedWorkflowOutlineNode);
                if (pre != null)
                {
                    this.SelectedWorkflowOutlineNode = pre;
                }
                else
                {
                    this.SearchPositionNotify = FindStart;
                }
            }
            else
            {
                this.ExecuteSearch();
            }
        }


        /// <summary>
        /// Regenerates  the project explorer tree when the workflow designer is changed .
        /// </summary>
        public void RefreshProjectExplorer()
        {
            if (this.Designer == null || this.Designer.Context.Services.GetService<ModelService>() == null)
                return;
            var root = this.Designer.Context.Services.GetService<ModelService>().Root;
            this.WorkflowOutlineNodes = new ObservableCollection<WorkflowOutlineNode>() 
            { 
                new WorkflowOutlineNode(root) 
            };

            if (!isInitialized)
            {
                this.IsSearchCurrentActivity = true;
                this.IsSearchTitle = true;
                this.IsSearchType = true;
                this.IsSearchParameter = true;
                this.isInitialized = true;
            }
        }

        #endregion

        private void WorkflowDesignerChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "WorkflowDesigner")
            {
                //register selection changed
                this.Designer.Context.Items.Subscribe<Selection>(ActivityFocusedChanged);
                this.Designer.ModelChanged += new EventHandler(Designer_ModelChanged);
                this.RefreshProjectExplorer();
            }
        }

        private void Designer_ModelChanged(object sender, EventArgs e)
        {
            this.RefreshProjectExplorer();
        }

        /// <summary>
        /// Changes the PE selection as workflowdesigner selection changed
        /// </summary>
        /// <param name="s"></param>
        private void ActivityFocusedChanged(Selection s)
        {
            WorkflowOutlineNode node = this.FindFocusedWorkflowOutlineNode();
            if (node != this.SelectedWorkflowOutlineNode)
                this.SelectedWorkflowOutlineNode = node;
        }

        /// <summary>
        /// find activities according the search conditions
        /// </summary>
        private void Search()
        {
            if (this.searchResult == null)
            {
                this.searchResult = new List<WorkflowOutlineNode>();
            }

            this.searchResult.Clear();
            if (this.SearchText != null && !string.IsNullOrEmpty(this.SearchText.Trim()))
            {
                if (this.IsSearchWholeWorkflow)
                {
                    this.SearchWorkflowOutlineNode(this.WorkflowOutlineNodes.FirstOrDefault(),
                        Filter);
                }
                else
                {
                    WorkflowOutlineNode focusedActivity = this.FindFocusedWorkflowOutlineNode();
                    if (focusedActivity != null)
                    {
                        this.SearchWorkflowOutlineNode(focusedActivity, Filter);
                    }
                }
                if (!this.searchResult.Any())
                {
                    this.SearchPositionNotify = NoResult;
                }
            }
        }

        /// <summary>
        /// Find the workflowoutlinenode that user focused
        /// </summary>
        /// <returns></returns>
        private WorkflowOutlineNode FindFocusedWorkflowOutlineNode()
        {
            ModelItem focusedActivity = this.Designer.Context.Items.GetValue<Selection>().PrimarySelection;
            WorkflowOutlineNode root = this.WorkflowOutlineNodes.FirstOrDefault();
            Queue<WorkflowOutlineNode> queue = new Queue<WorkflowOutlineNode>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                WorkflowOutlineNode top = queue.Dequeue();
                if (top.Model == focusedActivity)
                {
                    return top;
                }
                else if (top.Children != null)
                {
                    top.Children.ToList().ForEach(i => queue.Enqueue(i));
                }
            }
            return null;
        }

        private WorkflowOutlineNode Next(IEnumerable<WorkflowOutlineNode> source, Func<WorkflowOutlineNode, bool> predicate)
        {
            bool shouldReturn = false;
            foreach (WorkflowOutlineNode item in source)
            {
                if (shouldReturn)
                    return item;

                if (predicate(item))
                    shouldReturn = true;
            }
            return default(WorkflowOutlineNode);
        }

        private WorkflowOutlineNode Previous(IEnumerable<WorkflowOutlineNode> source, Func<WorkflowOutlineNode, bool> predicate)
        {
            WorkflowOutlineNode previous = default(WorkflowOutlineNode);
            foreach (WorkflowOutlineNode item in source)
            {
                if (predicate(item))
                    return previous;
                previous = item;
            }
            return default(WorkflowOutlineNode);
        }

        /// <summary>
        ///  Find with filter.
        /// </summary>
        /// <param name="startingItem"></param>
        /// <param name="filter"></param>
        private void SearchWorkflowOutlineNode(WorkflowOutlineNode startingItem, Predicate<WorkflowOutlineNode> filter)
        {
            if (startingItem != null)
            {
                if (filter(startingItem))
                    this.searchResult.Add(startingItem);
                if (startingItem.Children != null && startingItem.Children.Any())
                    foreach (var item in startingItem.Children)
                    {
                        this.SearchWorkflowOutlineNode(item, filter);
                    }
            }
        }

        private bool Filter(WorkflowOutlineNode act)
        {
            if (act == null)
                return false;
            if (act.Model != null)
            {
                var item = act.Model;

                if (IsSearchType)
                {
                    if (item.ItemType.Name.ToLower().Contains(this.SearchText.ToLower()))
                        return true;
                }

                if (IsSearchTitle && item.Properties[DisplayNamePropertyName] != null)
                {
                    if (act.NodeName.ToLower().Contains(this.SearchText.ToLower()))
                        return true;
                }

                if (IsSearchParameter)
                {
                    foreach (var property in item.Properties)
                    {
                        if (property.Value != null)
                        {
                            var value = property.Value.GetCurrentValue();
                            string strExpression = string.Empty;

                            if (property.PropertyType.Name == InArgumentTypeName
                                   || property.PropertyType.Name == OutArgumentTypeName
                                   || property.PropertyType.Name == InArgumentGenericTypeName
                                   || property.PropertyType.Name == OutArgumentGenericTypeName
                                   )
                            {
                                var argument = value as Argument;
                                if (argument != null)
                                    strExpression = ArgumentService.GetExpressionText(argument);
                            }

                            if (strExpression.ToLower().Contains(this.SearchText.ToLower()))
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Changes the PE selection as workflowdesigner selection changed
        /// </summary>
        /// <param name="s"></param>
        private void ActivitySelectionChanged(Selection s)
        {
            WorkflowOutlineNode node = this.FindFocusedWorkflowOutlineNode();
            if (node != this.SelectedWorkflowOutlineNode)
                this.SelectedWorkflowOutlineNode = node;
        }
    }
}
