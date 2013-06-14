using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.ServiceModel;

namespace CWF.DataContracts
{
    [DataContract]
    public class ContextValueTypesDeleteRequestDC : RequestHeader
    {
        private string inName;
        private string inShortName;

        [DataMember]
        public string InName
        {
            get { return inName; }
            set { inName = value; }
        }
        [DataMember]
        public string InShortName
        {
            get { return inShortName; }
            set { inShortName = value; }
        }
    }
}
