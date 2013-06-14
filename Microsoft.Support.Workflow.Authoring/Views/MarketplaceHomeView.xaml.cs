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
using System.Collections.Specialized;
using Microsoft.Support.Workflow.Authoring.ViewModels.Marketplace;
using System.ComponentModel;
using System.Windows.Media.Animation;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

namespace Microsoft.Support.Workflow.Authoring.Views
{
    /// <summary>
    /// Interaction logic for MarketplaceHomeView.xaml
    /// </summary>
    public partial class MarketplaceHomeView : Window
    {
        public MarketplaceHomeView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MarketplaceHomeView_Loaded);
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(Step1MarketplaceHome_DataContextChanged);
            CollectionView views = (CollectionView)CollectionViewSource.GetDefaultView(this.dgAssets.Items);
            ((INotifyCollectionChanged)views).CollectionChanged += new NotifyCollectionChangedEventHandler(MarketplaceResultsChanged);
            this.Closing += new CancelEventHandler(MarketplaceHomeView_Closing);
        }

        private void MarketplaceHomeView_Closing(object sender, CancelEventArgs e)
        {
            var vm = this.DataContext as MarketplaceViewModel;
            if (vm != null && !vm.CloseMarketplace())
                e.Cancel = true;
        }

        private void MarketplaceHomeView_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as MarketplaceViewModel;
            if (vm != null)
            {
                vm.SearchCommand.Execute();
            }
        }

        /// <summary>
        /// handle the marketplace results grid changed event,show the current sort information to user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MarketplaceResultsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var vm = this.DataContext as MarketplaceViewModel;
            if (vm != null)
            {
                DataGridColumn cl = this.dgAssets.Columns.SingleOrDefault(c => c.SortMemberPath == vm.SortMemberPath);
                cl.SortDirection = vm.IsAscending ? ListSortDirection.Ascending : ListSortDirection.Descending;
            }
        }

        /// <summary>
        /// Notification for the changing of the DataContext for the Marketplace View
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Step1MarketplaceHome_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = DataContext as MarketplaceViewModel;
            if (null != viewModel)
            {
                viewModel.PropertyChanged += (s1, e1) =>
                {
                    switch (e1.PropertyName)
                    {
                        case "IsDownloadCompleted":
                            {
                                if (viewModel.IsDownloadCompleted)
                                {
                                    Style normalStyle = App.Current.Resources["DownloadButtonFinishedStyle"] as Style;
                                    this.btnDownload.Style = normalStyle;
                                    this.btnDownload.IsEnabled = false;
                                    var storyboardDisapear = Resources["StoryboardDisapear"] as Storyboard;
                                    storyboardDisapear.Completed += (s, e2) =>
                                    {
                                        Style normalStyle1 = App.Current.Resources["DownloadButtonNormalStyle"] as Style;
                                        this.btnDownload.Style = normalStyle1;
                                        this.btnDownload.IsEnabled = true;
                                        this.btnClose.IsEnabled = true;
                                    };
                                    storyboardDisapear.Begin();
                                }
                            }
                            break;
                        case "IsBeginDownload":
                            {
                                if (viewModel.IsBeginDownload)
                                {
                                    Style btnDownloadingStyle = App.Current.Resources["DownloadButtonDownloadingStyle"] as Style;
                                    this.btnDownload.Style = btnDownloadingStyle;
                                    this.btnDownload.IsEnabled = false;
                                    this.btnClose.IsEnabled = false;
                                    this.BtnHome.IsEnabled = false;
                                    this.lbxFilters.IsEnabled = false;
                                }
                                else
                                {
                                    Style normalStyle1 = App.Current.Resources["DownloadButtonNormalStyle"] as Style;
                                    this.btnDownload.Style = normalStyle1;
                                    this.btnDownload.IsEnabled = true;
                                    this.btnClose.IsEnabled = true;
                                    this.BtnHome.IsEnabled = true;
                                    this.lbxFilters.IsEnabled = true;
                                }
                            }
                            break;
                    }
                };
            }
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Column.SortMemberPath))
            {
                var context = (MarketplaceViewModel)DataContext;
                if (context.SortMemberPath == e.Column.SortMemberPath)
                {
                    context.IsAscending = !context.IsAscending;
                }
                else
                {
                    context.SortMemberPath = e.Column.SortMemberPath;
                    context.IsAscending = true;
                    e.Column.SortDirection = ListSortDirection.Ascending;
                }
                context.SearchCommand.Execute();
            }
            e.Handled = true;
        }

        private void dgAssets_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var context = (MarketplaceViewModel)DataContext;
            var marketplaceAsset = context.SelectedAssetItem;
            if (null != marketplaceAsset && !context.IsBeginDownload)
            {
                context.OpenAssetCommand.Execute();
            }
        }

        private void SelectOrUnselectItem(object sender, RoutedEventArgs e)
        {
            var context = (MarketplaceViewModel)DataContext;
            if (context != null && !context.IsBeginDownload)
            {
                context.RaiseCommandsCanExecute();
            }
        }

        /// <summary>
        /// Search event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchTextBox_Search(object sender, RoutedEventArgs e)
        {
            var context = (MarketplaceViewModel)DataContext;
            if (context != null && !context.IsBeginDownload)
            {
                context.SearchCommand.Execute();
            }
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var context = (MarketplaceViewModel)DataContext;
            var model = btn.Tag as MarketplaceAssetModel;
            if (context != null && model != null)
            {
                context.OpenDownloadLocation(model.Location);
            }
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                e.Handled = true;
        }
    }
}
