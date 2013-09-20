using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Telerik.Windows.Controls;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Telerik.Windows;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using System.Activities;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Views
{
    /// <summary>
    /// Interaction logic for WorkflowProjectExplorer.xaml
    /// </summary>
    public partial class WorkflowProjectExplorerView : UserControl
    {
        private DragDropHelper dragDropHelper;
        private string oldNodeName;

        public WorkflowProjectExplorerView()
        {
            InitializeComponent();
            dragDropHelper = new DragDropHelper(projectExplorerTree);
        }

        #region project explorer
        /// <summary>
        /// Handle search click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchByKeyDown(object sender, KeyEventArgs e)
        {
            //Press K key to raise search activities
            if (e.Key == Key.Enter)
            {
                this.Search();
            }
        }

        private void SearchByLostFocus(object sender, RoutedEventArgs e)
        {
            this.Search();
        }

        private void Search() 
        {
            var vm = DataContext as ProjectExplorerViewModel;
            if (vm != null)
            {
                vm.ExecuteSearch();
                this.projectExplorerTree.Focus();
            }
        }

        /// <summary>
        /// handle Previous search click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_Previous(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as ProjectExplorerViewModel;
            if (vm != null)
            {
                vm.ExecutePreviousSearch();
                this.projectExplorerTree.Focus();
            }
        }

        /// <summary>
        /// handle next search click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_Next(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as ProjectExplorerViewModel;
            if (vm != null)
            {
                vm.ExecuteNextSearch();
                this.projectExplorerTree.Focus();
            }
        }

        /// <summary>
        /// handle TreeViewItem Selected event for DragDrop feature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_Selected(object sender, RadRoutedEventArgs e)
        {
            RadTreeViewItem item = (RadTreeViewItem)e.OriginalSource;
            dragDropHelper.DraggingItem = item;
        }

        /// <summary>
        /// handle TreeViewItem mouse move for drag drop feature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && this.projectExplorerTree.IsEditing == false)
            {
                dragDropHelper.MouseMove();
            }
        }

        /// <summary>
        /// handle TreeViewItem mouse DragOver for drag drop feature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = dragDropHelper.DragOver(e.OriginalSource as UIElement);
        }

        /// <summary>
        /// handle TreeViewItem mouse Drop for drag drop feature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_Drop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
            e.Effects = dragDropHelper.Drop(e.OriginalSource as UIElement);
        }

        private void radTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                RadTreeViewItem targetItem = e.OriginalSource as RadTreeViewItem;
                if (targetItem != null && this.projectExplorerTree.IsEditable)
                {
                    WorkflowOutlineNode oldActivity = targetItem.Item as WorkflowOutlineNode;
                    if (oldActivity != null)
                        oldNodeName = oldActivity.NodeName;
                    targetItem.IsInEditMode = true;
                    e.Handled = true;
                    targetItem.BeginEdit();
                    this.projectExplorerTree.Focus();
                }

            }

            if (e.Key == Key.Enter)
            {
                TextBox txt = sender as TextBox;
                if (txt != null && !string.IsNullOrEmpty(txt.Text))
                {
                    string newName = txt.Text;
                    RadTreeViewItem targetItem = this.projectExplorerTree.SelectedContainer;
                    if (targetItem != null && targetItem.IsInEditMode)
                    {
                        WorkflowOutlineNode newActivity = this.projectExplorerTree.SelectedItem as WorkflowOutlineNode;
                        if (newActivity != null && newName != oldNodeName && newActivity.Model.Properties[newActivity.PropertyNameOfNodeName] != null)
                        {
                            //set the new display name on activity
                            newActivity.Model.Properties[newActivity.PropertyNameOfNodeName].SetValue(newName);
                        }
                        targetItem.IsInEditMode = false;
                        e.Handled = true;
                    }

                }
            }
        }
        #endregion

        private void txtEditBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (box != null)
                box.Focus();
        }

        private void projectExplorerTree_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WorkflowOutlineNode data = this.projectExplorerTree.SelectedItem as WorkflowOutlineNode;
            if (data != null)
            {
                string path = string.Empty;
                string seperator = "\\";
                path = data.Id;
                WorkflowOutlineNode temp = data;
                while (temp.Parent != null)
                {
                    temp = temp.Parent;
                    path = temp.Id + "\\" + path;
                }
                this.projectExplorerTree.BringPathIntoView(path);
                this.projectExplorerTree.Focus();
                this.projectExplorerTree.ExpandItemByPath(path, seperator);
            }
        }

    }
}
