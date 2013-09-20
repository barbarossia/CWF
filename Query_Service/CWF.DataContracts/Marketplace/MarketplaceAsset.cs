using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CWF.DataContracts.Marketplace
{
    [DataContract]
    public class MarketplaceAsset : WorkflowReplayHeader
    {
        /// <summary>
        /// ActivityLibraryId
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public string UpdatedBy { get; set; }

        [DataMember]
        public DateTime UpdatedDate { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public AssetType AssetType { get; set; }

        [DataMember]
        public bool? IsTemplate { get; set; }

        [DataMember]
        public bool? IsPublishingWorkflow { get; set; }
    }
}
