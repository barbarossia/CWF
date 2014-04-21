using Microsoft.Support.Workflow.Authoring.ViewModels;
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
using System.Windows.Shapes;
using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

namespace Microsoft.Support.Workflow.Authoring.Views
{
    /// <summary>
    /// Interaction logic for EnvironmentSecurityOptions.xaml
    /// </summary>
    public partial class EnvironmentSecurityOptions : Window
    {
        public EnvironmentSecurityOptions()
        {
            InitializeComponent();
        }

        private void BtnCancel(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as EnvironmentSecurityOptionsViewModel;
            if (viewModel != null && viewModel.HasChanged)
            {
                MessageBoxResult result = MessageBox.Show(TextResources.CancelWithoutSavingConfirmationMsg, TextResources.Confirmation, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                    this.Close();
            }
            else { this.Close(); }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as EnvironmentSecurityOptionsViewModel;
            if (viewModel != null)
            {
                viewModel.SaveCommand.Execute();
                if (viewModel.HasSaved)
                    this.Close();
            }
        }
    }
}
