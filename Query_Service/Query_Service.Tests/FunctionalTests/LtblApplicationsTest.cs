//-----------------------------------------------------------------------
// <copyright file="LtblApplicationsTest.cs" company="Microsoft">
// Copyright
// A Test Class for Service Operations on ltblApplications table.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using Microsoft.Support.Workflow.Service.DataAccessServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;
using Query_Service.Tests.Common;

namespace Query_Service.Tests
{
    /// <summary>
    /// A Test Class for Service Operations on ltblApplications table.
    /// </summary>
    [TestClass]
    public class LtblApplicationsTest : QueryServiceTestBase
    {
         private const string TABLE_NAME = "ltblApplications";
        private const string INCALLER_IS_NULL_FAULT_EXCEPTION_MESSAGE = "1|Incaller| is null";
        private const string INCALLERVERSION_IS_NULL_FAULT_EXCEPTION_MESSAGE = "2|IncallerVersion| is null";

        private ApplicationsGetRequestDC getRequest;
        private ApplicationsGetReplyDC getReply;

        /// <summary>
        /// Verify GET FROM ltblApplications Table for Valid IDs
        /// </summary>
        /// <param name="id">id of row to do a get on</param>
        private void GetApplicationForValidIDAndValidate(int id)
        {
            getRequest = new ApplicationsGetRequestDC();
            getReply = null;

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.InId = id;

            try
            {
                getReply = devBranchProxy.ApplicationsGet(getRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from ltblApplications: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from ltblApplication: {0}", ex.Message);
            }

            Assert.IsNotNull(getReply, "getReply is null");
            Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(0, getReply.StatusReply.Errorcode, "Status reply error code is not 0. Instead it is {0}.", getReply.StatusReply.Errorcode);
            Assert.IsNotNull(getReply.List, "getReply.List is null");
            Assert.AreEqual(1, getReply.List.Count, "Get returned the wrong number of entries. InId: {0}.  It should have returned 1 but instead returned {1}.", id, getReply.List.Count);
            //Assert.AreEqual(getRequest.InId, getReply.List[0].Id, "Get returned wrong data. Expected id: {0}. Actual: {1}", getRequest.InId, getReply.List[0].Id);
        }

        /// <summary>
        /// Verify GET FROM ltblApplications Table for Null InCaller
        /// </summary>
        /// <param name="id">id of row to do a get on</param>
        private void GetApplicationForNullInCallerAndValidate(int id)
        {
            bool isFaultException = false;
            getRequest = new ApplicationsGetRequestDC();
            getReply = null;

            //Populate the request data
            getRequest.InId = id;

            try
            {
                getReply = devBranchProxy.ApplicationsGet(getRequest);
            }
            // Task 20943. Add fault exception validation.
            //catch (FaultException<www.microsoft.com.practices.EnterpriseLibrary._2007._01.wcf.validation.ValidationFault> exc)
            //{
            //    Assert.IsNotNull(exc.Detail.Details);
            //    Assert.AreEqual(2, exc.Detail.Details.Count);
            //    Assert.IsNotNull(exc.Detail.Details[0].Message);
            //    Assert.AreEqual(exc.Detail.Details[0].Message, INCALLER_IS_NULL_FAULT_EXCEPTION_MESSAGE);
            //    Assert.IsNotNull(exc.Detail.Details[1].Message);
            //    Assert.AreEqual(exc.Detail.Details[1].Message, INCALLERVERSION_IS_NULL_FAULT_EXCEPTION_MESSAGE);
            //    isFaultException = true;
            //}
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from ltblApplications: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from ltblApplication: {0}", ex.Message);
            }

            if (isFaultException)
            {
                Assert.IsNull(getReply);
                Assert.IsTrue(isFaultException);
            }
        }

