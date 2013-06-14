using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace Microsoft.Support.Workflow.Authoring.PrintCustomization
{
    public static class WidgetExtensions
    {
        public static bool IsActivity(this FrameworkElement element)
        {
            return element is Rectangle;
        }
    }
}
