using System;
using System.Activities.Core.Presentation;
using System.Activities.Presentation;
using System.Activities.Presentation.View;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Activities;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.AddIns.Views;
using Microsoft.Support.Workflow.Authoring.Behaviors;
using Microsoft.Support.Workflow.Authoring.CompositeActivity;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;
using System.Threading.Tasks;
using CWF.DataContracts;
using System.Windows.Threading;
using Microsoft.Support.Workflow.Authoring.Security;
using System.Security.Principal;
using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

namespace Microsoft.Support.Workflow.Authoring.AddIns
{
    public class DesignerAddIn : MarshalByRefObject, IDesignerContract, IDisposable
    {
        public static DesignerAddIn Current { get; set; }

        private WorkflowEditorView workflowView;
        private WorkflowProjectExplorerView peView;
        private WorkflowPropertyView propertyView;
        private ToolboxView toolbox;
        private UndoEngine undoEngine;
        private bool isTask;
        private List<TaskAssignment> tasks = new List<TaskAssignment>();

        public WorkflowEditorViewModel WorkflowEditorVM { get; private set; }

        public event EventHandler DesignerChanged;
        public event EventHandler PrintStateChanged;
        public event EventHandler DesignerReloaded;
        public event GetTaskLastVersionEventHandler GetTaskLastVersionChanged;
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        public DesignerAddIn()
        {
            CreateIntellisense(cancellationToken);

            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
            new DesignerMetadata().Register();
            WorkflowEditorVM = new WorkflowEditorViewModel(cancellationToken);
            WorkflowEditorVM.PropertyChanged += this.WorkflowEditorVM_PropertyChanged;
            WorkflowEditorVM.PrintStateChanged += new EventHandler(this.OnPrintStateChanged);
            WorkflowEditorVM.DesignerChanged += new EventHandler(this.WorkflowDesignerChanged);
            WorkflowEditorVM.GetTaskLastVersionChanged += new GetTaskLastVersionEventHandler(this.OnGetTaskLastVersion);
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            Task.Factory.StartNew(() => { var m = AuthorizationService.EnvPermissionMaps; }, cancellationToken.Token);
            Current = this;
        }

        public bool HasTask
        {
            get
            {
                var currentTasks = MultipleAuthorService.GetTasks(WorkflowEditorVM.WorkflowDesigner);
                return currentTasks.Count > 0;
            }
        }

        public List<TaskAssignment> Tasks
        {
            get
            {
                var currentTasks = MultipleAuthorService.GetTasks(WorkflowEditorVM.WorkflowDesigner);
                var unassignedTasks = tasks.Where(t => !currentTasks.Any(ct => ct.TaskId == t.TaskId));
                foreach (var t in unassignedTasks)
                {
                    t.TaskStatus = TaskActivityStatus.Unassigned;
                }
                var uploadTasks = currentTasks.Where(t => t.TaskStatus == TaskActivityStatus.New).ToList();
                uploadTasks.AddRange(unassignedTasks);
                return uploadTasks;
            }
        }

        private void OnGetTaskLastVersion(object sender, GetTaskEventArgs e)
        {
            if (GetTaskLastVersionChanged != null)
            {
                GetTaskLastVersionChanged(this, e);
            }
        }

        public void WorkflowDesignerChanged(object sender, EventArgs e)
        {
            if (DesignerChanged != null)
            {
                DesignerChanged(this, e);
            }
        }

        public void OnPrintStateChanged(object sender, EventArgs e)
        {
            if (PrintStateChanged != null)
            {
                PrintStateChanged(this, e);
            }
        }

        public void SetWorkflowName(string name)
        {
            WorkflowEditorVM.Name = name;
        }

        public void ClearUndo() {
            WorkflowEditorVM.ClearUndo();
        }

        public void InitWorkflow(string name, string xaml, bool isTask)
        {
            this.isTask = isTask;

            XamlIndexTreeHelper.CreateIndexTree(xaml);

            workflowView = new WorkflowEditorView();
            workflowView.DataContext = WorkflowEditorVM;

            WorkflowEditorVM.Init(name, xaml, isTask);

            peView = new WorkflowProjectExplorerView();
            var peVM = new ProjectExplorerViewModel(WorkflowEditorVM);
            peView.DataContext = peVM;
            peVM.WorkflowOutlineFocuceChanged += new ActivityFocuceEventHandler(workflowView.OnActivityFocuceChanged);

            propertyView = new WorkflowPropertyView();
            propertyView.DataContext = WorkflowEditorVM;

            toolbox = new ToolboxView();
            toolbox.DataContext = new ToolboxViewModel(isTask);
        }

