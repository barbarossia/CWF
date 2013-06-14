using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.Common
{
   public enum WorkflowTypeOperations
    {
        /// <summary>
        /// do nothing
        /// </summary>
        DoNothing = 0,
        /// <summary>
        ///add new workflow
        /// </summary>
        Add = 1,
        /// <summary>
        /// eidt existing workflow
        /// </summary>
        Edit = 2,
    }
}
