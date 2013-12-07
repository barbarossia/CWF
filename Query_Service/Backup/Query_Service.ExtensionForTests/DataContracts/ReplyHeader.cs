using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.ServiceModel;

namespace Query_Service.ExtensionForTests.DataProxy
{

    /// <summary>
    /// This class encapsulates the reply after executing a stored procedure
    /// </summary>
    [DataContract]
    public class ReplyHeader
    {
        private StatusReplyDC statusReply;

        public ReplyHeader()
        {
            statusReply = new StatusReplyDC();
            statusReply.Errorcode = 0;
            statusReply.ErrorMessage = string.Empty;
        }

        [DataMember]
        public StatusReplyDC StatusReply
        {
            get { return statusReply; }
            set { statusReply = value; }
        }
    }
}
