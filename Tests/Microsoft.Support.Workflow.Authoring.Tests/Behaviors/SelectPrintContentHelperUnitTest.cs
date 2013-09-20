using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Activities.Presentation;
using System.Activities.Statements;
using Microsoft.Support.Workflow.Authoring.Behaviors;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.View;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.Tests.Behaviors
{
    [TestClass]
    public class SelectPrintContentHelperUnitTest
    {
        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void SelectPrintContentHelper_TestConstructor()
        {
            WorkflowDesigner designer = new WorkflowDesigner();
            Sequence sq = new Sequence()
            {
                Activities = { new WriteLine() { DisplayName = "w1" } },
            };
            designer.Load(sq);

            ActivityDesigner dd = new ActivityDesigner();
            SelectPrintContentHelper helper = new SelectPrintContentHelper(designer.View as FrameworkElement, 1.0, dd);
            PrivateObject po = new PrivateObject(helper);
            Assert.IsNotNull(po.GetFieldOrProperty("designerView"));
            Assert.IsNotNull(po.GetFieldOrProperty("zoomFactor"));
            Assert.IsNotNull(po.GetFieldOrProperty("rootActivityDesigner"));
        }
    }
}
