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
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;

namespace Microsoft.Support.Workflow.Authoring.Views
{
    /// <summary>
    /// Interaction logic for OpenActivityByAdminConfirmation.xaml
    /// </summary>
    public partial class OpenActivityConfirmation : Window
    {
        public OpenActivityConfirmation()
        {
            InitializeComponent();
        }

        public MessageBoxResult Result { get; set; }

        public static MessageBoxResult ShowAsDialog(string message, string caption)
        {
            var view = new OpenActivityConfirmation();
            var viewmodel = new OpenActivityConfirmationViewModel(view, caption, message);
            view.DataContext = viewmodel;
            Window parent = Utility.FuncGetCurrentActiveWindow(App.Current);
            if (parent != null)
                view.Owner = parent;
            view.ShowDialog();
            return view.Result;
        }
    }
}