        /// <summary>
        /// Verify GET FROM ltblApplications Table for a valid name
        /// </summary>
        /// <param name="name">name from row to do a get on</param>
        /// <returns>returns the id of this row</returns>
        private int GetApplicationForValidNameAndValidateForMaxID(string name)
        {
            getRequest = new ApplicationsGetRequestDC();
            getReply = null;

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.InName = name;

            try
            {
                getReply = devBranchProxy.ApplicationsGet(getRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from ltblApplications: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from ltblApplication: {0}", ex.Message);
            }

            Assert.IsNotNull(getReply, "getReply is null");
            Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(0, getReply.StatusReply.Errorcode, "Status reply error code is not 0. Instead it is {0}.", getReply.StatusReply.Errorcode);
            Assert.IsNotNull(getReply.List, "getReply.List is null");
            Assert.AreNotEqual(0, getReply.List.Count, "Get returned the wrong number of entries. InName: {0}.  It should have returned {1} but instead returned 0.", name, getReply.List.Count);

            int index = getReply.List.Count - 1;
            int id = getReply.List[index].Id;

            return id;
        }

        /// <summary>
        /// Verify GET FROM ltblApplications Table for invalid names
        /// </summary>
        /// <param name="name">name from row to do a get on</param>
        /// <returns>returns the id of this row</returns>
        private void VerifyGetApplicationForInvalidName(string name)
        {
            getRequest = new ApplicationsGetRequestDC();
            getReply = null;

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.InName = name;

            try
            {
                getReply = devBranchProxy.ApplicationsGet(getRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from ltblApplications: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from ltblApplication: {0}", ex.Message);
            }

            Assert.IsNotNull(getReply, "getReply is null");
            Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(SprocValues.GET_INVALID_NAME, getReply.StatusReply.Errorcode, "Status reply error code is not {0}. Instead it is {1}.", SprocValues.GET_INVALID_NAME, getReply.StatusReply.Errorcode);
        }

        /// <summary>
        /// Verify GET FROM ltblApplications Table for Invalid IDs
        /// </summary>
        /// <param name="id">id of row to do a get on</param>
        private void GetApplicationForInvalidIDAndValidate(int id)
        {
            bool isFaultException = false;
            getRequest = new ApplicationsGetRequestDC();
            getReply = null;

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.InId = id;

            try
            {
                getReply = devBranchProxy.ApplicationsGet(getRequest);
            }
            // Task 20943. Add fault exception validation.
            //catch (FaultException<www.microsoft.com.practices.EnterpriseLibrary._2007._01.wcf.validation.ValidationFault> exc)
            //{
            //    if (id < 0)
            //    {
            //        Assert.IsNotNull(exc.Detail.Details);
            //        Assert.AreEqual(1, exc.Detail.Details.Count);
            //        Assert.IsNotNull(exc.Detail.Details[0].Message);
            //        Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_INID_MSG, exc.Detail.Details[0].Message);
            //        isFaultException = true;
            //    }
            //    else
            //    {
            //        Assert.Fail("Failed to get data from ltblApplications: {0}", exc.Message);
            //    }
            //}
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from ltblApplications: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from ltblApplication: {0}", ex.Message);
            }

            if (!isFaultException)
            {
                int errorConstant = GetErrorConstantInvalidID(id);

                Assert.IsNotNull(getReply, "getReply is null");
                Assert.IsNotNull(getReply.StatusReply, "getReply.Status is null");
                Assert.AreEqual(errorConstant, getReply.StatusReply.Errorcode, "Returned the wrong status error code. InId: {0}", id);
                Assert.IsNotNull(getReply.List);
                Assert.AreEqual(0, getReply.List.Count, "Get returned the wrong number of entries. InId: {0}. It should not have returned 0 and instead returned {1}", id, getReply.List.Count);
            }
        }

        /// <summary>
        /// Verify GET FROM ltblApplications Table for softDeleted IDs
        /// </summary>
        /// <param name="softDeletedID">id of row to do a get on</param>
        private void GetApplicationForSoftDeletedIDs(int softDeletedID)
        {
            getRequest = new ApplicationsGetRequestDC();
            getReply = null;

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.InId = softDeletedID;

            try
            {
                getReply = devBranchProxy.ApplicationsGet(getRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from ltblApplications: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from ltblApplication: {0}", ex.Message);
            }

            int errorConstant = GetErrorConstantSoftDeletedID();

            Assert.IsNotNull(getReply, "getReply is null");
            Assert.IsNotNull(getReply.List);
            Assert.AreEqual(0, getReply.List.Count, "Get returned the wrong number of entries. InId: {0}. It should have returned 0.", softDeletedID);
            Assert.IsNotNull(getReply.StatusReply, "getReply.Status is null");
            Assert.AreEqual(errorConstant, getReply.StatusReply.Errorcode, "Returned the wrong status error code. InId: {0}. Expected {1}. Actual {2}", softDeletedID, errorConstant, getReply.StatusReply.Errorcode);
        }

 
        /// <summary>
        /// Verifies that different WCF calls for the ltblApplications Table can be run simultaneously
        /// </summary>
        private void VerifyConcurrencyTestingForApplicationsByRunningThreads()
        {
            ThreadStart[] applicationThreadStarter = new ThreadStart[4];
            Thread[] applicationThread = new Thread[4];

            applicationThreadStarter[0] = delegate { GetApplicationForValidIDAndValidate(1); };
            applicationThreadStarter[1] = delegate { GetApplicationForValidIDAndValidate(3); };
            applicationThreadStarter[2] = delegate { GetApplicationForValidIDAndValidate(4); };
            applicationThreadStarter[3] = delegate { GetApplicationForValidIDAndValidate(1); };

            for (int i = 0; i < applicationThread.Length; i++)
            {
                applicationThread[i] = new Thread(applicationThreadStarter[i]);
            }

            for (int i = 0; i < applicationThread.Length; i++)
            {
                applicationThread[i].Start();
                //applicationThread[i].Join();
            }
        }

        [WorkItem(20908)]
        [Description("Verify GET FROM ltblApplications Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Func)]
        [TestMethod]
        public void VerifyGetApplicationForValidIDs()
        {
            for (int id = 1; id <= 10; id++)
            {
                GetApplicationForValidIDAndValidate(id);
            }
        }

        [WorkItem(21036)]
        [Description("Verify GET FROM ltblApplications Table for Null Incaller")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Func)]
        [TestMethod]
        public void VerifyGetApplicationForNullInCaller()
        {
            GetApplicationForNullInCallerAndValidate(1);
        }

        [WorkItem(21035)]
        [Description("Verify GET FROM ltblApplications Table For invalid name")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Func)]
        [TestMethod]
        public void VerifyGetApplicationForInvalidName()
        {
            VerifyGetApplicationForInvalidName(TEST_STRING);
        }
 
        [WorkItem(20966)]
        [Description("Verify ConcurrencyTesting FROM ltblApplications Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Func)]
        [TestMethod]
        public void VerifyConcurrencyTestingForApplications()
        {
            VerifyConcurrencyTestingForApplicationsByRunningThreads();
        }

    }
}
