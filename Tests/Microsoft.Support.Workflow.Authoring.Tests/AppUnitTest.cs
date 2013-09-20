using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Security;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Security.Policy;

namespace Microsoft.Support.Workflow.Authoring.Tests
{
    [TestClass]
    public class AppUnitTest
    {
        [Ignore]
        [WorkItem(322897)]
        [TestMethod]
        [Description("Check App starts with no access.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void App_TestStartup()
        {
            MessageBoxService.ShowFunc = ((msg, caption, button, image, result) =>
            {
                Assert.AreEqual("Your network account is not authorized to run the Common Workflow Foundry. " + Environment.NewLine + "You can request access by sending an email to .", msg);
                return MessageBoxResult.OK;
            });

            App app = new App();
            app.Run();
        }
    }
}
