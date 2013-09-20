using System.Runtime.Serialization;

namespace CWF.DataContracts
{
    [DataContract]
    public class ActivityCopyRequest : ActivityRequest
    {
        [DataMember]
        public string NewName { get; set; }
    }
}
