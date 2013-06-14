using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.ServiceModel;


namespace CWF.DataContracts
{
    [DataContract]
    public class mtblActivityContextGetRequestDC : RequestHeader
    {
        private int inId;
        private string inGuid;

        [DataMember]
        public int InId
        {
            get { return inId; }
            set { inId = value; }
        }

        [DataMember]
        public string InGuid
        {
            get { return inGuid; }
            set { inGuid = value; }
        }
    }
}
