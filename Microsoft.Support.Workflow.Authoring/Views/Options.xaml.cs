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

namespace Microsoft.Support.Workflow.Authoring.Views {
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window {
        private OptionsViewModel vm {
            get { return (OptionsViewModel)DataContext; }
        }

        public Options() {
            InitializeComponent();

            Closed += (s, e) => {
                vm.Close();
            };
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e) {
            vm.Save();
            if (vm.HasSaved)
                Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e) {
            if (vm.HasSaved)
                Close();
            else {
                MessageBoxResult result = MessageBox.Show("Are you sure to cancel without saving?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                    Close();
            }
        }

        private void ApplyButtonClick(object sender, RoutedEventArgs e) {
            vm.Save();
        }
    }
}
