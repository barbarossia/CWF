using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Converters
{
    public class EnumDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = value.ToString();
            int i = 1;
            while (i < text.Length)
            {
                if (char.IsUpper(text[i]))
                {
                    text = text.Insert(i, " ");
                    i++;
                }
                i++;
            }
            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
