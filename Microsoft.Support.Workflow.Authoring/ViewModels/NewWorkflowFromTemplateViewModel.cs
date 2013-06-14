// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewWorkflowFromTemplateViewModel.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.ViewModels
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.Text;
    using System.Text.RegularExpressions;
    using CWF.DataContracts;
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.ViewModel;
    using Microsoft.Support.Workflow.Authoring.Models;
    using Microsoft.Support.Workflow.Authoring.Properties;
    using Microsoft.Support.Workflow.Authoring.Services;
    using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;
    using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;

    /// <summary>
    /// Defines the ViewModel for New WorkflowFromTemplate
    /// </summary>
    public sealed class NewWorkflowFromTemplateViewModel : NotificationObject
    {

        /// <summary>
        /// The item created by the view model
        /// </summary>
        private WorkflowItem _createdWorkflowItem = null;

        /// <summary>
        /// The list of Templates that we can select from
        /// </summary>
        private List<WorkflowTemplateItem> _selectableTemplates = null;

        /// <summary>
        /// The selected item in the combobox
        /// </summary>
        private WorkflowTemplateItem _selectedWorkflowTemplateItem = null;

        /// <summary>
        /// The name of the Workflow
        /// </summary>
        private string _workflowName = null;

        /// <summary>
        /// The name of the Workflow class to create
        /// </summary>
        private string _workflowClassName = null;

        /// <summary>
        /// Flag to tell when we are Initialized
        /// </summary>
        private bool _isInitialized = false;

        /// <summary>
        /// Pluggable for testability
        /// </summary>
        private Func<IWorkflowsQueryService, WorkflowItem> GetWorkflowTemplateActivity { get; set; }

        /// <summary>
        /// Delegate command to create the WorkflowItem 
        /// </summary>
        public DelegateCommand CreateWorkflowItem { get; set; }


        /// <summary>
        /// ctor
        /// </summary>
        public NewWorkflowFromTemplateViewModel(IWorkflowsQueryService client)
        {
            Initialize(client);
        }


        /// <summary>
        /// The name of the workflow to create DisplayName
        /// </summary>
        public string WorkflowName
        {
            get
            {
                return _workflowName;
            }
            set
            {
                _workflowName = value;
                RaisePropertyChanged("WorkflowName");
            }
        }

        /// <summary>
        /// User-entered: the name of the workflow to create.
        /// </summary>
        public string WorkflowClassName
        {
            get
            {
                return _workflowClassName;
            }
            set
            {
                _workflowClassName = value;
                RaisePropertyChanged("WorkflowClassName");
            }
        }

        /// <summary>
        /// The item that was created by the View Model
        /// </summary>
        public WorkflowItem CreatedItem
        {
            get
            {
                return _createdWorkflowItem;
            }
            private set
            {
                _createdWorkflowItem = value;
                RaisePropertyChanged("CreatedItem");
            }
        }

        /// <summary>
        /// The list of templates that can be selected
        /// </summary>
        public List<WorkflowTemplateItem> SelectWorkflowTemplates
        {
            get
            {
                return _selectableTemplates;
            }
            private set
            {
                _selectableTemplates = value;
                RaisePropertyChanged("SelectWorkflowTemplates");
            }
        }

        /// <summary>
        /// The item that the user has selected in the Combo box
        /// </summary>
        public WorkflowTemplateItem SelectedWorkflowTemplateItem
        {
            get
            {
                return _selectedWorkflowTemplateItem;
            }
            set
            {
                _selectedWorkflowTemplateItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }

        /// <summary>
        /// If true then the ViewModel is Initialized
        /// </summary>
        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            private set
            {
                _isInitialized = value;
                RaisePropertyChanged("IsInitialized");
            }

        }


        /// <summary>
        /// Method to get the list of templates
        /// </summary>
        /// <returns></returns>
        public static List<WorkflowTemplateItem> GetListOfTemplates(IWorkflowsQueryService client)
        {
            List<WorkflowTemplateItem> templateList = new List<WorkflowTemplateItem>();
            WorkflowTypeGetReplyDC replyDC = null;

            replyDC = client.WorkflowTypeGet();

            if (null != replyDC && null != replyDC.StatusReply && 0 == replyDC.StatusReply.Errorcode)
            {
                foreach (WorkflowTypesGetBase wfType in replyDC.WorkflowActivityType)
                {
                    // Create a Workflow Template Item without XAML and add it to the Template list
                    WorkflowTemplateItem newItem = new WorkflowTemplateItem(wfType.WorkflowTemplateId, wfType.Name);
                    templateList.Add(newItem);
                }
            }
            // Return the list
            return templateList;
        }


        /// <summary>
        /// Initialize the ViewModel
        /// </summary>
        private void Initialize(IWorkflowsQueryService client)
        {
            GetWorkflowTemplateActivity = GetWorkflowTemplateActivityExecute;
            InitializeNotifications();
            InitializeData(client);
            InitializeCommands();
            IsInitialized = true;
        }

        /// <summary>
        /// Get any data that we need
        /// </summary>
        private void InitializeData(IWorkflowsQueryService client)
        {
            SelectWorkflowTemplates = NewWorkflowFromTemplateViewModel.GetListOfTemplates(client);
        }

        /// <summary>
        /// Event handlers are setup here 
        /// </summary>
        private void InitializeNotifications()
        {
            PropertyChanged += NewWorkflowFromTemplateViewModel_PropertyChanged;
        }

        /// <summary>
        /// Initialize the commands
        /// </summary>
        private void InitializeCommands()
        {
            // Set up the Create Workflow command and the CanExecute
            CreateWorkflowItem = new DelegateCommand(CreateWorkflowItemExecute, CanExecuteCreateWorkflowItem);
        }

        /// <summary>
        /// Execution for creating a Workflowitem
        /// </summary>
        private void CreateWorkflowItemExecute()
        {
            if (null == SelectedWorkflowTemplateItem)
            {
                // This should NEVER happen since the CanExecute should keep us from executing this command
                throw new ArgumentNullException();
            }

            // if we get here then it's okay to create a new workflow 
            // We have to get the referenced XAML from the specified StoreActivity 
            Utility.UsingClient(client =>
            {
                CreatedItem = GetWorkflowTemplateActivity(client);
                CreatedItem.Name = WorkflowClassName;
                CreatedItem.DisplayName = WorkflowName;
            });

        }

        /// <summary>
        /// Can Execute to manage usability of the UI
        /// </summary>
        /// <returns>True if Command Can Execute, false otherwise</returns>
        private bool CanExecuteCreateWorkflowItem()
        {
            return true == IsInitialized &&
                   null != SelectedWorkflowTemplateItem &&
                   !string.IsNullOrEmpty(WorkflowName);
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

            }
        }

        /// <summary>
        /// Gets the Workflow Template Activity Item based on the Selected Workflow Template, and its dependencies
        /// </summary>
        /// <returns>The Activity Item that will have the XAML required to create the workflow from the specified Workflow Template</returns>
        private WorkflowItem GetWorkflowTemplateActivityExecute(IWorkflowsQueryService client)
        {
            List<ActivityLibraryDC> activityLibraryList;
            WorkflowItem workflowActivityTemplateItem = null;
            // Throw if nothing selected CanExecute should prevent this
            if (null == SelectedWorkflowTemplateItem)
                throw new ArgumentNullException();

            List<StoreActivitiesDC> storeActivitiesList = null;
            ActivityAssemblyItem workflowActivityTemplateItemAssembly = null;
            StoreActivitiesDC targetDC = null;
            ActivityLibraryDC targetLibrary = null;

            // Create the Activity request
            StoreActivitiesDC requestActivity = new StoreActivitiesDC()
                                                    {
                                                        Incaller = Assembly.GetExecutingAssembly().GetName().Name,
                                                        IncallerVersion =
                                                            Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                                                        Id = SelectedWorkflowTemplateItem.WorkflowTemplateId
                                                    };
            // Get the List of Activities that qualify, should only be one
            storeActivitiesList = client.StoreActivitiesGet(requestActivity);

            if (null != storeActivitiesList)
            {
                // Get the first or one and only StoreActivity
                targetDC = storeActivitiesList.First();
                // We have to get the Activity Library associated with the Store Activity
                if (0 != targetDC.ActivityLibraryId)
                {
                    // Create the Library request
                    var requestLibrary = new ActivityLibraryDC()
                                             {
                                                 Incaller = Assembly.GetExecutingAssembly().GetName().Name,
                                                 IncallerVersion =
                                                     Assembly.GetExecutingAssembly().GetName().Version.ToString(),
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

                    var dependencies = Caching.ComputeDependencies(client, workflowActivityTemplateItemAssembly);
                    Caching.DownloadAndCacheAssembly(client, dependencies);

                    workflowActivityTemplateItem = DataContractTranslator.StoreActivitiyDCToWorkflowItem(targetDC,
                                                                                                         workflowActivityTemplateItemAssembly);
                    workflowActivityTemplateItem.WorkflowType = SelectedWorkflowTemplateItem.WorkflowTypeName;
                }
            }
            // Return the ActivityItem
            return workflowActivityTemplateItem;
        }

        /// <summary>
        /// Turns an inbound name into a valid Class name in C#
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string CleanClassName(string name)
        {
            Regex regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
            return regex.Replace(name, string.Empty);
        }

    }
}
