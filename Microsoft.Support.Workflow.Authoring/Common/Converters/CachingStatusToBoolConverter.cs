// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CachingStatusToBoolConverter.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Common.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Microsoft.Support.Workflow.Authoring.AddIns;

    /// <summary>
    /// The caching status to bool converter.
    /// </summary>
    public class CachingStatusToBoolConverter : IValueConverter
    {
        bool invert;

        /// <summary>
        /// Determines whether the result of the convert should be inverted.
        /// </summary>
        public bool Invert
        {
            get { return invert; }
            set { invert = value; }
        }


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
            var result = (status == CachingStatus.Latest);

            if (Invert)
                result = !result;

            return result;
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