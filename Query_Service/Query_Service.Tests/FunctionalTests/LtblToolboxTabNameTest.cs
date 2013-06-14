//-----------------------------------------------------------------------
// <copyright file="LtblToolBoxTabNameTest.cs" company="Microsoft">
// Copyright
// A test class that is used to verify service interaction with the ltblToolboxTabName table.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts;
using Query_Service.Tests.Common;

namespace Query_Service.Tests
{
    /// <summary>
    /// A test class that is used to verify service interaction with the ltblToolboxTabName table
    /// </summary>
    [TestClass]
    public class LtblToolBoxTabNameTest : QueryServiceTestBase
    {
        #region constants

        private const string TABLE_NAME = "ltblToolboxTabName";
        private const int MAX_NAME_SIZE = 30;

        #endregion

        #region Request Objects

        private ToolBoxItemsListGetRequestDC getRequest;
        private ToolBoxTabNameCreateOrUpdateRequestDC createOrUpdateRequest;
        private ToolBoxTabNameDeleteRequestDC deleteRequest;

        #endregion

        #region Reply Objects

        private ToolBoxItemsListGetReplyDC getReply;
        private ToolBoxTabNameCreateOrUpdateReplyDC createOrUpdateReply;
        private ToolBoxTabNameDeleteReplyDC deleteReply;

        #endregion

        #region Private Methods

