using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Service.DataAccessServices;
using CWF.DataContracts.Marketplace;

namespace Microsoft.Support.Workflow.Service.BusinessServices
{
    public static class MarketplaceBusinessService
    {
        public static MarketplaceSearchResult SearchMarketplace(MarketplaceSearchQuery request)
        {
            return Microsoft.Support.Workflow.Service.DataAccessServices.MarketplaceRepositoryService.SearchMarketplace(request);
        }

        /// <summary>
        /// This method invokes MarketplaceBusinessService.GetAssetDetails operation.
        /// </summary>
        /// <returns></returns>
        public static MarketplaceAssetDetails GetMarketplaceAssetDetails(MarketplaceSearchDetail request)
        {
            return Microsoft.Support.Workflow.Service.DataAccessServices.MarketplaceRepositoryService.GetMarketplaceAssetDetails(request);
        }
    }
}
