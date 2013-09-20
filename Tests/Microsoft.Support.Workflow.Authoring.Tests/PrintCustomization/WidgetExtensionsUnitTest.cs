using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.PrintCustomization;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Microsoft.Support.Workflow.Authoring.Tests.PrintCustomization
{
    [TestClass]
    public class WidgetExtensionsUnitTest
    {
        [TestMethod()]
        [WorkItem(356279)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void WidgetExtensions_IsActivityTest()
        {
            var isActivity = WidgetExtensions.IsActivity(new Rectangle());
            Assert.AreEqual(true, isActivity);

            isActivity = WidgetExtensions.IsActivity(new Grid());
            Assert.AreEqual(false, isActivity);
        }
    }
}
