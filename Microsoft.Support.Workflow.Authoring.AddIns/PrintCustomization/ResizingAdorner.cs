using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Support.Workflow.Authoring.PrintCustomization
{
    /// <summary>
    /// Adorner for resizing
    /// </summary>
    public class ResizingAdorner : Adorner
    {
        // To store and manage the adorner's visual children.
        private VisualCollection visualChildren;

        // Resizing adorner uses Thumbs for visual elements.  
        // The Thumbs have built-in mouse input handling.
        private ResizingThumb topLeft, topRight, bottomLeft, bottomRight;

        protected FrameworkElement adornedElement
        {
            get { return AdornedElement as FrameworkElement; }
        }

        protected virtual double adornedElementHeight
        {
            get { return adornedElement.DesiredSize.Height; }
        }

        protected virtual double adornedElementWidth
        {
            get { return adornedElement.DesiredSize.Width; }
        }

        // Override the VisualChildrenCount and GetVisualChild properties to interface with the adorner's visual collection.
        protected override int VisualChildrenCount
        {
            get { return visualChildren.Count; }
        }

        /// <summary>
        /// Event for dragging the adorner
        /// </summary>
        public event WidgetDraggingEventHandler WidgetDragging;

        /// <summary>
        /// Initialize the ResizingAdorner
        /// </summary>
        /// <param name="adornedElement"></param>
        public ResizingAdorner(FrameworkElement adornedElement)
            : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);

            // Call a helper method to initialize the Thumbs
            // with a customized cursors.
            topLeft = BuildAdornerCorner(true, true, Cursors.SizeNWSE);
            topRight = BuildAdornerCorner(true, false, Cursors.SizeNESW);
            bottomLeft = BuildAdornerCorner(false, true, Cursors.SizeNESW);
            bottomRight = BuildAdornerCorner(false, false, Cursors.SizeNWSE);
        }

        protected virtual void HandleDrag(ResizingThumb hitThumb, DragDeltaEventArgs args)
        {
            HandleDragMethod(hitThumb, args.HorizontalChange,args.VerticalChange);
        }

        private void HandleDragMethod(ResizingThumb hitThumb, double argsHorizontalChange, double argsVerticalChange)
        {
            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize();

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            if (hitThumb.IsLeft)
            {
                double widthCanChange = adornedElement.Width - hitThumb.DesiredSize.Width;
                double horizontalChange = Math.Min(argsHorizontalChange, widthCanChange);
                Canvas.SetLeft(adornedElement, Canvas.GetLeft(adornedElement) + horizontalChange);
                adornedElement.Width -= horizontalChange;
            }
            else
            {
                adornedElement.Width = Math.Max(adornedElement.Width + argsHorizontalChange, hitThumb.DesiredSize.Width);
            }

            if (hitThumb.IsTop)
            {
                double heightCanChange = adornedElement.Height - hitThumb.DesiredSize.Height;
                double verticalChange = Math.Min(argsVerticalChange, heightCanChange);
                Canvas.SetTop(adornedElement, Canvas.GetTop(adornedElement) + verticalChange);
                adornedElement.Height -= verticalChange;
            }
            else
            {
                adornedElement.Height = Math.Max(adornedElement.Height + argsVerticalChange, hitThumb.DesiredSize.Height);
            }

            OnWidgetDragging(new Point(
                Canvas.GetLeft(adornedElement) + adornedElement.Width,
                Canvas.GetTop(adornedElement) + adornedElement.Height));
        }

        public void OnWidgetDragging(Point rightBottom)
        {
            if (WidgetDragging != null)
            {
                WidgetDragging(adornedElement, new WidgetDraggingEventArgs(rightBottom));
            }
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            return HandleFinalSize(finalSize);
        }

        private Size HandleFinalSize(Size finalSize)
        {
            // desiredWidth and desiredHeight are the width and height of the element that's being adorned.  
            // These will be used to place the ResizingAdorner at the corners of the adorned element.  
            double desiredWidth = adornedElementWidth;
            double desiredHeight = adornedElementHeight;
            // adornerWidth & adornerHeight are used for placement as well.
            double adornerWidth = this.DesiredSize.Width;
            double adornerHeight = this.DesiredSize.Height;

            topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            topRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            bottomLeft.Arrange(new Rect(-adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            bottomRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));

            // Return the final size.
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return visualChildren[index];
        }

        // Handler for resizing from the top-left.
        private void HandleDrag(object sender, DragDeltaEventArgs args)
        {
            ResizingThumb hitThumb = sender as ResizingThumb;
            if (hitThumb == null)
                return;

            HandleDrag(hitThumb, args);
        }

        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        private ResizingThumb BuildAdornerCorner(bool isTop, bool isLeft, Cursor customizedCursor)
        {
            ResizingThumb cornerThumb = new ResizingThumb(isTop, isLeft);
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.BorderBrush = Brushes.Black;
            cornerThumb.Background = Brushes.White;
            cornerThumb.BorderThickness = new Thickness(1);
            cornerThumb.Height = cornerThumb.Width = 5;
            cornerThumb.DragDelta += HandleDrag;

            visualChildren.Add(cornerThumb);
            return cornerThumb;
        }

        // This method ensures that the Widths and Heights are initialized.  Sizing to content produces
        // Width and Height values of Double.NaN.  Because this Adorner explicitly resizes, the Width and Height
        // need to be set first.  It also sets the maximum size of the adorned element.
        public void EnforceSize()
        {
            if (adornedElement.Width.Equals(Double.NaN))
                adornedElement.Width = adornedElement.DesiredSize.Width;
            if (adornedElement.Height.Equals(Double.NaN))
                adornedElement.Height = adornedElement.DesiredSize.Height;

            FrameworkElement parent = adornedElement.Parent as FrameworkElement;
            if (parent != null)
            {
                adornedElement.MaxHeight = parent.ActualHeight;
                adornedElement.MaxWidth = parent.ActualWidth;
            }
        }
    }
}
