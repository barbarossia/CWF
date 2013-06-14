// -----------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Microsoft">
//  Copyright (c) Microsoft Corporation 2012.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace CWF.WorkflowQueryService.Resources
{
    /// <summary>
    /// Provides an easy way to access default app settings
    /// </summary>
    internal static class AppSettings
    {
        public const string AuthorGroupNameKey = "AuthorGroupName";

        /// <summary>
        /// Name of the authorization group that will be validated for access to the service.
        /// </summary>
        public static string AuthorGroupName { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        static AppSettings()
        {
            AuthorGroupName =
                System.Configuration.ConfigurationManager.AppSettings[AuthorGroupNameKey];
           
        }
    }
}
