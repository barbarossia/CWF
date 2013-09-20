using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CWF.DataContracts
{
    [DataContract]
    public class PermissionGetReplyDC 
    {
        [DataMember]
        public string AuthorGroupName { get; set; }
        [DataMember]
        public long Permission { get; set; }
        [DataMember]
        public string EnvironmentName { get; set; }
    }
}
