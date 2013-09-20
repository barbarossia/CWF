using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CWF.DataContracts.Marketplace
{
    [DataContract]
    public class MarketplaceSearchQuery : RequestHeader
    {
        [DataMember]
        public string SearchText { get; set; }

        [DataMember]
        public MarketplaceFilter FilterType { get; set; }
                
        /// <summary>
        /// Admin, Author
        /// </summary>
        [DataMember]
        public string UserRole { get; set; }

        [DataMember]
        /// <summary>
        /// Page number of the page being requested.  
        /// First page has number 1.
        /// </summary>
        public int PageNumber { get; set; }

        [DataMember]
        /// <summary>
        /// Number of items per page.
        /// </summary>
        public int PageSize { get; set; }

        [DataMember]
        /// <summary>
        ///Default value is Null.
        ///Primary sort is performed based on the 0th value.
        ///If more than one item is defined, secondary sorts are performed in the order of their appearance in this array.
        ///Null value or no valid value in the list defaults to sort by Date (updated date) in descending order.
        /// </summary>
        public List<SortCriterion> SortCriteria { get; set; }

        [DataMember]
        public bool IsNewest { get; set; }

    }
}
