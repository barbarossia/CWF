using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Microsoft.Support.Workflow.Service.Contracts.FaultContracts
{
    /// <summary>
    /// Defines a fault contract to raise input validation related web service faults.
    /// </summary>
    [DataContract]
    public class ValidationFault : ServiceFault
    {
    }
}
