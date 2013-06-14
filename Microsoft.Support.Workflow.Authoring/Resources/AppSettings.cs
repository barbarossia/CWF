// -----------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Microsoft">
//  Copyright (c) Microsoft Corporation 2012.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Resources
{
    /// <summary>
    /// Provides an easy way to access default app settings
    /// </summary>
    internal static class AppSettings
    {
        /// <summary>
        /// Email account to display for users to request access rights for the application.
        /// </summary>
        public static string AuthorizationContactEmail { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        static AppSettings()
        {
            AuthorizationContactEmail =
                System.Configuration.ConfigurationManager.AppSettings["AuthorizationContactEmail"];


        }
    }
}
