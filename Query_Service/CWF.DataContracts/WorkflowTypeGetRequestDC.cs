//-----------------------------------------------------------------------
// <copyright file="WorkflowTypeGetRequestDC.cs" company="Microsoft">
// Copyright
// WorkflowTypes Get Request DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class WorkflowTypesGetRequestDC : WorkflowRequestHeader
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
