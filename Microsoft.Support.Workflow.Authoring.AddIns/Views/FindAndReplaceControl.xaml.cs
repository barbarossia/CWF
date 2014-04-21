using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.UIControls;
using Microsoft.Support.Workflow.Authoring.ViewModels;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Views {
    /// <summary>
    /// Interaction logic for FindAndReplaceControl.xaml
    /// </summary>
    public partial class FindAndReplaceControl : UserControl {
        public FindAndReplaceViewModel VM { get; private set; }

        #region Properties

        public FlowDocument Document {
            get { return (FlowDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        public bool IsReadOnly {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        #endregion

        #region DependencyProperties

        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
            "Document",
            typeof(FlowDocument), typeof(FindAndReplaceControl), new PropertyMetadata(null, (s, e) => {
                ((FindAndReplaceControl)s).VM.Document = (FlowDocument)e.NewValue;
            }));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly",
            typeof(bool), typeof(FindAndReplaceControl), new PropertyMetadata(false, (s, e) => {
                ((FindAndReplaceControl)s).VM.IsReadOnly = (bool)e.NewValue;
            }));

        #endregion

        public FindAndReplaceControl() {
            InitializeComponent();

            KeyDown += ShortcutKeyDown;

            VM = new FindAndReplaceViewModel();
            VM.PropertyChanged += (s, e) => {
                if (e.PropertyName == DocumentProperty.Name)
                    Document = VM.Document;
                else if (e.PropertyName == "IsReplacementMode")
                    Height = VM.IsReplacementMode ? 56 : 30;
            };
            Root.DataContext = VM;

            searchTextBox.Loaded += ControlLoaded;
            searchTextBox.Search += (s, e) => { VM.Search(); };
        }

        public void ShortcutKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape) {
                CloseButtonClick(sender, e);
                e.Handled = true;
            }
        }

        public void ShowControl(bool isReplacementMode) {
            Visibility = Visibility.Visible;
            VM.IsReplacementMode = isReplacementMode;
            searchTextBox.Focus();
            searchTextBox.SelectAll();
        }

        private void ControlLoaded(object sender, RoutedEventArgs e) {
            searchTextBox.Focus();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e) {
            VM.ResetSearchResult();
            Visibility = Visibility.Collapsed;
            e.Handled = true;
        }
    }
}
