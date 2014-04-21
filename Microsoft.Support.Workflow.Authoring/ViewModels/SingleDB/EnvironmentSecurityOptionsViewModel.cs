using Microsoft.Practices.Prism.Commands;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using Microsoft.Support.Workflow.Authoring.Security;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Support.Workflow.Authoring.Views;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    public class EnvironmentSecurityOptionsViewModel : ViewModelBase
    {
        private static readonly string TestButtonContent = TextResources.Test;
        private static readonly string VerifyButtonContent = TextResources.Verifying;
        private static readonly string SecurityGroupNotValidMsg = TextResources.SecurityGroupInvalidMsgFormat;
        private static readonly string ConnectionSGSuccessfulMsg = TextResources.SGConnectionSucceedMsg;
        private static readonly string SaveSuccessfulMsg = TextResources.EnvironmentSavedMsg;
        private static readonly string SaveFailedMsg = TextResources.EnvironmentSaveFailureMsg;
        private static readonly string SGGroupsExist = TextResources.SGGroupExistsMsgFormat;
        private string tenantEndpoint;
        private string tenantName;
        private string tenantAdminGroup = string.Empty;
        private string sGname = string.Empty;
        private List<AuthorizationGroupDC> authorizationGroupDCs;
        private ObservableCollection<AuthorizationGroupDC> viewerGroups;
        private ObservableCollection<AuthorizationGroupDC> authorGroups;
        private ObservableCollection<AuthorizationGroupDC> adminGroups;
        private ObservableCollection<AuthorizationGroupDC> tenantStageAuthor;

        private List<string> sGType;
        private string selectedSGType;
        private bool isTesting;
        private string testButtonTitle;
        private bool hasChanged;
        private string cwfAdminGroup = string.Empty;
        public DelegateCommand TestCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand AddSGGroupCommand { get; private set; }
        public DelegateCommand<AuthorizationGroupDC> RemoveSGGroupCommand { get; private set; }

        public bool HasSaved
        {
            get;
            private set;
        }

        public bool CanSave { get { return this.SaveCommand.CanExecute(); } }

        public bool HasChanged
        {
            get { return this.hasChanged; }
            set
            {
                hasChanged = value;
                RaisePropertyChanged(() => this.HasChanged);
                RaiseCommandCanExecute();
            }
        }

        public string TestButtonTitle
        {
            get { return this.testButtonTitle; }
            set
            {
                this.testButtonTitle = value;
                RaisePropertyChanged(() => this.TestButtonTitle);
            }
        }

        public bool IsTesting
        {
            get { return this.isTesting; }
            set
            {
                this.isTesting = value;
                RaisePropertyChanged(() => this.IsTesting);
                RaiseCommandCanExecute();
            }
        }

        public List<string> SGType
        {
            get { return this.sGType; }
            set
            {
                this.sGType = value;
                RaisePropertyChanged(() => SGType);
            }
        }

        public string SelectedSGType
        {
            get { return this.selectedSGType; }
            set
            {
                this.selectedSGType = value;
                RaisePropertyChanged(() => this.SelectedSGType);
                RaiseCommandCanExecute();
            }
        }

        public ObservableCollection<AuthorizationGroupDC> ViewerGroups
        {
            get { return this.viewerGroups; }
            set
            {
                this.viewerGroups = value;
                RaisePropertyChanged(() => this.ViewerGroups);
            }
        }

        public ObservableCollection<AuthorizationGroupDC> AuthorGroups
        {
            get { return this.authorGroups; }
            set
            {
                this.authorGroups = value;
                RaisePropertyChanged(() => AuthorGroups);
            }
        }

        public ObservableCollection<AuthorizationGroupDC> AdminGroups
        {
            get { return this.adminGroups; }
            set
            {
                this.adminGroups = value;
                RaisePropertyChanged(() => this.AdminGroups);
            }
        }

        public ObservableCollection<AuthorizationGroupDC> TenantStageAuthorGroups
        {
            get { return this.tenantStageAuthor; }
            set
            {
                this.tenantStageAuthor = value;
                RaisePropertyChanged(() => this.TenantStageAuthorGroups);
            }
        }

        public string TenantEndpoint
        {
            get { return this.tenantEndpoint; }
            set
            {
                this.tenantEndpoint = value;
                RaisePropertyChanged(() => this.TenantEndpoint);
            }
        }

        public string TenantName
        {
            get { return this.tenantName; }
            set
            {
                this.tenantName = value;
                RaisePropertyChanged(() => this.TenantName);
            }
        }

        public string TenantAdminGroup
        {
            get { return this.tenantAdminGroup; }
            set
            {
                this.tenantAdminGroup = value;
                RaisePropertyChanged(() => this.TenantAdminGroup);
                HasChanged = true;
                RaiseCommandCanExecute();
            }
        }

        public string SGName
        {
            get { return this.sGname; }
            set
            {
                this.sGname = value;
                RaisePropertyChanged(() => SGName);
                RaiseCommandCanExecute();
            }
        }

        public EnvironmentSecurityOptionsViewModel()
        {
            TestButtonTitle = TestButtonContent;
            this.TestCommand = new DelegateCommand(this.TestCommandExecute,
                () => { return !string.IsNullOrWhiteSpace(this.TenantAdminGroup) && !IsTesting; });

            this.SaveCommand = new DelegateCommand(this.SaveCommandExecute, () => !IsTesting && HasChanged);

            this.AddSGGroupCommand = new DelegateCommand(this.AddSGGroupCommandExecute,
                () => { return this.SelectedSGType != null && !string.IsNullOrWhiteSpace(this.SGName) && !IsTesting; });

            this.RemoveSGGroupCommand = new DelegateCommand<AuthorizationGroupDC>(RemoveSGGroupCommandExecute);

            this.TenantName = AssetStore.AssetStoreProxy.TenantName;

            this.TenantEndpoint = AssetStore.AssetStoreProxy.ClientEndPoint;

            this.SGType = new List<string> 
            {
                TextResources.Viewer,
                TextResources.Author,
                TextResources.Admin,
                TextResources.StageAuthor
            };
        }

        public void LoadLiveData()
        {
            this.GetAuthorizationGroupDCs();
        }

        private void RaiseCommandCanExecute()
        {
            this.RemoveSGGroupCommand.RaiseCanExecuteChanged();
            this.AddSGGroupCommand.RaiseCanExecuteChanged();
            this.SaveCommand.RaiseCanExecuteChanged();
            this.TestCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged(() => this.CanSave);
        }

        private void TestCommandExecute()
        {
            bool isExist = false;
            if (this.ValidateGroupExists(this.TenantAdminGroup, true))
                return;
            IsTesting = true;
            Task.Factory.StartNew(() =>
            {
                TestButtonTitle = VerifyButtonContent;
                if (AuthorizationService.GroupExists(this.TenantAdminGroup))
                    isExist = true;
                TestButtonTitle = TestButtonContent;
                if (isExist)
                    MessageBoxService.ShowInfo(ConnectionSGSuccessfulMsg);
                else
                    MessageBoxService.ShowError(string.Format(SecurityGroupNotValidMsg, this.TenantAdminGroup));
                IsTesting = false;
            });
        }

        private bool ValidateGroupExists(string groupName, bool isEnvAdminGroup)
        {
            bool isExist = false;
            string group = groupName.Trim();
            if (this.ViewerGroups.Select(a => a.AuthGroupName).ToList().Any(s => s.Equals(group, StringComparison.OrdinalIgnoreCase))
                || this.AdminGroups.Select(a => a.AuthGroupName).ToList().Any(s => s.Equals(group, StringComparison.OrdinalIgnoreCase))
                || this.AuthorGroups.Select(a => a.AuthGroupName).ToList().Any(s => s.Equals(group, StringComparison.OrdinalIgnoreCase))
                || this.TenantStageAuthorGroups.Select(a => a.AuthGroupName).ToList().Any(s => s.Equals(group, StringComparison.OrdinalIgnoreCase))
                || !string.IsNullOrWhiteSpace(this.cwfAdminGroup) && this.cwfAdminGroup.Equals(group, StringComparison.OrdinalIgnoreCase)
                || (!isEnvAdminGroup && !string.IsNullOrWhiteSpace(this.TenantAdminGroup) && this.TenantAdminGroup.Trim().Equals(group, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBoxService.ShowInfo(string.Format(SGGroupsExist, group));
                isExist = true;
            }
            return isExist;
        }

        private void SaveCommandExecute()
        {
            try
            {
                HasSaved = false;

                if (!string.IsNullOrWhiteSpace(TenantAdminGroup))
                {
                    if (this.ValidateGroupExists(this.TenantAdminGroup, true))
                        return;
                    if (!AuthorizationService.GroupExists(this.TenantAdminGroup))
                    {
                        MessageBoxService.ShowError(string.Format(SecurityGroupNotValidMsg, this.TenantAdminGroup));
                        return;
                    }
                }
                Utility.DoTaskWithBusyCaption(TextResources.Saving, () =>
                {
                    using (WorkflowsQueryServiceClient client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
                    {
                        AuthGroupsCreateOrUpdateReplyDC reply = null;

                        //Save Tenant Admin Group
                        AuthGroupsCreateOrUpdateRequestDC requestTenantAdmin = new AuthGroupsCreateOrUpdateRequestDC();
                        requestTenantAdmin.SetIncaller();
                        requestTenantAdmin.RoleId = (int)Role.CWFEnvAdmin;
                        requestTenantAdmin.InAuthGroups = new List<string>();
                        if (!string.IsNullOrWhiteSpace(this.TenantAdminGroup))
                            requestTenantAdmin.InAuthGroups.Add(this.TenantAdminGroup.Trim());
                        reply = client.AuthorizationGroupCreateOrUpdate(requestTenantAdmin);
                        reply.StatusReply.CheckErrors();

                        //Save Viewer Groups
                        AuthGroupsCreateOrUpdateRequestDC requestViewer = new AuthGroupsCreateOrUpdateRequestDC();
                        requestViewer.SetIncaller();
                        requestViewer.RoleId = (int)Role.Viewer;
                        requestViewer.InAuthGroups = this.ViewerGroups.Select(n => n.AuthGroupName).ToList();
                        reply = client.AuthorizationGroupCreateOrUpdate(requestViewer);
                        reply.StatusReply.CheckErrors();

                        //Save Admin Groups
                        AuthGroupsCreateOrUpdateRequestDC requestAdmin = new AuthGroupsCreateOrUpdateRequestDC();
                        requestAdmin.SetIncaller();
                        requestAdmin.RoleId = (int)Role.Admin;
                        requestAdmin.InAuthGroups = this.AdminGroups.Select(n => n.AuthGroupName).ToList();
                        reply = client.AuthorizationGroupCreateOrUpdate(requestAdmin);
                        reply.StatusReply.CheckErrors();

                        //Save Author Groups
                        AuthGroupsCreateOrUpdateRequestDC requestAuthor = new AuthGroupsCreateOrUpdateRequestDC();
                        requestAuthor.SetIncaller();
                        requestAuthor.RoleId = (int)Role.Author;
                        requestAuthor.InAuthGroups = this.AuthorGroups.Select(n => n.AuthGroupName).ToList();
                        reply = client.AuthorizationGroupCreateOrUpdate(requestAuthor);
                        reply.StatusReply.CheckErrors();

                        //save tenantstageauthor 
                        AuthGroupsCreateOrUpdateRequestDC requestStageAuthor = new AuthGroupsCreateOrUpdateRequestDC();
                        requestStageAuthor.SetIncaller();
                        requestStageAuthor.RoleId = (int)Role.TenantStageAuthor;
                        requestStageAuthor.InAuthGroups = this.TenantStageAuthorGroups.Select(n => n.AuthGroupName).ToList();
                        reply = client.AuthorizationGroupCreateOrUpdate(requestStageAuthor);
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

        private int GetRoleId(string role)
        {
            if (role == TextResources.Viewer)
                return (int)Role.Viewer;
            else if (role == TextResources.Admin)
                return (int)Role.Admin;
            else if (role == TextResources.Author)
                return (int)Role.Author;
            else if (role == TextResources.StageAuthor)
                return (int)Role.TenantStageAuthor;
            return 0;
        }

        private void AddSGGroupCommandExecute()
        {
            //Test Security Group Exists
            if (!AuthorizationService.GroupExists(this.SGName))
            {
                MessageBoxService.ShowError(string.Format(SecurityGroupNotValidMsg, this.SGName));
                return;
            }
            if (ValidateGroupExists(this.SGName, false))
                return;

            AuthorizationGroupDC authGroup = new AuthorizationGroupDC();
            authGroup.AuthGroupName = SGName.Trim();
            authGroup.RoleId = this.GetRoleId(SelectedSGType);
            switch (authGroup.RoleId)
            {
                case (int)Role.Viewer:
                    if (this.ViewerGroups == null)
                        this.ViewerGroups = new ObservableCollection<AuthorizationGroupDC>();
                    this.ViewerGroups.Add(authGroup);
                    break;
                case (int)Role.Admin:
                    if (this.AdminGroups == null)
                        this.AdminGroups = new ObservableCollection<AuthorizationGroupDC>();
                    this.AdminGroups.Add(authGroup);
                    break;
                case (int)Role.Author:
                    if (this.AuthorGroups == null)
                        this.AuthorGroups = new ObservableCollection<AuthorizationGroupDC>();
                    this.AuthorGroups.Add(authGroup);
                    break;
                case (int)Role.TenantStageAuthor:
                    if (this.TenantStageAuthorGroups == null)
                        this.TenantStageAuthorGroups = new ObservableCollection<AuthorizationGroupDC>();
                    this.TenantStageAuthorGroups.Add(authGroup);
                    break;
                default:
                    break;
            }
            HasChanged = true;
        }

        private void RemoveSGGroupCommandExecute(AuthorizationGroupDC auth)
        {
            if (auth == null)
                return;
            if (Enum.IsDefined(typeof(Role), auth.RoleId))
            {
                Role selectedRole = (Role)auth.RoleId;
                switch (selectedRole)
                {
                    case Role.Viewer:
                        this.ViewerGroups.Remove(auth);
                        break;
                    case Role.Admin:
                        this.AdminGroups.Remove(auth);
                        break;
                    case Role.Author:
                        this.AuthorGroups.Remove(auth);
                        break;
                    case Role.TenantStageAuthor:
                        this.TenantStageAuthorGroups.Remove(auth);
                        break;
                    default:
                        break;
                }
                HasChanged = true;
            }
        }



        private void GetAuthorizationGroupDCs()
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
                        this.AdminGroups = new ObservableCollection<AuthorizationGroupDC>(this.authorizationGroupDCs.Where(a => a.RoleId == (int)Role.Admin));
                        this.AuthorGroups = new ObservableCollection<AuthorizationGroupDC>(this.authorizationGroupDCs.Where(a => a.RoleId == (int)Role.Author));
                        this.ViewerGroups = new ObservableCollection<AuthorizationGroupDC>(this.authorizationGroupDCs.Where(a => a.RoleId == (int)Role.Viewer));
                        this.TenantStageAuthorGroups = new ObservableCollection<AuthorizationGroupDC>(this.authorizationGroupDCs.Where(a => a.RoleId == (int)Role.TenantStageAuthor));
                        this.tenantAdminGroup = this.authorizationGroupDCs.IfNotNull(g => g.FirstOrDefault(a => a.RoleId == (int)Role.CWFEnvAdmin)).IfNotNull(a => a.AuthGroupName);
                        this.cwfAdminGroup = this.authorizationGroupDCs.IfNotNull(g => g.FirstOrDefault(a => a.RoleId == (int)Role.CWFAdmin)).IfNotNull(a => a.AuthGroupName);
                    }
                }
            });
        }
    }
}
