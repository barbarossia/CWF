using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using System.Activities.Presentation;

namespace Microsoft.Support.Workflow.Authoring.Tests
{
    
    
    /// <summary>
    ///This is a test class for DependencyAnalysisServiceTest and is intended
    ///to contain all DependencyAnalysisServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyAnalysisServiceTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for DependencyAnalysisService Constructor
        ///</summary>
        [TestMethod()]
        public void DependencyAnalysisServiceConstructorTest()
        {
            DependencyAnalysisService target = new DependencyAnalysisService();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AddReferencesInApplicationDomain
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.AddIns.dll")]
        public void AddReferencesInApplicationDomainTest()
        {
            HashSet<Assembly> referencedAssemblies = null; // TODO: Initialize to an appropriate value
            DependencyAnalysisService_Accessor.AddReferencesInApplicationDomain(referencedAssemblies);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CheckGenericBaseAndArgumentTypes
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.AddIns.dll")]
        public void CheckGenericBaseAndArgumentTypesTest()
        {
            Type item = null; // TODO: Initialize to an appropriate value
            IEnumerable<Type> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<Type> actual;
            actual = DependencyAnalysisService_Accessor.CheckGenericBaseAndArgumentTypes(item);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetBaseAndArgumentTypes
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.AddIns.dll")]
        public void GetBaseAndArgumentTypesTest()
        {
            HashSet<Type> referencedTypes = null; // TODO: Initialize to an appropriate value
            IEnumerable<Type> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<Type> actual;
            actual = DependencyAnalysisService_Accessor.GetBaseAndArgumentTypes(referencedTypes);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCompileProject
        ///</summary>
        [TestMethod()]
        public void GetCompileProjectTest()
        {
            WorkflowEditorViewModel focusedWorkflowItem = null; // TODO: Initialize to an appropriate value
            CompileProject expected = null; // TODO: Initialize to an appropriate value
            CompileProject actual;
            actual = DependencyAnalysisService.GetCompileProject(focusedWorkflowItem);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCompileProject
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.AddIns.dll")]
        public void GetCompileProjectTest1()
        {
            WorkflowDesigner disigner = null; // TODO: Initialize to an appropriate value
            CompileProject expected = null; // TODO: Initialize to an appropriate value
            CompileProject actual;
            actual = DependencyAnalysisService_Accessor.GetCompileProject(disigner);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetReferencedAssemblies
        ///</summary>
        [TestMethod()]
        public void GetReferencedAssembliesTest()
        {
            WorkflowDesigner workflowDesigner = null; // TODO: Initialize to an appropriate value
            IEnumerable<Type> referencedTypes = null; // TODO: Initialize to an appropriate value
            HashSet<Assembly> expected = null; // TODO: Initialize to an appropriate value
            HashSet<Assembly> actual;
            actual = DependencyAnalysisService.GetReferencedAssemblies(workflowDesigner, referencedTypes);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetReferencedTypes
        ///</summary>
        [TestMethod()]
        public void GetReferencedTypesTest()
        {
            WorkflowDesigner workflowDesigner = null; // TODO: Initialize to an appropriate value
            HashSet<Type> expected = null; // TODO: Initialize to an appropriate value
            HashSet<Type> actual;
            actual = DependencyAnalysisService.GetReferencedTypes(workflowDesigner);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
