namespace CWF.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Get all Activity library replyDC
    /// </summary>
    [DataContract]
    public class TaskActivityGetRequestDC : RequestHeader
    {
        [DataMember]
        public Guid TaskActivityGuid { get; set; }

        [DataMember]
        public string AssignedTo { get; set; }

        [DataMember]
        public bool FilterOlder { get; set; }

        [DataMember]
        public bool IncludeDetails { get; set; }

        [DataMember]
        public bool HideUnassignedTasks { get; set; }

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
