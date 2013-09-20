using CWF.DataContracts;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Support.Workflow.Authoring.AddIns.Converters;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.Common.Messages;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Security;
using Microsoft.Support.Workflow.Authoring.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Data = Microsoft.Support.Workflow.Authoring.AddIns.Data;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    public class CopyCurrentProjectViewModel : ViewModelBase
    {
        private const string DefaultVersion = "1.0.0.0";
        private WorkflowItem currentProject;
        private ObservableCollection<Data.Env> avaliableCopyToEnvs;
        private ObservableCollection<WorkflowTypesGetBase> workflowTemplates;
        private WorkflowTypesGetBase workflowTemplate;
        private Env? copyTo;

        public DelegateCommand CopyProjectCommand { get; private set; }

        public ObservableCollection<WorkflowTypesGetBase> WorkflowTemplates
        {
            get { return this.workflowTemplates; }
            set
            {
                this.workflowTemplates = value;
                RaisePropertyChanged(() => WorkflowTemplates);
            }
        }

        public StoreActivitiesDC CopiedActivity
        {
            get;
            set;
        }

        public Env? CopyTo
        {
            get { return this.copyTo; }
            set
            {
                this.copyTo = value;
                RaisePropertyChanged(() => this.CopyTo);
                this.CopyProjectCommand.RaiseCanExecuteChanged();
                if (CopyTo != null)
                    this.GetWorkflowTemplates(CopyTo.Value);
            }
        }

        public ObservableCollection<Data.Env> AvaliableCopyToEnvs
        {
            get { return this.avaliableCopyToEnvs; }
            set
            {
                this.avaliableCopyToEnvs = value;
                RaisePropertyChanged(() => this.AvaliableCopyToEnvs);
            }
        }

        public Data.Env CopyFrom
        {
            get { return this.currentProject.Env; }
        }

        public string ProjectName
        {
            get { return this.currentProject.WorkflowName; }
        }

        public string CreatedBy
        {
            get { return this.currentProject.CreatedBy; }
        }

        public WorkflowTypesGetBase WorkflowTemplate
        {
            get { return this.workflowTemplate; }
            set
            {
                workflowTemplate = value;
                RaisePropertyChanged(() => WorkflowTemplate);
                this.CopyProjectCommand.RaiseCanExecuteChanged();
            }
        }

        public CopyCurrentProjectViewModel(WorkflowItem workflow)
        {
            this.currentProject = workflow;
            RaisePropertyChanged(() => ProjectName);
            RaisePropertyChanged(() => CreatedBy);
            RaisePropertyChanged(() => CopyFrom);
            this.CopyProjectCommand = new DelegateCommand(CopyProjectCommandExecute, CopyProjectCommandCanExecute);
            this.InitializeCopyTo();
        }

        private void InitializeCopyTo()
        {
            this.AvaliableCopyToEnvs = new ObservableCollection<Data.Env>(AuthorizationService.GetAuthorizedEnvs(Permission.CopyWorkflow)); ;
            if (this.AvaliableCopyToEnvs.Contains(this.CopyFrom))
                this.AvaliableCopyToEnvs.Remove(this.CopyFrom);
        }

        private void GetWorkflowTemplates(Env env)
        {
            AssetStore.AssetStoreProxy.GetWorkflowTypes(env);
            this.WorkflowTemplates = AssetStore.AssetStoreProxy.WorkflowTypes;
        }

        /// <summary>
        /// request qs to copyWorkflow and get copiedWorkflow 
        /// </summary>
        private void CopyProjectCommandExecute()
        {
            try
            {
                Utility.DoTaskWithBusyCaption("Copying", () =>
                    {
                        using (WorkflowsQueryServiceClient client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
                        {  
                            //copy project
                            ActivityCopyRequest request = new ActivityCopyRequest();
                            request.SetIncaller();
                            request.Name = this.currentProject.Name;
                            request.Version = this.currentProject.Version;
                            request.WorkflowTypeId = this.WorkflowTemplate.Id;
                            request.Environment = this.currentProject.Env.ToString();
                            request.EnvironmentTarget = CopyTo.Value.ToString();
                            StoreActivitiesDC reply = client.ActivityCopy(request);
                            if (reply != null)
                                reply.StatusReply.CheckErrors();
                            if (reply != null && reply.StatusReply.Errorcode == 0)
                                this.CopiedActivity = reply;

                        }
                    });
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowException(ex, "Failed to Copy Project.");
            }
        }

        private bool CopyProjectCommandCanExecute()
        {
            return this.CopyTo != null && this.WorkflowTemplate != null && this.IsValid;
        }
    }
}
