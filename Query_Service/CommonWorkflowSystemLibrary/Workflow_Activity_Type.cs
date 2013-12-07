using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Microsoft.Support.Workflow
{
    [Serializable]
    [DataContract]
    public class Workflow_Activity_Type
    {
        /// <summary>
        /// The following two datamembers are added as a result of converting from the EDM to the ER DB model
        /// </summary>
        [DataMember]
        public Guid NEWWorkflowTypeid { get; set; }
        [DataMember]
        public Int32 NEWId { get; set; }

        /// <summary>
        /// Id is only valid in the EDM model
        /// </summary>
        [DataMember]
        public Guid id { get; set; }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Group { get; set; }
        [DataMember]
        public string PublishingWorkflow { get; set; }
        [DataMember]
        public string WorkflowTemplate { get; set; }
        [DataMember]
        public string SelectionWorkflow { get; set; }
        [DataMember]
        public AuthorityGroup AuthorityGroup { get; set; }
        [DataMember]
        public string ContextVariable { get; set; }
        [DataMember]
        public string HandleVariable { get; set; }
        [DataMember]
        public string PageViewVariable { get; set; }

        public Workflow_Activity_Type(string name, string group, string managementWorkflow)
        {
            id = Guid.NewGuid();
            Group = group;
            Name = name;
            PublishingWorkflow = managementWorkflow;
        }

        public Workflow_Activity_Type()
        {
            //// TODO: Complete member initialization
            id = Guid.NewGuid();
        }

        public static Workflow_Activity_Type Get(string name)
        {
            //todo replace with db lookup
            throw new NotImplementedException();

        }

        
    }
}
