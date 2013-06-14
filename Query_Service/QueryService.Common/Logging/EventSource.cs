using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Service.Common.Logging
{
    /// <summary>
    /// Defines the possible event sources.
    /// </summary>
    public enum EventSource
    {
        /// <summary>
        /// Database error.
        /// </summary>
        DatabaseError,
        /// <summary>
        /// Data access layer error.
        /// </summary>
        DataAccessLayerError,
        /// <summary>
        /// Business layer error.
        /// </summary>
        BusinessLayerError,
        /// <summary>
        /// Web service layer error.
        /// </summary>
        WebServiceLayerError,
        /// <summary>
        /// Log writer error.
        /// </summary>
        LogWriterError,
        /// <summary>
        /// Database validation.
        /// </summary>
        DatabaseValidation,
        /// <summary>
        /// Business layer validation.
        /// </summary>
        BusinessLayerValidation
    }
}
