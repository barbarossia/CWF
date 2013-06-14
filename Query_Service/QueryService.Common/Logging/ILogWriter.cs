using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Microsoft.Support.Workflow.Service.Common.Logging
{
    /// <summary>
    /// Defines the interface for a log writer to implement.
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Writes an event to the log.
        /// </summary>
        /// <param name="eventSourceName">Event source name.</param>
        /// <param name="eventId">Event ID.</param>
        /// <param name="eventLevel">Severity level of the event.</param>
        /// <param name="eventCategory">Event category. Possible values are defined in EventCategory enumeration.</param>
        /// <param name="insertionStrings">A list of objects to be used to create the event message by replacing the message format placeholders.</param>
        bool WriteEvent(string eventSourceName, int eventId, EventLogEntryType eventLevel, EventCategory eventCategory, params object[] insertionStrings);

        /// <summary>
        /// Writes an exception to the log.
        /// </summary>
        /// <param name="eventSourceName">Event source name.</param>
        /// <param name="eventId">Event ID.</param>
        /// <param name="eventLevel">Severity level of the event.</param>
        /// <param name="eventCategory">Event category. Possible values are defined in EventCategory enumeration.</param>
        /// <param name="insertionStrings">A list of objects to be used to create the event message by replacing the message format placeholders.</param>
        bool WriteException(string eventSourceName, int eventId, EventLogEntryType eventLevel, EventCategory eventCategory, params object[] insertionStrings);
    }
}
