using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CWF.DataContracts;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    /// <summary>
    /// Defines the data access services associated with Activity table as the primary table.
    /// </summary>
    public static class ActivityRepositoryService
    {
        /// <summary>
        /// Gets activities by activity library ID or Name & Version combination.
        /// </summary>
        /// <param name="request">Request that specifies activity library identifier info.</param>
        /// <param name="includeXaml">Flag that indicates whether activity XAML should be returned.</param>
        /// <returns>Response that contains a list of activities.</returns>
        public static List<GetLibraryAndActivitiesDC> GetActivitiesByActivityLibrary(GetLibraryAndActivitiesDC request, bool includeXaml)
        {
            List<GetLibraryAndActivitiesDC> reply = null;

            try
            {
                Database database = DatabaseFactory.CreateDatabase();
                DbCommand command = database.GetStoredProcCommand(StoredProcNames.ActivityGetByActivityLibrary);
                database.AddParameter(command, StoredProcParamNames.Id, DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.ActivityLibrary.Id);
                database.AddParameter(command, StoredProcParamNames.Name, DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.ActivityLibrary.Name);
                database.AddParameter(command, StoredProcParamNames.Version, DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.ActivityLibrary.VersionNumber);
                database.AddParameter(command, "@InEnvironmentTarget", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.ActivityLibrary.Environment);

                StoreActivitiesDC activity = null;
                List<StoreActivitiesDC> activities = new List<StoreActivitiesDC>();

                using (IDataReader reader = database.ExecuteReader(command))
                {

                    while (reader.Read())
                    {
                        activity = new StoreActivitiesDC();
                        activity.ActivityCategoryName = Convert.ToString(reader[DataColumnNames.ActivityCategoryName]);
                        activity.ActivityLibraryName = Convert.ToString(reader[DataColumnNames.ActivityLibraryName]);
                        activity.Name = Convert.ToString(reader[DataColumnNames.Name]);
                        activity.Description = Convert.ToString(reader[DataColumnNames.Description]) ?? string.Empty;
                        activity.DeveloperNotes = Convert.ToString(reader[DataColumnNames.DeveloperNotes]);
                        activity.Id = Convert.ToInt32(reader[DataColumnNames.Id]);
                        activity.IsCodeBeside = reader[DataColumnNames.IsCodeBeside] != DBNull.Value ? Convert.ToBoolean(reader[DataColumnNames.IsCodeBeside]) : false;
                        activity.IsService = Convert.ToBoolean(reader[DataColumnNames.IsService]);
                        activity.Locked = reader[DataColumnNames.Locked] != DBNull.Value ? Convert.ToBoolean(reader[DataColumnNames.Locked]) : false;
                        activity.LockedBy = Convert.ToString(reader[DataColumnNames.LockedBy]) ?? string.Empty;
                        activity.MetaTags = Convert.ToString(reader[DataColumnNames.MetaTags]) ?? string.Empty;
                        activity.Namespace = Convert.ToString(reader[DataColumnNames.Namespace]) ?? string.Empty;
                        activity.Guid = new Guid(Convert.ToString(reader[DataColumnNames.Guid]));
                        activity.ToolBoxtab = reader[DataColumnNames.ToolBoxtab] != DBNull.Value ? Convert.ToInt32(reader[DataColumnNames.ToolBoxtab]) : 0;
                        activity.Version = Convert.ToString(reader[DataColumnNames.Version]);
                        activity.WorkflowTypeName = Convert.ToString(reader[DataColumnNames.WorkflowTypeName]);
                        activity.Environment = Convert.ToString(reader[DataColumnNames.Environment]);
                        if (includeXaml)
                        {
                            activity.Xaml = Convert.ToString(reader[DataColumnNames.Xaml]) ?? string.Empty;
                        }
                        activities.Add(activity);
                    }
                }

                reply = new List<GetLibraryAndActivitiesDC>();
                reply.Add(new GetLibraryAndActivitiesDC());
                reply[0].StoreActivitiesList = activities;
            }
            catch (SqlException e)
            {
                e.HandleException();
            }

            return reply;
        }       

        /// <summary>
        /// Search the activities for a criteria and do paging to return only a certain amount of
        /// results.
        /// </summary>
        /// <param name="request">ActivitySearchRequestDC</param>
        /// <returns>List of activities.</returns>
        public static ActivitySearchReplyDC SearchActivities(ActivitySearchRequestDC request)
        {
            ActivitySearchReplyDC result = new ActivitySearchReplyDC();
            var resultCollection = new List<StoreActivitiesDC>();
            SqlDatabase database = null;
            DbCommand command = null;
            StoreActivitiesDC sab = null;
            int retValue = 0;
            string outErrorString = string.Empty;

            try
            {
                database = RepositoryHelper.CreateDatabase();
                command = database.GetStoredProcCommand(StoredProcNames.ActivitySearch);
                database.AddParameter(command, "@InAuthGroupName", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroupNames));
                database.AddParameter(command, "@SearchText", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.SearchText);
                database.AddParameter(command, "@SortColumn", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.SortColumn);
                database.AddParameter(command, "@SortAscending", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.SortAscending);
                database.AddParameter(command, "@PageSize", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.PageSize);
                database.AddParameter(command, "@PageNumber", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.PageNumber);
                database.AddParameter(command, "@FilterOlder", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.FilterOlder);
                database.AddParameter(command, "@FilterByName", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.FilterByName);
                database.AddParameter(command, "@FilterByDescription", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.FilterByDescription);
                database.AddParameter(command, "@FilterByTags", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.FilterByTags);
                database.AddParameter(command, "@FilterByType", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.FilterByType);
                database.AddParameter(command, "@FilterByVersion", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.FilterByVersion);
                database.AddParameter(command, "@FilterByCreator", DbType.Boolean, ParameterDirection.Input, null, DataRowVersion.Default, request.FilterByCreator);
                database.AddParameter(command, "@InEnvironments", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetEnvironments(request.Environments));
                database.AddParameter(command, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                database.AddOutParameter(command, "@outErrorString", DbType.String, 300);

                using (IDataReader reader = database.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        sab = new StoreActivitiesDC();
                        sab.Name = Convert.ToString(reader["Name"]);
                        sab.Description = Convert.ToString(reader["Description"]);
                        sab.Id = Convert.ToInt32(reader["Id"]);
                        sab.InInsertedByUserAlias = Convert.ToString(reader["InsertedByUserAlias"]);
                        sab.MetaTags = Convert.ToString(reader["MetaTags"]);
                        sab.Guid = new Guid(Convert.ToString(reader["Guid"]));
                        sab.Version = Convert.ToString(reader["Version"]);
                        sab.WorkflowTypeName = Convert.ToString(reader["WorkFlowTypeName"]);
                        sab.Environment = Convert.ToString(reader[DataColumnNames.Environment]);
                        if (reader["InsertedDateTime"] != DBNull.Value)
                        {
                            sab.InsertedDateTime = Convert.ToDateTime(reader["InsertedDateTime"]);
                        }
                        resultCollection.Add(sab);
                    }

                    result.SearchResults = resultCollection;
                    reader.NextResult();

                    if (reader.Read())
                    {
                        result.ServerResultsLength = Convert.ToInt32(reader["Total"]);
                    }
                    retValue = Convert.ToInt32(command.Parameters["@ReturnValue"].Value);
                    outErrorString = Convert.ToString(command.Parameters["@outErrorString"].Value);
                    if (retValue != 0)
                    {
                        result.StatusReply.ErrorMessage = outErrorString;
                        result.StatusReply.Errorcode = retValue;
                    }
                }
            }
            catch (SqlException ex)
            {
                ex.HandleException();
            }

            return result;
        }

        /// <summary>
        /// gets all StoreActivities rows that are of template type and not marked softdelete
        /// </summary>
        /// <param name="request">RequestHeader object</param>
        /// <param name="categoryType">categoryType string</param>
        /// <returns>List of StoreActivitiesDC type</returns>
        public static StatusReplyDC CheckActivityExists(StoreActivitiesDC request)
        {

            var reply = new StatusReplyDC();

            Database db = null;
            DbCommand cmd = null;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = db.GetStoredProcCommand(StoredProcNames.ActivityCheckExists);               
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Name);
                db.AddParameter(cmd, "@InVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Version);
                db.AddParameter(cmd, "@InEnvironmentName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Environment);

                reply.Output = Convert.ToString(false);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        reply.Output = Convert.ToString(true);
                    }
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