        public void RefreshTasks()
        {
            tasks = MultipleAuthorService.GetTasks(WorkflowEditorVM.WorkflowDesigner)
                .Where(t => t.TaskStatus != TaskActivityStatus.New).ToList();
        }

        public INativeHandleContract ToolboxView
        {
            get
            {
                return FrameworkElementAdapters.ViewToContractAdapter(this.toolbox);
            }
        }

        public INativeHandleContract WorkflowEditorView
        {
            get
            {
                return FrameworkElementAdapters.ViewToContractAdapter(this.workflowView);
            }
        }

        public INativeHandleContract ProjectExplorerView
        {
            get
            {
                return FrameworkElementAdapters.ViewToContractAdapter(this.peView);
            }
        }

        public INativeHandleContract WorkflowPropertyView
        {
            get
            {
                return FrameworkElementAdapters.ViewToContractAdapter(this.propertyView);
            }
        }

        public PrintAction PrintState
        {
            get
            {
                return WorkflowEditorVM.ShouldBePrint;
            }
        }

        public void Undo()
        {
            WorkflowEditorVM.WorkflowDesigner.Context.Services.GetService<UndoEngine>().Undo();
        }

        public void Redo()
        {
            WorkflowEditorVM.WorkflowDesigner.Context.Services.GetService<UndoEngine>().Redo();
        }

        public void Cut()
        {
            CutCopyPasteHelper.DoCut(WorkflowEditorVM.WorkflowDesigner.Context);
        }

        public void Copy()
        {
            CutCopyPasteHelper.DoCopy(WorkflowEditorVM.WorkflowDesigner.Context);
        }

        public void Paste()
        {
            CutCopyPasteHelper.DoPaste(WorkflowEditorVM.WorkflowDesigner.Context);
        }

        public void Print()
        {
            WorkflowEditorVM.ShouldBePrint = Behaviors.PrintAction.PrintUserSelection;
        }

        public void PrintAll()
        {
            WorkflowEditorVM.ShouldBePrint = Behaviors.PrintAction.PrintAll;
        }

        public bool CanUndo()
        {
            return WorkflowEditorVM.WorkflowDesigner.Context.Services.GetService<UndoEngine>().GetUndoActions().Any();
        }

        public bool CanRedo()
        {
            return WorkflowEditorVM.WorkflowDesigner.Context.Services.GetService<UndoEngine>().GetRedoActions().Any();
        }

        public bool CanCut()
        {
            return CutCopyPasteHelper.CanCut(WorkflowEditorVM.WorkflowDesigner.Context);
        }

        public bool CanCopy()
        {
            return CutCopyPasteHelper.CanCopy(WorkflowEditorVM.WorkflowDesigner.Context);
        }

        public bool CanPaste()
        {
            return CutCopyPasteHelper.CanPaste(WorkflowEditorVM.WorkflowDesigner.Context);
        }

        public IEnumerable<ActivityAssemblyItem> DependencyAssemblies
        {
            get
            {
                return AddInCaching.ActivityAssemblyItems;
            }
        }

        public CompileProject CompileProject
        {
            get
            {
                return DependencyAnalysisService.GetCompileProject(WorkflowEditorVM);
            }
        }

        public string Xaml
        {
            get
            {
                return WorkflowEditorVM.WorkflowDesigner.LooseXaml();
            }
        }

        public string CompiledXaml
        {
            get
            {
                return WorkflowEditorVM.WorkflowDesigner.CompilableXaml();
            }
        }

        /// <summary>
        /// Used to Properly determine if this workflow is a WorkflowService 
        /// </summary>
        /// <returns>True if a WorkfowService, false if not</returns>
        public bool IsWorkflowService()
        {
            bool isWorkflowService = false;
            var modelService = WorkflowEditorVM.WorkflowDesigner.GetModelService();
            if (modelService != null && modelService.Root.GetCurrentValue() is WorkflowService)
            {
                isWorkflowService = true;
            }
            return isWorkflowService;
        }

        public bool IsWorkflowValid()
        {
            return WorkflowEditorVM.IsValid;
        }

        public void SetReadOnly(bool isReadOnly)
        {
            WorkflowEditorVM.IsReadOnly = isReadOnly;
        }

