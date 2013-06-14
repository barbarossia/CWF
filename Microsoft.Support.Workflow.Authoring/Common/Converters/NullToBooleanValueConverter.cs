// -----------------------------------------------------------------------
// <copyright file="NullToBooleanValueConverter.cs" company="Microsoft">
//  Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Microsoft.Support.Workflow.Authoring.Common.Converters
{
    /// <summary>
    /// If null, returns false. Anything else returns true. If the Invert property is true, it inverts the result (ie, true becomes false, false becomes true)
    /// </summary>
    public class NullToBooleanValueConverter : IValueConverter
    {
        public bool Invert { get; set; }

        /// <summary>
        /// converts any nullable object to a true/false value. If it's null, it's false, otherwise it's true.
        /// in the case of value being boolean, it returns the value as a bool instead of testing for null.
        /// </summary>
        /// <param name="value">the value to be converted</param>
        /// <param name="targetType">any type that can be null, or, a boolean value</param>
        /// <param name="parameter">not used, required by the IValueConverter interface</param>
        /// <param name="culture">not used, required by the IValueConverter interface </param>
        /// <returns>true or false, depending on whether the value is null, or if it is a boolean type value and true/false.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = value != null;

            if (value is bool)
                result = (bool)value;

            if (Invert)
                result = !result;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // return value == null ? false : true;
            throw new NotImplementedException();
        }

    }
}
