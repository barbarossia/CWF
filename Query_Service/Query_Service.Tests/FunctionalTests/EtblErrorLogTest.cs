//-----------------------------------------------------------------------
// <copyright file="EtblErrorLogTest.cs" company="Microsoft">
// Copyright
// A test class used to verify service interaction with etblErrorLog table.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Management;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts;
using Query_Service.Tests.Common;

namespace Query_Service.Tests
{
    /// <summary>
    /// A test class used to verify service interaction with etblErrorLog table.
    /// </summary>
    [TestClass]
    public class EtblErrorLogTest : QueryServiceTestBase
    {
        DateTime startTime;

        #region constants

        private const string MACHINE_NAME = "TSEDEV-SS3PC4";
        private const string MESSAGE = "Test error log from TEST harness";
        private const string PROCESS_ID = "4235";
        private const string PROCESS_NAME = "TESTHarness.TESTetblErrorLogWrite";
        private const string SEVERITY = "1";
        private const string THREADNAME = "Thread Name";
        private const string TITLE = "Title";
        private const string WIN32THREADID = "Win32ThreadId";
        private const string LOGNAME = "Application";
        private const string SOURCE = "CWF.WorkflowsService";
        private const string SERVER = "pqocwfddb01";

        #endregion

        #region Request Object(s)

        private ErrorLogWriteRequestDC writeRequest;

        #endregion

        #region Reply Object(s)

        private ErrorLogWriteReplyDC writeReply;

        #endregion

        #region Private Methods

        /// <summary>
        /// Check Application in the EventViewer for CWF.WorkflowsService
        /// </summary>
        private void CheckEventLogs(string expectedMessage)
        {
            bool foundEntry = false;
            string message = string.Empty;
            try
            {
                if (!EventLog.SourceExists(LOGNAME, SERVER))
                {
                    EventSourceCreationData mySourceData = new EventSourceCreationData("", "");
                    mySourceData.LogName = LOGNAME;
                    mySourceData.MachineName = SERVER;
                }

                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog(LOGNAME, SERVER, SOURCE);
                myLog.Source = LOGNAME;

                EventLogPermission eventLogPermission = new EventLogPermission(EventLogPermissionAccess.Administer, SERVER);

                int count = myLog.Entries.Count;

                // start from the most recent entry
                for(int i = count - 1; i >= 0; i--)
                {
                    if (myLog.Entries[i].TimeWritten.CompareTo(startTime.AddSeconds(-1)) >= 0)
                    {
                        if (myLog.Entries[i].Source == SOURCE)
                        {
                            message = myLog.Entries[i].Message;
                            foundEntry = true;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to access the EventViewer on {0}. Exception: {1}", SERVER, ex.Message);
            }

            Assert.IsTrue(foundEntry, "Did not find the expected entry in the EventViewer on {0}, server");
            Assert.IsTrue(message.Contains(expectedMessage)); 
        }

        #endregion

        [WorkItem(21023)]
        [Description("Verify Write to Error Log Test")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyErrorLogWriteTest()
        {
            string expectedMessage = "Record # in etblErrorLog = ";
            startTime = DateTime.Now;

            writeRequest = new ErrorLogWriteRequestDC();
            writeReply = null;

            writeRequest.AppDomainName = string.Empty;
            writeRequest.ErrorGuid = Guid.NewGuid().ToString();
            writeRequest.EventId = 1;
            writeRequest.FormattedMessage = string.Empty;
            writeRequest.MachineName = MACHINE_NAME;
            writeRequest.Message = MESSAGE;
            writeRequest.Priority = 1;
            writeRequest.ProcessId = PROCESS_ID;
            writeRequest.ProcessName = PROCESS_NAME;
            writeRequest.Severity = SEVERITY;
            writeRequest.ThreadName = THREADNAME;
            writeRequest.Timestamp = DateTime.Now;
            writeRequest.Title = TITLE;
            writeRequest.Win32ThreadId = WIN32THREADID;
            writeRequest.IncallerVersion = IN_CALLER_VERSION;
            writeRequest.Incaller = IN_CALLER;

            try
            {
                writeReply = proxy.ErrorLogWrite(writeRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to write data to etblErrorLog: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to write data to etblErrorLog: {0}", ex.Message);
            }

            Assert.IsNotNull(writeReply);
            //Assert.AreEqual(0, writeReply.StatusReply.Errorcode); //Bug 15784  We need to separate the errorCode from etblErrorLogRowNo because if ps_etblErrorLogWrite executes successfully, it does not return 0, rather the id of the row created.

            ErrorLogGet(writeReply.EtblErrorLogRowNo, writeRequest);
            CheckEventLogs(expectedMessage);
        }

        [WorkItem(22206)]
        [Description("Verify Write to Event Viewer")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyWriteToEventViewer()
        {
            string expectedMessage = "Specified cast is not valid.";
            startTime = DateTime.Now;

            writeRequest = new ErrorLogWriteRequestDC();
            writeRequest.Timestamp = DateTime.Now;
            // Not filling in the writeRequest will cause the error to be not written to the etblErrorLog table, instead sent to the EventViewer on pqocwfddb01
            writeReply = null;

            try
            {
                writeReply = proxy.ErrorLogWrite(writeRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to write data to the EventViewer: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to write data to the EventViewer: {0}", ex.Message);
            }

            Assert.IsNotNull(writeReply, "writeReply is null");
            Assert.IsNotNull(writeReply.StatusReply, "writeReply.StatusReply is null");
            Assert.AreEqual(CWF.Constants.SprocValues.GENERIC_CATCH_ID, writeReply.StatusReply.Errorcode, "StatusReply returned the wrong error code. It returned {0} but should have returned -2", writeReply.StatusReply.Errorcode);
            Assert.AreEqual(expectedMessage, writeReply.StatusReply.ErrorMessage, "writeReply.StatusReply.ErrorMessage returned the wrong error message");
            CheckEventLogs(expectedMessage);
        }
    }
}
