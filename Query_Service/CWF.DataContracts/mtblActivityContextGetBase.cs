using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.ServiceModel;

namespace CWF.DataContracts
{
    [DataContract]
    public class mtblActivityContextGetBase
    {
        private int id;
        private Guid guid;
        private int contextId;
        private string contextName;
        private int storeActivitiesId;
        private string storeActivitiesName;
        private bool isRequired;
        private string notes;
        private int contextDirection;

        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public Guid Guid
        {
            get { return guid; }
            set { guid = value; }
        }
        
        [DataMember]
        public int ContextId
        {
            get { return contextId; }
            set { contextId = value; }
        }

        [DataMember]
        public string ContextName
        {
            get { return contextName; }
            set { contextName = value; }
        }

        [DataMember]
        public int StoreActivitiesId
        {
            get { return storeActivitiesId; }
            set { storeActivitiesId = value; }
        }

        [DataMember]
        public string StoreActivitiesName
        {
            get { return storeActivitiesName; }
            set { storeActivitiesName = value; }
        }

        [DataMember]
        public bool IsRequired
        {
            get { return isRequired; }
            set { isRequired = value; }
        }

        [DataMember]
        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }
        [DataMember]
        public int ContextDirection
        {
            get { return contextDirection; }
            set { contextDirection = value; }
        }
    }
}
