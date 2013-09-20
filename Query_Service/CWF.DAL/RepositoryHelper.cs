﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using CWF.DataContracts;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    public static class RepositoryHelper
    {
        public static DataTable GetAuthGroupName(string[] authGroups)
        {
            return GetTable(authGroups, "Name");
        }

        public static DataTable GetEnvironments(string[] enviroments)
        {
            return GetTable(enviroments, "Name");
        }

        private static DataTable GetTable(string[] array, string colName)
        {
            //Create a table
            var table = new DataTable("Temp");
            var nameColumn = new DataColumn(colName, typeof(string));
            table.Columns.Add(nameColumn);
            //Populate the table            
            array.ToList().ForEach(g => table.Rows.Add(g));
            return table;
        }

        public static SqlDatabase CreateDatabase()
        {
            return DatabaseFactory.CreateDatabase() as SqlDatabase;
        }
    }
}