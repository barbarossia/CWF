using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Service.DataAccessServices;
using CWF.DAL;

namespace Microsoft.Support.Workflow.Service.BusinessServices
{
    public static class WorkflowTypeBusinessService
    {

        /// <summary>
        /// Performs input validation (if any) and gets workflow types either by Id or by Name.  
        /// If none is specified, returns all the workflow types.
        /// </summary>
        /// <param name="id">Workflow type ID.</param>
        /// <param name="name">Workflow type name.</param>
        /// <returns>Reply that contains the list of workflow types found.</returns>
        public static WorkflowTypeGetReplyDC GetWorkflowTypes(WorkflowTypesGetRequestDC request)
        {
            WorkflowTypeGetReplyDC reply = null;
            try
            {
                reply = WorkflowTypeRepositoryService.GetWorkflowTypes(request);
            }
            catch (Exception e)
            {
                throw new BusinessException(-1, e.Message);
            }

            return reply;
        }

        /// <summary>
        /// Search workflow types
        /// If none is specified, returns all the workflow types.
        /// </summary>
        /// <returns>Reply that contains the list of workflow types found.</returns>
        public static WorkflowTypeSearchReply SearchWorkflowTypes(WorkflowTypeSearchRequest request)
        {
            WorkflowTypeSearchReply reply = null;
            try
            {
                reply = WorkflowTypeRepositoryService.SearchWorkflowTypes(request);
            }
            catch (Exception e)
            {
                throw new BusinessException(-1, e.Message);
            }

            return reply;
        }

        /// <summary>
        /// Add worklfow type or edit workflow type
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static WorkFlowTypeCreateOrUpdateReplyDC WorkflowTypeCreateOrUpdate(WorkFlowTypeCreateOrUpdateRequestDC request)
        {
            WorkFlowTypeCreateOrUpdateReplyDC reply = null;
            try
            {
                reply = WorkflowTypeRepositoryService.WorkflowTypeCreateOrUpdate(request);
            }
            catch (Exception e)
            {
                throw new BusinessException(-1, e.Message);
            }
            return reply;
        }
    }
}
