using CWF.DataContracts;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    public class MoveProjectViewModel : ViewModelBase
    {
        private WorkflowItem currentProject;
        private WorkflowTypesGetBase workflowTemplate;
        private ObservableCollection<Env> avaliableNextStatus;
        private Env? nextStatus;
        private ObservableCollection<WorkflowTypesGetBase> workflowTemplates;

        public DelegateCommand MoveProjectCommand { get; private set; }

        public ObservableCollection<WorkflowTypesGetBase> WorkflowTemplates
        {
            get { return this.workflowTemplates; }
            set
            {
                this.workflowTemplates = value;
                RaisePropertyChanged(() => WorkflowTemplates);
            }
        }

        public Env? NextStatus
        {
            get { return this.nextStatus; }
            set
            {
                this.nextStatus = value;
                RaisePropertyChanged(() => this.NextStatus);
                this.MoveProjectCommand.RaiseCanExecuteChanged();
                if (NextStatus != null)
                    this.GetWorkflowTemplates(NextStatus.Value);
            }
        }

        public ObservableCollection<Env> AvaliableNextStatus
        {
            get { return this.avaliableNextStatus; }
            set
            {
                this.avaliableNextStatus = value;
                RaisePropertyChanged(() => this.AvaliableNextStatus);
            }
        }

        public string ProjectName
        {
            get { return this.currentProject.WorkflowName; }
        }

        public Env CurrentStatus
        {
            get { return this.currentProject.Env; }
        }

        public WorkflowTypesGetBase WorkflowTemplate
        {
            get { return this.workflowTemplate; }
            set
            {
                workflowTemplate = value;
                RaisePropertyChanged(() => WorkflowTemplate);
                this.MoveProjectCommand.RaiseCanExecuteChanged();
            }
        }

        public MoveProjectViewModel(WorkflowItem workflow)
        {
            this.currentProject = workflow;
            RaisePropertyChanged(() => ProjectName);
            RaisePropertyChanged(() => CurrentStatus);
            this.MoveProjectCommand = new DelegateCommand(MoveProjectCommandExecute, () => { return this.WorkflowTemplate != null && this.NextStatus != null; });
            this.InitializeNextStatus();
        }

        private void InitializeNextStatus()
        {
            switch (CurrentStatus)
            {
                case Env.Dev:
                    AvaliableNextStatus = new ObservableCollection<Env>() { Env.Test };
                    break;
                case Env.Test:
                    AvaliableNextStatus = new ObservableCollection<Env>() { Env.Dev, Env.Stage };
                    break;
                case Env.Stage:
                    AvaliableNextStatus = new ObservableCollection<Env>() { Env.Test, Env.Prod };
                    break;
                case Env.Prod:
                    AvaliableNextStatus = new ObservableCollection<Env>() { Env.Stage };
                    break;
            }
        }

        private void MoveProjectCommandExecute()
        {
            try
            {
                Utility.DoTaskWithBusyCaption("Moving", () =>
                  {
                      using (WorkflowsQueryServiceClient client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
                      {
                          StoreActivitiesDC activity = new StoreActivitiesDC();
                          activity.Name = this.ProjectName;
                          activity.Version = this.currentProject.Version;
                          activity.Environment = this.NextStatus.Value.ToString();

                          //check activity exist
                          List<StoreActivitiesDC> replyActivty = client.StoreActivitiesGet(activity);
                          StoreActivitiesDC dc = replyActivty[0];
                          if (dc.StatusReply.Errorcode == 0)
                              throw new UserFacingException(string.Format("The workflow {0} has existed in {1},can not be moved.", this.ProjectName, this.NextStatus.Value.ToString()));

                          ActivityMoveRequest request = new ActivityMoveRequest();
                          request.SetIncaller();
                          request.Environment = this.NextStatus.Value.ToString();
                          request.Name = this.ProjectName;
                          request.Version = this.currentProject.Version;
                          request.Environment = this.currentProject.Env.ToString();
                          request.EnvironmentTarget = this.NextStatus.Value.ToString();
                          request.WorkflowTypeId = this.WorkflowTemplate.Id;
                          this.currentProject.Env = this.NextStatus.Value;
                          this.currentProject.WorkflowType = this.WorkflowTemplate.Name;
                          ActivityMoveReply reply = client.ActivityMove(request);
                          if (reply != null)
                              reply.StatusReply.CheckErrors();
                      }
                  });
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowException(ex, "Failed to Move Project.");
            }

        }

        private void GetWorkflowTemplates(Env env)
        {
            AssetStore.AssetStoreProxy.GetWorkflowTypes(env);
            this.WorkflowTemplates = AssetStore.AssetStoreProxy.WorkflowTypes;
        }

    }
}
