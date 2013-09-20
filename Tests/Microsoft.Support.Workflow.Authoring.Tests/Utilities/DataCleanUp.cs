using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;


namespace Microsoft.Support.Workflow.Authoring.Tests
{
    public class DataCleanUp
    {
        static string dataBaseName = ConfigurationManager.AppSettings["DataBaseName"];
        static string connectionTemplete = ConfigurationManager.AppSettings["ApplicationDbConnection"];
        public static string connectionString = string.Format(connectionTemplete, dataBaseName);
        public static string StoreActivity = "DELETE FROM [{0}].[dbo].[Activity] WHERE Name = '{1}'";
        public static string ActivityLibraries = "DELETE FROM [{0}].[dbo].[ActivityLibrary] WHERE Name = '{1}'";
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSetName"></param>
        /// <param name="srcTable"> </param>
        /// <param name="queryCmd"></param>
        /// <returns></returns>
        public static DataSet ExecuteSqlQuery(string queryCmd, string formatReplace)
        {
            string srcTable = "defaultSrc";
            var resultDataSet = new DataSet();
            queryCmd = string.Format(queryCmd, dataBaseName, formatReplace);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = queryCmd;
                    if (command.Connection.State == ConnectionState.Closed)
                    {
                        command.Connection.Open();
                    }
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        if (!string.IsNullOrEmpty(srcTable)) { adapter.Fill(resultDataSet, srcTable); }
                        else { adapter.Fill(resultDataSet); }

                        //Close connection [when done]
                        if (command.Connection.State == ConnectionState.Open)
                            command.Connection.Close();
                    }
                }
            }
            return resultDataSet;
        }

    }
}
