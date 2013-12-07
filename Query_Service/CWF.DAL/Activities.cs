//-----------------------------------------------------------------------
// <copyright file="Activities.cs" company="Microsoft">
// Copyright
// All DAL methods
// </copyright>
//-----------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using CWF.DataContracts;
    using Practices.EnterpriseLibrary.Data;
    using System.Linq;
    using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

    /// <summary>
    /// CWF.DAL.Activities is the interface class between the WCF and BAL layers and the sprocs in the NEW3PrototypeAssetStore database.
    /// It uses the Microsoft Enterprise Library (MEL) Data Application Access Blocks (DAABS) to create the framefork of a database connection
    /// and command.
    /// 
    /// The structure of each method is structured the same.
    /// 1. Create the reply data contract
    /// 2. Declare the database, command, return value, and return string variables
    /// 3. Populate the parameters from the request DC
    /// 4. Execute the DAABS command
    /// 5. If it was a GET call, populate the reply DC from the firehose reader
    /// 6. Check the return or the catch()
    /// 7. Write the log entry on a soft or hard error (non 0, hard error more than 0, soft error less than zero)
    /// 8. Populate the statusReply object with the results
    /// 9. Move the statusReply into the reply DC
    /// 10.Move the list (form a GET) to the reply DC
    /// 11. Return the reply DC
    /// </summary>
    public static class Activities
    {
        /// <summary>
        /// Deletes the StoreActitities row using the id
        /// </summary>
        /// <param name="request">request  object</param>
        /// <returns>StoreActivitiesDC object</returns>
        public static StoreActivitiesDC StoreActivitiesDelete(StoreActivitiesDC request)
        {
            var reply = new StoreActivitiesDC();
            var status = new StatusReplyDC();
            SqlDatabase db = null;
            DbCommand cmd = null;
            int retValue = 0;
            string outErrorString = string.Empty;

            try
            {
                db = RepositoryHelper.CreateDatabase();
                cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.ActivityDelete);

                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InAuthGroupName", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroupNames));
                db.AddParameter(cmd, "@InEnvironmentName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Environment);
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Name);
                db.AddParameter(cmd, "@InVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Version);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                db.ExecuteNonQuery(cmd);
                retValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                outErrorString = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                if (retValue != 0)
                {
                    status.ErrorMessage = outErrorString;
                    status.Errorcode = retValue;
                    Logging.Log(retValue,
                                EventLogEntryType.Error,
                                SprocValues.STORE_ACTIVITIES_DELETE_CALL_ERROR_MSG,
                                outErrorString);
                }
            }
            catch (SqlException ex)
            {
                ex.HandleException();
            }

            reply.StatusReply = status;
            return reply;
        }

        /// <summary>
        /// If the parameter request.InId is valid, it is an update, otherwise it is a create operation.
        /// </summary>
        /// <param name="request">request  object</param>
        /// <returns>StoreActivitiesDC object</returns>
        public static StoreActivitiesDC StoreActivitiesCreateOrUpdate(StoreActivitiesDC request)
        {
            var reply = new StoreActivitiesDC();
            SqlDatabase db = null;
            DbCommand cmd = null;
            int retValue = 0;
            string outErrorString = string.Empty;

            try
            {
                db = RepositoryHelper.CreateDatabase();
                cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.ActivityCreateOrUpdate);
                
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InAuthGroupName", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroupNames));
                db.AddParameter(cmd, "@InId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.Id);
                db.AddParameter(cmd, "@InGuid", DbType.Guid, ParameterDirection.Input, null, DataRowVersion.Default, request.Guid);
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Name);
                db.AddParameter(cmd, "@InShortName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.ShortName);
                db.AddParameter(cmd, "@InDescription", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Description);
                db.AddParameter(cmd, "@InMetaTags", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.MetaTags);
                db.AddParameter(cmd, "@InIsService", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.IsService);
                db.AddParameter(cmd, "@InActivityLibraryName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.ActivityLibraryName);
                db.AddParameter(cmd, "@InActivityLibraryVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.ActivityLibraryVersion);
                db.AddParameter(cmd, "@InCategoryName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.ActivityCategoryName);
                db.AddParameter(cmd, "@InVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Version);
                db.AddParameter(cmd, "@InStatusName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.StatusCodeName);
                db.AddParameter(cmd, "@InWorkflowTypeName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.WorkflowTypeName);
                db.AddParameter(cmd, "@InLocked", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.Locked);
                db.AddParameter(cmd, "@InLockedBy", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.LockedBy);
                db.AddParameter(cmd, "@InIsCodeBeside", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.IsCodeBeside);
                db.AddParameter(cmd, "@InXAML", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Xaml);
                db.AddParameter(cmd, "@InDeveloperNotes", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.DeveloperNotes);
                db.AddParameter(cmd, "@InNamespace", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Namespace);
                db.AddParameter(cmd, "@InEnvironmentTarget", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Environment);
                db.AddParameter(cmd, "@InInsertedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InInsertedByUserAlias);
                db.AddParameter(cmd, "@InUpdatedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InUpdatedByUserAlias);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        reply = new StoreActivitiesDC();
                        reply.ActivityCategoryName = Convert.ToString(reader["ActivityCategoryName"]);

                        if (reader["ActivityLibraryName"] == DBNull.Value)
                            reply.ActivityLibraryName = string.Empty;
                        else
                            reply.ActivityLibraryName = Convert.ToString(reader["ActivityLibraryName"]);

                        if (reader["ActivityLibraryId"] == DBNull.Value)
                            reply.ActivityLibraryId = 0;
                        else
                            reply.ActivityLibraryId = Convert.ToInt32(reader["ActivityLibraryId"]);
                        reply.ActivityLibraryVersion = Convert.ToString(reader["ActivityLibraryVersion"]);
                        reply.Name = Convert.ToString(reader["Name"]);
                        reply.ShortName = Convert.ToString(reader["ShortName"]);
                        reply.Description = Convert.ToString(reader["Description"]) ?? string.Empty;
                        reply.DeveloperNotes = Convert.ToString(reader["DeveloperNotes"]) ?? string.Empty;
                        reply.Id = Convert.ToInt32(reader["Id"]);
                        if (reader["IsCodeBeside"] == DBNull.Value)
                            reply.IsCodeBeside = false;
                        else
                            reply.IsCodeBeside = Convert.ToBoolean(reader["IsCodeBeside"]);
                        reply.IsService = Convert.ToBoolean(reader["IsService"]);
                        if (reader["Locked"] == DBNull.Value)
                            reply.Locked = false;
                        else
                            reply.Locked = Convert.ToBoolean(reader["Locked"]);
                        reply.LockedBy = Convert.ToString(reader["LockedBy"]) ?? string.Empty;
                        reply.MetaTags = Convert.ToString(reader["MetaTags"]) ?? string.Empty;
                        reply.Name = Convert.ToString(reader["Name"]);
                        reply.Namespace = Convert.ToString(reader["Namespace"]) ?? string.Empty;
                        reply.Guid = new Guid(Convert.ToString(reader["Guid"]));
                        if (reader["ToolBoxtab"] == DBNull.Value)
                            reply.ToolBoxtab = 0;
                        else
                            reply.ToolBoxtab = Convert.ToInt32(reader["ToolBoxtab"]);
                        reply.Version = Convert.ToString(reader["Version"]);
                        reply.WorkflowTypeName = Convert.ToString(reader["WorkFlowTypeName"]);
                        reply.Xaml = Convert.ToString(reader["XAML"]) ?? string.Empty;
                        reply.InInsertedByUserAlias = Convert.ToString(reader["InsertedByUserAlias"]);
                        reply.InsertedDateTime = Convert.ToDateTime(reader["InsertedDateTime"]);
                        reply.InUpdatedByUserAlias = Convert.ToString(reader["UpdatedByUserAlias"]);
                        reply.UpdatedDateTime = Convert.ToDateTime(reader["UpdatedDateTime"]);
                        reply.Environment = Convert.ToString(reader["Environment"]);
                    }
                }

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
        /// If the request parameters other than @InCaller & @InCallerversion are null, returns a list of entries, otherwise returns a list of one entry.
        /// </summary>
        /// <param name="request">StoreActivitiesDC</param>
        /// <returns>List<StoreActivitiesDC></returns>
        public static List<StoreActivitiesDC> StoreActivitiesGet(StoreActivitiesDC request)
        {
            List<StoreActivitiesDC> reply = new List<StoreActivitiesDC>();
            StatusReplyDC status = new StatusReplyDC();
            string outErrorString = string.Empty;
            StoreActivitiesDC sab = null;
            Database db = null;
            DbCommand cmd = null;
            int retValue = 0;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.ActivityGet);

                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.Id);
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Name);
                db.AddParameter(cmd, "@InGUID", DbType.Guid, ParameterDirection.Input, null, DataRowVersion.Default, request.Guid);
                db.AddParameter(cmd, "@InVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Version);
                db.AddParameter(cmd, "@InEnvironmentName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Environment);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        sab = new StoreActivitiesDC();
                        sab.ActivityCategoryName = Convert.ToString(reader["ActivityCategoryName"]);

                        if (reader["ActivityLibraryName"] == DBNull.Value)
                            sab.ActivityLibraryName = string.Empty;
                        else
                            sab.ActivityLibraryName = Convert.ToString(reader["ActivityLibraryName"]);

                        if (reader["ActivityLibraryId"] == DBNull.Value)
                            sab.ActivityLibraryId = 0;
                        else
                            sab.ActivityLibraryId = Convert.ToInt32(reader["ActivityLibraryId"]);
                        sab.ActivityLibraryVersion = Convert.ToString(reader["ActivityLibraryVersion"]);
                        sab.AuthGroupName = Convert.ToString(reader["AuthgroupName"]);
                        sab.Name = Convert.ToString(reader["Name"]);
                        sab.ShortName = Convert.ToString(reader["ShortName"]);
                        sab.Description = Convert.ToString(reader["Description"]);
                        sab.DeveloperNotes = Convert.ToString(reader["DeveloperNotes"]);
                        sab.Id = Convert.ToInt32(reader["Id"]);
                        if (reader["IsCodeBeside"] == DBNull.Value)
                            sab.IsCodeBeside = false;
                        else
                            sab.IsCodeBeside = Convert.ToBoolean(reader["IsCodeBeside"]);
                        sab.IsService = Convert.ToBoolean(reader["IsService"]);
                        if (reader["Locked"] == DBNull.Value)
                            sab.Locked = false;
                        else
                            sab.Locked = Convert.ToBoolean(reader["Locked"]);
                        sab.LockedBy = Convert.ToString(reader["LockedBy"]) ?? string.Empty;
                        sab.MetaTags = Convert.ToString(reader["MetaTags"]) ?? string.Empty;
                        sab.Name = Convert.ToString(reader["Name"]);
                        sab.Namespace = Convert.ToString(reader["Namespace"]) ?? string.Empty;
                        sab.Guid = new Guid(Convert.ToString(reader["Guid"]));
                        sab.ToolBoxtab = reader["ToolBoxtab"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ToolBoxtab"]);
                        sab.Version = Convert.ToString(reader["Version"]);
                        sab.WorkflowTypeName = Convert.ToString(reader["WorkFlowTypeName"]);
                        sab.Xaml = Convert.ToString(reader["XAML"]) ?? string.Empty;
                        sab.InInsertedByUserAlias = Convert.ToString(reader["InsertedByUserAlias"]);
                        sab.InsertedDateTime = Convert.ToDateTime(reader["InsertedDateTime"]);
                        sab.InUpdatedByUserAlias = Convert.ToString(reader["UpdatedByUserAlias"]);
                        sab.UpdatedDateTime = Convert.ToDateTime(reader["UpdatedDateTime"]);
                        sab.StatusCodeName = Convert.ToString(reader["StatusCodeName"]);
                        sab.Environment = Convert.ToString(reader["Environment"]);
                        reply.Add(sab);
                    }

                    retValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                    outErrorString = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                    if (retValue != 0)
                    {
                        status.ErrorMessage = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                        status.Errorcode = retValue;
                        if (reply.Count == 0)
                            reply.Add(new StoreActivitiesDC());
                    }
                }
            }
            catch (SqlException ex)
            {
                ex.HandleException();
            }

            if (reply.Count == 0)
                reply.Add(new StoreActivitiesDC());
            reply[0].StatusReply = status;
            return reply;
        }

        /// <summary>
        /// Get all records for a workflow name that are public or retired, plus all workflows with that 
        /// name that are private and owned by the specified user.
        /// </summary>
        /// <param name="activityName">the name of the workflow we are looking for</param>
        /// <param name="userName">the user who owns the private records</param>
        /// <returns></returns>
        public static List<StoreActivitiesDC> StoreActivitiesGetByName(string activityName, string env)
        {
            List<StoreActivitiesDC> reply = new List<StoreActivitiesDC>();
            Database db = null;
            DbCommand cmd = null;
            StoreActivitiesDC sat;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.ActivityGetByName);

                db.AddParameter(cmd, "@Name", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, activityName);
                db.AddParameter(cmd, "@Env", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, env);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        sat = new StoreActivitiesDC();
                        sat.ActivityCategoryName = reader["ActivityCategoryName"].ToString();
                        sat.ActivityLibraryVersion = reader["ActivityLibraryVersion"].ToString();
                        sat.AuthGroupName = reader["AuthgroupName"].ToString();
                        sat.Name = reader["Name"].ToString();
                        sat.ShortName = reader["ShortName"].ToString();
                        sat.Description = Convert.ToString(reader["Description"]);
                        sat.DeveloperNotes = Convert.ToString(reader["DeveloperNotes"]);
                        sat.Id = Convert.ToInt32(reader["Id"]);
                        sat.IsService = Convert.ToBoolean(reader["IsService"]);
                        sat.LockedBy = Convert.ToString(reader["LockedBy"]);
                        sat.MetaTags = Convert.ToString(reader["MetaTags"]);
                        sat.Namespace = Convert.ToString(reader["Namespace"]);
                        sat.Guid = new Guid(Convert.ToString(reader["Guid"]));
                        sat.Version = reader["Version"].ToString();
                        sat.WorkflowTypeName = reader["WorkFlowTypeName"].ToString();
                        sat.Xaml = Convert.ToString(reader["XAML"]);

                        sat.ActivityLibraryName = (reader["ActivityLibraryName"] == DBNull.Value)
                                        ? string.Empty
                                        : Convert.ToString(reader["ActivityLibraryName"]);

                        sat.ActivityLibraryId = (reader["ActivityLibraryId"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["ActivityLibraryId"]);
                        sat.IsCodeBeside = reader["IsCodeBeside"] != DBNull.Value && Convert.ToBoolean(reader["IsCodeBeside"]);
                        sat.Locked = reader["Locked"] != DBNull.Value && Convert.ToBoolean(reader["Locked"]);
                        sat.ToolBoxtab = (reader["ToolBoxtab"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["ToolBoxtab"]);
                        sat.InInsertedByUserAlias = Convert.ToString(reader["InsertedByUserAlias"]);
                        sat.InsertedDateTime = Convert.ToDateTime(reader["InsertedDateTime"]);
                        sat.Environment = Convert.ToString(reader["Environment"]);
                        reply.Add(sat);
                    }
                }
            }
            catch (SqlException ex)
            {
                ex.HandleException();
            }

            return reply;
        }

        /// <summary>
        /// Update the activity locked and lockedby feild
        /// </summary>
        /// <param name="request">request  object</param>
        /// <returns>StoreActivitiesDC object</returns>
        public static StoreActivitiesDC StoreActivitiesUpdateLock(StoreActivitiesDC request, DateTime lockedTime)
        {
            var reply = new StoreActivitiesDC();
            SqlDatabase db = null;
            DbCommand cmd = null;
            int retValue = 0;
            string outErrorString = string.Empty;

            try
            {
                db = RepositoryHelper.CreateDatabase();
                cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.ActivityUpdateLock);
                
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InAuthGroupName", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroupNames));
                db.AddParameter(cmd, "@InEnvironmentName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Environment);
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Name);
                db.AddParameter(cmd, "@InVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Version);
                db.AddParameter(cmd, "@InLocked", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.Locked);
                db.AddParameter(cmd, "@InOperatorUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.LockedBy);
                db.AddParameter(cmd, "@InLockedTime", DbType.DateTime2, ParameterDirection.Input, null, DataRowVersion.Default, lockedTime);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                db.ExecuteNonQuery(cmd);
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
        /// Update the activity locked and lockedby feild
        /// </summary>
        /// <param name="request">request  object</param>
        /// <returns>StoreActivitiesDC object</returns>
        public static StoreActivitiesDC StoreActivitiesOverrideLock(StoreActivitiesDC request, DateTime lockedTime)
        {
            var reply = new StoreActivitiesDC();
            SqlDatabase db = null;
            DbCommand cmd = null;
            int retValue = 0;
            string outErrorString = string.Empty;

            try
            {
                db = RepositoryHelper.CreateDatabase();
                cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.ActivityOverrideLock);
                
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InAuthGroupName", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroupNames));
                db.AddParameter(cmd, "@InEnvironmentName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Environment);
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Name);
                db.AddParameter(cmd, "@InVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Version);
                db.AddParameter(cmd, "@InOperatorUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.LockedBy);
                db.AddParameter(cmd, "@InLockedTime", DbType.DateTime2, ParameterDirection.Input, null, DataRowVersion.Default, lockedTime);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                db.ExecuteNonQuery(cmd);
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

        public static ChangeAuthorReply ChangeAuthor(ChangeAuthorRequest request)
        {
            var reply = new ChangeAuthorReply();
            SqlDatabase db = null;
            DbCommand cmd = null;
            int retValue = 0;
            string outErrorString = string.Empty;

            try
            {
                db = RepositoryHelper.CreateDatabase();
                cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.ActivityChangeAuthor);

                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InAuthGroupName", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroupNames));
                db.AddParameter(cmd, "@InEnvironmentName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Environment);
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Name);
                db.AddParameter(cmd, "@InVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Version);
                db.AddParameter(cmd, "@InAuthorAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.AuthorAlias);
                db.AddParameter(cmd, "@InOperatorUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InUpdatedByUserAlias);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                db.ExecuteNonQuery(cmd);
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

        public static StoreActivitiesDC ActivityCopy(ActivityCopyRequest request)
        {
            var reply = new StoreActivitiesDC();
            SqlDatabase db = null;
            DbCommand cmd = null;
            int retValue = 0;
            string outErrorString = string.Empty;

            try
            {
                db = RepositoryHelper.CreateDatabase();
                cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.ActivityCopy);

                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InAuthGroupName", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroupNames));
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Name);
                db.AddParameter(cmd, "@InVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Version);
                db.AddParameter(cmd, "@InNewVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.NewName);
                db.AddParameter(cmd, "@InEnvironment", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Environment);
                db.AddParameter(cmd, "@InEnvironmentTarget", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.EnvironmentTarget);
                db.AddParameter(cmd, "@InWorkflowTypeID", DbType.Int64, ParameterDirection.Input, null, DataRowVersion.Default, request.WorkflowTypeId);
                db.AddParameter(cmd, "@InInsertedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InInsertedByUserAlias);
                db.AddParameter(cmd, "@InUpdatedByUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InUpdatedByUserAlias);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        reply.ActivityCategoryName = Convert.ToString(reader["ActivityCategoryName"]);

                        if (reader["ActivityLibraryName"] == DBNull.Value)
                            reply.ActivityLibraryName = string.Empty;
                        else
                            reply.ActivityLibraryName = Convert.ToString(reader["ActivityLibraryName"]);

                        if (reader["ActivityLibraryId"] == DBNull.Value)
                            reply.ActivityLibraryId = 0;
                        else
                            reply.ActivityLibraryId = Convert.ToInt32(reader["ActivityLibraryId"]);
                        reply.ActivityLibraryVersion = Convert.ToString(reader["ActivityLibraryVersion"]);
                        reply.Name = Convert.ToString(reader["Name"]);
                        reply.ShortName = Convert.ToString(reader["ShortName"]);
                        reply.Description = Convert.ToString(reader["Description"]) ?? string.Empty;
                        reply.DeveloperNotes = Convert.ToString(reader["DeveloperNotes"]) ?? string.Empty;
                        reply.Id = Convert.ToInt32(reader["Id"]);
                        if (reader["IsCodeBeside"] == DBNull.Value)
                            reply.IsCodeBeside = false;
                        else
                            reply.IsCodeBeside = Convert.ToBoolean(reader["IsCodeBeside"]);
                        reply.IsService = Convert.ToBoolean(reader["IsService"]);
                        if (reader["Locked"] == DBNull.Value)
                            reply.Locked = false;
                        else
                            reply.Locked = Convert.ToBoolean(reader["Locked"]);
                        reply.LockedBy = Convert.ToString(reader["LockedBy"]) ?? string.Empty;
                        reply.MetaTags = Convert.ToString(reader["MetaTags"]) ?? string.Empty;
                        reply.Name = Convert.ToString(reader["Name"]);
                        reply.Namespace = Convert.ToString(reader["Namespace"]) ?? string.Empty;
                        reply.Guid = new Guid(Convert.ToString(reader["Guid"]));
                        if (reader["ToolBoxtab"] == DBNull.Value)
                            reply.ToolBoxtab = 0;
                        else
                            reply.ToolBoxtab = Convert.ToInt32(reader["ToolBoxtab"]);
                        reply.Version = Convert.ToString(reader["Version"]);
                        reply.WorkflowTypeName = Convert.ToString(reader["WorkFlowTypeName"]);
                        reply.Xaml = Convert.ToString(reader["XAML"]) ?? string.Empty;
                        reply.InInsertedByUserAlias = Convert.ToString(reader["InsertedByUserAlias"]);
                        reply.InsertedDateTime = Convert.ToDateTime(reader["InsertedDateTime"]);
                        reply.InUpdatedByUserAlias = Convert.ToString(reader["UpdatedByUserAlias"]);
                        reply.UpdatedDateTime = Convert.ToDateTime(reader["UpdatedDateTime"]);
                        reply.Environment = Convert.ToString(reader["Environment"]);
                    }
                }

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

        public static ActivityMoveReply ActivityMove(ActivityMoveRequest request)
        {
            var reply = new ActivityMoveReply();
            SqlDatabase db = null;
            DbCommand cmd = null;
            int retValue = 0;
            string outErrorString = string.Empty;

            try
            {
                db = RepositoryHelper.CreateDatabase();
                cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.ActivityMove);
                
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InAuthGroupName", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroupNames));
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Name);
                db.AddParameter(cmd, "@InVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Version);
                db.AddParameter(cmd, "@InEnvironment", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Environment);
                db.AddParameter(cmd, "@InEnvironmentTarget", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.EnvironmentTarget);
                db.AddParameter(cmd, "@InWorkflowTypeID", DbType.Int64, ParameterDirection.Input, null, DataRowVersion.Default, request.WorkflowTypeId);
                db.AddParameter(cmd, "@InOperatorUserAlias", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InUpdatedByUserAlias);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                db.ExecuteNonQuery(cmd);
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
    }
}
