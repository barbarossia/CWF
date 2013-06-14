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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using System.ComponentModel;

namespace Microsoft.Support.Workflow.Authoring.ExpressionEditor
{
    /// <summary>
    /// Interaction logic for IntellisensePopup.xaml
    /// </summary>
    public partial class IntellisensePopup
    {
        private ToolTip toolTip;
        private TreeNode treeNode;
        /// <summary>
        /// ViewModel of this view
        /// </summary>
        public IntellisensePopupViewModel ViewModel { get; private set; }

        /// <summary>
        /// Contructor
        /// </summary>
        public IntellisensePopup()
        {
            InitializeComponent();

            ViewModel = new IntellisensePopupViewModel();
            ViewModel.PropertyChanged += new PropertyChangedEventHandler(ViewModelPropertyChanged);
            DataContext = ViewModel;
        }

        internal event EventHandler<KeyEventArgs> ListBoxKeyDown;
        internal event EventHandler ListBoxItemDoubleClick;

        protected void OnListBoxItemDoubleClick(object sender, EventArgs e)
        {
            ListBoxItemDoubleClick(sender, e);
        }

        protected void OnListBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            ListBoxKeyDown(sender, e);
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedIndex":
                case "SelectedItem":
                    if (ViewModel.SelectedItem != null)
                    {
                        IntelliSenseListBox.ScrollIntoView(ViewModel.SelectedItem);
                        IntelliSenseListBox.UpdateLayout();
                        ListBoxItem item = IntelliSenseListBox.ItemContainerGenerator.ContainerFromItem(ViewModel.SelectedItem) as ListBoxItem;
                        if (treeNode == ViewModel.SelectedItem)
                        {
                            if (toolTip != null)
                                toolTip.PlacementTarget = item;
                        }
                        else
                        {
                            if (toolTip != null)
                                toolTip.IsOpen = false;
                            if (!string.IsNullOrEmpty(ViewModel.SelectedItem.Description))
                            {
                                toolTip = new ToolTip() { Content = ViewModel.SelectedItem.Description };
                                toolTip.PlacementTarget = item;
                                toolTip.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                                toolTip.IsOpen = true;
                            }
                            treeNode = ViewModel.SelectedItem;
                        }
                    }
                    else
                    {
                        if (toolTip != null)
                            toolTip.IsOpen = false;
                    }

                    break;
            }
        }

        private void ListBoxMouseLeftButtonDown(object sender, EventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            TreeNode node = (TreeNode)item.Content;
            ViewModel.SelectedItem = node;
        }
    }
}
