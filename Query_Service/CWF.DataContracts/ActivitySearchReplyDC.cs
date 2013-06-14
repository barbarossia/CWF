namespace CWF.DataContracts
{
    using System.Runtime.Serialization;
    using CWF.DataContracts;
    using System.Collections.Generic;
    
    [DataContract]
    public class ActivitySearchReplyDC
    {
        [DataMember]
        public List<StoreActivitiesDC> SearchResults { get; set; }

        [DataMember]
        public int ServerResultsLength { get; set; }
    }
}