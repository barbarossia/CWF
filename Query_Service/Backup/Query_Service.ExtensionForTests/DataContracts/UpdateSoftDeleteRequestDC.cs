using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Query_Service.ExtensionForTests.DataProxy
{
    /// <summary>
    /// This class encapsulates the parameters needed to call the stored procedure UpdateSoftDelete
    /// </summary>
    [DataContract]
    public class UpdateSoftDeleteRequestDC:RequestHeader
    {
        private string id;
        private string tableName;

        [DataMember]
        public string Id 
        {
            get { return id; }
            set {id = value;} 
        }

        [DataMember]
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
    }
}
