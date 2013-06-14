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

    public static class StatusCode
    {
        /// <summary>
        /// If the request parameters other than @InCaller & @InCallerversion are null, returns a list of entries, otherwise returns a list of one entry.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static StatusCodeGetReplyDC StatusCodeGet(StatusCodeGetRequestDC request)
        {
            var reply = new StatusCodeGetReplyDC();
            var status = new StatusReplyDC();
            IList<StatusCodeAttributes> list = new List<StatusCodeAttributes>();
            StatusCodeAttributes sa = null;
            int retValue = 0;
            string outErrorString = string.Empty;
            Database db = null;
            DbCommand cmd = null;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = db.GetStoredProcCommand(StoredProcNames.StatusCodeGet);
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@inCode", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.InCode);
                db.AddParameter(cmd, "@inName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InName);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        sa = new StatusCodeAttributes();
                        sa.Description = Convert.ToString(reader["Description"]) ?? string.Empty;
                        if (reader["IsDeleted"] == DBNull.Value)
                            sa.IsDeleted = false;
                        else
                            sa.IsDeleted = Convert.ToBoolean(reader["IsDeleted"]);
                        if (reader["IsEligibleForCleanUp"] == DBNull.Value)
                            sa.IsEligibleForCleanUp = false;
                        else
                            sa.IsEligibleForCleanUp = Convert.ToBoolean(reader["IsEligibleForCleanUp"]);
                        sa.LockForChanges = Convert.ToBoolean(reader["LockForChanges"]);
                        sa.Name = Convert.ToString(reader["Name"]);
                        sa.Code = Convert.ToInt32(reader["Code"]);
                        sa.ShowInProduction = Convert.ToBoolean(reader["ShowInProduction"]);
                        list.Add(sa);
                    }
                }

                retValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                outErrorString = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                if (retValue != 0)
                {
                    status.ErrorMessage = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                    status.Errorcode = retValue;
                    Logging.Log(retValue,
                                EventLogEntryType.Error,
                                SprocValues.STATUS_CODE_GET_CALL_ERROR_MSG,
                                outErrorString);
                }
            }
            catch (Exception ex)
            {
                status = Logging.Log(SprocValues.GENERIC_CATCH_ID,
                                     EventLogEntryType.Error,
                                     SprocValues.STATUS_CODE_GET_CALL_ERROR_MSG,
                                     ex);
            }

            reply.StatusReply = status;
            reply.List = list;
            return reply;
        }
    }
}
