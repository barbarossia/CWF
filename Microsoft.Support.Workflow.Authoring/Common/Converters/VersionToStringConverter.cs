namespace Microsoft.Support.Workflow.Authoring.Common.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Globalization;

    public class VersionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnValue = string.Empty;
            if (value != null && targetType == typeof(string))
            {
                Version v = value as Version;
                if (v != null)
                    returnValue = v.ToString();
            }
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
