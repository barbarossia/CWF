using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Security;
using System.Security.Principal;

namespace Microsoft.Support.Workflow.Authoring.Tests
{
    [TestClass]
    public class StartUpUnitTest
    {
        [Ignore]
        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void StartUp_TestStartUp()
        {
            using (var service = new ImplementationOfType(typeof(AuthorizationService)))
            {
                // TODO: setup permissions
                Startup.Main();
            }
        }
    }
}
