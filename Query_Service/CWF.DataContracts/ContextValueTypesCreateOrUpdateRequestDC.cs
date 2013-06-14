using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.ServiceModel;


namespace CWF.DataContracts
{
    [DataContract]
    public class ContextValueTypesCreateOrUpdateRequestDC : RequestHeader
    {
        private int inId;
        private string inName;
        private string inType;
        private string inDescription;
        private string inInsertedByUserAlias;
        private string inUpdatedByUserAlias;

        [DataMember]
        public int InId
        {
            get { return inId; }
            set { inId = value; }
        }

        [DataMember]
        public string InName
        {
            get { return inName; }
            set { inName = value; }
        }

        [DataMember]
        public string InType
        {
            get { return inType; }
            set { inType = value; }
        }

        [DataMember]
        public string InDescription
        {
            get { return inDescription; }
            set { inDescription = value; }
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
