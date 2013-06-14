namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using CWF.DAL;
    using CWF.DataContracts;
    using Practices.EnterpriseLibrary.Data;
    using Microsoft.Support.Workflow.Service.Contracts;
    using System.Data.SqlClient;

    public class AuthorizationGroup
    {
        /// <summary>
        /// Get AuthorizationGroup
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static AuthorizationGroupGetReplyDC GetAuthorizationGroups(AuthorizationGroupGetRequestDC request)
        {
            AuthorizationGroupGetReplyDC reply = new AuthorizationGroupGetReplyDC();
            IList<AuthorizationGroupDC> agList = new List<AuthorizationGroupDC>();
            try
            {
                AuthorizationGroupDC ag = null;

                Database db = DatabaseFactory.CreateDatabase();
                DbCommand cmd = db.GetStoredProcCommand(StoredProcNames.AuthorizationGroupGet);
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.AuthGroupId);
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.AuthGroupName);
                db.AddParameter(cmd, "@InGuid", DbType.Guid, ParameterDirection.Input, null, DataRowVersion.Default, request.Guid);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        ag = new AuthorizationGroupDC();
                        ag.Guid = (Guid)reader[DataColumnNames.Guid];
                        ag.AuthGroupId = reader[DataColumnNames.Id] != DBNull.Value
                            ? Convert.ToInt32(reader[DataColumnNames.Id]) : 0;
                        ag.AuthGroupName = reader[DataColumnNames.Name] != DBNull.Value
                            ? Convert.ToString(reader[DataColumnNames.Name]) : string.Empty;
                        agList.Add(ag);
                    }
                }
                reply.AuthorizationGroups = agList;
            }
            catch (SqlException e)
            {
                e.HandleException();
            }

            return reply;
        }
    }
}
