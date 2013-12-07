

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Data;
    using CWF.DataContracts;
    using Practices.Prism.Commands;
    using Practices.Prism.ViewModel;
    using Services;
    using System.ServiceModel;
    using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
    using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;
    using System.Threading;
    using System.Windows.Threading;
    using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
    using Microsoft.Support.Workflow.Authoring.AddIns.Data;
    using Microsoft.Support.Workflow.Authoring.Common;
    using System.Collections.Generic;
    using Microsoft.Support.Workflow.Authoring.Security;

    /// <summary>
    /// ViewModel for OpenWorkflowFromServerView
    /// </summary>
    public class OpenWorkflowFromServerViewModel : ViewModelBase
    {
        private const string DefaultSortColumn = "Name";

        private StoreActivitiesDC selectedWorkflow;
        private ObservableCollection<StoreActivitiesDC> existingWorkflows;
        private CollectionViewSource workflowsView;
        private bool shouldDownloadDependencies = false;
        private bool filterByName = true;
        private bool filterByDescription;
        private bool filterByType;
        private bool filterByTags = true;
        private bool filterByVersion;
        private bool filterOldVersions = true;
        private bool filterByCreatedBy = false;
        private string searchFilter;
        private int pageSize = 13;
        private string sortColumn = DefaultSortColumn;
        private ListSortDirection sortDirection = ListSortDirection.Ascending;
        private List<EnvFilter> envFilters;

        private DataPagingViewModel dpViewModel;
        /// <summary>
        /// Open workflow command. This is the main action for this viewmodel.
        /// </summary>
        public DelegateCommand OpenSelectedWorkflowCommand { get; set; }
        public DelegateCommand<string> SortCommand { get; set; }
        public DelegateCommand SearchCommand { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public OpenWorkflowFromServerViewModel()
        {
            SearchCommand = new DelegateCommand(new Action(SearchCommandExecuted));
            OpenSelectedWorkflowCommand = new DelegateCommand(delegate { GetSelectedWorkflow(); }, () => SelectedWorkflow != null);
            SortCommand = new DelegateCommand<string>(new Action<string>(SortCommandExecute));
            this.DataPagingVM = new DataPagingViewModel();
            this.DataPagingVM.SearchExecute = this.LoadData;
            this.DataPagingVM.PageSize = pageSize;
            ExistingWorkflows = new ObservableCollection<StoreActivitiesDC>();
            WorkflowsView = new CollectionViewSource();
            this.InitializeEnvs();
            CanEdit = true;
            OpenForEditing = DefaultValueSettings.OpenForEditingMode;
            ShouldDownloadDependencies = DefaultValueSettings.EnableDownloadDependecies;
        }

        private void InitializeEnvs()
        {
            Env[] envs = AuthorizationService.GetAuthorizedEnvs(Permission.OpenWorkflow);
            if (envs != null)
            {
                this.EnvFilters = envs.ToList().Select(e => new EnvFilter()
                    {
                        Env = e,
                        IsFilted = false
                    }).ToList();
                this.EnvFilters.ForEach(e =>
                {
                    if (e.Env == DefaultValueSettings.Environment)
                        e.IsFilted = true;
                });
            }
            else
                this.EnvFilters = new List<EnvFilter>();
        }

        private void SearchCommandExecuted()
        {

            this.DataPagingVM.ResetPageIndex = true;
            LoadData();
        }

        private void GetSelectedWorkflow()
        {
            if (SelectedWorkflow != null)
            {
                using (var client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
                {
                    selectedWorkflow.Incaller = Assembly.GetExecutingAssembly().GetName().Name;
                    selectedWorkflow.IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    var result = client.StoreActivitiesGet(SelectedWorkflow);
                    if (result.Any())
                    {
                        SelectedWorkflow = result[0];
                    }
                }
            }
        }

        /// <summary>
        /// The sort command is being executed
        /// </summary>
        /// <param name="columnName"></param>
        private void SortCommandExecute(string columnName)
        {
            sortColumn = columnName;
            sortDirection = sortDirection == ListSortDirection.Descending ? ListSortDirection.Ascending : ListSortDirection.Descending;
            LoadData();
        }

        /// <summary>
        /// Load list of workflows from the server
        /// </summary>
        public void LoadData()
        {
            if (!CanSearchWorkflows)
            {
                MessageBoxService.ShowInfo("Please specify an environment to search.");
                return;
            }
            using (var client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
            {
                LoadLiveData(client);
            }
        }

        public bool CanEdit { get; set; }
        public bool OpenForEditing { get; set; }

        /// <summary>
        /// Gets or sets DataPagingViewModel
        /// </summary>
        public DataPagingViewModel DataPagingVM
        {
            get { return this.dpViewModel; }
            set
            {
                this.dpViewModel = value;
                RaisePropertyChanged(() => DataPagingVM);
            }
        }

        public List<EnvFilter> EnvFilters
        {
            get { return this.envFilters; }
            set
            {
                this.envFilters = value;
                RaisePropertyChanged(() => this.EnvFilters);
            }
        }

        /// <summary>
        /// Choose if you want to download the dependency dlls when opening the workflow.
        /// </summary>
        public bool ShouldDownloadDependencies
        {
            get
            {
                return shouldDownloadDependencies;
            }
            set
            {
                shouldDownloadDependencies = value;
                RaisePropertyChanged(() => ShouldDownloadDependencies);
            }
        }

        /// <summary>
        /// Filter results by the name field
        /// </summary>
        public bool FilterByName
        {
            get
            {
                return filterByName;
            }
            set
            {
                filterByName = value;
                RaisePropertyChanged(() => FilterByName);
            }
        }

        /// <summary>
        /// Filter results by the description field
        /// </summary>
        public bool FilterByDescription
        {
            get
            {
                return filterByDescription;
            }
            set
            {
                filterByDescription = value;
                RaisePropertyChanged(() => FilterByDescription);
            }
        }

        /// <summary>
        /// Filter results by the type field
        /// </summary>
        public bool FilterByType
        {
            get
            {
                return filterByType;
            }
            set
            {
                filterByType = value;
                RaisePropertyChanged(() => FilterByType);
            }
        }

        /// <summary>
        /// Filter results by the tags field
        /// </summary>
        public bool FilterByTags
        {
            get
            {
                return filterByTags;
            }
            set
            {
                filterByTags = value;
                RaisePropertyChanged(() => FilterByTags);
            }
        }

        /// <summary>
        /// Filter results by the version field
        /// </summary>
        public bool FilterByVersion
        {
            get
            {
                return filterByVersion;
            }
            set
            {
                filterByVersion = value;
                RaisePropertyChanged(() => FilterByVersion);
            }
        }

        /// <summary>
        /// Filter results by the created by field
        /// </summary>
        public bool FilterByCreatedBy
        {
            get
            {
                return filterByCreatedBy;
            }
            set
            {
                filterByCreatedBy = value;
                RaisePropertyChanged(() => FilterByCreatedBy);
            }
        }

        /// <summary>
        /// View of the collection of workflows, used for filtering/searching
        /// </summary>
        public CollectionViewSource WorkflowsView
        {
            get
            {
                return workflowsView;
            }
            set
            {
                if (value != null)
                {
                    workflowsView = value;
                    RaisePropertyChanged(() => WorkflowsView);
                }
            }
        }

        /// <summary>
        /// Remove old versions of the workflows when displaying results
        /// </summary>
        public bool FilterOldVersions
        {
            get
            {
                return filterOldVersions;
            }
            set
            {
                filterOldVersions = value;
                RaisePropertyChanged(() => FilterOldVersions);
                this.DataPagingVM.ResetPageIndex = true;
                LoadData();
            }
        }

        /// <summary>
        /// The text to filter the results with.
        /// </summary>
        public string SearchFilter
        {
            get
            {
                return searchFilter;
            }
            set
            {
                searchFilter = value.Trim();
                RaisePropertyChanged(() => SearchFilter);
            }
        }

        /// <summary>
        /// OpenWorkflowFromServerView binds to this property to get the "return value"
        /// </summary>
        public StoreActivitiesDC SelectedWorkflow
        {
            get
            {
                return selectedWorkflow;
            }
            set
            {
                if (value != selectedWorkflow)
                {
                    selectedWorkflow = value;

                    CanEdit = selectedWorkflow == null || AuthorizationService.Validate(selectedWorkflow.Environment.ToEnv(), Permission.SaveWorkflow);
                    if (!CanEdit)
                        OpenForEditing = false;

                    RaisePropertyChanged(() => SelectedWorkflow);
                    RaisePropertyChanged(() => CanEdit);
                    RaisePropertyChanged(() => OpenForEditing);
                    OpenSelectedWorkflowCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Collection of the existing workflows
        /// </summary>
        public ObservableCollection<StoreActivitiesDC> ExistingWorkflows
        {
            get
            {
                return existingWorkflows;
            }
            set
            {
                existingWorkflows = value;
                RaisePropertyChanged(() => ExistingWorkflows);
            }
        }

        public bool CanSearchWorkflows
        {
            get
            {
                return this.EnvFilters.Any(i => i.IsFilted == true);
            }
        }

        /// <summary>
        /// Fill ExistingWorkflows collection with data
        /// </summary>
        [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]
        public void LoadLiveData(IWorkflowsQueryService client)
        {
            if (this.EnvFilters.All(a => !a.IsFilted))
            {
                WorkflowsView = new CollectionViewSource();
                existingWorkflows = new ObservableCollection<StoreActivitiesDC>();
                WorkflowsView.Source = existingWorkflows;
                return;
            }

            ActivitySearchRequestDC request = new ActivitySearchRequestDC();
            request.SearchText = SearchFilter;
            request.PageNumber = this.DataPagingVM.ResetPageIndex ? 1 : this.DataPagingVM.PageIndex;
            request.PageSize = pageSize;
            request.FilterByCreator = FilterByCreatedBy;
            request.FilterByDescription = FilterByDescription;
            request.FilterByName = filterByName;
            request.FilterByVersion = FilterByVersion;
            request.FilterByTags = FilterByTags;
            request.FilterByType = FilterByType;
            request.FilterOlder = FilterOldVersions;
            request.Environments = this.EnvFilters.Where(a => a.IsFilted).Select(e => e.Env.ToString()).ToList();

            if (!string.IsNullOrEmpty(sortColumn))
            {
                request.SortColumn = sortColumn.ToLower().Trim();
                request.SortAscending = sortDirection == ListSortDirection.Ascending;
            }

            Utility.WithContactServerUI(() =>
            {
                ActivitySearchReplyDC searchResults = client.SearchActivities(request);
                searchResults.StatusReply.CheckErrors();
                this.DataPagingVM.ResultsLength = searchResults.ServerResultsLength;
                ExistingWorkflows = new ObservableCollection<StoreActivitiesDC>(searchResults.SearchResults);
            });

            WorkflowsView = new CollectionViewSource();
            WorkflowsView.Source = existingWorkflows;
        }

    }
}
