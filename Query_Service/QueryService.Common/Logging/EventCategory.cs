using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Service.Common.Logging
{
    /// <summary>
    /// Defines event categories.
    /// </summary>
    public enum EventCategory
    {
        /// <summary>
        /// The cause and resolution are known and the event offers a simple way to recover. 
        /// Looking at the event, the administrator can take a specific action. 
        /// For e.g. a configuration file is missing, a required configuration entry is missing, 
        /// </summary>
        Administrative = 1,
        /// <summary>
        /// There is a problem that requires operatThese events are useful to the developer in diagnosing
        /// the problem. Only logged when the application is in Debug mode.ions intervention. 
        /// The resolution is unknown. E.g. AppFabric fails to create cache.
        /// </summary>
        Operational,
        /// <summary>
        /// These events are helpful in analyzing a problem. The recommended behavior is to log these 
        /// only when the application is taken to “analytic” mode. A developer is still not involved 
        /// at this stage.
        /// </summary>
        Analytic,
        /// <summary>
        /// These events are useful to the developer in diagnosing the problem. Only logged when the 
        /// application is in Debug mode.
        /// </summary>
        Debug,
        /// <summary>
        /// These events are useful to the developer in diagnosing the problem. Only logged when the 
        /// application is in Debug mode.
        /// </summary>
        ApiUsage,
        /// <summary>
        /// Unless the application is logging a strictly informational message of no particular 
        /// operational impact, it is recommended not to use this value.
        /// </summary>
        None
    }
}
