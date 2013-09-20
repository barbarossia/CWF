using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels
{

    [TestClass]
    public class VersionDisplayViewModelUnitTest
    {
        [WorkItem(322311)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void VersionDisplay_PropertyChangedNotificationsAreRaised()
        {
            var vm = new VersionDisplayViewModel();
            var versionFault =  new CWF.WorkflowQueryService.Versioning.VersionFault();
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "VersionFault", () => vm.VersionFault = versionFault);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "HasMajorChanged", () => vm.HasMajorChanged = true);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "ChangeMajorCanExecute", () => vm.ChangeMajorCanExecute = true);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Major", () => vm.Major = 2);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Minor", () => vm.Minor = 1);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Build", () => vm.Build = 1);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Revision", () => vm.Revision = 2);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Caption", () => vm.Caption = "admin");

            Assert.AreEqual(vm.MajorContent, "-");
            Assert.AreEqual(vm.VersionFault ,versionFault);
            Assert.AreEqual(vm.HasMajorChanged , true);
            Assert.AreEqual(vm.ChangeMajorCanExecute, true);
            Assert.AreEqual(vm.Major, 2);
            Assert.AreEqual(vm.Build, 1);
            Assert.AreEqual(vm.Minor,1);
            Assert.AreEqual(vm.Revision, 2);
            Assert.AreEqual(vm.Caption, "admin");
            Assert.AreEqual(vm.Version, "2.1.1.2");
            vm.ChangeMajorCommand.Execute();
            Assert.AreEqual(vm.Major,1);

            vm.Version = string.Empty;
            Assert.IsTrue(vm.Major == 0);

            PrivateObject po = new PrivateObject(vm);
            vm.HasMajorChanged = false;
            vm.Major = 2;
            po.Invoke("ChangeMajorCommandCommandExecute");
            Assert.AreEqual(vm.Major, 3);

        }
    }
}
