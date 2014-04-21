using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Microsoft.Support.Workflow.Authoring.CDS;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using NuGet;
using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

namespace Microsoft.Support.Workflow.Authoring.Common.Converters {
    public class PackageStatusConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            IPackage package = (IPackage)values.SingleOrDefault(v => v is IPackage);
            if (package == null)
                return null;

            PackageSearchType type = (PackageSearchType)values.Single(v => v is PackageSearchType);
            switch (type) {
                case PackageSearchType.Local:
                case PackageSearchType.Online:
                    bool isInstalled = CDSService.IsInstalled(package);
                    if (isInstalled)
                        return TextResources.Uninstall;
                    else
                        return TextResources.Install;
                case PackageSearchType.Update:
                    return TextResources.Update;
                default:
                    throw new NotImplementedException();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
