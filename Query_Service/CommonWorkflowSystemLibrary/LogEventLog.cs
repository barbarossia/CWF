using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace Microsoft.Support.Workflow.CWFHelpers
{
    public static class LogEventLog
    {
      
        /// <summary>
        /// Logs message to event log
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="EntryType"></param>
        public static void Log2EventLog(string Message, System.Diagnostics.EventLogEntryType EntryType)
        {
            System.Diagnostics.EventLog.WriteEntry("CWF2", Message ,  EntryType);


        }

        public static void Log2EventLog(string Message, object Object, System.Diagnostics.EventLogEntryType EntryType)
        {
            string sm;
            try
            {
                DataContractSerializer dcs = new DataContractSerializer(Object.GetType());
                MemoryStream ms = new MemoryStream();

                dcs.WriteObject(ms, Object);

                StreamReader sr = new StreamReader(ms);
                sm = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                string tm = "Error trying to serialize object in Log2EventLog: " + ex.Message;
                System.Diagnostics.EventLog.WriteEntry("CWF2", tm, System.Diagnostics.EventLogEntryType.Error);
                sm = "object not serialized";
            }
            string tm2 = Message + "\r\n Object: \r\n" + sm;

        }

    }
}
