using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Query_Service.ExtensionForTests.DataProxy
{
    /// <summary>
    /// This class encapsulates the parameters need to clear lock for activity
    /// </summary>
    [DataContract]
    public class ClearLockRequestDC : RequestHeader
    {
        private string name;
        private string version;

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public string Version
        {
            get { return version; }
            set { version = value; }
        }
    }
}