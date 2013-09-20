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
    public class UserInfoPaneViewModelUnitTest
    {
        [WorkItem(322305)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void UserInfo_PropertyChangedNotificationsAreRaised()
        {
            var vm = new UserInfoPaneViewModel();
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "UserName", () => vm.UserName = "name");
            Assert.AreEqual(vm.UserName, "name");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "UserImage", () => vm.UserImage = null);
            Assert.AreEqual(vm.UserImage, null);
            //Assert.AreEqual(vm.EndpointFriendlyName, "PPE");
        }
    }
}
