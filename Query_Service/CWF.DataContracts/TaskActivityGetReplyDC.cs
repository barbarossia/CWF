namespace CWF.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class TaskActivityGetReplyDC : ReplyHeader
    {
        /// <summary>
        /// List of ActivityLibraryDC
        /// </summary>
        [DataMember]
        public List<TaskActivityDC> List { get; set; }

        [DataMember]
        public int ServerResultsLength { get; set; }

    }
}
