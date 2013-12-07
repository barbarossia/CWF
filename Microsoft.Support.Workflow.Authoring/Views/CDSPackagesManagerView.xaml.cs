﻿using System;
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
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using Microsoft.Support.Workflow.Authoring.ViewModels;

namespace Microsoft.Support.Workflow.Authoring.Views {
    /// <summary>
    /// Interaction logic for CDSPackagesManagerView.xaml
    /// </summary>
    public partial class CDSPackagesManagerView : Window {
        private CDSPackagesManagerViewModel vm {
            get { return (CDSPackagesManagerViewModel)DataContext; }
        }
        private List<TreeViewItem> headers = new List<TreeViewItem>();

        public CDSPackagesManagerView() {
            InitializeComponent();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e) {
            Close();
        }

        private void txtSearch_Search(object sender, RoutedEventArgs e) {
            vm.SearchWithReset();
        }

        private void TreeView_Selected(object sender, RoutedEventArgs e) {
            TreeViewItem selectedTreeViewItem = (TreeViewItem)e.OriginalSource;
            TreeViewItem parent = GetParentTreeViewItem(selectedTreeViewItem);

            if (parent != null) {
                string parentHeader = parent.Header.ToString();
                PackageSearchType type = PackageSearchType.Local;
                switch (parentHeader) {
                    case "Installed packages":
                        type = PackageSearchType.Local;
                        break;
                    case "Online":
                        type = PackageSearchType.Online;
                        break;
                    case "Updates":
                        type = PackageSearchType.Update;
                        break;
                }
                vm.SearchType = type;
                vm.Source = selectedTreeViewItem.Header.ToString();
                vm.SearchText = string.Empty;
                vm.SearchWithReset();
            }
            else {
                foreach (var header in headers) {
                    header.IsExpanded = false;
                }
                if (!headers.Contains(selectedTreeViewItem))
                    headers.Add(selectedTreeViewItem);
                selectedTreeViewItem.IsExpanded = true;
                vm.Packages = null;
            }
        }

        private TreeViewItem GetParentTreeViewItem(DependencyObject item) {
            DependencyObject parent = item;
            do {
                parent = VisualTreeHelper.GetParent(parent);
            }
            while (!(parent is TreeViewItem || parent is TreeView));
            return parent as TreeViewItem;
        }
    }
}
