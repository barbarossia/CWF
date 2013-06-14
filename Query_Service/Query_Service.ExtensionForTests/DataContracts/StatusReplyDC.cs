using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Query_Service.ExtensionForTests.DataProxy
{
    /// This class encapsulates the reply after executing a stored procedure
    [DataContract]
    public class StatusReplyDC
    {
        private int errorCode;
        private string errorMessage;

        [DataMember]
        public int Errorcode 
        {
            get { return errorCode; }
            set { errorCode = value; } 
        }

        [DataMember]
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }
    }
}
