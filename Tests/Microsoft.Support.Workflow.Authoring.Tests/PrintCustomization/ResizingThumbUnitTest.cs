using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.PrintCustomization;

namespace Microsoft.Support.Workflow.Authoring.Tests.PrintCustomization
{
    [TestClass]
    public class ResizingThumbUnitTest
    {
        [TestMethod()]
        [WorkItem(356277)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void ResizingThumb_Test()
        {
            ResizingThumb resizingThumb = new ResizingThumb(true, true);
            Assert.AreEqual(true, resizingThumb.IsTop);
            Assert.AreEqual(true, resizingThumb.IsLeft);

            ResizingThumb resizingThumbFalse = new ResizingThumb(false, false);
            Assert.AreEqual(false, resizingThumbFalse.IsTop);
            Assert.AreEqual(false, resizingThumbFalse.IsLeft);
        }
    }
}
