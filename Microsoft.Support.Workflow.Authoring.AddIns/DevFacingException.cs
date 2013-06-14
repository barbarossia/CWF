using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.AddIns
{
    /// <summary>
    /// Throw this exception for non-user-correctable error conditions, e.g. bad data in the database. Exception details will be logged or something but not exposed to user.
    /// </summary>
    [Serializable]
    public class DevFacingException : Exception
    {
        public DevFacingException(string msg) : base(msg)
        {

        }
    }
}
