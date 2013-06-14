namespace CWF.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class WorkflowTypeSearchDC : WorkflowTypesGetBase
    {
        [DataMember]
        public int WorkflowsCount { get; set; }
    }
}
