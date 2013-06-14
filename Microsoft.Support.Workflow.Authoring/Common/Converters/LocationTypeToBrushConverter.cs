// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocationTypeToBrushConverter.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.Common.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// The location type to brush converter.
    /// This converter used to convert local location type to a solid color brush.
    /// To indicate if user provide a local location for referenced assembly.
    /// None will get red brush. New/Cached will get transparent brush.
    /// </summary>
    public class LocationTypeToBrushConverter : IValueConverter
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
            var locationType = (LocationType)value;

            SolidColorBrush brush = null;

            switch (locationType)
            {
                case LocationType.New:
                case LocationType.Cached:
                    brush = new SolidColorBrush(Colors.Transparent);
                    break;
                case LocationType.None:
                    brush = new SolidColorBrush(Colors.Red);
                    break;
                default:
                    brush = new SolidColorBrush(Colors.Transparent);
                    break;
            }

            return brush;
        }

        /// <summary>
        /// The convert back. Never be called.
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