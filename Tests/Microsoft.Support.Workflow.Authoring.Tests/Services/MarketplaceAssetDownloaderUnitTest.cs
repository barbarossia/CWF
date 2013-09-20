using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Tests.HelpClass;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using System.IO;
using Microsoft.DynamicImplementations;
using CWF.DataContracts.Marketplace;
using CWF.DataContracts;
using AuthoringToolTests.Services;
using System.Threading;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;

namespace Microsoft.Support.Workflow.Authoring.Tests.Services
{
    [TestClass]
    public class MarketplaceAssetDownloaderUnitTest
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [WorkItem(322351)]
        [TestCategory("Unit-NoDif")]
        [Owner("v-kason")]
        [TestMethod]
        public void AssetDownloader_VerifyPropertyChanged()
        {
            var downloader = new MarketplaceAssetDownloader(null, null);
            downloader.MarketpalceAssetsToDownload = null;
            Assert.AreEqual(downloader.MarketpalceAssetsToDownload, null);
        }

        [WorkItem(322364)]
        [Description("verify if download is completed when MarketpalceAssetsToDownload is null or empty")]
        [TestCategory("Unit-NoDif")]
        [Owner("v-kason")]
        [TestMethod]
        public void AssetDownloader_VerifyShouldStopDownload()
        {
            MarketplaceDataHelper helper = new MarketplaceDataHelper(this.TestContext);
            var activities = helper.GetTestActivities();
            var models = helper.GetMarketplaceSearchResult().Items.Select(i => (MarketplaceAssetModel)i);
            var asset = new System.Collections.ObjectModel.ObservableCollection<MarketplaceAssetModel>(models.Where(i => i.AssetType == AssetType.Activities));

            var downloader = new MarketplaceAssetDownloader(null, null);

            downloader.IsCancelDownload = true;
            Assert.IsTrue(downloader.ShouldStopDownload());

            downloader.IsCancelDownload = false;
            downloader.MarketpalceAssetsToDownload = null;
            Assert.IsTrue(downloader.ShouldStopDownload());

            downloader.IsCancelDownload = false;
            downloader.MarketpalceAssetsToDownload = asset;
            Assert.IsFalse(downloader.ShouldStopDownload());
        }

        [WorkItem(322366)]
        [Description("verify if download can execute")]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [TestMethod]
        public void AssetDownloader_VerifyStartDownload()
        {
            MarketplaceDataHelper helper = new MarketplaceDataHelper(this.TestContext);
            var activities = helper.GetTestActivities();
            var models = helper.GetMarketplaceSearchResult().Items.Select(i => (MarketplaceAssetModel)i);
            var asset = new System.Collections.ObjectModel.ObservableCollection<MarketplaceAssetModel>(models.Where(i => i.AssetType == AssetType.Activities));

            using (new CachingIsolator())
            using (var client = new Implementation<WorkflowsQueryServiceClient>())
            {
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                using (var downloader = new Implementation<MarketplaceAssetDownloader>())
                {
                    bool assetIsDownloaded = false;


                    //cancel download if no asset to download;
                    downloader.Instance.IsCancelDownload = false;
                    downloader.Instance.MarketpalceAssetsToDownload = null;
                    downloader.Instance.StartDownload();
                    Assert.IsTrue(downloader.Instance.IsCancelDownload);
                    Assert.AreEqual(downloader.Instance.DownloadedNumber, 0);
                    Assert.AreEqual(downloader.Instance.ToDownloadCount, 0);

                    //can download
                    downloader.Register(d => d.DownloadProjects(client.Instance))
                        .Execute(() =>
                        {
                            assetIsDownloaded = true;
                            return;
                        });
                    downloader.Register(d => d.DownloadActivities(client.Instance))
                        .Execute(() =>
                        {
                            assetIsDownloaded = true;
                            return;
                        });
                    downloader.Instance.IsCancelDownload = false;
                    downloader.Instance.MarketpalceAssetsToDownload = asset;

                    downloader.Instance.DownloadCompleted += (s, e) =>
                    {
                        assetIsDownloaded = true;
                    };

                    string progress = string.Empty;
                    downloader.Instance.DownloadedPercentChanged += (s, e) =>
                    {
                        progress = e.DownloadPercent;
                    };

                    downloader.Instance.StartDownload();
                    Thread.Sleep(2000);//wait fow startdownload complete.
                    Assert.IsTrue(assetIsDownloaded);
                    Assert.AreEqual(progress, "0/" + downloader.Instance.ToDownloadCount.ToString());
                }
            }
        }

