﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DataContracts;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    /// <summary>
    /// Defines the data access services associated with ActivityLibraryDependency table as the primary table.
    /// </summary>
    public static class ActivityLibraryDependencyRepositoryService
    {
        /// <summary>
        /// Gets the list of all the dependencies in the dependency hierarchy for an an activity library.
        /// </summary>
        /// <param name="request">Request that defines the root activity library for which the dependencies are to be found.</param>
        /// <returns>Response that contains a list of dependencies.</returns>
        public static List<StoreActivityLibrariesDependenciesDC> GetActivityLibraryDependencyTree(StoreActivityLibrariesDependenciesDC request)
        {
            List<StoreActivityLibrariesDependenciesDC> reply = new List<StoreActivityLibrariesDependenciesDC>();
            StoreActivityLibrariesDependenciesDC dependencies = new StoreActivityLibrariesDependenciesDC();
            dependencies.StoreDependenciesRootActiveLibrary = new StoreDependenciesRootActiveLibrary();
            dependencies.StoreDependenciesDependentActiveLibraryList = new List<StoreDependenciesDependentActiveLibrary>();
            reply.Add(dependencies);

            try
            {
                Database database = DatabaseFactory.CreateDatabase();
                DbCommand command = null;
                command = database.GetStoredProcCommand(StoredProcNames.ActivityLibraryDependencyGetTree);

                database.AddParameter(command, StoredProcParamNames.Name, DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, Convert.ToString(request.StoreDependenciesRootActiveLibrary.ActivityLibraryName));
                database.AddParameter(command, StoredProcParamNames.Version, DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, Convert.ToString(request.StoreDependenciesRootActiveLibrary.ActivityLibraryVersionNumber));

                using (IDataReader reader = database.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        StoreDependenciesDependentActiveLibrary dependentLib = new StoreDependenciesDependentActiveLibrary();
                        dependentLib.ActivityLibraryDependentId = Convert.ToInt32(reader[DataColumnNames.DependentActivityLibraryId]);
                        dependentLib.ActivityLibraryParentId = reader[DataColumnNames.ActivityLibraryId] != DBNull.Value ? Convert.ToInt32(reader[DataColumnNames.ActivityLibraryId]) : 0;
                        dependencies.StoreDependenciesDependentActiveLibraryList.Add(dependentLib);
                    }

                    if (!reader.IsClosed) reader.Close();
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
