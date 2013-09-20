using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.HelpClass;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels
{
    [TestClass]
    public class ReviewActivityViewModelUnitTest
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [WorkItem(322353)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void ReviewActivity_VerifyReviewActivityViewModel()
        {
            MarketplaceDataHelper helper = new MarketplaceDataHelper(TestContext);
            ActivityAssemblyItem assembly = helper.GetTestActivities().FirstOrDefault();
            ActivityAssemblyItemViewModel viewModel = new ActivityAssemblyItemViewModel(assembly);
            var vm = new ReviewActivityViewModel(viewModel);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedActivityItem", () => vm.SelectedActivityItem = null);
            Assert.AreEqual("Review Activities in " + assembly.Name, vm.Title);
            Assert.AreEqual(vm.ActivityAssemblyItem, viewModel);
            Assert.AreEqual(vm.SelectedActivityItem, viewModel.ActivityItems[0]);
            vm.ReviewAssemblyCommand.Execute();
            Assert.IsTrue(vm.ActivityAssemblyItem.IsReviewed);
        }
    }
}
