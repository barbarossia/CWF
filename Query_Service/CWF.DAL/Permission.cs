using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using CWF.DataContracts;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    public static class Permission
    {
        /// <summary>
        /// If the request parameters other than @InCaller & @InCallerversion are null, returns a list of entries, otherwise returns a list of one entry.
        /// </summary>
        /// <param name="request">StoreActivitiesDC</param>
        /// <returns>List<StoreActivitiesDC></returns>
        public static PermissionGetListReply PermissionGet(RequestHeader request)
        {
            PermissionGetListReply reply = new PermissionGetListReply();
            List<PermissionGetReplyDC> list = new List<PermissionGetReplyDC>();
            string outErrorString = string.Empty;
            PermissionGetReplyDC sab = null;
            Database db = null;
            DbCommand cmd = null;
            int retValue = 0;
            StatusReplyDC status = new StatusReplyDC();

            reply.List = list;
            reply.StatusReply = status;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = db.GetStoredProcCommand(StoredProcNames.PermissionGet);
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        sab = new PermissionGetReplyDC();
                        sab.AuthorGroupName = Convert.ToString(reader["AuthorGroupName"]);
                        sab.EnvironmentName = Convert.ToString(reader["EnvironmentName"]);                        
                        sab.Permission = Convert.ToInt64(reader["Permission"]);
                        list.Add(sab);
                    }

                    retValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                    outErrorString = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                    if (retValue != 0)
                    {
                        status.ErrorMessage = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                        status.Errorcode = retValue;
                        Logging.Log(retValue,
                                    EventLogEntryType.Error,
                                    SprocValues.STORE_PERMISSION_GET_CALL_ERROR_MSG,
                                    outErrorString);
                    }
                }
            }
            catch (Exception ex)
            {
                status = Logging.Log(SprocValues.GENERIC_CATCH_ID,
                                     EventLogEntryType.Error,
                                     SprocValues.STORE_PERMISSION_GET_CALL_ERROR_MSG,
                                     ex);
            }

            return reply;
        }
    }
}
