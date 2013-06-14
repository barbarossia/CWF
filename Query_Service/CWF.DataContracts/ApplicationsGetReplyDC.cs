//-----------------------------------------------------------------------
// <copyright file="ApplicationsGetReplyDC.cs" company="Microsoft">
// Copyright
// Applications Get Repl yDC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Reply data contract for ApplicationsGet
    /// Status in ReplyHeader
    /// </summary>
    [DataContract]
    public class ApplicationsGetReplyDC : ReplyHeader
    {
        private IList<ApplicationGetBase> list;

        /// <summary>
        /// List of applications
        /// </summary>
        [DataMember]
        public IList<ApplicationGetBase> List
        {
            get { return list; }
            set { list = value; }
        }
    }
}
