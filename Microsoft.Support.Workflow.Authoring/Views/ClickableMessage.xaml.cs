
using Microsoft.Support.Workflow.Authoring.ViewModels;

namespace Microsoft.Support.Workflow.Authoring.Views
{
    using System.Windows;
    using Microsoft.Support.Workflow.Authoring.Services;

    /// <summary>
    /// Interaction logic for ClickableMessage.xaml
    /// </summary>
    public partial class ClickableMessage : Window
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ClickableMessage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// seperate from ShowAsDialog for unit test
        /// </summary>
        /// <returns></returns>
        public static ClickableMessage GetClickableMessage()
        {
            return new ClickableMessage();
        }

        public static void ShowAsDialog(string message, string caption, string url)
        {
            var view = GetClickableMessage();
            var viewmodel = new ClickableMessageViewModel();
            viewmodel.Url = url;
            viewmodel.Title = caption;
            viewmodel.Message = message;
            view.DataContext = viewmodel;
            Window parent = Utility.FuncGetCurrentActiveWindow(App.Current);
            if (parent != null)
                view.Owner = parent;
            view.ShowDialog();

        }
    }
}
