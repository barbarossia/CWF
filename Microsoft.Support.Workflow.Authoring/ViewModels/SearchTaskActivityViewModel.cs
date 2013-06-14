using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using System.ServiceModel;
using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using System.Reflection;
using System.ComponentModel;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    public class SearchTaskActivityViewModel : ViewModelBase
    {
        private const int pageSize = 13;
        private const string DefaultSortColumn = "Name";
        private TaskActivityDC selectedActivity;
        private ObservableCollection<TaskActivityDC> activities;
        private bool filterOldVersions = true;
        private bool hideUnassignedTasks = true;
        private string searchFilter;
        private DataPagingViewModel dpViewModel;
        private bool sortByAscending;
        private string sortColumn;
        private ListSortDirection sortDirection = ListSortDirection.Ascending;

        #region commands
        public DelegateCommand SearchCommand { get; set; }
        public DelegateCommand OpenActivityCommand { get; set; }
        public DelegateCommand<string> SortCommand { get; set; }
        #endregion

        public TaskActivityDC SelectedActivity
        {
            get { return this.selectedActivity; }
            set
            {
                this.selectedActivity = value;
                RaisePropertyChanged(() => SelectedActivity);
            }
        }

        public ObservableCollection<TaskActivityDC> Activities
        {
            get { return this.activities; }
            set
            {
                this.activities = value;
                RaisePropertyChanged(() => Activities);
            }
        }

        #region search workflows

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
                DataPagingVM.ResetPageIndex = true;
                filterOldVersions = value;
                RaisePropertyChanged(() => FilterOldVersions);
                WorkflowsQueryServiceUtility.UsingClient(this.GetActivities);
            }
        }

        public bool HideUnassignedTasks
        {
            get { return this.hideUnassignedTasks; }
            set
            {
                this.DataPagingVM.ResetPageIndex = true;
                hideUnassignedTasks = value;
                RaisePropertyChanged(() => HideUnassignedTasks);
                WorkflowsQueryServiceUtility.UsingClient(this.GetActivities);
            }
        }

        /// <summary>
        /// Sort by column name
        /// </summary>
        public string SortColumn
        {
            get { return this.sortColumn; }
            set
            {
                this.sortColumn = value;
                RaisePropertyChanged(() => SortColumn);
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
                SearchCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => SearchFilter);
            }
        }

        /// <summary>
        /// Gets or sets a indicator if sort by asc
        /// </summary>
        public bool SortByAsceinding
        {
            get { return this.sortByAscending; }
            set
            {
                this.sortByAscending = value;
                RaisePropertyChanged(() => this.SortByAsceinding);
            }
        }


        #endregion

        public SearchTaskActivityViewModel()
        {
            SearchCommand = new DelegateCommand(new Action(SearchCommandExecuted));
            OpenActivityCommand = new DelegateCommand(GetSelectedActivity);
            SortCommand = new DelegateCommand<string>(new Action<string>(SortCommandExecute));
            this.DataPagingVM = new DataPagingViewModel();
            this.DataPagingVM.SearchExecute = this.LoadData;
            this.DataPagingVM.PageSize = pageSize;
        }

        public void LoadData()
        {
            Utility.WithContactServerUI(() => WorkflowsQueryServiceUtility.UsingClient(this.GetActivities)); ;
        }

        #region private methods

        /// <summary>
        /// The sort command is being executed
        /// </summary>
        /// <param name="columnName"></param>
        private void SortCommandExecute(string columnName)
        {
            SortColumn = columnName;
            sortDirection = sortDirection == ListSortDirection.Descending ? ListSortDirection.Ascending : ListSortDirection.Descending;
            this.DataPagingVM.ResetPageIndex = true;
            LoadData();
        }

        private void GetSelectedActivity()
        {
            if (this.SelectedActivity != null)
            {
                using (var client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
                {
                    var taskActivity = new TaskActivityDC();
                    taskActivity.Incaller = Assembly.GetExecutingAssembly().GetName().Name;
                    taskActivity.IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    taskActivity.Id = this.SelectedActivity.Id;
                    var result = client.TaskActivityGet(taskActivity);
                    if (result.StatusReply.Errorcode == 0)
                        this.SelectedActivity = result;
                    else
                        MessageBoxService.ShowError(result.StatusReply.ErrorMessage);
                }
            }
        }

        private void SearchCommandExecuted()
        {
            this.DataPagingVM.ResetPageIndex = true;
            this.LoadData();
        }

        private void GetActivities(IWorkflowsQueryService client)
        {
            TaskActivityGetRequestDC request = new TaskActivityGetRequestDC();
            request.Incaller = Assembly.GetExecutingAssembly().GetName().Name;
            request.IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            request.AssignedTo = Environment.UserName;
            request.FilterOlder = this.FilterOldVersions;
            request.PageSize = pageSize;
            request.PageNumber = this.DataPagingVM.ResetPageIndex ? 1 : this.DataPagingVM.PageIndex;
            request.SortColumn = SortColumn;
            request.SortAscending = sortDirection == ListSortDirection.Ascending;
            request.SearchText = this.SearchFilter;
            request.HideUnassignedTasks = this.HideUnassignedTasks;
            try
            {
                TaskActivityGetReplyDC searchResults = null;
                searchResults = client.SearchTaskActivities(request);
                this.DataPagingVM.ResultsLength = searchResults.ServerResultsLength;
                if (searchResults.StatusReply.Errorcode != 0)
                {
                    IsBusy = false;
                    throw new UserFacingException(searchResults.StatusReply.ErrorMessage);
                }
                else
                    this.Activities = new ObservableCollection<TaskActivityDC>(searchResults.List);
            }
            catch (FaultException<ServiceFault> ex)
            {
                IsBusy = false;
                throw new CommunicationException(ex.Detail.ErrorMessage);
            }
            catch (FaultException<ValidationFault> ex)
            {
                IsBusy = false;
                throw new BusinessValidationException(ex.Detail.ErrorMessage);
            }
            catch (Exception ex)
            {
                IsBusy = false;
                throw new CommunicationException(ex.Message);
            }
        }
        #endregion
    }
}
