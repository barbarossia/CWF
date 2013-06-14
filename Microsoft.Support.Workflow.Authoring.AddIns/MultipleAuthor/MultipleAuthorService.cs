using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reflection;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.AddIns.CompositeActivity;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.CompositeActivity;
using Microsoft.Support.Workflow.Authoring.Services;

namespace Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor
{
    /// <summary>
    /// Help class for multiple author
    /// </summary>
    public static class MultipleAuthorService
    {
        public static List<TaskAssignment> GetTasks(WorkflowDesigner designer)
        {
            Contract.Requires(designer != null);

            return FindTaskActivity(designer)
                .Where(m => !string.IsNullOrEmpty(((TaskActivity)m.GetCurrentValue()).Alias))
                .Select(m => new TaskAssignment()
                {
                    TaskId = ((TaskActivity)m.GetCurrentValue()).TaskId,
                    AssignTo = ((TaskActivity)m.GetCurrentValue()).Alias,
                    TaskStatus = ((TaskActivity)m.GetCurrentValue()).Status,
                    Xaml = GetXamlOfTaskBody(designer, m),
                }).ToList();
        }

        public static void SetNewTasksToAssigned(WorkflowDesigner designer, Guid[] ids) {
            foreach (ModelItem m in FindTaskActivity(designer)) {
                TaskActivity task = (TaskActivity)m.GetCurrentValue();
                if (ids.Contains(task.TaskId)) {
                    m.Properties["Status"].SetValue(TaskActivityStatus.Assigned);
                }
            }
        }

        public static void RollbackAssignedTasks(WorkflowDesigner designer, Guid[] ids) {
            foreach (ModelItem m in FindTaskActivity(designer)) {
                TaskActivity task = (TaskActivity)m.GetCurrentValue();
                if (ids.Contains(task.TaskId)) {
                    m.Properties["Status"].SetValue(TaskActivityStatus.New);
                }
            }
        }

        public static List<TaskItem> GetTaskItems(WorkflowDesigner designer) {
            Contract.Requires(designer != null);

            return FindTaskActivity(designer).Select(m => new TaskItem(m)).ToList();
        }

        public static void FinishTaskAssigned(WorkflowDesigner designer)
        {
            Contract.Requires(designer != null);

            CompositeService.UpdateModelItem(FindTaskActivity(designer).ToDictionary(
                m => m, m =>
                {
                    var task = m.GetTaskActivity();
                    if (task.Status == TaskActivityStatus.New && !string.IsNullOrEmpty(task.Alias))
                    {
                        m.Properties["Status"].SetValue(TaskActivityStatus.Assigned);
                    }
                    return new TaskActivity(task.Group, task.Alias, task.TaskId, task.TaskBody, task.Status, task.DisplayName) as Activity;
                }));
        }

        public static IEnumerable<ModelItem> FindTaskActivity(WorkflowDesigner workflowDesigner)
        {
            Contract.Requires(workflowDesigner != null);

            var root = workflowDesigner.GetRoot();
            if (root != null)
                return ModelItemService.Find(
                    root,
                    (modelItemType) => typeof(TaskActivity).IsAssignableFrom(modelItemType));

            return new List<ModelItem>();
        }

        public static bool CanUnassign(ModelItem source)
        {
            Contract.Requires(source != null);

            var task = source.GetTaskActivity();
            return task.Status != TaskActivityStatus.Editing;
        }

        public static bool CanMerge(ModelItem source)
        {
            Contract.Requires(source != null);

            var task = source.GetTaskActivity();
            return task.Status == TaskActivityStatus.CheckedIn;
        }

        public static bool CheckIsTask(ModelItem source)
        {
            Contract.Requires(source != null);

            return source.GetTaskActivity() != null ? true : false;
        }

        public static bool CanAssign(ModelItem source)
        {
            Contract.Requires(source != null);

            return !CheckIsTask(source) &&
                !CompositeService.GetParents(source).Any(m => m.GetTaskActivity() != null);
        }

        public static void Assign(ModelItem selected)
        {
            Contract.Requires(selected != null);

            CompositeService.UpdateModelItem(selected, selected.GetActivity().CreateTaskActivity());
        }

        public static void GetLastVersion(ModelItem source, WorkflowEditorViewModel workflowItem)
        {
            Contract.Requires(source != null);
            Contract.Requires(workflowItem != null);

            UpdateTaskItem(source, workflowItem);
        }

        public static void GetAllLastVersion(IEnumerable<ModelItem> tasks, WorkflowEditorViewModel workflowItem)
        {
            Contract.Requires(workflowItem != null);
            Contract.Requires(workflowItem.WorkflowDesigner != null);

            UpdateTaskItem(tasks, workflowItem);
        }

        public static void UnassignTask(ModelItem source)
        {
            Contract.Requires(source != null);

            var updatedBody = source.GetTaskActivity().TaskBody;
            if (updatedBody != null)
            {
                CompositeService.UpdateModelItem(source, updatedBody);
            }
            else
            {
                CompositeService.DeleteModelItem(source);
            }
        }

