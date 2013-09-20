using System;
using System.Activities.Presentation.Toolbox;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.AddIns.Converters;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class ActivityAssemblyItemsToToolboxWrappersConverterUnitTest
    {
        [WorkItem(321677)]
        [TestMethod]
        [Description("Check if ActivityAssemblyItemsToToolboxWrappersConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        [Ignore]
        public void Aconvert_TestActivityAssemblyItemsToToolboxWrappersConverterConvert()
        {
            using (var ctxMock = new ImplementationOfType(typeof(SynchronizationContext)))
            {
                SynchronizationContext ctx = new SynchronizationContext();
                ctxMock.Register(() => SynchronizationContext.Current).Return(ctx);
                using (var tbMock = new ImplementationOfType(typeof(ToolboxControlService)))
                {
                    tbMock.Register(() => ToolboxControlService.CreateToolboxes(Argument<ObservableCollection<ActivityAssemblyItem>>.Any, Argument<bool>.Any))
                        .Execute<ObservableCollection<ActivityAssemblyItem>, ToolboxControl>((ObservableCollection<ActivityAssemblyItem> aais) =>
                        {
                            ToolboxControl tb = null;
                            Thread t = new Thread(() =>
                            {
                                tb = new ToolboxControl();
                                foreach (var i in aais)
                                {
                                    tb.Categories.Add(new ToolboxCategory(i.Name));
                                }
                            });
 
                            t.SetApartmentState(ApartmentState.STA);
                            t.Start();
                            t.Join();
                            return tb;
                        });
                    ObservableCollection<ActivityAssemblyItem> items = new ObservableCollection<ActivityAssemblyItem>()
                    {
                        Microsoft.Support.Workflow.Authoring.Tests.TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1,
                    };

                    ActivityAssemblyItemsToToolboxWrappersConverter converter = new ActivityAssemblyItemsToToolboxWrappersConverter();
                    object[] values = new object[] { items, false };
                    ObservableCollection<ToolboxControl> result = converter.Convert(
                        values, null, null, Thread.CurrentThread.CurrentCulture) as ObservableCollection<ToolboxControl>;
                    ToolboxControl expected = ToolboxControlService.CreateToolboxes(items, false);
                    Assert.AreEqual(1, result.Count);
                    Assert.AreEqual(expected.Categories.Count, result[0].Categories.Count);
                    for (int i = 0; i < expected.Categories.Count; i++)
                    {
                        Assert.AreEqual(expected.Categories[i].CategoryName, result[0].Categories[i].CategoryName);
                    }

                    items.Add(Microsoft.Support.Workflow.Authoring.Tests.TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib3);
                    expected = ToolboxControlService.CreateToolboxes(items, false);
                    Assert.AreEqual(1, result.Count);
                    Assert.AreEqual(1, result[0].Categories.Count);
                }
            }
        }

        [WorkItem(321678)]
        [TestMethod]
        [Description("Check ActivityAssemblyItemsToToolboxWrappersConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestActivityAssemblyItemsToToolboxWrappersConverterConvertBack()
        {
            ActivityAssemblyItemsToToolboxWrappersConverter converter = new ActivityAssemblyItemsToToolboxWrappersConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }

        [TestCleanup]
        public void TestCleanup() { System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); }
    }
}
