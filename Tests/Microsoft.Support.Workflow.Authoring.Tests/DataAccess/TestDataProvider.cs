using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.Support.Workflow.Authoring.Tests.DataAccess
{
    /// <summary>
    /// TestDataProvider which access the database and perform queries
    /// </summary>
    public class TestDataProvider
    {
        private const string ConnectionStringFormat =
            "Data Source={0};Initial Catalog={1};User Id={2};Password={3};Integrated Security=SSPI;";

        /// <summary>
        /// Returns a SQL connection for the given database.
        /// </summary>
        public static SqlConnection GetConnectionForDb()
        {
            return new SqlConnection(GetConnectionString());
        }

        /// <summary>
        /// Run the given SQL command against the specified database.
        /// Does not return results.
        /// </summary>
        public static void ExecuteNonQuery(string sql)
        {
            ExecuteNonQuery(sql, null);
        }

        /// <summary>
        /// Run the given SQL command against the specified database.
        /// Does not return results.
        /// 
        /// Use this if there is only one parameter input for the given sql.
        /// </summary>
        public static void ExecuteNonQuery(
           string sql,
           string paramName,
           string paramValue)
        {
            ExecuteNonQuery(sql, GetCommandParams(paramName, paramValue));
        }

        /// <summary>
        /// Run the given SQL command against the specified database.
        /// Does not return results.
        /// </summary>
        public static void ExecuteNonQuery(
            string sql,
            IDictionary<string, object> parameters)
        {
            VerifyInput(sql, parameters);

            using (SqlConnection connection = GetConnectionForDb())
            {
                SqlCommand command = BuildSqlCommand(sql, connection, parameters);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Run the given SQL query against the specified database. 
        /// Will return only a single value from the database
        /// </summary>
        public static object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, null);
        }

        /// <summary>
        /// Run the given SQL query against the specified database. 
        /// Will return only a single value from the database.
        /// 
        /// Use this if there is only one parameter input for the given sql.
        /// </summary>
        public static object ExecuteScalar(
           string sql,
           string paramName,
           string paramValue)
        {
            return ExecuteScalar(sql, GetCommandParams(paramName, paramValue));
        }

        /// <summary>
        /// Run the given SQL query against the specified database. 
        /// Will return only a single value from the database
        /// </summary>
        public static object ExecuteScalar(
            string sql,
            IDictionary<string, object> parameters)
        {
            VerifyInput(sql, parameters);

            object result = null;

            using (SqlConnection connection = GetConnectionForDb())
            {
                SqlCommand command = BuildSqlCommand(sql, connection, parameters);
                connection.Open();
                result = command.ExecuteScalar();
            }

            return result;
        }

        /// <summary>
        /// Return a DataTable representing the results of the SQL Select String
        /// </summary>
        public static DataTable ExecuteSelect(
            string sql)
        {
            return ExecuteSelect(sql, null);
        }

        /// <summary>
        /// Return a DataTable representing the results of the SQL Select String
        /// 
        /// Use this if there is only one parameter input for the given sql.
        /// </summary>
        public static DataTable ExecuteSelect(
           string sql,
           string paramName,
           string paramValue)
        {
            return ExecuteSelect(sql, GetCommandParams(paramName, paramValue));
        }

        /// <summary>
        /// Return a DataTable representing the results of the SQL Select String
        /// </summary>
        public static DataTable ExecuteSelect(
            string sql,
            IDictionary<string, object> parameters)
        {
            VerifyInput(sql, parameters);

            DataTable table = null;

            using (SqlConnection connection = GetConnectionForDb())
            {
                SqlCommand command = BuildSqlCommand(sql, connection, parameters);
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                table = new DataTable("MyDataTable");
                adapter.Fill(table);
            }

            return table;
        }

        /// <summary>
        /// Use this method to verify results from the result of ExecuteSelect
        /// </summary>
        public static bool VerifyValuesInResult(DataTable dataTableResult, string columnName, params string[] values)
        {
            if (dataTableResult == null)
            {
                return false;
            }

            if (String.IsNullOrEmpty(columnName))
            {
                throw new ArgumentNullException("Column name cannot be null or empty.");
            }

            if (values != null && values.Length > 0)
            {
                foreach (string value in values)
                {
                    bool result = IsColumnValueMatches(dataTableResult, columnName, value);

                    if (!result)
                    {
                        return result;
                    }
                }
            }

            return true;
        }

        private static void VerifyInput(string sql, IDictionary<string, object> commandParams)
        {
            if (String.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("Sql statement cannot be null or empty.");
            }

            VerifyParams(sql, commandParams);
        }

        private static void VerifyParams(string sql, IDictionary<string, object> commandParams)
        {
            if (sql.Contains("@") && (commandParams == null || commandParams.Count == 0))
            {
                return;
            }

            string[] sqlTokens = sql.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in sqlTokens)
            {
                if (token.StartsWith("@"))
                {
                    if (!commandParams.Keys.Contains(token.Trim(new char[] { '(', ',', ')', ' ' })))
                    {
                        throw new ArgumentException(token + " parameter input is missing.");
                    }
                }
            }

            return;
        }

        private static bool IsColumnValueMatches(DataTable dataTable, string columnName, string valueToSearchFor)
        {
            bool result = false;

            if (dataTable != null)
            {
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string value = row[columnName] == null ? null : (string)row[columnName];

                        if (String.Equals(valueToSearchFor, value, StringComparison.OrdinalIgnoreCase))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private static string GetConnectionString()
        {
            string serverName = ConfigurationManager.AppSettings["DatabaseServer"];
            string databaseName = ConfigurationManager.AppSettings["DatabaseName"];
            string userName = ConfigurationManager.AppSettings["DatabaseUserName"];
            string password = ConfigurationManager.AppSettings["DatabasePassword"];

            return DataCleanUp.connectionString;
        }

        private static SqlCommand BuildSqlCommand(string sql, SqlConnection connection, IDictionary<string, object> parameters)
        {
            SqlCommand command = new SqlCommand(sql, connection);

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    command.Parameters.Add(
                        new SqlParameter(parameter.Key, parameter.Value ?? DBNull.Value));
                }
            }

            return command;
        }

        private static IDictionary<string, object> GetCommandParams(string paramName, object paramValue)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add(paramName, paramValue);

            return parameters;
        }
    }
}
