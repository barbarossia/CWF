using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Service.Common.Logging;

namespace Microsoft.Support.Workflow.Service.BusinessServices
{
    /// <summary>
    /// Defines utility methods related to validation exception.
    /// </summary>
    internal static class ValidationExceptionExtensions
    {
        /// <summary>
        /// Handles a validation exception.
        /// </summary>
        /// <param name="e">ValidationException.</param>
        internal static void HandleException(this ValidationException e)
        {
            if (e == null) return;
            LogWriterFactory.LogWriter.WriteEvent(EventSource.BusinessLayerValidation.ToString(),
                e.ErrorCode, System.Diagnostics.EventLogEntryType.Error, EventCategory.Analytic);
            throw new BusinessException(e.ErrorCode);
        }
    }
}
