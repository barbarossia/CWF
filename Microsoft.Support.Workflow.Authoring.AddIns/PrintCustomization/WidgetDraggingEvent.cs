using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.PrintCustomization
{
    /// <summary>
    /// Event arguments for WidgetDraggingEventHandler
    /// </summary>
    public class WidgetDraggingEventArgs : EventArgs
    {
        public Point RightBottom { get; private set; }

        public WidgetDraggingEventArgs(Point rightBottom)
        {
            RightBottom = rightBottom;
        }
    }

    /// <summary>
    /// Event handler for dragging the widget
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void WidgetDraggingEventHandler(object sender, WidgetDraggingEventArgs e);
}
