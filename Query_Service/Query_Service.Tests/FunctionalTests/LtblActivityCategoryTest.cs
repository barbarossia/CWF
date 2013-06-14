//-----------------------------------------------------------------------
// <copyright file="LtblActivityCategoryTest.cs" company="Microsoft">
// Copyright
// A test class used to verify service interaction with LtblActivityCategoryTest table.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;

using CWF.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Query_Service.Tests.Common;
using Microsoft.Support.Workflow.Service.DataAccessServices;

namespace Query_Service.Tests
{
    /// <summary>
    /// A Test class used to verify service interaction to the ltblActivityCategory table
    /// </summary>
    [TestClass]
    public class LtblActivityCategoryTest : QueryServiceTestBase
    {
        private const string TABLE_NAME = "ActivityCategory";
        private const string IN_AUTH_GROUP_NAME = "pqocwfauthors";

        private ActivityCategoryByNameGetRequestDC getByNameRequest;
        private ActivityCategoryCreateOrUpdateRequestDC createOrUpdateRequest;

        private ActivityCategoryCreateOrUpdateReplyDC createOrUpdateReply;
        private List<ActivityCategoryByNameGetReplyDC> getByNameReplyList;

        /// <summary>
        /// Verify GET FROM ltblActivityCategory Table for Valid IDs
        /// </summary>
        /// <param name="id">id of row to do a get on</param>
        private void GetActivityCategoriesForValidIDs(int id)
        {
            getByNameRequest = new ActivityCategoryByNameGetRequestDC();
            getByNameReplyList = new List<ActivityCategoryByNameGetReplyDC>();

            getByNameRequest.Incaller = IN_CALLER;
            getByNameRequest.IncallerVersion = IN_CALLER_VERSION;

            getByNameRequest.InId = id;

            try
            {
                getByNameReplyList = new List<ActivityCategoryByNameGetReplyDC>(devBranchProxy.ActivityCategoryGet(getByNameRequest));
            }
            catch (FaultException ex)
            {
                Assert.Fail("Caught WCF FaultException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
            }
            catch (Exception e)
            {
                Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
            }

            // Validate
            Assert.IsNotNull(getByNameReplyList, "getByNameReplyList is null.");
            Assert.AreEqual(1, getByNameReplyList.Count, "getByNameReplyList.Count is not 1.");
            Assert.IsNotNull(getByNameReplyList[0].StatusReply, "getByNameReplyList[0].StatusReply is null");
            Assert.AreEqual(0, getByNameReplyList[0].StatusReply.Errorcode, "StatusReply.Errorcode is not 0. Instead it is {0}.", getByNameReplyList[0].StatusReply.Errorcode);
            Assert.IsTrue(string.IsNullOrEmpty(getByNameReplyList[0].StatusReply.ErrorMessage), "Error Message is not null. Error Message: {0}", getByNameReplyList[0].StatusReply.ErrorMessage);
            Assert.IsTrue(string.IsNullOrEmpty(getByNameReplyList[0].StatusReply.ErrorGuid), "ErrorGuid is not null. ErrorGuid: {0}", getByNameReplyList[0].StatusReply.ErrorGuid);
            Assert.AreEqual(getByNameRequest.InId, getByNameReplyList[0].Id, "Service returned the wrong record. Expected Id: {0}, Actual Id: {1}", getByNameRequest.InId, getByNameReplyList[0].Id);
        }

        /// <summary>
        /// Verify GET FROM ltblActivityCategory Table for a valid name
        /// </summary>
        /// <param name="name">name from row to do a get on</param>
        /// <returns>returns the id of this row</returns>
        private int GetActivityCategoriesForValidName(string name)
        {
            getByNameRequest = new ActivityCategoryByNameGetRequestDC();
            getByNameReplyList = new List<ActivityCategoryByNameGetReplyDC>();

            getByNameRequest.Incaller = IN_CALLER;
            getByNameRequest.IncallerVersion = IN_CALLER_VERSION;

            getByNameRequest.InName = name;

            try
            {
                getByNameReplyList = new List<ActivityCategoryByNameGetReplyDC>(devBranchProxy.ActivityCategoryGet(getByNameRequest));
            }
            catch (FaultException ex)
            {
                Assert.Fail("Caught WCF FaultException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
            }
            catch (Exception e)
            {
                Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
            }

            // Validate
            Assert.IsNotNull(getByNameReplyList, "getByNameReplyList is null.");
            Assert.AreNotEqual(0, getByNameReplyList.Count, "getByNameReplyList.Count is 0");
            Assert.IsNotNull(getByNameReplyList[0].StatusReply, "getByNameReplyList[0].StatusReply is null");
            Assert.IsTrue(string.IsNullOrEmpty(getByNameReplyList[0].StatusReply.ErrorMessage), "Error Message is not null. Error Message: {0}", getByNameReplyList[0].StatusReply.ErrorMessage);
            Assert.IsTrue(string.IsNullOrEmpty(getByNameReplyList[0].StatusReply.ErrorGuid), "ErrorGuid is not null. ErrorGuid: {0}", getByNameReplyList[0].StatusReply.ErrorGuid);
            int id = getByNameReplyList[0].Id;

            return id;
        }

        /// <summary>
        /// Verify GET FROM ltblActivityCategory Table for Invalid IDs
        /// </summary>
        /// <param name="nonExistingID">id of row to do a get on</param>
        private void GetActivityCategoriesForInvalidIDs(int nonExistingID)
        {
            bool isFaultException = false;

            getByNameRequest = new ActivityCategoryByNameGetRequestDC();
            getByNameReplyList = new List<ActivityCategoryByNameGetReplyDC>();

            getByNameRequest.Incaller = IN_CALLER;
            getByNameRequest.IncallerVersion = IN_CALLER_VERSION;

            getByNameRequest.InId = nonExistingID;

            try
            {
                getByNameReplyList = new List<ActivityCategoryByNameGetReplyDC>(devBranchProxy.ActivityCategoryGet(getByNameRequest));
            }
            catch (FaultException ex)
            {
                Assert.Fail("Caught WCF FaultException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
            }
            catch (Exception e)
            {
                Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
            }

            if (!isFaultException)
            {
                
                // Validate
                Assert.IsNotNull(getByNameReplyList, "getByNameReplyList is null.");
                Assert.AreNotEqual(0, getByNameReplyList.Count, "getByNameReplyList.Count is 0");
                Assert.IsNotNull(getByNameReplyList[0].StatusReply, "getByNameReplyList[0].StatusReply is null");
                Assert.IsTrue(string.IsNullOrEmpty(getByNameReplyList[0].StatusReply.ErrorMessage), "Error Message is null.");

                int id = getByNameReplyList[0].Id;
                Assert.IsFalse(id > 1, "Id should be gereater than 1");

                //int errorConstant = GetErrorConstantInvalidID(nonExistingID);
                //Assert.AreNotEqual(errorConstant, getByNameReplyList[0].StatusReply.Errorcode, "Returned the wrong status error code. InId: {0}", nonExistingID);
            }
        }

        /// <summary>
        /// Verify GET FROM ltblActivityCategory Table for softDeleted IDs
        /// </summary>
        /// <param name="softDeletedID">id of row to do a get on</param>
        private void GetActivityCategoriesForSoftDeletedIDs(int softDeletedID)
        {
            getByNameRequest = new ActivityCategoryByNameGetRequestDC();
            
            getByNameRequest.Incaller = IN_CALLER;
            getByNameRequest.IncallerVersion = IN_CALLER_VERSION;

            getByNameRequest.InId = softDeletedID;

            GetActivityCategoriesForSoftDeletedIDs(getByNameRequest);
        }

        /// <summary>
        /// Verify GET FROM ltblActivityCategory Table for softDeleted IDs
        /// </summary>
        /// <param name="getByNameRequest">object to do a get on</param>
        private void GetActivityCategoriesForSoftDeletedIDs(ActivityCategoryByNameGetRequestDC getByNameRequest)
        {
            getByNameReplyList = new List<ActivityCategoryByNameGetReplyDC>();

            try
            {
                getByNameReplyList = new List<ActivityCategoryByNameGetReplyDC>(devBranchProxy.ActivityCategoryGet(getByNameRequest));
            }
            catch (FaultException ex)
            {
                Assert.Fail("Caught WCF FaultException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
            }
            catch (Exception e)
            {
                Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
            }

            int errorConstant = GetErrorConstantSoftDeletedID();

            // Validate
            Assert.IsNotNull(getByNameReplyList, "getByNameReplyList is null.");
            Assert.AreNotEqual(0, getByNameReplyList.Count, "getByNameReplyList.Count is 0");
            Assert.IsNotNull(getByNameReplyList[0].StatusReply, "getByNameReplyList[0].StatusReply is null");
            Assert.IsNotNull(getByNameReplyList[0].StatusReply.ErrorMessage, "Error Message is null.");
            Assert.AreEqual(errorConstant, getByNameReplyList[0].StatusReply.Errorcode, "Returned the wrong status error code. InId: {0}", getByNameRequest.InId);
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblActivityCategory Table and then does a cleanup. This will be an insert as id is set to 0.
        /// </summary>
        private void VerifyCreateOrUpdateActivityCategoriesAndCleanup()
        {
            string testFieldName = TEST_FIELD_NAME + Guid.NewGuid();

            // Create
            int id = CreateActivityCategoriesWithIdIsZero(testFieldName, TEST_STRING, TEST_STRING);

            // Update
            UpdateActivityCategories(id, testFieldName, TEST_STRING_2, TEST_STRING_2);
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblActivityCategory Table. This will be an insert as id is set to 0.
        /// </summary>
        /// <param name="name">name to do a create or update on.</param>
        /// <param name="description">description to do a create or update on.</param>
        /// <param name="metaTags">metaTag to do a create or update on.</param>
        /// <returns>returns the id of this row</returns>
        private int CreateActivityCategoriesWithIdIsZero(string name, string description, string metaTags)
        {
            int id = 0;

            CreateOrUpdateActivityCategories(id, name, description, metaTags);
            Thread.Sleep(1000);

            int newId = GetActivityCategoriesForValidName(name);

            Assert.IsNotNull(getByNameReplyList, "getByNameReplyList is null.");
            Assert.AreNotEqual(0, getByNameReplyList.Count, "getByNameReplyList.Count is 0");
            Assert.AreEqual(description, getByNameReplyList[0].Description, "Description did not get inserted correctly");
            Assert.AreEqual(name, getByNameReplyList[0].Name, "Name did not get inserted correctly");
            Assert.AreEqual(metaTags, getByNameReplyList[0].MetaTags, "MetaTags did not get inserted correctly");

            return newId;
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblActivityCategory Table. This will be an update as id != 0.
        /// </summary>
        /// <param name="id"> id to do a create or update on. id is 0 if it will be a create.</param>
        /// <param name="name">name to do a create or update on.</param>
        /// <param name="description">description to do a create or update on.</param>
        /// <param name="metaTags">metaTag to do a create or update on.</param>
        private void UpdateActivityCategories(int id, string name, string description, string metaTags)
        {
            Assert.AreNotEqual(0, id, "id = 0 should not be passed into this method, as this would be an insert.");

            CreateOrUpdateActivityCategories(id, name, description, metaTags);
            // Sleep for one second for next call
            Thread.Sleep(1000);
            GetActivityCategoriesForValidIDs(id);
            Assert.IsNotNull(getByNameReplyList, "getByNameReplyList is null.");
            Assert.AreNotEqual(0, getByNameReplyList.Count, "getByNameReplyList.Count is 0");
            Assert.AreEqual(description, getByNameReplyList[0].Description, "Description did not get inserted correctly");
            Assert.AreEqual(name, getByNameReplyList[0].Name, "Name did not get inserted correctly");
            Assert.AreEqual(metaTags, getByNameReplyList[0].MetaTags, "MetaTags did not get inserted correctly");
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblActivityCategory Table
        /// </summary>
        /// <param name="id"> id to do a create or update on. id is 0 if it will be a create.</param>
        /// <param name="name">name to do a create or update on.</param>
        /// <param name="description">description to do a create or update on.</param>
        /// <param name="metaTags">metaTags to do a create or update on.</param> 
        private void CreateOrUpdateActivityCategories(int id, string name, string description, string metaTags)
        {
            createOrUpdateRequest = new ActivityCategoryCreateOrUpdateRequestDC();

            createOrUpdateReply = null;

            //Populate the request data
            createOrUpdateRequest.Incaller = IN_CALLER;
            createOrUpdateRequest.IncallerVersion = IN_CALLER_VERSION;
            createOrUpdateRequest.InId = id;
            createOrUpdateRequest.InGuid = Guid.NewGuid();
            createOrUpdateRequest.InName = name;
            createOrUpdateRequest.InDescription = description;
            createOrUpdateRequest.InInsertedByUserAlias = USER;
            createOrUpdateRequest.InUpdatedByUserAlias = USER;
            createOrUpdateRequest.InMetaTags = metaTags;
            createOrUpdateRequest.InAuthGroupName = IN_AUTH_GROUP_NAME;

            try
            {
                createOrUpdateReply = devBranchProxy.ActivityCategoryCreateOrUpdate(createOrUpdateRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblActivityCategory: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblActivityCategory: {0}", ex.Message);
            }

            Assert.IsNotNull(createOrUpdateReply, "ActivityCategoryCreateOrUpdateReplyDC object null");
            Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.StatusReply is null");
            Assert.AreEqual(0, createOrUpdateReply.StatusReply.Errorcode, "createOrUpdateReply.StatusReply. Errorcode is not 0. Instead it is {0}.", createOrUpdateReply.StatusReply.Errorcode);
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorMessage, "createOrUpdateReply.StatusReply.ErrorMessage is not null");
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorGuid, "createOrUpdateReply.StatusReply.ErrorGuid is not null");
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblActivityCategory Table and then does a cleanup. This will be a create since id is null.
        /// </summary>
        public void VerifyCreateOrUpdateActivityCategoriesWithNullIDAndCleanup()
        {
            string testFieldName = TEST_FIELD_NAME + Guid.NewGuid();
            // Create with null id
            int id = CreateActivityCategoriesWithNullId(testFieldName, TEST_STRING, TEST_STRING);

            // Update
            UpdateActivityCategories(id, testFieldName, TEST_STRING_2, TEST_STRING_2);

        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblActivityCategory Table. This will be a create since id is null.
        /// </summary>
        /// <param name="name">name to do a create or update on.</param>
        /// <param name="description">description to do a create or update on.</param>
        /// <param name="metaTag">metaTag to do a create or update on.</param>
        /// <param name="newId">id that will be created if it's an update.</param>
        /// <returns>returns the id created</returns>
        private int CreateActivityCategoriesWithNullId(string name, string description, string metaTags)
        {
            createOrUpdateRequest = new ActivityCategoryCreateOrUpdateRequestDC();

            createOrUpdateReply = null;

            //Populate the request data
            createOrUpdateRequest.Incaller = IN_CALLER;
            createOrUpdateRequest.IncallerVersion = IN_CALLER_VERSION;
            createOrUpdateRequest.InGuid = Guid.NewGuid();
            createOrUpdateRequest.InName = name;
            createOrUpdateRequest.InDescription = description;
            createOrUpdateRequest.InInsertedByUserAlias = USER;
            createOrUpdateRequest.InUpdatedByUserAlias = USER;
            createOrUpdateRequest.InMetaTags = metaTags;
            createOrUpdateRequest.InAuthGroupName = IN_AUTH_GROUP_NAME;

            try
            {
                createOrUpdateReply = devBranchProxy.ActivityCategoryCreateOrUpdate(createOrUpdateRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblActivityCategory: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblActivityCategory: {0}", ex.Message);
            }

            Assert.IsNotNull(createOrUpdateReply, "ActivityCategoryCreateOrUpdateReplyDC object null");
            Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.StatusReply is null");
            Assert.AreEqual(SprocValues.REPLY_ERRORCODE_VALUE_OK, createOrUpdateReply.StatusReply.Errorcode, "createOrUpdateReply.StatusReply.Errorcode is not 0. Instead it is {0}.", createOrUpdateReply.StatusReply.Errorcode);
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorMessage, "createOrUpdateReply.StatusReply.ErrorMessage is not null");
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorGuid, "createOrUpdateReply.StatusReply.ErrorGuid is not null");

            int id = GetActivityCategoriesForValidName(name);
            //Sleep one second for next call
            Thread.Sleep(1000);
            GetActivityCategoriesForValidIDs(id);
            Assert.IsNotNull(getByNameReplyList, "getByNameReplyList is null.");
            Assert.AreEqual(1, getByNameReplyList.Count, "getByNameReplyList.Count is not 1.");
            Assert.IsNotNull(getByNameReplyList[0].StatusReply, "getByNameReplyList[0].StatusReply is null");
            Assert.AreEqual(0, getByNameReplyList[0].StatusReply.Errorcode, "StatusReply.Errorcode is not 0. Instead it is {1}.", getByNameReplyList[0].StatusReply.Errorcode);
            Assert.IsTrue(string.IsNullOrEmpty(getByNameReplyList[0].StatusReply.ErrorMessage), "Error Message is not null. Error Message: {0}", getByNameReplyList[0].StatusReply.ErrorMessage);
            Assert.IsTrue(string.IsNullOrEmpty(getByNameReplyList[0].StatusReply.ErrorGuid), "ErrorGuid is not null. ErrorGuid: {0}", getByNameReplyList[0].StatusReply.ErrorGuid);
            Assert.AreEqual(description, getByNameReplyList[0].Description, "Description did not get inserted correctly");
            Assert.AreEqual(name, getByNameReplyList[0].Name, "Name did not get inserted correctly");
            Assert.AreEqual(metaTags, getByNameReplyList[0].MetaTags, "MetaTags did not get inserted correctly");

            return id;
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblActivityCategory Table for Invalid IDs. id is invalid if it's not 0 or not already in the table.
        /// </summary>
        /// <param name="id">id to try to insert or update</param>
        /// <param name="name">name to try to insert or update</param>
        private void CreateOrUpdateActivityCategoriesForInvalidId(int id, string name)
        {
            bool isFaultException = false;

            createOrUpdateRequest = new ActivityCategoryCreateOrUpdateRequestDC();

            createOrUpdateReply = null;

            //Populate the request data
            createOrUpdateRequest.Incaller = IN_CALLER;
            createOrUpdateRequest.IncallerVersion = IN_CALLER_VERSION;
            createOrUpdateRequest.InId = id;
            createOrUpdateRequest.InGuid = Guid.NewGuid();
            createOrUpdateRequest.InName = name;
            createOrUpdateRequest.InDescription = TEST_STRING;
            createOrUpdateRequest.InInsertedByUserAlias = USER;
            createOrUpdateRequest.InUpdatedByUserAlias = USER;
            createOrUpdateRequest.InMetaTags = TEST_STRING;
            createOrUpdateRequest.InAuthGroupName = IN_AUTH_GROUP_NAME;

            try
            {
                createOrUpdateReply = devBranchProxy.ActivityCategoryCreateOrUpdate(createOrUpdateRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblActivityCategory: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblActivityCategory: {0}", ex.Message);
            }

            if (!isFaultException)
            {
                int errorConstant = GetErrorConstantInvalidIDForUpdate(id);

                Assert.IsNotNull(createOrUpdateReply, "ActivityCategoryCreateOrUpdateReplyDC object null");
                Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.StatusReply is null");
                Assert.AreEqual(createOrUpdateReply.StatusReply.Errorcode, errorConstant, "createOrUpdateReply.StatusReply. Errorcode is not {0}. Instead it is {1}.", errorConstant, createOrUpdateReply.StatusReply.Errorcode);
                Assert.IsNotNull(createOrUpdateReply.StatusReply.ErrorMessage, "createOrUpdateReply.StatusReply.ErrorMessage is null");
            }
        }

        /// <summary>
        /// Verifies that different WCF calls for the ltblActivityCategory Table can be run simultaneously
        /// </summary>
        private void VerifyConcurrencyTestingForActivityCategoriesByRunningThreads()
        {
            ThreadStart[] activityCategoriesThreadStarter = new ThreadStart[10];
            Thread[] activityCategoriesThread = new Thread[8];

            activityCategoriesThreadStarter[0] = delegate { GetActivityCategoriesForValidIDs(11); };
            activityCategoriesThreadStarter[1] = delegate { GetActivityCategoriesForValidIDs(13); };
            activityCategoriesThreadStarter[2] = delegate { VerifyCreateOrUpdateActivityCategoriesAndCleanup(); };
            activityCategoriesThreadStarter[3] = delegate { VerifyCreateOrUpdateActivityCategoriesWithNullIDAndCleanup(); };
            activityCategoriesThreadStarter[4] = delegate { GetActivityCategoriesForValidIDs(15); };
            activityCategoriesThreadStarter[5] = delegate { VerifyCreateOrUpdateActivityCategoriesAndCleanup(); };
            activityCategoriesThreadStarter[6] = delegate { VerifyCreateOrUpdateActivityCategoriesWithNullIDAndCleanup(); };
            activityCategoriesThreadStarter[7] = delegate { GetActivityCategoriesForValidIDs(16); };

            for (int i = 0; i < activityCategoriesThread.Length; i++)
            {
                activityCategoriesThread[i] = new Thread(activityCategoriesThreadStarter[i]);
            }

            for (int i = 0; i < activityCategoriesThread.Length; i++)
            {
                activityCategoriesThread[i].Start();
                activityCategoriesThread[i].Join();
            }
        }

        [WorkItem(20907)]
        [Description("Verify GET FROM ltblActivityCategory Table for Valid IDs")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetActivityCategoriesForValidIDS()
        {
            int startId = 2;
            int stopId = 9;
            for (int id = startId; id <= stopId; id++)
            {
                GetActivityCategoriesForValidIDs(id);
            }
        }

        [WorkItem(21026)]
        [Description("Verify GET FROM ltblActivityCategory Table for Invalid IDs")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetActivityCategoriesForInvalidIDs()
        {
            int[] nonExistingIDs = new int[] 
            {
                Int32.MinValue,
                -1,
                Int32.MaxValue
            };

            foreach (int nonExistingID in nonExistingIDs)
            {
                GetActivityCategoriesForInvalidIDs(nonExistingID);
            }
        }

     
        [WorkItem(20975)]
        [Description("Verify CreateOrUpdate FROM ltblActivityCategory Table with id = 0")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyCreateOrUpdateActivityCategories()
        {
            VerifyCreateOrUpdateActivityCategoriesAndCleanup();
        }

        [WorkItem(20977)]
        [Description("Verify CreateOrUpdate FROM ltblActivityCategory Table with null id")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyCreateOrUpdateActivityCategoriesWithNullID()
        {
            VerifyCreateOrUpdateActivityCategoriesWithNullIDAndCleanup();
        }

        [WorkItem(20976)]
        [Description("Verify CreateOrUpdate FROM ltblActivityCategory Table For Invalid Ids")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyCreateOrUpdateActivityCategoriesForInvalidId()
        {
            int[] invalidIDs = new int[]
            {
                -10,
                Int32.MaxValue,
                Int32.MinValue,
                -1
            };

            foreach (int id in invalidIDs)
            {
                // Check that it's not already there
                GetActivityCategoriesForInvalidIDs(id);

                // Create
                CreateOrUpdateActivityCategoriesForInvalidId(id, TEST_FIELD_NAME);

                // Do a get to make sure it worked
                GetActivityCategoriesForInvalidIDs(id);
            }
        }

        [WorkItem(20962)]
        [Description("Verify ConcurrencyTesting FROM ltblActivityCategory Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        [Ignore]
        public void VerifyConcurrencyTestingForActivityCategories()
        {
            VerifyConcurrencyTestingForActivityCategoriesByRunningThreads();
        }
    }
}

