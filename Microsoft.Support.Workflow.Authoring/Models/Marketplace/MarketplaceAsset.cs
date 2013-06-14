using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.Models.Marketplace
{
    /// <summary>
    /// Defines the properties needed to return the information in an asset search result response.
    /// </summary>
    public class MarketplaceAsset
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Version Version { get; set; }
        public AssetType AssetType { get; set; }
        public bool IsTemplate { get; set; }
        public bool IsPublishingWorkflow { get; set; }

        public MarketplaceAsset() { }
    }
}
