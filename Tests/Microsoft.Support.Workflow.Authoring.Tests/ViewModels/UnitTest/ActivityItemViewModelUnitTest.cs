using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels
{
    [TestClass]
   public class ActivityItemViewModelUnitTest
    {
        [WorkItem(322306)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void ActivityItem_PropertyChangedNotificationsAreRaised()
        {
            var vm = new ActivityItemViewModel();
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedStatus", () => vm.SelectedStatus = "public");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedCategory", () => vm.SelectedCategory = "admin");
        }
    }
}
