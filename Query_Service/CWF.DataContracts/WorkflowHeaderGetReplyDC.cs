using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.ServiceModel;

namespace CWF.DataContracts
{
    [DataContract]
    public class WorkflowHeaderGetReplyDC : ReplyHeader
    {
        private IList<WorkflowHeaderGetBase> workflowHeaderGetReplyItemsList;

        [DataMember]
        public IList<WorkflowHeaderGetBase> WorkflowHeaderGetReplyItemsList
        {
            get { return workflowHeaderGetReplyItemsList; }
            set { workflowHeaderGetReplyItemsList = value; }
        }
    }
    [DataContract]
    public class WorkflowHeaderGetBase
    {
        private int id;
        private Guid storeActivitiesId;
        private string name;
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

        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        [DataMember]
        public Guid StoreActivitiesId
        {
            get { return storeActivitiesId; }
            set { storeActivitiesId = value; }
        }
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        [DataMember]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        [DataMember]
        public string MetaTags
        {
            get { return metaTags; }
            set { metaTags = value; }
        }
        [DataMember]
        public bool IsService
        {
            get { return isService; }
            set { isService = value; }
        }
        [DataMember]
        public bool IsSwitch
        {
            get { return isSwitch; }
            set { isSwitch = value; }
        }
        [DataMember]
        public bool IsUxActivity
        {
            get { return isUxActivity; }
            set { isUxActivity = value; }
        }
        [DataMember]
        public string Version
        {
            get { return version; }
            set { version = value; }
        }
        [DataMember]
        public string StatusCodeName
        {
            get { return statusCodeName; }
            set { statusCodeName = value; }
        }
        [DataMember]
        public string WorkflowTypeName
        {
            get { return workflowTypeName; }
            set { workflowTypeName = value; }
        }
        [DataMember]
        public string WorkflowGroup
        {
            get { return workflowGroup; }
            set { workflowGroup = value; }
        }
        [DataMember]
        public string ContextVariable
        {
            get { return contextVariable; }
            set { contextVariable = value; }
        }
        [DataMember]
        public string HandleVariable
        {
            get { return handleVariable; }
            set { handleVariable = value; }
        }
        [DataMember]
        public string PageViewVariable
        {
            get { return pageViewVariable; }
            set { pageViewVariable = value; }
        }
        [DataMember]
        public string SelectionWorkflow
        {
            get { return selectionWorkflow; }
            set { selectionWorkflow = value; }
        }
        [DataMember]
        public string WorkflowTemplate
        {
            get { return workflowTemplate; }
            set { workflowTemplate = value; }
        }
        [DataMember]
        public bool Locked
        {
            get { return locked; }
            set { locked = value; }
        }
        [DataMember]
        public string LockedBy
        {
            get { return lockedBy; }
            set { lockedBy = value; }
        }
        [DataMember]
        public string ActivityCategoryName
        {
            get { return activityCategoryName; }
            set { activityCategoryName = value; }
        }
        [DataMember]
        public string AuthGroupName
        {
            get { return authGroupName; }
            set { authGroupName = value; }
        }
        [DataMember]
        public string PublishingWorkflow
        {
            get { return publishingWorkflow; }
            set { publishingWorkflow = value; }
        }
    }
}
