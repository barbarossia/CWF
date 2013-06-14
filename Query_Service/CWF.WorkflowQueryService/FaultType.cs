using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CWF.WorkflowQueryService
{
    /// <summary>
    /// Defines the fault types used in the application.
    /// </summary>
    public enum FaultType
    {
        /// <summary>
        /// Service fault which is most commonly used.
        /// </summary>
        ServiceFault,
        /// <summary>
        /// Publishing fault thrown to convey publishing errors.
        /// </summary>
        PublishingFault,
        /// <summary>
        /// Validation fault thrown to convey validation errors.
        /// </summary>
        ValidationFault
    }
}