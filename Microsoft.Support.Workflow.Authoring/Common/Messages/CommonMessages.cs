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
        public const string WorkflowNameErrorString = "Workflow Name is invalid. A valid workflow name must begin with a letter and only can includes [a-z,A-Z,0-9,_] characters.\r\n";
        public const string ClassNameRegularExpression = @"^[a-zA-Z][a-zA-Z0-9_]*$";
        /// <summary>
        /// Default waiting message for any operation that contacts the server.
        /// </summary>
        public const string BusyContactingServer = "Contacting Server";

        /// <summary>
        /// Title for the main window of the application.
        /// </summary>
        public const string MainWindowCaption = "Common Workflow Foundry - {0}";

        /// <summary>
        /// One of the content files is not a well formed xml document 
        /// </summary>
        public const string InvalidContentFile =
            "File {0} provided in the content options is not a well formed xml document and won't be loaded.";

        public const string CyclicDependenciesOnUpload =
            "Cyclic dependency. Unable to upload to database because the dependency graph is not a Directed acyclic graph. This may be due to a a problem detecting the dependencies in the import process.";
    }
}
