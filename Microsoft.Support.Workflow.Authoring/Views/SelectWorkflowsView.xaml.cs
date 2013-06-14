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
    /// Interaction logic for SelectWorkflowsView.xaml
    /// </summary>
    public partial class SelectWorkflowsView : Window
    {
        public SelectWorkflowsView()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((sender as TextBox) != null)
            {
                if (e.Key == Key.Enter)
                {
                    SearchButton.Command.Execute(null);
                    e.Handled = true;
                }
            }
        }
    }
}
