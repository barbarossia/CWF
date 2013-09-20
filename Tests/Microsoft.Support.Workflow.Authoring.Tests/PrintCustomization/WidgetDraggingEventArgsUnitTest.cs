using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.PrintCustomization;

namespace Microsoft.Support.Workflow.Authoring.Tests.PrintCustomization
{
    [TestClass]
    public class WidgetDraggingEventArgsUnitTest
    {
        [TestMethod()]
        [WorkItem(356285)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void WidgetDragging_EventArgsTest()
        {
            var point = new System.Windows.Point();
            WidgetDraggingEventArgs widgetDraggingEventArgs = new WidgetDraggingEventArgs(point);
            Assert.AreEqual(point, widgetDraggingEventArgs.RightBottom);
        }
    }
}
