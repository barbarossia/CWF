using System.Runtime.Serialization;

namespace CWF.DataContracts
{
    [DataContract]
    public class AuthGroupsCreateOrUpdateRequestDC : RequestHeader
    {
        [DataMember]
        public string[] InAuthGroups { get; set; }
        [DataMember]
        public int RoleId { get; set; }

    }
}
