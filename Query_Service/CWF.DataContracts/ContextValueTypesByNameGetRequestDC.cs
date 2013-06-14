using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.ServiceModel;



namespace CWF.DataContracts
{
    [DataContract]
    public class ContextValueTypesByNameGetRequestDC : RequestHeader
    {
        private string inName;

        [DataMember]
        public string InName
        {
            get { return inName; }
            set { inName = value; }
        }
    }
}
