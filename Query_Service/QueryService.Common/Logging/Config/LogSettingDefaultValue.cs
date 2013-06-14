using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Service.Common.Logging.Config
{
    /// <summary>
    /// Defines the default values used in case a corresponding value is not defined in the log setting configuration.
    /// </summary>
    public static class LogSettingDefaultValue
    {
        /// <summary>
        /// Log name default value for this application.
        /// </summary>
        public const string LogName = "WorkflowQueryService";

        /// <summary>
        /// Log level default value for this application.
        /// </summary>
        public const int LogLevelDefault = 0;
    }
}
