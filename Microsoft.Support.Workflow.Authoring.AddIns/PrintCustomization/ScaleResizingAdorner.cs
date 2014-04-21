using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Microsoft.Support.Workflow.Authoring.PrintCustomization
{
    /// <summary>
    /// Adorner for resizing with scaling
    /// </summary>
    public class ScaleResizingAdorner : ResizingAdorner
    {
        /// <summary>
        /// Current scale value
        /// </summary>
        public double Scale { get; private set; }

        protected override double adornedElementHeight
        {
            get
            {
                return base.adornedElement.ActualHeight;
            }
        }
        protected override double adornedElementWidth
        {
            get
            {
                return base.adornedElement.ActualWidth;
            }
        }

        /// <summary>
        /// Initialize the ScaleResizingAdorner
        /// </summary>
        /// <param name="adornedElement"></param>
        public ScaleResizingAdorner(FrameworkElement adornedElement)
            : base(adornedElement)
        {
            Scale = 1;
        }

        protected override void HandleDrag(ResizingThumb hitThumb, DragDeltaEventArgs args)
        {
            ExecuteHandleDrag(hitThumb, args.HorizontalChange, args.VerticalChange);
        }

        private void ExecuteHandleDrag(ResizingThumb hitThumb, double horizontalChange, double verticalChange)
        {
            double currentScale = Scale;
            double offsetX = hitThumb.IsLeft ? Math.Min(Canvas.GetLeft(adornedElement), -horizontalChange) : horizontalChange;
            double offsetY = hitThumb.IsTop ? Math.Min(Canvas.GetTop(adornedElement), -verticalChange) : verticalChange;
            double scaleX = (adornedElement.ActualWidth * Scale + offsetX) / adornedElement.ActualWidth;
            double scaleY = (adornedElement.ActualHeight * Scale + offsetY) / adornedElement.ActualHeight;
            Scale = Math.Min(scaleX, scaleY);
            Scale = Math.Max(0.5, Scale);

            if (hitThumb.IsLeft)
            {
                double move = (Scale - currentScale) * adornedElement.ActualWidth;
                Canvas.SetLeft(adornedElement, Canvas.GetLeft(adornedElement) - move);
            }
            if (hitThumb.IsTop)
            {
                double move = (Scale - currentScale) * adornedElement.ActualHeight;
                Canvas.SetTop(adornedElement, Canvas.GetTop(adornedElement) - move);
            }
            adornedElement.LayoutTransform = new ScaleTransform(Scale, Scale);

            OnWidgetDragging(new Point(
                Canvas.GetLeft(adornedElement) + adornedElement.ActualWidth * Scale,
                Canvas.GetTop(adornedElement) + adornedElement.ActualHeight * Scale));
        }
    }
}
