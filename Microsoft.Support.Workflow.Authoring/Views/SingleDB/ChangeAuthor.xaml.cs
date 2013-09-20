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
    /// Interaction logic for ChangeAuthor.xaml
    /// </summary>
    public partial class ChangeAuthor : Window
    {
        public ChangeAuthor()
        {
            InitializeComponent();
        }

        private void btnChangeAuthor_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ChangeAuthorViewModel;
            if (vm != null)
            {
                vm.ChangeAuthorCommand.Execute();
                if (vm.IsSaved)
                {
                    DialogResult = true;
                    this.Close();
                }
            }
        }
    }
}
