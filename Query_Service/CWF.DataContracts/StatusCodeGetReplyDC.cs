//-----------------------------------------------------------------------
// <copyright file="StatusCodeGetReplyDC.cs" company="Microsoft">
// Copyright
// StatusCode Get Reply DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Reply data contract for StatusCodeGet
    /// Status returned in ReplyHeader
    /// </summary>
    [DataContract]
    public class StatusCodeGetReplyDC : ReplyHeader
    {
        private IList<StatusCodeAttributes> list = null;

        /// <summary>
        /// List of StatusCode attributes
        /// </summary>
        [DataMember]
        public IList<StatusCodeAttributes> List
        {
            get { return list; }
            set { list = value; }
        }
    }
}
