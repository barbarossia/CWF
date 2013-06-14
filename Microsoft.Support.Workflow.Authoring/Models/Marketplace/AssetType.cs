using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.Models.Marketplace
{
    /// <summary>
    /// Enumeration with following values
    /// </summary>
    public enum AssetType
    {
        /// <summary>
        ///We mean a single project, so it is singular
        /// </summary>
        Project,

        /// <summary>
        ///We mean a list of activities, so it is plural 
        /// </summary>
        Activities,
    }
}
