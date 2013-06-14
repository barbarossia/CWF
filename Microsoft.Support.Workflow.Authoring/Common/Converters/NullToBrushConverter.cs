// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullToBrushConverter.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Common.Converters
{
    using System;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Transform a null value to an error brush (Red)
    /// </summary>
    public class NullToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var bindingValue = (value ?? string.Empty).ToString();

            if (!string.IsNullOrEmpty(bindingValue))
            {
                return Brushes.Transparent;
            }
            else
            {
                return Brushes.Red;
               
            }
        }

        /// <summary>
        /// Convert to the previous value. Not implemented as this is never used.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
