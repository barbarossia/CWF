using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.PrintCustomization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.DynamicImplementations;

namespace Microsoft.Support.Workflow.Authoring.Tests.PrintCustomization
{
    [TestClass]
    public class ScaleResizingAdornerUnitTest
    {
        [TestMethod()]
        [WorkItem(357063)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void ScaleResizingAdorner_ConstructorTest()
        {
            ScaleResizingAdorner scaleResizingAdorner = new ScaleResizingAdorner(new System.Windows.FrameworkElement() { });
            Assert.AreEqual(1, scaleResizingAdorner.Scale);
        }

        [TestMethod()]
        [WorkItem(357078)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void ScaleResizingAdorner_ExecuteHandleDragTest()
        {
            using (var canvas = new ImplementationOfType(typeof(Canvas)))
            {
                using (var frameworkElement = new Implementation<FrameworkElement>())
                {
                    canvas.Register(() => Canvas.GetLeft(Argument<UIElement>.Any)).Return(20);
                    canvas.Register(() => Canvas.GetTop(Argument<UIElement>.Any)).Return(20);

                    frameworkElement.Register(f => f.ActualHeight).Return(20);
                    frameworkElement.Register(f => f.ActualWidth).Return(20);
                    frameworkElement.Register(f => f.Width).Return(20);
                    frameworkElement.Register(f => f.Height).Return(20);

                    ScaleResizingAdorner scaleResizingAdorner = new ScaleResizingAdorner(frameworkElement.Instance);
                    Point point = new Point();
                    scaleResizingAdorner.WidgetDragging += ((s, e) =>
                    {
                        point = e.RightBottom;
                    });

                    PrivateObject privateInstance = new PrivateObject(scaleResizingAdorner);
                    ScaleResizingAdorner_Accessor accessor = new ScaleResizingAdorner_Accessor(privateInstance);
                    accessor.ExecuteHandleDrag(new ResizingThumb(true, true), 15, 15);
                    Assert.AreEqual(0.5, scaleResizingAdorner.Scale);

                    Assert.AreEqual(new Point(
                        Canvas.GetLeft(frameworkElement.Instance) + frameworkElement.Instance.ActualWidth * scaleResizingAdorner.Scale,
                        Canvas.GetTop(frameworkElement.Instance) + frameworkElement.Instance.ActualHeight * scaleResizingAdorner.Scale
                        ),
                        point);
                    
                    accessor.ExecuteHandleDrag(new ResizingThumb(false, false), 15, 15);
                    Assert.AreEqual(1.25, scaleResizingAdorner.Scale);

                }
                
            }
           
          
        }
    }
}
