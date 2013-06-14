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
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
namespace Microsoft.Support.Workflow.Authoring.Views
{
    /// <summary>
    /// Interaction logic for ManageWorkflowTypeView.xaml
    /// </summary>
    public partial class ManageWorkflowTypeView : Window
    {
        public ManageWorkflowTypeView()
        {
            InitializeComponent();
        }

        private void txtSearchFilter_KeyDown(object sender, KeyEventArgs e)
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
