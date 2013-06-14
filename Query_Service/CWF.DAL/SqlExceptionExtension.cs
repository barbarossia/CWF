using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Support.Workflow.QueryService.Common;
using Microsoft.Support.Workflow.Service.Common.Logging;
using CWF.DAL;
using Microsoft.Support.Workflow.Service.Common;

namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    internal static class SqlExceptionExtensions
    {
        private const string errorNumberStartTag = "<ErrorNumber>";
        private const string errorNumberEndTag = "</ErrorNumber>";

        /// <summary>
        /// Gets the original error number associated with a SqlException.
        /// Error_Handle stored procedure is used to handle the database errors in the CATCH
        /// block of a DAL facing stored procedure.  Error_Handle stored procedure re-raises
        /// this error with the original error message.  When an error is raised with a message 
        /// using RAISERROR, it is not possible to specify an error number, it defaults to 50000
        /// instead.  Even if we wanted to use the RAISERROR overload that sends an error number
        /// it has to be a custom error number - above 50000 - which we have created 
        /// specifically for our application.  Error numbers below 50000 which are system
        /// errors cannot be re-raised with original error number.
        /// Due to this restriction of SQL Server 2008 R2, we append the error number to the beginning
        /// of the ErrorMessage.  The error number is defined inside an XML style node named ErrorNumber
        /// to easily identify the error number from the message.
        /// This method attempts to extract that specific error number from the error message of 
        /// SqlException.
        /// </summary>
        /// <param name="e">SqlException.</param>
        /// <returns>Original error number.</returns>
        private static int GetOriginalErrorNumber(this SqlException e)
        {            
            int errorNumber = e.Number;
            if ((e.Number == EventCode.DatabaseEvent.Error.DefaultSqlError)
                && (!string.IsNullOrWhiteSpace(e.Message) && e.Message.StartsWith(errorNumberStartTag)))
            {
                // Error number is embedded into the message, extract the error number from message.
                string errorNumberStr = e.Message.Substring(errorNumberStartTag.Length, e.Message.IndexOf(errorNumberEndTag) - errorNumberStartTag.Length);

                Int32.TryParse(errorNumberStr, out errorNumber);
            }
            return errorNumber;
        }

        /// <summary>
        /// Handles a SqlException.  
        /// </summary>
        /// <param name="e">SqlException.</param>
        internal static void HandleException(this SqlException e)
        {
            int errorNumber = e.GetOriginalErrorNumber();

            if (LogWriterFactory.LogWriter != null)
            {
                if (errorNumber >= EventCodeRange.DatabaseErrorStart && errorNumber <= EventCodeRange.DatabaseErrorEnd)
                {
                    LogWriterFactory.LogWriter.WriteEvent(EventSource.DatabaseError.ToString(), errorNumber, System.Diagnostics.EventLogEntryType.Error, EventCategory.Operational);
                }
                else if (errorNumber >= EventCodeRange.DatabaseValidationStart && errorNumber <= EventCodeRange.DatabaseValidationEnd)
                {
                    LogWriterFactory.LogWriter.WriteEvent(EventSource.DatabaseValidation.ToString(), errorNumber, System.Diagnostics.EventLogEntryType.Information, EventCategory.Debug);
                }
                else
                {
                    LogWriterFactory.LogWriter.WriteException(EventSource.DatabaseValidation.ToString(), errorNumber, System.Diagnostics.EventLogEntryType.Error, EventCategory.Operational, e);
                }
            }

            throw new DataAccessException(errorNumber);
        }
    }
}
