using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DAL;
using Microsoft.DynamicImplementations;
using CWF.DataContracts;
using System.Diagnostics;

namespace Query_Service.Tests.Common
{

    /// <summary>
    /// Source mock objects that look enough like the EventLog class for our testing purposes
    /// </summary>
    public class EventLogMockFactory
    {
        private EventLogMockFactory() { }

        static List<Tuple<int, EventLogEntryType, long, string>> mockLogEntries = new List<Tuple<int, EventLogEntryType, long, string>>();

        /// <summary>
        /// This holds the entries made to our fake event log
        /// </summary>
        public static List<Tuple<int, EventLogEntryType, long, string>> MockLogEntries
        {
            get { return EventLogMockFactory.mockLogEntries; }
            set { EventLogMockFactory.mockLogEntries = value; }
        }


        /// <summary>
        /// generate a mock object that looks like EventLog, and return it
        /// </summary>
        /// <returns>a mock of Eventlog</returns>
        public static Implementation<EventLog> GetEventLogMock()
        {
            Implementation<EventLog> mock = new Implementation<EventLog>();

               mock.Register("get_Source")
                  .Execute(() =>
                  {
                      // we don't care about the Source property for testing purposes
                      return String.Empty;
                  });

            mock.Register("set_Source")
                    .Execute((string theValue) =>
                    {
                        // we don't care about the Source property for testing purposes
                    });


            mock.Register(instance => instance.WriteEvent(Argument<EventInstance>.Any, Argument<object[]>.Any))
                .Execute((EventInstance eventInstance, object[] args) =>
                {
                    mockLogEntries.Add(
                            new Tuple<int, EventLogEntryType, long, string>
                            (
                                eventInstance.CategoryId,
                                eventInstance.EntryType,
                                eventInstance.InstanceId,
                                args.Aggregate((a, b) => string.Format("{0}\r\n\r\n{1}", a, b)).ToString()
                            )
                        );
                });


            return mock;
        }
    }

}
