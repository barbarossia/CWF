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

namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    public static class TaskActivityRepositoryService
    {

        /// <summary>
        /// If the parameter request.InId is valid, it is an update, otherwise it is a create operation.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static TaskActivityDC TaskActivitiesCreateOrUpdate(TaskActivityDC request)
        {
            var taskActivity = new TaskActivityDC();
            var storeActivityDc = new StoreActivitiesDC();
            var status = new StatusReplyDC();
            taskActivity.StatusReply = status;

            Database db = null;
            DbCommand cmd = null;
            int retValue = 0;
            string outErrorString = string.Empty;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = db.GetStoredProcCommand(StoredProcNames.TaskActivityCreateOrUpdate);
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.Id);
                db.AddParameter(cmd, "@InActivityId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.ActivityId);
                db.AddParameter(cmd, "@InGuid", DbType.Guid, ParameterDirection.Input, null, DataRowVersion.Default, request.Guid);
                db.AddParameter(cmd, "@InAssignedTo", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.AssignedTo);
                db.AddParameter(cmd, "@InStatus", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Status.ToString());
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        taskActivity = new TaskActivityDC();
                        storeActivityDc = new StoreActivitiesDC();
                        storeActivityDc.ActivityCategoryName = Convert.ToString(reader["ActivityCategoryName"]);

                        if (reader["ActivityLibraryName"] == DBNull.Value)
                            storeActivityDc.ActivityLibraryName = string.Empty;
                        else
                            storeActivityDc.ActivityLibraryName = Convert.ToString(reader["ActivityLibraryName"]);

                        if (reader["ActivityLibraryId"] == DBNull.Value)
                            storeActivityDc.ActivityLibraryId = 0;
                        else
                            storeActivityDc.ActivityLibraryId = Convert.ToInt32(reader["ActivityLibraryId"]);
                        storeActivityDc.ActivityLibraryVersion = Convert.ToString(reader["ActivityLibraryVersion"]);
                        storeActivityDc.Name = Convert.ToString(reader["Name"]);
                        storeActivityDc.ShortName = Convert.ToString(reader["ShortName"]);
                        storeActivityDc.Description = Convert.ToString(reader["Description"]) ?? string.Empty;
                        storeActivityDc.DeveloperNotes = Convert.ToString(reader["DeveloperNotes"]) ?? string.Empty;
                        storeActivityDc.Id = Convert.ToInt32(reader["Id"]);
                        if (reader["IsCodeBeside"] == DBNull.Value)
                            storeActivityDc.IsCodeBeside = false;
                        else
                            storeActivityDc.IsCodeBeside = Convert.ToBoolean(reader["IsCodeBeside"]);
                        storeActivityDc.IsService = Convert.ToBoolean(reader["IsService"]);
                        if (reader["Locked"] == DBNull.Value)
                            storeActivityDc.Locked = false;
                        else
                            storeActivityDc.Locked = Convert.ToBoolean(reader["Locked"]);
                        storeActivityDc.LockedBy = Convert.ToString(reader["LockedBy"]) ?? string.Empty;
                        storeActivityDc.MetaTags = Convert.ToString(reader["MetaTags"]) ?? string.Empty;
                        storeActivityDc.Name = Convert.ToString(reader["Name"]);
                        storeActivityDc.Namespace = Convert.ToString(reader["Namespace"]) ?? string.Empty;
                        storeActivityDc.Guid = new Guid(Convert.ToString(reader["Guid"]));
                        if (reader["ToolBoxtab"] == DBNull.Value)
                            storeActivityDc.ToolBoxtab = 0;
                        else
                            storeActivityDc.ToolBoxtab = Convert.ToInt32(reader["ToolBoxtab"]);
                        storeActivityDc.Version = Convert.ToString(reader["Version"]);
                        storeActivityDc.WorkflowTypeName = Convert.ToString(reader["WorkFlowTypeName"]);
                        storeActivityDc.InsertedByUserAlias = Convert.ToString(reader["InsertedByUserAlias"]);
                        storeActivityDc.InsertedDateTime = Convert.ToDateTime(reader["InsertedDateTime"]);
                        storeActivityDc.UpdatedByUserAlias = Convert.ToString(reader["UpdatedByUserAlias"]);
                        storeActivityDc.UpdatedDateTime = Convert.ToDateTime(reader["UpdatedDateTime"]);
                        taskActivity.Id = Convert.ToInt32(reader["TaskActivityId"]);
                        taskActivity.ActivityId = Convert.ToInt32(reader["ActivityId"]);
                        if (reader["AssignedTo"] != DBNull.Value)
                            taskActivity.AssignedTo = Convert.ToString(reader["AssignedTo"]);
                        if (reader["Status"] != DBNull.Value)
                            taskActivity.Status = (TaskActivityStatus)Enum.Parse(typeof(TaskActivityStatus), Convert.ToString(reader["Status"]));
                        taskActivity.Guid = new Guid(Convert.ToString(reader["TaskActivityGuid"]));
                    }

                    taskActivity.Activity = storeActivityDc;
                    retValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                    if (retValue != 0)
                    {
                        status.ErrorMessage = outErrorString;
                        status.Errorcode = retValue;
                        Logging.Log(retValue,
                            EventLogEntryType.Error,
                            "Store_TaskActivity_Create_OR_Update_Error_MSG",
                            outErrorString);
                    }
                }
            }
            catch (Exception ex)
            {
                status.ErrorMessage = "Store_TaskActivity_Create_OR_Update_Error_MSG" + ex.Message;
                status.Errorcode = SprocValues.GENERIC_CATCH_ID;
                Logging.Log(SprocValues.GENERIC_CATCH_ID,
                            EventLogEntryType.Error,
                            "Store_TaskActivity_Create_OR_Update_Error_MSG",
                            ex);
            }
            taskActivity.StatusReply = status;
            return taskActivity;
        }

        /// <summary>
        /// If the request parameters other than @InCaller & @InCallerversion are null, returns a list of entries, otherwise returns a list of one entry.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static TaskActivityGetReplyDC SearchTaskActivities(TaskActivityGetRequestDC request)
        {
            TaskActivityGetReplyDC reply = new TaskActivityGetReplyDC();
            StatusReplyDC status = new StatusReplyDC();
            string outErrorString = string.Empty;
            TaskActivityDC taskActivity = null;
            Database db = null;
            DbCommand cmd = null;
            int retValue = 0;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = db.GetStoredProcCommand(StoredProcNames.TaskActivitySearch);
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InTaskActivityGUID", DbType.Guid, ParameterDirection.Input, null, DataRowVersion.Default, request.TaskActivityGuid);
                db.AddParameter(cmd, "@InAssignedTo", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.AssignedTo);
                db.AddParameter(cmd, "@InFilterOlder", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.FilterOlder);
                db.AddParameter(cmd, "@SearchText", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.SearchText);
                db.AddParameter(cmd, "@SortColumn", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.SortColumn);
                db.AddParameter(cmd, "@SortAscending", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.SortAscending);
                db.AddParameter(cmd, "@PageSize", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.PageSize);
                db.AddParameter(cmd, "@PageNumber", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.PageNumber);
                db.AddParameter(cmd, "@HideUnassignedTasks", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.HideUnassignedTasks);
                
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    reply.List = new List<TaskActivityDC>();
                    while (reader.Read())
                    {
                        taskActivity = new TaskActivityDC();
                        StoreActivitiesDC sab = new StoreActivitiesDC();
                        sab.Id = Convert.ToInt32(reader["Id"]);
                        sab.Guid = new Guid(Convert.ToString(reader["Guid"]));
                        sab.Name = Convert.ToString(reader["Name"]);
                        sab.Version = Convert.ToString(reader["Version"]);
                        sab.Description = Convert.ToString(reader["Description"]);
                        sab.MetaTags = Convert.ToString(reader["MetaTags"]) ?? string.Empty;
                        sab.InsertedByUserAlias = Convert.ToString(reader["InsertedByUserAlias"]);
                        sab.InsertedDateTime = Convert.ToDateTime(reader["InsertedDateTime"]);

                        if (request.IncludeDetails)
                        {
                            if (reader["Locked"] == DBNull.Value)
                                sab.Locked = false;
                            else
                                sab.Locked = Convert.ToBoolean(reader["Locked"]);
                            sab.LockedBy = Convert.ToString(reader["LockedBy"]) ?? string.Empty;
                            sab.ActivityCategoryName = Convert.ToString(reader["ActivityCategoryName"]);
                            if (reader["ActivityLibraryName"] == DBNull.Value)
                                sab.ActivityLibraryName = string.Empty;
                            else
                                sab.ActivityLibraryName = Convert.ToString(reader["ActivityLibraryName"]);
                            if (reader["ActivityLibraryId"] == DBNull.Value)
                                sab.ActivityLibraryId = 0;
                            else
                                sab.ActivityLibraryId = Convert.ToInt32(reader["ActivityLibraryId"]);
                            sab.Xaml = Convert.ToString(reader["XAML"]) ?? string.Empty;
                            sab.ActivityLibraryVersion = Convert.ToString(reader["ActivityLibraryVersion"]);
                            sab.AuthGroupName = Convert.ToString(reader["AuthgroupName"]);
                            sab.ShortName = Convert.ToString(reader["ShortName"]);
                            sab.DeveloperNotes = Convert.ToString(reader["DeveloperNotes"]);
                            if (reader["IsCodeBeside"] == DBNull.Value)
                                sab.IsCodeBeside = false;
                            else
                                sab.IsCodeBeside = Convert.ToBoolean(reader["IsCodeBeside"]);
                            sab.IsService = Convert.ToBoolean(reader["IsService"]);
                            sab.Namespace = Convert.ToString(reader["Namespace"]) ?? string.Empty;
                            sab.ToolBoxtab = reader["ToolBoxtab"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ToolBoxtab"]);
                            sab.WorkflowTypeName = Convert.ToString(reader["WorkFlowTypeName"]);
                            sab.UpdatedByUserAlias = Convert.ToString(reader["UpdatedByUserAlias"]);
                            sab.UpdatedDateTime = Convert.ToDateTime(reader["UpdatedDateTime"]);
                            sab.StatusCodeName = Convert.ToString(reader["StatusCodeName"]);
                        }
                        taskActivity.Id = Convert.ToInt32(reader["TaskActivityId"]);
                        taskActivity.ActivityId = Convert.ToInt32(reader["ActivityId"]);
                        if (reader["AssignedTo"] != DBNull.Value)
                            taskActivity.AssignedTo = Convert.ToString(reader["AssignedTo"]);
                        if (reader["Status"] != DBNull.Value)
                            taskActivity.Status = (TaskActivityStatus)Enum.Parse(typeof(TaskActivityStatus), Convert.ToString(reader["Status"]));
                        taskActivity.Guid = new Guid(Convert.ToString(reader["TaskActivityGuid"]));
                        taskActivity.Activity = sab;
                        reply.List.Add(taskActivity);
                    }

                    reader.NextResult();
                    if (reader.Read())
                    {
                        reply.ServerResultsLength = Convert.ToInt32(reader["Total"]);
                    }

                    retValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                    outErrorString = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                    if (retValue != 0)
                    {
                        status.ErrorMessage = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                        status.Errorcode = retValue;
                        Logging.Log(retValue,
                                    EventLogEntryType.Error,
                                    "TaskActivity_Get_ERROR_MSG",
                                    outErrorString);
                    }
                }
            }
            catch (Exception ex)
            {
                status = Logging.Log(SprocValues.GENERIC_CATCH_ID,
                                     EventLogEntryType.Error,
                                     "TaskActivity_Get_ERROR_MSG",
                                     ex);
            }
            reply.StatusReply = status;
            return reply;
        }

        public static TaskActivityDC TaskActivity_SetStatus(TaskActivityDC request) 
        {
            TaskActivityDC reply = new TaskActivityDC();
            StatusReplyDC status = new StatusReplyDC();
            string outErrorString = string.Empty;
            Database db = null;
            DbCommand cmd = null;
            int retValue = 0;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = db.GetStoredProcCommand(StoredProcNames.TaskActivityUpdateStatus);
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InTaskActivityGUID", DbType.Guid, ParameterDirection.Input, null, DataRowVersion.Default, request.Guid);
                db.AddParameter(cmd, "@InId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.Id);
                db.AddParameter(cmd, "@InStatus", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Status.ToString());
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
                                "Update TaskActivity Status Error",
                                outErrorString);
                }
            }
            catch (Exception ex)
            {
                status.ErrorMessage = "Update TaskActivity Status Error: " + ex.Message;
                status.Errorcode = SprocValues.GENERIC_CATCH_ID;
                status = Logging.Log(SprocValues.GENERIC_CATCH_ID,
                                     EventLogEntryType.Error,
                                     "TaskActivity_UpdateStatus_ERROR_MSG",
                                     ex);
            }
            reply.StatusReply = status;
            return reply;
        }

        public static TaskActivityDC TaskActivityGet(TaskActivityDC request)
        {
            TaskActivityDC reply = new TaskActivityDC();
            StatusReplyDC status = new StatusReplyDC();
            string outErrorString = string.Empty;
            Database db = null;
            DbCommand cmd = null;
            int retValue = 0;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = db.GetStoredProcCommand(StoredProcNames.TaskActivityGet);
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.Id);
                db.AddParameter(cmd, "@InTaskActivityGUID", DbType.Guid, ParameterDirection.Input, null, DataRowVersion.Default, request.Guid);
                db.AddParameter(cmd, "@InActivityId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.ActivityId);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);
                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        reply = new TaskActivityDC();
                        StoreActivitiesDC sab = new StoreActivitiesDC();
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
                        sab.Xaml = Convert.ToString(reader["XAML"]) ?? string.Empty;
                        sab.Name = Convert.ToString(reader["Name"]);
                        sab.Namespace = Convert.ToString(reader["Namespace"]) ?? string.Empty;
                        sab.Guid = new Guid(Convert.ToString(reader["Guid"]));
                        sab.ToolBoxtab = reader["ToolBoxtab"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ToolBoxtab"]);
                        sab.Version = Convert.ToString(reader["Version"]);
                        sab.WorkflowTypeName = Convert.ToString(reader["WorkFlowTypeName"]);
                        sab.InsertedByUserAlias = Convert.ToString(reader["InsertedByUserAlias"]);
                        sab.InsertedDateTime = Convert.ToDateTime(reader["InsertedDateTime"]);
                        sab.UpdatedByUserAlias = Convert.ToString(reader["UpdatedByUserAlias"]);
                        sab.UpdatedDateTime = Convert.ToDateTime(reader["UpdatedDateTime"]);
                        sab.StatusCodeName = Convert.ToString(reader["StatusCodeName"]);
                        reply.Id = Convert.ToInt32(reader["TaskActivityId"]);
                        reply.ActivityId = Convert.ToInt32(reader["ActivityId"]);
                        if (reader["AssignedTo"] != DBNull.Value)
                            reply.AssignedTo = Convert.ToString(reader["AssignedTo"]);
                        if (reader["Status"] != DBNull.Value)
                            reply.Status = (TaskActivityStatus)Enum.Parse(typeof(TaskActivityStatus), Convert.ToString(reader["Status"]));
                        reply.Guid = new Guid(Convert.ToString(reader["TaskActivityGuid"]));
                        reply.Activity = sab;
                    }

                    retValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                    outErrorString = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                    if (retValue != 0)
                    {
                        status.ErrorMessage = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                        status.Errorcode = retValue;
                        Logging.Log(retValue,
                                    EventLogEntryType.Error,
                                    "TaskActivity_Get_ERROR_MSG",
                                    outErrorString);
                    }
                }
            }
            catch (Exception ex)
            {
                status = Logging.Log(SprocValues.GENERIC_CATCH_ID,
                                     EventLogEntryType.Error,
                                     "TaskActivity_Get_ERROR_MSG",
                                     ex);
            }
            reply.StatusReply = status;
            return reply;
        }
    }
}
