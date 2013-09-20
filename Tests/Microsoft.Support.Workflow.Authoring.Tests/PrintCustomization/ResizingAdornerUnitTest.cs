using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.PrintCustomization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.DynamicImplementations;

namespace Microsoft.Support.Workflow.Authoring.Tests.PrintCustomization
{
    [TestClass]
    public class ResizingAdornerUnitTest
    {
        [WorkItem(356680)]
        [TestMethod]
        [TestCategory("unit")]
        [Owner("v-toy")]
        public void ResizingAdorner_ConstructorTest()
        {
            ResizingAdorner resizingAdorner = new ResizingAdorner(new FrameworkElement() { });
            
            PrivateObject privateInstance = new PrivateObject(resizingAdorner);
            VisualCollection visualChildren = privateInstance.GetField("visualChildren") as VisualCollection;
            Assert.IsTrue(visualChildren.Count == 4);
            
        }

        [WorkItem(357079)]
        [TestMethod]
        [TestCategory("unit")]
        [Owner("v-toy")]
        public void ResizingAdorner_HandleDragMethodTest()
        {
            var frameworkElement = new FrameworkElement() { Width=30, Height=30 };
            var canvas = new Canvas();
            canvas.Children.Add(frameworkElement);

            ResizingAdorner resizingAdorner = new ResizingAdorner(frameworkElement);
            var doWidgetDragging = false;

            resizingAdorner.WidgetDragging += ((s,e) =>
            {
                doWidgetDragging = true;
            });
            
            PrivateObject privateInstance = new PrivateObject(resizingAdorner);
            ResizingAdorner_Accessor accessor = new ResizingAdorner_Accessor(privateInstance);
            accessor.HandleDragMethod(new ResizingThumb(true, true) { }, 20, 20);
            Assert.IsTrue(frameworkElement.Height == 10);
            Assert.IsTrue(frameworkElement.Width == 10);

            accessor.HandleDragMethod(new ResizingThumb(false, false) { }, 20, 20);
            Assert.IsTrue(frameworkElement.Height == 30);
            Assert.IsTrue(frameworkElement.Width == 30);

            Assert.IsTrue(doWidgetDragging);
        }


        [WorkItem(356933)]
        [TestMethod]
        [TestCategory("unit")]
        [Owner("v-toy")]
        public void ResizingAdorner_HandleFinalSizeTest()
        {
            using (var frameworkElement = new Implementation<FrameworkElement>())
            {
                frameworkElement.Register(f => f.DesiredSize).Return(new Size(20,20));
                frameworkElement.Register(f => f.Height).Return(new Size(20, 20));
                frameworkElement.Register(f => f.Width).Return(new Size(20, 20));
                ResizingAdorner resizingAdorner = new ResizingAdorner(frameworkElement.Instance);
                PrivateObject privateInstance = new PrivateObject(resizingAdorner);
                ResizingAdorner_Accessor accessor = new ResizingAdorner_Accessor(privateInstance);
                var finalSize = accessor.HandleFinalSize(new Size(10, 10));          

                ResizingThumb topLeft = privateInstance.GetField("topLeft") as ResizingThumb;
                Assert.IsTrue(topLeft.Height == 5);
                Assert.IsTrue(topLeft.Width == 5);

                ResizingThumb topRight = privateInstance.GetField("topRight") as ResizingThumb;
                Assert.IsTrue(topRight.Height == 5);
                Assert.IsTrue(topRight.Width == 5);

                ResizingThumb bottomLeft = privateInstance.GetField("bottomLeft") as ResizingThumb;
                Assert.IsTrue(bottomLeft.Height == 5);
                Assert.IsTrue(bottomLeft.Width == 5);

                ResizingThumb bottomRight = privateInstance.GetField("bottomRight") as ResizingThumb;
                Assert.IsTrue(bottomRight.Height == 5);
                Assert.IsTrue(bottomRight.Width == 5);
            }


        }

        [WorkItem(356987)]
        [TestMethod]
        [TestCategory("unit")]
        [Owner("v-toy")]
        public void ResizingAdorner_EnforceSizeTest()
        {
            using (var canvas = new Implementation<Canvas>())
            {
                canvas.Register(c => c.ActualHeight).Return(100);
                canvas.Register(c => c.ActualWidth).Return(100);
                using (var frameworkElement = new Implementation<FrameworkElement>())
                {
                    frameworkElement.Register(f => f.DesiredSize).Return(new Size(20, 20));
          
                    canvas.Instance.Children.Add(frameworkElement.Instance);
                    ResizingAdorner resizingAdorner = new ResizingAdorner(frameworkElement.Instance);
                    resizingAdorner.EnforceSize();
                    Assert.AreEqual(frameworkElement.Instance.Height, frameworkElement.Instance.DesiredSize.Height);
                    Assert.AreEqual(frameworkElement.Instance.Width, frameworkElement.Instance.DesiredSize.Width);
                    Assert.AreEqual(canvas.Instance.ActualHeight, frameworkElement.Instance.MaxHeight);
                    Assert.AreEqual(canvas.Instance.ActualWidth, frameworkElement.Instance.MaxWidth);
                }
            }
            
        }

    }
}
