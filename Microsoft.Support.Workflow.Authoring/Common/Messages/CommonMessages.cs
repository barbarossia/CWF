// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonMessages.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Common.Messages
{
    /// <summary>
    /// Messages for common areas of the application
    /// </summary>
    internal static class CommonMessages
    {
        /// <summary>
        /// Default waiting message for any operation that contacts the server.
        /// </summary>
        public const string BusyContactingServer = "Contacting Server";

        /// <summary>
        /// Title for the main window of the application.
        /// </summary>
        public const string MainWindowCaption = "Common Workflow Foundry - {0} ({1})";

        /// <summary>
        /// One of the content files is not a well formed xml document 
        /// </summary>
        public const string InvalidContentFile =
            "File {0} provided in the content options is not a well formed xml document and won't be loaded.";

        public const string CyclicDependenciesOnUpload =
            "Cyclic dependency. Unable to upload to database because the dependency graph is not a Directed acyclic graph. This may be due to a a problem detecting the dependencies in the import process.";
    }
}
