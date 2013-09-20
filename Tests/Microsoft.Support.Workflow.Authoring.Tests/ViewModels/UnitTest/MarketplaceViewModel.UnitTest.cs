using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Text;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Windows;
using System;
using AuthoringToolTests.Services;
using System.Collections.ObjectModel;
using Microsoft.Support.Workflow.Authoring.Common;
using System.Collections.Generic;
using Microsoft.Support.Workflow.Authoring;
using System.Activities;
using System.Linq;
using Microsoft.Support.Workflow.Authoring.Tests.Services;
using System.Diagnostics;
using System.IO;
using Microsoft.Support.Workflow.Authoring.ViewModels.Marketplace;
using System.Security.Principal;
using Microsoft.Support.Workflow.Authoring.Tests.HelpClass;
using Microsoft.DynamicImplementations;
using CWF.DataContracts.Marketplace;
using Microsoft.Support.Workflow.Authoring.Security;
using System.Threading;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
namespace Authoring.Tests.Unit
{
    [TestClass]
    public class MarketplaceViewModelUnitTests
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [WorkItem(322313)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MarketplaceAsset_PropertyChangedNotificationsAreRaised()
        {
            using (new CachingIsolator())

            using (var principal = new Implementation<WindowsPrincipal>())
            {
                using (var viewModel = new Implementation<MarketplaceViewModel>())
                {
                    var client = new WorkflowsQueryServiceClient();

                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client;
                    viewModel.Register(inst => inst.SearchMarketplaceAssets(client)).Execute(() => { });

                    // TODO: setup permissions
                    Thread.CurrentPrincipal = principal.Instance;

                    var vm = viewModel.Instance;
                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "DownloadProgress", () => vm.DownloadProgress = "1/3");
                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "IsDownloadCompleted", () => vm.IsDownloadCompleted = false);
                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "ResultList", () => vm.ResultList = new ObservableCollection<MarketplaceAssetModel>());
                    Assert.IsTrue(vm.NoResultsTextVisible);

                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "IsBeginDownload", () => vm.IsBeginDownload = true);
                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SortMemberPath", () => vm.SortMemberPath = "Name");
                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "IsAscending", () => vm.IsAscending = true);

                    vm.AssetDownloader = null;
                    Assert.AreEqual(vm.AssetDownloader, null);

                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "IsOnlyShowLatestVersion", () => vm.IsOnlyShowLatestVersion = true);
                    Assert.AreEqual(vm.IsOnlyShowLatestVersion, true);

                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "FilterListEntries", () => vm.FilterListEntries = null);
                    Assert.AreEqual(vm.FilterListEntries, null);

                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedAssetItem", () => vm.SelectedAssetItem = null);
                    Assert.AreEqual(vm.SelectedAssetItem, null);

                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "ResultsPerPage", () => vm.ResultsPerPage = 1);
                    Assert.AreEqual(vm.ResultsPerPage, 1);

                    vm.PageChangedCommand.Execute();

                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                }
            }
        }

        [WorkItem(322335)]
        [Description("Check the default search criterion when the marketplace load at first.")]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MarketplaceAsset_VerifyDefaultSearchCriterionAfterLoadingMarketplace_Author()
        {
            using (new CachingIsolator())
            {
                TestUtilities.RegistLoginUserRole(Role.Admin);
                var vm = new MarketplaceViewModel(null);
                Assert.AreEqual(vm.FilterListEntries.Count, 5);

                int resultsPerPage = int.Parse(System.Configuration.ConfigurationManager.AppSettings["RowsPerPage"]);
                Assert.AreEqual("UpdatedDate", vm.SortMemberPath);
                Assert.AreEqual(false, vm.IsAscending);
                Assert.AreEqual(string.IsNullOrEmpty(""), string.IsNullOrEmpty(vm.SearchText));
                Assert.AreEqual("ALL", vm.Filter);
                Assert.AreEqual(resultsPerPage, vm.ResultsPerPage);
                Assert.AreEqual(1, vm.CurrentPage);
            }
        }

        [WorkItem(322334)]
        [Description("Check the default search criterion when the marketplace load at first.")]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MarketplaceAsset_VerifyDefaultSearchCriterionAfterLoadingMarketplace_Admin()
        {
            using (new CachingIsolator())
            {
                TestUtilities.RegistLoginUserRole(Role.Admin);
                var vm = new MarketplaceViewModel(null);
                Assert.AreEqual(vm.FilterListEntries.Count, 5);
            }
        }

        [WorkItem(322326)]
        [Description("activitiesSelections and projectSelections should be clear, all MarketplaceAsset's IsMarkedForDownload is set to false after download.")]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MarketplaceAsset_VerifyClearDownloadSelected_SuccessfulDownload()
        {
            using (new CachingIsolator())
            using (var principal = new Implementation<WindowsPrincipal>())
            {
                // TODO: setup permissions
                Thread.CurrentPrincipal = principal.Instance;

                var dataHelper = new MarketplaceDataHelper(TestContext);
                using (var cach = new CachingIsolator(dataHelper.GetTestActivities().ToArray()))
                {
                    var vm = new MarketplaceViewModel(null);
                    vm.ResultList = new ObservableCollection<MarketplaceAssetModel>(dataHelper.GetMarketplaceSearchResult().Items.Select(i => (MarketplaceAssetModel)i));
                    using (var impDownloader = new Implementation<MarketplaceAssetDownloader>())
                    {
                        impDownloader.Register(inst => inst.StartDownload()).Execute(() =>
                        {
                            var privateObj = new PrivateObject(impDownloader.Instance);
                            privateObj.Invoke("SetDownloadProgress");
                            impDownloader.Instance.RaiseDownloadCompleted();
                        });

                        vm.AssetDownloader = impDownloader.Instance;
                        vm.ResultList.ToList().ForEach(item => item.IsMarkedForDownload = true);
                        vm.DownloadCommand.Execute();

                        Assert.AreEqual(vm.DownloadProgress, "Downloading... (0/" + vm.ResultList.Count.ToString() + ")");
                        Assert.IsTrue(vm.ResultList.All(item => item.IsMarkedForDownload == false), "Failed to set IsMarkedForDownload to false after download");
                        Assert.IsTrue(vm.ActivitySelections.Count == 0, "Failed to clear activity selections after download.");
                        Assert.IsTrue(vm.ProjectSelections.Count == 0, "Failed to clear project selections after download.");
                    }
                }
            }
        }

        [WorkItem(322327)]
        [Description("activitiesSelections and projectSelections should be clear, all MarketplaceAsset's IsMarkedForDownload is set to false after download.")]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MarketplaceAsset_VerifyClearDownloadSelected_UnSuccessfulDownload()
        {
            using (new CachingIsolator())
            using (var principal = new Implementation<WindowsPrincipal>())
            {
                // TODO: setup permissions
                Thread.CurrentPrincipal = principal.Instance;

                var dataHelper = new MarketplaceDataHelper(TestContext);
                var vm = new MarketplaceViewModel(null);
                vm.ResultList = new ObservableCollection<MarketplaceAssetModel>(dataHelper.GetMarketplaceSearchResult().Items.Select(i => (MarketplaceAssetModel)i));

                using (var impDownloader = new Implementation<MarketplaceAssetDownloader>())
                {
                    impDownloader.Register(inst => inst.StartDownload()).Execute(() =>
                    {
                        impDownloader.Instance.IsCancelDownload = true;
                        impDownloader.Instance.RaiseDownloadCompleted();
                    });

                    vm.ResultList.ToList().ForEach(item => item.IsMarkedForDownload = true);
                    vm.AssetDownloader = impDownloader.Instance;
                    vm.DownloadCommand.Execute();

                    Assert.IsTrue(vm.ResultList.All(item => item.IsMarkedForDownload == false), "Failed to set IsMarkedForDownload to false after download");
                    Assert.IsTrue(vm.ActivitySelections.Count == 0, "Failed to clear activity selections after download.");
                    Assert.IsTrue(vm.ProjectSelections.Count == 0, "Failed to clear project selections after download.");
                }
            }
        }

        [WorkItem(322338)]
        [Description("When SearchCommand execute, ResultList should reflect to activitiesSelections and projectSelections")]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MarketplaceAsset_VerifyDownloadSelectedWhenSearchExecute()
        {
            using (new CachingIsolator())
                TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                {
                    using (var principal = new Implementation<WindowsPrincipal>())
                    {
                        // TODO: setup permissions

                        var dataHelper = new MarketplaceDataHelper(TestContext);

                        Thread.CurrentPrincipal = principal.Instance;
                        using (var cach = new CachingIsolator(dataHelper.GetTestActivities().ToArray()))
                        {
                            using (var impClient = new Implementation<WorkflowsQueryServiceClient>())
                            {
                                impClient.Register(inst => inst.SearchMarketplace(Argument<MarketplaceSearchQuery>.Any)).Return(dataHelper.GetMarketplaceSearchResult());

                                var vm = new MarketplaceViewModel(null);
                                vm.SearchMarketplaceAssets(impClient.Instance);
                                vm.ResultList.ToList().ForEach(i =>
                                {
                                    i.IsMarkedForDownload = true;
                                });

                                vm.SearchMarketplaceAssets(impClient.Instance);
                                Assert.IsTrue(vm.ResultList.All(i => i.IsMarkedForDownload == true));

                                var list = vm.ResultList.Where(i => i.AssetType == AssetType.Activities).ToList();
                                Assert.IsTrue(list.All(item => item.IsDownloaded == true));
                            }
                        }
                    }
                });
        }

        [WorkItem(322303)]
        [Description("check if the download command can execute when IsBeginDownload is set to true")]
        [Owner("v-kason")]
        [TestCategory("Unit")]
        [TestMethod]
        public void MarketplaceAsset_Download_CanExecute()
        {
            using (new CachingIsolator())
            using (var principal = new Implementation<WindowsPrincipal>())
            {
                // TODO: setup permissions
                Thread.CurrentPrincipal = principal.Instance;

                var dataHelper = new MarketplaceDataHelper(TestContext);
                var vm = new MarketplaceViewModel(null);
                vm.ResultList = new ObservableCollection<MarketplaceAssetModel>(dataHelper.GetMarketplaceSearchResult().Items.Select(i => (MarketplaceAssetModel)i));

                var download = vm.DownloadCommand;
                vm.IsBeginDownload = true;
                Assert.AreEqual(false, download.CanExecute());

                vm.IsBeginDownload = false;
                vm.ResultList.ToList().ForEach(i => i.IsMarkedForDownload = true);
                Assert.AreEqual(true, download.CanExecute());
            }
        }

        [WorkItem(321183)]
        [Description("Check if the canceldownload can execute when the IsBeginDownload is set to true")]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MarketplaceAsset_CancelDownload_CanExecute()
        {
            using (new CachingIsolator())
            using (var principal = new Implementation<WindowsPrincipal>())
            {
                // TODO: setup permissions
                Thread.CurrentPrincipal = principal.Instance;

                using (var impClient = new Implementation<WorkflowsQueryServiceClient>())
                {
                    impClient.Register(inst => inst.SearchMarketplace(Argument<MarketplaceSearchQuery>.Any)).Return(null);
                    var vm = new MarketplaceViewModel(null);
                    var cancelDownload = vm.CancelDownloadCommand;

                    vm.IsBeginDownload = true;
                    Assert.AreEqual(true, cancelDownload.CanExecute());
                }
            }
        }
        [WorkItem(322319)]
        [Description("check if the selections are clear after cancel download")]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MarketplaceAsset_VerifyCancelDownload()
        {
            using (new CachingIsolator())
            using (var principal = new Implementation<WindowsPrincipal>())
            {
                // TODO: setup permissions
                Thread.CurrentPrincipal = principal.Instance;

                var dataHelper = new MarketplaceDataHelper(TestContext);
                var vm = new MarketplaceViewModel(null);
                vm.IsDownloadCompleted = false;
                vm.IsBeginDownload = true;
                vm.ResultList = new ObservableCollection<MarketplaceAssetModel>(dataHelper.GetMarketplaceSearchResult().Items.Select(i => (MarketplaceAssetModel)i));
                vm.ResultList.ToList().ForEach(i => i.IsMarkedForDownload = true);

                vm.CancelDownloadCommand.Execute();
                Assert.IsFalse(vm.IsDownloadCompleted);
                Assert.IsFalse(vm.IsBeginDownload);

                Assert.IsTrue(vm.ResultList.All(i => i.IsMarkedForDownload == false));
            }
        }

        [WorkItem(322339)]
        [Description("Verify FilterDescription Changes when Filter Changes")]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MarketplaceAsset_VerifyFilterDescChangedAfterFilterChanged()
        {
            using (new CachingIsolator())
            using (var principal = new Implementation<WindowsPrincipal>())
            {
                // TODO: setup permissions
                Thread.CurrentPrincipal = principal.Instance;
                TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                {
                    //return no result
                    using (var impClient = new Implementation<WorkflowsQueryServiceClient>())
                    {
                        impClient.Register(inst => inst.SearchMarketplace(Argument<MarketplaceSearchQuery>.Any)).Return(null);
                        using (var vm = new Implementation<MarketplaceViewModel>())
                        {
                            using (var utility = new ImplementationOfType(typeof(WorkflowsQueryServiceUtility)))
                            {
                                utility.Register(() => WorkflowsQueryServiceUtility.UsingClient(vm.Instance.SearchMarketplaceAssets)).Execute(() =>
                                {
                                    return;
                                });
                                var privateObj = new PrivateObject(vm.Instance);

                                vm.Instance.Filter = "ALL";
                                privateObj.Invoke("BuildMarketplaceSearchQuery");
                                Assert.AreEqual(vm.Instance.FilterDescription, "ALL PROJECTS AND ACTIVITIES");

                                vm.Instance.Filter = "PROJECTS";
                                privateObj.Invoke("BuildMarketplaceSearchQuery");
                                Assert.AreEqual(vm.Instance.FilterDescription, "PROJECTS");

                                vm.Instance.Filter = "ACTIVITIES";
                                privateObj.Invoke("BuildMarketplaceSearchQuery");
                                Assert.AreEqual(vm.Instance.FilterDescription, "ACTIVITIES");

                                vm.Instance.Filter = "TEMPLATES";
                                privateObj.Invoke("BuildMarketplaceSearchQuery");
                                Assert.AreEqual(vm.Instance.FilterDescription, "PROJECT TEMPLATES");

                                vm.Instance.Filter = "PUBLISHING";
                                privateObj.Invoke("BuildMarketplaceSearchQuery");
                                Assert.AreEqual(vm.Instance.FilterDescription, "PUBLISHING WORKFLOW PROJECTS");

                                vm.Instance.Filter = "";
                                privateObj.Invoke("BuildMarketplaceSearchQuery");
                                Assert.AreEqual(vm.Instance.Filter, MarketplaceFilter.None.ToString());
                            }
                        }
                    }
                });
            }
        }

        [WorkItem(322358)]
        [Description("Verify search marketplace, query service return no result, error connection...")]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void MarketplaceAsset_VerifySearchMarketplace()
        {
            using (new CachingIsolator())
            using (var principal = new Implementation<WindowsPrincipal>())
            {
                // TODO: setup permissions
                Thread.CurrentPrincipal = principal.Instance;

                var vm = new MarketplaceViewModel(null);
                var dataHelper = new MarketplaceDataHelper(TestContext);
                TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
                {
                    //return no result
                    using (var impClient = new Implementation<IWorkflowsQueryService>())
                    {
                        impClient.Register(inst => inst.SearchMarketplace(Argument<MarketplaceSearchQuery>.Any)).Return(null);

                        try
                        {
                            vm.SearchText = "abcd";
                            vm.SearchMarketplaceAssets(impClient.Instance);

                            Assert.AreEqual(vm.SearchText, "abcd");
                            Assert.AreEqual(vm.PageCount, 0);
                            Assert.AreEqual(vm.CurrentPage, 1);
                            Assert.IsFalse(vm.ResultList.Any());
                        }
                        catch (Exception)
                        {

                        }
                    }
                });
            }
        }
    }
}
