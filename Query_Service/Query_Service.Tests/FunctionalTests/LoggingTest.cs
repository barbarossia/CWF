using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


using Microsoft.VisualStudio.TestTools.UnitTesting;
using Query_Service.Tests.Common;
using Microsoft.Support.Workflow.Service.Common.Logging;
using Microsoft.Support.Workflow.Service.DataAccessServices;

namespace Query_Service.Tests.FunctionalTests
{
    [TestClass]
    public class LoggingTest
    {
        const string TEST_OWNER = "v-richt";
        const string EVENT_SOURCE_NAME = "Testing - Query_Service.Tests.FunctionalTests.LoggingTest";
        const string LOG_LOCATION = "Application";
        const int ADMINISTRATIVE_EVENT_ID = 56012;
        const int OPERATIONAL_EVENT_ID = 55013;
        const int ANALYTIC_EVENT_ID = 54014;
        const int DEBUG_EVENT_ID = 53100;
        const int APIUSAGE_EVENT_ID = 52999;
        const int MAX_EVENT_ID = 0xFFFF;
        const EventCategory DEFAULT_EVENT_CATEGORY = EventCategory.Analytic;
        const EventLogEntryType EVENT_LOG_ENTRY_TYPE = EventLogEntryType.Information;
        const string TEST_MESSAGE = "Test Event logging message - please ignore ({0})";
        private const short EVENT_CATEGORY_RANGE_LOW = -1000;
        private const int EVENT_CATEGORY_RANGE_HIGH = 99000;
        private const short EVENT_CATEGORY_RANGE_STEP = 500;

        [WorkItem(86716)]
        [Description("Verify string description overload of WriteEvent()")]
        [Owner("DiffReqTest")]//[Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void Verify_Log_StringOverload()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            string theTestMessage = string.Format(TEST_MESSAGE, random.Next(10000)); // a message with a unique-enough number in it, so we can identify it when we look for entries we have written

            Logging.LogClient = EventLogMockFactory.GetEventLogMock().Instance;   // inject the mock event log into the logging class

            Logging.Log(ADMINISTRATIVE_EVENT_ID, EventLogEntryType.Warning, EVENT_SOURCE_NAME, theTestMessage);
            CheckNewLogEntry(ADMINISTRATIVE_EVENT_ID, EVENT_SOURCE_NAME, theTestMessage);

            Logging.Log(OPERATIONAL_EVENT_ID, EventLogEntryType.Warning, EVENT_SOURCE_NAME, theTestMessage);
            CheckNewLogEntry(OPERATIONAL_EVENT_ID, EVENT_SOURCE_NAME, theTestMessage);

            Logging.Log(ANALYTIC_EVENT_ID, EventLogEntryType.Warning, EVENT_SOURCE_NAME, theTestMessage);
            CheckNewLogEntry(ANALYTIC_EVENT_ID, EVENT_SOURCE_NAME, theTestMessage);

            Logging.Log(DEBUG_EVENT_ID, EventLogEntryType.Warning, EVENT_SOURCE_NAME, theTestMessage);
            CheckNewLogEntry(DEBUG_EVENT_ID, EVENT_SOURCE_NAME, theTestMessage);

            Logging.Log(APIUSAGE_EVENT_ID, EventLogEntryType.Warning, EVENT_SOURCE_NAME, theTestMessage);
            CheckNewLogEntry(APIUSAGE_EVENT_ID, EVENT_SOURCE_NAME, theTestMessage);

            Logging.Log(MAX_EVENT_ID, EventLogEntryType.Warning, EVENT_SOURCE_NAME, theTestMessage);
            CheckNewLogEntry(MAX_EVENT_ID, EVENT_SOURCE_NAME, theTestMessage);

            Logging.LogClient = null;  // reset the test injection
        }

