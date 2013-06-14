namespace CWF.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class WorkflowTypeSearchReply:ReplyHeader
    {
        [DataMember]
        public IList<WorkflowTypeSearchDC> SearchResults { get; set; }

        [DataMember]
        public int ServerResultsLength { get; set; }
    }
}
