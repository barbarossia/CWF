using CWF.DataContracts;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;


namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    public class TenantSecurityOptionsViewModel : ViewModelBase
    {
        private static readonly string SaveSuccessfulMsg = TextResources.TenantSettingsSavedMsg;
        private static readonly string SaveFailedMsg = TextResources.TenantSettingsSaveFailureMsg;

        private List<AuthorizationGroupDC> authorizationGroupDCs;
        private ObservableCollection<AuthorizationGroupDC> viewerGroupsEnabled;
        private ObservableCollection<AuthorizationGroupDC> authorGroupsEnabled;
        private ObservableCollection<AuthorizationGroupDC> adminGroupsEnabled;
        private ObservableCollection<AuthorizationGroupDC> stageAuthorGroupsEnabled;
        private ObservableCollection<AuthorizationGroupDC> viewerGroupsDisEnabled;
        private ObservableCollection<AuthorizationGroupDC> authorGroupsDisEnabled;
        private ObservableCollection<AuthorizationGroupDC> adminGroupsDisEnabled;
        private ObservableCollection<AuthorizationGroupDC> stageAuthorGroupsDisEnabled;
        private AuthorizationGroupDC selectedDisenabledViewerGroup;
        private AuthorizationGroupDC selectedDisenabledAuthorGroup;
        private AuthorizationGroupDC selectedDisenabledAdminGroup;
        private AuthorizationGroupDC selectedDisenableStageAuthorGroup;

        private bool hasChanged;

        public DelegateCommand<string> AddSGGroupCommand { get; private set; }
        public DelegateCommand<AuthorizationGroupDC> RemoveSGGroupCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }

        public bool CanSave { get { return this.SaveCommand.CanExecute(); } }
        public bool HasSaved
        {
            get;
            private set;
        }
        public bool HasChanged
        {
            get { return this.hasChanged; }
            set
            {
                hasChanged = value;
                RaisePropertyChanged(() => this.HasChanged);
                RaiseCommandExecute();
            }
        }

        public AuthorizationGroupDC SelectedDisenableViewerGroup
        {
            get { return this.selectedDisenabledViewerGroup; }
            set
            {
                this.selectedDisenabledViewerGroup = value;
                RaisePropertyChanged(() => this.SelectedDisenableViewerGroup);
                RaiseCommandExecute();
            }
        }

        public AuthorizationGroupDC SelectedDisenableAuthorGroup
        {
            get { return this.selectedDisenabledAuthorGroup; }
            set
            {
                this.selectedDisenabledAuthorGroup = value;
                RaisePropertyChanged(() => this.SelectedDisenableAuthorGroup);
                RaiseCommandExecute();
            }
        }

        public AuthorizationGroupDC SelectedDisenableAdminGroup
        {
            get { return this.selectedDisenabledAdminGroup; }
            set
            {
                this.selectedDisenabledAdminGroup = value;
                RaisePropertyChanged(() => this.SelectedDisenableAdminGroup);
                RaiseCommandExecute();
            }
        }

        public ObservableCollection<AuthorizationGroupDC> ViewerGroupsDisEnabled
        {
            get { return this.viewerGroupsDisEnabled; }
            set
            {
                this.viewerGroupsDisEnabled = value;
                RaisePropertyChanged(() => this.ViewerGroupsDisEnabled);
            }
        }

        public ObservableCollection<AuthorizationGroupDC> AuthorGroupsDisEnabled
        {
            get { return this.authorGroupsDisEnabled; }
            set
            {
                this.authorGroupsDisEnabled = value;
                RaisePropertyChanged(() => AuthorGroupsDisEnabled);
            }
        }

        public ObservableCollection<AuthorizationGroupDC> AdminGroupsDisEnabled
        {
            get { return this.adminGroupsDisEnabled; }
            set
            {
                this.adminGroupsDisEnabled = value;
                RaisePropertyChanged(() => this.AdminGroupsDisEnabled);
            }
        }

        public ObservableCollection<AuthorizationGroupDC> ViewerGroupsEnabled
        {
            get { return this.viewerGroupsEnabled; }
            set
            {
                this.viewerGroupsEnabled = value;
                RaisePropertyChanged(() => this.ViewerGroupsEnabled);
            }
        }

        public ObservableCollection<AuthorizationGroupDC> AuthorGroupsEnabled
        {
            get { return this.authorGroupsEnabled; }
            set
            {
                this.authorGroupsEnabled = value;
                RaisePropertyChanged(() => AuthorGroupsEnabled);
            }
        }

        public ObservableCollection<AuthorizationGroupDC> AdminGroupsEnabled
        {
            get { return this.adminGroupsEnabled; }
            set
            {
                this.adminGroupsEnabled = value;
                RaisePropertyChanged(() => this.AdminGroupsEnabled);
            }
        }

        //new security group
        public ObservableCollection<AuthorizationGroupDC> StageAuthorGroupsEnabled
        {
            get { return this.stageAuthorGroupsEnabled; }
            set
            {
                this.stageAuthorGroupsEnabled = value;
                RaisePropertyChanged(() => this.StageAuthorGroupsEnabled);
            }
        }


        public ObservableCollection<AuthorizationGroupDC> StageAuthorGroupsDisEnabled
        {
            get { return this.stageAuthorGroupsDisEnabled; }
            set
            {
                this.stageAuthorGroupsDisEnabled = value;
                RaisePropertyChanged(() => this.StageAuthorGroupsDisEnabled);
            }
        }

        public AuthorizationGroupDC SelectedDisenableStageAuthorGroup
        {
            get { return this.selectedDisenableStageAuthorGroup; }
            set
            {
                this.selectedDisenableStageAuthorGroup = value;
                RaisePropertyChanged(() => this.SelectedDisenableStageAuthorGroup);
                RaiseCommandExecute();
            }
        }

        public TenantSecurityOptionsViewModel()
        {
            this.SaveCommand = new DelegateCommand(this.SaveCommandExecute, () => this.HasChanged == true);
            this.AddSGGroupCommand = new DelegateCommand<string>(this.AddSGGroupCommandExecute, AddSGGroupCommandCanExecute);
            this.RemoveSGGroupCommand = new DelegateCommand<AuthorizationGroupDC>(this.RemoveSGGroupCommandExecute);
        }

        public void LoadLiveData()
        {
            this.GetAuthGroups();
        }

        private void GetAuthGroups()
        {
            Utility.DoTaskWithBusyCaption(TextResources.Loading, () =>
            {
                using (WorkflowsQueryServiceClient client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
                {
                    AuthorizationGroupGetReplyDC reply = null;
                    AuthorizationGroupGetRequestDC request = new AuthorizationGroupGetRequestDC();
                    request.SetIncaller();
                    reply = client.GetAuthorizationGroups(request);
                    reply.StatusReply.CheckErrors();
                    this.authorizationGroupDCs = reply.AuthorizationGroups;
                    if (this.authorizationGroupDCs.Count > 0)
                    {
                        this.AdminGroupsEnabled = new ObservableCollection<AuthorizationGroupDC>(this.authorizationGroupDCs.Where(a => a.RoleId == (int)Role.Admin && a.Enabled));
                        this.AuthorGroupsEnabled = new ObservableCollection<AuthorizationGroupDC>(this.authorizationGroupDCs.Where(a => a.RoleId == (int)Role.Author && a.Enabled));
                        this.ViewerGroupsEnabled = new ObservableCollection<AuthorizationGroupDC>(this.authorizationGroupDCs.Where(a => a.RoleId == (int)Role.Viewer && a.Enabled));
                        this.StageAuthorGroupsEnabled = new ObservableCollection<AuthorizationGroupDC>(this.authorizationGroupDCs.Where(a => a.RoleId == (int)Role.TenantStageAuthor && a.Enabled));
                        this.AdminGroupsDisEnabled = new ObservableCollection<AuthorizationGroupDC>(this.authorizationGroupDCs.Where(a => a.RoleId == (int)Role.Admin && !a.Enabled));
                        this.AuthorGroupsDisEnabled = new ObservableCollection<AuthorizationGroupDC>(this.authorizationGroupDCs.Where(a => a.RoleId == (int)Role.Author && !a.Enabled));
                        this.ViewerGroupsDisEnabled = new ObservableCollection<AuthorizationGroupDC>(this.authorizationGroupDCs.Where(a => a.RoleId == (int)Role.Viewer && !a.Enabled));
                        this.StageAuthorGroupsDisEnabled = new ObservableCollection<AuthorizationGroupDC>(this.authorizationGroupDCs.Where(a => a.RoleId == (int)Role.TenantStageAuthor && !a.Enabled));
                        this.SelectDefaultGroup();
                    }
                }
            });
        }

        private void SaveCommandExecute()
        {
            try
            {
                Utility.DoTaskWithBusyCaption(TextResources.Saving, () =>
                {
                    using (WorkflowsQueryServiceClient client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
                    {

                        AuthGroupsEnableOrDisableRequestDC request = new AuthGroupsEnableOrDisableRequestDC();
                        request.SetIncaller();
                        AuthGroupsEnableOrDisableReplyDC reply = null;
                        List<string> enabledAuthGroups = new List<string>();
                        enabledAuthGroups.AddRange(this.ViewerGroupsEnabled.Select(a => a.AuthGroupName));
                        enabledAuthGroups.AddRange(this.AuthorGroupsEnabled.Select(a => a.AuthGroupName));
                        enabledAuthGroups.AddRange(this.AdminGroupsEnabled.Select(a => a.AuthGroupName));
                        enabledAuthGroups.AddRange(this.StageAuthorGroupsEnabled.Select(a => a.AuthGroupName));

                        request.InAuthGroups = enabledAuthGroups;
                        request.InEnabled = true;
                        reply = client.AuthorizationGroupEnableOrDisable(request);
                        reply.StatusReply.CheckErrors();

                        List<string> disenabledAuthGroups = new List<string>();
                        disenabledAuthGroups.AddRange(this.ViewerGroupsDisEnabled.Select(a => a.AuthGroupName));
                        disenabledAuthGroups.AddRange(this.AuthorGroupsDisEnabled.Select(a => a.AuthGroupName));
                        disenabledAuthGroups.AddRange(this.AdminGroupsDisEnabled.Select(a => a.AuthGroupName));
                        disenabledAuthGroups.AddRange(this.StageAuthorGroupsDisEnabled.Select(a => a.AuthGroupName));
                        request.InAuthGroups = disenabledAuthGroups;
                        request.InEnabled = false;
                        reply = client.AuthorizationGroupEnableOrDisable(request);
                        reply.StatusReply.CheckErrors();
                        HasChanged = false;
                        MessageBoxService.ShowInfo(SaveSuccessfulMsg);
                        HasSaved = true;
                    }
                });

            }
            catch (Exception ex)
            {
                MessageBoxService.ShowException(ex, SaveFailedMsg);
            }
        }

        private void AddSGGroupCommandExecute(string parameter)
        {
            AuthorizationGroupDC temp = null;
            if (parameter == "Admin" && this.SelectedDisenableAdminGroup != null)
            {
                temp = SelectedDisenableAdminGroup;
                this.AdminGroupsDisEnabled.Remove(SelectedDisenableAdminGroup);
                temp.Enabled = true;
                this.AdminGroupsEnabled.Add(temp);
            }
            else if (parameter == "Author" && this.SelectedDisenableAuthorGroup != null)
            {
                temp = SelectedDisenableAuthorGroup;
                this.AuthorGroupsDisEnabled.Remove(SelectedDisenableAuthorGroup);
                temp.Enabled = true;
                this.AuthorGroupsEnabled.Add(temp);
            }
            else if (parameter == "Viewer" && this.SelectedDisenableViewerGroup != null)
            {
                temp = this.SelectedDisenableViewerGroup;
                this.ViewerGroupsDisEnabled.Remove(SelectedDisenableViewerGroup);
                temp.Enabled = true;
                this.ViewerGroupsEnabled.Add(temp);
            }
            else if (parameter == "StageAuthor" && this.SelectedDisenableStageAuthorGroup != null)
            {
                temp = this.SelectedDisenableStageAuthorGroup;
                this.StageAuthorGroupsDisEnabled.Remove(SelectedDisenableStageAuthorGroup);
                temp.Enabled = true;
                this.StageAuthorGroupsEnabled.Add(temp);
            }
            this.SelectDefaultGroup();
            HasChanged = true;
        }

        private void RaiseCommandExecute()
        {
            this.AddSGGroupCommand.RaiseCanExecuteChanged();
            this.RemoveSGGroupCommand.RaiseCanExecuteChanged();
            this.SaveCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged(() => this.CanSave);
        }

        private bool AddSGGroupCommandCanExecute(string parameter)
        {
            if (parameter == "Admin" && this.SelectedDisenableAdminGroup != null)
                return true;
            else if (parameter == "Author" && this.SelectedDisenableAuthorGroup != null)
                return true;
            else if (parameter == "Viewer" && this.SelectedDisenableViewerGroup != null)
                return true;
            else if (parameter == "StageAuthor" && this.SelectedDisenableStageAuthorGroup != null)
                return true;
            return false;
        }

        private void RemoveSGGroupCommandExecute(AuthorizationGroupDC parameter)
        {
            if (parameter == null)
                return;

            if (parameter.RoleId == (int)Role.Admin)
            {
                this.AdminGroupsEnabled.Remove(parameter);
                parameter.Enabled = false;
                this.AdminGroupsDisEnabled.Add(parameter);
            }
            else if (parameter.RoleId == (int)Role.Author)
            {
                this.AuthorGroupsEnabled.Remove(parameter);
                parameter.Enabled = false;
                this.AuthorGroupsDisEnabled.Add(parameter);
            }
            else if (parameter.RoleId == (int)Role.Viewer)
            {
                this.ViewerGroupsEnabled.Remove(parameter);
                parameter.Enabled = false;
                this.ViewerGroupsDisEnabled.Add(parameter);
            }
            else if (parameter.RoleId == (int)Role.TenantStageAuthor)
            {
                this.StageAuthorGroupsEnabled.Remove(parameter);
                parameter.Enabled = false;
                this.StageAuthorGroupsDisEnabled.Add(parameter);
            }
            this.SelectDefaultGroup();
            HasChanged = true;
        }

        private void SelectDefaultGroup()
        {
            this.SelectedDisenableAuthorGroup = this.AuthorGroupsDisEnabled.FirstOrDefault();
            this.SelectedDisenableAdminGroup = this.AdminGroupsDisEnabled.FirstOrDefault();
            this.SelectedDisenableViewerGroup = this.ViewerGroupsDisEnabled.FirstOrDefault();
            this.SelectedDisenableStageAuthorGroup = this.StageAuthorGroupsDisEnabled.FirstOrDefault();
        }
    }
}
