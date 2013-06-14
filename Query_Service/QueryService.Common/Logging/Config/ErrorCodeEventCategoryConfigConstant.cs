using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Service.Common.Logging.Config
{
    /// <summary>
    /// Defines constants for literals used in "error code - event category map configuration" reading classes.
    /// </summary>
    public static class ErrorCodeEventCategoryConfigConstant
    {
        /// <summary>
        /// Defines configuration attribute names.
        /// </summary>
        public static class AttributeName
        {
            /// <summary>
            /// Error code attribute.
            /// </summary>
            public const string ErrorCode = "errorCode";

            /// <summary>
            /// Event category attribute.
            /// </summary>
            public const string EventCategory = "eventCategory";

            /// <summary>
            /// Description attribute.
            /// </summary>
            public const string Description = "description";

            /// <summary>
            /// Severity attribute.
            /// </summary>
            public const string Severity = "severity";
        }

        /// <summary>
        /// Defines configuration node names.
        /// </summary>
        public static class NodeName
        {
            /// <summary>
            /// Error code - event category configuration node name.
            /// </summary>
            public const string ErrorCodeEventCategoryConfiguration = "errorCodeEventCategoryConfiguration";

            /// <summary>
            /// Error codes node name.
            /// </summary>
            public const string ErrorCodes = "errorCodes";
        }
    }
}
