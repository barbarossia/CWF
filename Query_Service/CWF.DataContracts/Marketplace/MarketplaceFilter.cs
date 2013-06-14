using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CWF.DataContracts.Marketplace
{
    /// <summary>
    /// The filter for sarch marketplace
    /// </summary>
    public enum MarketplaceFilter
    {
        /// <summary>
        /// No filters
        /// </summary>
        None=0,

        /// <summary>
        /// Returns all records with "openable" projects excluding Templates and Publishing Workflows
        /// </summary>
        Projects=1,

        /// <summary>
        /// Return all records with downloadable, compiled library
        /// </summary>
        Activities=2,

        /// <summary>
        /// [Available for admin only] returns "openable" projects that are Templates
        /// </summary>
        Templates=3,

        /// <summary>
        /// [Available for admin only] returns "openable" projects that are Publishing Workflows
        /// </summary>
        PublishingWorkflows=4,
    }
}
