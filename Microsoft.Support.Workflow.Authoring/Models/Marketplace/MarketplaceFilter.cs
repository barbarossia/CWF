using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Microsoft.Support.Workflow.Authoring.Models.Marketplace
{
    /// <summary>
    /// Enumeration with following values
    /// </summary>
    public enum MarketplaceFilter
    {
        Newest,
        Libraries,
        Projects,
        [DescriptionAttribute("")]
        Activities,
        Templates,
        PublishingWorkflows,
        All
    }
}
