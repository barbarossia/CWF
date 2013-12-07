using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow
{
    public interface IWorkflowStoreItem
    {
        /// <summary>
        /// Added during conversion from EDm to ER model DB
        /// </summary>
        Int32 NEWId { get; set; }

        /// <summary>
        /// Old EDm model
        /// </summary>
        Guid ID { get; set; }

        string Name { get; set; }
        Version Version { get; set; }
        //MetaTags MetaTags { get; set; }
        Status Status { get; set; }
        //Workflow_Activity_Type Type { get; set; }
        
    }
}
