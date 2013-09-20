//-----------------------------------------------------------------------
// <copyright file="WorkFlowTypeCreateOrUpdateRequestDC.cs" company="Microsoft">
// Copyright
// WorkFlowType Create Or Update Request DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    using Microsoft.Practices.EnterpriseLibrary.Validation;
    using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

    /// <summary>
    /// Request data contract for WorkflowTypeCreateOrUpdate
    /// </summary>
    [DataContract]
    public class WorkFlowTypeCreateOrUpdateRequestDC : WorkflowRequestHeader
    {
        /// <summary>
        /// PK of workflowtype row
        /// </summary>
        [DataMember]
        public int InId
        {
            get;
            set;
        }

        [DataMember]
        public bool IsDeleted
        {
            get;
            set;
        }

        /// <summary>
        /// Guidof workflowtype row
        /// </summary>
        [DataMember]
        public string InWorkflowTypeid
        {
            get;
            set;
        }

        /// <summary>
        /// Nameof workflowtype row
        /// </summary>
        [DataMember]
        public string InName
        {
            get;
            set;
        }

        /// <summary>
        /// Nameof workflowtype row
        /// </summary>
        [DataMember]
        public Guid InGuid
        {
            get;
            set;
        }

        /// <summary>
        /// Workflow group
        /// </summary>
        [DataMember]
        public int InWorkflowGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Publishing workflow
        /// </summary>
        [DataMember]
        public int InPublishingWorkflowId
        {
            get;
            set;
        }

        /// <summary>
        /// Workflow template
        /// </summary>
        [DataMember]
        public int InWorkflowTemplateId
        {
            get;
            set;
        }

        /// <summary>
        /// Handle variable
        /// </summary>
        [DataMember]
        public string InHandleVariable
        {
            get;
            set;
        }

        /// <summary>
        /// Page view variable
        /// </summary>
        [DataMember]
        public string InPageViewVariable
        {
            get;
            set;
        }

        /// <summary>
        /// Auth group
        /// </summary>
        [DataMember]
        public int InAuthGroupId
        {
            get;
            set;
        }

        /// <summary>
        /// Selection workflow
        /// </summary>
        [DataMember]
        public int InSelectionWorkflowId
        {
            get;
            set;
        }
    }
}
