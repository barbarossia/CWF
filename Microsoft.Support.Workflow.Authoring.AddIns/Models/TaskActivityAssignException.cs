using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Models
{
    [Serializable]
    public class TaskActivityAssignException : Exception
    {
        public TaskActivityAssignException() : base() { }
        public TaskActivityAssignException(string msg) : base(msg)
        {
        }

        public TaskActivityAssignException(string msgFmt, params string[] msgParams)
            : base(string.Format(msgFmt, msgParams))
        {
        }

        public TaskActivityAssignException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}
