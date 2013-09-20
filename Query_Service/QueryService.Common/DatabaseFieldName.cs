using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.QueryService.Common
{
    /// <summary>
    /// Defines data field names.
    /// </summary>
    public static class DataFieldName
    {
        /// <summary>
        /// Defines data field names for activity library table interaction.
        /// </summary>
        public static class ActivityLibrary
        {
            public const string AuthGroupId = "AuthGroupId";

            public const string AuthGroupName = "AuthGroupName";

            public const string Category = "Category";

            public const string CategoryId = "CategoryId";

            public const string Description = "Description";

            public const string Guid = "Guid";

            public const string HasActivities = "HasActivities";

            public const string Id = "Id";

            public const string ImportedBy = "ImportedBy";

            public const string MetaTags = "MetaTags";

            public const string Name = "Name";

            public const string Status = "Status";

            public const string StatusName = "StatusName";

            public const string VersionNumber = "VersionNumber";

            public const string Executable = "Executable";

            public const string FriendlyName = "FriendlyName";

            public const string ReleaseNotes = "ReleaseNotes";

            public const string Environment = "Environment";
        }
    }
}