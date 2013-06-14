//-----------------------------------------------------------------------
// <copyright file="Logging.cs" company="Microsoft">
// Copyright
// logging class
// </copyright>
//-----------------------------------------------------------------------



namespace CWF.WorkflowQueryService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using DataContracts;
    using Microsoft.Support.Workflow.Service.Common.Logging;
    using Microsoft.Support.Workflow.Service.DataAccessServices;

    /// <summary>
    /// Logging class.
    /// </summary>
    public class Logging
    {
        private static Dictionary<int, EventCategory> categoryRangeLookup = new Dictionary<int, EventCategory>
        {
            {59000, EventCategory.Administrative},          
            {56000, EventCategory.Administrative},
            {55000, EventCategory.Operational},
            {54000, EventCategory.Analytic},
            {53000, EventCategory.Debug},
            {52000, EventCategory.ApiUsage},
        };

        /// <summary>
        /// The client that we will be writing the log entries to. This exists to support dependeny injection 
        /// of a mock during testing. EventLog does not have a base class or interface which defines 
        /// WriteEvent/WriteLog/etc, so we can't create a mock that looks like the full implementation of
        /// an EventLog object, or demand an interface here. Instead we will create an actual event log reference 
        /// when asked for one if there is no reference, or accept an injection for testing, 
        /// and call the method dynamically at run/test time - v-richt 3-Nov-2011
        /// </summary>
        private static dynamic logClient;
        public static dynamic LogClient
        {
            get
            {
                if (null == logClient)
                    logClient = new EventLog(SprocValues.LOG_LOCATION);

                return logClient;
            }

            set
            {
                logClient = value;
            }
        }

        /// <summary>
        /// Write an exception to the event log
        /// </summary>
        /// <param name="errorCode">the error code returned from a SQL Server call</param>
        /// <param name="eventLevel">The event level of the message logged (for example, Error, Warning, Information...)</param>
        /// <param name="source">the stored procedure that was called</param>
        /// <param name="ex">the exception that we are logging</param>
        /// <returns>a StatusReplyDC object describing the event log entry</returns>
        public static StatusReplyDC Log(int errorCode, EventLogEntryType eventLevel, string source, Exception ex)
        {
            return WriteEvent(source, errorCode, eventLevel, GetEventCategory(errorCode), new[] { ex.ToString() });
        }

        /// <summary>
        /// Write a message to the event log
        /// </summary>
        /// <param name="errorCode">the error code returned from a SQL Server call</param>
        /// <param name="eventLevel">The event level of the message logged (for example, Error, Warning, Information...)</param>
        /// <param name="source">the stored procedure that was called</param>
        /// <param name="ex">the message that we are logging</param>
        /// <returns>a StatusReplyDC object describing the event log entry</returns>
        public static StatusReplyDC Log(int errorCode, EventLogEntryType eventLevel, string source, string errorMessage)
        {
            return WriteEvent(source, errorCode, eventLevel, GetEventCategory(errorCode), new[] { errorMessage });
        }

        /// <summary>
        /// Write an event to the log.
        /// </summary>
        /// <param name="eventSourceName">Event source name</param>
        /// <param name="eventId">Event id</param>
        /// <param name="eventLevel">Severity level of the event</param>
        /// <param name="eventCategory">Event category. Possible values are in EventCategory enumeration - Administrative, Operational, Analytic, Debug, APIUsage</param>
        /// <param name="description">The list of messages to be logged.</param>
        /// <returns>a Status Reply object suitable for being returned from the WCF service, giving info about the event written</returns>
        private static StatusReplyDC WriteEvent(string eventSourceName, int eventId, EventLogEntryType eventLevel, EventCategory eventCategory, object[] insertionStrings)
        {
            StatusReplyDC result = new StatusReplyDC
            {
                Errorcode = eventId,
                ErrorGuid = string.Format("{0}\r\n\r\n{1}",
                                        SprocValues.LOG_LOCATION + ":" + eventSourceName,
                                          insertionStrings.ToList()
                                                          .Aggregate((first, last) => string.Format("{0}\r\n{1}", first, last))
                                                          .ToString()),
            };

            LogClient.Source = SprocValues.DAL_CALLER_INFO;
            LogClient.Log = SprocValues.LOG_LOCATION;
            LogClient.WriteEvent(new EventInstance(eventId, (int)eventCategory, eventLevel), insertionStrings);

            return result;
        }

        /// <summary>
        /// By the range of the error code provided, determine what the category should be.
        /// </summary>
        /// <remarks>See categoryRangeLookup in this class for the mapping of ranges to categories</remarks>
        /// <param name="errorCode">the error code that we are going to map</param>
        /// <returns>the appropriate category for the error code, or EventCategory.None if none could be found</returns>
        public static EventCategory GetEventCategory(int errorCode)
        {
            EventCategory result = EventCategory.None;

            var entry = categoryRangeLookup.Where(currentEntry => ((errorCode / 1000) * 1000) == currentEntry.Key)
                                           .FirstOrDefault();

            if (entry.Key != 0)
                result = entry.Value;

            return result;
        }



    }
}
