using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.Support.Workflow.Authoring.Models.Marketplace
{
    public class MarketplaceSearchResult
    {
        
        /// <summary>
        /// Number of items in this array is less than or equal to the PageSize defined in the MarketplaceSearchQuery
        /// </summary>
        public ObservableCollection<MarketplaceAsset> Items = new ObservableCollection<MarketplaceAsset>();
        
        /// <summary>
        /// Current page number.  
        /// If the user requested the page 10 and there were actually only 5 pages of data, 
        /// the user will be presented with the 5th page.  
        /// So it is useful to know which page of data is being actually returned irrespective of what page the user requested.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// This is an echo of the requested page size and may not be really needed 
        /// since the user knows the requested page size and the service do not change it
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of pages of data available in the server that matches the specified search and filter criteria
        /// </summary>
        public int PageCount { get; set; }
    }
}
