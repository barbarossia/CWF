using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CWF.DataContracts;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    public class SelectWorkflowViewModel : ViewModelBase
    {
        private const int pageSize = 13;
        private const string DefaultSortColumn = "Name";
        private StoreActivitiesDC selectedActivity;
        private ObservableCollection<StoreActivitiesDC> activities;
        private bool filterOldVersions;
        private string searchFilter;
        private DataPagingViewModel dpViewModel;

        #region commands
        public DelegateCommand SearchCommand { get; set; }
        #endregion

        public StoreActivitiesDC SelectedActivity
        {
            get { return this.selectedActivity; }
            set
            {
                this.selectedActivity = value;
                RaisePropertyChanged(() => SelectedActivity);
            }
        }

        public ObservableCollection<StoreActivitiesDC> Activities
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

        #endregion

        public SelectWorkflowViewModel()
        {
            SearchCommand = new DelegateCommand(new Action(SearchCommandExecuted));
            this.DataPagingVM = new DataPagingViewModel();
            this.DataPagingVM.SearchExecute = this.LoadData;
            this.DataPagingVM.PageSize = pageSize;
        }

        public void LoadData()
        {
            Utility.WithContactServerUI(() => WorkflowsQueryServiceUtility.UsingClient(this.GetActivities)); ;
        }

        #region private methods

        private void SearchCommandExecuted()
        {
            this.DataPagingVM.ResetPageIndex = true;
            this.LoadData();
        }

        private void GetActivities(IWorkflowsQueryService client)
        {
            ActivitySearchRequestDC request = new ActivitySearchRequestDC();
            request.SearchText = this.SearchFilter;
            request.PageNumber = this.DataPagingVM.ResetPageIndex ? 1 : this.DataPagingVM.PageIndex;
            request.PageSize = pageSize;
            request.FilterByCreator = false;
            request.FilterByDescription = false;
            request.FilterByName = true;
            request.FilterByVersion = false;
            request.FilterByTags = false;
            request.FilterByType = false;
            request.FilterOlder = this.FilterOldVersions;

            try
            {
                ActivitySearchReplyDC searchResults = null;
                searchResults = client.SearchActivities(request);
                this.DataPagingVM.ResultsLength = searchResults.ServerResultsLength;
                this.Activities = new ObservableCollection<StoreActivitiesDC>(searchResults.SearchResults);
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
