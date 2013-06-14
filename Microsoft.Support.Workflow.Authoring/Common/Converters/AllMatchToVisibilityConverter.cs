
using System;
using System.Windows;
using System.Windows.Data;
using System.Linq;

namespace Microsoft.Support.Workflow.Authoring.Common.Converters
{
    /// <summary>
    /// If all items match, return Visible. Otherwise return Collapsed.
    /// This allows us to bind to two (or more) values at the same time, and if they are equal, turn on an indicator showing the 
    /// item as the currently selected item (for instance).
    /// </summary>
    public class AllMatchToVisibilityConverter : IMultiValueConverter
    {
        /// <summary>
        /// Invert the output. Allows unselected (or items not meeting the criteria) to display some style indicating they 
        /// are 'dimmed out' or unselected (for instance).
        /// </summary>
        public bool Invert { get; set; }

        /// <summary>
        /// Compare a list of values, and if they are all equal, return Visible. Otherwise return Collapsed.
        /// </summary>
        /// <param name="values">The list of values to be compared.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Visible if all items are equal, Collapsed if not.</returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool allEqual = values.Distinct().Count() == 1;

            if (Invert)
                allEqual = !allEqual;

            return allEqual ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
