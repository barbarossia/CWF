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
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
    using Microsoft.Support.Workflow.Authoring.Behaviors;
    using Microsoft.Support.Workflow.Authoring.Common;
    using Models;
    using Practices.Prism.Commands;
    using Security;
    using Services;

    /// <summary>
    /// The main window view model.
    /// </summary>
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private const string UploadToMarketplaceBusyMessage = "Uploading {0} version {1} to Marketplace..";   // The message we will display while uploading.
        private const string UploadAsPrivateBusyMessage = "Saving {0} version {1} to the server...";          // The message we will display while saving as private.
        private const string DefaultSaveFileLocalExtension = ".wf";                                           // When saving a file local, if no file extension is specified, this will be used.

        private ActivityItem focusedActivityItem;   // The focused activity item.
        private WorkflowItem focusedWorkflowItem;   // The focused workflow item.
        private UserInfoPaneViewModel userInfo;     // Exposes info about the current logged user (name, friendly name of environment, user pic)
        private string title;                       // The title.
        private ObservableCollection<WorkflowItem> workflowItems;   // The workflow items.
        private string errorMessage;                // error message exposed to a consumer of this viewmodel
        private string errorMessageType;            // error message type exposed to a consumer of this viewmodel - intended use is as a caption, perhaps in a message box
        private bool isAdministrator;
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
            Task.Factory.StartNew(() => UserInfo = new UserInfoPaneViewModel());

            var currentUserSecurityLevel =
               AuthorizationService.GetSecurityLevel(AuthorizationService.CurrentPrincipalFunc() as WindowsPrincipal);
            if (currentUserSecurityLevel == SecurityLevel.Administrator)
                IsAdministrator = true;
            else
                IsAdministrator = false;
            Title = string.Format(CommonMessages.MainWindowCaption, Environment.UserName, currentUserSecurityLevel);
            PropertyChanged += MainWindowViewModelPropertyChanged;

            InitializeCommands();
            InitializeWorkspace();

            OnSaveToLocal = SaveToLocal;
            OnSaveToServer = SaveToServer;
            OnStoreActivitesUnlockWithBusy = StoreActivitesUnlockWithBusy;
            OnStoreActivitesUnlock = StoreActivitesUnlock;
        }

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

        /// <summary>
        /// Get or set a value indicate if Admin is logining.
        /// </summary>
        public bool IsAdministrator
        {
            get { return this.isAdministrator; }
            set
            {
                this.isAdministrator = value;
                RaisePropertyChanged(() => IsAdministrator);
            }
        }

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
        /// Gets CurrentUserSecurityLevel. Represent the security level of current user. Not a dependency property since it is set only in ctor.
        /// </summary>
        public SecurityLevel CurrentUserSecurityLevel { get; private set; }

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
                        result = MessageBoxService.ShowClosingComfirmation(wf.Name);
                    }
                    else if (wf.IsOpenFromServer && !wf.IsDataDirty) //open from server and not make the changes
                    {
                        result = MessageBoxService.ShowKeepLockedComfirmation(wf.Name);
                    }
                    else if (!wf.IsOpenFromServer && wf.IsDataDirty)//open from local and make the changes
                    {
                        result = MessageBoxService.ShowLocalSavingComfirmation(wf.Name);
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

        public string ErrorMessage
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
                   FocusedWorkflowItem.IsValid;
        }

        /// <summary>
        /// Called when the compile command execute.
        /// </summary>
        private void CompileCommandExecute()
        {
            try
            {
                Utility.DoTaskWithBusyCaption("Compiling...", () =>
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
                MessageBoxService.Show("There was an error compiling the workflow." + Environment.NewLine + result.Exception.Message,
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
                    result = MessageBoxService.ShowClosingComfirmation(itemToClose.Name);
                }
                else if (itemToClose.IsOpenFromServer && !itemToClose.IsDataDirty) //open from server and not make the changes
                {
                    result = MessageBoxService.ShowKeepLockedComfirmation(itemToClose.Name);
                }
                else if (!itemToClose.IsOpenFromServer && itemToClose.IsDataDirty)//open from local and make the changes
                {
                    result = MessageBoxService.ShowLocalSavingComfirmation(itemToClose.Name);
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
                    if (result.Value.HasFlag(SavingResult.Unlock))
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
            CheckInTaskActivityCommand = new DelegateCommand(TaskActivityCheckInCommandExecute);
            OpenTaskActivitiesCommand = new DelegateCommand(OpenTaskActivitiesCommandExecute);
            NewWorkflowCommand = new DelegateCommand(NewWorkflowCommandExecute);
            SaveFocusedWorkflowCommand = new DelegateCommand<string>(SaveFocusedWorkflowCommandExecute, CanSaveFocusedWorkflow);
            OpenWorkflowCommand = new DelegateCommand<string>(OpenWorkflowCommandExecute);
            CompileCommand = new DelegateCommand(CompileCommandExecute, CanCompileCommandExecute);
            PublishCommand = new DelegateCommand(PublishCommandExecute, CanPublishCommandExecute);
            CloseFocusedWorkflowCommand = new DelegateCommand(CloseFocusedWorkflowCommandExecute, CanCloseFocusedWorkflowCommandExecute);
            ImportAssemblyCommand = new DelegateCommand(ImportAssemblyCommandExecute, ImportCommandCanExecute);
            UploadAssemblyCommand = new DelegateCommand(UploadAssemblyCommandExecute);
            SelectAssemblyAndActivityCommand = new DelegateCommand(SelectAssemblyAndActivityCommandExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute);

            OpenMarketplaceCommand = new DelegateCommand(OpenMarketplaceCommandExecute);
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
            ManageWorkflowTypeCommand = new DelegateCommand(ManageWokflowTypesCommandExecute, () => { return IsAdministrator; });
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
                                Utility.DoTaskWithBusyCaption("UI may not respond. Please wait...",
                                () =>
                                {
                                    using (var client = WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient())
                                    {
                                        TaskActivityDC request = new TaskActivityDC()
                                        {
                                            Incaller = Utility.GetCallerName(),
                                            IncallerVersion = Utility.GetCallerVersion(),
                                            Id = viewModel.SelectedActivity.Id,
                                            Status = TaskActivityStatus.Editing
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

        private bool ImportCommandCanExecute()
        {
            return this.FocusedWorkflowItem != null;
        }

        /// <summary>
        /// Called when create new workflow
        /// </summary>
        private void NewWorkflowCommandExecute()
        {
            BusyCaption = CommonMessages.BusyContactingServer;
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
                    Utility.DoTaskWithBusyCaption("Loading...", () =>
                    {
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
                    string workflowFileName = DialogService.ShowOpenFileDialogAndReturnResult("Workflow files (*.wf)|*.wf", "Open Workflow File");
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
                Utility.DoTaskWithBusyCaption("Loading...", () =>
                {
                    var recoverdWorkflow = (WorkflowItem)Utility.DeserializeSavedContent(fileName);

                    // Add it or focus it!
                    if (CheckIsAlreadyInListOrAdd(recoverdWorkflow))
                    {
                        FocusedWorkflowItem.IsReadOnly = false;
                        FocusedWorkflowItem.IsDataDirty = false;
                        FocusedWorkflowItem.IsOpenFromServer = false;
                    }
                }, false);
            }
            catch (SerializationException)
            {
                throw new UserFacingException(String.Format("'{0}' is not a valid workflow", fileName));
            }
        }

        private void UnlockCommandExecute()
        {
            bool needToSave = false;
            if (!FocusedWorkflowItem.IsSavedToServer)
            {
                SavingResult? result = MessageBoxService.ShowUnlockComfirmation(FocusedWorkflowItem.Name);
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
                    Version = FocusedWorkflowItem.OldVersion
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
                            if (!AuthorizationService.IsAdministrator(AuthorizationService.CurrentPrincipalFunc()))
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
                FocusedWorkflowItem.IsReadOnly = true;
                FocusedWorkflowItem.IsSavedToServer = true;
                FocusedWorkflowItem.IsDataDirty = false;
            }

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
            viewModel.LoadData();
            if (DialogService.ShowDialog(viewModel).GetValueOrDefault())
            {
                if (viewModel.SelectedWorkflow != null)
                    this.OpenActivityFromServer(viewModel.SelectedWorkflow, viewModel.ShouldDownloadDependencies);
            }
        }

        private void OpenActivityFromServer(StoreActivitiesDC activity, bool ShouldDownloadDependencies, bool isTask = false)
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

                assembly = new ActivityAssemblyItem { Name = activityLibraryName, Version = System.Version.Parse(version) };
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
                OpenStoreActivitiesDC(selectedWorkflowDC, assembly, references, isTask: isTask);
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
                   FocusedWorkflowItem.IsValid;
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
                Utility.DoTaskWithBusyCaption("Publishing...", () =>
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
                    MessageBoxService.Show(reply.StatusReply.ErrorMessage, "Publish Error", MessageBoxButton.OK,
                                           MessageBoxImage.Error);
                }
                else
                {

                    if (string.IsNullOrEmpty(reply.PublishErrors))
                    {
                        MessageBoxService.ShowClickable(
                            string.Format("Workflow: {0} Version {1} was published to:", this.FocusedWorkflowItem.Name,
                                          reply.PublishedVersion), "Publish Status", reply.PublishedLocation);
                    }
                    else
                    {
                        MessageBoxService.ShowClickable(
                            string.Format(
                                "Workflow: {0} Version {1} was published with errors reported during publish: {2}",
                                this.FocusedWorkflowItem.Name, reply.PublishedVersion, reply.PublishErrors), "Publish Status",
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
                WorkflowVersion = workflow.Version
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

                case "SaveAsToLocal":
                    canSave = 0 < WorkflowItems.Count && (null != FocusedWorkflowItem);
                    break;

                case "ToMarketplace":
                    canSave = (0 < WorkflowItems.Count && null != FocusedWorkflowItem);
                    break;

                case "ToServer":
                    canSave = (0 < WorkflowItems.Count && null != FocusedWorkflowItem
                        && FocusedWorkflowItem.IsDataDirty
                        && !FocusedWorkflowItem.IsReadOnly);
                    break;

                case "SaveAsTemplate":
                    canSave = (0 < WorkflowItems.Count
                               && null != FocusedWorkflowItem
                               && FocusedWorkflowItem.IsSavedToServer);
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
            Utility.DoTaskWithBusyCaption("Saving...", () =>
            {
                result = WorkflowsQueryServiceUtility.UsingClientReturn(client =>
                {
                    return UploadWorkflow(client, workflow);
                });
            }, false);

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
                ErrorMessageType = "Error Saving to Server";
                ErrorMessage = upResult.ErrorMessage + "\r\n\r\nYour changes were not successfully saved to the server.";
                return false;
            }
            else
            {
                OnStoreActivitesUnlock(workflow, false);
                workflow.OriginalName = workflow.Name;
                workflow.OldVersion = workflow.Version;
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
            SaveFocusedWorkflowCommand.RaiseCanExecuteChanged();
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
                string filter = "Workflow files (*.wf)|*.wf|XAML Text files (*.xaml)|*.xaml|JPEG Image files (*.jpg)|*.jpg";
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
                            throw new UserFacingException("The file type is not recognized. The save to local could not be completed.");
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
            bool isTask = false)
        {
            bool isReadOnly = false;
            bool isAdministrator = AuthorizationService.IsAdministrator(AuthorizationService.CurrentPrincipalFunc());

            if (!workflowDC.Locked ||
                (isAdministrator) ||
                (!isAdministrator && workflowDC.Locked && workflowDC.LockedBy == Environment.UserName))
            {
                MessageBoxResult result = MessageBoxService.OpenActivity(workflowDC.Name);
                if (result == MessageBoxResult.Yes)
                {
                    //open workflow for edit mode
                    isReadOnly = false;
                }
                else if (result == MessageBoxResult.No)
                {
                    //open worflow for readonly mode
                    isReadOnly = true;
                }
                else //cancel
                {
                    return;
                }
            }
            else
            {
                if (MessageBoxService.OpenLockedActivityByNonAdmin(workflowDC.LockedBy) == MessageBoxResult.OK)
                    isReadOnly = true;
                else
                    return;
            }


            Utility.DoTaskWithBusyCaption("UI may not respond. Please wait...", () =>
            {
                if (!isReadOnly)
                {
                    StoreActivitesSetLock(workflowDC);
                }

                CheckIsAlreadyInListOrAdd(DataContractTranslator.StoreActivitiyDCToWorkflowItem(workflowDC, fakeLibrary, references, isTask: isTask));
            }, false);

            FocusedWorkflowItem.IsReadOnly = isReadOnly;
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
                var reply = client.StoreActivitiesSetLock(workflowDC, workflowDC.UpdatedDateTime);
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
                    var reply = client.StoreActivitiesSetLock(workflowDC, workflowDC.UpdatedDateTime);
                    reply.CheckErrors();
                });
            }
        }
    }
}
