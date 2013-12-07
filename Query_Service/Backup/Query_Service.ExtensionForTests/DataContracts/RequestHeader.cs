using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace Query_Service.ExtensionForTests.DataProxy
{
    /// <summary>
    /// This class encapsulates parameters needed to call stored procedures
    /// </summary>
    [DataContract]
    public class RequestHeader
    {
        private string incaller;
        private string incallerVersion;

        [DataMember]
        [NotNullValidator(MessageTemplate = "1|{1}| is null")] 
        public string Incaller
        {
            get { return incaller; }
            set { incaller = value; }
        }

        [DataMember]
        [NotNullValidator(MessageTemplate = "2|{1}| is null")] 
        public string IncallerVersion
        {
            get { return incallerVersion; }
            set { incallerVersion = value; }
        }
    }
}
