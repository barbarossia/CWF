using System.Runtime.Serialization;

namespace CWF.DataContracts
{
    [DataContract]
    public class AuthGroupsEnableOrDisableRequestDC : RequestHeader
    {
        [DataMember]
        public string[] InAuthGroups { get; set; }
        [DataMember]
        public bool InEnabled { get; set; }
    }
}
