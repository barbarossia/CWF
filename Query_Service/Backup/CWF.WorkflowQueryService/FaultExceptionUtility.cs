using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;
using Microsoft.Support.Workflow.Service.Common;

namespace CWF.WorkflowQueryService
{
    /// <summary>
    /// Provides a utility to map error code with fault exceptions.
    /// </summary>
    public static class FaultExceptionUtility
    {
        /// <summary>
        /// Gets a fault exception based on the error code.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <returns>Fault exception.</returns>
        public static FaultException GetFaultException(int errorCode)
        {
            UserErrorInfo errorInfo = UserErrorInfo.GetUserErrorInfo(errorCode);

            if (errorInfo.FaultType ==  FaultType.ValidationFault)
            {
                return new FaultException<ValidationFault>(new ValidationFault { ErrorCode = errorCode, ErrorMessage = errorInfo.Message });
            }
            
            if (errorInfo.FaultType == FaultType.PublishingFault)
            {
                return new FaultException<PublishingFault>(new PublishingFault { ErrorCode = errorCode, ErrorMessage = errorInfo.Message });
            }
            
            return new FaultException<ServiceFault>(new ServiceFault { ErrorCode = errorCode, ErrorMessage = errorInfo.Message });            
        }
    }
}