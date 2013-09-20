using System;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using AuthoringToolTests.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.DataAccess;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Tests.HelpClass;
using Microsoft.DynamicImplementations;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels
{
    [TestClass]
    public class UploadAssemblyViewModelUnitTest
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [WorkItem(322314)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void UploadAssembly_PropertyChangedNotificationsAreRaised()
        {
            var vm = new UploadAssemblyViewModel();
            vm.DisplayActivityAssemblyItems = new System.Collections.ObjectModel.ObservableCollection<ActivityAssemblyItem>();
            vm.DisplayActivityAssemblyItems.Add(new ActivityAssemblyItem() { UserWantsToUpload = true });

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectAllAssemblies", () => vm.SelectAllAssemblies = false);
            Assert.AreEqual(vm.SelectAllAssemblies, false);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "DisplayActivityAssemblyItems", () => vm.DisplayActivityAssemblyItems = null);
            Assert.AreEqual(vm.DisplayActivityAssemblyItems, null);
        }

        [WorkItem(322371)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void UploadAssembly_VerifyUploadCommandCanExecute()
        {
            var vm = new UploadAssemblyViewModel();
            vm.SelectedLocation = Authoring.AddIns.Data.Env.Dev;
            vm.DisplayActivityAssemblyItems = new System.Collections.ObjectModel.ObservableCollection<ActivityAssemblyItem>();
            Assert.IsFalse(vm.UploadCommand.CanExecute());
            vm.DisplayActivityAssemblyItems.Add(new ActivityAssemblyItem() { UserWantsToUpload = true });
            Assert.IsTrue(vm.UploadCommand.CanExecute());
        }

        [WorkItem(322372)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void UploadAssembly_VerifyUploadCommandExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                MarketplaceDataHelper helper = new MarketplaceDataHelper(this.TestContext);
                var activities = helper.GetTestActivities().Take(1);
                bool isUpload = false;
                using (var service = new ImplementationOfType(typeof(MessageBoxService)))
                {
                    service.Register(() => MessageBoxService.NotifyUploadResult(Argument<string>.Any, Argument<bool>.Any))
                        .Execute(() =>
                        {
                            isUpload = true;
                        });
                    using (var client = new Implementation<WorkflowsQueryServiceClient>())
                    {
                        WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                        client.Register(inst => inst.GetMissingActivityLibraries(Argument<GetMissingActivityLibrariesRequest>.Any))
                            .Execute(() =>
                        {
                            GetMissingActivityLibrariesReply reply = new GetMissingActivityLibrariesReply();
                            reply.MissingActivityLibraries = new System.Collections.Generic.List<ActivityLibraryDC>();
                            return reply;
                        });

                        var vm = new UploadAssemblyViewModel();
                        vm.SelectedLocation = Authoring.AddIns.Data.Env.Dev;
                        vm.Initialize(activities);
                        vm.SelectAllAssemblies = true;

                        vm.UploadCommand.Execute();
                        Assert.IsTrue(isUpload);
                    }
                }
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
            });
        }

        [WorkItem(322370)]
        [Description("Verify dependency of selecting assembly are also automatically selected")]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod()]
        public void UploadAssembly_VerifyUploadAssemblywithDependency()
        {
            using (new CachingIsolator(
                            TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib3,
                            TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib2,
                            TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1))
            {

                ActivityAssemblyItem activityAssemblyItem3 = TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib3;
                ActivityAssemblyItem activityAssemblyItem2 = TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib2;
                ActivityAssemblyItem activityAssemblyItem1 = TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1;

                var vmUploadAssembly = new UploadAssemblyViewModel();
                vmUploadAssembly.Initialize(new[] { activityAssemblyItem3, activityAssemblyItem2, activityAssemblyItem1 });

                activityAssemblyItem3.UserWantsToUpload = true;
                vmUploadAssembly.NotifyUploadAssemblyItemChange(activityAssemblyItem3, true);

                var selected = from assemblies in vmUploadAssembly.DisplayActivityAssemblyItems
                               where assemblies.UserWantsToUpload == true
                               select assemblies;

                Assert.AreEqual(3, selected.Count());

                Assert.IsTrue(activityAssemblyItem2.UserWantsToUpload);
                Assert.IsTrue(activityAssemblyItem1.UserWantsToUpload);

                activityAssemblyItem1.UserWantsToUpload = false;
                vmUploadAssembly.NotifyUploadAssemblyItemChange(activityAssemblyItem1, false);

                Assert.IsFalse(activityAssemblyItem2.UserWantsToUpload);
                Assert.IsFalse(activityAssemblyItem3.UserWantsToUpload);
            }
        }
    }
}
