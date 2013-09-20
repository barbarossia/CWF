using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Logger
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
        /// <param name="insertionStrings">A list of objects to be used to create the event message by replacing the message format placeholders.</param>
        bool WriteEvent(int eventId, params object[] insertionStrings);
    }
}
