using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CWF.WorkflowQueryService.UserError.Config;

namespace CWF.WorkflowQueryService
{
    /// <summary>
    /// Defines a user error.
    /// </summary>
    public struct UserErrorInfo
    {
        /// <summary>
        /// User error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Fault type associated with user error.
        /// </summary>
        public FaultType FaultType { get; set; }

        /// <summary>
        /// Default user error message.
        /// </summary>
        public const string DefaultMessage = "An error occurred.";

        /// <summary>
        /// Gets the user error info associated with an error number.
        /// </summary>
        /// <param name="errorNumber">Error number.</param>
        /// <returns>User error info.</returns>
        public static UserErrorInfo GetUserErrorInfo(int errorNumber)
        {
            UserErrorConfigSection messageConfig = UserErrorConfigSection.Current;

            if (messageConfig == null || messageConfig.Errors == null || messageConfig.Errors[errorNumber] == null)
            {
                return new UserErrorInfo { Message = DefaultMessage, FaultType = FaultType.ServiceFault };
            }

            FaultType faultType;
            Enum.TryParse<FaultType>(messageConfig.Errors[errorNumber].FaultType, true, out faultType);
            return new UserErrorInfo { Message = messageConfig.Errors[errorNumber].UserMessage, FaultType = faultType };
        }
    }
}