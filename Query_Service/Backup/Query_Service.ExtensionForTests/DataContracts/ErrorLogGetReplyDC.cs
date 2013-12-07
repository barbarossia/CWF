//-----------------------------------------------------------------------
// <copyright file="ErrorLogGetReplyDC.cs" company="Microsoft">
// Copyright
// Reply data contract for ErrorLogGet.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Query_Service.ExtensionForTests.DataProxy
{
    /// <summary>
    /// Reply data contract for ErrorLogGet
    /// Status in ReplyHeader
    /// </summary>
    [DataContract]
    public class ErrorLogGetReplyDC : ReplyHeader
    {
        private int priority;
        private string severity;
        private string title;
        private DateTime timestamp;
        private string machineName;
        private string appDomainName;
        private string processId;
        private string processName;
        private string threadName;
        private string win32ThreadId;
        private string message;
        private string formattedMessage;

        /// <summary>
        /// Prioirty of error log record
        /// </summary>
        [DataMember]
        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        /// <summary>
        /// Severity of error log record
        /// </summary>
        [DataMember]
        public string Severity
        {
            get { return severity; }
            set { severity = value; }
        }

        /// <summary>
        /// Title of error log record
        /// </summary>
        [DataMember]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// Timestamp of error log record
        /// </summary>
        [DataMember]
        public DateTime Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        /// <summary>
        /// Machine name of error log record
        /// </summary>
        [DataMember]
        public string MachineName
        {
            get { return machineName; }
            set { machineName = value; }
        }

        /// <summary>
        /// App domain name of error log record
        /// </summary>
        [DataMember]
        public string AppDomainName
        {
            get { return appDomainName; }
            set { appDomainName = value; }
        }

        /// <summary>
        /// Process ID of error log record
        /// </summary>
        [DataMember]
        public string ProcessId
        {
            get { return processId; }
            set { processId = value; }
        }

        /// <summary>
        /// Process name of error log record
        /// </summary>
        [DataMember]
        public string ProcessName
        {
            get { return processName; }
            set { processName = value; }
        }

        /// <summary>
        /// Thread name of error log record
        /// </summary>
        [DataMember]
        public string ThreadName
        {
            get { return threadName; }
            set { threadName = value; }
        }

        /// <summary>
        /// Win32 thread Id of error log record
        /// </summary>
        [DataMember]
        public string Win32ThreadId
        {
            get { return win32ThreadId; }
            set { win32ThreadId = value; }
        }

        /// <summary>
        /// Message of error log record
        /// </summary>
        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        /// <summary>
        /// Formatted message of error log record
        /// </summary>
        [DataMember]
        public string FormattedMessage
        {
            get { return formattedMessage; }
            set { formattedMessage = value; }
        }
    }
}