using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Service.Common.Logging.Config
{
    /// <summary>
    /// Defines constants for literals used in log setting configuration reading classes.
    /// </summary>
    public static class LogSettingConfigConstant
    {
        /// <summary>
        /// Defines configuration attribute names.
        /// </summary>
        public static class AttributeName
        {
            /// <summary>
            /// Item key attribute.
            /// </summary>
            public const string Key = "key";

            /// <summary>
            /// Item value attribute.
            /// </summary>
            public const string Value = "value";
        }

        /// <summary>
        /// Defines configuration node names.
        /// </summary>
        public static class NodeName
        {
            /// <summary>
            /// Logging configuration root node name.
            /// </summary>
            public const string LoggingConfiguration = "loggingConfiguration";

            /// <summary>
            /// Log settings collection node name.
            /// </summary>
            public const string Settings = "settings";
        }
    }
}
