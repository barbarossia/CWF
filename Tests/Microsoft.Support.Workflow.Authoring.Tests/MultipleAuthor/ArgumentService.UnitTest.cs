using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.CompositeActivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Support.Workflow.Authoring.Tests.MultipleAuthor
{
    [TestClass]
    public class ArgumentServiceTest
    {
        private WorkflowDesigner CreateWorkflowDesigner(Activity activity)
        {
            WorkflowDesigner wfDesigner = new WorkflowDesigner();

            ActivityBuilder activityBuilder = new ActivityBuilder();
            activityBuilder.Implementation = activity;

            wfDesigner.Load(activityBuilder);

            return wfDesigner;
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void TestGetAvailableOneVariable()
        {
            Activity at = new Sequence()
            {
                Activities = { new WriteLine() { Text = "Hello" } },
                Variables = { new Variable<string> { Name = "sepp", Default = "bla" } }
            };

            var wfDesigner = CreateWorkflowDesigner(at);
            var modelItem = ModelItemService.Find(wfDesigner.GetRoot(), (modelItemType) => typeof(Sequence).IsAssignableFrom(modelItemType)).First();
            var vars = ArgumentService.GetAvailableVariables(modelItem);

            Assert.AreEqual(1, vars.Count());
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void TestGetAvailableTwoVariable()
        {
            Activity at = new Sequence()
            {
                Activities = { new Sequence()
                {
                    Activities = { new WriteLine() { Text = "Hello" } },
                    Variables = { new Variable<string> { Name = "sepp2", Default = "bla" } }
                }},
                Variables = { new Variable<string> { Name = "sepp1", Default = "bla" } }
            };

            var wfDesigner = CreateWorkflowDesigner(at);
            var modelItem = ModelItemService.Find(wfDesigner.GetRoot(), (modelItemType) => typeof(WriteLine).IsAssignableFrom(modelItemType)).First();

            var vars = ArgumentService.GetAvailableVariables(modelItem);

            Assert.AreEqual(2, vars.Count());
        }

        [TestMethod]
        [Owner("v-toy")]
        [TestCategory("Unit-NoDif")]
        public void ArgumentService_GetExpressionTextTest()
        {
            string s = null;

            s = ArgumentService.GetExpressionText(null);
            Assert.AreEqual(string.Empty, s);
        }

    }
}
