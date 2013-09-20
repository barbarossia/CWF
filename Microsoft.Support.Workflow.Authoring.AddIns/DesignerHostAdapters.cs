using System;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.Behaviors;

namespace Microsoft.Support.Workflow.Authoring.AddIns
{
    [Serializable]
    public class DesignerHostAdapters : MarshalByRefObject
    {
        private IDesignerContract proxy;
        private string workflowName = string.Empty;
        private bool isTask;

        public event EventHandler DesignerChanged;
        public event EventHandler PrintStateChanged;
        public event EventHandler DesignerReloaded;
        public event GetTaskLastVersionEventHandler GetTaskLastVersionChanged;

        #region Property

        public FrameworkElement View
        {
            get
            {
                return FrameworkElementAdapters.ContractToViewAdapter(this.proxy.WorkflowEditorView);
            }
        }

        public FrameworkElement PropertyInspectorView
        {
            get
            {
                return FrameworkElementAdapters.ContractToViewAdapter(this.proxy.WorkflowPropertyView);
            }
        }

        public FrameworkElement ProjectExplorerView
        {
            get { return FrameworkElementAdapters.ContractToViewAdapter(this.proxy.ProjectExplorerView); }
        }

        public FrameworkElement ToolboxView
        {
            get { return FrameworkElementAdapters.ContractToViewAdapter(this.proxy.ToolboxView); }
        }

        public IEnumerable<ActivityAssemblyItem> DependencyAssemblies
        {
            get { return this.proxy.DependencyAssemblies; }
        }

        public CompileProject CompileProject
        {
            get { return this.proxy.CompileProject; }
        }

        public string XamlCode
        {
            get
            {
                return this.proxy.Xaml;
            }
        }

        public string CompiledXaml
        {
            get
            {
                return this.proxy.CompiledXaml;
            }
        }

        public bool IsWorkflowService
        {
            get { return this.proxy.IsWorkflowService(); }
        }

        public PrintAction PrintState
        {
            get
            {
                return this.proxy.PrintState;
            }
        }

        public bool HasTask
        {
            get { return proxy.HasTask; }
        }

        public List<TaskAssignment> Tasks
        {
            get { return proxy.Tasks; }
        }

        #endregion

        public DesignerHostAdapters() { }
        public DesignerHostAdapters(string name, string projectXamlCode, List<ActivityAssemblyItem> references, bool isTask)
            : base()
        {
            this.workflowName = name;
            this.isTask = isTask;
            this.InitAddIn(name, projectXamlCode, references, isTask);
        }

        /// <summary>
        /// Unload AppDoamin when close workflow
        /// </summary>
        public void UnloadAddIn()
        {
            AddinPreloader.Current.Unload(this.proxy);
        }

        public void SetReadOnly(bool isReadOnly)
        {
            proxy.SetReadOnly(isReadOnly);
        }

        public void Undo()
        {
            this.proxy.Undo();
        }

        public bool CanUndo()
        {
            return this.proxy.CanUndo();
        }

        public void Redo()
        {
            this.proxy.Redo();
        }

        public bool CanRedo()
        {
            return this.proxy.CanRedo();
        }

        public void Cut()
        {
            this.proxy.Cut();
        }

        public bool CanCut()
        {
            return this.proxy.CanCut();
        }

        public void Copy()
        {
            this.proxy.Copy();
        }

        public bool CanCopy()
        {
            return this.proxy.CanCopy();
        }

        public void Paste()
        {
            this.proxy.Paste();
        }

        public bool CanPaste()
        {
            return this.proxy.CanPaste();
        }

        public void Print()
        {
            this.proxy.Print();
        }

        public void PrintAll()
        {
            this.proxy.PrintAll();
        }

        public void SetWorkflowName(string name)
        {
            this.workflowName = name;
            this.proxy.SetWorkflowName(name);
        }

        public void Save(string targetFileName)
        {
            this.proxy.Save(targetFileName);
        }

        public void SaveWorkflowToBitmap()
        {
            this.proxy.SaveWorkflowToBitmap();
        }

        public void SaveWorkflowToBitmap(string targetFile)
        {
            this.proxy.SaveWorkflowToBitmap(targetFile);
        }

        public void RefreshDesignerFromXamlCode()
        {
            this.proxy.RefreshDesignerFromXamlCode();
        }

        public void ImportAssemblies(IEnumerable<ActivityAssemblyItem> importAssemblies, bool canRefresh = true)
        {
            if (!this.proxy.ImportAssemblies(importAssemblies, canRefresh))
            {
                this.ReloadAddIn();
            }
        }

        public bool IsValid()
        {
            return this.proxy.IsWorkflowValid();
        }

        /// <summary>
        /// reload AddIn
        /// </summary>
        public void ReloadAddIn()
        {
            string xaml = this.XamlCode;
            List<ActivityAssemblyItem> dependencies = new List<ActivityAssemblyItem>(this.DependencyAssemblies);
            this.UnloadAddIn();
            this.InitAddIn(this.workflowName, xaml, dependencies, this.isTask);
            this.OnDesignerReloaded(this, new EventArgs());
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void FinishTaskAssigned()
        {
            this.proxy.FinishTaskAssigned();
        }

        public void SetNewTasksToAssigned(Guid[] ids)
        {
            proxy.SetNewTasksToAssigned(ids);
        }

        public void RollbackAssignedTasks(Guid[] ids)
        {
            proxy.RollbackAssignedTasks(ids);
        }

        #region Private methods

        private void InitAddIn(string name, string projectXamlCode, List<ActivityAssemblyItem> references, bool isTask)
        {
            this.proxy = AddinPreloader.Current.Get();
            this.proxy.InitWorkflow(name, projectXamlCode, isTask);
            if (references != null && references.Any())
            {
                this.proxy.ImportAssemblies(references);
            }
            this.proxy.RefreshTasks();
            this.proxy.DesignerChanged += this.OnDesignerChanged;
            this.proxy.DesignerReloaded += this.OnDesignerReloaded;
            this.proxy.PrintStateChanged += this.OnPrintStateChanged;
            this.proxy.GetTaskLastVersionChanged += this.GetTaskLastVersion;
        }

        private void GetTaskLastVersion(object sender, GetTaskEventArgs e)
        {
            if (this.GetTaskLastVersionChanged != null)
            {
                this.GetTaskLastVersionChanged(this, e);
            }
        }

        private void OnDesignerChanged(object sender, EventArgs e)
        {
            if (this.DesignerChanged != null)
            {
                this.DesignerChanged(this, e);
            }
        }

        private void OnDesignerReloaded(object sender, EventArgs e)
        {
            if (this.DesignerReloaded != null)
            {
                this.DesignerReloaded(this, e);
            }
        }

        private void OnPrintStateChanged(object sender, EventArgs e)
        {
            if (this.PrintStateChanged != null)
            {
                this.PrintStateChanged(this, e);
            }
        }
        #endregion
    }
}
