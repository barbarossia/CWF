using System.Runtime.Serialization;

namespace CWF.DataContracts
{
    [DataContract]
    public class WorkflowRequestReplayHeader : RequestReplyCommonHeader
    {
        /// <summary>
        /// Set the target environment
        /// </summary>
        [DataMember]
        public string Environment { get; set; }
    }
}
