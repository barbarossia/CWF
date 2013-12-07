

namespace Microsoft.Support.Workflow.Authoring.AddIns.Converters
{
    using System;
    using System.Windows.Data;

    public class BooleanInverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool) && targetType != typeof(bool?))
                throw new InvalidOperationException("The target must be a boolean");

            if (value == null)
                return null;

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
