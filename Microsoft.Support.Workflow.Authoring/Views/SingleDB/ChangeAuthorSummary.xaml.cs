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
    /// Interaction logic for ChangeAuthorSummary.xaml
    /// </summary>
    public partial class ChangeAuthorSummary : Window
    {
        public ChangeAuthorSummary()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
