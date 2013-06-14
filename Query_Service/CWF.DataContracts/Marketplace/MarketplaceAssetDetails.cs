using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CWF.DataContracts.Marketplace
{
    [DataContract]
    public class MarketplaceAssetDetails
    {
        /// <summary>
        /// ActivityLibraryId
        /// </summary>
        [DataMember] 
        public long Id { get; set; }

        [DataMember] 
        public string Name { get; set; }

        [DataMember] 
        public string Description { get; set; }

        [DataMember]
        public string MetaTages { get; set; }

        [DataMember]
        public string CategoryName { get; set; }

        [DataMember]
        public string ThumbnailUrl { get; set; }

        [DataMember]
        public List<ActivityQuickInfo> Activities { get; set; }
    }
}
