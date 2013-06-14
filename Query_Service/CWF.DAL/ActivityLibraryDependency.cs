
namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using CWF.DataContracts;
    using Practices.EnterpriseLibrary.Data;

    public static class ActivityLibraryDependency
    {
        /// <summary>
        /// Creates or updates an mtblActivityLibraryDependencies entry
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>StoreActivityLibrariesDependenciesDC object</returns>
        public static StatusReplyDC ActivityLibraryDependenciesListHeadCreateOrUpdate(ActivityLibraryDependenciesListHeadCreateOrUpdateRequestDC request)
        {
            var reply = new StatusReplyDC();
            Database db = null;
            DbCommand cmd = null;
            int retValue = 0;
            string outErrorString = string.Empty;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = db.GetStoredProcCommand(StoredProcNames.ActivityLibraryDependencyCreteOrUpdateListHead);
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@inActivityLibraryName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Name);
                db.AddParameter(cmd, "@inActivityLibraryVersionNumber", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Version);
                db.AddParameter(cmd, "@InInsertedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InInsertedByUserAlias);
                db.AddParameter(cmd, "@InUpdatedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InUpdatedByUserAlias);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                db.ExecuteScalar(cmd);
                retValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                outErrorString = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                if (retValue != 0)
                {
                    reply.ErrorMessage = outErrorString;
                    reply.Errorcode = retValue;
                    reply = Logging.Log(retValue,
                                        EventLogEntryType.Error,
                                        SprocValues.ACTIVITY_LIBRARY_DEPENDENCIES_LIST_HEAD_CREATE_OR_UPDATE_CALL_ERROR_MSG,
                                        outErrorString);
                }
            }
            catch (Exception ex)
            {
                reply.ErrorMessage = SprocValues.ACTIVITY_LIBRARY_DEPENDENCIES_LIST_HEAD_CREATE_OR_UPDATE_CALL_ERROR_MSG + ex.Message;
                reply.Errorcode = SprocValues.GENERIC_CATCH_ID;
                Logging.Log(SprocValues.GENERIC_CATCH_ID,
                            EventLogEntryType.Error,
                            SprocValues.ACTIVITY_LIBRARY_DEPENDENCIES_CREATE_OR_UPDATE_CALL_ERROR_MSG,
                            ex);
            }

            return reply;
        }

        /// <summary>
        /// Creates or updates an mtblActivityLibraryDependencies entry
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>StoreActivityLibrariesDependenciesDC object</returns>
        public static StoreActivityLibrariesDependenciesDC StoreActivityLibraryDependenciesCreateOrUpdate(StoreActivityLibrariesDependenciesDC request)
        {
            var reply = new StoreActivityLibrariesDependenciesDC();
            var status = new StatusReplyDC();
            Database db = null;
            DbCommand cmd = null;
            int retValue = 0;
            string outErrorString = string.Empty;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = db.GetStoredProcCommand(StoredProcNames.ActivityLibraryDependencyCreateOrUpdate);
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@inActivityLibraryName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.StoreDependenciesRootActiveLibrary.ActivityLibraryName);
                db.AddParameter(cmd, "@inActivityLibraryVersionNumber", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.StoreDependenciesRootActiveLibrary.ActivityLibraryVersionNumber);
                db.AddParameter(cmd, "@inActivityLibraryDependentName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.StoreDependenciesDependentActiveLibraryList[0].ActivityLibraryDependentName);
                db.AddParameter(cmd, "@inActivityLibraryDependentVersionNumber", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.StoreDependenciesDependentActiveLibraryList[0].ActivityLibraryDependentVersionNumber);
                db.AddParameter(cmd, "@InInsertedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InsertedByUserAlias);
                db.AddParameter(cmd, "@InUpdatedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.UpdatedByUserAlias);
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
                                SprocValues.ACTIVITY_LIBRARY_DEPENDENCIES_CREATE_OR_UPDATE_CALL_ERROR_MSG,
                                outErrorString);
                }
            }
            catch (Exception ex)
            {
                status.ErrorMessage = SprocValues.ACTIVITY_LIBRARY_DEPENDENCIES_CREATE_OR_UPDATE_CALL_ERROR_MSG + ex.Message;
                status.Errorcode = SprocValues.GENERIC_CATCH_ID;
                Logging.Log(SprocValues.GENERIC_CATCH_ID,
                            EventLogEntryType.Error,
                            SprocValues.ACTIVITY_LIBRARY_DEPENDENCIES_CREATE_OR_UPDATE_CALL_ERROR_MSG,
                            ex);
            }

            reply.StatusReply = status;
            return reply;
        }
    }
}
