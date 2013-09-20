using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Support.Workflow.Authoring.Behaviors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Support.Workflow.Authoring.Tests.Behaviors
{
    [TestClass]
    public class WindowClosingBehaviorUnitTest
    {
        [WorkItem(322682)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void WindowClosing_TestClosing()
        {
            DelegateCommand command = new DelegateCommand(() => { }, () => false);
            Window window = new Window(); 

            WindowClosingBehavior.SetClosing(window, null);
            Assert.AreEqual(null, WindowClosingBehavior.GetClosing(window));

            WindowClosingBehavior.SetClosing(window, command);
            Assert.AreEqual(command, WindowClosingBehavior.GetClosing(window));
            window.Close();
        }
    }
}