        [WorkItem(322337)]
        [Description("Verify StartDownload function,check if all selected AssemblyActivityItem's UserSelected is setted to ture, and all projects is saved to localmachine")]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void AssetDownloader_VerifyDownloadActivities()
        {
            MarketplaceDataHelper helper = new MarketplaceDataHelper(this.TestContext);
            var activities = helper.GetTestActivities();
            var models = helper.GetMarketplaceSearchResult().Items.Select(i => (MarketplaceAssetModel)i);
            var asset = new System.Collections.ObjectModel.ObservableCollection<MarketplaceAssetModel>(models.Where(i => i.AssetType == AssetType.Activities));
            var loader = new MarketplaceAssetDownloader(null, null);

            using (new CachingIsolator())
            using (var client = new Implementation<IWorkflowsQueryService>())
            {
                using (var assemblyDownloader = new ImplementationOfType(typeof(AssemblyDownloader)))
                {
                    var request = new List<ActivityAssemblyItem>() 
                    {
                        new ActivityAssemblyItem { Name = asset[loader.DownloadedNumber].Name, Version = System.Version.Parse(asset[loader.DownloadedNumber].Version) } 
                    };

                    client.Register(inst => inst.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any))
                       .Execute(() =>
                       {
                           var actDc = DataContractTranslator.AssemblyItemToActivityLibraryDataContract(activities[loader.DownloadedNumber]);
                           actDc.Executable = helper.GetActivityData(activities[loader.DownloadedNumber].Location);
                           return new List<ActivityLibraryDC> { actDc };
                       });


                    client.Register(inst => inst.StoreActivityLibraryDependenciesTreeGet(Argument<StoreActivityLibrariesDependenciesDC>.Any))
                    .Return(new List<StoreActivityLibrariesDependenciesDC>());

                    client.Register(inst => inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any))
                        .Execute(() =>
                        {
                            var replay1 = new GetAllActivityLibrariesReplyDC();
                            replay1.List = new List<ActivityLibraryDC>();
                            return replay1;
                        });

                    assemblyDownloader.Register(() => AssemblyDownloader.GetActivityItemsByActivityAssemblyItem(Argument<ActivityAssemblyItem>.Any, client.Instance))
                        .Execute(() =>
                        {
                            return new List<ActivityItem>();
                        });

                    loader.MarketpalceAssetsToDownload = asset;
                    Assert.AreEqual(loader.ToDownloadCount, asset.Count);
                    loader.DownloadActivities(client.Instance);
                    Assert.AreEqual(loader.ToDownloadCount, Caching.ActivityAssemblyItems.Count, "Failed to download all activities");
                    Assert.IsTrue(Caching.ActivityAssemblyItems.Any(i => i.UserSelected == true), "Downloaded Assembly is not set UserSelected.");
                }
            }
        }

        [WorkItem(322349)]
        [Description("Verify StartDownload function,check if all selected projects is downloaded")]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void AssetDownloader_VerifyProjectsDownload()
        {
            MarketplaceDataHelper helper = new MarketplaceDataHelper(this.TestContext);
            var wf = helper.GetTestProjects()[0];
            //wf.InitializeWorkflowDesigner();
            //wf.WorkflowDesigner.RefreshDesignerFromXamlCode();
            StoreLibraryAndActivitiesRequestDC dc = DataContractTranslator.WorkflowToStoreLibraryAndActivitiesRequestDC(wf, new List<ActivityAssemblyItem>(), new List<TaskAssignment>());
            var storeActivities = dc.StoreActivitiesList;
            int downloadCount = 0;
            var models = helper.GetMarketplaceSearchResult().Items.Select(i => (MarketplaceAssetModel)i);

            using (new CachingIsolator())
            using (var cach = new ImplementationOfType(typeof(Caching)))
            {
                using (var client = new Implementation<WorkflowsQueryServiceClient>())
                {
                    using (var utility = new ImplementationOfType(typeof(Utility)))
                    {
                        client.Register(inst => inst.StoreActivitiesGet(Argument<StoreActivitiesDC>.Any))
                            .Execute(delegate(StoreActivitiesDC c)
                            {
                                downloadCount++;
                                return new List<StoreActivitiesDC>() { c };
                            });

                        bool isGot = false;
                        List<ActivityAssemblyItem> references = null;
                        cach.Register(() => Caching.ComputeDependencies(client.Instance, Argument<ActivityAssemblyItem>.Any)).Return(new List<ActivityAssemblyItem>());
                        cach.Register(() => Caching.CacheAndDownloadAssembly(client.Instance, Argument<List<ActivityAssemblyItem>>.Any)).Execute(() =>
                        {
                            isGot = true;
                            return references;
                        });

                        utility.Register(() => Utility.GetProjectsDirectoryPath()).Return(".");
                        var loader = new MarketplaceAssetDownloader(null, null);
                        loader.MarketpalceAssetsToDownload =
                               new System.Collections.ObjectModel.ObservableCollection<MarketplaceAssetModel>(models.Where(i => i.AssetType == AssetType.Project));
                        loader.DownloadProjects(client.Instance);
                        Assert.AreEqual(loader.ToDownloadCount, downloadCount, "Failed to download all projects");
                    }
                }
            }
        }

        [WorkItem(322320)]
        [Description("Verify cancel download function,check if all downloaded asset is deleted")]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void AssetDownloader_VerifyCancelDownload()
        {
            MarketplaceDataHelper helper = new MarketplaceDataHelper(this.TestContext);
            var asset = helper.GetMarketplaceSearchResult().Items.Select(i => (MarketplaceAssetModel)i);
            var loader = new MarketplaceAssetDownloader(null, null);
            int deletedCount = 0;

            using (new CachingIsolator())
            using (var cach = new CachingIsolator(helper.GetTestActivities().ToArray()))
            {
                using (var cache = new ImplementationOfType(typeof(Caching)))
                {
                    cache.Register(() => Caching.Refresh()).Execute(() =>
                    {

                    });
                    using (var dir = new ImplementationOfType(typeof(Directory)))
                    {
                        dir.Register(() => Directory.Exists(Argument<string>.Any)).Execute(() =>
                        {
                            return true;
                        });

                        using (var utility = new ImplementationOfType(typeof(FileService)))
                        {
                            utility.Register(() => FileService.DeleteDirectory(Argument<string>.Any)).Execute(() =>
                            {
                                deletedCount++;
                            });

                            using (var file = new ImplementationOfType(typeof(File)))
                            {
                                file.Register(() => File.Exists(Argument<string>.Any)).Execute(() =>
                                {
                                    return true;
                                });
                                file.Register(() => File.Delete(Argument<string>.Any)).Execute(() =>
                                {
                                    deletedCount++;
                                });
                                loader.MarketpalceAssetsToDownload = new System.Collections.ObjectModel.ObservableCollection<MarketplaceAssetModel>(asset);
                                Assert.AreEqual(loader.ToDownloadCount, asset.Count());
                                loader.IsCancelDownload = true;
                                loader.CancelDownload();
                                Assert.AreEqual(Caching.ActivityAssemblyItems.Count, 0, "Failed to delete ActivityAssemblyItem in memory.");
                                Assert.AreEqual(Caching.ActivityItems.Count, 0, "Failed to delete ActivityItem in memory.");
                                Assert.AreEqual(loader.ToDownloadCount, deletedCount, "Falied to delete file in loaclmachine.");
                            }
                        }
                    }
                }
            }
        }

        [TestCleanup]
        public void TestCleanup() { System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); }
    }
}
