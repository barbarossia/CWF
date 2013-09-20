using System.Linq;
using Microsoft.Support.Workflow.Authoring;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Tests.Services;
using System.Collections.ObjectModel;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

namespace Authoring.Tests.Services
{
    [TestClass]
    public class ToolboxControlServiceUnitTests
    {
        [WorkItem(321640)]
        [Description("Verify that ToolboxControlService creates Basic/Advanced/User/Favorite toolbox controls")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void ToolboxControl_CreateToolboxes_Output_InDetail()
        {
            var lib3 = TestInputs.ActivityAssemblyItems.TestInput_Lib3;
            lib3.ActivityItems[0].IsUserFavorite = true;
            lib3.ActivityItems[0].Category = "My Category";
            var toolbox = ToolboxControlService.CreateToolboxes(
                new ObservableCollection<ActivityAssemblyItem>
                { 
                    TestInputs.ActivityAssemblyItems.TestInput_Lib1,
                    TestInputs.ActivityAssemblyItems.TestInput_Lib2,
                    lib3
                }, false);
            Assert.AreEqual(6, toolbox.Categories.Count);
            // Favorite controls, My Category, has only Activity3 in it
            Assert.AreEqual("Favorite",     toolbox.Categories[0].CategoryName);
            Assert.AreEqual(1,              toolbox.Categories[0].Tools.Count);
            Assert.AreEqual("Activity1",    toolbox.Categories[0].Tools.First().DisplayName);
            // User controls, My Category and OAS Basic Controls. 1 and 2.
            Assert.AreEqual("My Category",  toolbox.Categories[2].CategoryName);
            Assert.AreEqual(1,              toolbox.Categories[2].Tools.Count);
            Assert.IsNull(toolbox.Categories[1].CategoryName);
            Assert.AreEqual(2,              toolbox.Categories[1].Tools.Count);
            // Basic controls, Statements. 13 activities + PickBranch,FlowDecision,FlowSwitch from System.Activities, 2 activities from System.ServiceModel.
            Assert.AreEqual("Basic Logic",  toolbox.Categories[3].CategoryName);
            Assert.AreEqual(18,             toolbox.Categories[3].Tools.Count);
            // Advanced controls, 92 from System.Activities, 5 from System.ServiceModel.
            Assert.AreEqual("Multipe Author",     toolbox.Categories[4].CategoryName);
            Assert.AreEqual(1,             toolbox.Categories[4].Tools.Count);
        }
    }
}
