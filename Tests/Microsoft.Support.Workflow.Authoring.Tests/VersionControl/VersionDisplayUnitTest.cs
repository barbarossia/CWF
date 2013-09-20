using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Common;

namespace Microsoft.Support.Workflow.Authoring.Tests.VersionControl
{
    [TestClass]
    public class VersionDisplayUnitTest
    {
        [WorkItem(322316)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void VersionDisplay_PropertyChangedNotificationsAreRaised()
        {
            var vm = new VersionDisplay();
            
            var versionFault = new CWF.WorkflowQueryService.Versioning.VersionFault();
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "VersionFault", () => vm.VersionFault = versionFault);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "HasMajorChanged", () => vm.HasMajorChanged = true);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Version", () => vm.Version = "1.0.0.1");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Caption", () => vm.Caption = "admin");

            Assert.AreEqual(vm.VersionFault, versionFault);
            Assert.AreEqual(vm.HasMajorChanged, true);
            Assert.AreEqual(vm.Version, "1.0.0.1");
            Assert.AreEqual(vm.Caption, "admin");
        }
    }
}
