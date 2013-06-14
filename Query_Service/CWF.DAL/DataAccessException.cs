using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.QueryService.Common;

namespace CWF.DAL
{
    /// <summary>
    /// Defines an exception to be thrown to report errors encountered in data access layer.  
    /// </summary>
    public class DataAccessException : BaseException
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataAccessException()
        { }

        /// <summary>
        /// Overloaded constructor.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        public DataAccessException(int errorCode)
            : base(errorCode, string.Empty)
        { }

        /// <summary>
        /// Overloaded constructor.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        public DataAccessException(int errorCode, string message)
            : base(errorCode, message)
        { }
    }
}
