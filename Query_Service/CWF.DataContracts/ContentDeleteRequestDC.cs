using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.ServiceModel;
//
//using Microsoft.Support.Workflow.Catalog;


namespace CWF.DataContracts
{
    [DataContract]
    public class ContentDeleteRequestDC :RequestHeader
    {
        private int inId;
        private Guid contentID;

        [DataMember]
        public int InId
        {
            get { return inId; }
            set { inId = value; }
        }

        [DataMember]
        public Guid ContentID
        {
            get { return contentID; }
            set { contentID = value; }
        }
    }
}
