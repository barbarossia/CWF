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
    public class ContentGetReplyDC : ReplyHeader
    {
        private int id;
        private Guid contentId;
        private string text;
        private string keywords;

        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        [DataMember]
        public Guid ContentId
        {
            get { return contentId; }
            set { contentId = value; }
        }
        [DataMember]
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        [DataMember]
        public string Keywords
        {
            get { return keywords; }
            set { keywords = value; }
        }
    }
}
