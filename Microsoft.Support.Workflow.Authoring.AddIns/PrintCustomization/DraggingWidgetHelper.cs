using System;
using System.Activities.Presentation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Support.Workflow.Authoring.PrintCustomization
{
    /// <summary>
    /// Helper for dragging in print window
    /// </summary>
    public class DraggingWidgetHelper
    {
        private int zIndex = 1;
        private bool isMouseCaptured;
        private Point clickPoint;
        private AdornerLayer adornerLayer;
        private Dictionary<FrameworkElement, ResizingAdorner> elements;
        
        /// <summary>
        /// All elements and their right bottom corner positions
        /// </summary>
        public Dictionary<FrameworkElement, Point> ElementsRightBottom { get; private set; }
        /// <summary>
        /// The page container panel
        /// </summary>
        public Canvas Panel { get; private set; }

        /// <summary>
        /// Fires when element is being dragged
        /// </summary>
        public event EventHandler WidgetDragged;

        /// <summary>
        /// Initialize the dragging helper
        /// </summary>
        /// <param name="panel"></param>
        public DraggingWidgetHelper(Canvas panel)
        {
            Panel = panel;
            adornerLayer = AdornerLayer.GetAdornerLayer(panel);
            elements = new Dictionary<FrameworkElement, ResizingAdorner>();
            ElementsRightBottom = new Dictionary<FrameworkElement, Point>();
        }

        /// <summary>
        /// Add an element
        /// </summary>
        /// <param name="element"></param>
        public void AddWidget(FrameworkElement element)
        {
            element.Cursor = Cursors.SizeAll;
            element.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            element.AddHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(MouseLeftButtonUp), true);
            element.AddHandler(FrameworkElement.MouseMoveEvent, new MouseEventHandler(MouseMove), true);

            ResizingAdorner adorner = element.IsActivity() ? new ScaleResizingAdorner(element) : new ResizingAdorner(element);
            adorner.WidgetDragging += OnWidgetDragging;
            adornerLayer.Add(adorner);

            elements.Add(element, adorner);
            ElementsRightBottom.Add(element, new Point()); // element may has not loaded
        }

        /// <summary>
        /// Finalize the helper 
        /// </summary>
        public void Close()
        {
            List<FrameworkElement> eles = elements.Keys.ToList();
            foreach (FrameworkElement ele in eles)
            {
                elements[ele].WidgetDragging -= OnWidgetDragging;
                adornerLayer.Remove(elements[ele]);

                ele.Cursor = null;
                ele.RemoveHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown));
                ele.RemoveHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(MouseLeftButtonUp));
                ele.RemoveHandler(FrameworkElement.MouseMoveEvent, new MouseEventHandler(MouseMove));

                elements.Remove(ele);
                ElementsRightBottom.Remove(ele);
            }
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseLeftButtomDownMethod((FrameworkElement)sender, e.GetPosition((FrameworkElement)sender));
            e.Handled = true;
        }

        private void MouseLeftButtomDownMethod(FrameworkElement element, Point point)
        {
            isMouseCaptured = true;
            element.CaptureMouse();
            clickPoint = point;

            MakeFront(element);
        }

        private void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseLeftButtonUpMethod((FrameworkElement)sender);

            e.Handled = true;
        }

        private void MouseLeftButtonUpMethod(FrameworkElement element)
        {
            element.ReleaseMouseCapture();
            isMouseCaptured = false;

            double scale = 1;
            if (element.IsActivity())
            {
                scale = ((ScaleResizingAdorner)elements[element]).Scale;
            }

            OnWidgetDragging(element, new WidgetDraggingEventArgs(new Point(
                Canvas.GetLeft(element) + element.ActualWidth * scale, Canvas.GetTop(element) + element.ActualHeight * scale)));
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseCaptured)
            {
                MouseMoveMethod((FrameworkElement)sender, e.GetPosition(Panel));

                e.Handled = true;
            }
        }

        private void MouseMoveMethod(FrameworkElement element, Point currentPosition)
        {
            double scale = 1;
            if (element.IsActivity())
            {
                scale = ((ScaleResizingAdorner)elements[element]).Scale;
            }

            double left = (currentPosition.X - clickPoint.X * scale);
            double top = (currentPosition.Y - clickPoint.Y * scale);
            left = Math.Max(left, 0);
            top = Math.Max(top, 0);
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);
        }

        private void OnWidgetDragging(object sender, WidgetDraggingEventArgs e)
        {
            OnWidgetDraggingMethod((FrameworkElement)sender, e.RightBottom);
        }

        private void OnWidgetDraggingMethod(FrameworkElement element, Point rightBottom)
        {
            ElementsRightBottom[element] = rightBottom;
            MakeFront(element);

            if (WidgetDragged != null)
            {
                WidgetDragged(element, new EventArgs());
            }
        }

        private void MakeFront(FrameworkElement element)
        {
            int z = Canvas.GetZIndex(element);
            if (z < zIndex)
                Canvas.SetZIndex(element, ++zIndex);
        }
    }
}
