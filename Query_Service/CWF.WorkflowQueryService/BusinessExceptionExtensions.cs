using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using CWF.BAL;
using Microsoft.Support.Workflow.Service.BusinessServices;

namespace CWF.WorkflowQueryService
{
    /// <summary>
    /// Defines extension methods for BusinessException class.
    /// </summary>
    internal static class BusinessExceptionExtensions
    {
        /// <summary>
        /// Handles a BusinessException.
        /// </summary>
        /// <param name="e">BusinessException.</param>
        internal static void HandleException(this BusinessException e)
        {
            // BusinessException is not logged here since the original issue
            // is logged in the business layer before throwing.
            throw FaultExceptionUtility.GetFaultException(e.ErrorCode);
        }
    }
}