// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectAssemblyAndActivityView.xaml.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Models;
    using Services;
    using ViewModels;
    using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    /// <summary>
    /// Interaction logic for SelectAssemblyAndActivityView.xaml
    /// </summary>
    public partial class SelectAssemblyAndActivityView : Window
    {

        private SelectAssemblyAndActivityViewModel selectedAssemblyAndActivityViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectAssemblyAndActivityView"/> class.
        /// </summary>
        public SelectAssemblyAndActivityView()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                MinHeight = Height;
                MinWidth = Width;
            };
        }

        /// <summary>
        /// The cancel button clicked. To close the window.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// The get detail hyperlink_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void GetDetailHyperlink_Click(object sender, RoutedEventArgs e)
        {
            var activityAssemblyItem = (sender as Hyperlink).Tag as ActivityAssemblyItem;
            var viewModel = DataContext as SelectAssemblyAndActivityViewModel;

            WorkflowsQueryServiceUtility.UsingClient(client => viewModel.GetActivityItemsByActivityAssemblyItem(client, activityAssemblyItem));
        }

        /// <summary>
        /// The OK button clicked. To close the window.
        /// ViewModel's OkCommand will be executed at same time.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Set the foreground of text in the selected caching row to "black"
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void DataGridRow_OnSelected(object sender, RoutedEventArgs e)
        {
            var row = (DataGridRow)sender;
            if (row != null)
            {
                var assemblyItem = row.DataContext as ActivityAssemblyItem;
                if (assemblyItem.IsReviewed)
                {
                    DataGridCellsPresenter cellsPresenter = GetVisualChild<DataGridCellsPresenter>(row);
                    if (cellsPresenter != null)
                    {
                        DataGridCell cell = (DataGridCell)cellsPresenter.ItemContainerGenerator.ContainerFromIndex(3);
                        cell.Foreground = new SolidColorBrush(Colors.Black);
                    }
                }
            }
        }

        private void DataGrid_SelectedCellChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (selectedAssemblyAndActivityViewModel == null)
                selectedAssemblyAndActivityViewModel = DataContext as SelectAssemblyAndActivityViewModel;

            if (selectedAssemblyAndActivityViewModel.CurrentActivityItem != null)
            {
                activityItemView.SelectedStatus = selectedAssemblyAndActivityViewModel.CurrentActivityItem.Status;
                activityItemView.SelectedCategory = selectedAssemblyAndActivityViewModel.CurrentActivityItem.Category;
            }
        }

        /// <summary>
        /// Get Cells from oen Given DataGridRow
        /// </summary>
        private T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual visual = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = visual as T;
                if (child == null)
                    child = GetVisualChild<T>(visual);
                if (child != null)
                    break;
            }
            return child;
        }

        public static void ShowAsDialog()
        {
            var view = new SelectAssemblyAndActivityView { Owner = Application.Current.MainWindow };

            var viewModel = Utility.WithContactServerUI(() =>
                {
                    var viewModel1 = new SelectAssemblyAndActivityViewModel(); // Don't create it on STA thread
                    WorkflowsQueryServiceUtility.UsingClient(viewModel1.LoadLiveData);
                    return viewModel1;
                });

            view.DataContext = viewModel;
            view.ShowDialog();
        }

    }
}