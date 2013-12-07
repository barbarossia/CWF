namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using CWF.DAL;
    using CWF.DataContracts;
    using Practices.EnterpriseLibrary.Data;
    using Microsoft.Support.Workflow.Service.Contracts;
    using System.Data.SqlClient;
    using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

    public class AuthorizationGroup
    {
        /// <summary>
        /// Get AuthorizationGroup
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static AuthorizationGroupGetReplyDC GetAuthorizationGroups(AuthorizationGroupGetRequestDC request)
        {
            AuthorizationGroupGetReplyDC reply = new AuthorizationGroupGetReplyDC();
            IList<AuthorizationGroupDC> agList = new List<AuthorizationGroupDC>();
            try
            {
                AuthorizationGroupDC ag = null;

                SqlDatabase db = RepositoryHelper.CreateDatabase();
                DbCommand cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.AuthorizationGroupGet);

                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.AuthGroupId);
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.AuthGroupName);
                db.AddParameter(cmd, "@InGuid", DbType.Guid, ParameterDirection.Input, null, DataRowVersion.Default, request.Guid);
                db.AddParameter(cmd, "@inRoleId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.RoleId);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        ag = new AuthorizationGroupDC();
                        ag.Guid = (Guid)reader[DataColumnNames.Guid];
                        ag.AuthGroupId = reader[DataColumnNames.Id] != DBNull.Value
                            ? Convert.ToInt32(reader[DataColumnNames.Id]) : 0;
                        ag.AuthGroupName = reader[DataColumnNames.Name] != DBNull.Value
                            ? Convert.ToString(reader[DataColumnNames.Name]) : string.Empty;
                        ag.RoleId = reader["RoleId"] != DBNull.Value
                            ? Convert.ToInt32(reader["RoleId"]) : 0;
                        ag.Enabled = Convert.ToBoolean(reader["Enabled"]);
                        agList.Add(ag);
                    }
                }
                reply.AuthorizationGroups = agList;
            }
            catch (SqlException e)
            {
                e.HandleException();
            }

            return reply;
        }

        /// <summary>
        /// If the parameter request.InId is valid, it is an update, otherwise it is a create operation.
        /// </summary>
        /// <param name="request">AuthGroupsCreateOrUpdateRequestDC object</param>
        /// <returns>AuthGroupsCreateOrUpdateReplyDC object</returns>
        public static AuthGroupsCreateOrUpdateReplyDC AuthGroupsCreateOrUpdate(AuthGroupsCreateOrUpdateRequestDC request)
        {
            AuthGroupsCreateOrUpdateReplyDC reply = new AuthGroupsCreateOrUpdateReplyDC();
            StatusReplyDC status = new StatusReplyDC();
            SqlDatabase db = null;
            DbCommand cmd = null;
            int retValue = 0;
            string outErrorString = string.Empty;
            try
            {
                db = RepositoryHelper.CreateDatabase();
                cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.AuthorizationGroupCreateOrUpdate);
                
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InAuthGroupName", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroupNames));
                db.AddParameter(cmd, "@InAuthGroups", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroups));
                db.AddParameter(cmd, "@inRoleId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.RoleId);
                db.AddParameter(cmd, "@InInsertedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InInsertedByUserAlias);
                db.AddParameter(cmd, "@InUpdatedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InUpdatedByUserAlias);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                db.ExecuteScalar(cmd);
                retValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                outErrorString = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                if (retValue != 0)
                {
                    status.ErrorMessage = outErrorString;
                    status.Errorcode = retValue;
                    Logging.Log(retValue,
                                EventLogEntryType.Error,
                                SprocValues.AUTHGROUPS_CREATE_OR_UPDATE_CALL_ERROR_MSG,
                                outErrorString);
                }
            }
            catch (Exception ex)
            {
                status.ErrorMessage = SprocValues.AUTHGROUPS_CREATE_OR_UPDATE_CALL_ERROR_MSG + ex.Message;
                status.Errorcode = SprocValues.GENERIC_CATCH_ID;
                Logging.Log(SprocValues.GENERIC_CATCH_ID,
                            EventLogEntryType.Error,
                            SprocValues.AUTHGROUPS_CREATE_OR_UPDATE_CALL_ERROR_MSG,
                            ex);
            }

            reply.StatusReply = status;
            return reply;
        }

        /// <summary>
        /// If the parameter request.InId is valid, it is an update, otherwise it is a create operation.
        /// </summary>
        /// <param name="request">AuthGroupsCreateOrUpdateRequestDC object</param>
        /// <returns>AuthGroupsCreateOrUpdateReplyDC object</returns>
        public static AuthGroupsEnableOrDisableReplyDC AuthGroupsEnableOrDisable(AuthGroupsEnableOrDisableRequestDC request)
        {
            AuthGroupsEnableOrDisableReplyDC reply = new AuthGroupsEnableOrDisableReplyDC();
            StatusReplyDC status = new StatusReplyDC();
            SqlDatabase db = null;
            DbCommand cmd = null;
            int retValue = 0;
            string outErrorString = string.Empty;
            try
            {
                db = RepositoryHelper.CreateDatabase();
                cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.AuthorizationGroupEnableOrDisable);
                
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InAuthGroupName", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroupNames));
                db.AddParameter(cmd, "@InAuthGroups", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroups));
                db.AddParameter(cmd, "@inEnabled", DbType.Byte, ParameterDirection.Input, null, DataRowVersion.Default, request.InEnabled);
                db.AddParameter(cmd, "@InInsertedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InInsertedByUserAlias);
                db.AddParameter(cmd, "@InUpdatedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InUpdatedByUserAlias);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                db.ExecuteScalar(cmd);
                retValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                outErrorString = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                if (retValue != 0)
                {
                    status.ErrorMessage = outErrorString;
                    status.Errorcode = retValue;
                    Logging.Log(retValue,
                                EventLogEntryType.Error,
                                SprocValues.AUTHGROUPS_CREATE_OR_UPDATE_CALL_ERROR_MSG,
                                outErrorString);
                }
            }
            catch (Exception ex)
            {
                status.ErrorMessage = SprocValues.AUTHGROUPS_CREATE_OR_UPDATE_CALL_ERROR_MSG + ex.Message;
                status.Errorcode = SprocValues.GENERIC_CATCH_ID;
                Logging.Log(SprocValues.GENERIC_CATCH_ID,
                            EventLogEntryType.Error,
                            SprocValues.AUTHGROUPS_CREATE_OR_UPDATE_CALL_ERROR_MSG,
                            ex);
            }

            reply.StatusReply = status;
            return reply;
        }
    }
}
