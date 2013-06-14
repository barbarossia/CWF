using System;
using System.AddIn.Contract;
using System.Collections.Generic;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
using Microsoft.Support.Workflow.Authoring.Behaviors;

namespace Microsoft.Support.Workflow.Authoring.AddIns
{
    /// <summary>
    /// Defines the services that an add-in will provide to a host application
    /// </summary>
    public interface IDesignerContract
    {
        List<TaskAssignment> Tasks { get; }
        void SetNewTasksToAssigned(Guid[] ids);
        void RollbackAssignedTasks(Guid[] ids);

        //Get designer views
        INativeHandleContract WorkflowEditorView { get; }
        INativeHandleContract ProjectExplorerView { get; }
        INativeHandleContract WorkflowPropertyView { get; }
        INativeHandleContract ToolboxView { get; }
        
        //designer changed event
        event EventHandler DesignerChanged;
        event EventHandler DesignerReloaded;
        event EventHandler PrintStateChanged;
        event GetTaskLastVersionEventHandler GetTaskLastVersionChanged;
        void WorkflowDesignerChanged(object sender, EventArgs e);

        string CompiledXaml { get; }
        bool IsWorkflowService();
        bool IsWorkflowValid();
        string Xaml { get; }
        PrintAction PrintState { get; }

        IEnumerable<ActivityAssemblyItem> DependencyAssemblies { get; }
        CompileProject CompileProject { get; }

        //operate workflow
        void SetReadOnly(bool isReadOnly);
        void SetWorkflowName(string name);
        void InitWorkflow(string name, string xaml, bool isTask);

        //commands
        void Undo();
        void Redo();
        void Cut();
        void Copy();
        void Paste();
        void Print();
        void PrintAll();

        bool CanUndo();
        bool CanRedo();
        bool CanCut();
        bool CanCopy();
        bool CanPaste();

        void SaveWorkflowToBitmap();
        void SaveWorkflowToBitmap(string targetFile);
        void Save(string targetFileName);
        void RefreshDesignerFromXamlCode();
        void FinishTaskAssigned();
        void Close();
        bool ImportAssemblies(IEnumerable<ActivityAssemblyItem> importedAssemblies, bool canRefresh = true);
        void RefreshTasks();

    }
}
