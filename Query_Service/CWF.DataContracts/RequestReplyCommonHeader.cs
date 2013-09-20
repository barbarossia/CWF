//-----------------------------------------------------------------------
// <copyright file="RequestReplyCommonHeader.cs" company="Microsoft">
// Copyright
// Request and Reply Common Header
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Common request/reply header used only in activityLibraries and StoreActivity data contracts
    /// </summary>
    [DataContract]
    public class RequestReplyCommonHeader : RequestHeader
    {
        private StatusReplyDC statusReply;

        public RequestReplyCommonHeader()
        {
            statusReply = new StatusReplyDC();
            statusReply.Errorcode = 0;
            statusReply.ErrorGuid = string.Empty;
            statusReply.ErrorMessage = string.Empty;
        }
       
        /// <summary>
        /// <para>StatusReply is used to transmit detailed status information from the sproc back to the service. It is returned as a part of the reply </para>
        /// <para>CreateOrUpdate </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a </para>
        /// <para>&#160;&#160;&#160;&#160;Reply - required </para>
        /// <para>Delete  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- Required </para>
        /// <para>Get  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- Required </para>
        /// </summary>
        //// [NotNullValidator(MessageTemplate = "2|{1}| is null")]
        [DataMember]
        public StatusReplyDC StatusReply
        {
            get { return statusReply; }
            set { statusReply = value; }
        }
    }
}
