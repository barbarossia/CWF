using CWF.BAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CWF.BAL.Versioning;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using CWF.WorkflowQueryService.Versioning;


namespace Query_Service.Tests
{
    /// <summary>
    ///This is a test class for VersionFaultTest and is intended
    ///to contain all VersionFaultTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VersionFaultTest
    {

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///A test for VersionFault Constructor
        ///</summary>
        [TestMethod()]
        public void VersionFaultConstructorTest()
        {
            var target = new VersionFault();

            Assert.IsTrue(null != target);
        }

        /// <summary>
        ///A test for Message
        ///</summary>
        [TestMethod()]
        public void MessageTest()
        {
            VersionFault target = new VersionFault();

            string expected = new String('A', 150);
            string actual;
            target.Message = expected;
            actual = target.Message;

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Rule
        ///</summary>
        [TestMethod()]
        public void RuleTest()
        {
            VersionFault target = new VersionFault(); 
            Rule expected = new Rule(RequestedOperation.Compile,
                                     false,
                                     true,
                                     RequiredChange.MustChange,
                                     RequiredChange.MustIncrement,
                                     RequiredChange.MustReset,
                                     RequiredChange.NoActionRequired,
                                     RequiredChange.MustChange);

            Rule actual;

            target.Rule = expected;
            actual = target.Rule;

            Assert.AreEqual(expected, actual);
        }


    }
}
