// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthMessages.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CWF.WorkflowQueryService.Authentication
{
    /// <summary>
    /// Error Messages for authorization exceptions
    /// </summary>
    internal sealed class AuthMessages
    {
        /// <summary>
        /// No credentials were provided
        /// </summary>
        public const string NullCredentials = "Access is denied. No user credentials were provided";
        /// <summary>
        /// The configuration section for the administrators group is missing in the config file for the service.
        /// </summary>
        public const string ConfigurationMissing = "Access is denied. Credentials configuration is missing.";
        /// <summary>
        /// User is connected in an anonymous way.
        /// </summary>
        public const string AnonymousAccess = "Access denied. Anonymous access is not allowed.";
        /// <summary>
        /// User does not belong to the admin group stated in the configuration file.
        /// </summary>
        public const string InvalidCredentials = "Invalid Credentials. The user {0} is not part of the security group {1}.";

    }
}