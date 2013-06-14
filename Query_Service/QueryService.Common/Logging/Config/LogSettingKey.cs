using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Service.Common.Logging.Config
{
    /// <summary>
    /// Defines the item key names used in log setting configuration.
    /// </summary>
    public static class LogSettingKey
    {
        /// <summary>
        /// Log name key.
        /// </summary>
        public const string LogName = "LogName";

        /// <summary>
        /// Default log level key.
        /// </summary>
        public const string LogLevelDefault = "logLevel.default";

        /// <summary>
        /// Log writer name key.
        /// </summary>
        public const string LogWriter = "LogWriter";

        /// <summary>
        /// Gets the log level key that corresponds to a given event source name.
        /// </summary>
        /// <param name="eventSource">Event source name.</param>
        /// <returns>Log level key name for the event source.</returns>
        public static string GetLogLevelKey(string eventSource)
        {
            return String.Concat("loglevel.", eventSource.ToLower());
        }
    }
}
