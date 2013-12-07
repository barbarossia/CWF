using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.PrintCustomization;
using Microsoft.Support.Workflow.Authoring.Models;

namespace Microsoft.Support.Workflow.Authoring.Tests.PrintCustomization
{
    [TestClass]
    public class PrintViewModeOnUIUnitTest
    {
        [TestMethod()]
        [WorkItem(356281)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void PrintViewModel_PrintViewModeOnUITest()
        {
            EnumOnUI<PrintViewMode> printViewModeOnUI = new EnumOnUI<PrintViewMode>(PrintViewMode.ActualSize);
            Assert.AreEqual(PrintViewMode.ActualSize, printViewModeOnUI.Value);
            Assert.AreEqual("Actual Size", printViewModeOnUI.DisplayName);

            printViewModeOnUI = new EnumOnUI<PrintViewMode>(PrintViewMode.FitToWindow);
            Assert.AreEqual(PrintViewMode.FitToWindow, printViewModeOnUI.Value);
            Assert.AreEqual("Fit To Window", printViewModeOnUI.DisplayName);
        }
    }
}
