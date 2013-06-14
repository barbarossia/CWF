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
    public class ContentCreateOrUpdateRequestDC : RequestHeader
    {
        private int inId;
        private Guid inContentId;
        private string inText;
        private string inKeywords;
        private string inInsertedByUserAlias;
        private string inUpdatedByUserAlias;

        [DataMember]
        public int InId
        {
            get { return inId; }
            set { inId = value; }
        }

        [DataMember]
        public Guid InContentId
        {
            get { return inContentId; }
            set { inContentId = value; }
        }

        [DataMember]
        public string InText
        {
            get { return inText; }
            set { inText = value; }
        }

        [DataMember]
        public string InKeywords
        {
            get { return inKeywords; }
            set { inKeywords = value; }
        }

        [DataMember]
        public string InInsertedByUserAlias
        {
            get { return inInsertedByUserAlias; }
            set { inInsertedByUserAlias = value; }
        }

        [DataMember]
        public string InUpdatedByUserAlias
        {
            get { return inUpdatedByUserAlias; }
            set { inUpdatedByUserAlias = value; }
        }
    }
}