        [WorkItem(86716)]
        [Description("Verify Exception class overload of WriteEvent()")]
        [Owner("DiffReqTest")]//[Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void Verify_Log_ExceptionOverload()
        {

            Random random = new Random(DateTime.Now.Millisecond);
            string theTestMessage = string.Format(TEST_MESSAGE, random.Next(10000)); // a message with a unique-enough number in it, so we can identify it when we look for entries we have written
            Exception theException = new ArgumentException(theTestMessage);

            Logging.LogClient = EventLogMockFactory.GetEventLogMock().Instance;  // inject the mock event log into the logging class

            Logging.Log(ADMINISTRATIVE_EVENT_ID, EventLogEntryType.Warning, EVENT_SOURCE_NAME, theException);
            CheckNewLogEntry(ADMINISTRATIVE_EVENT_ID, EVENT_SOURCE_NAME, theException.ToString());

            Logging.Log(OPERATIONAL_EVENT_ID, EventLogEntryType.Warning, EVENT_SOURCE_NAME, theException);
            CheckNewLogEntry(OPERATIONAL_EVENT_ID, EVENT_SOURCE_NAME, theException.ToString());

            Logging.Log(ANALYTIC_EVENT_ID, EventLogEntryType.Warning, EVENT_SOURCE_NAME, theException);
            CheckNewLogEntry(ANALYTIC_EVENT_ID, EVENT_SOURCE_NAME, theException.ToString());

            Logging.Log(DEBUG_EVENT_ID, EventLogEntryType.Warning, EVENT_SOURCE_NAME, theException);
            CheckNewLogEntry(DEBUG_EVENT_ID, EVENT_SOURCE_NAME, theException.ToString());

            Logging.Log(APIUSAGE_EVENT_ID, EventLogEntryType.Warning, EVENT_SOURCE_NAME, theException);
            CheckNewLogEntry(APIUSAGE_EVENT_ID, EVENT_SOURCE_NAME, theException.ToString());

            Logging.Log(MAX_EVENT_ID, EventLogEntryType.Warning, EVENT_SOURCE_NAME, theException);
            CheckNewLogEntry(MAX_EVENT_ID, EVENT_SOURCE_NAME, theException.ToString());

            Logging.LogClient = null;  // reset the test injection
        }


        /// <summary>
        /// get the current list of entries in the log we are writing to. We will use this to check to see if we
        /// are successfully writing to the log.
        /// </summary>
        /// <returns>the entries in our log</returns>
        private List<EventLogEntry> GetCurrentLogEntries()
        {
            List<EventLogEntry> rawList = new List<EventLogEntry>();
            List<EventLogEntry> list;

            using (EventLog log = new EventLog(LOG_LOCATION))
            {
                foreach (EventLogEntry entry in log.Entries)
                    rawList.Add(entry);
            }

            list = rawList.Where(entry => (entry.Source == EVENT_SOURCE_NAME))
                          .OrderByDescending(entry => entry.TimeWritten)
                          .ToList();

            return list;
        }


        /// <summary>
        /// Check to see if the specified log entry exists. Assert that it is there (and fail the test if it isn't)
        /// </summary>
        /// <param name="eventId">The event ID for the log entry</param>
        /// <param name="eventSourceName">The event source for the log entry</param>
        /// <param name="message">The specific message that we expect to have been posted.</param>
        private void CheckNewLogEntry(int eventId, string eventSourceName, string message)
        {
            var query = from entry in EventLogMockFactory.MockLogEntries
                        where entry.Item4.StartsWith(message) &&
                              entry.Item3 == eventId
                        select entry;

            Assert.IsTrue(query.Count() > 0,
                         string.Format("Event Log entry with event ID={0}, Source={1}, Message='{2}' was not written correctly to the event log.",
                                       eventId,
                                       eventSourceName,
                                       message));
        }

        [WorkItem(97518)]
        [Description("Dal: GetEventcategory No range check Throws exception")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void Verify_Log_GetEventCategory()
        {
            for (int i = -EVENT_CATEGORY_RANGE_LOW; i < EVENT_CATEGORY_RANGE_HIGH; i += EVENT_CATEGORY_RANGE_STEP)
                Logging.GetEventCategory(i);
        }
    }
}
