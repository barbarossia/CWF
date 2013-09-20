using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using Microsoft.Support.Workflow.Authoring.Security;
using Microsoft.Support.Workflow.Authoring.Services;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Converters {
    /// <summary>
    /// Check if the user has specific permission in the environment
    /// </summary>
    public class EnvPermissionConverter : IValueConverter {
        /// <summary>
        /// Convert Env and Permission to boolean or Visibility
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            // Parse what permission is required
            Permission requires;
            if (parameter is string) {
                requires = (Permission)Enum.Parse(typeof(Permission), (string)parameter);
            }
            else if (parameter is Permission) {
                requires = (Permission)parameter;
            }
            else
                throw new ArgumentException("The value of argument 'parameter' is invalid.");

            // Check if the user has the permission
            bool hasPermission = AuthorizationService.Validate(value is Env ? (Env)value : (Env)0, requires);
            // return with target type
            if (targetType == typeof(Visibility))
                return hasPermission ? Visibility.Visible : Visibility.Collapsed;
            else
                return hasPermission;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
