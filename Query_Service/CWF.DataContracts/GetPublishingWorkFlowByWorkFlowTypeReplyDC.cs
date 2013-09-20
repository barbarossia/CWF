//-----------------------------------------------------------------------
// <copyright file="GetPublishingWorkFlowByWorkFlowTypeReplyDC.cs" company="Microsoft">
// Copyright
// StatusReply DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Reply from Get Publishing WorkFlow By WorkFlow Type
    /// </summary>
    [DataContract]
    public partial class GetPublishingWorkFlowByWorkFlowTypeReplyDC : ReplyHeader
    {
        private byte[] publishingDll;

        /// <summary>
        /// Publishing dll code
        /// </summary>
        [DataMember]
        public byte[] PublishingDll
        {
            get { return publishingDll; }
            set { publishingDll = value; }
        }
    }
}
