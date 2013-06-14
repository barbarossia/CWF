// -----------------------------------------------------------------------
// <copyright file="NegativeNumberConverter.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.Common.Converters
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Converts a number to its negation. On null, returns zero.
    /// </summary>
    public class NegativeNumberConverter : IValueConverter
    {
        /// <summary>
        /// Converts the value parameter to its negative, if possible. If not, returns 0 if null or an unparseable value.
        /// </summary>
        /// <param name="value">The value to be negated, if possible.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>The negation of the value parameter.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var theValue = value ?? 0;
            double result = 0;

            double.TryParse("0" + theValue.ToString(), out result);

            return -result;
        }

        /// <summary>
        /// Converts a negative number back to its original value. Not used/Not implemented. This is here to satisfy the IValueConverter
        /// Interface.
        /// </summary>
        /// <param name="value">Not used.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Not used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
