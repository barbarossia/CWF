//-----------------------------------------------------------------------
// <copyright file="LtblStatusCodesTest.cs" company="Microsoft">
// Copyright
// A test class used to verify service interaction with ltblStatusCodes table.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using CWF.DataContracts;
using Query_Service.Tests.Common;
using Microsoft.Support.Workflow.Service.Test.Common;

namespace Query_Service.Tests
{
    /// <summary>
    /// A test class used to verify service interaction with ltblStatusCodes table
    /// </summary>
    [TestClass]
    public class LtblStatusCodesTest : QueryServiceTestBase
    {
         private const string TABLE_NAME = "ltblStatusCodes";
        private const string STATUSCODE_NAME = "Public";

        private StatusCodeGetRequestDC getRequest;
        private StatusCodeGetReplyDC getReply;

        /// <summary>
        /// Verify GET FROM ltblStatusCodes Table for Valid IDs
        /// </summary>
        /// <param name="softDeletedIncode">incode of row to do a get on</param>
        private void GetStatusCodeForValidIncode(int inCode)
        {
            getRequest = new StatusCodeGetRequestDC();
            getReply = null;

            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.InCode = inCode;

            try
            {
                getReply = devBranchProxy.StatusCodeGet(getRequest);
            }
            catch (FaultException ex)
            {
                Assert.Fail("Caught WCF FaultExceptionException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
            }
            catch (Exception e)
            {
                Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
            }

            Assert.IsNotNull(getReply, "StatusCodeGetRequestDC object null");
            Assert.IsNotNull(getReply.List, "getReply.List is null");
            Assert.AreEqual(1, getReply.List.Count, "Get returned the wrong number of entries. It should have returned 1 but instead returned {0}.", getReply.List.Count);
            Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(getReply.List[0].Code, getRequest.InCode);
        }

        /// <summary>
        /// Verify GET FROM ltblStatusCodes Table for Valid Name
        /// </summary>
        /// <param name="name">name from row to do a get on</param>
        private void GetStatusCodeForValidName(string name)
        {
            getRequest = new StatusCodeGetRequestDC();
            getReply = null;

            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            getRequest.InName = name;

            try
            {
                getReply = devBranchProxy.StatusCodeGet(getRequest);
            }
            catch (FaultException ex)
            {
                Assert.Fail("Caught WCF FaultExceptionException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
            }
            catch (Exception e)
            {
                Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
            }

            Assert.IsNotNull(getReply, "StatusCodeGetRequestDC object null");
            Assert.IsNotNull(getReply.List, "getReply.List is null");
            Assert.AreEqual(1, getReply.List.Count, "Get returned the wrong number of entries. It should have returned 1 but instead returned {0}.", getReply.List.Count);
            Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(getReply.List[0].Name, getRequest.InName);
        }

        /// <summary>
        /// Verify GET FROM ltblStatusCodes Table for Invalid IDs
        /// </summary>
        /// <param name="softDeletedIncode">incode of row to do a get on</param>
        private void VerifyGetStatusCodeForInvalidIncode(int inCode)
        {
            bool isFaultException = false;
            getRequest = new StatusCodeGetRequestDC();
            getReply = null;

            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            getRequest.InCode = inCode;

            try
            {
                getReply = devBranchProxy.StatusCodeGet(getRequest);
            }
            // Task 20943. Add fault exception validation.
            //catch (FaultException<www.microsoft.com.practices.EnterpriseLibrary._2007._01.wcf.validation.ValidationFault> exc)
            //{
            //    if (getRequest.InCode < 0)
            //    {
            //        Assert.IsNotNull(exc.Detail.Details);
            //        Assert.AreEqual(1, exc.Detail.Details.Count);
            //        Assert.IsNotNull(exc.Detail.Details[0].Message);
            //        Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_InCode_MSG, exc.Detail.Details[0].Message);
            //        isFaultException = true;
            //    }
            //    else
            //    {
            //        Assert.Fail("Failed to get data from ltblStatusCodes: {0}", exc.Message);
            //    }
            //}
            catch (FaultException ex)
            {
                Assert.Fail("Caught WCF FaultExceptionException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
            }
            catch (Exception e)
            {
                Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
            }

            if (!isFaultException)
            {
                int errorConstant = GetErrorConstantInvalidIncode(inCode);

                Assert.IsNotNull(getReply, "getReply is null.");
                Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
                Assert.AreEqual(0, getReply.List.Count, "Service returned wrong number of records. InCode= {0}. It should have returned 0 but instead returned {1}.", inCode, getReply.List.Count);
                Assert.IsNotNull(getReply.StatusReply.ErrorMessage, "Error Message is null");
                Assert.AreEqual(errorConstant, getReply.StatusReply.Errorcode, "Service returned unexpected error code. Expected: {0}, Returned: {1}", errorConstant, getReply.StatusReply.Errorcode);
            }
        }

        /// <summary>
        /// Verify GET FROM ltblStatusCodes Table for softDeleted IDs
        /// </summary>
        /// <param name="softDeletedIncode">incode of row to do a get on</param>
        private void GetStatusCodeForSoftDeletedIncodes(int softDeletedIncode)
        {
            getRequest = new StatusCodeGetRequestDC();

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.InCode = softDeletedIncode;

            try
            {
                getReply = devBranchProxy.StatusCodeGet(getRequest);
            }
            catch (FaultException ex)
            {
                Assert.Fail("Caught WCF FaultExceptionException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
            }
            catch (Exception e)
            {
                Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
            }

            int errorConstant = GetErrorConstantSoftDeletedID();

            Assert.IsNotNull(getReply, "getReply is null.");
            Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(0, getReply.List.Count, "Service returned wrong number of records. InId= {0}. It should have returned 0 but instead returned {1}.", softDeletedIncode, getReply.List.Count);
            Assert.IsNotNull(getReply.StatusReply.ErrorMessage, "Error Message is null"); 
            Assert.AreEqual(errorConstant, getReply.StatusReply.Errorcode, "Service returned unexpected error code. Expected: {0}, Returned: {1}", errorConstant, getReply.StatusReply.Errorcode);
        }

  
        static int x = 100000;

        private int GetNext()
        {
            lock(this)
            {
                x = x + 1;
                return x;
            }
        }

        [WorkItem(22192)]
        [Description("Verify GET FROM ltblStatusCodes Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetStatusCodeForValidId()
        {
            GetStatusCodeForValidIncode(Utility.GetRandomStatusCode);
        }

        [WorkItem(22190)]
        [Description("Verify GET FROM ltblStatusCodes Table with valid name")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetStatusCodeForValidName()
        {
            GetStatusCodeForValidName(STATUSCODE_NAME);
        }

        [WorkItem(22191)]
        [Description("Verify GET FROM ltblStatusCodes Table using Invalid Incode")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetStatusCodeForInValidIDs()
        {
            int[] nonExistingIncodes = new int[] 
           {
               Int32.MinValue,
               -1,
               6,
               2000,
               Int32.MaxValue         
           };

            foreach (int nonExistingIncode in nonExistingIncodes)
            {
                VerifyGetStatusCodeForInvalidIncode(nonExistingIncode);
            }
        }

    }
}
