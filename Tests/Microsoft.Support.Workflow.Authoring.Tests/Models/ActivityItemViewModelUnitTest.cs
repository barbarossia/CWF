using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.DynamicImplementations;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Windows.Threading;

namespace Microsoft.Support.Workflow.Authoring.Tests
{


    /// <summary>
    ///This is a test class for ActivityItemViewModelTest and is intended
    ///to contain all ActivityItemViewModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ActivityItemViewModelUnitTest
    {
        private const string ANY_TEST_STRING = "any test string";

        [TestMethod()]
        [Description("Check to see if we can instantiate the target class correctly (Bug #86473)")]
        [Owner("v-richt")]
        [TestCategory("Unit-NoDif")]
        public void ActivityItem_ActivityItemViewModelConstructorTest()
        {
            ActivityItemViewModel target = new ActivityItemViewModel();
            Assert.IsNotNull(target, "An instance of ActivityItemViewModel could not be instantiated.");
        }

        [TestMethod()]
        [Description("Test to see if we can retrieve the (static, non database sourced) list of categories (Bug #86473)")]
        [Owner("v-richt")]
        [TestCategory("Unit-Dif")]
        [Ignore()]
        public void ActivityItem_CategoriesTest()
        {
            using (var impClient = new Implementation<WorkflowsQueryServiceClient>())
            {
                impClient.Register(inst => inst.ActivityCategoryGet(Argument<ActivityCategoryByNameGetRequestDC>.Any)).Execute(() =>
                {

                    var reply = new List<ActivityCategoryByNameGetReplyDC>();
                    reply.Add(new ActivityCategoryByNameGetReplyDC { Name = "Admin" });
                    reply.Add(new ActivityCategoryByNameGetReplyDC { Name = "tool" });
                    return reply;
                });
                impClient.Register(inst => inst.WorkflowTypeGet(Argument<WorkflowTypesGetRequestDC>.Any)).Execute(() =>
                {
                    WorkflowTypeGetReplyDC reply = new WorkflowTypeGetReplyDC();
                    reply.WorkflowActivityType = new List<WorkflowTypesGetBase>();
                    return reply;
                });

                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => impClient.Instance;
                
                Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
                {
                    ActivityItemViewModel target = new ActivityItemViewModel();
                    int itemCount = 0;
                    ObservableCollection<string> actual = target.Categories;
                    foreach (var col in actual)
                    {
                        itemCount++;
                    }
                    Assert.IsTrue(itemCount > 1, "No categories could be retrieved."); // this is from a static list and not loaded from a database - there should always be entries
                }));
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
            }
        }

        [TestMethod()]
        [Description("Test to see if SelectedCategory returns what it is set to (Bug #86473)")]
        [Owner("v-richt")]
        [TestCategory("Unit-NoDif")]
        public void ActivityItem_SelectedCategoryTest()
        {
            ActivityItemViewModel target = new ActivityItemViewModel();
            string expected = ANY_TEST_STRING;
            string actual;

            target.SelectedCategory = expected;
            actual = target.SelectedCategory;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [Description("Test to see if SelectedStatus returns what it is set to (Bug #86473)")]
        [Owner("v-richt")]
        [TestCategory("Unit-NoDif")]
        public void ActivityItem_SelectedStatusTest()
        {
            ActivityItemViewModel target = new ActivityItemViewModel();
            string expected = ANY_TEST_STRING;
            string actual;

            target.SelectedStatus = expected;
            actual = target.SelectedStatus;
            Assert.AreEqual(expected, actual);
        }

        [TestCleanup]
        public void TestCleanup() { System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); }
    }


}
