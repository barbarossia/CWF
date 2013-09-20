using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.CompositeActivity;
using Microsoft.Support.Workflow.Authoring.Services;

namespace Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor
{
    public static class TaskService
    {
        public static TaskActivityDC GetLastVersionTaskActivityDC(Guid taskGuid)
        {
            TaskActivityDC result = WorkflowsQueryServiceUtility.UsingClientReturn(client => client.TaskActivityGet(new TaskActivityDC()
            {
                Guid = taskGuid,
            }.CommonHeaderSetIncaller()));

            result.StatusReply.CheckErrors();

            return result;
        }

        public static IEnumerable<TaskActivityDC> GetLastVersionTaskActivityDC(Guid[] taskGuids)
        {
            var result = WorkflowsQueryServiceUtility.UsingClientReturn(client => client.TaskActivityGetList(new TaskActivityGetListRequest()
            {
                List = taskGuids.Select(t => new TaskActivityDC() { Guid = t }).ToList(),
            }.SetIncaller()));

            result.StatusReply.CheckErrors();

            return result.List;
        }

        public static TaskActivity CreateTaskActivity(this Activity body)
        {
            Contract.Requires(body != null);

            return new TaskActivity(body);
        }

        public static TaskActivity CreateTaskActivity(this TaskActivity task, ModelItem body)
        {
            Contract.Requires(body != null);

            return new TaskActivity(
                task.Alias,
                task.TaskId,
                CompositeService.GetBody(body),
                task.Status,
                task.DisplayName);
        }
    }
}
