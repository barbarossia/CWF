using Microsoft.Practices.Prism.Commands;
using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data = Microsoft.Support.Workflow.Authoring.AddIns.Data;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Microsoft.Support.Workflow.Authoring.Security;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    public class ChangeAuthorViewModel : ViewModelBase
    {
        private WorkflowItem currentProject;
        private Principal targetAuthor;
        private List<Principal> avaliableAuthors;
        private string originalAuthor;
        public DelegateCommand ChangeAuthorCommand { get; private set; }

        public bool IsSaved { get; private set; }
        public bool HasChanged { get { return this.ChangeAuthorCommand.CanExecute(); } }
        public List<Principal> AvaliableAuthors
        {
            get { return this.avaliableAuthors; }
            set
            {
                this.avaliableAuthors = value;
                RaisePropertyChanged(() => this.AvaliableAuthors);
            }
        }

        public Principal TargetAuthor
        {
            get { return this.targetAuthor; }
            set
            {
                this.targetAuthor = value;
                RaisePropertyChanged(() => TargetAuthor);
                RaiseCommandChanged();
            }
        }

        public Data.Env Environment
        {
            get { return this.currentProject.Env; }
        }

        public string ProjectName
        {
            get { return this.currentProject.WorkflowName; }
        }

        public string CreatedBy
        {
            get { return this.originalAuthor; }
            set
            {
                this.originalAuthor = value;
                RaisePropertyChanged(() => this.CreatedBy);
            }
        }

        public ChangeAuthorViewModel(WorkflowItem workflow)
        {
            this.currentProject = workflow;
            this.CreatedBy = this.currentProject.CreatedBy;
            RaisePropertyChanged(() => this.ProjectName);
            RaisePropertyChanged(() => this.Environment);
            this.GetAvaliableAuthors();
            this.ChangeAuthorCommand = new DelegateCommand(ChangeAuthorCommandExecute, ChangeAuthorCommandCanExecute);
        }

        private void RaiseCommandChanged()
        {
            this.ChangeAuthorCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged(() => this.HasChanged);
        }

        private void ChangeAuthorCommandExecute()
        {
            try
            {
                IsSaved = false;
                if (this.TargetAuthor.SamAccountName == this.CreatedBy)
                {
                    MessageBoxService.ShowError(TextResources.ChangeAuthorFailureDueToSameAuthorMsg);
                    return;
                }
                Utility.DoTaskWithBusyCaption(TextResources.Saving, () =>
                {
                    using (WorkflowsQueryServiceClient client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
                    {
                        ChangeAuthorRequest request = new ChangeAuthorRequest();
                        request.SetIncaller();
                        request.Name = this.currentProject.Name;
                        request.Version = this.currentProject.Version;
                        request.AuthorAlias = this.TargetAuthor.SamAccountName;
                        request.Environment = this.currentProject.Env.ToString();
                        this.currentProject.CreatedBy = this.TargetAuthor.SamAccountName;
                        ChangeAuthorReply reply = null;
                        reply = client.ChangeAuthor(request);
                        if (reply != null)
                            reply.StatusReply.CheckErrors();
                    }
                });
                var viewModel = new ChangeAuthorSummaryViewModel(this.ProjectName, this.CreatedBy, this.TargetAuthor.SamAccountName);
                DialogService.ShowDialog(viewModel);
                IsSaved = true;
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowException(ex, TextResources.ChangeAuthorFailureMsg);
            }
        }

        private bool ChangeAuthorCommandCanExecute()
        {
            return this.TargetAuthor != null;
        }

        private void GetAvaliableAuthors()
        {
            Utility.DoTaskWithBusyCaption(TextResources.Loading, () =>
            {
                AvaliableAuthors = AuthorizationService.GetAuthorizedPrincipals(Permission.SaveWorkflow, currentProject.Env);
            });
        }
    }
}
