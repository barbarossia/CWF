using System.Runtime.Serialization;

namespace CWF.DataContracts
{
    [DataContract]
    public class WorkflowSearchRequestHeader : RequestReplyCommonHeader
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

        [DataMember]
        public string[] Environments { get; set; }
    }
}
