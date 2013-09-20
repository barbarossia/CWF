using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Behaviors;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using Microsoft.DynamicImplementations;

namespace Microsoft.Support.Workflow.Authoring.Tests.Behaviors
{
    [TestClass]
    public class ZoomFactorHelperUnitTest
    {
        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void ZoomFactorHelper_TestConstructor()
        {
            using (var print = new ImplementationOfType(typeof(SelectPrintContentHelper)))
            {
                Grid root = new Grid();
                StatusBar bar = new StatusBar();
                Slider slider = new Slider();
                slider.Value = 3;
                bar.Items.Add(slider);

                root.Children.Add(bar);

                print.Register(() => SelectPrintContentHelper.SearchDependencyObject(bar, typeof(Slider)))
                    .Return(slider);
                
                ZoomFactorHelper help = new ZoomFactorHelper(root);
                Assert.IsTrue(help.ZoomFactor != 0);
            }
        }
    }
}
