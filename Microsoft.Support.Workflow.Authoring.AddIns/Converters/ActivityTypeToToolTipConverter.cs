// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityTypeToToolTipConverter.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.AddIns.Converters
{
    using System;
    using System.Activities.Presentation.Toolbox;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

    /// <summary>
    /// The activity type to tool tip converter. Used to show tool tip in tool box.
    /// There's a dictionary, key is Activity type, value is it's description. The description will be shown as tool tip.
    /// </summary>
    public class ActivityTypeToToolTipConverter : IValueConverter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="ActivityTypeToToolTipConverter"/> class.
        /// </summary>
        static ActivityTypeToToolTipConverter()
        {
            ToolTipDictionary = new Dictionary<Type, string>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets tool tip Dictionary. Key is Activity type, value is description for this type.
        /// Description stored in ActivityItem.Description.
        /// </summary>
        public static Dictionary<Type, string> ToolTipDictionary { get; set; }

        #endregion

        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>
        /// The convert. Input value is an Activity type, output is its description.
        /// </summary>
        /// <param name="value">
        /// The input value.
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
        /// The converted value. The decription of this Activity type. Shown as tool tip.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var activityType = value as Type;
            if (activityType == null)
            {
                return null;
            }
            else if (ToolTipDictionary.Keys.Contains(activityType))
            {
                // key/value was inserted by ToolboxControlService.CreateUserActivitiesToolbox
                return ToolTipDictionary[activityType]; // Version, Description
            }
            else
            {
                return string.Format(TextResources.BuiltInActivityTooltipFormat, activityType.Assembly.GetName().Version);
            }
        }

        /// <summary>
        /// The convert back. Won't be called.
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

        #endregion

        #endregion
    }
}