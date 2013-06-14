

namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;
    using System.Collections.Generic;


    [DataContract]
    public class AuthorizationGroupDC
    {
        [DataMember]
        public Guid Guid { get; set; }
        [DataMember]
        public int AuthGroupId { get; set; }
        [DataMember]
        public string AuthGroupName { get; set; }
    }

    [DataContract]
    public class AuthorizationGroupGetReplyDC : ReplyHeader
    {
        [DataMember]
        public IList<AuthorizationGroupDC> AuthorizationGroups { get; set; }
    }

    [DataContract]
    public class AuthorizationGroupGetRequestDC : RequestHeader
    {
        [DataMember]
        public string Guid { get; set; }
        [DataMember]
        public int AuthGroupId { get; set; }
        [DataMember]
        public string AuthGroupName { get; set; }
    }

}
