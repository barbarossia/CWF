using System.Runtime.Serialization;

namespace CWF.DataContracts
{
    [DataContract]
    public class WorkflowReplayHeader : ReplyHeader
    {
        /// <summary>
        /// The environment of activity
        /// </summary>
        [DataMember]
        public string Environment { get; set; }
    }
}
