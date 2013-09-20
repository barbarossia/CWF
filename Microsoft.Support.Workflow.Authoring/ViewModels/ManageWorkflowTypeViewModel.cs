using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CWF.DataContracts;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.Security;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    public class ManageWorkflowTypeViewModel : ViewModelBase
    {
        private const string DefaultSortColumn = "Name";
        private const int pageSize = 13;

        private bool sortByAscending;
        private string sortColumn;
        private string searchText;
        private WorkflowTypeSearchDC selectedWorkflowType;
        private ObservableCollection<WorkflowTypeSearchDC> workflowTypes;
        private EditWorkflowTypeViewModel editWorkflowTypeViewModel;
        private bool isEditing;
        private DataPagingViewModel dpViewModel;
        private Env selectedEnv = Env.Dev;
        private List<Env> envFilters;
        /// <summary>
        /// Gets or sets SearchWorkflowTypeCommand
        /// </summary>
        public DelegateCommand SearchWorkflowTypeCommand { get; set; }

        /// <summary>
        /// Gets or sets AddWorkTypeflowCommand
        /// </summary>
        public DelegateCommand AddWorkTypeflowTypeCommand { get; set; }

        /// <summary>
        /// Gets or sets EditWorkflowCommand
        /// </summary>
        public DelegateCommand EditWorkflowTypeCommand { get; set; }

        /// <summary>
        ///  Gets or sets DeleteWorkflowTypeCommand
        /// </summary>
        public DelegateCommand DeleteWorkflowTypeCommand { get; set; }

        /// <summary>
        /// Upload Workflow type changes to server
        /// </summary>
        public DelegateCommand UploadWorkflowCommand { get; set; }

        /// <summary>
        /// Cancel workflow changes
        /// </summary>
        public DelegateCommand CancelEditWorkflowCommand { get; set; }

        #region search workfklow type

        public List<Env> EnvFilters
        {
            get { return this.envFilters; }
            set
            {
                this.envFilters = value;
                RaisePropertyChanged(() => this.EnvFilters);
            }
        }

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

        /// <summary>
        ///Gets or sets SearchTxt to search worklfow type
        /// </summary>
        public string SearchText
        {
            get { return this.searchText; }
            set
            {
                this.searchText = value;
                RaisePropertyChanged(() => this.SearchText);
            }
        }

        #endregion

        public Env SelectedEnv
        {
            get { return this.selectedEnv; }
            set
            {
                this.selectedEnv = value;
                RaisePropertyChanged(() => this.SelectedEnv);
                this.SearchCommandExecuted();
                this.AddWorkTypeflowTypeCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the EditWorkflowTypeViewModel
        /// </summary>
        public EditWorkflowTypeViewModel EditWorkflowTypeVM
        {
            get { return this.editWorkflowTypeViewModel; }
            set
            {
                this.editWorkflowTypeViewModel = value;
                RaisePropertyChanged(() => this.EditWorkflowTypeVM);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if a workflow type is being edited
        /// </summary>
        public bool IsEditing
        {
            get { return this.isEditing; }
            set
            {
                this.isEditing = value;
                RaisePropertyChanged(() => this.IsEditing);
            }
        }

        /// <summary>
        /// Gets or sets SelectedWorkflowType
        /// </summary>
        public WorkflowTypeSearchDC SelectedWorkflowType
        {
            get { return this.selectedWorkflowType; }
            set
            {
                this.selectedWorkflowType = value;
                RaisePropertyChanged(() => SelectedWorkflowType);
            }
        }

        /// <summary>
        /// Gets or sets WorkflowTypes
        /// </summary>
        public ObservableCollection<WorkflowTypeSearchDC> WorkflowTypes
        {
            get { return this.workflowTypes; }
            set
            {
                this.workflowTypes = value;
                RaisePropertyChanged(() => WorkflowTypes);
            }
        }

        /// <summary>
        /// constructor, initialize commands
        /// </summary>
        public ManageWorkflowTypeViewModel()
        {
            this.SearchWorkflowTypeCommand = new DelegateCommand(SearchCommandExecuted);
            this.AddWorkTypeflowTypeCommand = new DelegateCommand(this.OnAddWorkflowType,()=>(int)this.SelectedEnv>0);
            this.EditWorkflowTypeCommand = new DelegateCommand(this.OnEditWorkflowType);
            this.DeleteWorkflowTypeCommand = new DelegateCommand(() => { WorkflowsQueryServiceUtility.UsingClient(this.DeleteWorkflowType); });
            this.UploadWorkflowCommand = new DelegateCommand(UploadWorkflowType, this.CanUploadWorkflowType);
            this.CancelEditWorkflowCommand = new DelegateCommand(() => { IsEditing = false; });
            this.DataPagingVM = new DataPagingViewModel();
            this.DataPagingVM.SearchExecute = this.LoadData;
            this.DataPagingVM.PageSize = pageSize;
            this.InitializeEnvs();
        }

        private void InitializeEnvs()
        {
            this.EnvFilters = AuthorizationService.GetAuthorizedEnvs(Permission.ManageWorkflowType).ToList();
            if (this.EnvFilters.Contains(DefaultValueSettings.Environment))
                this.SelectedEnv = DefaultValueSettings.Environment;
        }

        private bool CanUploadWorkflowType()
        {
            if (this.EditWorkflowTypeVM == null)
                return false;
            if (this.EditWorkflowTypeVM.Name == null || string.IsNullOrEmpty(this.EditWorkflowTypeVM.Name.Trim()))
                return false;
            if (EditWorkflowTypeVM.SelectedAuthGroup == null)
                return false;
            return true;
        }

        private void RefreshUploadCommandExecute()
        {
            this.UploadWorkflowCommand.RaiseCanExecuteChanged();
        }

        private void SearchCommandExecuted()
        {
            this.DataPagingVM.ResetPageIndex = true;
            //WorkflowsQueryServiceUtility.UsingClient(this.GetWorkflowTypes);
            this.LoadData();
        }

        /// <summary>
        /// load workflowtype from query service
        /// </summary>
        public void LoadData()
        {
            Utility.WithContactServerUI(() => WorkflowsQueryServiceUtility.UsingClient(this.GetWorkflowTypes));
        }

        /// <summary>
        /// Delete WorkflowType by Id
        /// </summary>
        /// <param name="workflowTypeId"></param>
        private void DeleteWorkflowType(IWorkflowsQueryService client)
        {
            if (this.SelectedWorkflowType == null)
                return;
            if (this.SelectedWorkflowType.WorkflowsCount > 0)
            {
                MessageBoxService.ShowError(string.Format("The {0} can't be deleted, because there are workflows using it.", this.SelectedWorkflowType.Name));
                return;
            }

            WorkFlowTypeCreateOrUpdateRequestDC request = new WorkFlowTypeCreateOrUpdateRequestDC();
            request.SetIncaller();
            request.InId = this.SelectedWorkflowType.Id;
            request.IsDeleted = true;
            request.InAuthGroupId = this.SelectedWorkflowType.AuthGroupId;
            request.InPublishingWorkflowId = this.SelectedWorkflowType.PublishingWorkflowId;
            request.InWorkflowTemplateId = this.SelectedWorkflowType.WorkflowTemplateId;
            request.Environment = this.SelectedWorkflowType.Environment;
            WorkFlowTypeCreateOrUpdateReplyDC reply = client.WorkflowTypeCreateOrUpdate(request);
            if (reply != null && reply.StatusReply != null)
                reply.StatusReply.CheckErrors();
            this.GetWorkflowTypes(client);
        }

        /// <summary>
        /// Prepare to Add workflow type
        /// </summary>
        private void OnAddWorkflowType()
        {
            try
            {
                WorkflowTypeSearchDC type = new WorkflowTypeSearchDC();
                type.Guid = Guid.NewGuid();
                type.Environment = this.SelectedEnv.ToString();
                EditWorkflowTypeVM = new EditWorkflowTypeViewModel(Common.WorkflowTypeOperations.Add, type, RefreshUploadCommandExecute);
                IsEditing = true;
                this.UploadWorkflowCommand.RaiseCanExecuteChanged();
            }
            catch (Exception e)
            {
                throw new UserFacingException(e.Message);
            }
        }

        /// <summary>
        /// Prepare to edit workflow type
        /// </summary>
        private void OnEditWorkflowType()
        {
            try
            {
                if (this.SelectedWorkflowType == null)
                    return;
                EditWorkflowTypeVM = new EditWorkflowTypeViewModel(Common.WorkflowTypeOperations.Edit, this.SelectedWorkflowType, RefreshUploadCommandExecute);
                IsEditing = true;
                this.UploadWorkflowCommand.RaiseCanExecuteChanged();
            }
            catch (Exception e)
            {
                throw new UserFacingException(e.Message);
            }
        }

        /// <summary>
        /// Upload a workflow type to query service
        /// </summary>
        private void UploadWorkflowType()
        {
            try
            {
                WorkflowsQueryServiceUtility.UsingClient(this.EditWorkflowTypeVM.UploadWorkflowType);
                IsEditing = false;
                WorkflowsQueryServiceUtility.UsingClient(this.GetWorkflowTypes);
                this.EditWorkflowTypeVM = null;
            }
            catch (Exception e)
            {
                throw new UserFacingException(e.Message);
            }
        }

        /// <summary>
        /// Gets workflowTypes from QueryService
        /// </summary>
        /// <param name="client"></param>
        private void GetWorkflowTypes(IWorkflowsQueryService client)
        {
            WorkflowTypeSearchReply replyDC = null;
            WorkflowTypeSearchRequest request = new WorkflowTypeSearchRequest();
            request.SearchText = this.SearchText;
            request.PageSize = pageSize;
            request.PageNumber = this.DataPagingVM.ResetPageIndex ? 1 : this.DataPagingVM.PageIndex;
            request.SortColumn = DefaultSortColumn;
            request.SortAscending = this.SortByAsceinding;
            if (this.SelectedEnv > 0 && Enum.IsDefined(typeof(Env), this.SelectedEnv))
                request.Environments = new List<string>() { this.SelectedEnv.ToString() };

            replyDC = client.SearchWorkflowTypes(request);
            if (null != replyDC && null != replyDC.StatusReply && 0 == replyDC.StatusReply.Errorcode)
            {
                this.WorkflowTypes = new ObservableCollection<WorkflowTypeSearchDC>(replyDC.SearchResults);
                DataPagingVM.ResultsLength = replyDC.ServerResultsLength;
            }
            else
            {
                this.WorkflowTypes = new ObservableCollection<WorkflowTypeSearchDC>();
                DataPagingVM.ResultsLength = 0;
            }
        }
    }
}
