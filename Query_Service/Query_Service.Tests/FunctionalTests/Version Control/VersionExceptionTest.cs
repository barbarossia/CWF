using CWF.BAL.Versioning;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Query_Service.Tests
{


    /// <summary>
    ///This is a test class for VersionExceptionTest and is intended
    ///to contain all VersionExceptionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VersionExceptionTest
    {

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///A test for VersionException Constructor
        ///</summary>
        [TestMethod()]
        public void VersionExceptionConstructorTest()
        {
            string message = new string('A', 150);
            Rule rule = new Rule(RequestedOperation.Compile,
                                    false,
                                    true,
                                    RequiredChange.MustChange,
                                    RequiredChange.MustIncrement,
                                    RequiredChange.MustReset,
                                    RequiredChange.NoActionRequired,
                                    RequiredChange.MustChange);

            var target = new VersionException(message, rule);

            Assert.IsNotNull(target);

            Assert.AreEqual(message, target.Message);
            Assert.AreEqual(rule, target.Rule);
        }
    }
}
