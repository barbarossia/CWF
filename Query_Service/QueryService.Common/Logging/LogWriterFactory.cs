using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Service.Common.Logging.Config;

namespace Microsoft.Support.Workflow.Service.Common.Logging
{
    /// <summary>
    /// Defines a factory to create single instance of an appropriate log writer.  
    /// The type of log writer could be configured to dynamically return the relevant
    /// writer for the application.  The target log writer is encapsulated from the caller.
    /// The particular log writer target could be event log, through IMF, SQL or any 
    /// mechanism or a combination.  A log writer of type ILogWriter has to be defined
    /// to provide the desired behavior of log writing to the target.
    /// </summary>
    public class LogWriterFactory
    {
        private static ILogWriter logWriter = null;
        private static string logWriterTypeName = string.Empty;
        private static object syncRoot = new object();

        /// <summary>
        /// Returns an instance of an ILogWriter that corresponds to the type 
        /// value found in the configuration.
        /// </summary>
        /// <returns>ILogWriter instance.</returns>
        private static ILogWriter InitializeLogWriter()
        {
            try
            {
                logWriterTypeName = LogSettingConfigSection.Current.Settings[LogSettingKey.LogWriter].Value;
                Type type = Type.GetType(logWriterTypeName);
                if (type == null) return null;
                ILogWriter writer = (ILogWriter)Activator.CreateInstance(type);
                return writer;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the instance ILogWriter that is configured to log events and exceptions.
        /// </summary>
        public static ILogWriter LogWriter
        {
            get
            {
                // Detect if the configuration value has changed.
                string newTypeName = LogSettingConfigSection.Current.Settings[LogSettingKey.LogWriter].Value;
                if (newTypeName != logWriterTypeName)
                {
                    lock (syncRoot)
                    {
                        // Thread safe instantiation of new instance when a change is detected.
                        if (newTypeName != logWriterTypeName)
                        {
                            logWriter = InitializeLogWriter();
                        }
                    }
                }
                return logWriter;
            }
        }
    }
}
