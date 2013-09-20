using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Tests.HelpClass;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.ViewModels.Marketplace;
using System.Collections.ObjectModel;
using Microsoft.DynamicImplementations;
using CWF.DataContracts.Marketplace;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels
{

    [TestClass]
    public class MarketplaceAssetDetailsViewModelUnitTest
    {
        private TestContext testContextInstance;
        private MarketplaceDataHelper helper;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            helper = new MarketplaceDataHelper(TestContext);
        }
        [WorkItem(322315)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void AssetDetails_PropertyChangedNotificationsAreRaised()
        {
            using (var imp = new Implementation<WorkflowsQueryServiceClient>())
            {
                imp.Register(inst => inst.GetMarketplaceAssetDetails(Argument<MarketplaceSearchDetail>.Any)).Return(null);

                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => imp.Instance;
                var vm = new MarketplaceAssetDetailsViewModel(null);
                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "IncludedActivities", () => vm.IncludedActivities = new ObservableCollection<ActivityQuickInfo>());
                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "IconUrl", () => vm.IconUrl = "");
                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "MarketplaceAssetFieldValues", () => vm.MarketplaceAssetFieldValues = new ObservableCollection<FieldValue>());
                TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "RightPaneTitle", () => vm.RightPaneTitle = "11111");
                TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
            }
        }

        [WorkItem(322343)]
        [Description("Verify when the AssetType is project,the included activities is empty")]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void AssetDetails_VerifyIncludedActivitiesCountIfProject()
        {
            using (var imp = new Implementation<WorkflowsQueryServiceClient>())
            {
                imp.Register(inst => inst.GetMarketplaceAssetDetails(Argument<MarketplaceSearchDetail>.Any)).Return(helper.Project_GetMarketplaceAssetDetails());
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => imp.Instance;
                var vm = new MarketplaceAssetDetailsViewModel(new MarketplaceAssetModel() { AssetType = AssetType.Project, });
                vm.SearchMarketplaceAssertDetails(imp.Instance);

                Assert.IsTrue(vm.IncludedActivities.Count == 0);
                Assert.IsFalse(vm.ActivitiesVisible);
                Assert.IsTrue(string.IsNullOrEmpty("") == string.IsNullOrEmpty(vm.RightPaneTitle), "MarketplaceDetails display included activities when seleted asset is project");
                TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
            }
        }

        [WorkItem(322347)]
        [Description("Verify the metadata collection when the AssetType is project")]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [TestMethod]
        public void AssetDetails_VerifyMetaDataIfProject()
        {
            using (var imp = new Implementation<WorkflowsQueryServiceClient>())
            {
                imp.Register(inst => inst.GetMarketplaceAssetDetails(Argument<MarketplaceSearchDetail>.Any)).Return(helper.Project_GetMarketplaceAssetDetails());
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => imp.Instance;
                var vm = new MarketplaceAssetDetailsViewModel(new MarketplaceAssetModel() { AssetType = AssetType.Project, });
                vm.SearchMarketplaceAssertDetails(imp.Instance);
                ObservableCollection<FieldValue> valuesExpected = new ObservableCollection<FieldValue> 
                { 
                    new FieldValue{Field="Type"},
                    new FieldValue{Field="Version"},
                    new FieldValue{Field="Description"},
                    new FieldValue{Field="Tags"},
                 };
                CollectionAssert.AreEqual(valuesExpected, vm.MarketplaceAssetFieldValues);
                TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
            }
        }

        [WorkItem(322344)]
        [Description("Verify the metadata collection when the AssetType is activities")]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [TestMethod]
        public void AssetDetails_VerifyMetaDataIfActivityIncludeActivities()
        {
            using (var imp = new Implementation<WorkflowsQueryServiceClient>())
            {
                imp.Register(inst => inst.GetMarketplaceAssetDetails(Argument<MarketplaceSearchDetail>.Any)).Return(helper.Activities_GetMarketplaceAssetDetailsIncludeActivityItems());
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => imp.Instance;
                var vm = new MarketplaceAssetDetailsViewModel(new MarketplaceAssetModel() { AssetType = AssetType.Activities, });
                vm.SearchMarketplaceAssertDetails(imp.Instance);
                ObservableCollection<FieldValue> valuesExpected = new ObservableCollection<FieldValue> 
                { 
                    new FieldValue{Field="Type"},
                    new FieldValue{Field="Version"},
                    new FieldValue{Field="Description"},
                    new FieldValue{Field="Tags"},
                    new FieldValue{Field="Category"},
                    new FieldValue{Field="Activities"},
                 };
                CollectionAssert.AreEqual(valuesExpected, vm.MarketplaceAssetFieldValues);
                TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
            }
        }

        [WorkItem(322346)]
        [Description("Verify the metadata collection when the AssetType is activities")]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [TestMethod]
        public void AssetDetails_VerifyMetaDataIfActivityWithNoActivities()
        {
            using (var imp = new Implementation<WorkflowsQueryServiceClient>())
            {
                var details = helper.Activities_GetMarketplaceAssetDetailsNoActivityItems();
                imp.Register(inst => inst.GetMarketplaceAssetDetails(Argument<MarketplaceSearchDetail>.Any))
                    .Return(details);
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => imp.Instance;
                var model = new MarketplaceAssetModel() { AssetType = AssetType.Activities, };
                var vm = new MarketplaceAssetDetailsViewModel(model);
                vm.SearchMarketplaceAssertDetails(imp.Instance);
                ObservableCollection<FieldValue> valuesExpected = new ObservableCollection<FieldValue> 
                { 
                    new FieldValue{Field="Type"},
                    new FieldValue{Field="Version"},
                    new FieldValue{Field="Description"},
                    new FieldValue{Field="Tags"},
                    new FieldValue{Field="Category"},
                    new FieldValue{Field="Activities"},
                 };
                Assert.AreEqual(vm.RightPaneTitle, "INCLUDED ACTIVITIES(0)");
                Assert.IsTrue(vm.MetaDataVisible);
                Assert.AreEqual(vm.IconUrl, details.ThumbnailUrl);
                Assert.AreEqual(vm.AssetName, details.Name);
                Assert.IsTrue(vm.IsActivityType);
                Assert.AreEqual(vm.SelectedMarketplaceAsset, model);
                CollectionAssert.AreEqual(valuesExpected, vm.MarketplaceAssetFieldValues);
                TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
            }
        }

        [TestCleanup]
        public void TestCleanup() { System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); }
    }
}
