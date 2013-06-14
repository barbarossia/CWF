using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    /// <summary>
    /// Defines stored procedure parameter names.  These parameter names could be used by 
    /// multiple stored procedures.
    /// </summary>
    public static class StoredProcParamNames
    {
        public const string Id = "@Id";
        public const string Name = "@Name";
        public const string Version = "@Version";
    }
}
