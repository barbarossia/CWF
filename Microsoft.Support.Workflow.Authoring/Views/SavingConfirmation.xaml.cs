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
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.AddIns;

namespace Microsoft.Support.Workflow.Authoring.Views
{
    /// <summary>
    /// Interaction logic for PromptMessage.xaml
    /// </summary>
    public partial class SavingConfirmation : Window
    {
        public SavingConfirmation()
        {
            InitializeComponent();
        }

        public SavingResult? Result { get; set; }

        public static SavingResult? ShowAsDialog(string message, string caption, bool canKeepLocked, bool shouldUnlock, bool unlockVisibility)
        {
            var view = new SavingConfirmation();
            var viewModel = new SavingConfirmationViewModel(view, caption, message, canKeepLocked, shouldUnlock, unlockVisibility);
            view.DataContext = viewModel;
            Window parent = Utility.FuncGetCurrentActiveWindow(App.Current);
            if (parent != null)
                view.Owner = parent;
            view.ShowDialog();
            return view.Result;
        }
    }
}