        public void Save(string targetFileName)
        {
            WorkflowEditorVM.WorkflowDesigner.Save(targetFileName);
        }

        public void SaveWorkflowToBitmap()
        {
            BitmapFrame frame = this.ConvertWorkflowToBitmap();
            string fileName = AddInDialogService.ShowSaveDialogAndReturnResult(this.WorkflowEditorVM.Name + ".jpg", TextResources.ImageFilesFilter);
            if (!String.IsNullOrEmpty(fileName))
            {
                FileService.SaveImageToDisk(fileName, frame);
            }
        }

        public void SaveWorkflowToBitmap(string targetFile)
        {
            BitmapFrame frame = this.ConvertWorkflowToBitmap();
            FileService.SaveImageToDisk(targetFile, frame);
        }

        public void RefreshDesignerFromXamlCode()
        {
            WorkflowEditorVM.RefreshDesignerFromXamlCode();
        }

        public bool ImportAssemblies(IEnumerable<ActivityAssemblyItem> importAssemblies, bool canRefresh = true)
        {
            if (!AddInCaching.ImportAssemblies(importAssemblies))
            {
                return false;
            }

            //refresh toolbox
            this.toolbox.DataContext = new ToolboxViewModel(isTask);

            CreateIntellisense(cancellationToken);

            if (canRefresh)
            {
                RefreshDesignerFromXamlCode();
            }

            return true;
        }

        public void FinishTaskAssigned()
        {
            WorkflowEditorVM.FinishTaskAssigned();
            RefreshTasks();
        }

        public void ShowSearchBar(bool isReplacementMode) {
            workflowView.ShowXamlAndSearchBar(isReplacementMode);
        }

        private static void CreateIntellisense(CancellationTokenSource cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                ExpressionEditorHelper.CreateIntellisenseList();
            }, cancellationToken.Token);
        }

        private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            return AssemblyService.Resolve(args.Name, AddInCaching.ActivityAssemblyItems);
        }

        private void WorkflowEditorVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "WorkflowDesigner":
                    {
                        this.undoEngine = WorkflowEditorVM.WorkflowDesigner.Context.Services.GetService<UndoEngine>();
                        this.undoEngine.UndoRedoBufferChanged += new EventHandler(WorkflowDesignerChanged);
                    }
                    break;
                case "TaskActivityAssignException":
                    {
                        WorkflowEditorVM.WorkflowDesigner.Context.Services.GetService<UndoEngine>().Undo();
                        AddInMessageBoxService.CannotAssign();
                    }
                    break;
            }
        }

        /// <summary>
        /// Saves a workflow to an image file
        /// </summary>
        /// <param name="workflow"></param>
        /// <returns></returns>
        private BitmapFrame ConvertWorkflowToBitmap()
        {
            const double dpi = 96.0;
            var RoutedCommand = this.GetRoutedCommand();
            RoutedCommand.Execute(null,
                this.WorkflowEditorVM.WorkflowDesigner.Context.Services.GetService<DesignerView>());

            // this is the designer area that we are going to save
            Visual areaToSave = ((DesignerView)VisualTreeHelper.GetChild(
                this.WorkflowEditorVM.WorkflowDesigner.View, 0)).RootDesigner;

            // get the size of the targeting area
            Rect size = VisualTreeHelper.GetDescendantBounds(areaToSave);
            var bitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height,
               dpi, dpi, PixelFormats.Pbgra32);
            bitmap.Render(areaToSave);
            return BitmapFrame.Create(bitmap);
        }

        /// <summary>
        /// separated from SaveWorkflowToBitmap for unit test
        /// </summary>
        /// <returns></returns>
        public RoutedCommand GetRoutedCommand()
        {
            var RoutedCommand = DesignerView.FitToScreenCommand as RoutedCommand;
            return RoutedCommand;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void SetNewTasksToAssigned(Guid[] ids)
        {
            MultipleAuthorService.SetNewTasksToAssigned(WorkflowEditorVM.WorkflowDesigner, ids);
        }

        public void RollbackAssignedTasks(Guid[] ids)
        {
            MultipleAuthorService.RollbackAssignedTasks(WorkflowEditorVM.WorkflowDesigner, ids);
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (disposing)
            {
                // Dispose managed resources.
                cancellationToken.Cancel();
                cancellationToken.Dispose();
                Dispatcher.CurrentDispatcher.InvokeShutdown();
            }
        }

    }
}