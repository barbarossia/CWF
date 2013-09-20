using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DataContracts.Marketplace;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    /// <summary>
    /// Repository of marketplase search
    /// </summary>
    public static class MarketplaceRepositoryService
    {
        private const string Author = "Author";
        private const string Admin = "Admin";
        private static readonly MarketplaceSearchRule[] marketplaceSearchRules = new[] 
            {
                new MarketplaceSearchRule(MarketplaceFilter.None,  Author,  0, false, false ),
                new MarketplaceSearchRule(MarketplaceFilter.Activities,  Author,  1, null, null ),
                new MarketplaceSearchRule(MarketplaceFilter.Projects,  Author,  2, false, false ),
                new MarketplaceSearchRule(MarketplaceFilter.Templates,  Author,  2, false, false ),
                new MarketplaceSearchRule(MarketplaceFilter.PublishingWorkflows,  Author,  2, false, false ),
                new MarketplaceSearchRule(MarketplaceFilter.None,  Admin,  0, null, null ),
                new MarketplaceSearchRule(MarketplaceFilter.Activities,  Admin,  1, null, null ),
                new MarketplaceSearchRule(MarketplaceFilter.Projects,  Admin,  2, null, null ),
                new MarketplaceSearchRule(MarketplaceFilter.Templates,  Admin,  2, true, null ),
                new MarketplaceSearchRule(MarketplaceFilter.PublishingWorkflows,  Admin, 2, null, true)
            };

        /// <summary>
        /// Find Executable items 
        /// Find XAML items 
        /// Union Executable items and XAML items if the union is needed.  
        /// Union happens only when the filter in the UI is set to “All” which means no filtering is applied.  
        /// In all other filter values we either search for XAML items or for Executable items. 
        /// Apply filtering Apply sorting
        /// Select the requested page
        /// </summary>
        /// <param name="request"></param>
        /// <returns>MarketplaceSearchResult object</returns>
        public static MarketplaceSearchResult SearchMarketplace(MarketplaceSearchQuery request)
        {
            MarketplaceSearchResult result = new MarketplaceSearchResult();
            var resultCollection = new List<MarketplaceAsset>();
            MarketplaceAsset sab = null;
            int retValue = 0;
            string outErrorString = string.Empty;

            try
            {
                var spInputParam = ConvertSearchQueryToStoredProc(request);
                if (spInputParam == null)
                {
                    result.StatusReply.ErrorMessage = SprocValues.INVALID_PARMETER_VALUE_MARKETPLACE;
                    result.StatusReply.Errorcode = SprocValues.INVALID_PARMETER_VALUE_INCODE_ID;
                    return result;
                }

                //This is an invalid operation since the author is not allowed to view templates or publishing workflows.
                if (request.UserRole == Author && (request.FilterType == MarketplaceFilter.Templates || request.FilterType == MarketplaceFilter.PublishingWorkflows))
                {
                    result.StatusReply.ErrorMessage = SprocValues.INVALID_PARMETER_VALUE_MARKETPLACE;
                    result.StatusReply.Errorcode = SprocValues.INVALID_PARMETER_VALUE_INCODE_ID;
                    return result;
                }

                if (request.SortCriteria == null)
                    request.SortCriteria = new List<SortCriterion>() { new SortCriterion() { FieldName = "UpdatedDate", IsAscending = false } };

                SqlDatabase database = RepositoryHelper.CreateDatabase();
                DbCommand command = database.GetStoredProcCommand("[dbo].[Marketplace_Search]");

                database.AddParameter(command, "@InAuthGroupName", SqlDbType.Structured, ParameterDirection.Input, null, DataRowVersion.Default, RepositoryHelper.GetAuthGroupName(request.InAuthGroupNames));
                database.AddParameter(command, "@SearchText", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.SearchText);
                database.AddParameter(command, "@AssetType", DbType.Int16, ParameterDirection.Input, null, DataRowVersion.Default, spInputParam.Item1);
                database.AddParameter(command, "@GetTemplates", DbType.Byte, ParameterDirection.Input, null, DataRowVersion.Default, spInputParam.Item2);
                database.AddParameter(command, "@GetPublishingWorkflows", DbType.Byte, ParameterDirection.Input, null, DataRowVersion.Default, spInputParam.Item3);
                database.AddParameter(command, "@PageSize", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.PageSize);
                database.AddParameter(command, "@PageNumber", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.PageNumber);
                database.AddParameter(command, "@SortColumn", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.SortCriteria[0].FieldName);
                database.AddParameter(command, "@SortAscending", DbType.Byte, ParameterDirection.Input, null, DataRowVersion.Default, request.SortCriteria[0].IsAscending);
                database.AddParameter(command, "@FilterOlder", DbType.Byte, ParameterDirection.Input, null, DataRowVersion.Default, request.IsNewest);
                database.AddParameter(command, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                database.AddOutParameter(command, "@outErrorString", DbType.String, 300);

                using (IDataReader reader = database.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        sab = new MarketplaceAsset();
                        sab.Id = Convert.ToInt64(reader["Id"]);
                        sab.Name = Convert.ToString(reader["Name"]);
                        sab.CreatedBy = Convert.ToString(reader["InsertedByUserAlias"]);
                        sab.UpdatedBy = Convert.ToString(reader["UpdatedByUserAlias"]);
                        sab.UpdatedDate = Convert.ToDateTime(reader["UpdatedDateTime"]);
                        sab.Version = Convert.ToString(reader["Version"]);
                        if (Convert.ToString(reader["AssetType"]) == "XAML")
                            sab.AssetType = AssetType.Project;
                        else
                            sab.AssetType = AssetType.Activities;
                        sab.IsTemplate = reader["IsTemplate"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(reader["IsTemplate"]);
                        sab.IsPublishingWorkflow = reader["IsPublishingWorkflow"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(reader["IsPublishingWorkflow"]);
                        sab.Environment = Convert.ToString(reader["Environment"]);
                        resultCollection.Add(sab);
                    }

                    reader.NextResult();
                    if (reader.Read())
                    {
                        result.PageNumber = Convert.ToInt32(reader["PageNumber"]);
                        result.PageCount = Convert.ToInt32(reader["PageCount"]);
                    }

                    if (resultCollection.Any())
                    {
                        result.Items = resultCollection;
                        result.PageSize = request.PageSize;
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

        private static Tuple<int, bool?, bool?> ConvertSearchQueryToStoredProc(MarketplaceSearchQuery request)
        {
            return (from rule in marketplaceSearchRules
                    where rule.FilterType == request.FilterType
                            && rule.UserRole == request.UserRole
                    select new Tuple<int, bool?, bool?>(rule.AssetType, rule.IsGetTemplates, rule.IsGetPublishingWorkflows))
                         .FirstOrDefault();
        }

        /// <summary>
        /// This should return the fields from ActivityLibraries and StoreActivities tables 
        /// to match the MarketplaceAssetDetails data contract
        /// </summary>
        /// <param name="request"></param>
        /// <returns>MarketplaceAssetDetails object</returns>
        public static MarketplaceAssetDetails GetMarketplaceAssetDetails(MarketplaceSearchDetail request)
        {
            MarketplaceAssetDetails reply = null;
            List<ActivityQuickInfo> activities = new List<ActivityQuickInfo>();
            try
            {
                Database database = DatabaseFactory.CreateDatabase();
                DbCommand command = database.GetStoredProcCommand("[dbo].[Marketplace_GetAssetDetails]");
                database.AddParameter(command, StoredProcParamNames.Id, DbType.Int64, ParameterDirection.Input, null, DataRowVersion.Default, request.Id);
                database.AddParameter(command, "@AssetType", DbType.Int16, ParameterDirection.Input, null, DataRowVersion.Default, request.AssetType);

                using (IDataReader reader = database.ExecuteReader(command))
                {
                    if (reader.Read())
                    {
                        reply = new MarketplaceAssetDetails();
                        reply.Id = Convert.ToInt64(reader["Id"]);
                        reply.Name = Convert.ToString(reader["Name"]);
                        reply.Description = Convert.ToString(reader["Description"]);
                        reply.MetaTages = Convert.ToString(reader["MetaTags"]);
                        reply.CategoryName = Convert.ToString(reader["CategoryName"]);
                        reply.ThumbnailUrl = Convert.ToString(reader["ThumbnailUrl"]);
                        if (request.AssetType == AssetType.Activities)
                        {
                            reader.NextResult();
                            while (reader.Read())
                            {
                                ActivityQuickInfo activity = new ActivityQuickInfo();
                                activity.Version = Convert.ToString(reader["Version"]);
                                activity.Id = Convert.ToInt64(reader["ActivityId"]);
                                activity.Name = Convert.ToString(reader["ActivityName"]);
                                activities.Add(activity);
                            }
                            reply.Activities = activities;
                        }
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
