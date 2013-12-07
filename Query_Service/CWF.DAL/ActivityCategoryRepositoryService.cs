using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DataContracts;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using CWF.DAL;
using System.Diagnostics;
using System.Data.SqlClient;

namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    /// <summary>
    /// Defines the data access services associated with ActivityCategory table as the primary table.
    /// </summary>
    public static class ActivityCategoryRepositoryService
    {
        /// <summary>
        /// Gets activity categories by ID, Name or by ID & Name.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>List of activity categories found.</returns>
        public static List<ActivityCategoryByNameGetReplyDC> GetActivityCategories(ActivityCategoryByNameGetRequestDC request)
        {
            List<ActivityCategoryByNameGetReplyDC> reply = null;           
          
            try
            {
                Database database = DatabaseFactory.CreateDatabase();
                DbCommand command = RepositoryHelper.PrepareCommandCommand(database, StoredProcNames.ActivityCategoryGet);

                database.AddParameter(command, StoredProcParamNames.Id, DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.InId);
                database.AddParameter(command, StoredProcParamNames.Name, DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InName);

                using (IDataReader reader = database.ExecuteReader(command))
                {
                    ActivityCategoryByNameGetReplyDC activityCategoryEntry = null;
                    reply = new List<ActivityCategoryByNameGetReplyDC>();
                    while (reader.Read())
                    {
                        activityCategoryEntry = new ActivityCategoryByNameGetReplyDC();
                        activityCategoryEntry.Guid = new Guid(Convert.ToString(reader[DataColumnNames.Guid]));
                        activityCategoryEntry.Description = Convert.ToString(reader[DataColumnNames.Description]) ?? string.Empty;
                        activityCategoryEntry.Id = Convert.ToInt32(reader[DataColumnNames.Id]);
                        activityCategoryEntry.MetaTags = Convert.ToString(reader[DataColumnNames.MetaTags]) ?? string.Empty;
                        activityCategoryEntry.Name = Convert.ToString(reader[DataColumnNames.Name]);
                        activityCategoryEntry.AuthGroupId = Convert.ToInt32(reader[DataColumnNames.AuthGroupId]);
                        activityCategoryEntry.AuthGroupName = Convert.ToString(reader[DataColumnNames.AuthGroupName]);
                        reply.Add(activityCategoryEntry);
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
