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
    public class RequestReplyCommonHeader
    {
        private string incaller;
        private string incallerVersion;
        private string insertedByUserAlias;
        private string updatedByUserAlias;
        private StatusReplyDC statusReply;

        public RequestReplyCommonHeader()
        {
            statusReply = new StatusReplyDC();
            statusReply.Errorcode = 0;
            statusReply.ErrorGuid = string.Empty;
            statusReply.ErrorMessage = string.Empty;
        }

        /// <summary>
        /// <para>Incaller is used in the error log to associate the caller alias with the request - Required </para>
        /// <para>CreateOrUpdate </para>
        /// <para>&#160;&#160;&#160;&#160;Request - Required </para>
        /// <para>&#160;&#160;&#160;&#160;Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - Required </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - Required </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- will be included as a return column </para>
        /// </summary>
        //// [NotNullValidator(MessageTemplate = "1|{1}| is null")]
        [DataMember]
        public string Incaller
        {
            get { return incaller; }
            set { incaller = value; }
        }

        /// <summary>
        /// <para>IncallerVersion is used in the error log to associate the caller alias with the request - Required </para>
        /// <para>CreateOrUpdate </para>
        /// <para>&#160;&#160;&#160;&#160;Request - Required </para>
        /// <para>&#160;&#160;&#160;&#160;Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - Required </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - Required </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- will be included as a return column </para>
        //// [NotNullValidator(MessageTemplate = "2|{1}| is null")]
        [DataMember]        
        public string IncallerVersion
        {
            get { return incallerVersion; }
            set { incallerVersion = value; }
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
        /// <summary>
        /// <para>user alias that inserted this row</para>
        /// <para>CreateOrUpdate </para>
        /// <para>&#160;&#160;&#160;&#160;Request - Required </para>
        /// <para>&#160;&#160;&#160;&#160;Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a</para>
        /// <para>&#160;&#160;&#160;&#160;Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string InsertedByUserAlias
        {
            get { return insertedByUserAlias; }
            set { insertedByUserAlias = value; }
        }

        /// <summary>
        /// <para>user alias that updated this row</para>
        /// <para>CreateOrUpdate </para>
        /// <para>&#160;&#160;&#160;&#160;Request - Required </para>
        /// <para>&#160;&#160;&#160;&#160;Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a</para>
        /// <para>&#160;&#160;&#160;&#160;Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string UpdatedByUserAlias
        {
            get { return updatedByUserAlias; }
            set { updatedByUserAlias = value; }
        }
    }
}
