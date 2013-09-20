

namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using CWF.DataContracts;
    using Practices.EnterpriseLibrary.Data;

    /// <summary>
    /// Defines the data access services associated with ActivityLibrary table as the primary table.
    /// </summary>
    public class ActivityLibraryRepositoryService
    {
        /// <summary>
        /// Get an activity library by ID if a positive ID value is provided.  Otherwise, gets by GUID
        /// if a non-empty GUID is provided.  If the ID is not positive and the GUID is empty, then 
        /// a list of activity libraries matching the name and version combination.
        /// </summary>
        /// <param name="request">ActivityLibraryDC</param>
        /// <param name="includeDll">Flag that indicates whether the DLL binaries should be included in the response.</param>
        /// <returns>List of activity libraries.</returns>
        public static List<ActivityLibraryDC> GetActivityLibraries(ActivityLibraryDC request, bool includeDll)
        {
            var reply = new List<ActivityLibraryDC>();
            var status = new StatusReplyDC();
            Database database = null;
            DbCommand command = null;
            string outErrorString = string.Empty;
            try
            {
                database = DatabaseFactory.CreateDatabase();
                command = database.GetStoredProcCommand(StoredProcNames.ActivityLibraryGet);
                database.AddParameter(command, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                database.AddParameter(command, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                database.AddParameter(command, "@InId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.Id);
                database.AddParameter(command, "@InGuid", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, Convert.ToString(request.Guid));
                database.AddParameter(command, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, Convert.ToString(request.Name));
                database.AddParameter(command, "@InVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, Convert.ToString(request.VersionNumber));
                database.AddParameter(command, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);

                using (IDataReader reader = database.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        var activeLibraryDCreply = new ActivityLibraryDC();
                        activeLibraryDCreply.AuthGroupId = Convert.ToInt32(reader["AuthGroupId"]);
                        activeLibraryDCreply.AuthGroupName = Convert.ToString(reader["AuthGroupName"]);
                        activeLibraryDCreply.Category = (reader["Category"] == DBNull.Value) ? Guid.Empty : new Guid(Convert.ToString(reader["Category"]));
                        activeLibraryDCreply.CategoryId = (reader["CategoryId"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["CategoryId"]);
                        activeLibraryDCreply.Description = Convert.ToString(reader["Description"]) ?? string.Empty;
                        activeLibraryDCreply.Guid = (reader["Guid"] == DBNull.Value) ? Guid.Empty : new Guid(Convert.ToString(reader["Guid"]));
                        activeLibraryDCreply.HasActivities = (reader["HasActivities"] == DBNull.Value) ? false : Convert.ToBoolean(reader["HasActivities"]);
                        activeLibraryDCreply.Id = Convert.ToInt32(reader["Id"]);
                        activeLibraryDCreply.ImportedBy = Convert.ToString(reader["ImportedBy"]);
                        activeLibraryDCreply.MetaTags = Convert.ToString(reader["MetaTags"]) ?? string.Empty;
                        activeLibraryDCreply.Name = Convert.ToString(reader["Name"]);
                        activeLibraryDCreply.Status = (reader["Status"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Status"]);
                        activeLibraryDCreply.StatusName = Convert.ToString(reader["StatusName"]);
                        activeLibraryDCreply.VersionNumber = Convert.ToString(reader["VersionNumber"]);
                        activeLibraryDCreply.FriendlyName = Convert.ToString(reader["FriendlyName"]);
                        activeLibraryDCreply.ReleaseNotes = Convert.ToString(reader["ReleaseNotes"]);
                        activeLibraryDCreply.HasExecutable = reader["Executable"] != DBNull.Value;
                        activeLibraryDCreply.Environment = Convert.ToString(reader[DataColumnNames.Environment]);
                        if (includeDll)
                        {
                            activeLibraryDCreply.Executable = (reader["Executable"] == DBNull.Value) ? null : (byte[])reader["Executable"];
                        }

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

        /// <summary>
        /// Get a list of activity libraries that don't exist in the repository based on an initial list to check.
        /// </summary>
        /// <param name="request">ActivityLibraryDC</param>
        /// <returns>List of activity libraries.</returns>
        public static List<ActivityLibraryDC> GetMissingActivityLibraries(GetMissingActivityLibrariesRequest request)
        {
            var resultCollection = new List<ActivityLibraryDC>();
            var status = new StatusReplyDC();
            Database database = null;

            try
            {
                database = DatabaseFactory.CreateDatabase();

                using (var connection = new SqlConnection(database.ConnectionString))
                {
                    using (var command = new SqlCommand(StoredProcNames.ActivityLibraryGetMissing, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //Create a table
                        var table = new DataTable("temp");
                        var nameColumn = new DataColumn("Name", typeof(string));
                        var versionColumn = new DataColumn("Version", typeof(string));

                        table.Columns.Add(nameColumn);
                        table.Columns.Add(versionColumn);

                        //Populate the table
                        foreach (var assemblyItem in request.ActivityLibrariesList)
                        {
                            var rowValues = new object[2];
                            rowValues[0] = assemblyItem.Name;
                            rowValues[1] = assemblyItem.VersionNumber;
                            table.Rows.Add(rowValues);
                        }

                        //Send the table-valued parameter to the stored proc
                        SqlParameter parameter = command.Parameters.AddWithValue("@inActivityLibraries", table);
                        parameter.SqlDbType = SqlDbType.Structured;
                        parameter.TypeName = "[dbo].[ActivityLibraryTableType]";

                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        //Read results from stored proc
                        while (reader.Read())
                        {
                            ActivityLibraryDC activityLibrary =
                                request.ActivityLibrariesList.Find(item =>
                                                                   item.Name == reader.GetString(0) &&
                                                                   item.VersionNumber == reader.GetString(1));
                            if (activityLibrary != null)
                            {
                                resultCollection.Add(activityLibrary);
                            }
                        }

                        reader.Close();
                    }

                }
            }
            catch (SqlException ex)
            {
                ex.HandleException();
            }

            return resultCollection;
        }
    }
}
