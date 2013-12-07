using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DataContracts;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using CWF.DAL;
using System.Diagnostics;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    /// <summary>
    /// Defines the data access services associated with WorkflowType table as the primary table.
    /// </summary>
    public static class WorkflowTypeRepositoryService
    {

        /// <summary>
        /// If the parameter request.InId is valid, it is an update, otherwise it is a create operation.
        /// </summary>
        /// <param name="request">WorkFlowTypeCreateOrUpdateRequestDC object</param>
        /// <returns>WorkFlowTypeCreateOrUpdateReplyDC object</returns>
        public static WorkFlowTypeCreateOrUpdateReplyDC WorkflowTypeCreateOrUpdate(WorkFlowTypeCreateOrUpdateRequestDC request)
        {
            var reply = new WorkFlowTypeCreateOrUpdateReplyDC();
            SqlDatabase db = null;
            DbCommand cmd = null;
            int retValue = 0;
            string outErrorString = string.Empty;
            try
            {
                db = RepositoryHelper.CreateDatabase();
                cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.WorkflowTypeCreateOrUpdate);
                
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InHandleVariable", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InHandleVariable);
                db.AddParameter(cmd, "@InAuthGroupName", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroupNames));              
                db.AddParameter(cmd, "@InId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.InId);
                db.AddParameter(cmd, "@InGuid", DbType.Guid, ParameterDirection.Input, null, DataRowVersion.Default, request.InGuid);
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InName);
                db.AddParameter(cmd, "@InPageViewVariable", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InPageViewVariable);
                db.AddParameter(cmd, "@InPublishingWorkflowId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.InPublishingWorkflowId);
                db.AddParameter(cmd, "@InSelectionWorkflowId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.InSelectionWorkflowId);
                db.AddParameter(cmd, "@InWorkflowTemplateId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.InWorkflowTemplateId);
                db.AddParameter(cmd, "@InAuthGroupId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.InAuthGroupId);
                db.AddParameter(cmd, "@InSoftDelete", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.IsDeleted);
                db.AddParameter(cmd, "@InInsertedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InInsertedByUserAlias);
                db.AddParameter(cmd, "@InUpdatedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InUpdatedByUserAlias);
                db.AddParameter(cmd, "@InEnvironmentTarget", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Environment);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                db.ExecuteScalar(cmd);
                retValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                outErrorString = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                if (retValue != 0)
                {
                    reply.StatusReply.ErrorMessage = outErrorString;
                    reply.StatusReply.Errorcode = retValue;
                }
            }
            catch (SqlException ex)
            {
                ex.HandleException();
            }

            return reply;
        }

        /// <summary>
        /// Get workflow types either by Id or by Name.  If none is specified, returns all the 
        /// workflow types.
        /// </summary>
        /// <param name="id">Workflow type ID.</param>
        /// <param name="name">Workflow type name.</param>
        /// <returns>Reply that contains the list of workflow types found.</returns>
        public static WorkflowTypeGetReplyDC GetWorkflowTypes(WorkflowTypesGetRequestDC request)
        {
            WorkflowTypeGetReplyDC reply = new WorkflowTypeGetReplyDC();
            IList<WorkflowTypesGetBase> wftList = new List<WorkflowTypesGetBase>();
            int retValue = 0;
            string outErrorString = string.Empty;

            try
            {
                WorkflowTypesGetBase wfat = null;

                SqlDatabase db = RepositoryHelper.CreateDatabase();
                DbCommand cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.WorkflowTypeGet);

                db.AddParameter(cmd, "@InEnvironmentName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Environment);
                db.AddParameter(cmd, StoredProcParamNames.Id, DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.Id);
                db.AddParameter(cmd, StoredProcParamNames.Name, DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Name);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        wfat = new WorkflowTypesGetBase();
                        wfat.AuthGroupId = Convert.ToInt32(reader[DataColumnNames.AuthGroupId]);
                        wfat.AuthGroupName = reader[DataColumnNames.AuthGroupName] != DBNull.Value ? Convert.ToString(reader[DataColumnNames.AuthGroupName]) : string.Empty;
                        wfat.Guid = (Guid)reader[DataColumnNames.Guid];
                        wfat.HandleVariableId = reader[DataColumnNames.HandleVariableId] != DBNull.Value
                            ? Convert.ToInt32(reader[DataColumnNames.HandleVariableId]) : 0;
                        wfat.Id = Convert.ToInt32(reader[DataColumnNames.Id]);
                        wfat.Name = Convert.ToString(reader[DataColumnNames.Name]);
                        wfat.PageViewVariableId = reader[DataColumnNames.PageViewVariableId] != DBNull.Value
                            ? Convert.ToInt32(reader[DataColumnNames.PageViewVariableId]) : 0;
                        wfat.PublishingWorkflow = reader[DataColumnNames.PublishingWorkflowName] != DBNull.Value
                            ? Convert.ToString(reader[DataColumnNames.PublishingWorkflowName]) : string.Empty;

                        wfat.PublishingWorkflowVersion = reader[DataColumnNames.PublishingWorkflowVersion] != DBNull.Value
                            ? Convert.ToString(reader[DataColumnNames.PublishingWorkflowVersion]) : string.Empty;

                        wfat.PublishingWorkflowId = reader[DataColumnNames.PublishingWorkflowId] != DBNull.Value
                            ? Convert.ToInt32(reader[DataColumnNames.PublishingWorkflowId]) : 0;
                        wfat.SelectionWorkflowId = reader[DataColumnNames.SelectionWorkflowId] != DBNull.Value
                            ? Convert.ToInt32(reader[DataColumnNames.SelectionWorkflowId]) : 0;
                        wfat.WorkflowTemplate = reader[DataColumnNames.WorkflowTemplateName] != DBNull.Value
                            ? Convert.ToString(reader[DataColumnNames.WorkflowTemplateName]) : string.Empty;
                        wfat.WorkflowTemplateId = reader[DataColumnNames.WorkflowTemplateId] != DBNull.Value
                            ? Convert.ToInt32(reader[DataColumnNames.WorkflowTemplateId]) : 0;
                        wfat.Environment = Convert.ToString(reader[DataColumnNames.Environment]);
                        wftList.Add(wfat);
                    }

                    reply.WorkflowActivityType = wftList;

                    retValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                    outErrorString = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                    if (retValue != 0)
                    {
                        reply.StatusReply.ErrorMessage = outErrorString;
                        reply.StatusReply.Errorcode = retValue;
                    }
                }
            }
            catch (SqlException e)
            {
                e.HandleException();
            }

            return reply;
        }

        /// <summary>
        /// Search workflowtypes 
        /// </summary>
        /// <returns></returns>
        public static WorkflowTypeSearchReply SearchWorkflowTypes(WorkflowTypeSearchRequest request)
        {
            WorkflowTypeSearchReply reply = new WorkflowTypeSearchReply();
            IList<WorkflowTypeSearchDC> wftList = new List<WorkflowTypeSearchDC>();
            int retValue = 0;
            string outErrorString = string.Empty;

            try 
            {
                WorkflowTypeSearchDC wfat = null;

                SqlDatabase db = RepositoryHelper.CreateDatabase();
                DbCommand cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.WorkflowType_Search);

                db.AddParameter(cmd, "@InAuthGroupName", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroupNames));           
                db.AddParameter(cmd, "@SearchText", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.SearchText);
                db.AddParameter(cmd, "@SortColumn", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.SortColumn);
                db.AddParameter(cmd, "@SortAscending", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.SortAscending);
                db.AddParameter(cmd, "@PageSize", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.PageSize);
                db.AddParameter(cmd, "@PageNumber", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.PageNumber);
                db.AddParameter(cmd, "@InEnvironments", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetEnvironments(request.Environments));
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        wfat =new WorkflowTypeSearchDC();

                        wfat.Guid = reader[DataColumnNames.Guid] != DBNull.Value ?(Guid)reader[DataColumnNames.Guid] : Guid.Empty;

                        wfat.Id = Convert.ToInt32(reader[DataColumnNames.Id]);
                        
                        wfat.Name = Convert.ToString(reader[DataColumnNames.Name]);

                        wfat.AuthGroupId = Convert.ToInt32(reader[DataColumnNames.AuthGroupId]);

                        wfat.AuthGroupName = reader[DataColumnNames.AuthGroupName] != DBNull.Value ? Convert.ToString(reader[DataColumnNames.AuthGroupName]) : string.Empty;
                      
                        wfat.PublishingWorkflow = reader[DataColumnNames.PublishingWorkflowName] != DBNull.Value
                            ? Convert.ToString(reader[DataColumnNames.PublishingWorkflowName]) : string.Empty;

                        wfat.PublishingWorkflowVersion = reader[DataColumnNames.PublishingWorkflowVersion] != DBNull.Value
                            ? Convert.ToString(reader[DataColumnNames.PublishingWorkflowVersion]) : string.Empty;

                        wfat.PublishingWorkflowId = reader[DataColumnNames.PublishingWorkflowId] != DBNull.Value
                            ? Convert.ToInt32(reader[DataColumnNames.PublishingWorkflowId]) : 0;

                        wfat.WorkflowTemplate = reader[DataColumnNames.WorkflowTemplateName] != DBNull.Value
                            ? Convert.ToString(reader[DataColumnNames.WorkflowTemplateName]) : string.Empty;

                        wfat.WorkflowTemplateVersion = reader[DataColumnNames.WorkflowTemplateVersion] != DBNull.Value
                            ? Convert.ToString(reader[DataColumnNames.WorkflowTemplateVersion]) : string.Empty;

                        wfat.WorkflowTemplateId = reader[DataColumnNames.WorkflowTemplateId] != DBNull.Value
                            ? Convert.ToInt32(reader[DataColumnNames.WorkflowTemplateId]) : 0;

                        wfat.WorkflowsCount = reader[DataColumnNames.WorkflowsCount] != DBNull.Value
                            ? Convert.ToInt32(reader[DataColumnNames.WorkflowsCount]) : 0;

                        wfat.Environment = Convert.ToString(reader[DataColumnNames.Environment]);
                        
                        wftList.Add(wfat);
                    }

                    reply.SearchResults = wftList;
                    reader.NextResult();
                    if (reader.Read())
                    {
                        reply.ServerResultsLength = Convert.ToInt32(reader["Total"]);
                    }

                    retValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                    outErrorString = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                    if (retValue != 0)
                    {
                        reply.StatusReply.ErrorMessage = outErrorString;
                        reply.StatusReply.Errorcode = retValue;
                    }
                }
            }
            catch (SqlException e)
            {
                e.HandleException();
            }

            return reply;
        }
    }
}
