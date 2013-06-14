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
    /// Interaction logic for SearchTaskActivitiesView.xaml
    /// </summary>
    public partial class SearchTaskActivitiesView : Window
    {
        public SearchTaskActivitiesView()
        {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void WorkflowsGridSorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;
            if (e.Column.SortMemberPath != "Tags" && e.Column.SortMemberPath != "Status")
                ((SearchTaskActivityViewModel)DataContext).SortCommand.Execute(e.Column.SortMemberPath);
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
