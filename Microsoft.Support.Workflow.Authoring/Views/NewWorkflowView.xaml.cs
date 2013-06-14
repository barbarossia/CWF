// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewWorkflowFromTemplate.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for NewWorkflowView.xaml
    /// </summary>
    public partial class NewWorkflowView
    {
        public NewWorkflowView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Cancel returns false and dismisses Dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Okay returns true and dismisses Dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            dynamic viewModel = DataContext;

            viewModel.Validate();

            if (viewModel.IsValid)
                DialogResult = true;
        }

    }
}
