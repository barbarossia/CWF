using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DataContracts;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Reflection;
using System.ServiceModel;
using System.Windows;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    public class EditWorkflowTypeViewModel : ViewModelBase
    {
        private const string addWorkflowType = "Add Workflow Type";
        private const string editWorkflowType = "Edit Workflow Type";
        private const string nameTooLong = "The Name is too long.";
        private const int maxNameLength = 50;
        private WorkflowTypeSearchDC workflowType;
        private string windowTitle;
        private string name;
        private int templateId;
        private string template;
        private int publishingWorkflowId;
        private string publishingWorkflow;
        private List<AuthorizationGroupDC> authGroups;
        private AuthorizationGroupDC selectAuthGroup;
        private Action refreshCommandCanExecute;

        #region commands

        /// <summary>
        /// Gets or sets BroswerPublishingWorkflowsCommand
        /// </summary>
        public DelegateCommand BroswerPublishingWorkflowsCommand { get; set; }

        /// <summary>
        /// gets or sets BroswerTemplateWorkflowsCommand
        /// </summary>
        public DelegateCommand BroswerTemplateWorkflowsCommand { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets SelectedAuthGroup
        /// </summary>
        public AuthorizationGroupDC SelectedAuthGroup
        {
            get { return this.selectAuthGroup; }
            set
            {
                this.selectAuthGroup = value;
                RaisePropertyChanged(() => this.SelectedAuthGroup);
                if (refreshCommandCanExecute != null)
                    refreshCommandCanExecute();
            }
        }

        /// <summary>
        /// Gets or sets AuthGroups
        /// </summary>
        public List<AuthorizationGroupDC> AuthGroups
        {
            get { return this.authGroups; }
            set
            {
                this.authGroups = value;
                RaisePropertyChanged(() => this.AuthGroups);

            }
        }

        /// <summary>
        /// Gets or sets WindowTitle
        /// </summary>
        public string WindowTitle
        {
            get { return this.windowTitle; }
            set
            {
                this.windowTitle = value;
                RaisePropertyChanged(() => this.WindowTitle);
            }
        }

        /// <summary>
        /// Gets or sets workflow type name
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                RaisePropertyChanged(() => Name);
                if (refreshCommandCanExecute != null)
                    refreshCommandCanExecute();
            }
        }

        /// <summary>
        /// Gets or sets PublishingWorkflowId
        /// </summary>
        public int PublishingWorkflowId
        {
            get { return this.publishingWorkflowId; }
            set
            {
                this.publishingWorkflowId = value;
                RaisePropertyChanged(() => this.PublishingWorkflowId);
            }
        }

        /// <summary>
        /// Gets or sets PublishingWorkflow
        /// </summary>
        public string PublishingWorkflow
        {
            get { return this.publishingWorkflow; }
            set
            {
                this.publishingWorkflow = value;
                RaisePropertyChanged(() => this.PublishingWorkflow);
            }
        }

        /// <summary>
        /// Gets or sets TemplateId
        /// </summary>
        public int TemplateId
        {
            get { return this.templateId; }
            set
            {
                this.templateId = value;
                RaisePropertyChanged(() => TemplateId);
            }
        }

        /// <summary>
        /// Gets or sets Template
        /// </summary>
        public string Template
        {
            get { return this.template; }
            set
            {
                this.template = value;
                RaisePropertyChanged(() => this.Template);
            }
        }

        public EditWorkflowTypeViewModel(WorkflowTypeOperations operations, WorkflowTypeSearchDC workflowTypeParam, Action canUploadWorkflowType)
        {
            this.refreshCommandCanExecute = canUploadWorkflowType;
            this.BroswerPublishingWorkflowsCommand = new DelegateCommand(BrowserPublishingWorkflows);
            this.BroswerTemplateWorkflowsCommand = new DelegateCommand(BrowserTemplateWorkflows);
            if (workflowTypeParam != null)
                this.workflowType = workflowTypeParam;
            else
            {
                this.workflowType = new WorkflowTypeSearchDC();
                this.workflowType.Guid = Guid.NewGuid();
            }

            if (operations == WorkflowTypeOperations.Add)
                this.WindowTitle = addWorkflowType;
            else
                this.WindowTitle = editWorkflowType;
            this.Name = this.workflowType.Name;
            this.PublishingWorkflow = this.workflowType.PublishingWorkflow;
            this.PublishingWorkflowId = this.workflowType.PublishingWorkflowId;
            this.Template = this.workflowType.WorkflowTemplate;
            this.TemplateId = this.workflowType.WorkflowTemplateId;
            WorkflowsQueryServiceUtility.UsingClient(this.GetAuthGroups);
        }

        /// <summary>
        /// Upload workflowtype to server
        /// </summary>
        public void UploadWorkflowType(IWorkflowsQueryService client)
        {
            WorkFlowTypeCreateOrUpdateRequestDC request = new WorkFlowTypeCreateOrUpdateRequestDC();
            request.SetIncaller();
            request.InGuid = this.workflowType.Guid;
            request.InId = this.workflowType.Id;
            request.InName = this.Name;
            request.InPublishingWorkflowId = this.PublishingWorkflowId;
            request.InWorkflowTemplateId = this.TemplateId;
            request.InAuthGroupId = this.SelectedAuthGroup != null ? this.SelectedAuthGroup.AuthGroupId : 0;
            request.IsDeleted = false;
            request.Environment = this.workflowType.Environment;
            this.ValidWorkflowType(request);

            WorkFlowTypeCreateOrUpdateReplyDC reply = client.WorkflowTypeCreateOrUpdate(request);
            if (reply != null && reply.StatusReply != null)
            {
                try
                {
                    StatusReplyDC error = reply.StatusReply.CheckErrors();
                }
                catch (Exception ex)
                {
                    throw new UserFacingException(ex.Message);
                }
            }
        }


        #region private methods

        /// <summary>
        /// Open SelectWorkflow window and select template or publishingworkflow
        /// </summary>
        private void BrowserPublishingWorkflows()
        {
            SelectWorkflowViewModel viewModel = new SelectWorkflowViewModel(this.workflowType.Environment);
            viewModel.LoadData();
            if (DialogService.ShowDialog(viewModel).GetValueOrDefault() && viewModel.SelectedActivity != null)
            {
                this.PublishingWorkflowId = viewModel.SelectedActivity.Id;
                this.PublishingWorkflow = viewModel.SelectedActivity.Name;
            }
        }

        /// <summary>
        /// Open SelectWorkflow window and select template or publishingworkflow
        /// </summary>
        private void BrowserTemplateWorkflows()
        {
            SelectWorkflowViewModel viewModel = new SelectWorkflowViewModel(this.workflowType.Environment);
            viewModel.LoadData();
            if (DialogService.ShowDialog(viewModel).GetValueOrDefault() && viewModel.SelectedActivity != null)
            {
                this.TemplateId = viewModel.SelectedActivity.Id;
                this.Template = viewModel.SelectedActivity.Name;
            }
        }

        /// <summary>
        /// Get AuthorizationGroupDCs from query service
        /// </summary>
        /// <param name="client"></param>
        private void GetAuthGroups(IWorkflowsQueryService client)
        {
            AuthorizationGroupGetRequestDC request = new AuthorizationGroupGetRequestDC();
            request.SetIncaller();
            AuthorizationGroupGetReplyDC reply = client.GetAuthorizationGroups(request);
            if (reply != null && reply.StatusReply != null)
                reply.StatusReply.CheckErrors();
            this.AuthGroups = reply.AuthorizationGroups;
            if (this.workflowType != null)
                this.SelectedAuthGroup = this.AuthGroups.SingleOrDefault(a => a.AuthGroupId == this.workflowType.AuthGroupId);
            else
                this.SelectedAuthGroup = new AuthorizationGroupDC();
        }

        /// <summary>
        /// Validate workflow type 
        /// Name length
        /// </summary>
        /// <returns></returns>
        private bool ValidWorkflowType(WorkFlowTypeCreateOrUpdateRequestDC request) 
        {
            if (request == null)
                return false;
            if (request.InName.Length > maxNameLength)
            {
                throw new UserFacingException(nameTooLong);
            }
            return true;
        }

        #endregion
    }
}
