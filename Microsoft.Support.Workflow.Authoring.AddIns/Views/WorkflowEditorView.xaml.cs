// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowEditorView.xaml.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.AddIns.Views
{
    using System;
    using System.Activities;
    using System.Activities.Presentation;
    using System.Activities.Presentation.Model;
    using System.Activities.Presentation.Services;
    using System.Activities.Presentation.View;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
    using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
    using Practices.Prism.Commands;
    using Utilities;
    using System.Windows.Documents;
    using System.Text;

    /// <summary>
    /// Interaction logic for WorkflowItemView.xaml
    /// </summary>
    public partial class WorkflowEditorView
    {
        private const string MakeStepOnHead = "_Make step";
        private const string UpdateAllOthersOnHead = "_Update all others";
        private const string AssignOnHead = "_Assign";
        private const string MergeOnHead = "_Merge";
        private const string MergeAllOnHead = "_Merge All";
        private const string UnassignOnHead = "_Unassign";
        private const string UnassignAllOnHead = "_Unassign All";
        private DelegateCommand MakeStepCommand { get; set; }
        private DelegateCommand UpdateCommand { get; set; }
        private DelegateCommand AssignCommand { get; set; }
        private DelegateCommand MergeCommand { get; set; }
        private DelegateCommand MergeAllCommand { get; set; }
        private DelegateCommand UnassignCommand { get; set; }
        private DelegateCommand UnassignAllCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowEditorView"/> class.
        /// </summary>
        public WorkflowEditorView()
        {
            InitializeComponent();
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            MakeStepCommand = new DelegateCommand(MakeStepCommandExecute);
            UpdateCommand = new DelegateCommand(UpdateCommandExecute);
            AssignCommand = new DelegateCommand(AssignCommandExecute, CanAssignCommandExecute);
            MergeCommand = new DelegateCommand(MergeCommandExecute, CanMergeCommandExecute);
            MergeAllCommand = new DelegateCommand(MergeAllCommandExecute, CanMergeAllCommandExecute);
            UnassignCommand = new DelegateCommand(UnassignCommandExecute, CanUnassignCommandExecute);
            UnassignAllCommand = new DelegateCommand(UnassignAllCommandExecute, CanUnassignAllCommandExecute);
        }

        private void UpdateCommandExecute()
        {
            var workItem = GetWorkItem();
            var selected = GetSelectdModelItem(workItem);

            workItem.CompositeWorkflow.UpdateReference(selected);
        }

        private void MakeStepCommandExecute()
        {
            var selected = GetSelectdModelItem(GetWorkItem());

            if (selected != null)
            {
                var activity = selected.GetCurrentValue() as Activity;
                if (activity != null)
                {
                    // Set DisplayName to "Step" if it isn't set already
                    if (!activity.ShouldSerializeDisplayName())
                    {
                        selected.Properties["DisplayName"].SetValue("Step");
                    }
                    // Now it has a DisplayName, which definitely makes it a step.
                    // Recompute the Step list to make sure it picks this step up.
                    ComputeSteps();
                }
            }
        }

        private bool CanAssignCommandExecute()
        {
            var workItem = GetWorkItem();
            var selected = GetSelectdModelItem(workItem);
            return !workItem.IsReadOnly && selected != null && MultipleAuthorService.CanAssign(selected);
        }

        private void AssignCommandExecute()
        {
            var selected = GetSelectdModelItem(GetWorkItem());

            try
            {
                MultipleAuthorService.Assign(selected);
            }
            catch (TaskActivityAssignException)
            {
                AddInMessageBoxService.CannotAssign();
            }
            catch (UserFacingException)
            {
                AddInMessageBoxService.CannotAssignUseSpecialActivity();
            }
        }

        private bool CanMergeCommandExecute()
        {
            var workItem = GetWorkItem();
            var selected = GetSelectdModelItem(workItem);

            return !workItem.IsReadOnly &&
                selected != null &&
                MultipleAuthorService.CheckIsTask(selected) &&
                MultipleAuthorService.CanMerge(selected);
        }

        private void MergeCommandExecute()
        {
            var workflowItem = GetWorkItem();
            var selected = GetSelectdModelItem(workflowItem);

            try
            {
                MultipleAuthorService.GetLastVersion(selected, workflowItem);
                AddInMessageBoxService.MergeCompleted();
            }
            catch (CommunicationException)
            {
                AddInMessageBoxService.CannotDownloadTask();
            }
        }

        private bool CanMergeAllCommandExecute()
        {
            return !GetWorkItem().IsReadOnly;
        }

        private void MergeAllCommandExecute()
        {
            var workflowItem = GetWorkItem();
            var allTasks = MultipleAuthorService.FindTaskActivity(workflowItem.WorkflowDesigner);
            if (!allTasks.Any())
            {
                AddInMessageBoxService.CannotMergeAll();
                return;
            }

            var canMergeTasks = allTasks.Where(t => MultipleAuthorService.CanMerge(t));
            if (!canMergeTasks.Any())
            {
                AddInMessageBoxService.CannotMergeSpecialTask();
                return;
            }

            if (allTasks.Count() > canMergeTasks.Count())
            {
                AddInMessageBoxService.PromptOnlyMergePartTasks();
            }

            try
            {
                MultipleAuthorService.GetAllLastVersion(canMergeTasks, workflowItem);
                AddInMessageBoxService.MergeCompleted();
            }
            catch (CommunicationException)
            {
                AddInMessageBoxService.CannotDownloadTask();
            }
        }

        private bool CanUnassignCommandExecute()
        {
            var workflowItem = GetWorkItem();
            var selected = GetSelectdModelItem(workflowItem);
            return !workflowItem.IsReadOnly &&
                selected != null &&
                MultipleAuthorService.CheckIsTask(selected) &&
                MultipleAuthorService.CanUnassign(selected);
        }

        private void UnassignCommandExecute()
        {
            if (AddInMessageBoxService.CannotMergeNextTime() == MessageBoxResult.OK)
            {
                MultipleAuthorService.UnassignTask(GetSelectdModelItem(GetWorkItem()));
            }
        }

        private bool CanUnassignAllCommandExecute()
        {
            return !GetWorkItem().IsReadOnly;
        }

        private void UnassignAllCommandExecute()
        {
            var workflowItem = GetWorkItem();
            var allTasks = MultipleAuthorService.FindTaskActivity(workflowItem.WorkflowDesigner);
            if (!allTasks.Any())
            {
                AddInMessageBoxService.CannotUnAssign();
                return;
            }

            var canUnassignTasks = allTasks.Where(t => MultipleAuthorService.CanUnassign(t));
            if (!canUnassignTasks.Any())
            {
                AddInMessageBoxService.CannotUnAssignSpecialTask();
                return;
            }

            if (AddInMessageBoxService.CannotMergeNextTime() == MessageBoxResult.OK)
            {
                canUnassignTasks
                .ToList()
                .ForEach(o => MultipleAuthorService.UnassignTask(o));
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == Control.DataContextProperty)
            {
                var workflowItem = e.NewValue as WorkflowEditorViewModel;
                if (workflowItem != null)
                {
                    ConfigureWorkItem(workflowItem);
                }
            }
        }

        private ModelItem GetSelectdModelItem(WorkflowEditorViewModel workItem)
        {
            return workItem.IfNotNull(dataContext => dataContext.WorkflowDesigner)
                .IfNotNull(workflowDesigner => workflowDesigner.Context)
                .Items.GetValue<Selection>().PrimarySelection;
        }

        private WorkflowEditorViewModel GetWorkItem()
        {
            return (DataContext as WorkflowEditorViewModel);
        }

        private void ConfigureWorkItem(WorkflowEditorViewModel workflowItem)
        {
            workflowItem.WorkflowDesignerChanged += OnWorkflowDesignerChanged;
            workflowItem.DesignerChanged += OnDesignerChanged;
        }

        private void OnDesignerChanged(object sender, EventArgs e)
        {
            if (stepList.Visibility == System.Windows.Visibility.Visible)
            {
                this.ComputeSteps();
            }

            RegisterActivityFocusedChanged();
        }

        private void RegisterActivityFocusedChanged()
        {
            var workflowItem = GetWorkItem();
            workflowItem.WorkflowDesigner.Context.Items.Subscribe<Selection>(ActivityFocusedChanged);
        }

        private void ActivityFocusedChanged(Selection s)
        {
            AssignCommand.RaiseCanExecuteChanged();
            MergeCommand.RaiseCanExecuteChanged();
            MergeAllCommand.RaiseCanExecuteChanged();
            UnassignCommand.RaiseCanExecuteChanged();
            UnassignAllCommand.RaiseCanExecuteChanged();
        }

        private void OnWorkflowDesignerChanged(object sender, EventArgs e)
        {
            var workflowItem = sender as WorkflowEditorViewModel;
            ConfigureWorkflowDesigner(workflowItem, workflowItem.IsTask);
        }
        /// <summary>
        /// Every time we get a new WorkflowDesigner, make sure it has a "Make Steps" context menu
        /// and recompute steps
        /// </summary>
        /// <param name="workflowDesigner"></param>
        private void ConfigureWorkflowDesigner(WorkflowEditorViewModel workflowItem, bool isTask)
        {
            ContextMenu menu = null;
            if (workflowItem.WorkflowDesigner != null)
                menu = workflowItem.WorkflowDesigner.ContextMenu;
            // There may be a better way of attaching MenuItems to each templated
            // child of a template, but for now we are checking menu item headers
            // to attach a "Make Step" menu item once and only once to a given
            // WorkflowDesigner
            if (menu != null)
            {
                if (!menu.Items.OfType<MenuItem>().Any(menuItem =>
                    object.ReferenceEquals(menuItem.Header, MakeStepOnHead)))
                {
                    menu.Items.Insert(0, new MenuItem
                    {
                        Header = MakeStepOnHead,
                        Command = MakeStepCommand
                    });
                }

                if (!menu.Items.OfType<MenuItem>().Any(menuItem =>
                    object.ReferenceEquals(menuItem.Header, UpdateAllOthersOnHead)))
                {
                    menu.Items.Insert(1, new MenuItem
                    {
                        Header = UpdateAllOthersOnHead,
                        Command = UpdateCommand
                    });
                }

                if (!isTask)
                {
                    menu.Items.Insert(2, new Separator());

                    if (!menu.Items.OfType<MenuItem>().Any(menuItem =>
                        object.ReferenceEquals(menuItem.Header, AssignOnHead)))
                    {
                        menu.Items.Insert(3, new MenuItem
                        {
                            Header = AssignOnHead,
                            Command = AssignCommand
                        });
                    }

                    if (!menu.Items.OfType<MenuItem>().Any(menuItem =>
                        object.ReferenceEquals(menuItem.Header, MergeOnHead)))
                    {
                        menu.Items.Insert(4, new MenuItem
                        {
                            Header = MergeOnHead,
                            Command = MergeCommand,
                        });
                    }

                    if (!menu.Items.OfType<MenuItem>().Any(menuItem =>
                        object.ReferenceEquals(menuItem.Header, MergeAllOnHead)))
                    {
                        menu.Items.Insert(5, new MenuItem
                        {
                            Header = MergeAllOnHead,
                            Command = MergeAllCommand,
                        });
                    }

                    if (!menu.Items.OfType<MenuItem>().Any(menuItem =>
                        object.ReferenceEquals(menuItem.Header, UnassignOnHead)))
                    {
                        menu.Items.Insert(6, new MenuItem
                        {
                            Header = UnassignOnHead,
                            Command = UnassignCommand
                        });
                    }

                    if (!menu.Items.OfType<MenuItem>().Any(menuItem =>
                        object.ReferenceEquals(menuItem.Header, UnassignAllOnHead)))
                    {
                        menu.Items.Insert(7, new MenuItem
                        {
                            Header = UnassignAllOnHead,
                            Command = UnassignAllCommand
                        });
                    }

                    menu.Items.Insert(8, new Separator());

                    RegisterActivityFocusedChanged();
                }
            }
        }

        /// <summary>
        /// To export xaml code to a local file.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The mouse button event args.
        /// </param>
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var workflowItem = DataContext as WorkflowEditorViewModel;

            if (workflowItem == null)
            {
                return;
            }

            string fileName = AddInDialogService.ShowSaveDialogAndReturnResult(workflowItem.Name, "XAML files (*.xaml)|*.xaml");

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            File.WriteAllText(fileName, workflowItem.WorkflowDesigner.CompilableXaml());
        }

        /// <summary>
        /// To adjust grid row to show/hide xaml code area.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The mouse event args.
        /// </param>
        private void ShowXamlButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleBottomPanel(XamlCodeEditor);
            HighlightSelection();
        }

        private bool isInitialiedSteps = false;
        private void StepsButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleBottomPanel(stepList, () =>
            {
                if (!isInitialiedSteps)
                    ComputeSteps();
            });
        }

        private void TasksButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleBottomPanel(tasksTable);
        }

        private void ToggleBottomPanel(FrameworkElement control, Action onDisplay = null)
        {
            bool visible = control.Visibility == Visibility.Visible;

            XamlCodeEditor.Visibility
                = stepList.Visibility
                = tasksTable.Visibility
                = Visibility.Collapsed;

            if (!visible)
            {
                control.Visibility = Visibility.Visible;
                if (onDisplay != null)
                    onDisplay();
            }
        }

        private void XamlCodeEditor_MightHaveBeenEdited(object sender, RoutedEventArgs e)
        {
            var workflowItem = ((WorkflowEditorViewModel)DataContext);
            // Since XamlCodeEditor.Text has a one-way binding to workflowItem.XamlCode,
            // if the workflow's XamlCode doesn't match the actual text on the screen, 
            // then the user edited the XAML so we need to refresh the designer to 
            // display the XAML he typed.
            string txtXaml = GetRichTextXamlCode();
            if (workflowItem.XamlCode.Trim() != txtXaml)
            {
                // copy back the Xaml to where the designer can read it from
                workflowItem.RaiseDesignerChanged();
                XamlIndexTreeHelper.Refresh(txtXaml);
                workflowItem.XamlCode = txtXaml;
                workflowItem.RefreshDesignerFromXamlCode();
                ConfigureWorkflowDesigner(workflowItem, workflowItem.IsTask); // recompute steps for the new WorkflowDesigner
            }
        }

        private string GetRichTextXamlCode()
        {
            string xaml = new TextRange(this.TxtXamlCode.Document.ContentStart, this.TxtXamlCode.Document.ContentEnd).Text.Trim();
            return xaml;
        }

        ObservableCollection<ModelItem> steps = new ObservableCollection<ModelItem>();
        public ObservableCollection<ModelItem> Steps
        {
            get { return steps; }
        }

        private void ComputeSteps()
        {
            Steps.Clear();
            (DataContext as WorkflowEditorViewModel).IfNotNull(workflowItem => workflowItem.WorkflowDesigner.IfNotNull(workflowDesigner =>
            {
                var modelService = workflowDesigner.Context.Services.GetService<ModelService>();
                if (modelService != null)
                {
                    foreach (var modelActivity in modelService.Find(modelService.Root, typeof(Activity)))
                    {
                        Activity activity = modelActivity.GetCurrentValue() as Activity;
                        if (activity.ShouldSerializeDisplayName())
                        {
                            // if DisplayName has been explicitly set
                            Steps.Add(modelActivity);
                        }
                    }
                }
            }));
            isInitialiedSteps = true;
        }

        private void FocusCurrentStep(object sender, RoutedEventArgs e)
        {
            Contract.Requires(sender is Button);
            Contract.Requires(((Button)sender).Tag is ModelItem);
            var button = (Button)sender;
            var step = (ModelItem)button.Tag;
            // Focus on the selected element
            step.Focus();
        }

        /// <summary>
        /// Handles the start of the drop operation in the design surface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpressionTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if ((sender != null) && (sender is ExpressionTextBox))
            {
                ExpressionTextBox textBox = sender as ExpressionTextBox;

                //Validate the dragging value is key string         
                if (e.Data.GetDataPresent(DataFormats.StringFormat))
                {
                    textBox.BeginEdit();
                }
            }
        }

        /// <summary>
        /// In a corner case in the designer, sometimes the focus gets frozen in one element and doesn't allow
        /// for new drag and drop operations to other elements, so we need to refresh the designer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpressionTextBox_DragLeave(object sender, DragEventArgs e)
        {

            if ((sender != null) && (sender is ExpressionTextBox))
            {
                var selected = (DataContext as WorkflowEditorViewModel).IfNotNull(dataContext => dataContext.WorkflowDesigner)
                   .IfNotNull(workflowDesigner => workflowDesigner.Context)
                   .Items.GetValue<Selection>().PrimarySelection;

                //Focus wasn't updated correctly by the designer
                if ((selected == null) || (selected != (sender as ExpressionTextBox).OwnerActivity))
                {
                    //(sender as ExpressionTextBox).UpdateLayout();
                    (sender as ExpressionTextBox).Focus();
                    (sender as ExpressionTextBox).BeginEdit();
                }
            }
        }

        /// <summary>
        /// Clear the value before the drop occurs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpressionTextBox_PreviewDrop(object sender, RoutedEventArgs e)
        {
            if (sender is ExpressionTextBox)
            {
                ((ExpressionTextBox)sender).Expression = null;
            }
        }

        private void DocChanged(object s, RoutedEventArgs e)
        {
            this.findControl.Document = this.TxtXamlCode.Document;
        }

        public void OnActivityFocuceChanged(object sender, ActivityFocuceEventArgs e)
        {
            selection = e.Node;
            HighlightSelection();
        }

        private void HighlightSelection()
        {
            if (XamlCodeEditor.Visibility == Visibility.Visible)
            {
                if (selection != null)
                {
                    try
                    {
                        if (selection.Offset == 0 || selection.Length == 0)
                        {
                            XamlIndexNode index = XamlIndexTreeHelper.Search(selection);
                            selection.Offset = index.Offset;
                            selection.Length = index.Length;
                        }

                        TxtXamlCode.HighlightSelection(selection.Offset, selection.Length);
                    }
                    catch (ArgumentNullException)
                    {
                        selection = null;
                    }
                    catch (ApplicationException)
                    {
                        selection = null;
                    }
                }
            }
        }

        private WorkflowOutlineNode selection;

        private void TxtXamlCode_GotFocus(object sender, RoutedEventArgs e)
        {
            if (selection != null)
                TxtXamlCode.ClearSelction(selection.Offset, selection.Length);
        }
    }
}
