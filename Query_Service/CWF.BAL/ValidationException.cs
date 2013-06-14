using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.QueryService.Common;

namespace Microsoft.Support.Workflow.Service.BusinessServices
{
    /// <summary>
    /// Defines an exception to be thrown to report validation issues encountered in business layer.  
    /// This exception is used internally to report validation issues, and not thrown to the caller.
    /// The business layer should catch this exception and throw BusinessException to the caller.
    /// </summary>
    internal class ValidationException : BaseException
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal ValidationException()
        { }

        /// <summary>
        /// Overloaded constructor.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        internal ValidationException(int errorCode)
            : base(errorCode, string.Empty)
        { }

        /// <summary>
        /// Overloaded constructor.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        internal ValidationException(int errorCode, string message)
            : base(errorCode, message)
        { }
    }
}
