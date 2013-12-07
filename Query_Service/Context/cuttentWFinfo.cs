using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Context
{
    public class CurrentWFinfo
    {
        public string BookmarkName { get; set; }
        
        
        private string bookmarkName;
        /// <summary>
        /// The unique identifier for the workflow instance used for the correlation handling (i.e. identifying the workflow instance to be reconstituted when control is returned to the workflow engine.
        /// </summary>
        public Guid WfId { get; set; }
        /// <summary>
        /// The current activity that is being executed in the workflow.  Returned to the calling application to that application can determine the action it should take with the returned context.
        /// </summary>
        public string CurActivityName { get; set; }
      
    }
}
