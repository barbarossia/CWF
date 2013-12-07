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

    public static class ActivityCategory
    {
        /// <summary>
        /// If the parameter request.InId is valid, it is an update, otherwise it is a create operation.
        /// </summary>
        /// <param name="request">ActivityCategoryCreateOrUpdateRequestDC object</param>
        /// <returns>ActivityCategoryCreateOrUpdateReplyDC object</returns>
        public static ActivityCategoryCreateOrUpdateReplyDC ActivityCategoryCreateOrUpdate(ActivityCategoryCreateOrUpdateRequestDC request)
        {
            var reply = new ActivityCategoryCreateOrUpdateReplyDC();
            var status = new StatusReplyDC();
            Database db = null;
            DbCommand cmd = null;
            int retValue = 0;
            string outErrorString = string.Empty;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.ActivityCategoryCreateOrUpdate);
                
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.InId);
                db.AddParameter(cmd, "@InGuid", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, Convert.ToString(request.InGuid));
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InName);
                db.AddParameter(cmd, "@InDescription", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InDescription);
                db.AddParameter(cmd, "@InMetaTags", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InMetaTags);
                db.AddParameter(cmd, "@InAuthGroupName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InAuthGroupName);
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
                                SprocValues.ACTIVITY_CATEGORY_CREATE_OR_UPDATE_CALL_ERROR_MSG,
                                outErrorString);
                }
            }
            catch (Exception ex)
            {
                status.ErrorMessage = SprocValues.ACTIVITY_CATEGORY_CREATE_OR_UPDATE_CALL_ERROR_MSG + ex.Message;
                status.Errorcode = SprocValues.GENERIC_CATCH_ID;
                Logging.Log(SprocValues.GENERIC_CATCH_ID,
                            EventLogEntryType.Error,
                            SprocValues.ACTIVITY_CATEGORY_CREATE_OR_UPDATE_CALL_ERROR_MSG,
                            ex);
            }

            reply.StatusReply = status;
            return reply;
        }
    }
}
