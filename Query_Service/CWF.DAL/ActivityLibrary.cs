
namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using CWF.DataContracts;
    using Practices.EnterpriseLibrary.Data;
    using System.Data.SqlClient;
    using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

    public static class ActivityLibrary
    {
        /// <summary>
        /// If the parameter request.InId is valid, it is an update, otherwise it is a create operation.
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>ActivityLibraryDC object</returns>
        public static ActivityLibraryDC ActivityLibraryCreateOrUpdate(ActivityLibraryDC request)
        {
            var reply = new ActivityLibraryDC();
            var status = new StatusReplyDC();
            int retValue = 0;
            SqlDatabase db = null;
            DbCommand cmd = null;
            string outErrorString = string.Empty;
            try
            {
                db = RepositoryHelper.CreateDatabase();
                cmd = db.GetStoredProcCommand(StoredProcNames.ActivityLibraryCreateOrUpdate);
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InAuthGroupName", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroupNames));         
                db.AddParameter(cmd, "@InId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.Id);
                db.AddParameter(cmd, "@InGUID", DbType.Guid, ParameterDirection.Input, null, DataRowVersion.Default, request.Guid);
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Name);
                db.AddParameter(cmd, "@InCategoryGUID", DbType.Guid, ParameterDirection.Input, null, DataRowVersion.Default, request.Category);
                db.AddParameter(cmd, "@InCategoryName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.CategoryName);
                db.AddParameter(cmd, "@InExecutable", DbType.Binary, ParameterDirection.Input, null, DataRowVersion.Default, request.Executable);
                db.AddParameter(cmd, "@InHasActivities", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.HasActivities);
                db.AddParameter(cmd, "@InDescription", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Description);
                db.AddParameter(cmd, "@InImportedBy", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.ImportedBy);
                db.AddParameter(cmd, "@InVersionNumber", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.VersionNumber);
                db.AddParameter(cmd, "@InMetaTags", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.MetaTags);
                db.AddParameter(cmd, "@InInsertedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InInsertedByUserAlias);
                db.AddParameter(cmd, "@InUpdatedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InUpdatedByUserAlias);
                db.AddParameter(cmd, "@InFriendlyName", DbType.String, ParameterDirection.Input, null,
                                DataRowVersion.Default, request.FriendlyName);
                db.AddParameter(cmd, "@InReleaseNotes", DbType.String, ParameterDirection.Input, null,
                               DataRowVersion.Default, request.ReleaseNotes);
                db.AddParameter(cmd, "@inStatusName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.StatusName);
                db.AddParameter(cmd, "@InEnvironmentTarget", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Environment);
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
                                SprocValues.ACTIVITY_LIBRARY_CREATE_OR_UPDATE_CALL_ERROR_MSG,
                                outErrorString);
                }
            }
            catch (Exception ex)
            {
                status.ErrorMessage = SprocValues.ACTIVITY_LIBRARY_CREATE_OR_UPDATE_CALL_ERROR_MSG + ex.Message;
                status.Errorcode = SprocValues.GENERIC_CATCH_ID;
                Logging.Log(SprocValues.GENERIC_CATCH_ID,
                            EventLogEntryType.Error,
                            SprocValues.ACTIVITY_LIBRARY_CREATE_OR_UPDATE_CALL_ERROR_MSG,
                            ex);
            }

            reply.StatusReply = status;
            return reply;
        }

        /// <summary>
        /// If the request parameters other than @InCaller & @InCallerversion are null, returns a list of entries, otherwise returns a list of one entry.
        /// </summary>
        /// <param name="request">ActivityLibraryDC</param>
        /// <returns>List of ActivityLibraryDC objects</returns>
        public static List<ActivityLibraryDC> ActivityLibraryGet(ActivityLibraryDC request, bool includeDll)
        {
            List<ActivityLibraryDC> reply = new List<ActivityLibraryDC>();
            StatusReplyDC status = new StatusReplyDC();
            ActivityLibraryDC activeLibraryDCreply = null;
            Database db = null;
            DbCommand cmd = null;
            string outErrorString = string.Empty;
            int retValue = 0;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = db.GetStoredProcCommand(StoredProcNames.ActivityLibraryGet);
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.Id);
                db.AddParameter(cmd, "@InGuid", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, Convert.ToString(request.Guid));
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, Convert.ToString(request.Name));
                db.AddParameter(cmd, "@InVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, Convert.ToString(request.VersionNumber));

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        activeLibraryDCreply = new ActivityLibraryDC();
                        activeLibraryDCreply.AuthGroupId = Convert.ToInt32(reader["AuthGroupId"]);
                        activeLibraryDCreply.AuthGroupName = Convert.ToString(reader["AuthGroupName"]);
                        if (reader["Category"] == DBNull.Value)
                            activeLibraryDCreply.Category = Guid.Empty;
                        else
                            activeLibraryDCreply.Category = new Guid(Convert.ToString(reader["Guid"]));

                        if (reader["CategoryId"] == DBNull.Value)
                            activeLibraryDCreply.CategoryId = 0;
                        else
                            activeLibraryDCreply.CategoryId = Convert.ToInt32(reader["CategoryId"]);

                        activeLibraryDCreply.Description = Convert.ToString(reader["Description"]) ?? string.Empty;
                        if (includeDll)
                        {
                            if (reader["Executable"] == DBNull.Value)
                                activeLibraryDCreply.Executable = null;
                            else
                                activeLibraryDCreply.Executable = (byte[])reader["Executable"];
                        }

                        if (reader["Guid"] == DBNull.Value)
                            activeLibraryDCreply.Guid = Guid.Empty;
                        else
                            activeLibraryDCreply.Guid = new Guid(Convert.ToString(reader["Guid"]));

                        if (reader["HasActivities"] == DBNull.Value)
                            activeLibraryDCreply.HasActivities = false;
                        else
                            activeLibraryDCreply.HasActivities = Convert.ToBoolean(reader["HasActivities"]);
                        activeLibraryDCreply.Id = Convert.ToInt32(reader["Id"]);
                        activeLibraryDCreply.ImportedBy = Convert.ToString(reader["ImportedBy"]);
                        activeLibraryDCreply.MetaTags = Convert.ToString(reader["MetaTags"]) ?? string.Empty;
                        activeLibraryDCreply.Name = Convert.ToString(reader["Name"]);
                        if (reader["Status"] == DBNull.Value)
                            activeLibraryDCreply.Status = 0;
                        else
                            activeLibraryDCreply.Status = Convert.ToInt32(reader["Status"]);
                        activeLibraryDCreply.StatusName = Convert.ToString(reader["StatusName"]);
                        activeLibraryDCreply.VersionNumber = Convert.ToString(reader["VersionNumber"]);
                        activeLibraryDCreply.FriendlyName = Convert.ToString(reader["FriendlyName"]);
                        activeLibraryDCreply.ReleaseNotes = Convert.ToString(reader["ReleaseNotes"]);
                        activeLibraryDCreply.Environment = Convert.ToString(reader["Environment"]);
                        reply.Add(activeLibraryDCreply);
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
