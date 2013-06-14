using System;
using System.Diagnostics;
using System.Threading;
using System.Web;
using System.Configuration;
using Microsoft.Support.Workflow.Service.Common.Logging.Config;
using Microsoft.Support.Workflow.QueryService.Common;

namespace Microsoft.Support.Workflow.Service.Common.Logging
{
    /// <summary>
    /// A class for writing an event to Windows Event Log. Besides the message to log, the caller can specify 
    /// the event source, type (error/warning/information etc) and the event id.
    /// </summary>
    public class EventLogWriter : ILogWriter
    {
        private const string eventLogSource = "CWF.DAL";
        private static LogSettingConfigSection logSettings = LogSettingConfigSection.Current;
        private static string eventLogName =  string.Empty;
        private static int defaultLevel = GetDefaultLogLevel();

        /// <summary>
        /// Default constructor that reads the log name from the AppSetting "EventLogName"
        /// </summary>
        public EventLogWriter()
        {
            eventLogName = logSettings.Settings[LogSettingKey.LogName].Value;

            if (String.IsNullOrWhiteSpace(eventLogName))
            {
                eventLogName = LogSettingDefaultValue.LogName;
            }
        }

        /// <summary>
        /// Constructor that takes the log name as input. If an empty or null string is passed, the default is "SharedContext"
        /// </summary>
        /// <param name="logName">Event log name.</param>
        public EventLogWriter(string logName)
        {
            if (String.IsNullOrWhiteSpace(logName))
            {
                eventLogName = LogSettingDefaultValue.LogName;
            }
            defaultLevel = GetDefaultLogLevel();
        }

        /// <summary>
        /// Gets default log level for any event source.
        /// </summary>
        /// <returns></returns>
        private static int GetDefaultLogLevel()
        {
            int configLevel = LogSettingDefaultValue.LogLevelDefault;
            if (logSettings.Settings[LogSettingKey.LogLevelDefault] != null)
            {
                int.TryParse(logSettings.Settings[LogSettingKey.LogLevelDefault].Value, out configLevel);
            }
            return configLevel;
        }

        /// <summary>
        /// Write an event to the log. This class uses a message resource file to use insertion strings into message description templates in the message resource file.
        /// </summary>
        /// <param name="eventSource">Event source.</param>
        /// <param name="eventId">Event id</param>
        /// <param name="eventLevel">Severity level of the event</param>
        /// <param name="eventCategory">Event category. Possible values are in EventCategory enumeration - Administrative, Operational, Analytic, Debug, APIUsage</param>
        /// <param name="insertionStrings">A list of objects to be used to create the event message.
        /// </param>
        public bool WriteEvent(string eventSource, int eventId, EventLogEntryType eventLevel, EventCategory eventCategory, params object[] insertionStrings)
        {
            try
            {
                // see if a category and severity is defined in the configuration file.
                // An entry in the config file overwrites the user supplied value.
                ErrorCodeEventCategoryConfigSection errorCodeConfigSection = ErrorCodeEventCategoryConfigSection.Current;

                if (errorCodeConfigSection != null && errorCodeConfigSection.ErrorCodes[eventId] != null)
                {
                    eventCategory = errorCodeConfigSection.ErrorCodes[eventId].Category;
                    string severity = errorCodeConfigSection.ErrorCodes[eventId].Severity;

                    switch (severity.ToLower())
                    {
                        case "error":
                            eventLevel = EventLogEntryType.Error;
                            break;
                        case "warning":
                            eventLevel = EventLogEntryType.Warning;
                            break;
                        case "information":
                            eventLevel = EventLogEntryType.Information;
                            break;
                        case "successaudit":
                            eventLevel = EventLogEntryType.SuccessAudit;
                            break;
                        case "failureaudit":
                            eventLevel = EventLogEntryType.FailureAudit;
                            break;
                        default:
                            // if no value specified, we will use use-supplied value
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Catching "Exception" to prevent the application from crashing during the configuration reading.

                // Log the exception and get out. if reading the configuration file gives error, use the user-supplied values.
                WriteToEventLog(eventLogName, EventSource.LogWriterError.ToString(), 
                    EventCode.WebServiceLayerEvent.Error.ErrorCodeToEventCategoryMappingConfigNotFound, 
                    EventLogEntryType.Error, EventCategory.Administrative, new string[] { ex.ToString() });
            }

            int msgLevel;
            switch (eventLevel)
            {
                case EventLogEntryType.FailureAudit:
                case EventLogEntryType.Error:
                    msgLevel = 0;
                    break;
                case EventLogEntryType.Warning:
                    msgLevel = 1;
                    break;
                default:
                    msgLevel = 2;
                    break;
            }

            int logLevel = defaultLevel;

            string level = logSettings.Settings[LogSettingKey.GetLogLevelKey(eventSource)].Value;

            if (!String.IsNullOrWhiteSpace(level))
            {
                logLevel = int.TryParse(level, out logLevel) ? logLevel : defaultLevel;
            }

            if (logLevel < msgLevel)
            {
                return true;
            }

            return WriteToEventLog(eventLogName, eventSource, eventId, eventLevel, eventCategory, insertionStrings);
        }

        private bool WriteToEventLog(string logName, string eventSource, int eventId, EventLogEntryType eventLevel, EventCategory eventCategory, object[] insertionStrings)
        {
            try
            {
                using (EventLog operationalLog = new EventLog(logName))
                {
                    operationalLog.Source = eventLogSource;
                    operationalLog.WriteEvent(new EventInstance(eventId, (int)eventCategory, eventLevel), insertionStrings);
                }

                return true;
            }
            catch
            {
                //TODO: could not log to event log. Do we need to do anything?
                return false;
            }
        }

        /// <summary>
        /// Write an event to the log. Convenience wrapper for WriteEvent. 
        /// Difference from WriteEvent is that this method transforms Exceptions to events via Exception.ToString().
        /// </summary>
        /// <param name="eventSource">Event source name</param>
        /// <param name="eventId">Event id</param>
        /// <param name="eventLevel">Severity level of the event</param>
        /// <param name="eventCategory">Event category. Possible values are in EventCategory enumeration - Administrative, Operational, Analytic, Debug, APIUsage</param>
        /// <param name="insertionStrings">A list of objects to be used to create the event message.</param>
        public bool WriteException(string eventSource, int eventId, EventLogEntryType eventLevel, EventCategory eventCategory, params object[] insertionStrings)
        {
            for (int i = 0; i < insertionStrings.Length; i++)
            {
                if (insertionStrings[i] is Exception)
                {
                    Exception e = (Exception)insertionStrings[i];
                    insertionStrings[i] = String.Format("message = {0}", e.ToString());
                }
            }
            return WriteEvent(eventSource, eventId, eventLevel, eventCategory, insertionStrings);
        }

    }
}
