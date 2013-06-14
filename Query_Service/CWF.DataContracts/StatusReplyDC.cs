//-----------------------------------------------------------------------
// <copyright file="StatusReplyDC.cs" company="Microsoft">
// Copyright
// StatusReply DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Common StatusReply data contract used in inherited ReplyHeader
    /// </summary>
    [DataContract]
    public class StatusReplyDC
    {
        private int errorCode;
        private string errorMessage;
        private string errorGuid;
        private string output;

        /// <summary>
        /// Error code
        /// </summary>
        [DataMember]
        public int Errorcode 
        {
            get { return errorCode; }
            set { errorCode = value; } 
        }

        /// <summary>
        /// error message
        /// </summary>
        [DataMember]
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        /// <summary>
        /// Error guid
        /// </summary>
        [DataMember]
        public string ErrorGuid
        {
            get { return errorGuid; }
            set { errorGuid = value; }
        }

        [DataMember]
        public string Output
        {
            get { return output; }
            set { output = value; }
        }
    }
}