        /// <summary>
        /// Verify GET FROM ltblToolboxTabName Table for Valid IDs
        /// </summary>
        /// <param name="id">id of row to do a get on</param>
        private void GetToolBoxTabNameForValidIDs(int validID)
        {
            getRequest = new ToolBoxItemsListGetRequestDC();
            getReply = null;

            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            getRequest.InId = validID;

            try
            {
                getReply = proxy.ToolboxTabNameGet(getRequest);
            }
            catch (FaultException ex)
            {
                Assert.Fail("Caught WCF FaultExceptionException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
            }
            catch (Exception e)
            {
                Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
            }

            Assert.IsNotNull(getReply, "ToolboxTabNameGetReplyDC object null");
            Assert.IsNotNull(getReply.List, "getReply.List is null");
            Assert.AreEqual(1, getReply.List.Count, "Get returned the wrong number of entries. It should have returned 1 but instead return {0}", getReply.List.Count);
            Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(getRequest.InId, getReply.List[0].Id, "Service returned the wrong record");
        }

        /// <summary>
        /// Verify GET FROM ltblToolboxTabName Table for a valid name
        /// </summary>
        /// <param name="name">name from row to do a get on</param>
        /// <returns>returns the id of this row</returns>
        private int GetToolboxTabNameForValidNameForMaxID(string name)
        {
            getRequest = new ToolBoxItemsListGetRequestDC();
            getReply = null;

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.InName = name;

            try
            {
                getReply = proxy.ToolboxTabNameGet(getRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from ltblToolboxTabName: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from ltblToolboxTabName: {0}", ex.Message);
            }

            Assert.IsNotNull(getReply, "ltblToolboxTabNameGetReplyDC object null");
            Assert.IsNotNull(getReply.List, "getReply.List is null");
            Assert.AreEqual(1, getReply.List.Count, "Get returned the wrong number of entries. InName: {0}. It should have returned 1 but instead returned {1}.", name, getReply.List.Count);
            Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
            int index = getReply.List.Count - 1;
            int id = getReply.List[index].Id;

            return id;
        }

        /// <summary>
        /// Verify GET FROM ltblToolboxTabName Table for Invalid IDs
        /// </summary>
        /// <param name="nonExistingID">id of row to do a get on</param>
        private void GetToolBoxTabNameForInValidIDs(int inValidID)
        {
            bool isFaultException = false;

            getRequest = new ToolBoxItemsListGetRequestDC();
            getReply = null;

            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            getRequest.InId = inValidID;

            try
            {
                getReply = proxy.ToolboxTabNameGet(getRequest);
            }
            // Task 20943. Add fault exception validation.
            //catch (FaultException<www.microsoft.com.practices.EnterpriseLibrary._2007._01.wcf.validation.ValidationFault> exc)
            //{
            //    if (getRequest.InId < 0)
            //    {
            //        Assert.IsNotNull(exc.Detail.Details);
            //        Assert.AreEqual(1, exc.Detail.Details.Count);
            //        Assert.IsNotNull(exc.Detail.Details[0].Message);
            //        Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_INID_MSG, exc.Detail.Details[0].Message);
            //        isFaultException = true;
            //    }
            //    else
            //    {
            //        Assert.Fail("Failed to get data from ltblToolboxTabName: {0}", exc.Message);
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
                int errorConstant = GetErrorConstantInvalidID(inValidID);

                Assert.IsNotNull(getReply, "getReply is null.");
                Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
                Assert.AreEqual(0, getReply.List.Count, "Service returned wrong number of records. InId= {0}. It should have returned 0 but instead returned {1}.", inValidID, getReply.List.Count);
                Assert.IsNotNull(getReply.StatusReply.ErrorMessage, "Error Message is null");
                Assert.AreEqual(errorConstant, getReply.StatusReply.Errorcode, "Service returned unexpected error code: {0}. It should have returned {1}", getReply.StatusReply.Errorcode, errorConstant);
            }
        }

        /// <summary>
        /// Verify GET FROM ltblToolboxTabName Table for softDeleted IDs
        /// </summary>
        /// <param name="softDeletedIncode">incode of row to do a get on</param>
        private void GetToolBoxTabNameForSoftDeletedIDs(int softDeletedIncode)
        {
            getRequest = new ToolBoxItemsListGetRequestDC();

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.InId = softDeletedIncode;

            try
            {
                getReply = proxy.ToolboxTabNameGet(getRequest);
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

        /// <summary>
        /// Verify DELETE FROM ltblToolboxTabName Table for Valid IDs. Will set softdelete to 1. Afterwards will do a cleanup.
        /// </summary>
        /// <param name="id">id of row to do a delete on</param>
        private void VerifyDeleteToolboxTabNameForValidIDsAndCleanup(int id)
        {
            DeleteToolboxTabNameForValidIDs(id);

            // put the soft delete back in so other tests won't be affected
            UpdateSoftDelete(id.ToString(), TABLE_NAME);

            GetToolBoxTabNameForValidIDs(id);
        }

        /// <summary>
        /// Verify DELETE FROM ltblToolboxTabName Table for Valid IDs. Will set softdelete to 1.
        /// </summary>
        /// <param name="id">id of row to do a delete on</param>
        private void DeleteToolboxTabNameForValidIDs(int id)
        {
            deleteRequest = new ToolBoxTabNameDeleteRequestDC();
            deleteReply = null;

            deleteRequest.Incaller = IN_CALLER;
            deleteRequest.IncallerVersion = IN_CALLER_VERSION;

            deleteRequest.InId = id;

            try
            {
                deleteReply = proxy.ToolBoxTabNameDelete(deleteRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to delete data from ltblToolboxTabName: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to delete data from ltblToolboxTabName: {0}", ex.Message);
            }

            Assert.IsNotNull(deleteReply, "ToolboxTabNameGetReplyDC object null");
            Assert.IsNotNull(deleteReply.StatusReply, "deleteReply.StatusReply is null");
            Assert.AreEqual(0, deleteReply.StatusReply.Errorcode, "Delete operation not successful.");

            // Now check to see if we have that record in the table
            GetToolBoxTabNameForSoftDeletedIDs(id);
        }

        /// <summary>
        /// Verify DELETE FROM ltblToolboxTabName Table for Invalid IDs
        /// </summary>
        /// <param name="nonExistingID">id of row to do a delete on. This id does not exist in the table</param>
        private void DeleteToolBoxTabNameForInvalidIDs(int nonExistingID)
        {
            bool isFaultException = false;

            deleteRequest = new ToolBoxTabNameDeleteRequestDC();
            deleteReply = null;

            deleteRequest.Incaller = IN_CALLER;
            deleteRequest.IncallerVersion = IN_CALLER_VERSION;

            deleteRequest.InId = nonExistingID;

            try
            {
                deleteReply = proxy.ToolBoxTabNameDelete(deleteRequest);
            }
            // Task 20943. Add fault exception validation.
            //catch (FaultException<www.microsoft.com.practices.EnterpriseLibrary._2007._01.wcf.validation.ValidationFault> exc)
            //{
            //    if (deleteRequest.InId < 0)
            //    {
            //        Assert.IsNotNull(exc.Detail.Details);
            //        Assert.AreEqual(1, exc.Detail.Details.Count);
            //        Assert.IsNotNull(exc.Detail.Details[0].Message);
            //        Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_INID_MSG, exc.Detail.Details[0].Message);
            //        isFaultException = true;
            //    }
            //    else if (deleteRequest.InId == 0 && (deleteRequest.InName == null || deleteRequest.InName == string.Empty))
            //    {
            //        Assert.IsNotNull(exc.Detail.Details);
            //        Assert.AreEqual(1, exc.Detail.Details.Count);
            //        Assert.IsNotNull(exc.Detail.Details[0].Message);
            //        Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_InIdInNameCannotBeNull_MSG, exc.Detail.Details[0].Message);
            //        isFaultException = true;
            //    }
            //    else
            //    {
            //        Assert.Fail("Failed to delete data from ltblToolboxTabName: {0}", exc.Message);
            //    }
            //}
            catch (FaultException e)
            {
                Assert.Fail("Failed to delete data from ltblToolboxTabName: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to delete data from ltblToolboxTabName: {0}", ex.Message);
            }

            if (!isFaultException)
            {
                int errorcode = (nonExistingID == 0) ? CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_INIDINNAMECANNOTBENULL_ID : GetErrorConstantDeleteInvalidID(nonExistingID);
                
                Assert.IsNotNull(deleteReply, "ToolBoxTabNameGetReplyDC object null");
                Assert.IsNotNull(deleteReply.StatusReply, "deleteReply.StatusReply is null");
                Assert.AreEqual(errorcode, deleteReply.StatusReply.Errorcode, "Delete operation not successful.");
                Assert.IsNotNull(deleteReply.StatusReply.ErrorMessage, "Error Message is null");
            }
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblToolboxTabName Table and then cleanup. This will be an insert as id is set to 0.
        /// </summary>
        private void VerifyCreateOrUpdateToolboxTabNameAndCleanup()
        {
            string testFieldName = (TEST_FIELD_NAME + Guid.NewGuid()).Substring(0, MAX_NAME_SIZE);

            // Create
            int id = CreateToolBoxTabNameWithIdIsZero(testFieldName);

            // Update
            UpdateToolboxTabName(id, testFieldName);

            // Delete if it was created
            DeleteToolboxTabNameForValidIDs(id);
            HardDeleteThisRow(id, TABLE_NAME, TEST_DATABASE);

            GetToolBoxTabNameForInValidIDs(id);
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblToolboxTabName Table. This will be an insert as id is set to 0.
        /// </summary>
        /// <param name="name">name to do a create or update on.</param>
        /// <returns>returns the id of this row</returns>
        private int CreateToolBoxTabNameWithIdIsZero(string name)
        {
            int id = 0;

            CreateOrUpdateToolBoxTabName(id, name);

            int newId = GetToolboxTabNameForValidNameForMaxID(name);
            GetToolBoxTabNameForValidIDs(newId);
            Assert.IsNotNull(getReply, "getReplyList is null.");
            Assert.IsNotNull(getReply.List, "getReplyList is null.");
            Assert.AreNotEqual(0, getReply.List.Count, " getReply.List.Count is 0.");
            Assert.AreEqual(name, getReply.List[0].Name, "Name did not get inserted or updated correctly");

            return newId;
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblToolboxTabName Table. This will be an update as id != 0.
        /// </summary>
        /// <param name="id"> id to do a create or update on. id is 0 if it will be a create.</param>
        /// <param name="name">name to do a create or update on.</param>
        private void UpdateToolboxTabName(int id, string name)
        {
            Assert.AreNotEqual(0, id, "id = 0 should not be passed into this method, as it is an insert instead of an update");

            CreateOrUpdateToolBoxTabName(id, name);

            GetToolBoxTabNameForValidIDs(id);
            Assert.IsNotNull(getReply, "getReplyList is null.");
            Assert.IsNotNull(getReply.List, "getReplyList is null.");
            Assert.AreNotEqual(0, getReply.List.Count, " getReply.List.Count is 0.");
            Assert.AreEqual(name, getReply.List[0].Name, "Name did not get inserted or updated correctly");
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblToolboxTabName Table
        /// </summary>
        /// <param name="id"> id to do a create or update on. id is 0 if it will be a create.</param>
        /// <param name="name">name to do a create or update on.</param>
        private void CreateOrUpdateToolBoxTabName(int id, string name)
        {
            createOrUpdateRequest = new ToolBoxTabNameCreateOrUpdateRequestDC();

            createOrUpdateReply = null;

            //Populate the request data
            createOrUpdateRequest.Incaller = IN_CALLER;
            createOrUpdateRequest.IncallerVersion = IN_CALLER_VERSION;
            createOrUpdateRequest.InId = id;
            createOrUpdateRequest.InTabName = name;
            createOrUpdateRequest.InInsertedByUserAlias = USER;
            createOrUpdateRequest.InUpdatedByUserAlias = USER;

            try
            {
                createOrUpdateReply = proxy.ToolBoxTabNameCreateOrUpdate(createOrUpdateRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblToolboxTabName: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblToolboxTabName: {0}", ex.Message);
            }

            Assert.IsNotNull(createOrUpdateReply, "ToolBoxTabNameCreateOrUpdateReplyDC object null");
            Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.StatusReply is null");
            Assert.AreEqual(0, createOrUpdateReply.StatusReply.Errorcode, "createOrUpdateReply.StatusReply.Errorcode is not 0. Instead it is {0}.", createOrUpdateReply.StatusReply.Errorcode);
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorMessage, "createOrUpdateReply.StatusReply.ErrorMessage is not null");
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorGuid, "createOrUpdateReply.StatusReply.ErrorGuid is not null");
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblToolboxTabName Table and then cleanup. This will be a create since id is null.
        /// </summary>
        private void VerifyCreateOrUpdateToolboxTabNameWithNullIDAndCleanup()
        {
            // Create with null id
            int id = CreateToolboxTabNameWithNullID(TEST_FIELD_NAME);

            // Update
            UpdateToolboxTabName(id, TEST_FIELD_NAME);

            // Delete if it was created
            DeleteToolboxTabNameForValidIDs(id);
            HardDeleteThisRow(id, TABLE_NAME, TEST_DATABASE);

            GetToolBoxTabNameForInValidIDs(id);
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblToolboxTabName Table. This will be a create since id is null.
        /// </summary>
        /// <param name="name">name to be created</param>
        /// <returns>returns the id created</returns>
        private int CreateToolboxTabNameWithNullID(string name)
        {
            createOrUpdateRequest = new ToolBoxTabNameCreateOrUpdateRequestDC();

            createOrUpdateReply = null;

            //Populate the request data
            createOrUpdateRequest.Incaller = IN_CALLER;
            createOrUpdateRequest.IncallerVersion = IN_CALLER_VERSION;
            createOrUpdateRequest.InTabName = name;
            createOrUpdateRequest.InInsertedByUserAlias = USER;
            createOrUpdateRequest.InUpdatedByUserAlias = USER;

            try
            {
                createOrUpdateReply = proxy.ToolBoxTabNameCreateOrUpdate(createOrUpdateRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblToolboxTabName: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblToolboxTabName: {0}", ex.Message);
            }

            Assert.IsNotNull(createOrUpdateReply, "ToolBoxTabNameCreateOrUpdateReplyDC object null");
            Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.StatusReply is null");
            Assert.AreEqual(0, createOrUpdateReply.StatusReply.Errorcode, "createOrUpdateReply.StatusReply.Errorcode is not 0. Instead it is {0}.", createOrUpdateReply.StatusReply.Errorcode);
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorMessage, "createOrUpdateReply.StatusReply.ErrorMessage is not null");
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorGuid, "createOrUpdateReply.StatusReply.ErrorGuid is not null");

            int id = GetToolboxTabNameForValidNameForMaxID(name);
            GetToolBoxTabNameForValidIDs(id);
            Assert.AreEqual(name, getReply.List[0].Name, "Name did not get inserted or updated corrected");

            return id;
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblToolboxTabName Table for Invalid IDs. id is invalid if it's not 0 or not already in the table.
        /// </summary>
        /// <param name="id">id to try to insert or update</param>
        /// <param name="name">name to try to insert or update</param>
        private void CreateOrUpdateToolBoxTabNameForInvalidId(int id, string name)
        {
            bool isFaultException = false;

            createOrUpdateRequest = new ToolBoxTabNameCreateOrUpdateRequestDC();

            createOrUpdateReply = null;

            //Populate the request data
            createOrUpdateRequest.Incaller = IN_CALLER;
            createOrUpdateRequest.IncallerVersion = IN_CALLER_VERSION;
            createOrUpdateRequest.InId = id;
            createOrUpdateRequest.InTabName = name;
            createOrUpdateRequest.InInsertedByUserAlias = USER;
            createOrUpdateRequest.InUpdatedByUserAlias = USER;

            try
            {
                createOrUpdateReply = proxy.ToolBoxTabNameCreateOrUpdate(createOrUpdateRequest);
            }
            // Task 20943. Add fault exception validation.
            //catch (FaultException<www.microsoft.com.practices.EnterpriseLibrary._2007._01.wcf.validation.ValidationFault> exc)
            //{
            //    Assert.IsNotNull(exc.Detail.Details);
            //    Assert.AreEqual(1, exc.Detail.Details.Count);
            //    Assert.IsNotNull(exc.Detail.Details[0].Message);
            //    if (createOrUpdateRequest.InId < 0)
            //    {
            //        Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_INID_MSG, exc.Detail.Details[0].Message);
            //        isFaultException = true;
            //    }
            //    else if (createOrUpdateRequest.InId == 0)
            //    {
            //        if (createOrUpdateRequest.InTabName == null || createOrUpdateRequest.InTabName == string.Empty)
            //        {
            //            Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_INNAME_MSG, exc.Detail.Details[0].Message);
            //            isFaultException = true;
            //        }
            //        else if (createOrUpdateRequest.InInsertedByUserAlias == null || createOrUpdateRequest.InInsertedByUserAlias == string.Empty)
            //        {
            //            Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_ININSERTEDBYUSERALIAS_MSG, exc.Detail.Details[0].Message);
            //            isFaultException = true;
            //        }
            //    }
            //    if (!isFaultException)
            //    {
            //        Assert.Fail("Failed to get data from ltblToolboxTabName: {0}", exc.Message);
            //    }
            //}
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblToolboxTabName: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblToolboxTabName: {0}", ex.Message);
            }

            if (!isFaultException)
            {
                int errorConstant = GetErrorConstantInvalidIDForUpdate(id);

                Assert.IsNotNull(createOrUpdateReply, "ToolBoxTabNameCreateOrUpdateRequestDC object null");
                Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.Status is null");
                Assert.AreEqual(errorConstant, createOrUpdateReply.StatusReply.Errorcode, "Returned the wrong status error code. InId: {0}", id);
                Assert.IsNotNull(createOrUpdateReply.StatusReply.ErrorMessage, "createOrUpdateReply.StatusReply.ErrorMessage is null");
            }
        }

        /// <summary>
        /// Verifies that different WCF calls for the ltblToolboxTabName Table can be run simultaneously
        /// </summary>
        private void VerifyConcurrencyTestingForToolBoxTabNameByRunningThreads()
        {
            ThreadStart[] toolBoxTabNameThreadStarter = new ThreadStart[10];
            Thread[] toolBoxTabNameThread = new Thread[10];

            toolBoxTabNameThreadStarter[0] = delegate { GetToolBoxTabNameForValidIDs(1); };
            toolBoxTabNameThreadStarter[1] = delegate { GetToolBoxTabNameForValidIDs(3); };
            toolBoxTabNameThreadStarter[2] = delegate { VerifyDeleteToolboxTabNameForValidIDsAndCleanup(2); };
            toolBoxTabNameThreadStarter[3] = delegate { VerifyCreateOrUpdateToolboxTabNameAndCleanup(); };
            toolBoxTabNameThreadStarter[4] = delegate { VerifyCreateOrUpdateToolboxTabNameWithNullIDAndCleanup(); };
            toolBoxTabNameThreadStarter[5] = delegate { GetToolBoxTabNameForValidIDs(4); };
            toolBoxTabNameThreadStarter[6] = delegate { VerifyCreateOrUpdateToolboxTabNameAndCleanup(); };
            toolBoxTabNameThreadStarter[7] = delegate { VerifyDeleteToolboxTabNameForValidIDsAndCleanup(5); };
            toolBoxTabNameThreadStarter[8] = delegate { VerifyCreateOrUpdateToolboxTabNameWithNullIDAndCleanup(); };
            toolBoxTabNameThreadStarter[9] = delegate { GetToolBoxTabNameForValidIDs(1); };

            for (int i = 0; i < toolBoxTabNameThread.Length; i++)
            {
                toolBoxTabNameThread[i] = new Thread(toolBoxTabNameThreadStarter[i]);
            }

            for (int i = 0; i < toolBoxTabNameThread.Length; i++)
            {
                toolBoxTabNameThread[i].Start();
                toolBoxTabNameThread[i].Join();
            }
        }

        #endregion

        #region Test Methods

        [WorkItem(20913)]
        [Description("Verify GET FROM ltblToolboxTabName Table for Valid IDs")]
        [Owner(TEST_OWNER)]
        //[TestCategory(Common.TestCategory.BVT)]
        [TestCategory(Common.TestCategory.Smoke)]
        [TestMethod]
        public void VerifyGetToolBoxTabNameForValidIDs()
        {
            int[] validIDs = new int[]
            {
                1, 2, 3, 4                       
            };

            foreach (int validID in validIDs)
            {
                GetToolBoxTabNameForValidIDs(validID);
            }
        }

        [WorkItem(22198)]
        [Description("Verify GET FROM ltblToolboxTabName Table for Invalid IDs")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetToolBoxTabNameForInValidIDs()
        {
            int[] inValidIDs = new int[]
            {
               Int32.MinValue, 
               -1, 
               1000, 
               Int32.MaxValue 
            };

            foreach (int inValidID in inValidIDs)
            {
                GetToolBoxTabNameForInValidIDs(inValidID);
            }
        }

        [WorkItem(21020)]
        [Description("Verify DELETE FROM ltblToolboxTabName Table for valid IDs")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyDeleteToolboxTabNameForValidIDs()
        {
            int id = 1;

            VerifyDeleteToolboxTabNameForValidIDsAndCleanup(id);
        }

        [WorkItem(22214)]
        [Description("Verify DELETE FROM ltblToolboxTabName Table for invalid IDs")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyDeleteToolboxTabNameForInvalidIDs()
        {
            int[] nonExistingIDs = new int[]
            {
                0,  
                1000, 
                Int32.MaxValue, 
                Int32.MinValue,
                -1
            };

            foreach (int nonExistingID in nonExistingIDs)
            {
                DeleteToolBoxTabNameForInvalidIDs(nonExistingID);
            }
        }

        [WorkItem(20997)]
        [Description("Verify CreateOrUpdate FROM ltblToolboxTabName Table with ID = 0")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyCreateOrUpdateToolboxTabName()
        {
            VerifyCreateOrUpdateToolboxTabNameAndCleanup();
        }

        [WorkItem(20999)]
        [Description("Verify CreateOrUpdate FROM ltblToolboxTabName Table with Null ID")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyCreateOrUpdateToolboxTabNameWithNullID()
        {
            VerifyCreateOrUpdateToolboxTabNameWithNullIDAndCleanup();
        }

        [WorkItem(20998)]
        [Description("Verify CreateOrUpdate FROM ltblToolboxTabName Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyCreateOrUpdateToolBoxTabNameForInvalidID()
        {
            int[] invalidIDs = new int[]
            {
                150, 
                Int32.MaxValue,
                Int32.MinValue,
                -1
            };

            foreach (int id in invalidIDs)
            {
                // Create
                GetToolBoxTabNameForInValidIDs(id);

                // Check that it's not already there
                CreateOrUpdateToolBoxTabNameForInvalidId(id, TEST_STRING);

                // Do a get to make sure it worked
                GetToolBoxTabNameForInValidIDs(id);
            }
        }

        [WorkItem(20969)]
        [Description("Verify ConcurrencyTesting FROM ltblToolboxTabName Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyConcurrencyTestingForToolBoxTabName()
        {
            VerifyConcurrencyTestingForToolBoxTabNameByRunningThreads();
        }

        #endregion
    }
}
