using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.PrintCustomization;

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
            PrintViewModeOnUI printViewModeOnUI = new PrintViewModeOnUI(PrintViewMode.ActualSize);
            Assert.AreEqual(PrintViewMode.ActualSize, printViewModeOnUI.ViewMode);
            Assert.AreEqual("Actual Size", printViewModeOnUI.DisplayName);

            printViewModeOnUI = new PrintViewModeOnUI(PrintViewMode.FitToWindow);
            Assert.AreEqual(PrintViewMode.FitToWindow, printViewModeOnUI.ViewMode);
            Assert.AreEqual("Fit To Window", printViewModeOnUI.DisplayName);
        }
    }
}
