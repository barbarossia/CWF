// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadAssemblyView.xaml.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using Models;
    using Services;
    using ViewModels;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    

    /// <summary>
    /// Interaction logic for UploadAssemblyView.xaml
    /// </summary>
    public partial class UploadAssemblyView
    {
     
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadAssemblyView"/> class.
        /// </summary>
        public UploadAssemblyView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The Upload button clicked. To close the window, ViewModel's UploadCommand command will be called at the same time.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CheckBox_Clicked(object sender, RoutedEventArgs e)
        {
            var assembliesCheckBox = sender as CheckBox;

            if (assembliesCheckBox != null)
            {
                var viewModel = DataContext as UploadAssemblyViewModel;

                if (viewModel != null)
                {
                    viewModel.NotifyUploadAssemblyItemChange(
                        assembliesCheckBox.DataContext as ActivityAssemblyItem,
                        assembliesCheckBox.IsChecked.GetValueOrDefault());
                }
            }
        }
   
        public static void ShowAsDialog()
        {
            var vm = new UploadAssemblyViewModel();
            vm.Initialize(Caching.ActivityAssemblyItems);
            var v = new UploadAssemblyView { Owner = Application.Current.MainWindow, DataContext = vm };
            v.ShowDialog();
        }
      
    }
}