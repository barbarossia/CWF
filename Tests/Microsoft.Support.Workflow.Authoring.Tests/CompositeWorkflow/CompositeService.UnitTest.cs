using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.CompositeActivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.AddIns.CompositeActivity;

namespace Microsoft.Support.Workflow.Authoring.Tests.CompositeWorkflow
{
    [TestClass]
    public class CompositeServiceUnitTest
    {
        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void TestNonGetParents()
        {
            Activity at = new Sequence()
            {
                Activities = { new TaskActivity() }

            };

            var wfDesigner = TestUtilities.CreateWorkflowDesigner(at);
            var modelItem = ModelItemService.Find(wfDesigner.GetRoot(), (modelItemType) => typeof(TaskActivity).IsAssignableFrom(modelItemType)).First();
            var parents = CompositeService.GetParents(modelItem);

        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void TestOneGetParents()
        {
            Activity at = new Sequence()
            {
                Activities = { new TaskActivity() { TaskBody = new TaskActivity() } }

            };

            var wfDesigner = TestUtilities.CreateWorkflowDesigner(at);
            var modelItem = ModelItemService.Find(wfDesigner.GetRoot(), (modelItemType) => typeof(TaskActivity).IsAssignableFrom(modelItemType)).Last();
            var parents = CompositeService.GetParents(modelItem);

        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void TestOneMoreGetParents()
        {
            Activity at = new Sequence()
            {
                Activities = 
                { 
                    new TaskActivity() 
                    { 
                        TaskBody = new Sequence()
                        {
                            Activities = { new TaskActivity()},
                        }
                    } 
                }
            };

            var wfDesigner = TestUtilities.CreateWorkflowDesigner(at);
            var modelItem = ModelItemService.Find(wfDesigner.GetRoot(), (modelItemType) => typeof(TaskActivity).IsAssignableFrom(modelItemType)).Last();
            var parents = CompositeService.GetParents(modelItem);

        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void TestGetActivitiesFalse()
        {
            Activity at = new Sequence()
            {
                Activities = 
                { 
                    new While() {},
                }
            };

            Assert.IsFalse(CompositeService.GetActivities(at).Any(c => c is TaskActivity));
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void TestGetActivitiesTrue()
        {
            Activity at = new Sequence()
            {
                Activities = 
                { 
                    new Sequence() {Activities = { new TaskActivity()}},
                }
            };

            Assert.IsFalse(CompositeService.GetActivities(at).Any(c => c is TaskActivity));
        }
    }
}
