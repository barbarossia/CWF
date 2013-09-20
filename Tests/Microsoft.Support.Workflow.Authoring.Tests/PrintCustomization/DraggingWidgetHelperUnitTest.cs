using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.PrintCustomization;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.DynamicImplementations;
using System.Windows.Documents;

namespace Microsoft.Support.Workflow.Authoring.Tests.PrintCustomization
{
    /// <summary>
    ///This is a test class for DraggingWidgetHelper and is intended
    ///to contain all PrintCustomizationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DraggingWidgetHelperUnitTest
    {
        [TestMethod()]
        [WorkItem(356013)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void DraggingWidgetHelper_ConstructorTest()
        {
            Canvas panel = new Canvas();
            DraggingWidgetHelper target = new DraggingWidgetHelper(panel);
            Assert.IsNotNull(target.ElementsRightBottom);
        }

        [TestMethod()]
        [WorkItem(356030)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void DraggingWidgetHelper_AddWidgetAndCloseTest()
        {
            Window window = new Window();
            Canvas panel = new Canvas();
            window.Content = panel;
            AdornerLayer adornerLayer = null;
            window.Loaded += ((s, e) =>
            {
                adornerLayer = AdornerLayer.GetAdornerLayer(window.Content as Canvas);
                window.Close();
            });
            window.Show();

            using (var a = new ImplementationOfType(typeof(AdornerLayer)))
            {
                a.Register(() => AdornerLayer.GetAdornerLayer(Argument<Visual>.Any)).Return(adornerLayer);

                DraggingWidgetHelper target = new DraggingWidgetHelper(panel);

                target.AddWidget(panel);
                Assert.IsTrue(target.ElementsRightBottom.Count > 0);

                Rectangle rect = new Rectangle()
                {
                    Height = 10,
                    Width = 10,
                };
                target.AddWidget(rect);
                Assert.IsTrue(target.ElementsRightBottom.Count > 0);

                PrivateObject privateInstance = new PrivateObject(target);
                DraggingWidgetHelper_Accessor accesser = new DraggingWidgetHelper_Accessor(privateInstance);
                accesser.MouseMoveMethod(rect, new Point() { X = 10, Y = 10 });

                var doWidgetDragged = false;
                target.WidgetDragged += (s, e) =>
                {
                    doWidgetDragged = true;
                };

                accesser.MouseLeftButtonUpMethod(rect);
                Assert.IsTrue(doWidgetDragged);

                target.Close();
                Dictionary<FrameworkElement, ResizingAdorner> elements = privateInstance.GetField("elements") as Dictionary<FrameworkElement, ResizingAdorner>;
                Assert.IsTrue(elements.Count == 0);
            }
        }


        [TestMethod()]
        [WorkItem(357715)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void DraggingWidgetHelper_MouseLeftButtomDownMethodTest()
        {
            Window window = new Window();
            Canvas panel = new Canvas();
            window.Content = panel;

            DraggingWidgetHelper target = new DraggingWidgetHelper(panel);
            PrivateObject privateInstance = new PrivateObject(target);
            DraggingWidgetHelper_Accessor accesser = new DraggingWidgetHelper_Accessor(privateInstance);
            int zIndex = int.Parse(privateInstance.GetField("zIndex").ToString());
            accesser.MouseLeftButtomDownMethod(panel, new Point() { X = 10, Y = 10 });
            Assert.AreEqual(++zIndex, Canvas.GetZIndex(panel));
        }


        [TestMethod()]
        [WorkItem(357721)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void DraggingWidgetHelper_OnWidgetDraggingMethodTest()
        {
            var doWidgetDragged = false;
            Window window = new Window();
            Canvas panel = new Canvas();
            window.Content = panel;

            DraggingWidgetHelper target = new DraggingWidgetHelper(panel);
            target.WidgetDragged += (s, e) =>
            {
                doWidgetDragged = true;
            };
            PrivateObject privateInstance = new PrivateObject(target);
            DraggingWidgetHelper_Accessor accesser = new DraggingWidgetHelper_Accessor(privateInstance);
            accesser.OnWidgetDraggingMethod(panel, new Point() { X = 10, Y = 10 });
            Assert.IsTrue(doWidgetDragged);
        }


        [TestCleanup]
        public void TestCleanup()
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

    }
}
