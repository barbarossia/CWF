namespace Microsoft.Support.Workflow.Authoring.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using ViewModels;

    /// <summary>
    /// Interaction logic for OpenWorkflowView.xaml. ShowDialog() returns true if a workflow is selected at exit.
    /// </summary>
    public partial class OpenWorkflowFromServerView
    {

        public OpenWorkflowFromServerView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// DataGrid doesn't support commands for double-click so we attach the command in in code-behind
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkflowsGridRowLoaded(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;

            if (row != null)
            {
                row.InputBindings.Add(
                    new MouseBinding(((OpenWorkflowFromServerViewModel)DataContext).OpenSelectedWorkflowCommand,
                                     new MouseGesture() { MouseAction = MouseAction.LeftDoubleClick }));
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void WorkflowsGridSorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;
            ((OpenWorkflowFromServerViewModel)DataContext).SortCommand.Execute(e.Column.SortMemberPath);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((sender as TextBox) != null)
            {
                if ((e.Key == Key.Enter) && (!string.IsNullOrEmpty((sender as TextBox).Text)))
                {
                    SearchButton.Command.Execute(null);
                    e.Handled = true;
                }
            }
        }
    }
}
