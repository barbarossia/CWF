using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.Support.Workflow.Authoring.Models.Marketplace
{
    public class MarketplaceSearchQuery
    {
        public string SearchText { get; set; }
        
        public MarketplaceFilter FilterType { get; set; }
        
        /// <summary>
        /// Admin, Author
        ///UserRole is used to decide whether or not “Templates” and “PublishingWorkflows” could be returned to the user.  Authors do not get to see them
        /// </summary>
        public string UserRole { get; set; }
        
        /// <summary>
        ///Page number of the page being requested.  First page has number 1 
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        ///Number of items per page 
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        ///Default value is Null. 
        ///Primary sort is performed based on the 0th value
        ///If more than one item is defined, secondary sorts are performed in the order of their appearance in this array
        ///Null value or no valid value in the list defaults to sort by Date (updated date) in descending order.
        /// </summary>
        public ObservableCollection<SortCriterion> SortCriterions { get; set; }
    }
}
