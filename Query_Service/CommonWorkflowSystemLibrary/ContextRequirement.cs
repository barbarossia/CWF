using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Microsoft.Support.Workflow
{
    [DataContract]
    public enum ContextDirection
    {
        [EnumMember]
        In,
        [EnumMember]
        Out,
        [EnumMember]
        InOut
    }

    [DataContract]
    public class ContextRequirement
    {
        /// <summary>
        /// Added during conversion of EDm to ER model DB
        /// </summary>
        [DataMember]
        public Guid NEWActivityContextId {get; set;}
        [DataMember]
        public Int32 NEWId { get; set; }
        
        /// <summary>
        /// Old EDm model
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        
        [DataMember]
        public xxContextItem Context { get; set; }
        [DataMember]
        public bool Required { get; set; }
        [DataMember]
        public ContextDirection Direction { get; set; }
        [DataMember]
        public string Notes { get; set; }

       
    }
}
