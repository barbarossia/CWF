//-----------------------------------------------------------------------
// <copyright file="EtblStoreActivitiesTest.cs" company="Microsoft">
// Copyright
// A test class used to verify service interaction with etblStoreActivities table.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using Microsoft.Support.Workflow.Service.DataAccessServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;
using Query_Service.Tests.Common;
using Query_Service.ExtensionForTests.DataProxy;

namespace Query_Service.Tests
{
    /// <summary>
    /// A test class used to verify service interaction with etblStoreActivities table.
    /// </summary>
    [TestClass]
    public class EtblStoreActivitiesTest : QueryServiceTestBase
    {
        private const string TABLE_NAME = "etblStoreActivities";
        private const string VERSION = "1.0.1.0";
        private const string TOOL_BOX_NAME = "OASP";
        private const string ACTIVITY_LIBRARY_VERSION = "1.0.0.0";
        private const string STATUS_CODE_NAME = "Draft";
        private const string ACTIVITY_LIBRARY_NAME = "OASP.Core2";
        private const string STORE_ACTIVITY_NAME = "PublishingWorkflow";
        private const string INVALID_VERSION = "1.0.0.0";
        private const string VALID_GUID = "F0AF0E23-58DE-4C90-8E2D-A3677430D1D8";
        private const string IN_CATEGORY_NAME = "OAS Basic Controls";
        private const string AUTH_GROUP_NAME = "pqocwfadmin";
        private const string BASETYPE = "ListBox";
        private const string WORKFLOWTYPENAME = "Metadata";
        private const string XAML = "<XamlBeginTag></XamlBeginTag>";


        StoreActivitiesDC getRequest;
        List<StoreActivitiesDC> getReplyList;
        CWF.DataContracts.StatusReplyDC statusReplyDC;

