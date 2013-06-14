// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReviewActivityView.xaml.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using ViewModels;

    /// <summary>
    /// Interaction logic for ReviewActivityView.xaml
    /// </summary>
    public partial class ReviewActivityView : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewActivityView"/> class.
        /// </summary>
        public ReviewActivityView()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectedCellChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var viewModel = DataContext as ReviewActivityViewModel;

            if (viewModel != null)
            {
                 if (viewModel.SelectedActivityItem != null)
                 {
                     activityItemView.SelectedCategory = viewModel.SelectedActivityItem.Category;
                 }
            }
        }

        /// <summary>
        /// Handles the click event of the Cancel button.
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
        /// Handles the click event of the OK Button.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
       
    }
}