using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Service.DataAccessServices;
using CWF.DAL;

namespace Microsoft.Support.Workflow.Service.BusinessServices
{
    public static class TaskActivityBusinessService
    {
        public static TaskActivityGetReplyDC GetTaskActivities(TaskActivityGetRequestDC request)
        {
            TaskActivityGetReplyDC reply = new TaskActivityGetReplyDC();
            try
            {
                request.ValidateRequest();
                reply = TaskActivityRepositoryService.SearchTaskActivities(request);
            }
            catch (Exception e)
            {
                throw new BusinessException(-1, e.Message);
            }

            return reply;
        }

        public static TaskActivityDC TaskActivityGet(TaskActivityDC request)
        {
            TaskActivityDC reply = new TaskActivityDC();
            try
            {
                reply = TaskActivityRepositoryService.TaskActivityGet(request);
            }
            catch (Exception e)
            {
                throw new BusinessException(-1, e.Message);
            }

            return reply;
        }

        public static TaskActivityDC TaskActivityUpdateStatus(TaskActivityDC request)
        {
            TaskActivityDC reply = new TaskActivityDC();
            try
            {
                reply = TaskActivityRepositoryService.TaskActivity_SetStatus(request);
            }
            catch (Exception e)
            {
                throw new BusinessException(-1, e.Message);
            }

            return reply;
        }

        public static TaskActivityDC TaskActivityCreateOrUpdate(TaskActivityDC request)
        {
            TaskActivityDC reply = new TaskActivityDC();
            try
            {
                reply = TaskActivityRepositoryService.TaskActivitiesCreateOrUpdate(request);
            }
            catch (Exception e)
            {
                throw new BusinessException(-1, e.Message);
            }

            return reply;
        }

        public static TaskActivityGetListReply TaskActivityGetList(TaskActivityGetListRequest request)
        {
            TaskActivityGetListReply reply = new TaskActivityGetListReply();
            try
            {
                reply.List = new List<TaskActivityDC>();
                if (request.List != null)
                {
                    foreach (var ta in request.List)
                    {
                        ta.Incaller = request.Incaller;
                        ta.IncallerVersion = request.IncallerVersion;
                        var result = TaskActivityRepositoryService.TaskActivityGet(ta);
                        if (result != null)
                            reply.List.Add(result);
                    }
                }
            }
            catch (Exception e)
            {
                throw new BusinessException(-1,e.Message);
            }
            return reply;
        }
    }
}
