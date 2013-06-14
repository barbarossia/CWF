//-----------------------------------------------------------------------
// <copyright file="WorkflowAllGetReplyDC.cs" company="Microsoft">
// Copyright
// Workflow All Get Reply DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Reply data contract for WorkflowAllGet
    /// Status returned in RequestreplyCommonHeader
    /// </summary>
    [DataContract]
    public class WorkflowAllGetReplyDC : RequestReplyCommonHeader
    {
        private IList<WorkflowAllGetBase> list;

        [DataMember]
        public IList<WorkflowAllGetBase> List
        {
            get { return list; }
            set { list = value; }
        }
    }

    [DataContract]
    public class WorkflowAllGetBase
    {
        private int id;
        private string storeActivitiesId;
        private string storeActivitiesname;
        private string description;
        private string metaTags;
        private bool isService;
        private bool isSwitch;
        private bool isUxActivity;
        private string version;
        private string statusCodeName;
        private string workflowTypeName;
        private string workflowGroup;
        private string contextVariable;
        private string handleVariable;
        private string pageViewVariable;
        private string selectionWorkflow;
        private string workflowTemplate;
        private bool locked;
        private string lockedBy;
        private string activityCategoryName;
        private string authGroupName;
        private string publishingWorkflow;

        /// <summary>
        /// PK of StoreActivity
        /// </summary>
        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Guid of StoreActivity
        /// </summary>
        [DataMember]
        public string StoreActivitiesId
        {
            get { return storeActivitiesId; }
            set { storeActivitiesId = value; }
        }

        /// <summary>
        /// Name of StoreActivity
        /// </summary>
        [DataMember]
        public string StoreActivitiesname
        {
            get { return storeActivitiesname; }
            set { storeActivitiesname = value; }
        }

        /// <summary>
        /// Description of StoreActivity
        /// </summary>
        [DataMember]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Meta tags of StoreActivity
        /// </summary>
        [DataMember]
        public string MetaTags
        {
            get { return metaTags; }
            set { metaTags = value; }
        }

        /// <summary>
        /// Service boolean of StoreActivity
        /// </summary>
        [DataMember]
        public bool IsService
        {
            get { return isService; }
            set { isService = value; }
        }

        /// <summary>
        /// Switch boolean of StoreActivity
        /// </summary>
        [DataMember]
        public bool IsSwitch
        {
            get { return isSwitch; }
            set { isSwitch = value; }
        }

        /// <summary>
        /// UxActivity boolean of StoreActivity
        /// </summary>
        [DataMember]
        public bool IsUxActivity
        {
            get { return isUxActivity; }
            set { isUxActivity = value; }
        }

        /// <summary>
        /// Version of StoreActivity
        /// </summary>
        [DataMember]
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// Status code name
        /// </summary>
        [DataMember]
        public string StatusCodeName
        {
            get { return statusCodeName; }
            set { statusCodeName = value; }
        }

        /// <summary>
        /// Work flow type name of StoreActivity
        /// </summary>
        [DataMember]
        public string WorkflowTypeName
        {
            get { return workflowTypeName; }
            set { workflowTypeName = value; }
        }

        /// <summary>
        /// Work flow group of StoreActivity
        /// </summary>
        [DataMember]
        public string WorkflowGroup
        {
            get { return workflowGroup; }
            set { workflowGroup = value; }
        }

        /// <summary>
        /// Context variable name of StoreActivity
        /// </summary>
        [DataMember]
        public string ContextVariable
        {
            get { return contextVariable; }
            set { contextVariable = value; }
        }

        /// <summary>
        /// Handle variable of StoreActivity
        /// </summary>
        [DataMember]
        public string HandleVariable
        {
            get { return handleVariable; }
            set { handleVariable = value; }
        }

        /// <summary>
        /// Page view variable  of StoreActivity
        /// </summary>
        [DataMember]
        public string PageViewVariable
        {
            get { return pageViewVariable; }
            set { pageViewVariable = value; }
        }

        /// <summary>
        /// Selection work flow of StoreActivity
        /// </summary>
        [DataMember]
        public string SelectionWorkflow
        {
            get { return selectionWorkflow; }
            set { selectionWorkflow = value; }
        }

        /// <summary>
        /// Workflow template of StoreActivity
        /// </summary>
        [DataMember]
        public string WorkflowTemplate
        {
            get { return workflowTemplate; }
            set { workflowTemplate = value; }
        }

        /// <summary>
        /// Locked boolean of StoreActivity
        /// </summary>
        [DataMember]
        public bool Locked
        {
            get { return locked; }
            set { locked = value; }
        }

        /// <summary>
        /// Locked by alias of StoreActivity
        /// </summary>
        [DataMember]
        public string LockedBy
        {
            get { return lockedBy; }
            set { lockedBy = value; }
        }

        /// <summary>
        /// Activity Category name of StoreActivity
        /// </summary>
        [DataMember]
        public string ActivityCategoryName
        {
            get { return activityCategoryName; }
            set { activityCategoryName = value; }
        }

        /// <summary>
        /// Auth group Name of StoreActivity
        /// </summary>
        [DataMember]
        public string AuthGroupName
        {
            get { return authGroupName; }
            set { authGroupName = value; }
        }

        /// <summary>
        /// Publishing Workflow of StoreActivity
        /// </summary>
        [DataMember]
        public string PublishingWorkflow
        {
            get { return publishingWorkflow; }
            set { publishingWorkflow = value; }
        }
    }
}
