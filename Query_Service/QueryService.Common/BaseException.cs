using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.QueryService.Common
{
    /// <summary>
    /// Defines a base exception for this service.  Specific exceptions for data access 
    /// layer, business layer etc are derived from this.
    /// </summary>
    public abstract class BaseException : ApplicationException
    { 
        /// <summary>
        /// Gets or sets the error code associated with the exception.
        /// </summary>
        public int ErrorCode { get; set; }
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseException()
        { }

        /// <summary>
        /// Overloaded constructor.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        public BaseException(int errorCode, string message)
            : base(message)
        {
            this.ErrorCode = errorCode;
        }
    }
}
