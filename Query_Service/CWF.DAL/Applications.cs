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

    public static class Applications
    {
        /// <summary>
        /// If the request parameters other than @InCaller & @InCallerversion are null, returns a list of entries, otherwise returns a list of one entry.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static ApplicationsGetReplyDC ApplicationsGet(ApplicationsGetRequestDC request)
        {
            var reply = new ApplicationsGetReplyDC();
            var status = new StatusReplyDC();
            ApplicationGetBase app = null;
            var list = new List<ApplicationGetBase>();
            Database db = null;
            DbCommand cmd = null;
            int retValue = 0;
            string outErrorString = string.Empty;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = RepositoryHelper.PrepareCommandCommand(db, StoredProcNames.ApplicationGet);
                
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.Incaller);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.IncallerVersion);
                db.AddParameter(cmd, "@InId", DbType.Int32, ParameterDirection.Input, null, DataRowVersion.Default, request.InId);
                db.AddParameter(cmd, "@InName", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, request.InName);
                db.AddParameter(cmd, "@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue, null, DataRowVersion.Default, 0);
                db.AddOutParameter(cmd, "@outErrorString", DbType.String, 300);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        app = new ApplicationGetBase();
                        app.Guid = (Guid)reader["GUID"];
                        app.Description = Convert.ToString(reader["Description"]) ?? string.Empty;
                        app.Id = Convert.ToInt32(reader["Id"]);
                        app.Name = Convert.ToString(reader["Name"]);
                        list.Add(app);
                    }

                    reply.List = list;
                }

                retValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                outErrorString = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                if (retValue != 0)
                {
                    status.ErrorMessage = Convert.ToString(cmd.Parameters["@outErrorString"].Value);
                    status.Errorcode = retValue;
                    Logging.Log(retValue,
                                EventLogEntryType.Error,
                                SprocValues.APPLICATIONS_GET_CALL_ERROR_MSG,
                                outErrorString);
                }
            }
            catch (Exception ex)
            {
                status = Logging.Log(SprocValues.GENERIC_CATCH_ID,
                                     EventLogEntryType.Error,
                                     SprocValues.APPLICATIONS_GET_CALL_ERROR_MSG,
                                     ex);
            }

            reply.StatusReply = status;
            return reply;
        }
    }
}
