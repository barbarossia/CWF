using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using Microsoft.Support.Workflow.Authoring.Tests;
using System.Activities.Presentation.Toolbox;
using Microsoft.Support.Workflow.Authoring.AddIns.Converters;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Authoring.Tests.Services
{
    [TestClass]
    public class ActivityAssemblyItemsToToolboxWrappersConverterUnitTests
    {
        [WorkItem(321639)]
        [Description("Verify that the ObservableCollection created by ActivityAssemblyItemsToToolboxWrappersConverter is linked to input ObservableCollection for updates")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void Aconvert_OutputCollectionEventuallyUpdatedWhenInputCollectionIsUpdated()
        {
            // We have to do some complicated setup to test the asynchronous behavior on an STA 
            // thread with a Dispatcher event loop running. 

            ObservableCollection<ToolboxControl> controls = null; // need handle external to arrangeAct() so we can use it in assert() later
            Action arrangeAct = () =>
                {
                    // Arrange                   
                    var asms = new ObservableCollection<ActivityAssemblyItem>(new[] { TestInputs.ActivityAssemblyItems.TestInput_Lib1 });                    
                    object[] values = new object[]{asms, false};
                    var converter = new ActivityAssemblyItemsToToolboxWrappersConverter();
                    controls = (ObservableCollection<ToolboxControl>)converter.Convert(values, null, null, null);

                    // There should be one User control before the update
                    Assert.AreEqual("OAS Basic Controls", controls[0].Categories[1].CategoryName);
                    Assert.AreEqual(1, controls[0].Categories[1].Tools.Count);

                    // Act
                    asms.Add(TestInputs.ActivityAssemblyItems.TestInput_Lib2);
                };
            Action assert = () =>
                {
                    // There should be two User controls after the update
                    Assert.AreEqual("OAS Basic Controls", controls[0].Categories[1].CategoryName);
                    Assert.AreEqual(2, controls[0].Categories[1].Tools.Count);
                };
            TestUtilities.Execute_Sequentially_On_Dispatcher(arrangeAct, assert);
        }
    }
}