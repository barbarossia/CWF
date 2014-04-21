// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Windows;
    using Build.Execution;
    using Common.Messages;
    using CWF.DataContracts;
    using CWF.WorkflowQueryService.Versioning;
    using Microsoft.Support.Workflow.Authoring.AddIns;
    using Microsoft.Support.Workflow.Authoring.AddIns.Converters;
    using Microsoft.Support.Workflow.Authoring.AddIns.Data;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
    using Microsoft.Support.Workflow.Authoring.Behaviors;
    using Microsoft.Support.Workflow.Authoring.Common;
    using Models;
    using Practices.Prism.Commands;
    using Security;
    using Services;
    using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

    /// <summary>
    /// The main window view model.
    /// </summary>
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private static readonly string UploadToMarketplaceBusyMessage = TextResources.UploadingToMarketplaceMsgFormat;   // The message we will display while uploading.
        private static readonly string UploadAsPrivateBusyMessage = TextResources.SavingToServerMsgFormat;          // The message we will display while saving as private.
        private const string DefaultSaveFileLocalExtension = ".wf";                                           // When saving a file local, if no file extension is specified, this will be used.

        private bool isToolboxVisible = true;
        private bool isProjectExplorerVisible = true;
        private bool isPropertiesVisible = true;
        private bool isWorkflowInfoVisible = true;
        private bool isContentVisible = true;
        private bool isToolboxSelected = true;
        private bool isProjectExplorerSelected = false;
        private bool isPropertiesSelected = false;
        private bool isWorkflowInfoSelected = true;
        private bool isContentSelected = false;
        private bool isToolboxPinned = true;
        private bool isProjectExplorerPinned = true;
        private bool isPropertiesPinned = true;
        private bool isWorkflowInfoPinned = true;
        private bool isContentPinned = true;
        private bool[] radPaneSelectedStatus;
        private ActivityItem focusedActivityItem;   // The focused activity item.
        private WorkflowItem focusedWorkflowItem;   // The focused workflow item.
        private UserInfoPaneViewModel userInfo;     // Exposes info about the current logged user (name, friendly name of environment, user pic)
        private string title;                       // The title.
        private ObservableCollection<WorkflowItem> workflowItems;   // The workflow items.
        private string errorMessage;                // error message exposed to a consumer of this viewmodel
        private string errorMessageType;            // error message type exposed to a consumer of this viewmodel - intended use is as a caption, perhaps in a message box
        // Save to local delegate for testing
        public Func<WorkflowItem, bool, bool> OnSaveToLocal { get; set; }
        // Save to server delegate for testing
        public Func<WorkflowItem, bool> OnSaveToServer { get; set; }
        // unlock delegate for testing
        public Action<WorkflowItem, bool> OnStoreActivitesUnlockWithBusy { get; set; }
        public Action<WorkflowItem, bool> OnStoreActivitesUnlock { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            AllEnvs = Env.All;
            AnyEnv = Env.Any;
            Task.Factory.StartNew(() => UserInfo = new UserInfoPaneViewModel());

            Title = string.Format(TextResources.MainWindowCaptionFormat, Environment.UserName);
            PropertyChanged += MainWindowViewModelPropertyChanged;

            InitializeCommands();
            InitializeWorkspace();

            OnSaveToLocal = SaveToLocal;
            OnSaveToServer = SaveToServer;
            OnStoreActivitesUnlockWithBusy = StoreActivitesUnlockWithBusy;
            OnStoreActivitesUnlock = StoreActivitesUnlock;
        }

        public DelegateCommand ClosingCommand { get; private set; }

        public DelegateCommand OpenEnvSecurityOptionsCommand { get; private set; }

        public DelegateCommand OpenTenantSecurityOptionsCommand { get; private set; }

        public DelegateCommand MoveProjectCommand { get; private set; }

        public DelegateCommand CopyProjectCommand { get; private set; }

        public DelegateCommand DeleteProjectCommand { get; private set; }

        public DelegateCommand ChangeAuthorCommand { get; private set; }


        public DelegateCommand OpenTaskActivitiesCommand { get; private set; }

        public DelegateCommand CheckInTaskActivityCommand { get; private set; }

        /// <summary>
        /// Saves the currently focused workflow as a template.
        /// </summary>
        public DelegateCommand SaveAsTemplateCommand { get; private set; }

        /// <summary>
        /// Gets or sets CompileCommand. Execute this command to compile workflow.
        /// </summary>
        public DelegateCommand CompileCommand { get; private set; }

        /// <summary>
        /// The ICommand for the CloseCommand
        /// </summary>
        public DelegateCommand CloseCommand { get; private set; }

        /// <summary>
        /// Closes the currently focused workflow.
        /// </summary>
        public DelegateCommand CloseFocusedWorkflowCommand { get; private set; }

        /// <summary>
        /// Gets or sets ImportAssemblyCommand. Execute this command to import assembly and all its referenced assemblies.
        /// </summary>
        public DelegateCommand ImportAssemblyCommand { get; set; }

        /// <summary>
        /// Gets or sets ImportAssemblyCommand. Execute this command to import assembly and all its referenced assemblies.
        /// </summary>
        public DelegateCommand OpenMarketplaceCommand { get; set; }

        /// <summary>
        /// Gets or sets NewWorkflowCommand. All commands are initialized in InitializeCommands method.
        /// </summary>
        public DelegateCommand NewWorkflowCommand { get; private set; }

        /// <summary>
        /// Gets or sets OpenWorkflowCommand. Execute this command to open a saved WorkflowItem.
        /// </summary>
        public DelegateCommand<string> OpenWorkflowCommand { get; private set; }

        /// <summary>
        /// Gets or sets PublishCommand.
        /// </summary>
        public DelegateCommand PublishCommand { get; private set; }

        /// <summary>
        /// Gets or sets ManageWorkflowTypeCommand
        /// </summary>
        public DelegateCommand ManageWorkflowTypeCommand { get; private set; }

        /// <summary>
        /// Gets or sets PrintCommand.
        /// </summary>
        public DelegateCommand PrintCommand { get; private set; }

        /// <summary>
        /// Gets or sets PrintAllCommand.
        /// </summary>
        public DelegateCommand PrintAllCommand { get; private set; }

        /// <summary>
        /// Undoes the last edit or operation
        /// </summary>
        public DelegateCommand UndoCommand { get; private set; }

        /// <summary>
        /// Redoes the last edit or operation
        /// </summary>
        public DelegateCommand RedoCommand { get; private set; }

        public DelegateCommand CutCommand { get; private set; }

        public DelegateCommand CopyCommand { get; private set; }

        public DelegateCommand PasteCommand { get; private set; }

        /// <summary>
        /// Refreshes the currently viewable workflow in the design surface.
        /// </summary>
        public DelegateCommand RefreshCommand { get; private set; }

        /// <summary>
        /// Unlock focused workflow
        /// </summary>
        public DelegateCommand UnlockCommand { get; private set; }

        /// <summary>
        /// Checks in the currently focused workflow to the marketplace, as private.
        /// </summary>
        public DelegateCommand CheckInAsPrivateCommand { get; private set; }

        /// <summary>
        /// Gets or sets SaveFocusedWorkflowCommand. Execute this command to save focused WorkflowItem instance.
        /// </summary>
        public DelegateCommand<string> SaveFocusedWorkflowCommand { get; private set; }

        /// <summary>
        /// Gets or sets SelectAssemblyAndActivityCommand. Execute this command to select activity assemblies with its activity type.
        /// Selected activity types will be shown in toolbox.
        /// At the same time, user can set if a activity type is favorite. Favorite activity types will be shown in Favorite toolbox.
        /// </summary>
        public DelegateCommand SelectAssemblyAndActivityCommand { get; private set; }

        /// <summary>
        /// Gets or sets UploadAssemblyCommand. Execute this command to upload selected assemblies to database.
        /// </summary>
        public DelegateCommand UploadAssemblyCommand { get; private set; }

        public DelegateCommand ShowAboutViewCommand { get; private set; }

        public DelegateCommand ShowClickableMessageCommand { get; private set; }

        public DelegateCommand OpenOptionsCommand { get; private set; }

        public DelegateCommand OpenCDSPackagesManagerCommand { get; private set; }

        public DelegateCommand TogglePanelVisibilitiesCommand { get; private set; }

        public DelegateCommand FindCommand { get; private set; }

        public DelegateCommand ReplaceCommand { get; private set; }

        public Env AllEnvs { get; private set; }
        public Env AnyEnv { get; private set; }

        public bool TaskActivityOpened
        {
            get
            {
                if (this.FocusedWorkflowItem != null
                    && this.FocusedWorkflowItem.TaskActivityGuid.HasValue
                    && this.FocusedWorkflowItem.IsValid
                    && !this.FocusedWorkflowItem.IsReadOnly)
                    return true;
                else
                    return false;
            }
        }

        public bool IsAllPanelsHidden {
            get { return !(IsToolboxVisible || IsProjectExplorerVisible || IsPropertiesVisible || IsWorkflowInfoVisible || IsContentVisible); }
        }

        public bool IsToolboxVisible {
            get { return isToolboxVisible; }
            set {
                isToolboxVisible = value;
                RaisePropertyChanged(() => IsToolboxVisible);
                RaisePropertyChanged(() => IsAllPanelsHidden);
            }
        }

        public bool IsProjectExplorerVisible {
            get { return isProjectExplorerVisible; }
            set {
                isProjectExplorerVisible = value;
                RaisePropertyChanged(() => IsProjectExplorerVisible);
                RaisePropertyChanged(() => IsAllPanelsHidden);
            }
        }

        public bool IsPropertiesVisible {
            get { return isPropertiesVisible; }
            set {
                isPropertiesVisible = value;
                RaisePropertyChanged(() => IsPropertiesVisible);
                RaisePropertyChanged(() => IsAllPanelsHidden);
            }
        }

        public bool IsWorkflowInfoVisible {
            get { return isWorkflowInfoVisible; }
            set {
                isWorkflowInfoVisible = value;
                RaisePropertyChanged(() => IsWorkflowInfoVisible);
                RaisePropertyChanged(() => IsAllPanelsHidden);
            }
        }

        public bool IsContentVisible {
            get { return isContentVisible; }
            set {
                isContentVisible = value;
                RaisePropertyChanged(() => IsContentVisible);
                RaisePropertyChanged(() => IsAllPanelsHidden);
            }
        }

        public bool IsToolboxSelected {
            get { return isToolboxSelected; }
            set {
                isToolboxSelected = value;
                RaisePropertyChanged(() => IsToolboxSelected);
            }
        }

        public bool IsProjectExplorerSelected {
            get { return isProjectExplorerSelected; }
            set {
                isProjectExplorerSelected = value;
                RaisePropertyChanged(() => IsProjectExplorerSelected);
            }
        }

        public bool IsPropertiesSelected {
            get { return isPropertiesSelected; }
            set {
                isPropertiesSelected = value;
                RaisePropertyChanged(() => IsPropertiesSelected);
            }
        }

        public bool IsWorkflowInfoSelected {
            get { return isWorkflowInfoSelected; }
            set {
                isWorkflowInfoSelected = value;
                RaisePropertyChanged(() => IsWorkflowInfoSelected);
            }
        }

        public bool IsContentSelected {
            get { return isContentSelected; }
            set {
                isContentSelected = value;
                RaisePropertyChanged(() => IsContentSelected);
            }
        }

        public bool IsToolboxPinned {
            get { return isToolboxPinned; }
            set {
                isToolboxPinned = value;
                RaisePropertyChanged(() => IsToolboxPinned);
            }
        }

        public bool IsProjectExplorerPinned {
            get { return isProjectExplorerPinned; }
            set {
                isProjectExplorerPinned = value;
                RaisePropertyChanged(() => IsProjectExplorerPinned);
            }
        }

        public bool IsPropertiesPinned {
            get { return isPropertiesPinned; }
            set {
                isPropertiesPinned = value;
                RaisePropertyChanged(() => IsPropertiesPinned);
            }
        }

        public bool IsWorkflowInfoPinned {
            get { return isWorkflowInfoPinned; }
            set {
                isWorkflowInfoPinned = value;
                RaisePropertyChanged(() => IsWorkflowInfoPinned);
            }
        }

        public bool IsContentPinned {
            get { return isContentPinned; }
            set {
                isContentPinned = value;
                RaisePropertyChanged(() => IsContentPinned);
            }
        }

        /// <summary>
        /// Gets or sets FocusedActivityItem. This ActivityItem's information will be show in pane.
        /// </summary>
        public ActivityItem FocusedActivityItem
        {
            get { return focusedActivityItem; }

            set
            {
                focusedActivityItem = value;
                RaisePropertyChanged(() => FocusedActivityItem);
            }
        }

        /// <summary>
        /// Gets/sets information for the User Info panel/login information/etc..
        /// </summary>
        public UserInfoPaneViewModel UserInfo
        {
            get { return userInfo; }

            set
            {
                userInfo = value;
                RaisePropertyChanged(() => UserInfo);
            }
        }

        /// <summary>
        /// Gets or sets FocusedWorkflowItem. Focused WorkflowItem means user is editing it.
        /// </summary>
        public WorkflowItem FocusedWorkflowItem
        {
            get { return focusedWorkflowItem; }
            set
            {
                focusedWorkflowItem = value;
                if (focusedWorkflowItem != null)
                    Version = FocusedWorkflowItem.Version;
                RaisePropertyChanged(() => FocusedWorkflowItem);
                if (focusedWorkflowItem != null && FocusedWorkflowItem.WorkflowDesigner == null)
                    FocusedWorkflowItem.InitializeWorkflowDesigner();
                RaisePropertyChanged(() => this.TaskActivityOpened);
            }
        }

        /// <summary>
        /// Gets or sets Title of main window.
        /// </summary>
        public string Title
        {
            get { return title; }

            set
            {
                title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        string version = String.Empty;
        public string Version
        {
            get { return version; }
            set
            {
                version = value;
                RaisePropertyChanged(() => Version);
            }
        }

        VersionFault versionFault;
        public VersionFault VersionFault
        {
            get { return versionFault; }
            set
            {
                versionFault = value;
                RaisePropertyChanged(() => VersionFault);
            }
        }

        /// <summary>
        /// Gets or sets WorkflowItems. This collection represent all WorkflowItems shown in MainWindow
        /// </summary>
        public ObservableCollection<WorkflowItem> WorkflowItems
        {
            get { return workflowItems; }

            set
            {
                workflowItems = value;
                RaisePropertyChanged(() => WorkflowItems);
                workflowItems.CollectionChanged += WorkflowItems_CollectionChanged;
            }
        }

        /// <summary>
        /// Method called when we have to execute a Data Dirty check on all documents in the Authoring Tool
        /// </summary>
        public bool CheckShouldCancelExit()
        {
            // Assume we have dirty data, in otherwords we will set e.Cancel = true
            bool shouldCancelExit = WorkflowItems.Count != 0;

            // If all the Items are saved
            for (int i = WorkflowItems.Count - 1; i >= 0; i--)
            {
                var wf = WorkflowItems[i];
                if (wf.IsReadOnly)
                {
                    shouldCancelExit = false;
                    CloseWorkflowItem(i, wf);
                }
                else
                {
                    //open from server and make the changes
                    SavingResult? result = null;
                    bool isSaveToLocal = false;
                    if (wf.IsOpenFromServer && wf.IsDataDirty)
                    {
                        result = MessageBoxService.ShowClosingConfirmation(wf.Name);
                    }
                    else if (wf.IsOpenFromServer && !wf.IsDataDirty) //open from server and not make the changes
                    {
                        result = MessageBoxService.ShowKeepLockedConfirmation(wf.Name);
                    }
                    else if (!wf.IsOpenFromServer && wf.IsDataDirty)//open from local and make the changes
                    {
                        result = MessageBoxService.ShowLocalSavingConfirmation(wf.Name);
                        isSaveToLocal = true;
                    }
                    else//open from local and not make the changes
                    {
                        shouldCancelExit = false;
                        CloseWorkflowItem(i, wf);
                        continue;
                    }
                    if (result != null)
                    {
                        shouldCancelExit = false;
                        if (isSaveToLocal && result.Value.HasFlag(SavingResult.Save))
                        {
                            shouldCancelExit = !OnSaveToLocal(wf, true);
                        }
                        else if (!isSaveToLocal && result.Value.HasFlag(SavingResult.Save))
                        {
                            shouldCancelExit = !OnSaveToServer(wf);
                        }
                        if (result.Value.HasFlag(SavingResult.Unlock))
                        {
                            OnStoreActivitesUnlockWithBusy(wf, result.Value.HasFlag(SavingResult.Save));
                        }

                        if (shouldCancelExit)
                        {
                            return shouldCancelExit;
                        }
                        else
                        {
                            CloseWorkflowItem(i, wf);
                        }
                    }
                    else
                    {
                        shouldCancelExit = true;
                        return shouldCancelExit;
                    }
                }
            }

            return shouldCancelExit;
        }

        public void CloseWorkflowItem(int index, WorkflowItem itemToClose)
        {
            RemoveWorkflowItemAndSetFocusedWorkflowItem(itemToClose);
            WorkflowItems.RemoveAt(index);
        }

        private void CloseWorkflowItem(WorkflowItem itemToClose)
        {
            RemoveWorkflowItemAndSetFocusedWorkflowItem(itemToClose);
            WorkflowItems.Remove(itemToClose);
        }

        private void RemoveWorkflowItemAndSetFocusedWorkflowItem(WorkflowItem itemToClose)
        {
            itemToClose.Close();
            FocusedWorkflowItem = WorkflowItems.FirstOrDefault(w => w != itemToClose);
        }

        public new string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                RaisePropertyChanged(() => ErrorMessage);
            }
        }

        public string ErrorMessageType
        {
            get { return errorMessageType; }
            set
            {
                errorMessageType = value;
                RaisePropertyChanged(() => ErrorMessageType);
            }
        }

        /// <summary>
        /// The Compile cannot execute unless the currently selected document is clean!
        /// </summary>
        /// <returns>True if we can Compile according to the CanExecute business rules</returns>
        private bool CanCompileCommandExecute()
        {
            return null != FocusedWorkflowItem &&
                   !FocusedWorkflowItem.IsService &&
                   FocusedWorkflowItem.IsValid &&
                   (!FocusedWorkflowItem.IsOpenFromServer ||
                   AuthorizationService.Validate(FocusedWorkflowItem.Env, Permission.CompileWorkflow));
        }

        /// <summary>
        /// Called when the compile command execute.
        /// </summary>
        private void CompileCommandExecute()
        {
            try
            {
                Utility.DoTaskWithBusyCaption(TextResources.Compiling, () =>
                    {
                        // Increment the version of the WorkflowItem. Long-term this could come from the server.
                        // Note: this is the NEXT version of the workflow because that is the one that will show
                        // in the editor if they continue editing. The previous version number has now been 
                        // compiled into the assembly and is now frozen.
                        var thisVersion = new Version(FocusedWorkflowItem.Version);
                        var nextVersion = new Version(thisVersion.Major,
                                                      Math.Max(thisVersion.Minor, 0),
                                                      Math.Max(thisVersion.Build, 0) + 1,
                                                      0);
                        FocusedWorkflowItem.OldVersion = FocusedWorkflowItem.Version;
                        FocusedWorkflowItem.Version = nextVersion.ToString();
                        Version = FocusedWorkflowItem.Version;
                        CompileFocusedWorkflow();
                    }, false);
            }
            catch (Exception ex)
            {
                throw new UserFacingException(ex.Message);
            }
        }

        private void CompileFocusedWorkflow()
        {
            //Compile on a different thread because the UI thread is STAThread and objects created here
            //don't play nicely with BuildManager.Build's thread
            CompileProject compileProject = FocusedWorkflowItem.WorkflowDesigner.CompileProject;
            compileProject.ProjectName = FocusedWorkflowItem.Name;
            compileProject.ProjectVersion = FocusedWorkflowItem.Version;

            CompileResult result = Compiler.Compile(compileProject);

            if (result.BuildResultCode == BuildResultCode.Success)
            {
                if (Application.Current != null && Application.Current.Dispatcher != null)
                    Application.Current.Dispatcher.Invoke(new Action(() => CompileCommandExecutePostCompile(FocusedWorkflowItem, result)));
                else
                    CompileCommandExecutePostCompile(FocusedWorkflowItem, result);
            }
            else
            {
                MessageBoxService.Show(TextResources.CompileFailureMsg + Environment.NewLine + result.Exception.Message,
                                       Assembly.GetExecutingAssembly().GetName().Name,
                                       MessageBoxButton.OK,
                                       MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Logic for Compile button, after WorkflowCompiler has done its work. Separated out for testability.
        /// </summary>
        private void CompileCommandExecutePostCompile(WorkflowItem compiledWorkflow, CompileResult compileResult)
        {
            if (compiledWorkflow == null)
            {
                throw new ArgumentNullException("compiledWorkflow");
            }

            if (compileResult == null)
            {
                throw new ArgumentNullException("compileResult");
            }

            // Import compile result. We signal to the UX that there has been a change, and it should respond by running the 
            // import process
            try
            {
                Compiler.AddToCaching(compileResult.FileName);
                MessageBoxService.CompileSuccessed(compiledWorkflow.Name);
            }
            catch (Exception ex)
            {
                throw new UserFacingException(ex.Message);
            }
        }

        private string assemblyToImportFile = String.Empty;
        public string AssemblyToImportFile
        {
            get { return assemblyToImportFile; }
            set
            {
                assemblyToImportFile = value;
                RaisePropertyChanged(() => AssemblyToImportFile);
            }
        }

        public bool CanCloseFocusedWorkflowCommandExecute()
        {
            return FocusedWorkflowItem != null;
        }

        /// <summary>
        /// Close the workflowItem passed in from the designer.
        /// </summary>
        /// <param name="itemToClose">workflowItem To close</param>
        public void CloseWorkflowCommandExecute(WorkflowItem itemToClose)
        {
            // We'll allow it without prompt if it's saved either to server or locally
            SavingResult? result = null;
            bool isSaveToLocal = false;
            //open from server and make the changes
            if (itemToClose.IsReadOnly)
            {
                CloseWorkflowItem(itemToClose);
            }
            else
            {
                if (itemToClose.IsOpenFromServer && itemToClose.IsDataDirty)
                {
                    result = MessageBoxService.ShowClosingConfirmation(itemToClose.Name);
                }
                else if (itemToClose.IsOpenFromServer && !itemToClose.IsDataDirty) //open from server and not make the changes
                {
                    result = MessageBoxService.ShowKeepLockedConfirmation(itemToClose.Name);
                }
                else if (!itemToClose.IsOpenFromServer && itemToClose.IsDataDirty)//open from local and make the changes
                {
                    result = MessageBoxService.ShowLocalSavingConfirmation(itemToClose.Name);
                    isSaveToLocal = true;
                }
                else//open from local and not make the changes
                {
                    CloseWorkflowItem(itemToClose);
                }
                if (result != null)
                {
                    bool shouldCancel = false;
                    if (isSaveToLocal && result.Value.HasFlag(SavingResult.Save))
                    {
                        shouldCancel = !SaveToLocal(itemToClose, true);
                    }
                    else if (!isSaveToLocal && result.Value.HasFlag(SavingResult.Save))
                    {
                        shouldCancel = !SaveToServer(itemToClose);
                    }
                    if (result.Value.HasFlag(SavingResult.Unlock) && CheckActivitiesUnlock(itemToClose))
                    {
                        OnStoreActivitesUnlockWithBusy(itemToClose, result.Value.HasFlag(SavingResult.Save));
                    }

                    if (!shouldCancel)
                    {
                        CloseWorkflowItem(itemToClose);
                    }
                }
            }
        }

        public void CloseFocusedWorkflowCommandExecute()
        {
            var wf = FocusedWorkflowItem;
            // We'll allow it without prompt if it's saved either to server or locally
            CloseWorkflowCommandExecute(wf);
        }

        /// <summary>
        /// Called when the ImportAssemblyCommand execute.
        /// </summary>
        private void ImportAssemblyCommandExecute()
        {
            try
            {
                SelectImportAssemblyViewModel vm = new SelectImportAssemblyViewModel(this.FocusedWorkflowItem);
                DialogService.ShowDialog(vm);
            }
            catch (Exception ex)
            {
                throw new UserFacingException(ex.Message);
            }
        }

        /// <summary>
        /// Closes Application
        /// </summary>
        private void CloseCommandExecute()
        {
            if (!CheckShouldCancelExit())
            {
                Application.Current.Shutdown(0);
            }
        }

        /// <summary>
        /// Initialize delegate commands with Execute and CanExecute callback method
        /// All command execute callback method should named as format CommandNameExecute
        /// All command can execute callback method should named as format CommandNameCanExecute
        /// </summary>
        private void InitializeCommands()
        {
            ClosingCommand = new DelegateCommand(CloseCommandExecute, CheckShouldCancelExit);

            OpenOptionsCommand = new DelegateCommand(DefaultValueSettingCommandExecute);
            OpenCDSPackagesManagerCommand = new DelegateCommand(OpenCDSPackagesManagerCommandExecute);
            OpenEnvSecurityOptionsCommand = new DelegateCommand(OpenEnvSecurityOptionsCommandExecute, OpenEnvSecurityOptionsCommandCanExecute);
            OpenTenantSecurityOptionsCommand = new DelegateCommand(OpenTenantSecurityOptionsCommandExecute, OpenTenantSecurityOptionsCommandCanExecute);
            MoveProjectCommand = new DelegateCommand(MoveProjectCommandExecute, MoveProjectCommandCanExecute);
            CopyProjectCommand = new DelegateCommand(CopyProjectCommandExecute, CopyProjectCommandCanExecute);
            DeleteProjectCommand = new DelegateCommand(DeleteProjectCommandExecute, DeleteProjectCommandCanExecute);
            ChangeAuthorCommand = new DelegateCommand(ChangeAuthorCommandExecute, ChangeAuthorCommandCanExecute);

            CheckInTaskActivityCommand = new DelegateCommand(TaskActivityCheckInCommandExecute, CanCheckInTaskActivityCommandExecute);
            OpenTaskActivitiesCommand = new DelegateCommand(OpenTaskActivitiesCommandExecute, CanOpenTaskActivitiesCommandExecute);
            NewWorkflowCommand = new DelegateCommand(NewWorkflowCommandExecute, CanNewWorkflowCommandExecute);
            SaveFocusedWorkflowCommand = new DelegateCommand<string>(SaveFocusedWorkflowCommandExecute, CanSaveFocusedWorkflow);
            OpenWorkflowCommand = new DelegateCommand<string>(OpenWorkflowCommandExecute, CanOpenWorkflowCommandExecute);
            CompileCommand = new DelegateCommand(CompileCommandExecute, CanCompileCommandExecute);
            PublishCommand = new DelegateCommand(PublishCommandExecute, CanPublishCommandExecute);
            CloseFocusedWorkflowCommand = new DelegateCommand(CloseFocusedWorkflowCommandExecute, CanCloseFocusedWorkflowCommandExecute);
            ImportAssemblyCommand = new DelegateCommand(ImportAssemblyCommandExecute, CanImportAssemblyCommandExecute);
            UploadAssemblyCommand = new DelegateCommand(UploadAssemblyCommandExecute, CanUploadAssemblyCommandExecute);
            SelectAssemblyAndActivityCommand = new DelegateCommand(SelectAssemblyAndActivityCommandExecute, CanSelectActivityCommandExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute);

            OpenMarketplaceCommand = new DelegateCommand(OpenMarketplaceCommandExecute, CanOpenMarketplaceCommandExecute);
            ShowAboutViewCommand = new DelegateCommand(ShowAboutViewCommandExecute);
            ShowClickableMessageCommand = new DelegateCommand(ShowClickableMessageExecute);
            PrintCommand = new DelegateCommand(PrintCommandExecute, PrintCommandCanExecute);
            PrintAllCommand = new DelegateCommand(PrintAllCommandExecute, PrintCommandCanExecute);
            UndoCommand = new DelegateCommand(UndoCommandExecute, UndoCommandCanExecute);
            RedoCommand = new DelegateCommand(RedoCommandExecute, RedoCommandCanExecute);
            CutCommand = new DelegateCommand(CutCommandExecute, CutCommandCanExecute);
            CopyCommand = new DelegateCommand(CopyCommandExecute, CopyCommandCanExecute);
            PasteCommand = new DelegateCommand(PasteCommandExecute, PasteCommandCanExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute, RefreshCommandCanExecute);
            UnlockCommand = new DelegateCommand(UnlockCommandExecute, UnlockCommandCanExecute);
            ManageWorkflowTypeCommand = new DelegateCommand(ManageWokflowTypesCommandExecute, CanManageWorflowTypeCommandExecute);
            TogglePanelVisibilitiesCommand = new DelegateCommand(TogglePanelVisibilitiesCommandExecute);
            FindCommand = new DelegateCommand(FindCommandExecute, CanFindCommandExecute);
            ReplaceCommand = new DelegateCommand(ReplaceCommandExecute, CanReplaceCommandExecute);
        }

        private void TogglePanelVisibilitiesCommandExecute() {
            // If not all panels are hidden, backup their status
            if (!IsAllPanelsHidden)
                radPaneSelectedStatus = new bool[] {
                    IsToolboxSelected, IsProjectExplorerSelected, IsPropertiesSelected, IsWorkflowInfoSelected, IsContentSelected 
                };
            
            IsToolboxVisible = IsProjectExplorerVisible = IsPropertiesVisible = IsWorkflowInfoVisible = IsContentVisible = 
                IsAllPanelsHidden;

            // If all panel are visible, restore their status
            if (!IsAllPanelsHidden) {
                IsToolboxSelected = radPaneSelectedStatus[0];
                IsProjectExplorerSelected = radPaneSelectedStatus[1];
                IsPropertiesSelected = radPaneSelectedStatus[2];
                IsWorkflowInfoSelected = radPaneSelectedStatus[3];
                IsContentSelected = radPaneSelectedStatus[4];

                if (!IsToolboxSelected && !IsProjectExplorerSelected) {
                    if (IsToolboxPinned)
                        IsToolboxSelected = true;
                    else if (IsProjectExplorerPinned)
                        IsProjectExplorerSelected = true;
                }
                if (!IsPropertiesSelected && !IsWorkflowInfoSelected && !IsContentSelected) {
                    if (IsPropertiesPinned)
                        IsPropertiesSelected = true;
                    else if (IsWorkflowInfoPinned)
                        IsWorkflowInfoSelected = true;
                    else if (IsContentPinned)
                        IsContentSelected = true;
                }
            }
        }

        private void FindCommandExecute() {
            FocusedWorkflowItem.WorkflowDesigner.ShowSearchBar(false);
        }

        private bool CanFindCommandExecute() {
            return FocusedWorkflowItem != null;
        }

        private void ReplaceCommandExecute() {
            FocusedWorkflowItem.WorkflowDesigner.ShowSearchBar(true);
        }

        private bool CanReplaceCommandExecute() {
            return FocusedWorkflowItem != null && !FocusedWorkflowItem.IsReadOnly;
        }

        private void OpenCDSPackagesManagerCommandExecute() 
        {
            var viewModel = new CDSPackagesManagerViewModel();
            DialogService.ShowDialog(viewModel);
        }

        private void DefaultValueSettingCommandExecute()
        {
            var viewModel = new OptionsViewModel();
            DialogService.ShowDialog(viewModel);
        }

        private void OpenTenantSecurityOptionsCommandExecute()
        {
            var viewModel = new TenantSecurityOptionsViewModel();
            viewModel.LoadLiveData();
            DialogService.ShowDialog(viewModel);
        }

        private void OpenEnvSecurityOptionsCommandExecute()
        {
            var viewModel = new EnvironmentSecurityOptionsViewModel();
            viewModel.LoadLiveData();
            DialogService.ShowDialog(viewModel);

        }

        private bool OpenEnvSecurityOptionsCommandCanExecute()
        {
            return AuthorizationService.Validate(Env.All, Permission.ManageEnvAdmin);
        }

        private bool OpenTenantSecurityOptionsCommandCanExecute()
        {
            return AuthorizationService.Validate(Env.All, Permission.ManageRoles);
        }

        private void MoveProjectCommandExecute()
        {
            if (!this.FocusedWorkflowItem.IsOpenFromServer || (this.FocusedWorkflowItem.IsOpenFromServer && this.FocusedWorkflowItem.IsDataDirty))
            {
                MessageBoxService.ShouldSaveWorkflow();
                return;
            }
            var viewModel = new MoveProjectViewModel(this.FocusedWorkflowItem);
            if (DialogService.ShowDialog(viewModel).GetValueOrDefault())
            {
                if (!AuthorizationService.Validate(this.FocusedWorkflowItem.Env, Permission.SaveWorkflow))
                    this.FocusedWorkflowItem.IsReadOnly = true;
                this.UpdateCommandsCanExecute();
            }
        }

        private bool MoveProjectCommandCanExecute()
        {
            return this.FocusedWorkflowItem != null && FocusedWorkflowItem.IsOpenFromServer && !FocusedWorkflowItem.TaskActivityGuid.HasValue
                && (this.FocusedWorkflowItem.WorkflowDesigner != null && !this.FocusedWorkflowItem.WorkflowDesigner.HasTask)
                && AuthorizationService.Validate(this.FocusedWorkflowItem.Env, Permission.MoveWorkflow);
        }

        private void CopyProjectCommandExecute()
        {
            if (!this.FocusedWorkflowItem.IsOpenFromServer || (this.FocusedWorkflowItem.IsOpenFromServer && this.FocusedWorkflowItem.IsDataDirty))
            {
                MessageBoxService.ShouldSaveWorkflow();
                return;
            }
            var viewModel = new CopyCurrentProjectViewModel(this.FocusedWorkflowItem);
            if (DialogService.ShowDialog(viewModel).GetValueOrDefault() && viewModel.CopiedActivity != null)
            {
                this.OpenActivityFromServer(viewModel.CopiedActivity, true,!DefaultValueSettings.EnableTaskAssignment);
            }
        }

        private bool CopyProjectCommandCanExecute()
        {
            return this.FocusedWorkflowItem != null && FocusedWorkflowItem.IsOpenFromServer && !FocusedWorkflowItem.TaskActivityGuid.HasValue
                && (this.FocusedWorkflowItem.WorkflowDesigner != null && !this.FocusedWorkflowItem.WorkflowDesigner.HasTask)
                && AuthorizationService.Validate(this.FocusedWorkflowItem.Env, Permission.CopyWorkflow);
        }

        private void DeleteProjectCommandExecute()
        {
            if (!this.FocusedWorkflowItem.IsOpenFromServer || (this.FocusedWorkflowItem.IsOpenFromServer && this.FocusedWorkflowItem.IsDataDirty))
            {
                MessageBoxService.ShouldSaveWorkflow();
                return;
            }
            if (MessageBoxService.ShouldDeleteWorkflow())
            {
                try
                {
                    //request delete
                    Utility.DoTaskWithBusyCaption(TextResources.Deleting, () =>
                    {
                        using (var client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
                        {
                            StoreActivitiesDC activity = DataContractTranslator.ActivityItemToStoreActivitiyDC(this.FocusedWorkflowItem);
                            StoreActivitiesDC reply = client.ActivityDelete(activity);
                            if (reply != null)
                                reply.StatusReply.CheckErrors();
                        }
                    });
                    //remove from current workflow list
                    this.CloseWorkflowItem(this.FocusedWorkflowItem);
                }
                catch (Exception ex)
                {
                    MessageBoxService.ShowException(ex, TextResources.DeleteWorkflowFailureMsg);
                }
            }
        }

        private bool DeleteProjectCommandCanExecute()
        {
            return this.FocusedWorkflowItem != null && FocusedWorkflowItem.IsOpenFromServer && !FocusedWorkflowItem.TaskActivityGuid.HasValue
                && (this.FocusedWorkflowItem.WorkflowDesigner != null && !this.FocusedWorkflowItem.WorkflowDesigner.HasTask)
                && AuthorizationService.Validate(this.FocusedWorkflowItem.Env, Permission.DeleteWorkflow);
        }

        private void ChangeAuthorCommandExecute()
        {
            if (!this.FocusedWorkflowItem.IsOpenFromServer || (this.FocusedWorkflowItem.IsOpenFromServer && this.FocusedWorkflowItem.IsDataDirty))
            {
                MessageBoxService.ShouldSaveWorkflow();
                return;
            }
            var viewModel = new ChangeAuthorViewModel(this.FocusedWorkflowItem);
            DialogService.ShowDialog(viewModel);
        }

        private bool ChangeAuthorCommandCanExecute()
        {
            return this.FocusedWorkflowItem != null && FocusedWorkflowItem.IsOpenFromServer && !FocusedWorkflowItem.TaskActivityGuid.HasValue
                && AuthorizationService.Validate(this.FocusedWorkflowItem.Env, Permission.ChangeWorkflowAuthor);
        }

        private bool CanNewWorkflowCommandExecute()
        {
            return AuthorizationService.Validate(Env.Any, Permission.SaveWorkflow);
        }

        private bool CanOpenWorkflowCommandExecute(string parameter)
        {
            if (parameter == "FromServer")
                return AuthorizationService.Validate(Env.Any, Permission.OpenWorkflow);
            else
                return true;
        }

        private void TaskActivityCheckInCommandExecute()
        {
            if (this.FocusedWorkflowItem != null && this.FocusedWorkflowItem.TaskActivityGuid.HasValue)
            {
                this.FocusedWorkflowItem.TaskActivityStatus = TaskActivityStatus.CheckedIn;
                this.SaveToServer(this.FocusedWorkflowItem);
                this.FocusedWorkflowItem.IsReadOnly = true;
            }
        }

        private void OpenTaskActivitiesCommandExecute()
        {
            var viewModel = new SearchTaskActivityViewModel();
            viewModel.LoadData();
            var dialogResult = DialogService.ShowDialog(viewModel);
            if (dialogResult.GetValueOrDefault())
            {
                var result = viewModel.SelectedActivity;
                if (result != null && result.Activity != null)
                {
                    //Set TaskActivity Status = Editing
                    try
                    {
                        this.OpenActivityFromServer(result.Activity, true, true);
                        if (this.FocusedWorkflowItem.Name == result.Activity.Name && this.FocusedWorkflowItem.Version == result.Activity.Version)
                        {
                            this.FocusedWorkflowItem.TaskActivityGuid = result.Guid;
                            if (!this.FocusedWorkflowItem.IsReadOnly)
                            {
                                this.FocusedWorkflowItem.TaskActivityStatus = TaskActivityStatus.Editing;
                                Utility.DoTaskWithBusyCaption(TextResources.UiMayNotRespondMsg,
                                () =>
                                {
                                    using (var client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
                                    {
                                        TaskActivityDC request = new TaskActivityDC()
                                        {
                                            Incaller = Utility.GetCallerName(),
                                            IncallerVersion = Utility.GetCallerVersion(),
                                            Id = viewModel.SelectedActivity.Id,
                                            Status = TaskActivityStatus.Editing,
                                            Environment = result.Activity.Environment
                                        };
                                        TaskActivityDC reply = client.TaskActivityUpdateStatus(request);
                                        if (reply.StatusReply.Errorcode != 0)
                                        {
                                            throw new UserFacingException(reply.StatusReply.ErrorMessage);
                                        }
                                    }
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxService.ShowError(ex.Message);
                    }
                }
            }
        }

        private void OpenTask(StoreActivitiesDC activity)
        {
            this.OpenActivityFromServer(activity, true, isTask: true);
        }

        /// <summary>
        /// Look for Property changes in this View Model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindowViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                // When the Focused workflow changes we re-evaluate all CanExecutes
                case "FocusedWorkflowItem":
                    // Update the Can Executes
                    UpdateCommandsCanExecute();
                    break;
            }
        }

        /// <summary>
        /// The initialize workspace, no new document by default since we create from templates
        /// </summary>
        private void InitializeWorkspace()
        {
            WorkflowItems = new ObservableCollection<WorkflowItem>();
        }

        /// <summary>
        /// Manage workflowtypes execute
        /// </summary>
        private void ManageWokflowTypesCommandExecute()
        {
            ManageWorkflowTypeViewModel vm = new ManageWorkflowTypeViewModel();
            vm.LoadData();
            var dialogResult = DialogService.ShowDialog(vm);
        }

        /// <summary>
        /// Open Marketplace Home window
        /// </summary>
        private void OpenMarketplaceCommandExecute()
        {
            var viewModel = new Microsoft.Support.Workflow.Authoring.ViewModels.Marketplace.MarketplaceViewModel(this.FocusedWorkflowItem);
            DialogService.ShowDialog(viewModel);
        }

        private bool CanImportAssemblyCommandExecute()
        {
            return this.FocusedWorkflowItem != null;
        }

        private bool CanUploadAssemblyCommandExecute()
        {
            return AuthorizationService.Validate(Env.Any, Permission.UploadAssemblyToMarketplace);
        }

        private bool CanSelectActivityCommandExecute()
        {
            return AuthorizationService.Validate(Env.Any, Permission.ViewMarketplace);
        }

        private bool CanOpenMarketplaceCommandExecute()
        {
            return AuthorizationService.Validate(Env.Any, Permission.ViewMarketplace);
        }

        private bool CanManageWorflowTypeCommandExecute()
        {
            return AuthorizationService.Validate(Env.Any, Permission.ManageWorkflowType);
        }

        private bool CanOpenTaskActivitiesCommandExecute()
        {
            return AuthorizationService.Validate(Env.Any, Permission.OpenWorkflow);
        }

        private bool CanCheckInTaskActivityCommandExecute()
        {
            return AuthorizationService.Validate(Env.Any, Permission.SaveWorkflow);
        }

        /// <summary>
        /// Called when create new workflow
        /// </summary>
        private void NewWorkflowCommandExecute()
        {
            BusyCaption = TextResources.ContactingServer;
            IsBusy = true;

            var viewModel = new NewWorkflowViewModel();
            IsBusy = false;
            var dialogResult = DialogService.ShowDialog(viewModel);

            if (dialogResult.GetValueOrDefault())
            {
                if (viewModel.CreatedItem != null)
                {
                    // Add the Item to our workflow items list
                    WorkflowItems.Add(viewModel.CreatedItem);
                    Utility.WithContactServerUI(() =>
                    {
                        viewModel.CreatedItem.IsTask = !DefaultValueSettings.EnableTaskAssignment;
                        FocusedWorkflowItem = viewModel.CreatedItem;
                    }, false);
                }
            }
        }

        /// <summary>
        /// Called when the open workflow command execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        private void OpenWorkflowCommandExecute(string parameter)
        {
            switch (parameter)
            {
                case "FromLocal":
                    string workflowFileName = DialogService.ShowOpenFileDialogAndReturnResult(TextResources.WorkflowFileFilter, TextResources.OpenWorkflowFile);
                    if (!string.IsNullOrEmpty(workflowFileName))
                    {
                        OpenWorkflowFromLocal(workflowFileName);
                    }
                    break;
                case "FromServer":
                    OpenWorkflowFromServer();
                    break;
            }
        }

        /// <summary>
        /// Open workflow from local file (serialized WorkflwoItem object).
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private void OpenWorkflowFromLocal(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if (String.IsNullOrEmpty(Path.GetExtension(fileName)))
            {
                fileName += ".wf";
            }

            // Recover WorkflowItem from XAML or XML file
            try
            {
                Utility.DoTaskWithBusyCaption(TextResources.Loading, () =>
                {
                    var recoverdWorkflow = (WorkflowItem)Utility.DeserializeSavedContent(fileName);
                    recoverdWorkflow.Env = Env.Dev;

                    // Add it or focus it!
                    var itemToFind = WorkflowItems.FirstOrDefault(wfi => 0 == wfi.CompareTo(recoverdWorkflow));
                    if (null == itemToFind)
                    {
                        recoverdWorkflow.IsReadOnly = false;
                        recoverdWorkflow.IsDataDirty = false;
                        recoverdWorkflow.IsOpenFromServer = false;
                    }
                    recoverdWorkflow.IsTask = !DefaultValueSettings.EnableTaskAssignment;
                    CheckIsAlreadyInListOrAdd(recoverdWorkflow);
                }, false);
            }
            catch (SerializationException)
            {
                throw new UserFacingException(String.Format(TextResources.InvalidWorkflowMsgFormat, fileName));
            }
        }

        private void UnlockCommandExecute()
        {
            bool needToSave = false;
            if (!FocusedWorkflowItem.IsSavedToServer)
            {
                SavingResult? result = MessageBoxService.ShowUnlockConfirmation(FocusedWorkflowItem.Name);
                if (result == null)
                {
                    return;
                }
                else if (result.Value.HasFlag(SavingResult.Save))
                {
                    needToSave = true;
                }
            }

            Utility.WithContactServerUI(() =>
            {
                if (needToSave)
                {
                    WorkflowsQueryServiceUtility.UsingClient(client => UploadWorkflow(client, FocusedWorkflowItem));
                }

                StoreActivitiesDC focused = new StoreActivitiesDC()
                {
                    Incaller = Assembly.GetExecutingAssembly().GetName().Name,
                    IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    Name = FocusedWorkflowItem.Name,
                    Version = FocusedWorkflowItem.OldVersion,
                    Environment = FocusedWorkflowItem.Env.ToString(),

                };
                var result = WorkflowsQueryServiceUtility.UsingClientReturn(client => client.StoreActivitiesGet(focused));
                if (result.Any())
                {
                    StoreActivitiesDC wf = result[0];
                    wf.StatusReply.CheckErrors();
                    if (wf.Locked)
                    {
                        if (wf.LockedBy == Environment.UserName)
                        {
                            OnStoreActivitesUnlock(FocusedWorkflowItem, needToSave);
                        }
                        else
                        {
                            bool canOverrideLock = AuthorizationService.Validate(wf.Environment.ToEnv(), Permission.OverrideLock);
                            if (!canOverrideLock)
                            {
                                MessageBoxService.LockChangedWhenAuthorUnlocking(wf.LockedBy);
                            }
                            else if (MessageBoxService.LockChangedWhenAdminUnlocking(wf.LockedBy) == MessageBoxResult.Yes)
                            {
                                OnStoreActivitesUnlock(FocusedWorkflowItem, needToSave);
                            }

                        }
                    }
                }
            });

            if (needToSave)
            {
                FocusedWorkflowItem.IsSavedToServer = true;
                FocusedWorkflowItem.IsDataDirty = false;
            }
            FocusedWorkflowItem.IsReadOnly = true;

            UnlockCommand.RaiseCanExecuteChanged();
        }

        private bool UnlockCommandCanExecute()
        {
            return (FocusedWorkflowItem != null)
                && FocusedWorkflowItem.IsOpenFromServer
                && !FocusedWorkflowItem.IsReadOnly;
        }

        /// <summary>
        /// Open workflow from server.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        private void OpenWorkflowFromServer()
        {
            var viewModel = new OpenWorkflowFromServerViewModel();
            if (DialogService.ShowDialog(viewModel).GetValueOrDefault())
            {
                if (viewModel.SelectedWorkflow != null)
                    this.OpenActivityFromServer(viewModel.SelectedWorkflow, viewModel.ShouldDownloadDependencies, !DefaultValueSettings.EnableTaskAssignment, viewModel.OpenForEditing);
            }
        }

        private void OpenActivityFromServer(StoreActivitiesDC activity, bool ShouldDownloadDependencies, bool isTask = false, bool openForEditing = true)
        {
            StoreActivitiesDC selectedWorkflowDC;
            ActivityAssemblyItem assembly = null;
            string activityLibraryName;
            string version;
            selectedWorkflowDC = activity;
            if (selectedWorkflowDC != null) // will be null if user cancelled
            {
                activityLibraryName = selectedWorkflowDC.ActivityLibraryName;
                version = selectedWorkflowDC.ActivityLibraryVersion;

                assembly = new ActivityAssemblyItem { Name = activityLibraryName, Version = System.Version.Parse(version), Env = selectedWorkflowDC.Environment.ToEnv() };
                List<ActivityAssemblyItem> references = new List<ActivityAssemblyItem>();

                Utility.WithContactServerUI(() =>
                {
                    using (var client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
                    {
                        List<ActivityAssemblyItem> referencedItems = Caching.ComputeDependencies(client, assembly);
                        if (ShouldDownloadDependencies)
                        {
                            references = Caching.CacheAndDownloadAssembly(client, referencedItems);
                        }
                        else
                        {
                            references = Caching.Match(referencedItems);
                        }
                    }
                });
                OpenStoreActivitiesDC(selectedWorkflowDC, assembly, references, isTask: isTask, openForEditing: openForEditing);
            }
        }

        /// <summary>
        /// Checks to see if the WorkflowItem is already in the editor active and adds it if it is not
        /// </summary>
        /// <param name="itemToCheck">Workflow to check</param>
        public bool CheckIsAlreadyInListOrAdd(WorkflowItem itemToCheck)
        {
            Contract.Requires(itemToCheck != null);
            var itemToFind = WorkflowItems.FirstOrDefault(wfi => 0 == wfi.CompareTo(itemToCheck));
            if (null != itemToFind)
            {
                FocusedWorkflowItem = itemToFind;
                return false;
            }
            else
            {
                WorkflowItems.Add(itemToCheck);
                FocusedWorkflowItem = itemToCheck;
                return true;
            }
        }

        /// <summary>
        /// Control when we can publish
        /// </summary>
        /// <returns>True if we can publish according to the CanExecute business rules</returns>
        private bool CanPublishCommandExecute()
        {
            return null != FocusedWorkflowItem &&
                   FocusedWorkflowItem.IsValid &&
                   AuthorizationService.Validate(FocusedWorkflowItem.Env, Permission.PublishWorkflow);
        }

        /// <summary>
        /// The publish command execute.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        private void PublishCommandExecute()
        {
            PublishingReply reply = new PublishingReply();
            try
            {
                Utility.DoTaskWithBusyCaption(TextResources.Publishing, () =>
                    {
                        // First, save to server if necessary, so that Publish can find it
                        FocusedWorkflowItem.Status = MarketplaceStatus.Public.ToString();
                        FocusedWorkflowItem.XamlCode = FocusedWorkflowItem.WorkflowDesigner.CompileProject.ProjectXaml;

                        // Stop publishing if saving failed.
                        if (!WorkflowsQueryServiceUtility.UsingClientReturn(client => UploadWorkflow(client, this.FocusedWorkflowItem, shouldBeLoose: false)))
                            return;

                        reply = WorkflowsQueryServiceUtility.UsingClientReturn(client => PublishCommandExecute_Implementation(client, this.FocusedWorkflowItem));
                    }, false);

                if (reply.StatusReply == null)
                {
                    return;
                }
                else if (0 != reply.StatusReply.Errorcode)
                {
                    MessageBoxService.Show(reply.StatusReply.ErrorMessage, TextResources.PublishError, MessageBoxButton.OK,
                                           MessageBoxImage.Error);
                }
                else
                {

                    if (string.IsNullOrEmpty(reply.PublishErrors))
                    {
                        MessageBoxService.ShowClickable(
                            string.Format(TextResources.WorkflowPublishedMsgFormat, this.FocusedWorkflowItem.Name,
                                          reply.PublishedVersion), TextResources.PublishStatus, reply.PublishedLocation);
                    }
                    else
                    {
                        MessageBoxService.ShowClickable(
                            string.Format(
                                TextResources.WorkflowPublishedWithErrorsMsgFormat,
                                this.FocusedWorkflowItem.Name, reply.PublishedVersion, reply.PublishErrors), TextResources.PublishStatus,
                            reply.PublishedLocation);
                        // Stop publishing if saving failed.                    
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFacingException(ex.Message);
            }
        }

        /// <summary>
        /// Separated out for testability
        /// </summary>
        public PublishingReply PublishCommandExecute_Implementation(IWorkflowsQueryService client, WorkflowItem workflow)
        {
            // Now, start the Publish process. In general this may take arbitrarily long.
            return client.PublishWorkflow(new PublishingRequest
            {
                Incaller = Environment.UserName,
                IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                WorkflowName = workflow.Name,
                WorkflowVersion = workflow.Version,
                Environment = workflow.Env.ToString()
            });
        }

        private void PrintCommandExecute()
        {
            if (this.FocusedWorkflowItem.WorkflowDesigner != null)
            {
                this.FocusedWorkflowItem.Print();
            }
        }

        private void PrintAllCommandExecute()
        {
            if (this.FocusedWorkflowItem.WorkflowDesigner != null)
            {
                this.FocusedWorkflowItem.PrintAll();
            }
        }

        private bool PrintCommandCanExecute()
        {
            bool canExecute = WorkflowItems != null &&
                WorkflowItems.Count > 0 &&
                FocusedWorkflowItem != null &&
                FocusedWorkflowItem.IsValid &&
                FocusedWorkflowItem.PrintState == PrintAction.NoneAction;
            return canExecute;
        }

        private void RefreshCommandExecute()
        {
            if (this.FocusedWorkflowItem.WorkflowDesigner != null)
                FocusedWorkflowItem.WorkflowDesigner.RefreshDesignerFromXamlCode();
        }

        private bool RefreshCommandCanExecute()
        {
            bool canExecute = WorkflowItems != null && WorkflowItems.Count > 0 && FocusedWorkflowItem != null && this.FocusedWorkflowItem.WorkflowDesigner != null;
            return canExecute;
        }

        private bool UndoCommandCanExecute()
        {
            bool canExecute = false;
            if (WorkflowItems != null && WorkflowItems.Count > 0 && FocusedWorkflowItem != null)
            {
                if (this.FocusedWorkflowItem.WorkflowDesigner != null)
                    if (FocusedWorkflowItem.WorkflowDesigner.CanUndo())
                    {
                        canExecute = true;
                    }
            }
            return canExecute;
        }

        private void UndoCommandExecute()
        {
            if (this.FocusedWorkflowItem.WorkflowDesigner != null)
            {
                FocusedWorkflowItem.WorkflowDesigner.Undo();
                FocusedWorkflowItem.DesignerView.Focus();
            }
        }

        private bool RedoCommandCanExecute()
        {
            bool canExecute = false;
            if (WorkflowItems != null && WorkflowItems.Count > 0 && FocusedWorkflowItem != null)
            {
                if (this.FocusedWorkflowItem.WorkflowDesigner != null)
                    if (FocusedWorkflowItem.WorkflowDesigner.CanRedo())
                    {
                        canExecute = true;
                    }
            }
            return canExecute;
        }

        private void RedoCommandExecute()
        {
            if (this.FocusedWorkflowItem.WorkflowDesigner != null)
            {
                FocusedWorkflowItem.WorkflowDesigner.Redo();
                FocusedWorkflowItem.DesignerView.Focus();
            }
        }

        private bool PasteCommandCanExecute()
        {
            bool canExecute = false;

            if (WorkflowItems != null && WorkflowItems.Count > 0 && FocusedWorkflowItem != null)
            {
                if (this.FocusedWorkflowItem.WorkflowDesigner != null)
                    canExecute = FocusedWorkflowItem.WorkflowDesigner.CanPaste();
            }

            return canExecute;
        }

        private bool CopyCommandCanExecute()
        {
            bool canExecute = false;
            if (WorkflowItems != null && WorkflowItems.Count > 0 && FocusedWorkflowItem != null)
            {
                if (this.FocusedWorkflowItem.WorkflowDesigner != null)
                    canExecute = FocusedWorkflowItem.WorkflowDesigner.CanCopy();
            }
            return canExecute;
        }

        private bool CutCommandCanExecute()
        {
            bool canExecute = false;

            if (WorkflowItems != null && WorkflowItems.Count > 0 && FocusedWorkflowItem != null)
            {
                if (this.FocusedWorkflowItem.WorkflowDesigner != null)
                    canExecute = FocusedWorkflowItem.WorkflowDesigner.CanCut();
            }
            return canExecute;
        }


        private void PasteCommandExecute()
        {
            if (this.FocusedWorkflowItem.WorkflowDesigner != null)
                FocusedWorkflowItem.WorkflowDesigner.Paste();
        }

        private void CutCommandExecute()
        {
            if (this.FocusedWorkflowItem.WorkflowDesigner != null)
                FocusedWorkflowItem.WorkflowDesigner.Cut();
        }

        private void CopyCommandExecute()
        {
            if (this.FocusedWorkflowItem.WorkflowDesigner != null)
                FocusedWorkflowItem.WorkflowDesigner.Copy();
        }

        /// <summary>
        /// Controls when the Save button is enabled via it's SynchronousDelegateCommand and CanExecute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool CanSaveFocusedWorkflow(string parameter)
        {
            bool canSave = false;

            switch (parameter)
            {
                case "ToLocal":
                    canSave = 0 < WorkflowItems.Count && (null != FocusedWorkflowItem);
                    break;

                case "ToMarketplace":
                    canSave = (0 < WorkflowItems.Count && null != FocusedWorkflowItem
                        && AuthorizationService.Validate(FocusedWorkflowItem.Env, Permission.UploadProjectToMarketplace));
                    break;

                case "ToServer":
                    canSave = (0 < WorkflowItems.Count && null != FocusedWorkflowItem
                        && FocusedWorkflowItem.IsDataDirty
                        && !FocusedWorkflowItem.IsReadOnly
                        && AuthorizationService.Validate(FocusedWorkflowItem.Env, Permission.SaveWorkflow));
                    break;

                case "ToImage":
                    canSave = (0 < WorkflowItems.Count && null != FocusedWorkflowItem);
                    break;
            }

            return canSave;
        }

        /// <summary>
        /// Called when the SaveFocusedWorkflowToLocal command execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void SaveFocusedWorkflowCommandExecute(string parameter)
        {
            bool wasSaved = false;

            switch (parameter)
            {
                case "ToLocal":
                    bool forceSaveAs = (parameter == "ToLocal");
                    wasSaved = SaveToLocal(this.FocusedWorkflowItem, forceSaveAs);
                    {
                        // Make sure DataDirty is set to the result of the Save
                        FocusedWorkflowItem.IsDataDirty = !wasSaved;
                    }
                    break;

                case "ToServer":
                    FocusedWorkflowItem.Status = MarketplaceStatus.Private.ToString();
                    SaveToServer(FocusedWorkflowItem);
                    break;

                case "ToMarketplace":
                    FocusedWorkflowItem.Status = MarketplaceStatus.Public.ToString();
                    SaveToServer(FocusedWorkflowItem);
                    break;

                case "ToImage":
                    SaveToImageFile(FocusedWorkflowItem);
                    break;
            }
        }

        /// <summary>
        /// Save a workflow to an image representation
        /// </summary>
        /// <param name="workflow"></param>
        public void SaveToImageFile(WorkflowItem workflow)
        {
            if (workflow == null)
            {
                throw new ArgumentNullException("workflow");
            }
            if (workflow.WorkflowDesigner != null)
                workflow.WorkflowDesigner.SaveWorkflowToBitmap();
        }

        /// <summary>
        /// Save focused workflow to server.
        /// </summary>
        /// <param name="workflow">The workflow to be saved.</param>
        // TODO: Changed to new Composite DataContract, needs to be verified
        public bool SaveToServer(WorkflowItem workflow)
        {
            bool isSuccess;
            IsBusy = true;
            BusyCaption = string.Format(UploadAsPrivateBusyMessage, workflow.Name, workflow.Version);
            isSuccess = this.UploadWorkflowWithBusy(workflow);
            workflow.HasMajorChanged = false;
            IsBusy = false;
            BusyCaption = String.Empty;
            return isSuccess;
        }

        public bool UploadWorkflowWithBusy(WorkflowItem workflow)
        {
            bool result = false;
            using (var client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
            {
                Utility.WithContactServerUI(() =>
                {
                    result = UploadWorkflow(client, workflow);
                }, false);
            }

            return result;
        }

        public bool UploadWorkflow(IWorkflowsQueryService client, WorkflowItem workflow, bool shouldBeLoose = true)
        {
            StoreActivitiesDC workflowDC = null;
            ActivityAssemblyItem fakeLibrary = null;

            var checkResult = WorkflowUploader.CheckCanUpload(client, workflow);
            if (!checkResult.Item1 && checkResult.Item2 == null)
            {
                return false;
            }
            else if (!checkResult.Item1 && checkResult.Item2 != null)
            {
                workflowDC = checkResult.Item2;
                fakeLibrary = GetActivityLibraries(client, workflowDC);
            }

            if (shouldBeLoose)
                workflow.SetXamlCode();

            var upResult = WorkflowUploader.Upload(client, workflow);
            if (upResult.Errorcode != 0)
            {
                ErrorMessageType = TextResources.ErrorSavingToServer;
                ErrorMessage = upResult.ErrorMessage + TextResources.SavingFailureMsg;
                return false;
            }

            if (workflowDC != null && fakeLibrary != null)
            {
                OpenStoreActivitiesDC(workflowDC, fakeLibrary);
            }

            if (this.FocusedWorkflowItem.Name == workflow.Name && this.FocusedWorkflowItem.Version == workflow.Version)
                this.FocusedWorkflowItem.TaskActivityGuid = workflow.TaskActivityGuid;

            return true;
        }

        /// <summary>
        /// Called when the SelectAssemblyAndActivityCommand execute.
        /// </summary>
        private void SelectAssemblyAndActivityCommandExecute()
        {
            var viewModel = new SelectAssemblyAndActivityViewModel();
            DialogService.ShowDialog(viewModel);
        }

        /// <summary>
        /// Called when upload assembly command execute.
        /// </summary>
        private void UploadAssemblyCommandExecute()
        {
            var viewModel = new UploadAssemblyViewModel();
            viewModel.Initialize(Caching.ActivityAssemblyItems);
            DialogService.ShowDialog(viewModel);
        }

        private void ShowAboutViewCommandExecute()
        {
            var viewModel = new AboutViewModel();
            DialogService.ShowDialog(viewModel);
        }

        private void ShowClickableMessageExecute()
        {
            var viewModel = new ClickableMessageViewModel();
            DialogService.ShowDialog(viewModel);
        }

        /// <summary>
        /// The event handler for WorkflowItems.CollectionChanged event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void WorkflowItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                    {
                        var workflowItem = item as WorkflowItem;
                        if (workflowItem != null)
                        {
                            workflowItem.PropertyChanged += workflowItem_PropertyChanged;
                        }
                    }
                    UpdateCommandsCanExecute();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    // Re-evaluate the CanExecutes that are in the UI
                    UpdateCommandsCanExecute();
                    break;
            }
        }

        /// <summary>
        /// Watch for property changes on the WorkflowItems 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void workflowItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsSavedToServer":
                case "IsDataDirty":
                case "XamlCode":
                case "PrintState":
                    {    // Re-evaluate the CanExecutes that are in the UI
                        UpdateCommandsCanExecute();
                    }
                    break;
                case "IsValid":
                    {
                        RaisePropertyChanged(() => this.TaskActivityOpened);
                        break;
                    }
                case "TaskActivityStatus":
                    RaisePropertyChanged(() => this.TaskActivityOpened);
                    break;
            }
        }

        /// <summary>
        /// Execute the CanExecutes in the UI
        /// </summary>
        private void UpdateCommandsCanExecute()
        {
            ClosingCommand.RaiseCanExecuteChanged();
            NewWorkflowCommand.RaiseCanExecuteChanged();
            SaveFocusedWorkflowCommand.RaiseCanExecuteChanged();
            OpenWorkflowCommand.RaiseCanExecuteChanged();
            PublishCommand.RaiseCanExecuteChanged();
            CompileCommand.RaiseCanExecuteChanged();
            PrintCommand.RaiseCanExecuteChanged();
            PrintAllCommand.RaiseCanExecuteChanged();
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
            CutCommand.RaiseCanExecuteChanged();
            CopyCommand.RaiseCanExecuteChanged();
            PasteCommand.RaiseCanExecuteChanged();
            RefreshCommand.RaiseCanExecuteChanged();
            CloseFocusedWorkflowCommand.RaiseCanExecuteChanged();
            UnlockCommand.RaiseCanExecuteChanged();
            ImportAssemblyCommand.RaiseCanExecuteChanged();
            UploadAssemblyCommand.RaiseCanExecuteChanged();
            OpenMarketplaceCommand.RaiseCanExecuteChanged();
            SelectAssemblyAndActivityCommand.RaiseCanExecuteChanged();
            ManageWorkflowTypeCommand.RaiseCanExecuteChanged();
            CheckInTaskActivityCommand.RaiseCanExecuteChanged();
            OpenTaskActivitiesCommand.RaiseCanExecuteChanged();

            CopyProjectCommand.RaiseCanExecuteChanged();
            ChangeAuthorCommand.RaiseCanExecuteChanged();
            OpenEnvSecurityOptionsCommand.RaiseCanExecuteChanged();
            OpenTenantSecurityOptionsCommand.RaiseCanExecuteChanged();
            DeleteProjectCommand.RaiseCanExecuteChanged();
            MoveProjectCommand.RaiseCanExecuteChanged();

            FindCommand.RaiseCanExecuteChanged();
            ReplaceCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Persists a workflow to disk, in various formats, such as:
        ///     wf      binary serialized
        ///     jpg     image
        ///     xaml    straight XAML text only, no metadata
        /// </summary>
        /// <param name="workflow">The workflow to be persisted to disk.</param>
        /// <param name="forceSaveAs">If true, forces the save as dialog to appear.</param>
        /// <returns>If true then the file was saved</returns>
        public bool SaveToLocal(WorkflowItem workflow, bool forceSaveAs)
        {
            bool isSuccess = false;   // This will be set to true on a successful save.
            string targetFileName;
            string fileExtension;

            // If the file name is null, or forceSaveAs is true, show the dialog and get the file name.
            if (string.IsNullOrEmpty(workflow.LocalFileFullName) || forceSaveAs)
            {
                string filter = TextResources.FoundryFileFilter;
                targetFileName = DialogService.ShowSaveDialogAndReturnResult(workflow.Name, filter);
                if (!string.IsNullOrEmpty(targetFileName))
                {
                    fileExtension = Path.GetExtension(targetFileName);
                    if (string.IsNullOrEmpty(fileExtension))
                    {
                        targetFileName += "." + DefaultSaveFileLocalExtension;
                        fileExtension = DefaultSaveFileLocalExtension;
                    }

                    switch (fileExtension.ToLower())
                    {
                        case ".wf":
                            using (var stream = File.Open(targetFileName, FileMode.Create))
                            {
                                var formatter = new BinaryFormatter();
                                workflow.SetXamlCode();
                                workflow.SetReferences();
                                formatter.Serialize(stream, workflow);
                                isSuccess = true;
                            }
                            break;

                        case ".jpg":
                            if (workflow.WorkflowDesigner != null)
                            {
                                workflow.WorkflowDesigner.SaveWorkflowToBitmap(targetFileName);
                                isSuccess = true;
                            }
                            break;
                        case ".xaml":
                            workflow.WorkflowDesigner.Save(targetFileName);
                            isSuccess = true;
                            break;
                        default:
                            throw new UserFacingException(TextResources.SaveToLocalFailureMsg);
                    }
                }
            }

            return isSuccess;
        }

        private ActivityAssemblyItem GetActivityLibraries(IWorkflowsQueryService client, StoreActivitiesDC activities)
        {
            string activityLibraryName = activities.ActivityLibraryName;
            Version version = System.Version.Parse(activities.ActivityLibraryVersion);

            ActivityAssemblyItem fakeLibrary = new ActivityAssemblyItem { Name = activityLibraryName, Version = version };
            Caching.DownloadAndCacheAssembly(client, Caching.ComputeDependencies(client, fakeLibrary));

            return fakeLibrary;
        }

        /// <summary>
        /// Opens a store activity from server and checks if messages should be popped up
        /// </summary>
        /// <param name="workflowDC"></param>
        /// <param name="fakeLibrary"></param>
        public void OpenStoreActivitiesDC(
            StoreActivitiesDC workflowDC,
            ActivityAssemblyItem fakeLibrary,
            List<ActivityAssemblyItem> references = null,
            bool isTask = false,
            bool openForEditing = true)
        {
            Env env = workflowDC.Environment.ToEnv();
            bool canOverrideLock = AuthorizationService.Validate(env, Permission.OverrideLock);

            if (!Enum.IsDefined(typeof(Env), env))
                throw new ArgumentException("Env is invalid.");

            if (!AuthorizationService.Validate(env, Permission.SaveWorkflow)) {
                openForEditing = false;
                }
            else if (AuthorizationService.Validate(env, Permission.OverrideLock)
                || (workflowDC.Locked && workflowDC.LockedBy == Environment.UserName)) {
            }
            else if (workflowDC.Locked && openForEditing) {
                openForEditing = false;
                MessageBoxService.OpenLockedActivityByNonAdmin(workflowDC.LockedBy);
            }

            Utility.DoTaskWithBusyCaption(TextResources.UiMayNotRespondMsg, () =>
            {
                if (openForEditing)
                {
                    StoreActivitesSetLock(workflowDC);
                }

                CheckIsAlreadyInListOrAdd(DataContractTranslator.StoreActivitiyDCToWorkflowItem(workflowDC, fakeLibrary, references, isTask: isTask));
            }, false);

            FocusedWorkflowItem.IsReadOnly = !openForEditing;
            FocusedWorkflowItem.IsSavedToServer = true;
            FocusedWorkflowItem.IsDataDirty = false;
        }

        private void StoreActivitesSetLock(StoreActivitiesDC workflowDC)
        {
            WorkflowsQueryServiceUtility.UsingClient(client =>
            {
                workflowDC.Incaller = Assembly.GetExecutingAssembly().GetName().Name;
                workflowDC.IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                workflowDC.Locked = true;
                workflowDC.LockedBy = Environment.UserName;
                workflowDC.UpdatedDateTime = DateTime.UtcNow;
                var reply = client.StoreActivitiesUpdateLock(workflowDC, workflowDC.UpdatedDateTime);
                reply.CheckErrors();
            });
        }

        /// <summary>
        /// Unlock a workflow on query service
        /// </summary>
        /// <param name="workflowDC"></param>
        public void StoreActivitesUnlockWithBusy(WorkflowItem workflow, bool shouldUpdateNewVersion)
        {
            Utility.WithContactServerUI(() =>
            {
                StoreActivitesUnlock(workflow, shouldUpdateNewVersion);
            });
        }

        public void StoreActivitesUnlock(WorkflowItem workflow, bool shouldUpdateNewVersion)
        {
            if (!workflow.IsReadOnly)
            {
                StoreActivitiesDC workflowDC = DataContractTranslator.ActivityItemToStoreActivitiyDC(workflow);
                WorkflowsQueryServiceUtility.UsingClient(client =>
                {
                    workflowDC.Name = workflow.OriginalName;
                    workflowDC.Incaller = Assembly.GetExecutingAssembly().GetName().Name;
                    workflowDC.IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    workflowDC.Locked = false;
                    workflowDC.LockedBy = Environment.UserName;
                    workflowDC.UpdatedDateTime = DateTime.UtcNow;
                    workflowDC.Version = shouldUpdateNewVersion ? workflow.Version : workflow.OldVersion;
                    var reply = client.StoreActivitiesUpdateLock(workflowDC, workflowDC.UpdatedDateTime);
                    reply.CheckErrors();
                });
            }
        }

        private bool CheckActivitiesUnlock(WorkflowItem workflow)
        {
            if (workflow.IsReadOnly)
                return false;

            StoreActivitiesDC wf = null;
            WorkflowsQueryServiceUtility.UsingClient(client =>
                {
                    StoreActivitiesDC workflowDC = new StoreActivitiesDC()
                    {
                        Name = workflow.OriginalName ?? workflow.Name,
                        Version = workflow.OldVersion ?? workflow.Version,
                        Environment = workflow.Env.ToString()
                    };
                    workflowDC.SetIncaller();

                    var result = client.StoreActivitiesGet(workflowDC);
                    if (result.Any())
                    {
                        wf = result[0];
                        wf.StatusReply.CheckErrors();
                    }
                });

                if (wf.Locked)
                {
                    if (wf.LockedBy == Environment.UserName)
                    {
                        return true;
                    }
                    else
                    {
                        bool canOverrideLock = AuthorizationService.Validate(wf.Environment.ToEnv(), Permission.OverrideLock);
                        if (!canOverrideLock)
                        {
                            MessageBoxService.LockChangedWhenAuthorUnlocking(wf.LockedBy);
                            return false;
                        }

                    }
                }

                return true;
        }

    }
}
