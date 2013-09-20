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
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels
{
    [TestClass]
    public class ActivityAssemblyItemViewModelUnitTest
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [WorkItem(322310)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void ActivityAssembly_PropertyChangedNotificationsAreRaised()
        {
            MarketplaceDataHelper helper = new MarketplaceDataHelper(TestContext);
            ActivityAssemblyItem assembly = helper.GetTestActivities().FirstOrDefault();
            assembly.ActivityItems.ToList().ForEach(item => item.Tags = string.Empty);
            var vm = new ActivityAssemblyItemViewModel(assembly);
            Assert.AreEqual(vm.Source, assembly);
            Assert.IsTrue(vm.ActivityItems.Count == assembly.ActivityItems.Count);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "AuthorityGroup", () => vm.AuthorityGroup = "admin");
            Assert.AreEqual(vm.AuthorityGroup, "admin");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "IsReviewed", () => vm.IsReviewed = false);
            Assert.AreEqual(vm.IsReviewed, false);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "NotSafeForTypeLoad", () => vm.NotSafeForTypeLoad = false);
            Assert.AreEqual(vm.NotSafeForTypeLoad, false);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "CachingStatus", () => vm.CachingStatus = CachingStatus.Server);
            Assert.AreEqual(vm.CachingStatus, CachingStatus.Server);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Category", () => vm.Category = "toolbox");
            Assert.AreEqual(vm.Category, "toolbox");

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "CreationDateTime", () => vm.CreationDateTime=DateTime.MaxValue);
            Assert.AreEqual(vm.CreationDateTime, DateTime.MaxValue);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "CreatedBy", () => vm.CreatedBy = "admin");
            Assert.AreEqual(vm.CreatedBy, "admin");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Description", () => vm.Description = "admin");
            Assert.AreEqual(vm.Description, "admin");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "DisplayName", () => vm.DisplayName = "activity");
            Assert.AreEqual(vm.DisplayName, "activity");
            Assert.AreEqual(vm.FullName, assembly.AssemblyName.FullName);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Name", () => vm.Name = assembly.Name);
            Assert.AreEqual(vm.Name, assembly.Name);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Status", () => vm.Status ="public");
            Assert.AreEqual(vm.Status,"public");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Tags", () => vm.Tags = "public");
            Assert.AreEqual(vm.Tags, "public");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "UpdateDateTime", () => vm.UpdateDateTime = DateTime.MaxValue);
            Assert.AreEqual(vm.UpdateDateTime, DateTime.MaxValue);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "UpdatedBy", () => vm.UpdatedBy = "admin");
            Assert.AreEqual(vm.UpdatedBy, "admin");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "UserSelected", () => vm.UserSelected = true);
            Assert.AreEqual(vm.UserSelected, true);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "ReleaseNotes", () => vm.ReleaseNotes = "admin");
            Assert.AreEqual(vm.ReleaseNotes, "admin");

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Version", () => vm.Version = assembly.Version.ToString());
            Assert.AreEqual(vm.Version, assembly.Version.ToString());

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "AssemblyName", () => vm.AssemblyName = assembly.AssemblyName);
            Assert.AreEqual(vm.AssemblyName, assembly.AssemblyName);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Location", () => vm.Location = assembly.Location);
            Assert.AreEqual(vm.Location, assembly.Location);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "IsResolved", () => vm.IsResolved = true);
            Assert.AreEqual(vm.IsResolved, true);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "IsReadOnly", () => vm.IsReadOnly = false);
            Assert.AreEqual(vm.IsReadOnly, false);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "UserWantsToUpload", () => vm.UserWantsToUpload = true);
            Assert.AreEqual(vm.UserWantsToUpload, true);

            Assert.AreEqual(vm.ReferencedAssemblies.Count, assembly.ReferencedAssemblies.Count);

            Assert.IsTrue(vm.ReviewAssemblyCommand.CanExecute());
            vm.ReviewAssemblyCommand.Execute();
            Assert.IsTrue(vm.IsReviewed);
            vm.ToString();
            vm.Location = string.Empty;
            vm.ToString();

            try
            {
                vm.Version = "1111";
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual(string.Format("\"{0}\" is not a valid version number", "1111"), ex.ParamName);
            }
           
        }

        [TestCleanup]
        public void TestCleanup() { System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); }
    }
}
