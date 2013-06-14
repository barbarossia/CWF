using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.ServiceModel;


namespace CWF.DataContracts
{
    [DataContract]
    public class mtblActivityContextCreateOrUpdateRequestDC : RequestHeader
    {
        private int inId;
		private string inActivityContextID;
		private string inActivityName;
		private string inContextName;
		private bool inIsRequired;
		private string inNotes;
        private int inContextDirection;
        private string inInsertedByUserAlias;
        private string inUpdatedByUserAlias;

        [DataMember]
        public int InId
        {
            get { return inId; }
            set { inId = value; }
        }

        [DataMember]
        public string InActivityContextID
        {
            get { return inActivityContextID; }
            set { inActivityContextID = value; }
        }

        [DataMember]
        public string InActivityName
        {
            get { return inActivityName; }
            set { inActivityName = value; }
        }

        [DataMember]
        public string InContextName
        {
            get { return inContextName; }
            set { inContextName = value; }
        }

        [DataMember]
        public bool InIsRequired
        {
            get { return inIsRequired; }
            set { inIsRequired = value; }
        }

        [DataMember]
        public string InNotes
        {
            get { return inNotes; }
            set { inNotes = value; }
        }

        [DataMember]
        public int InContextDirection
        {
            get { return inContextDirection; }
            set { inContextDirection = value; }
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
