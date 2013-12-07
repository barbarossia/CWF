// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowItem.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using Microsoft.Support.Workflow.Authoring.AddIns;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    using Microsoft.Support.Workflow.Authoring.Behaviors;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;
    using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
    using Microsoft.Support.Workflow.Authoring.Services;
    using System.Reflection;
    using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
    using CWF.DataContracts;
    using Microsoft.Support.Workflow.Authoring.AddIns.Data;
    using Microsoft.Support.Workflow.Authoring.Common.Messages;

    /// <summary>
    /// The workflow item. 
    /// It is instance to wrap the rehost workflowdesigner and it's services
    /// </summary>
    [Serializable]
    public sealed partial class WorkflowItem : ActivityItem
    {
        /// <summary>
        /// Default to use for the workflow type that is not a service
        /// </summary>
        private const string WorkflowTypeDefaultActivity = "Workflow";

        /// <summary>
        /// Default to use for the workflow type that is a service
        /// </summary>
        private const string WorkflowTypeDefaultWfservice = "WorkflowService";

        /// <summary>
        /// Default version number
        /// Start at 1.0.0.0
        /// </summary>
        private const string DefaultVersion = "1.0.0.0";

        /// <summary>
        /// The is saved to server.
        /// </summary>
        private bool isSavedToServer;

        /// <summary>
        /// The local file full name.
        /// </summary>
        private string localFileFullName;
        private string workflowName;
        /// <summary>
        /// The type that was used to create this workflow
        /// </summary>
        private string workflowType;

        [NonSerialized]
        private DesignerHostAdapters workflowDesigner;

        /// <summary>
        /// The data dirty flag, defaults to true
        /// </summary>
        private bool isDataDirty = true;

        public bool IsTask { get; set; }
        private Guid? taskActivityGuid;

        private TaskActivityStatus? activityStatus;
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowItem"/> class.
        /// </summary>
        /// <param name="name">
        /// The name (class name) of the WorkflowItem.
        /// </param>
        /// <param name="displayName">
        /// The display name.
        /// </param>
        /// <param name="projectXamlCode">
        /// The xaml code.
        /// </param>
        /// <param name="workflowType">
        /// The workflow type that the workflow was based on
        /// </param>
        public WorkflowItem(string name, string displayName, string projectXamlCode, string workflowType, List<ActivityAssemblyItem> references = null)
        {
            this.References = references ?? new List<ActivityAssemblyItem>();
            this.XamlCode = projectXamlCode;
            Name = name;
            OriginalName = name;
            DisplayName = displayName;
            WorkflowType = workflowType;
            WorkflowName = Name;
            Version = DefaultVersion;
            IsValid = true;
        }

        public void InitializeWorkflowDesigner()
        {
            this.WorkflowDesigner = new DesignerHostAdapters(this.Name, this.XamlCode, this.References, this.IsTask);
            this.WorkflowDesigner.SetWorkflowName(this.Name);
            this.WorkflowDesigner.ClearUndo();
            this.WorkflowDesigner.DesignerChanged += this.DesignerChanged;
            this.WorkflowDesigner.DesignerReloaded += this.DesignerReload;
            this.WorkflowDesigner.PrintStateChanged += this.OnPrintStateChanged;
            this.workflowDesigner.GetTaskLastVersionChanged += this.DownloadTaskDependency;
            if (string.IsNullOrWhiteSpace(workflowType))
            {
                WorkflowType = IsService ? WorkflowTypeDefaultWfservice : WorkflowTypeDefaultActivity;
                IsDataDirty = true;
            }
        }

        public void Close()
        {
            if (this.WorkflowDesigner != null)
                this.WorkflowDesigner.UnloadAddIn();
            this.WorkflowDesigner = null;
        }

        public string OriginalName { get; set; }

        public Guid? TaskActivityGuid
        {
            get { return this.taskActivityGuid; }
            set
            {
                this.taskActivityGuid = value;
                RaisePropertyChanged(() => this.TaskActivityGuid);
            }
        }

        public TaskActivityStatus? TaskActivityStatus
        {
            get { return this.activityStatus; }
            set
            {
                this.activityStatus = value;
                RaisePropertyChanged(() => this.TaskActivityStatus);
            }
        }

        /// <summary>
        /// The type that was used to create this workflow
        /// </summary>
        public string WorkflowType
        {
            get
            {
                return workflowType;
            }
            set
            {
                workflowType = value;
                RaisePropertyChanged(() => WorkflowType);
            }
        }

        /// <summary>
        /// Overrides the Tags property in the base class to add functionality to make the workflow
        /// saveable/dirty when the Tag property changes.
        /// </summary>
        public override string Tags
        {
            get { return base.Tags; }
            set
            {
                base.Tags = value;
                MarkAsDirty();
            }
        }

        /// <summary>
        /// Public property to tell if the current workflow is valid
        /// </summary>
        [ReadOnly(true)]
        public override bool IsValid
        {
            get
            {
                return this.WorkflowDesigner != null ? this.WorkflowDesigner.IsValid() && base.IsValid : base.IsValid;
            }
        }

        /// <summary>
        /// Overrides the Version property in the base class to add functionality to make the workflow
        /// saveable/dirty when the Version property changes.
        /// </summary>
        public override string Version
        {
            get { return base.Version; }
            set
            {
                if (base.Version != value)
                {
                    base.Version = value;
                    MarkAsDirty();
                }
            }
        }

        private bool isLoadingDesigner;
        public bool IsLoadingDesigner
        {
            get { return this.isLoadingDesigner; }
            set
            {
                this.isLoadingDesigner = value;
                RaisePropertyChanged(() => IsLoadingDesigner);
            }
        }

        private bool hasMajorChanged;
        public bool HasMajorChanged
        {
            get { return hasMajorChanged; }
            set
            {
                hasMajorChanged = value;
                RaisePropertyChanged(() => HasMajorChanged);
            }
        }

        public void MarkAsDirty()
        {
            IsDataDirty = true;       // Set local data dirty flag
            IsSavedToServer = false;  // Will also need to re-save to Server
        }

        /// <summary>
        /// Indicates if the wf is opened from server
        /// </summary>
        public bool IsOpenFromServer { get; set; }

        /// <summary>
        /// Public property to tell if the current workflow is read-only
        /// </summary>
        public new bool IsReadOnly
        {
            get
            {
                return base.IsReadOnly;
            }
            set
            {
                base.IsReadOnly = value;
                if (this.WorkflowDesigner != null)
                    this.workflowDesigner.SetReadOnly(value);
            }
        }

        /// <summary>
        /// Public property that is used to tell if the data is dirty in the current model
        /// </summary>
        public bool IsDataDirty
        {
            get
            {
                return isDataDirty;
            }
            set
            {
                isDataDirty = value;
                RaisePropertyChanged(() => IsDataDirty);
            }
        }

        /// <summary>
        /// Public property to tell if a WorkflowItem is a WorkflowService
        /// </summary>
        [ReadOnly(true)]
        public bool IsService
        {
            get
            {
                return WorkflowDesigner != null ? WorkflowDesigner.IsWorkflowService : false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsSavedToServer.
        /// </summary>
        public bool IsSavedToServer
        {
            get
            {
                return isSavedToServer;
            }
            set
            {
                isSavedToServer = value;
                RaisePropertyChanged(() => IsSavedToServer);
            }
        }

        /// <summary>
        /// Gets or sets LocalFileFullName.
        /// </summary>
        public string LocalFileFullName
        {
            get
            {
                return localFileFullName;
            }
            set
            {
                localFileFullName = value;
                RaisePropertyChanged(() => LocalFileFullName);
            }
        }

        [Required]
        //[RegularExpression(ClassNameRegularExpression)]
        public string WorkflowName
        {
            get { return workflowName; }
            set
            {
                workflowName = value;
                Validate();
                RaisePropertyChanged(() => WorkflowName);
                if (base.IsValid && Name != workflowName)
                {
                    Name = workflowName;
                }
            }
        }

        public void Print()
        {
            this.PrintState = PrintAction.PrintUserSelection;
            this.WorkflowDesigner.Print();
        }

        public void PrintAll()
        {
            this.PrintState = PrintAction.PrintAll;
            this.WorkflowDesigner.PrintAll();
        }

        private PrintAction printState;
        public PrintAction PrintState
        {
            get
            {
                return printState;
            }
            set
            {
                printState = value;
                RaisePropertyChanged(() => PrintState);
            }
        }

        public DesignerHostAdapters WorkflowDesigner
        {
            get
            {
                return workflowDesigner;
            }
            set
            {
                this.workflowDesigner = value;
                if (workflowDesigner != null)
                {
                    this.DesignerView = this.workflowDesigner.View;
                    this.PEView = this.workflowDesigner.ProjectExplorerView;
                    this.PropertyView = this.workflowDesigner.PropertyInspectorView;
                    this.Toolbox = this.workflowDesigner.ToolboxView;
                }
                RaisePropertyChanged(() => this.WorkflowDesigner);
            }
        }

        [NonSerialized]
        private FrameworkElement designerView;
        public FrameworkElement DesignerView
        {
            get { return designerView; }
            set
            {
                this.designerView = value;
                RaisePropertyChanged(() => this.DesignerView);
            }
        }

        [NonSerialized]
        private FrameworkElement propertyView;//= new FrameworkElement()
        public FrameworkElement PropertyView
        {
            get { return propertyView; }
            set
            {
                this.propertyView = value;
                RaisePropertyChanged(() => this.PropertyView);
            }
        }

        [NonSerialized]
        private FrameworkElement pEView;
        public FrameworkElement PEView
        {
            get { return this.pEView; }
            set
            {
                this.pEView = value;
                RaisePropertyChanged(() => this.PEView);
            }
        }

        [NonSerialized]
        private FrameworkElement toolbox;
        public FrameworkElement Toolbox
        {
            get { return this.toolbox; }
            set
            {
                this.toolbox = value;
                RaisePropertyChanged(() => this.Toolbox);
            }
        }

        protected override void OnSetName()
        {
            // Make sure we sync the FullName
            FullName = Name;
            if (this.WorkflowDesigner != null)
            {
                this.WorkflowDesigner.SetWorkflowName(Name);
            }
        }

        public void SetXamlCode()
        {
            if (this.WorkflowDesigner != null)
            {
                this.XamlCode = this.WorkflowDesigner.XamlCode;
            }
        }

        public void SetReferences()
        {
            if (this.WorkflowDesigner != null)
            {
                this.References = this.WorkflowDesigner.DependencyAssemblies.ToList();
            }
        }

        public void FinishTaskAssigned()
        {
            this.WorkflowDesigner.FinishTaskAssigned();
        }

        private void DesignerChanged(object sender, EventArgs e)
        {
            this.IsSavedToServer = false;
            this.IsDataDirty = true;
        }

        private void OnPrintStateChanged(object sender, EventArgs e)
        {
            this.PrintState = this.WorkflowDesigner.PrintState;
        }

        private void DesignerReload(object sender, EventArgs e)
        {
            if (this.WorkflowDesigner != null)
            {
                this.PEView = this.WorkflowDesigner.ProjectExplorerView;
                this.PropertyView = this.WorkflowDesigner.PropertyInspectorView;
                this.DesignerView = this.WorkflowDesigner.View;
                this.Toolbox = this.WorkflowDesigner.ToolboxView;
            }
        }

        private void DownloadTaskDependency(object sender, GetTaskEventArgs e)
        {
            var activityAssemblyItem = GetActivityAssemblyItem(e);
            var references = DownloadTaskDependency(activityAssemblyItem);
            this.WorkflowDesigner.ImportAssemblies(references, canRefresh: false);
        }

        private ActivityAssemblyItem GetActivityAssemblyItem(string activityLibraryName, string versionNumber)
        {
            Version version;
            ActivityAssemblyItem activityAssemblyItem = new ActivityAssemblyItem();
            activityAssemblyItem.Assembly = null;
            activityAssemblyItem.AssemblyName = new AssemblyName(activityLibraryName);
            System.Version.TryParse(versionNumber, out version);
            activityAssemblyItem.Version = version;
            return activityAssemblyItem;
        }

        // download dependencies
        private IEnumerable<ActivityAssemblyItem> DownloadTaskDependency(List<ActivityAssemblyItem> items)
        {
            return WorkflowsQueryServiceUtility.UsingClientReturn(client =>
               {
                   return Caching.CacheAndDownloadAssembly(client, Caching.ComputeDependencies(client, items));
               }
           );
        }

        private List<ActivityAssemblyItem> GetActivityAssemblyItem(GetTaskEventArgs e)
        {
            if (e.IsCollection)
            {
                return e.ActivityLibraries.Select(a => GetActivityAssemblyItem(a.Key, a.Value)).ToList();
            }
            else
            {
                return new[] { GetActivityAssemblyItem(e.ActivityLibraryName, e.Version) }.ToList();
            }
        }

        public override void Validate() 
        {
            base.Validate();
            if (!string.IsNullOrWhiteSpace(this.WorkflowName)) 
            {
                var regex = new Regex(CommonMessages.ClassNameRegularExpression);
                if (!regex.IsMatch(this.WorkflowName))
                {
                    IsValid = false;
                    ErrorMessage += CommonMessages.WorkflowNameErrorString;
                    ErrorMessage = ErrorMessage.Trim();
                }
            }
        }
    }
}
