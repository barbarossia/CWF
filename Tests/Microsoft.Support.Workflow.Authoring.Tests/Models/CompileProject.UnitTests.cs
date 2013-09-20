using System.Activities.Statements;
using System.IO;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Activities.Presentation.Services;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Microsoft.Support.Workflow.Authoring.Tests.Models
{
    /// <summary>
    ///This is a test class for CompileProjectTest and is intended
    ///to contain all CompileProjectTest Unit Tests
    ///</summary>
    [TestClass]
    public class CompileProjectUnitTest
    {

        /// <summary>
        ///A test for CompileProject Constructor
        ///</summary>
        [WorkItem(262588)]
        [TestMethod]
        [TestCategory("Unit-NoDif")]
        public void CompileProject_CompileProjectConstructorTest()
        {
            const string name = "test";
            const string version = "1.0.0";
            const string wrongVersion = "Test";
            const string xaml = "test";

            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                        new CompileProject(null, version, xaml));
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                        new CompileProject(name, null, xaml));
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                        new CompileProject(name, version, null));
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                       new CompileProject(name, wrongVersion, xaml));

            Assert.IsNotNull(new CompileProject(name, version, xaml));
            Assert.IsNotNull(new CompileProject());
        }

        [TestCleanup]
        public void TestCleanup() { System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); }
    }
}
