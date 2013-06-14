using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Microsoft.Support.Workflow.Service.Contracts.FaultContracts
{
    /// <summary>
    /// Defines a fault contract to raise general web service faults.
    /// </summary>
    [DataContract]
    public class ServiceFault
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [DataMember]
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }
    }
}
