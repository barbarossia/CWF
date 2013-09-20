using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Activities.Statements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests;
using System.Activities.Presentation.Model;
using Microsoft.DynamicImplementations;

using System.Reflection;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.Behaviors;

namespace Authoring.Tests.Unit
{
    [TestClass]
    public class WorkflowItemUnitTests
    {
        [WorkItem(321633)]
        [Description("Test that updating XamlCode and calling RefreshDesignerFromXamlCode updates IsSavedToServer, IsDataDirty, and IsValid")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        public void WorkflowItem_RefreshDesignerFromXamlCode_UpdatesDependentFields()
        {
            var wf = new WorkflowItem("MyWF", "myWF", new Sequence().ToXaml(), string.Empty);
            wf.IsSavedToServer = true;
            wf.IsDataDirty = false;
            Assert.IsTrue(wf.IsValid); // can't set it so we just verify that it's in the expected state before we start the test

            wf.XamlCode = new Assign().ToString(); 
            Assert.AreEqual(true, wf.IsSavedToServer);
            Assert.AreEqual(false, wf.IsDataDirty);
            Assert.AreEqual(false, wf.IsValid);
        }

        [WorkItem(348266)]
        [TestCategory("Unit-NoDif")]
        [Owner("v-jillhu")]
        [TestMethod]
        public void WorkflowItem_ActivityItem()
        {
            var ai = new ActivityItem();
            TestUtilities.Assert_ShouldRaiseINPCNotification(ai, "IsReviewed", () => ai.IsReviewed = true);
            TestUtilities.Assert_ShouldRaiseINPCNotification(ai, "IsSwitch", () => ai.IsSwitch = true);
            TestUtilities.Assert_ShouldRaiseINPCNotification(ai, "IsUserInteraction", () => ai.IsUserInteraction = true);

            //test of Matchs in ActivityItem
            Assert.IsFalse(ai.Matchs(null));
            AssemblyName myassemblyName = new AssemblyName("TestAssembly,Version=1.0.0.2001,Culture=en-US,PublicKeyToken=null");
            ai.Name = "TestAssembly";
            ai.Version = "1.0.0.2001";
            Assert.IsTrue(ai.Matchs(myassemblyName));
            ai.Version = null;
            Assert.IsFalse(ai.Matchs(myassemblyName));

            //test of compareto
            Assert.IsTrue(ai.CompareTo(null) == -1);

            //test of GetActivityItemsFromAssembly
            TestUtilities.Assert_ShouldThrow<ArgumentNullException>(() => ActivityItem.GetActivityItemsFromAssembly(null));
            TestUtilities.Assert_ShouldThrow<ArgumentNullException>(() => ActivityItem.GetActivityItemsFromAssembly(new ActivityAssemblyItem()));
        }

        [WorkItem(348272)]
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void WorkflowItem_SetDatabaseReferences()
        {
            var ai = new ActivityAssemblyItem();
            TestUtilities.Assert_ShouldThrow<ArgumentNullException>(() => ai.SetDatabaseReferences(null));
            AssemblyName testAssembly1 = new AssemblyName("TestAssembly1,Version=1.0.0.2001,Culture=en-US,PublicKeyToken=null");
            AssemblyName testAssembly2 = new AssemblyName("TestAssembly2,Version=1.0.0.2001,Culture=en-US,PublicKeyToken=null");
            AssemblyName testnull = null;
            List<AssemblyName> assemblyList = new List<AssemblyName>();
            assemblyList.Add(testnull);
            TestUtilities.Assert_ShouldThrow<ArgumentNullException>(() => ai.SetDatabaseReferences(assemblyList));
            assemblyList.Remove(null);
            assemblyList.Add(testAssembly1);
            assemblyList.Add(testAssembly2);
            ai.SetDatabaseReferences(assemblyList);
            Assert.IsTrue(ai.ReferencedAssemblies.Any<AssemblyName>());
        }

        [WorkItem(348264)]
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void WorkflowItem_ActivityAssemblyItemMatches()
        {

            var ai = new ActivityAssemblyItem();

            TestUtilities.Assert_ShouldRaiseINPCNotification(ai, "FriendlyName", () => ai.FriendlyName = "FriendlyName");

            ActivityItem otherObject = null;
            Assert.IsFalse(ai.Matches(otherObject));
            otherObject = new ActivityItem();
            Assert.IsFalse(ai.Matches(otherObject));
            ai.AssemblyName = new AssemblyName("TestAssembly,Version=1.0.0.2001,Culture=en-US,PublicKeyToken=5795ec30fe62a48b");
            otherObject.Name = "TestAssembly";
            otherObject.Version = "1.0.0.2001";
            Assert.IsTrue(ai.Matches(otherObject));
        }

        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void WorkflowItem_PropertiesTest()
        {
            var wf = new WorkflowItem("MyWF", "myWF", new Sequence().ToXaml(), string.Empty);
            TestUtilities.Assert_ShouldRaiseINPCNotification(wf, "IsLoadingDesigner",()=>wf.IsLoadingDesigner=false);
            TestUtilities.Assert_ShouldRaiseINPCNotification(wf, "HasMajorChanged", () => wf.HasMajorChanged = false);
            TestUtilities.Assert_ShouldRaiseINPCNotification(wf, "PrintState", () => wf.PrintState = PrintAction.NoneAction);
        }

        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void aaWorkflowItem_InitializeWorkflowDesignerTest()
        {
            var wf = new WorkflowItem("MyWF", "myWF", new Sequence().ToXaml(), string.Empty);
            wf.InitializeWorkflowDesigner();            
            Assert.IsTrue(wf.IsDataDirty);
        }
    }
}
