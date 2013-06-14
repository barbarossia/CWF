//-----------------------------------------------------------------------
// <copyright file="Dal.cs" company="Microsoft">
// Copyright
// DAL methods to call stored procedures for testing purposes
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;
using System.Data;
using System.Configuration;

using Query_Service.ExtensionForTests.DataProxy;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Query_Service.ExtensionForTests.DAL
{
    /// <summary>
    /// This class encapsulates the data access layer for accessing stored procedures for testing purposes
    /// </summary>
    public class DAL
    {
        // list of (non-live) test databases that we can use the delete method on
        static string[] databases = { "NEW3PrototypeAssetStoreTest" }; 

        /// <summary>
        /// Put the soft delete back in so other tests won't be affected
        /// </summary>
        /// <param name="request">UpdateSoftDeleteRequestDC contains the id and table</param>
        /// <returns>StatusReplyDC contains information about the database command execution</returns>
        public static StatusReplyDC UpdateSoftDelete(UpdateSoftDeleteRequestDC request)
        {
            Database db = null;
            DbCommand cmd = null;
            StatusReplyDC statusReply = new StatusReplyDC();
            int retValue = 0;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = db.GetStoredProcCommand(StoredProcNames.TableRowUpdateSoftDelete);
                db.AddParameter(cmd, "@inId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.Id);
                db.AddParameter(cmd, "@tableName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.TableName);

                retValue = db.ExecuteNonQuery(cmd);

                if (retValue < 0)
                {
                    statusReply.ErrorMessage = "Update softdelete wasn't succesful.";
                }

                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                statusReply.ErrorMessage = String.Format("DAL.UpdateSoftDelete.ps_{0} call. Error code: {1}. Error message: {2}", request.TableName, -99, ex.Message);
            }

            return statusReply;
        }

        /// <summary>
        /// HARD deletes row given an id and name and database
        /// Use this method with caution
        /// </summary>
        /// <param name="id">The id of the row to be deleted</param>
        /// <param name="table">The table of the row to be deleted</param>
        /// <param name="database">The database of the row to be deleted. Database has to be in the list of test databases</param>
        /// <returns>StatusReplyDC contains information about the database command execution</returns>
        public static StatusReplyDC DeleteFromId(int id, string table, string database)
        {
            StatusReplyDC statusReply = new StatusReplyDC();
            Database db = null;
            DbCommand cmd = null;
            int retValue = 0;

            string webConfigDatabaseName = GetDatabaseNameFromWebConfig();

            if (database != webConfigDatabaseName)
            {
                statusReply.Errorcode = -99;
                statusReply.ErrorMessage = string.Format("The database: {0} does not match the database in web.config: {1}", database, webConfigDatabaseName);
                return statusReply;
            }

            // Don't delete from a database that's not in the list above
            if (!databases.Contains(database))
            {
                statusReply.Errorcode = -99;
                statusReply.ErrorMessage = string.Format("Not able to delete from this database: {0}", database);
                return statusReply;
            }

            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = db.GetStoredProcCommand(StoredProcNames.TableRowDeleteById);

                db.AddParameter(cmd, "@InId", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, id);
                db.AddParameter(cmd, "@tableName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, table);

                retValue = db.ExecuteNonQuery(cmd);

                if (retValue < 0)
                {
                    statusReply.ErrorMessage = "Delete wasn't successful.";
                }

                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                statusReply.ErrorMessage = String.Format("DAL.DeleteFromId.ps_DeleteFromId call. Error code: {0}. Error message: {1}", -99, ex.Message);
            }

            return statusReply;
        }

        /// <summary>
        /// HARD clear lock for given name and version in databas,
        /// so that other test cases can use the same test data.
        /// </summary>
        /// <param name="request">ClearLockRequestDC contains the name and version of activity</param>
        /// <returns>StatusReplyDC contains information about the database command execution</returns>
        public static StatusReplyDC ClearLock(ClearLockRequestDC request)
        {
            Database db = null;
            string cmd = null;
            StatusReplyDC statusReply = new StatusReplyDC();
            int retValue = 0;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = string.Format(@"UPDATE dbo.Activity SET Locked=0, LockedBy=Null WHERE Name='{0}' AND Version = '{1}'", request.Name, request.Version);

                retValue = db.ExecuteNonQuery(CommandType.Text, cmd);

                if (retValue < 0)
                {
                    statusReply.ErrorMessage = "Update lock fields were not succesful.";
                }
            }
            catch (Exception ex)
            {
                statusReply.ErrorMessage = String.Format("DAL.ClearLock call. Error code: {0}. Error message: {1}", -99, ex.Message);
            }

            return statusReply;
        }
        
        /// <summary>
        /// Parses the database name from the web.config file
        /// </summary>
        /// <returns>database name</returns>
        private static string GetDatabaseNameFromWebConfig()
        {
            string databaseName = string.Empty;

            try
            {
                Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                ConnectionStringSettings connString = config.ConnectionStrings.ConnectionStrings["ApplicationDbConnection"];
                string sConnection = connString.ConnectionString;
                string[] keyVals = sConnection.Split(';', '=');
                databaseName = keyVals[3];
                // We are parsing something of the form Data Source=PqoCwfddb02;Init1ial Catalog=NEW3PrototypeAssetStoreDevTest;Integrated Security=True;Min Pool Size=10;Connection Lifetime=1200;Connect Timeout=30;
            }
            catch
            {
                databaseName = "";
            }

            return databaseName;
        }
    }
}