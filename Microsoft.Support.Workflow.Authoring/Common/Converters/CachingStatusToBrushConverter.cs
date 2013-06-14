// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CachingStatusToBrushConverter.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.Common.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using Microsoft.Support.Workflow.Authoring.AddIns;

    /// <summary>
    /// The caching status to brush converter.
    /// </summary>
    public class CachingStatusToBrushConverter : IValueConverter
    {
        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (CachingStatus)value;
            SolidColorBrush brush = null;
            switch (status)
            {
                case CachingStatus.Server:
                    brush = Brushes.Red;
                    break;
                case CachingStatus.Latest:
                    brush = Brushes.LawnGreen;
                    break;
                case CachingStatus.CachedOld:
                    brush = Brushes.Yellow;
                    break;
                case CachingStatus.None:
                    brush = Brushes.Red;
                    break;
            }

            return brush;
        }

        /// <summary>
        /// The convert back.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The value converted back.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
      
    }
}