using System.Diagnostics;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Logger
{
    public class EventLogWriter : ILogWriter
    {
        private const string eventLogSource = "Microsoft.Support.Workflow.Foundry.exe";
        private const string eventLogName = "CWF Foundry";

        public EventLogWriter()
        {
            try
            {
                if (!EventLog.SourceExists(eventLogSource))
                    EventLog.CreateEventSource(eventLogSource, eventLogName);
            }
            catch
            {
                //TODO: could not log to event log. Do we need to do anything?
            }
        }

        /// <summary>
        /// Write an event to the log. This class uses a message resource file to use insertion strings into message description templates in the message resource file.
        /// </summary>
        /// <param name="eventSource">Event source.</param>
        /// <param name="eventId">Event id</param>
        /// <param name="insertionStrings">A list of objects to be used to create the event message.
        /// </param>
        public bool WriteEvent(int eventId, params object[] insertionStrings)
        {
            return WriteEvent(eventLogSource, eventId, EventLogEntryType.Error, EventCategory.Analytic, insertionStrings);
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
                using (EventLog operationalLog = new EventLog(eventLogName))
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
    }
}
