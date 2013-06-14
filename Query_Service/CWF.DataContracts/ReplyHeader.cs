//-----------------------------------------------------------------------
// <copyright file="ReplyHeader.cs" company="Microsoft">
// Copyright
// StatusReply DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Common reply header
    /// </summary>
     [DataContract]
    public class ReplyHeader
    {
         private StatusReplyDC statusReply;

         /// <summary>
         /// Initializes a new instance of the ReplyHeader class
         /// </summary>
         public ReplyHeader()
        {
            statusReply = new StatusReplyDC();
            statusReply.Errorcode = 0;
            statusReply.ErrorGuid = string.Empty;
            statusReply.ErrorMessage = string.Empty;
        }

         /// <summary>
         /// Statusreply record
         /// </summary>
        [DataMember]
         public StatusReplyDC StatusReply
        {
            get { return statusReply; }
            set { statusReply = value; }
        }
    }
}
