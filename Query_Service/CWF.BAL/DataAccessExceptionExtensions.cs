using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DAL;

namespace Microsoft.Support.Workflow.Service.BusinessServices
{
    internal static class DataAccessExceptionExtensions
    {
        internal static void HandleException(this DataAccessException e)
        {
            // Database exception is not logged here since the original issue
            // is logged in the data access layer before throwing DataAccessException.                
            throw new BusinessException(e.ErrorCode);
        }
    }
}
