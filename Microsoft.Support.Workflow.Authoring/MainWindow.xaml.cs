// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing.Design;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Support.Workflow.Authoring.AddIns;
    using Models;
    using Services;
    using ViewModels;
    using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    using System.Diagnostics;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const short ContentHeightOffset = 60;   // The height offset we subtract from the content canvas on resize. See mainWindow_SizeChanged().
        private MainWindowViewModel vm;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        /// <summary>
        /// The initialize data context.
        /// </summary>
        private void InitializeDataContext()
        {
            DataContextChanged += MainWindow_DataContextChanged;
        }

        /// <summary>
        /// Notification for the changing of the DataContext for the Main Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            var viewModel = DataContext as MainWindowViewModel;

            if (null != viewModel)
            {
                this.vm = viewModel;
                viewModel.PropertyChanged += (sender1, eventArgs) =>
                {
                    switch (eventArgs.PropertyName)
                    {
                        case "VersionFault": // TODO figure out why binding is not working on the version control
                            VersionDisplay.VersionFault = viewModel.VersionFault;
                            break;

                        case "ErrorMessage":
                            if (!string.IsNullOrEmpty(viewModel.ErrorMessage))
                                MessageBoxService.Show(viewModel.ErrorMessage,
                                                       viewModel.ErrorMessageType,
                                                       MessageBoxButton.OK,
                                                       MessageBoxImage.Error);
                            break;
                        case "IsToolboxVisible":
                        case "IsProjectExplorerVisible":
                        case "IsPropertiesVisible":
                            Focus();
                            break;
                    }
                };

                viewModel.WorkflowItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(WorkflowItems_CollectionChanged);
            }
        }

        void WorkflowItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var workflowItems = (ObservableCollection<WorkflowItem>)sender;
            if (workflowItems.Count == 0)
            {
                VersionDisplay.ViewModel.ChangeMajorCanExecute = false;
            }
            else
            {
                VersionDisplay.ViewModel.ChangeMajorCanExecute = true;
            }
        }


        /// <summary>
        /// Find the activity, if any, associated with ToolboxItem
        /// </summary>
        private static ActivityItem LookupToolboxItem(ToolboxItem itemToFind, IEnumerable<ActivityAssemblyItem> activityAssemblyItems)
        {
            if (itemToFind == null)
            {
                return null;
            }

            ActivityAssemblyItem hostActivityAssemblyItem =
                activityAssemblyItems.FirstOrDefault(
                    aai => aai.Matches(itemToFind.AssemblyName));

            if (hostActivityAssemblyItem != null)
            {
                return hostActivityAssemblyItem.ActivityItems.FirstOrDefault(ai => ai.FullName == itemToFind.TypeName);
            }
            return null;
        }

        /// <summary>
        /// Window closed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void MainWindowClosed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        // Workaround for WorkflowDesigner bug so that GridSplitter resize cursor will still show when workflow canvas is adjacent
        private void ResetCursor(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null; // use the default <-> cursor
        }

        private void CloseTabButton_Click(object sender, RoutedEventArgs e)
        {
            WorkflowItem itemToClose;
            if (sender != null)
            {
                itemToClose = ((Button)sender).Tag as WorkflowItem;

                if (DataContext != null)
                {
                    ((MainWindowViewModel)DataContext).CloseWorkflowCommandExecute(itemToClose);
                }
            }
        }

        private void PART_MAXIMIZE_RESTORE_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState = WindowState.Normal : WindowState = WindowState.Maximized;
        }

        private void PART_MINIMIZE_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //WorkAreaCanvas.Height = this.ActualHeight - ContentHeightOffset;
            //gridDesigner.Height = WorkAreaCanvas.Height;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            //if ((null != viewModel) && (null != viewModel.FocusedWorkflowItem))
            //    VersionDisplay.Version = viewModel.FocusedWorkflowItem.Version;
            //else
            //    VersionDisplay.Version = null;
            if (this.TabWorkflow.SelectedIndex < 0)
                this.menuDesigner.IsChecked = false;
            else
                this.menuDesigner.IsChecked = true;
        }

        private void VersionDisplay_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;

            if ((null != viewModel) && (null != viewModel.FocusedWorkflowItem))
                viewModel.FocusedWorkflowItem.Version = VersionDisplay.Version;
        }


        private void DocumentHost_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WorkAreaCanvas.Focus();
        }
    }
}
