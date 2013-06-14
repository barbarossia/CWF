using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using Microsoft.Support.Workflow.Service.Common.Logging;
using Microsoft.Support.Workflow.QueryService.Common;

namespace CWF.WorkflowQueryService
{
    /// <summary>
    /// Defines extension methods for Exception class.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Handles an Exception.  Logs it and throws appropriate fault exception.
        /// </summary>
        /// <param name="e">Exception.</param>
        public static void HandleException(this Exception e)
        {
            LogWriterFactory.LogWriter.WriteException(EventSource.WebServiceLayerError.ToString(), EventCode.WebServiceLayerEvent.Error.UnhandledException, System.Diagnostics.EventLogEntryType.Error, EventCategory.Operational, e);
            throw FaultExceptionUtility.GetFaultException(EventCode.WebServiceLayerEvent.Error.UnhandledException);
        }
    }
}