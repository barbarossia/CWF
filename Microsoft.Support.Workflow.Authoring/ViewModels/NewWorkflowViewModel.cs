// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewWorkflowFromTemplateViewModel.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Common;
    using CWF.DataContracts;
    using Models;
    using Practices.Prism.Commands;
    using Practices.Prism.ViewModel;
    using Services;
    using System.Threading;
    using System.ServiceModel;
    using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;
    using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
    using Microsoft.Support.Workflow.Authoring.Views;
    using System.Windows;
    using System.Windows.Threading;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    using Microsoft.Support.Workflow.Authoring.AddIns;
    using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
    using Microsoft.Support.Workflow.Authoring.AddIns.Data;
    using Microsoft.Support.Workflow.Authoring.Security;
    using Microsoft.Support.Workflow.Authoring.Common.Messages;


    /// <summary>
    /// Defines the ViewModel for New WorkflowFromTemplate
    /// </summary>
    public sealed class NewWorkflowViewModel : ViewModelBase
    {
        private const int MaximumClassNameLength = 50;
        private const string DefaultCategory = "Unassigned";
        private const string DefaultVersion = "1.0.0.0";
        private const string DevDefaultWorkflowType = "Workflow";
        private const string TestDefaultWorkflowType = "Workflow Template";
        private const string DefaultTags = "Meta Tags";
        private const string ClassNameReplaceRegularExpression = @"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]";
        private const string SelectATemplateErrorString = "\r\nYou must select a template.";
        private const string SelectALocationErrorString = "\r\nYou must select a location.";

        private WorkflowItem createdWorkflowItem;                   // The item created by the view model
        private List<WorkflowTemplateItem> selectableTemplates;     // The list of Templates that we can select from
        private WorkflowTemplateItem selectedWorkflowTemplateItem;  // The selected item in the combobox
        private string workflowName;                                // The name of the Workflow
        private string workflowClassName;                           // The name of the Workflow class to create
        private bool isInitialized;                                 // Flag to tell when we are Initialized
        private bool isCreatingBlank;                               // Flag to tell if we are creating a blank project.
        private List<Env> locations;
        private Env? selectedLocation;

        public Func<IWorkflowsQueryService, WorkflowItem> GetWorkflowTemplateActivity { get; set; }  // Pluggable for testability

        public string DefaultWorkflowTemplate
        {
            get
            {
                if (this.SelectedLocation == Env.Test)
                    return TestDefaultWorkflowType;
                else
                    return DevDefaultWorkflowType;
            }
        }

        public bool CanCreateWorkflow
        {
            get { return this.SelectedLocation != null; }
        }

        public List<Env> Locations
        {
            get { return this.locations; }
            set
            {
                this.locations = value;
                RaisePropertyChanged(() => this.Locations);
            }
        }

        public Env? SelectedLocation
        {
            get { return this.selectedLocation; }
            set
            {
                this.selectedLocation = value;
                RaisePropertyChanged(() => this.SelectedLocation);
                if (this.SelectedLocation != null)
                    this.GetWorkflowTemlates(this.SelectedLocation.Value);
                RaisePropertyChanged(() => this.CanCreateWorkflow);
            }
        }

        /// <summary>
        /// Delegate command to create the WorkflowItem 
        /// </summary>
        public DelegateCommand CreateWorkflowItem { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        public NewWorkflowViewModel()
        {
            Initialize();
        }

        /// <summary>
        /// The name of the workflow to create DisplayName
        /// </summary>
        [DisplayName("Workflow Name")]
        [Required]
        //[RegularExpression(ClassNameRegularExpression)]
        [StringLength(MaximumClassNameLength)]
        public string WorkflowName
        {
            get { return workflowName; }
            set
            {
                workflowName = value;
                RaisePropertyChanged(() => WorkflowName);
            }
        }

        /// <summary>
        /// User-entered: the name of the workflow to create.
        /// </summary>
        [DisplayName("Workflow Class Name")]
        [Required]
        //[RegularExpression(ClassNameRegularExpression)]
        [StringLength(MaximumClassNameLength)]
        public string WorkflowClassName
        {
            get { return workflowClassName; }
            set
            {
                workflowClassName = value;
                RaisePropertyChanged(() => WorkflowClassName);
            }
        }

        /// <summary>
        /// The item that was created by the View Model
        /// </summary>
        [DisplayName("Created Item")]
        public WorkflowItem CreatedItem
        {
            get { return createdWorkflowItem; }
            set
            {
                createdWorkflowItem = value;
                RaisePropertyChanged(() => CreatedItem);
            }
        }

        /// <summary>
        /// The list of templates that can be selected
        /// </summary>
        [DisplayName("Workflow Templates")]
        public List<WorkflowTemplateItem> SelectWorkflowTemplates
        {
            get { return selectableTemplates; }
            set
            {
                selectableTemplates = value;
                RaisePropertyChanged(() => SelectWorkflowTemplates);
            }
        }

        /// <summary>
        /// The item that the user has selected in the Combo box
        /// </summary>
        [DisplayName("Selected Workflow Template")]
        public WorkflowTemplateItem SelectedWorkflowTemplateItem
        {
            get { return selectedWorkflowTemplateItem; }
            set
            {
                selectedWorkflowTemplateItem = value;
                RaisePropertyChanged("SelectedWorkflowTemplateItem");
            }
        }

        /// <summary>
        /// The item that the user has selected in the Combo box
        /// </summary>
        [DisplayName("Is Creating Blank?")]
        public bool IsCreatingBlank
        {
            get { return isCreatingBlank; }
            set
            {
                isCreatingBlank = value;
                RaisePropertyChanged(() => IsCreatingBlank);
            }
        }

        /// <summary>
        /// If true then the ViewModel is Initialized
        /// </summary>
        [DisplayName("Is Initialized?")]
        public bool IsInitialized
        {
            get { return isInitialized; }
            set
            {
                isInitialized = value;
                RaisePropertyChanged(() => IsInitialized);
            }
        }

        /// <summary>
        /// Initialize the ViewModel
        /// </summary>
        private void Initialize()
        {
            GetWorkflowTemplateActivity = GetWorkflowTemplateActivityExecute;
            PropertyChanged += NewWorkflowFromTemplateViewModel_PropertyChanged;

            // Set up the Create Workflow command and the CanExecute
            CreateWorkflowItem = new DelegateCommand(CreateWorkflowItemExecute);
            this.Locations = new List<Env>(AuthorizationService.GetAuthorizedEnvs(Permission.SaveWorkflow));
            if (this.Locations.Contains(DefaultValueSettings.Environment))
                this.SelectedLocation = DefaultValueSettings.Environment;
            else if (this.Locations.Count > 0)
                this.SelectedLocation = Locations[0];
            IsInitialized = true;
        }

        private void GetWorkflowTemlates(Env locationParam)
        {
            Utility.DoTaskWithBusyCaption("Loading", () =>
            {
                using (var client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
                {
                    var templateList = new List<WorkflowTemplateItem>();
                    WorkflowTypeGetReplyDC replyDC = null;
                    WorkflowTypesGetRequestDC request = new WorkflowTypesGetRequestDC().SetIncaller();
                    request.Environment = locationParam.ToString();
                    replyDC = client.WorkflowTypeGet(request);

                    if (null != replyDC && null != replyDC.StatusReply && 0 == replyDC.StatusReply.Errorcode)
                    {
                        // Create a Workflow Template Item without XAML and add it to the Template list
                        if (replyDC.WorkflowActivityType.Any())
                        {
                            templateList.AddRange(
                                replyDC.WorkflowActivityType.Select(
                                    workflowType => new WorkflowTemplateItem(workflowType.WorkflowTemplateId, workflowType.Name)));
                        }
                    }
                    this.SelectWorkflowTemplates = templateList;
                }
            });
        }

        /// <summary>
        /// Execution for creating a Workflowitem
        /// </summary>
        private void CreateWorkflowItemExecute()
        {
            if (!CanExecuteCreateWorkflowItem())
                return;
            ErrorMessage = String.Empty;
            if (IsCreatingBlank)
            {
                CreatedItem = GetBlankProject();
                CreatedItem.Name = WorkflowClassName;
                CreatedItem.DisplayName = WorkflowName;
                CreatedItem.CreatedBy = Utility.GetCurrentUserName();
                CreatedItem.UpdatedBy = Utility.GetCurrentUserName();
            }
            else
            {
                // if we get here then it's okay to create a new workflow 
                // We have to get the referenced XAML from the specified StoreActivity 
                WorkflowsQueryServiceUtility.UsingClient(GetWorkflowTemplateActivityAction);
            }
            if (CreatedItem != null)
                CreatedItem.Env = this.SelectedLocation.Value;
        }

        private void GetWorkflowTemplateActivityAction(IWorkflowsQueryService client)
        {
            try
            {
                CreatedItem = GetWorkflowTemplateActivityExecute(client);
                CreatedItem.Name = WorkflowClassName;
                CreatedItem.DisplayName = WorkflowName;
                CreatedItem.WorkflowName = WorkflowClassName;
                CreatedItem.Version = DefaultVersion;
                CreatedItem.CreatedBy = Utility.GetCurrentUserName();
                CreatedItem.UpdatedBy = Utility.GetCurrentUserName();
            }
            catch (Exception)
            {
                MessageBoxService.ShowError("Can not download the selected template.");
            }
        }
        /// <summary>
        /// Can Execute to manage usability of the UI
        /// </summary>
        /// <returns>True if Command Can Execute, false otherwise</returns>
        private bool CanExecuteCreateWorkflowItem()
        {
            return IsInitialized
                   && IsValid
                   && (null != SelectedWorkflowTemplateItem || IsCreatingBlank)
                   && !string.IsNullOrEmpty(WorkflowName);
        }

        /// <summary>
        /// Property change notification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewWorkflowFromTemplateViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // See if we can execute the command based on what has changed
            switch (e.PropertyName)
            {
                case "IsInitialized":
                case "SelectedItem":
                    CreateWorkflowItem.RaiseCanExecuteChanged();
                    break;

                // We also clean up the class name when the Name changes
                case "WorkflowName":
                    WorkflowClassName = CleanClassName(WorkflowName);
                    CreateWorkflowItem.RaiseCanExecuteChanged();
                    break;

                case "IsCreatingBlank":
                    CreateWorkflowItem.RaiseCanExecuteChanged();
                    break;

            }
        }

        /// <summary>
        /// Gets the Workflow Template Activity Item based on the Selected Workflow Template, and its dependencies
        /// </summary>
        /// <returns>The Activity Item that will have the XAML required to create the workflow from the specified Workflow Template</returns>
        private WorkflowItem GetWorkflowTemplateActivityExecute(IWorkflowsQueryService client)
        {
            WorkflowItem workflowActivityTemplateItem = null;
            List<StoreActivitiesDC> storeActivitiesList = null;
            ActivityAssemblyItem workflowActivityTemplateItemAssembly = null;
            StoreActivitiesDC targetDC = null;
            ActivityLibraryDC targetLibrary = null;
            List<ActivityLibraryDC> activityLibraryList;

            // Throw if nothing selected. CanExecute should prevent this.
            if (null == SelectedWorkflowTemplateItem)
                throw new ArgumentNullException();
            // Create the Activity request
            var requestActivity = new StoreActivitiesDC()
                                                    {
                                                        Incaller = Assembly.GetExecutingAssembly().GetName().Name,
                                                        IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                                                        Id = SelectedWorkflowTemplateItem.WorkflowTemplateId,

                                                    };

            // Get the List of Activities that qualify, should only be one
            storeActivitiesList = client.StoreActivitiesGet(requestActivity);
            storeActivitiesList[0].StatusReply.CheckErrors();
            if (null != storeActivitiesList)
            {
                // Get the first or one and only StoreActivity
                targetDC = storeActivitiesList.First();
                // We have to get the Activity Library associated with the Store Activity
                if (0 != targetDC.ActivityLibraryId)
                {
                    List<ActivityAssemblyItem> references = new List<ActivityAssemblyItem>();
                    Utility.DoTaskWithBusyCaption("Loading...", () =>
                    {
                        // Create the Library request
                        var requestLibrary = new ActivityLibraryDC
                        {
                            Incaller = Assembly.GetExecutingAssembly().GetName().Name,
                            IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                            Id = targetDC.ActivityLibraryId
                        };
                        // Get the list
                        try
                        {
                            activityLibraryList = client.ActivityLibraryGet(requestLibrary);
                        }

                        catch (FaultException<ServiceFault> ex)
                        {
                            throw new CommunicationException(ex.Detail.ErrorMessage);
                        }
                        catch (FaultException<ValidationFault> ex)
                        {
                            throw new BusinessValidationException(ex.Detail.ErrorMessage);
                        }
                        catch (Exception ex)
                        {
                            throw new CommunicationException(ex.Message);
                        }

                        // Get the First one or null, should only be one
                        targetLibrary = activityLibraryList.FirstOrDefault();
                        workflowActivityTemplateItemAssembly = DataContractTranslator.ActivityLibraryDCToActivityAssemblyItem(targetLibrary);

                        // download dependencies
                        references = Caching.CacheAndDownloadAssembly(client, Caching.ComputeDependencies(client, workflowActivityTemplateItemAssembly));
                    });

                    workflowActivityTemplateItem = DataContractTranslator.StoreActivitiyDCToWorkflowItem(targetDC, workflowActivityTemplateItemAssembly, references);
                    workflowActivityTemplateItem.WorkflowType = SelectedWorkflowTemplateItem.WorkflowTypeName;
                    workflowActivityTemplateItem.IsOpenFromServer = false;
                }
            }
            return workflowActivityTemplateItem;   // Return the ActivityItem
        }

        private WorkflowItem GetBlankProject()
        {
            WorkflowItem workflowItem = null;

            workflowItem = new WorkflowItem(WorkflowClassName, WorkflowName, Properties.Resources.EmptyWorkflowTemplate, string.Empty)
                                   {
                                       CachingStatus = CachingStatus.None,
                                       Category = DefaultCategory,
                                       Name = WorkflowClassName,
                                       DisplayName = WorkflowName,
                                       Description = WorkflowName,
                                       FullName = WorkflowClassName,
                                       HasCodeBehind = false,
                                       IsSavedToServer = true,
                                       Version = DefaultVersion,
                                       IsDataDirty = false,
                                       Tags = DefaultTags,
                                       WorkflowType = DefaultWorkflowTemplate,
                                   };

            return workflowItem;
        }

        /// <summary>
        /// Turns an inbound name into a valid Class name in C#
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string CleanClassName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var regex = new Regex(ClassNameReplaceRegularExpression);
            return regex.Replace(name, string.Empty);
        }

        public override void Validate() {
            base.Validate();

            var regex = new Regex(CommonMessages.ClassNameRegularExpression);
            if (string.IsNullOrEmpty(WorkflowName) || !regex.IsMatch(WorkflowName)) {
                IsValid = false;
                ErrorMessage += CommonMessages.WorkflowNameErrorString;
                ErrorMessage = ErrorMessage.Trim();
            }
            else if ((!IsCreatingBlank) && (null == SelectedWorkflowTemplateItem)
                ) {
                IsValid = false;
                ErrorMessage += SelectATemplateErrorString;
                ErrorMessage = ErrorMessage.Trim();
            }
            else if (this.SelectedLocation == null) {
                IsValid = false;
                ErrorMessage += SelectALocationErrorString;
                ErrorMessage = ErrorMessage.Trim();
            }
        }
    }
}
