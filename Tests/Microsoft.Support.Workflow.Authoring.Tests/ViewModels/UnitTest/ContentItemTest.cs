using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Support.Workflow.Authoring.Common; 
using System.Reflection;
using System.Collections.Generic;

namespace Microsoft.Support.Workflow.Authoring.Tests
{
    
    
    /// <summary>
    ///This is a test class for ContentItemTest and is intended
    ///to contain all ContentItemTest Unit Tests
    ///</summary>
    [TestClass]
    public class ContentItemUnitTest
    {
        /// <summary>
        ///A test for ContentItem Constructor
        ///</summary>
        [TestMethod()]
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        public void ContentItem_ConstructorTest()
        {
            string text = "contentItem"; // TODO: Initialize to an appropriate value
            ContentItem target = new ContentItem(text);
            Assert.AreEqual(text,target.Value);
        }
    }

    [TestClass]
    public class WorkflowTemplateItemUnitTest
    {
        /// <summary>
        ///A test for  WorkflowTemplateItem
        ///</summary>
        [TestMethod()]
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        public void WorkflowTemplateItem_ToStringTest()
        {
            WorkflowTemplateItem target = new WorkflowTemplateItem(10, "Target workflow");            
            Assert.AreEqual("Target workflow",target.ToString());
        }

        [TestMethod]
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        public void WorkflowTemplateItem_ConstructorTest()
        {
            TestUtilities.Assert_ShouldThrow<ArgumentNullException>(() =>
                {
                    WorkflowTemplateItem target = new WorkflowTemplateItem(10, null);
                });
        }
    }

    [TestClass]
    public class CachingChangedEventArgsUnitTest 
    {
        [TestMethod]
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        public void CachingChangedEventArgs_CtorTest()
        {
            List<AssemblyName> assemblies=new List<AssemblyName>();
            CachingChangedEventArgs target=new CachingChangedEventArgs(assemblies);
            Assert.IsNotNull(target.Assemblies);
        }
    }
}
