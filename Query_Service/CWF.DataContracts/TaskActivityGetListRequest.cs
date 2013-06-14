namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;
    using System.Collections.Generic;

    public class TaskActivityGetListRequest : RequestHeader
    {
        private List<TaskActivityDC> list;

        /// <summary>
        /// List of ActivityLibraryDC
        /// </summary>
        [DataMember]
        public List<TaskActivityDC> List
        {
            get { return list; }
            set { list = value; }
        }
    }

    public class TaskActivityGetListReply : ReplyHeader 
    {
        private List<TaskActivityDC> list;

        /// <summary>
        /// List of ActivityLibraryDC
        /// </summary>
        [DataMember]
        public List<TaskActivityDC> List
        {
            get { return list; }
            set { list = value; }
        }
    }
}
