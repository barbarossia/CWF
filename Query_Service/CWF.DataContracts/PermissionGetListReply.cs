using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CWF.DataContracts
{
    [DataContract]
    public class PermissionGetListReply : ReplyHeader
    {
        [DataMember]
        public IList<PermissionGetReplyDC> List { get; set; }
    }
}
