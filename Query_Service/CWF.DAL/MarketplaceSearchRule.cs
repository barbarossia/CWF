using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DataContracts.Marketplace;

namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    public class MarketplaceSearchRule
    {
        public MarketplaceFilter FilterType { get; set; }
        public string UserRole { get; set; }
        public int AssetType { get; set; }
        public bool? IsGetTemplates { get; set; }
        public bool? IsGetPublishingWorkflows { get; set; }

        public MarketplaceSearchRule(MarketplaceFilter filterType,
                                        string userRole,
                                        byte assetType,
                                        bool? isGetTemplates,
                                        bool? isGetPublishingWorkflows)
        {
            FilterType = filterType;
            UserRole = userRole;
            AssetType = assetType;
            IsGetTemplates = isGetTemplates;
            IsGetPublishingWorkflows = isGetPublishingWorkflows;
        }
    }
}
