using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using System.Collections.ObjectModel;
using System.Threading;
using System.Activities.Presentation.Toolbox;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Converters
{
    /// <summary>
    /// Part of the View layer, creates ToolboxControls wrapped by ToolboxWrappers.
    /// </summary>
    [ValueConversion(typeof(ObservableCollection<ActivityAssemblyItem>), typeof(ObservableCollection<ToolboxControl>))]
    public class ActivityAssemblyItemsToToolboxWrappersConverter : IMultiValueConverter
    {
        /// <summary>
        /// Convert an ObservableCollection of ActivityAssemblyItems into an ObservableCollection of ToolboxWrappers
        /// and link them so that inserting/deleting ActivityAssemblyItems regenerates the ToolboxWrapper collection.
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Contract.Requires(values[0] is ObservableCollection<ActivityAssemblyItem>);
            Contract.Requires(values[1] is bool);

            var items = (ObservableCollection<ActivityAssemblyItem>)values[0];
            bool isTask = (bool)values[1];
            var controls = new ObservableCollection<ToolboxControl>(new[] { ToolboxControlService.CreateToolboxes(items, isTask) });
            return controls;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
