using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using CWF.DataContracts;
using System.Data;
using System.Runtime.Serialization;
using System.Reflection;

namespace CWF.DAL
{
    public static class DAL_Helper
    {
        public static object[] FillBySprocName<T>(string sprocName) where T : new()
        {
            StatusReplyDC StatusReply = new StatusReplyDC();
            Database db = null;
            DbCommand cmd = null;
            List<T> wfhList = new List<T>();
            T wfh;
            Type inf = typeof(T);
            PropertyInfo[] properties = inf.GetProperties();

            try
            {
                db = DatabaseFactory.CreateDatabase();
                cmd = db.GetStoredProcCommand("[dbo].[ps_WorkflowHeaderGet]");
                db.AddParameter(cmd, "@inCaller", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, Constants.Constants.DAL_CALLER_INFO);
                db.AddParameter(cmd, "@inCallerVersion", DbType.String, ParameterDirection.Input, null, DataRowVersion.Default, Constants.Constants.DAL_CALLER_VERSION);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        wfh = new T();
                        foreach (PropertyInfo property in properties)
                        {
                            if (property.GetCustomAttributes(typeof(DataMemberAttribute), false).Length == 0)
                                continue;
                            property.SetValue(wfh, Convert.ChangeType(reader[property.Name], property.PropertyType), null);
                        }
                        wfhList.Add(wfh);
                    }
                }
            }
            catch (Exception ex)
            {
                string guid = Guid.NewGuid().ToString();
                StatusReply.Errorcode = -1;
                StatusReply.ErrorGuid = guid;
                string title = ex.Message + "@" + ex.Source + "(" + ex.TargetSite + ")";
                StatusReply.ErrorMessage = "call on ps_WorkflowHeaderGet failed";
                etblErrorLogWriteRequestDC logRequest = Logging.EtblerrorLogHelper(ex.Message, title, 1);
                StatusReply.ErrorGuid = logRequest.ErrorGuid;
                Logging.etblErrorLogWrite(logRequest);
            }
            return new object[] { StatusReply, wfhList };
        }
    }
}