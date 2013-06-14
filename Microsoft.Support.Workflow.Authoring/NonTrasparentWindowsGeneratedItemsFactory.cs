using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls.Docking;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Navigation;
using System.Windows.Media;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring
{
    public class NonTrasparentWindowsGeneratedItemsFactory : DefaultGeneratedItemsFactory
    {
        public override ToolWindow CreateToolWindow()
        {
            var window = base.CreateToolWindow();

            RadWindowInteropHelper.SetAllowTransparency(window, false);
            RadWindowInteropHelper.SetClipMaskCornerRadius(window, new CornerRadius(3));
            RadWindowInteropHelper.SetOpaqueWindowBackground(window, Brushes.LightGray);

            return window;
        }
    }
}
