//-----------------------------------------------------------------------
// <copyright file="WorkflowTypeGetReplyDC.cs" company="Microsoft">
// Copyright
// WorkflowType Get Reply DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Reply data contract for WorkflowTypeGet
    /// Status returned in ReplyHeader
    /// </summary>
    [DataContract]
    public class WorkflowTypeGetReplyDC : ReplyHeader
    {
        private IList<WorkflowTypesGetBase> workflowActivityType;

        [DataMember]
        public IList<WorkflowTypesGetBase> WorkflowActivityType
        {
            get { return workflowActivityType; }
            set { workflowActivityType = value; }
        }
    }


    [DataContract]
    public class WorkflowTypesGetBase : WorkflowReplayHeader
    {
        /// <summary>
        /// PK of workflowtype row
        /// </summary>
        [DataMember]
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Guid of workflowtype row
        /// </summary>
        [DataMember]
        public Guid Guid
        {
            get;
            set;
        }

        /// <summary>
        /// Name of workflowtype row
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Publishing workflow name
        /// </summary>
        [DataMember]
        public string PublishingWorkflow
        {
            get;
            set;
        }

        /// <summary>
        /// PublishingWorkflowVersion
        /// </summary>
        [DataMember]
        public string PublishingWorkflowVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Publishing workflow Id
        /// </summary>
        [DataMember]
        public int PublishingWorkflowId
        {
            get;
            set;
        }

        /// <summary>
        /// Workflow template name
        /// </summary>
        [DataMember]
        public string WorkflowTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// workflow template version
        /// </summary>
        [DataMember]
        public string WorkflowTemplateVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Workflow template Id
        /// </summary>
        [DataMember]
        public int WorkflowTemplateId
        {
            get;
            set;
        }

        /// <summary>
        /// Handle variable name
        /// </summary>
        [DataMember]
        public string HandleVariable
        {
            get;
            set;
        }

        /// <summary>
        /// Handle variable Id
        /// </summary>
        [DataMember]
        public int HandleVariableId
        {
            get;
            set;
        }

        /// <summary>
        /// Pageview variable name
        /// </summary>
        [DataMember]
        public string PageViewVariable
        {
            get;
            set;
        }

        /// <summary>
        /// Pageview variable Id
        /// </summary>
        [DataMember]
        public int PageViewVariableId
        {
            get;
            set;
        }

        /// <summary>
        /// Auth group Id
        /// </summary>
        [DataMember]
        public int AuthGroupId
        {
            get;
            set;
        }

        /// <summary>
        /// Auth group name
        /// </summary>
        [DataMember]
        public string AuthGroupName
        {
            get;
            set;
        }

        /// <summary>
        /// Selection workflow name
        /// </summary>
        [DataMember]
        public string SelectionWorkflow
        {
            get;
            set;
        }

        /// <summary>
        /// Selection workflow Id
        /// </summary>
        [DataMember]
        public int SelectionWorkflowId
        {
            get;
            set;
        }
    }
}
