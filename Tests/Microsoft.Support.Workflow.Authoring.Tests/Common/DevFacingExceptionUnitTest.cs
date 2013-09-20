using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.AddIns;

namespace Microsoft.Support.Workflow.Authoring.Tests.Common
{
    [TestClass]
    public class DevFacingExceptionUnitTest
    {
        [TestMethod]
        [Owner("v-toy")]
        [TestCategory("Unit-NoDif")]
        public void DevFacingExceptionIntTest()
        {
            DevFacingException devFacingException = new DevFacingException("TestDevFacingException");
            Assert.AreEqual("TestDevFacingException", devFacingException.Message);
        }
    }
}
