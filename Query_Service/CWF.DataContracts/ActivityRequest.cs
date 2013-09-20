using System.Runtime.Serialization;

namespace CWF.DataContracts
{
    [DataContract]
    public class ActivityRequest : WorkflowRequestHeader
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public long WorkflowTypeId { get; set; }
        [DataMember]
        public string EnvironmentTarget { get; set; }
    }
}
