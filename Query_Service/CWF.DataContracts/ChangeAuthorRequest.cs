using System.Runtime.Serialization;

namespace CWF.DataContracts
{
    [DataContract]
    public class ChangeAuthorRequest : WorkflowRequestHeader
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string AuthorAlias { get; set; }
    }
}
