using System;
using Microsoft.Support.Workflow.QueryService.Common;

namespace Microsoft.Support.Workflow.Service.BusinessServices
{
    /// <summary>
    /// Defines an exception to be thrown to report errors encountered in business layer.  
    /// </summary>
    public class BusinessException : BaseException
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BusinessException()
        { }

        /// <summary>
        /// Overloaded constructor.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        public BusinessException(int errorCode)
            : base(errorCode, string.Empty)
        { }

        /// <summary>
        /// Overloaded constructor.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        public BusinessException(int errorCode, string message)
            : base(errorCode, message)
        { }
    }
}
