using System;
using System.Activities;
using System.ComponentModel;
using Microsoft.Support.Workflow.Authoring.CompositeActivity;
using System.Linq;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.Security;
using System.Activities.Validation;

namespace Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor
{
    /// <summary>
    /// An activity for creating a activity can assign task to special user.
    /// </summary>
    [Designer(typeof(TaskActivityDesigner))]
    public sealed class TaskActivity : NativeActivity
    {
        [Browsable(false)]
        public string Group { get; set; }
        [Browsable(false)]
        public Guid TaskId { get; set; }
        [Browsable(false)]
        public string Alias { get; set; }
        [Browsable(false)]
        public Activity TaskBody { get; set; }
        [Browsable(false)]
        public TaskActivityStatus Status { get; set; }

        /// <summary>
        /// Construct
        /// </summary>
        public TaskActivity()
            : this(null)
        {
        }

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="body">The assign activity</param>
        public TaskActivity(Activity body)
            : this(null, null, Guid.Empty, body, TaskActivityStatus.New, null)
        {
        }

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="group">The user group from AD by assigned</param>
        /// <param name="alias">The user alias from AD by assigned</param>
        /// <param name="taskId">Task indentity</param>
        /// <param name="body">The assign activity</param>
        public TaskActivity(string group, string alias, Guid taskId, Activity body, TaskActivityStatus status, string name) {
            Group = group;
            Alias = alias;
            TaskId = taskId;
            TaskBody = body;
            Status = status;

            if (string.IsNullOrWhiteSpace(Group)) {
                Group = AuthorizationService.SecurityLevelMaps.First(p => p.Value == SecurityLevel.Author).Key;
            }
            if (TaskId == Guid.Empty) {
                TaskId = Guid.NewGuid();
            }
            if (!string.IsNullOrEmpty(name)) {
                DisplayName = name;
            }

            if (ContainsTaskActivity(body)) {
                throw new TaskActivityAssignException();
            }
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            if (this.TaskBody != null)
            {
                metadata.AddChild(this.TaskBody);
                if (ContainsTaskActivity(this.TaskBody))
                {
                    metadata.AddValidationError(typeof(TaskActivityAssignException).Name);
                }
            }
        } 

        /// <summary>
        /// Performs the execution of the activity
        /// </summary>
        /// <param name="context">The execution context under which the activity executes</param>
        protected override void Execute(NativeActivityContext context)
        {
            throw new NotImplementedException();
        }

        private bool ContainsTaskActivity(Activity body)
        {
            if (body == null)
            {
                return false;
            }

            return CompositeService.GetActivities(body).Any(c => c is TaskActivity);
        }
    }
}
