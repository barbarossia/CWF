// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.Tests
{
    using System.Activities;
    using Microsoft.Support.Workflow.Authoring.Services;
    using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

    /// <summary>
    /// The class contains extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        #region Public Methods

        /// <summary>
        /// Get the xaml code from a activity instance.
        /// </summary>
        /// <param name="activity">
        /// The activity.
        /// </param>
        /// <returns>
        /// The xaml of a activity.
        /// </returns>
        public static string ToXaml(this Activity activity)
        {
            var activityBuilder = new ActivityBuilder { Implementation = activity, Name="SomeClass" };
            return XamlService.SerializeToXaml(activityBuilder);
        }

        #endregion
    }
}