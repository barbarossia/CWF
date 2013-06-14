using CWF.BAL.Versioning;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Query_Service.Tests
{


    /// <summary>
    ///This is a test class for RuleTest and is intended
    ///to contain all RuleTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RuleTest
    {



        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///A test for Rule Constructor
        ///</summary>
        [TestMethod()]
        public void RuleConstructorTest()
        {
            var newActionType = RequestedOperation.Compile;
            var newIsPublic = false;
            var newIsRetired = true;
            var newNameRequiredAction = RequiredChange.MustChange;
            var newMajorRequiredAction = RequiredChange.MustIncrement;
            var newMinorRequiredAction = RequiredChange.MustReset;
            var newBuildRequiredAction = RequiredChange.NoActionRequired;
            var newRevisionRequiredAction = RequiredChange.MustChange;

            Rule target = new Rule(newActionType,
                                                newIsPublic,
                                                newIsRetired,
                                                newNameRequiredAction,
                                                newMajorRequiredAction,
                                                newMinorRequiredAction,
                                                newBuildRequiredAction,
                                                newRevisionRequiredAction);

            Assert.IsTrue(target.IsPublic == newIsPublic);
            Assert.IsTrue(target.IsRetired == newIsRetired);
            Assert.IsTrue(target.NameRequiredChange == newNameRequiredAction);
            Assert.IsTrue(target.MajorRequiredChange == newMajorRequiredAction);
            Assert.IsTrue(target.MinorRequiredChange == newMinorRequiredAction);
            Assert.IsTrue(target.BuildRequiredChange == newBuildRequiredAction);
            Assert.IsTrue(target.BuildRequiredChange == newBuildRequiredAction);
            Assert.IsTrue(target.RevisionRequiredChange == newRevisionRequiredAction);
        }

    }
}
