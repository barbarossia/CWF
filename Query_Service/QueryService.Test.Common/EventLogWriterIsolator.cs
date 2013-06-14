using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Service.Common.Logging;
using System.Diagnostics;
namespace Microsoft.Support.Workflow.Service.Test.Common
{
    /// <summary>
    /// Defines dynamic implementations to isolate the event logging functionality.
    /// </summary>
    public static class EventLogWriterIsolator
    {
        /// <summary>
        /// Gets a dynamic implementation of event log writer that does not really perform any log writing.
        /// </summary>
        /// <returns>Dynamic implementation of type EventLogWriter.</returns>
        public static ImplementationOfType GetNoLoggingEventLogWriterMock()
        {
            ImplementationOfType impl = new ImplementationOfType(typeof(EventLogWriter));
            impl.Register<EventLogWriter>(inst => inst.WriteEvent(Argument<string>.Any, Argument<int>.Any, Argument<EventLogEntryType>.Any, Argument<EventCategory>.Any, Argument<object[]>.Any)).Execute(delegate { return true; });
            return impl;
        }
    }
}
