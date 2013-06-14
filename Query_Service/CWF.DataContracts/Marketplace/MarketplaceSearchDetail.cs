using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CWF.DataContracts.Marketplace
{
    [DataContract]
    public class MarketplaceSearchDetail
    {
        [DataMember]
        /// <summary>
        /// which is the same as the corresponding ActivityLibraryId.  
        /// </summary>
        public long Id { get; set; }

        [DataMember]
        public AssetType AssetType { get; set; }
    }
}
