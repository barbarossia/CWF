namespace CWF.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class WorkflowTypeSearchRequest:RequestReplyCommonHeader
    {
        [DataMember]
        public string SearchText { get; set; }

        [DataMember]
        public string SortColumn { get; set; }

        [DataMember]
        public bool SortAscending { get; set; }

        [DataMember]
        public int PageSize { get; set; }

        [DataMember]
        public int PageNumber { get; set; }
    }
        
}
