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

namespace Microsoft.Support.Workflow.Authoring.Views
{
    /// <summary>
    /// Interaction logic for DefaultValueSettingsView.xaml
    /// </summary>
    public partial class DefaultValueSettingsView : Window
    {
        public DefaultValueSettingsView()
        {
            InitializeComponent();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as DefaultValueSettingsViewModel;
            if (viewModel != null && viewModel.HasChanged)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure to cancel without saving?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                    this.Close();
            }
            else { this.Close(); }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as DefaultValueSettingsViewModel;
            if (viewModel != null)
            {
                viewModel.SaveCommand.Execute();
                if (viewModel.HasSaved)
                    this.Close();
            }
        }
    }
}
