using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.ServiceModel;


namespace CWF.DataContracts
{
    [DataContract]
    public class mtblActivityContextByIdDeleteRequestDC : RequestHeader
    {
        private int inId;

        [DataMember]
        public int InId
        {
            get { return inId; }
            set { inId = value; }
        }
    }
}