        /// <summary>
        /// Set lock status for store activities by following parameters: Name / Version / Locked / LockedBy
        /// </summary>
        /// <param name="name">The name of store activity</param>
        /// <param name="version">The version number of store activity</param>
        /// <param name="locked">If user want to lock this item</param>
        /// <param name="lockedBy">The user who want to lock this item</param>
        /// <returns></returns>
        private CWF.DataContracts.StatusReplyDC SetLockForStoreActivity(string name, string version, bool locked, string lockedBy)
        {
            getRequest = new StoreActivitiesDC();

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.Name = name;
            getRequest.Version = version;
            getRequest.Locked = locked;
            getRequest.LockedBy = lockedBy;

            try
            {
                return devBranchProxy.StoreActivitiesSetLock(getRequest, DateTime.Now);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from etblStoreActivities: {0}", e.Message);
                return null;
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from etblStoreActivities: {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Clear lock field from etblStoreActivities
        /// </summary>
        /// <param name="name">The name of activity</param>
        /// <param name="version">The version of activity</param>
        private void ClearLockForActivity(string name, string version)
        {
            ClearLockRequestDC request = new ClearLockRequestDC();
            request.Name = name;
            request.Version = version;
            request.Incaller = IN_CALLER;
            request.IncallerVersion = IN_CALLER_VERSION;

            try
            {
                testProxy.ClearLock(request);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to clear lock from etblStoreActivities: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to clear lock from etblStoreActivities: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Verify GET FROM etblStoreActivities Table for Valid IDs
        /// </summary>
        /// <param name="id">id of row to do a get on</param>
        private void VerifyGetStoreActivitiesForValidID(int id)
        {
            getRequest = new StoreActivitiesDC();
            getReplyList = null;

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.Id = id;

            try
            {
                getReplyList = new List<StoreActivitiesDC>(devBranchProxy.StoreActivitiesGet(getRequest));
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from etblStoreActivities: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from etblStoreActivities: {0}", ex.Message);
            }

            Assert.IsNotNull(getReplyList, "getReply.List is null");
            Assert.AreEqual(1, getReplyList.Count, "Get returned the wrong number of entries. id: {0}. It should have returned 1 but instead returned {1}.", id, getReplyList.Count);
            Assert.IsNotNull(getReplyList[0].StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(0, getReplyList[0].StatusReply.Errorcode, "StatusReply returned the wrong error code. Expected: 0. Actual: {0}", getReplyList[0].StatusReply.Errorcode);
            Assert.AreEqual(getRequest.Id, getReplyList[0].Id, "Get returned wrong data");
        }

        /// <summary>
        /// Verify GET FROM etblStoreActivities Table for valid name and version
        /// </summary>
        /// <param name="name">name of row to do a get on</param>
        /// <param name="version">version of name to do a get on</param>
        private void VerifyGetStoreActivitiesForValidNameAndVersion(string name, string version)
        {
            getRequest = new StoreActivitiesDC();
            getReplyList = new List<StoreActivitiesDC>();

            VerifyGetForValidNameAndVersion(name, version, getRequest, getReplyList);
        }

        /// <summary>
        /// Verify GET FROM etblStoreActivities Table for valid name and invalid version
        /// </summary>
        /// <param name="name">name of row to do a get on</param>
        /// <param name="version">version of name to do a get on</param>
        private void VerifyGetStoreActivitiesForValidNameAndInvalidVersion(string name, string version)
        {
            getRequest = new StoreActivitiesDC();
            getReplyList = null;

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.Name = name;
            getRequest.Version = version;

            try
            {
                getReplyList = new List<StoreActivitiesDC>(devBranchProxy.StoreActivitiesGet(getRequest));
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from etblStoreActivities: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from etblStoreActivities: {0}", ex.Message);
            }

            Assert.IsNotNull(getReplyList, "getReply.List is null");
            Assert.AreEqual(1, getReplyList.Count, "Get returned the wrong number of entries. name: {0}, version: {1}. It should have returned 1 but instead returned {2}.", name, version, getReplyList.Count);
            Assert.IsNotNull(getReplyList[0].StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(SprocValues.GET_INVALID_GETNAMEVERSION_ID, getReplyList[0].StatusReply.Errorcode, "StatusReply returned the wrong error code. Expected: 0. Actual: {0}", getReplyList[0].StatusReply.Errorcode);
        }

        /// <summary>
        /// Verify GET FROM etblStoreActivities Table for Valid guid
        /// </summary>
        /// <param name="guid">guid of row to do a get on</param>
        private void VerifyGetStoreActivitiesForValidGuid(Guid guid)
        {
            getRequest = new StoreActivitiesDC();
            getReplyList = null;

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.Guid = guid;

            try
            {
                getReplyList = new List<StoreActivitiesDC>(devBranchProxy.StoreActivitiesGet(getRequest));
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from etblStoreActivities: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from etblStoreActivities: {0}", ex.Message);
            }

            Assert.IsNotNull(getReplyList, "getReply.List is null");
            Assert.AreEqual(1, getReplyList.Count, "Get returned the wrong number of entries. guid: {0}. It should have returned 1 but instead returned {1}.", guid, getReplyList.Count);
            Assert.IsNotNull(getReplyList[0].StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(0, getReplyList[0].StatusReply.Errorcode, "StatusReply returned the wrong error code. Expected: 0. Actual: {0}", getReplyList[0].StatusReply.Errorcode);
            Assert.AreEqual(getRequest.Guid, getReplyList[0].Guid, "Get returned wrong data");
        }

        /// <summary>
        /// Verify GET FROM etblStoreActivities Table for a valid name
        /// </summary>
        /// <param name="name">name from row to do a get on</param>
        /// <returns>returns the id of this row</returns>
        private int VerifyGetStoreActivitiesForValidNamesForMaxID(string name)
        {
            getRequest = new StoreActivitiesDC();
            getReplyList = null;

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            getRequest.Name = name;

            try
            {
                getReplyList = new List<StoreActivitiesDC>(devBranchProxy.StoreActivitiesGet(getRequest));
            }
            catch (FaultException ex)
            {
                Assert.Fail("Caught WCF FaultExceptionException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
            }
            catch (Exception e)
            {
                Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
            }

            // Validate
            Assert.IsNotNull(getReplyList, "getReply.List is null");
            Assert.AreEqual(1, getReplyList.Count, "Get returned the wrong number of entries. name: {0}. It should have returned 1 but instead returned {1}.", name, getReplyList.Count);
            Assert.IsNotNull(getReplyList[0].StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(0, getReplyList[0].StatusReply.Errorcode, "StatusReply returned the wrong error code. Expected: 0. Actual: {0}", getReplyList[0].StatusReply.Errorcode);
            Assert.AreEqual(name, getReplyList[0].Name, "Get returned the wrong name. name: {0} Expected: {1}", getReplyList[0].Name, name);

            int index = getReplyList.Count - 1;
            int id = getReplyList[index].Id;

            //id = getReplyList.Last().Id; // todo: next sprint

            return id;
        }

        /// <summary>
        /// Verify GET FROM etblStoreActivities Table for Invalid IDs
        /// </summary>
        /// <param name="nonExistingID">id of row to do a get on</param>
        private void VerifyGetStoreActivitiesForInValidIDs(int nonExistingID)
        {
            getRequest = new StoreActivitiesDC();

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.Id = nonExistingID;

            getReplyList = null;

            try
            {
                getReplyList = new List<StoreActivitiesDC>(devBranchProxy.StoreActivitiesGet(getRequest));
            }
            catch (FaultException ex)
            {
                Assert.Fail("Caught WCF FaultExceptionException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
            }
            catch (Exception e)
            {
                Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
            }

            int errorConstant = GetErrorConstantInvalidID(nonExistingID);

            Assert.IsNotNull(getReplyList, "getReplyList is null.");
            Assert.AreEqual(1, getReplyList.Count, "Service returned wrong number of records. InId= {0}. It should have returned 1 but instead returned {1}.", nonExistingID, getReplyList.Count);
            Assert.IsNotNull(getReplyList[0].StatusReply, "getReplyList[0].StatusReply is null");
            Assert.AreEqual(errorConstant, getReplyList[0].StatusReply.Errorcode, "StatusReply returned the wrong error code. Expected: {0}. Actual: {1}", errorConstant, getReplyList[0].StatusReply.Errorcode);
            Assert.IsNotNull(getReplyList[0].StatusReply.ErrorMessage, "StatusReply.ErrorMessage is null");
        }

        /// <summary>
        /// Verify GET FROM etblStoreActivities Table for softDeleted IDs
        /// </summary>
        /// <param name="softDeletedID">id of row to do a get on</param>
        private void VerifyGetStoreActivitiesForSoftDeletedID(int softDeletedID)
        {
            getRequest = new StoreActivitiesDC();

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.Id = softDeletedID;

            getReplyList = null;

            try
            {
                getReplyList = new List<StoreActivitiesDC>(devBranchProxy.StoreActivitiesGet(getRequest));
            }
            catch (FaultException ex)
            {
                Assert.Fail("Caught WCF FaultExceptionException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
            }
            catch (Exception e)
            {
                Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
            }

            int errorConstant = SprocValues.GET_INVALID_GETID_ON_SOFTDELETEDROW_ID;

            Assert.IsNotNull(getReplyList, "getReplyList is null.");
            Assert.AreEqual(1, getReplyList.Count, "Service returned wrong number of records. InId= {0}. It should have returned 1 but instead returned {1}.", softDeletedID, getReplyList.Count);
            Assert.IsNotNull(getReplyList[0].StatusReply, "getReplyList[0].StatusReply is null");
            Assert.AreEqual(errorConstant, getReplyList[0].StatusReply.Errorcode, "StatusReply returned the wrong error code. Expected: {0}. Actual: {1}", errorConstant, getReplyList[0].StatusReply.Errorcode);
            Assert.IsNotNull(getReplyList[0].StatusReply.ErrorMessage, "StatusReply.ErrorMessage is null");
        }

        /// <summary>
        /// Verify lock store activity
        /// </summary>
        /// <param name="name">name of row to do a get on</param>
        /// <param name="version">version of name to do a get on</param>
        private void VerifySetLockTrueForStoreActivity(string name, string version)
        {
            bool tryToLock = true;
            string lockedBy = "TestAuthor01";
            statusReplyDC = SetLockForStoreActivity(name, version, tryToLock, lockedBy);

            //Clear the lock field for test activity, this will allow 
            //other test case to use the same test data
            ClearLockForActivity(name, version);

            Assert.AreEqual(statusReplyDC.Errorcode, 0, "Failed to lock store activity");
        }

        /// <summary>
        /// Veriy unlock store activity
        /// </summary>
        /// <param name="name">name of row to do a get on</param>
        /// <param name="version">version of name to do a get on</param>
        private void VerifySetLockFalseForStoreActivity(string name, string version)
        {
            bool tryToLock = true;
            string lockedBy = "TestAuthor01";
            //Lock the store activity first
            SetLockForStoreActivity(name, version, tryToLock, lockedBy);

            //unlock the locked store activity
            statusReplyDC = SetLockForStoreActivity(name, version, !tryToLock, lockedBy);

            //Clear the lock field for test activity, this will allow 
            //other test case to use the same test data
            ClearLockForActivity(name, version);

            Assert.AreEqual(statusReplyDC.Errorcode, 0, "Failed to lock store activity");
        }

        [WorkItem(22195)]
        [Description("Verify GET FROM etblStoreActivities Table for Valid IDs")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetStoreActivitiesForValidIDs()
        {
            //These are rows in the database at install time, it's possible this test will be updated
            // to be more dynamic in the future and not contain hardcode numbers.
            int[] ids = new int[] { 1, 2, 3, 4, 5 };

            foreach (int id in ids)
            {
                VerifyGetStoreActivitiesForValidID(id);
            }
        }

        [WorkItem(22197)]
        [Description("Verify GET FROM etblStoreActivities Table for Valid name and version")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetStoreActivitiesForValidNameAndVersion()
        {
            string name = STORE_ACTIVITY_NAME;
            string version = "1.0.1.0";
            VerifyGetStoreActivitiesForValidNameAndVersion(name, version);
        }

        [WorkItem(22196)]
        [Description("Verify GET FROM etblStoreActivities Table for Valid name and Invalid version")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetStoreActivitiesForValidNameAndInvalidVersion()
        {
            string name = STORE_ACTIVITY_NAME;
            string version = INVALID_VERSION;
            VerifyGetStoreActivitiesForValidNameAndInvalidVersion(name, version);
        }

        [WorkItem(22194)]
        [Description("Verify GET FROM etblStoreActivities Table for Valid guid")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetStoreActivitiesForValidGuid()
        {
            Guid guid = new Guid(VALID_GUID);
            VerifyGetStoreActivitiesForValidGuid(guid);
        }

        [WorkItem(22193)]
        [Description("Verify GET FROM etblStoreActivities Table for Invalid IDs")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetStoreActivitiesForInvalidIDs()
        {
            int[] nonExistingIDs = new int[] 
            {
                Int32.MinValue,
                -1,
                Int32.MaxValue
            };

            foreach (int nonExistingID in nonExistingIDs)
            {
                VerifyGetStoreActivitiesForInValidIDs(nonExistingID);
            }
        }

        [WorkItem(284020)]
        [Description("Verify check out locked activity for no action by different user")]
        [Owner("v-toy")]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyLockStoreActivity()
        {
            string name = STORE_ACTIVITY_NAME;
            string version = VERSION;
            VerifySetLockTrueForStoreActivity(name, version);
        }

        [WorkItem(284021)]
        [Description("Verify check out locked activity for no action by different user")]
        [Owner("v-toy")]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyUnlockStoreActivity()
        {
            string name = STORE_ACTIVITY_NAME;
            string version = VERSION;
            VerifySetLockFalseForStoreActivity(name, version);
        }

    }
}
