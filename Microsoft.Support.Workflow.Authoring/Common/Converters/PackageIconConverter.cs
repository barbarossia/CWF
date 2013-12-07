using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using NuGet;

namespace Microsoft.Support.Workflow.Authoring.Common.Converters {
    public class PackageIconConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return value ?? new Uri("pack://application:,,,/Resources/Images/packageicon.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
