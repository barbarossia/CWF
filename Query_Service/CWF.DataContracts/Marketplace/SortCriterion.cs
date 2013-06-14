using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CWF.DataContracts.Marketplace
{
    [DataContract]
    public class SortCriterion
    {
        /// <summary>
        /// Possible values:
        /// 1. UpdatedDate
        /// 2. Name
        /// 3. Version
        /// </summary>
        [DataMember]
        public string FieldName { get; set; }

        [DataMember]
        public bool IsAscending { get; set; }
    }
}
