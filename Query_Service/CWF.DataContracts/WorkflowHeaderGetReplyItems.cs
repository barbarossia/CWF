using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.ServiceModel;

namespace CWF.DataContracts
{
    [DataContract]
    public class WorkflowHeaderGetReplyItems
    {
        private int id;
        private string name = string.Empty;
        private string description = string.Empty;
        private string metatags = string.Empty;
        private string iconName = string.Empty;
        private bool isSwitch;
        private bool isService;
        private string activityLibraryName = string.Empty;
        private bool isUxActivity;
        private string defaultRender = string.Empty;
        private string activityCategoryName = string.Empty;
        private string toolBoxTabName = string.Empty;
        private bool isToolBoxActivity;
        private string version = string.Empty;
        private string statusCodeName = string.Empty;
        private string workflowTypeName = string.Empty;
        private bool locked;
        private string lockedBy = string.Empty;
        private bool isCodeBeside;
        // private string xaml = string.Empty;
        private string developerNotes = string.Empty;
        private string basetype = string.Empty;
        private string namespace1 = string.Empty;
        private string authGroupName = string.Empty;

        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
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
        public string Metatags
        {
            get { return metatags; }
            set { metatags = value; }
        }

        [DataMember]
        public string IconName
        {
            get { return iconName; }
            set { iconName = value; }
        }

        [DataMember]
        public bool IsSwitch
        {
            get { return isSwitch; }
            set { isSwitch = value; }
        }

        [DataMember]
        public bool IsService
        {
            get { return isService; }
            set { isService = value; }
        }

        [DataMember]
        public string ActivityLibraryName
        {
            get { return activityLibraryName; }
            set { activityLibraryName = value; }
        }

        [DataMember]
        public bool IsUxActivity
        {
            get { return isUxActivity; }
            set { isUxActivity = value; }
        }

        [DataMember]
        public string DefaultRender
        {
            get { return defaultRender; }
            set { defaultRender = value; }
        }

        [DataMember]
        public string ActivityCategoryName
        {
            get { return activityCategoryName; }
            set { activityCategoryName = value; }
        }

        [DataMember]
        public string ToolBoxTabName
        {
            get { return toolBoxTabName; }
            set { toolBoxTabName = value; }
        }
        [DataMember]
        public bool IsToolBoxActivity
        {
            get { return isToolBoxActivity; }
            set { isToolBoxActivity = value; }
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
        public bool IsCodeBeside
        {
            get { return isCodeBeside; }
            set { isCodeBeside = value; }
        }

        //[DataMember]
        //public string Xaml
        //{
        //    get { return xaml; }
        //    set { xaml = value; }
        //}

        [DataMember]
        public string DeveloperNotes
        {
            get { return developerNotes; }
            set { developerNotes = value; }
        }

        [DataMember]
        public string Basetype
        {
            get { return basetype; }
            set { basetype = value; }
        }

        [DataMember]
        public string Namespace
        {
            get { return namespace1; }
            set { namespace1 = value; }
        }

        [DataMember]
        public string AuthGroupName
        {
            get { return authGroupName; }
            set { authGroupName = value; }
        }
    }
}