        public static void UnassignAllTask(WorkflowDesigner designer)
        {
            Contract.Requires(designer != null);

            var allTasks = FindTaskActivity(designer);
            allTasks.ToList().ForEach(o => UnassignTask(o));
        }

        private static string GetXamlOfTaskBody(WorkflowDesigner rootDesigner, ModelItem taskItem)
        {
            TaskActivity taskActivity = taskItem.GetTaskActivity();
            WorkflowDesigner bodyDesigner = CompositeService.CreateWorkflowDesigner(taskActivity.TaskBody, 
                new TaskAssignment() { TaskId = taskActivity.TaskId }.GetFriendlyName(((ActivityBuilder)rootDesigner.GetRoot().GetCurrentValue()).Name));
            ModelItem rootItem = rootDesigner.GetRoot();
            ModelItem bodyItem = bodyDesigner.GetRoot();

            ArgumentService.AddArguments(bodyItem, rootItem);
            List<Variable> variables = ArgumentService.GetAvailableVariables(taskItem).ToList();
            ArgumentService.AddArguments(bodyItem,
                variables.Select(v => new DynamicActivityProperty(){
                    Name = v.Name, 
                    Type = typeof(InOutArgument<>).MakeGenericType(v.Type),
                }));

            return bodyDesigner.CompilableXaml();
        }

        private static ModelItem GetLastVersionActivity(ModelItem source, WorkflowEditorViewModel workflowItem)
        {
            TaskActivity task = source.GetTaskActivity();
            var result = TaskService.GetLastVersionTaskActivityDC(task.TaskId);

            workflowItem.DownloadTaskDependency(result);

            return CompositeService.CreateActivity(result.Activity.Xaml);
        }

        private static IDictionary<ModelItem, ModelItem> GetLastVersionActivity(IEnumerable<ModelItem> tasks, WorkflowEditorViewModel workflowItem)
        {
            var result = TaskService.GetLastVersionTaskActivityDC(tasks.Select(t => t.GetTaskActivity().TaskId).ToArray());

            workflowItem.DownloadTaskDependency(result);

            return (from t in tasks
                    from r in result
                    let ta = t.GetTaskActivity()
                    where ta.TaskId == r.Guid
                    select new
                    {
                        Key = t,
                        Value = CompositeService.CreateActivity(r.Activity.Xaml),
                    }).ToDictionary(d => d.Key, d => d.Value);
        }

        private static void UpdateTaskItem(ModelItem source, WorkflowEditorViewModel workflowItem)
        {
            ModelItem taskModelItem = GetLastVersionActivity(source, workflowItem);
            MergeTaskArgmentToParent(taskModelItem, workflowItem.WorkflowDesigner.GetRoot(), source);
            CompositeService.UpdateModelItem(source, source.GetTaskActivity().CreateTaskActivity(taskModelItem));         
        }

        private static void UpdateTaskItem(IEnumerable<ModelItem> source, WorkflowEditorViewModel workflowItem)
        {
            var taskModelItem = GetLastVersionActivity(source, workflowItem);
            MergeAllTaskArgmentToParent(
                taskModelItem,
                workflowItem.WorkflowDesigner.GetRoot());

            CompositeService.UpdateModelItem(taskModelItem.ToDictionary(
                t => t.Key,
                t => t.Key.GetTaskActivity().CreateTaskActivity(t.Value) as Activity));
        }

        private static void MergeTaskArgmentToParent(ModelItem taskItem, ModelItem rootItem, ModelItem selected)
        {
            var taskArgments = ArgumentService.GetArgument(taskItem.GetCurrentValue() as ActivityBuilder);
            var rootArgments = ArgumentService.GetArgument(rootItem.GetCurrentValue() as ActivityBuilder);
            List<Variable> variables = ArgumentService.GetAvailableVariables(selected).ToList();

            DoSameNameVariables(
                taskArgments.Except(rootArgments, new DynamicActivityPropertyEqualityComparer()),
                variables,
                taskItem,
                rootItem,
                selected);
        }

        private static void MergeAllTaskArgmentToParent(IDictionary<ModelItem, ModelItem> taskItems, ModelItem rootItem)
        {
            taskItems.ToList().ForEach(t => MergeTaskArgmentToParent(t.Value, rootItem, t.Key));
        }

        private static void DoSameNameVariables(
            IEnumerable<DynamicActivityProperty> arguments,
            IEnumerable<Variable> variables,
            ModelItem task,
            ModelItem root,
            ModelItem selected)
        {
            var existsVars = from a in arguments
                             join v in variables
                             on a.Name equals v.Name
                             select new
                             {
                                 Arg = a,
                                 Var = v
                             };

            var notExists = arguments.Except(
                existsVars.Select(e => e.Arg), new DynamicActivityPropertyEqualityComparer());

            var parent = ArgumentService.GetAvailableParent(selected);
            if (parent != null)
            {
                ArgumentService.AddVariable(parent, notExists.Select(n => ArgumentService.CovertArgumentToVariable(n)));
            }
            else
            {
                foreach (var arg in notExists)
                {
                    root.Properties[ArgumentService.Name_Prpoerty].Collection.Add(arg);
                }
            }
        }
    }
}
