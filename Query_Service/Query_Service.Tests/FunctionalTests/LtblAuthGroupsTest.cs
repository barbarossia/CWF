//-----------------------------------------------------------------------
// <copyright file="LtblAuthGroupsTest.cs" company="Microsoft">
// Copyright
// A Test Class for ltblAuthGroupsTest table.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;
using Query_Service.Tests.Common;

namespace Query_Service.Tests
{
    /// <summary>
    /// A Test Class for ltblAuthGroupsTest table
    /// </summary>
    [TestClass]
    public class LtblAuthGroupsTest : QueryServiceTestBase
    {
        #region constants

        private const string TABLE_NAME = "ltblAuthGroups";
        private const string VALID_GUID = "5089D7CB-C404-419D-B153-0C3CD989FE01";
        private const string VALID_NAME = "pqocwfdevuser";

        #endregion

        #region Request Objects

        private AuthGroupsGetRequestDC getRequest;
        private AuthGroupsDeleteRequestDC deleteRequest;
        private AuthGroupsCreateOrUpdateRequestDC createOrUpdateRequest;

        #endregion

        #region Reply Objects

        AuthGroupsGetReplyDC getReply;
        AuthGroupsCreateOrUpdateReplyDC createOrUpdateReply;
        AuthGroupsDeleteReplyDC deleteReply;

        #endregion

        #region Private Methods

        /// <summary>
        /// Verify GET FROM ltblAuthGroup Table for Valid IDs
        /// </summary>
        /// <param name="id">id of row to do a get on</param>
        private void GetAuthGroupForValidIDs(int id)
        {
            getRequest = new AuthGroupsGetRequestDC();

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            getRequest.InId = id;

            try
            {
                getReply = proxy.AuthGroupsGet(getRequest);
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
            Assert.IsNotNull(getReply, "getReply is null.");
            Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(1, getReply.List.Count, "Service returned wrong number of records.InId = {0}. It should have returned 1 but instead returned {1}.", id, getReply.List.Count);
            Assert.IsNull(getReply.StatusReply.ErrorMessage, "Error Message is not null. Error Message: {0}", getReply.StatusReply.ErrorMessage);
            Assert.IsNull(getReply.StatusReply.ErrorGuid, "ErrorGuid is not null. ErrorGuid: {0}", getReply.StatusReply.ErrorGuid);
            Assert.AreEqual(getReply.List[0].Id, getRequest.InId, "Service returned the wrong record. Expected Id: {0}, Actual Id: {1}", getRequest.InId, getReply.List[0].Id);
        }

        
        /// <summary>
        /// Verify GET FROM ltblAuthGroup Table for Valid guid
        /// </summary>
        /// <param name="guid">guid of row to do a get on</param>
        private void GetAuthGroupForValidGuid(Guid guid)
        {
            getRequest = new AuthGroupsGetRequestDC();

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            getRequest.InGuid = guid;

            try
            {
                getReply = proxy.AuthGroupsGet(getRequest);
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
            Assert.IsNotNull(getReply, "getReply is null.");
            Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(1, getReply.List.Count, "Service returned wrong number of records.InGuid = {0}. It should have returned 1 but instead returned {1}.", guid, getReply.List.Count);
            Assert.IsNull(getReply.StatusReply.ErrorMessage, "Error Message is not null. Error Message: {0}", getReply.StatusReply.ErrorMessage);
            Assert.IsNull(getReply.StatusReply.ErrorGuid, "ErrorGuid is not null. ErrorGuid: {0}", getReply.StatusReply.ErrorGuid);
            Assert.AreEqual(getReply.List[0].Guid, getRequest.InGuid, "Service returned the wrong record. Expected guid: {0}, Actual guid: {1}", getRequest.InGuid, getReply.List[0].Guid);
        }

        /// <summary>
        /// Verify GET FROM ltblAuthGroup Table for a valid name
        /// </summary>
        /// <param name="name">name from row to do a get on</param>
        /// <returns>returns the id of this row</returns>
        private int GetAuthGroupForValidNameForMaxID(string name)
        {
            getRequest = new AuthGroupsGetRequestDC();

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            getRequest.InName = name;

            try
            {
                getReply = proxy.AuthGroupsGet(getRequest);
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
            Assert.IsNotNull(getReply, "getReply is null.");
            Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(1, getReply.List.Count, "Service returned wrong number of records.inName = {0}. It should have returned 1 but instead returned {1}.", name, getReply.List.Count);
            Assert.IsNull(getReply.StatusReply.ErrorMessage, "Error Message is not null. Error Message: {0}", getReply.StatusReply.ErrorMessage);
            Assert.IsNull(getReply.StatusReply.ErrorGuid, "ErrorGuid is not null. ErrorGuid: {0}", getReply.StatusReply.ErrorGuid);

            int index = getReply.List.Count - 1;
            int id = getReply.List[index].Id;
            return id;
        }

        /// <summary>
        /// Verify GET FROM ltblAuthGroup Table for Invalid IDs
        /// </summary>
        /// <param name="nonExistingID">id of row to do a get on</param>
        private void GetAuthGroupForInValidIDs(int nonExistingID)
        {
            bool isFaultException = false;

            getRequest = new AuthGroupsGetRequestDC();

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.InId = nonExistingID;

            try
            {
                getReply = proxy.AuthGroupsGet(getRequest);
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
            //        Assert.Fail("Failed to get data from ltblActivityCategory: {0}", exc.Message);
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
                int errorConstant = GetErrorConstantInvalidID(nonExistingID);

                Assert.IsNotNull(getReply, "getReply is null.");
                Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
                Assert.AreEqual(0, getReply.List.Count, "Service returned wrong number of records. InId= {0}. It should have returned 0 but instead returned {1}.", nonExistingID, getReply.List.Count);
                Assert.IsNotNull(getReply.StatusReply.ErrorMessage, "Error Message is null");
                Assert.AreEqual(errorConstant, getReply.StatusReply.Errorcode, "Service returned unexpected error code. Expected: {0}, Returned: {1}", errorConstant, getReply.StatusReply.Errorcode);
            }
        }

        /// <summary>
        /// Verify GET FROM ltblAuthGroup Table for softDeleted IDs
        /// </summary>
        /// <param name="softDeletedID">id of row to do a get on</param>
        private void GetAuthGroupForSoftDeletedIDs(int softDeletedID)
        {
            getRequest = new AuthGroupsGetRequestDC();

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.InId = softDeletedID;

            try
            {
                getReply = proxy.AuthGroupsGet(getRequest);
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
            Assert.AreEqual(0, getReply.List.Count, "Service returned wrong number of records. InId= {0}. It should have returned 0 but instead returned {1}.", softDeletedID, getReply.List.Count);
            Assert.IsNotNull(getReply.StatusReply.ErrorMessage, "Error Message is null");
            Assert.AreEqual(errorConstant, getReply.StatusReply.Errorcode, "Service returned unexpected error code. Expected: {0}, Returned: {1}", errorConstant, getReply.StatusReply.Errorcode);
        }

        /// <summary>
        /// Verify DELETE FROM ltblAuthGroup Table for Valid IDs. Will set softdelete to 1. Afterwards do a cleanup.
        /// </summary>
        /// <param name="id">id of row to do a delete on</param>
        private void VerifyDeleteAuthGroupForValidIDsAndCleanup(int id)
        {
            DeleteAuthGroupsForValidIDs(id);

            // put the soft delete back in so other tests won't be affected
            UpdateSoftDelete(id.ToString(), TABLE_NAME);

            GetAuthGroupForValidIDs(id);
        }

        /// <summary>
        /// Verify DELETE FROM ltblAuthGroup Table for a Valid Name. Will set softdelete to 1. Afterwards do a cleanup.
        /// </summary>
        /// <param name="name">name of row to do a delete on</param>
        private void VerifyDeleteAuthGroupForValidNameAndCleanup(string name)
        {
            int id = DeleteAuthGroupsForValidName(name);

            // put the soft delete back in so other tests won't be affected
            UpdateSoftDelete(id.ToString(), TABLE_NAME);

            GetAuthGroupForValidIDs(id);
        }

        /// <summary>
        /// Verify DELETE FROM ltblAuthGroup Table for Valid IDs. Will set softdelete to 1.
        /// </summary>
        /// <param name="id">id of row to do a delete on</param>
        private void DeleteAuthGroupsForValidIDs(int id)
        {
            deleteRequest = new AuthGroupsDeleteRequestDC();
            deleteReply = null;

            deleteRequest.Incaller = IN_CALLER;
            deleteRequest.IncallerVersion = IN_CALLER_VERSION;
            deleteRequest.InId = id;

            try
            {
                deleteReply = proxy.AuthGroupsDelete(deleteRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to delete data from ltblAuthGroups: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to delete data from ltblAuthGroups: {0}", ex.Message);
            }

            Assert.IsNotNull(deleteReply, "AuthGroupsGetReplyDC object null");
            Assert.IsNotNull(deleteReply.StatusReply, "deleteReply.StatusReply is null");
            Assert.AreEqual(0, deleteReply.StatusReply.Errorcode, "Delete operation not successful.");

            // Now check to see if we don't have that record in the table
            GetAuthGroupForSoftDeletedIDs(id);
        }

        /// <summary>
        /// Verify DELETE FROM ltblAuthGroup Table for Valid Name. Will set softdelete to 1.
        /// </summary>
        /// <param name="name">name of row to do a delete on</param>
        /// <returns>returns the id of this row</returns>
        private int DeleteAuthGroupsForValidName(string name)
        {
            int id = GetAuthGroupForValidNameForMaxID(name);

            deleteRequest = new AuthGroupsDeleteRequestDC();
            deleteReply = null;

            deleteRequest.Incaller = IN_CALLER;
            deleteRequest.IncallerVersion = IN_CALLER_VERSION;
            deleteRequest.InName = name;

            try
            {
                deleteReply = proxy.AuthGroupsDelete(deleteRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to delete data from ltblAuthGroups: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to delete data from ltblAuthGroups: {0}", ex.Message);
            }

            Assert.IsNotNull(deleteReply, "AuthGroupsGetReplyDC object null");
            Assert.IsNotNull(deleteReply.StatusReply, "deleteReply.StatusReply is null");
            Assert.AreEqual(0, deleteReply.StatusReply.Errorcode, "Delete operation not successful.");

            // Now check to see if we don't have that record in the table
            GetAuthGroupForSoftDeletedIDs(id);

            return id;
        }

        /// <summary>
        /// Verify DELETE FROM ltblAuthGroup Table for Invalid IDs
        /// </summary>
        /// <param name="id">id of row to do a delete on. This id does not exist in the table</param>
        private void DeleteAuthGroupsForInvalidIDs(int id)
        {
            bool isFaultException = false;

            deleteRequest = new AuthGroupsDeleteRequestDC();
            deleteReply = null;

            deleteRequest.Incaller = IN_CALLER;
            deleteRequest.IncallerVersion = IN_CALLER_VERSION;
            deleteRequest.InId = id;

            try
            {
                deleteReply = proxy.AuthGroupsDelete(deleteRequest);
            }
            // Task 20943. Add fault exception validation.
            //catch (FaultException<www.microsoft.com.practices.EnterpriseLibrary._2007._01.wcf.validation.ValidationFault> exc)
            //{
            //    Assert.IsNotNull(exc.Detail.Details);
            //    Assert.AreEqual(1, exc.Detail.Details.Count);
            //    Assert.IsNotNull(exc.Detail.Details[0].Message);
            //    if (deleteRequest.InId < 0)
            //    {  
            //        Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_INID_MSG, exc.Detail.Details[0].Message);
            //        isFaultException = true;
            //    }
            //    else if ((deleteRequest.InId == 0) && (deleteRequest.InGuid == null || deleteRequest.InGuid.CompareTo(Guid.Empty) == 0) && (deleteRequest.InName == null || deleteRequest.InName == string.Empty))
            //    {
            //        Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_InIdInNameInGuidCannotAllBeNull_MSG, exc.Detail.Details[0].Message);
            //        isFaultException = true;
            //    }
            //    else
            //    {
            //        Assert.Fail("Failed to delete data from ltblAuthGroups: {0}", exc.Message);
            //    }
            //}
            catch (FaultException e)
            {
                Assert.Fail("Failed to delete data from ltblAuthGroups: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to delete data from ltblAuthGroups: {0}", ex.Message);
            }

            if (!isFaultException)
            {
                int errorcode = GetErrorConstantDeleteInvalidID(id);
                // special case if id = 0
                if (id == 0)
                {
                    errorcode = CWF.Constants.SprocValues.DELETE_NOPARAMETERS;
                }

                Assert.IsNotNull(deleteReply, "AuthGroupsGetReplyDC object null");
                Assert.IsNotNull(deleteReply.StatusReply, "deleteReply.StatusReply is null");
                Assert.AreEqual(errorcode, deleteReply.StatusReply.Errorcode, "Delete operation not successful.");
                Assert.IsNotNull(deleteReply.StatusReply.ErrorMessage, "Error Message is null");
            }
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblAuthGroup Table and then cleanup. This will be an insert as id is set to 0.
        /// </summary>
        private void VerifyCreateOrUpdateAuthGroupsAndCleanup()
        {
            string testFieldName = TEST_FIELD_NAME + Guid.NewGuid();

            // Create for id = 0
            int id = CreateAuthGroupsWithIdIsZero(testFieldName);

            // Update
            VerifyUpdateAuthGroups(id, testFieldName);

            // Delete if it was created
            DeleteAuthGroupsForValidIDs(id);
            HardDeleteThisRow(id, TABLE_NAME, TEST_DATABASE);

            GetAuthGroupForInValidIDs(id);
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblAuthGroup Table. This will be an insert as id is set to 0.
        /// </summary>
        /// <param name="name">name to do a create or update on.</param>
        /// <returns>the newly created id</returns>
        private int CreateAuthGroupsWithIdIsZero(string name)
        {
            CreateOrUpdateAuthGroups(0, name);

            int newId = GetAuthGroupForValidNameForMaxID(name);
            GetAuthGroupForValidIDs(newId);
            Assert.IsNotNull(getReply, "getReplyList is null.");
            Assert.IsNotNull(getReply.List, "getReplyList is null.");
            Assert.AreNotEqual(0, getReply.List.Count, " getReply.List.Count is 0.");
            Assert.AreEqual(name, getReply.List[0].Name, "Name did not get inserted or updated correctly");
            return newId;
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblAuthGroup Table. This will be an update as id != 0.
        /// </summary>
        /// <param name="id"> id to do a create or update on. id is 0 if it will be a create.</param>
        /// <param name="name">name to do a create or update on.</param>
        private void VerifyUpdateAuthGroups(int id, string name)
        {
            Assert.AreNotEqual(0, id, "id = 0 should not be passed into this method, as it is an insert instead of an update");

            CreateOrUpdateAuthGroups(id, name);

            GetAuthGroupForValidIDs(id);
            Assert.IsNotNull(getReply, "getReplyList is null.");
            Assert.IsNotNull(getReply.List, "getReplyList is null.");
            Assert.AreNotEqual(0, getReply.List.Count, " getReply.List.Count is 0.");
            Assert.AreEqual(name, getReply.List[0].Name, "Name did not get inserted or updated correctly");
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblAuthGroup Table
        /// </summary>
        /// <param name="id"> id to do a create or update on. id is 0 if it will be a create.</param>
        /// <param name="name">name to do a create or update on.</param>
        private void CreateOrUpdateAuthGroups(int id, string name)
        {
            createOrUpdateRequest = new AuthGroupsCreateOrUpdateRequestDC();

            createOrUpdateReply = null;

            //Populate the request data
            createOrUpdateRequest.Incaller = IN_CALLER;
            createOrUpdateRequest.IncallerVersion = IN_CALLER_VERSION;
            createOrUpdateRequest.InId = id;
            createOrUpdateRequest.InGuid = Guid.NewGuid();
            createOrUpdateRequest.InName = name;
            createOrUpdateRequest.InInsertedByUserAlias = USER;
            createOrUpdateRequest.InUpdatedByUserAlias = USER;

            try
            {
                createOrUpdateReply = proxy.AuthGroupsCreateOrUpdate(createOrUpdateRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblAuthGroups: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblAuthGroups: {0}", ex.Message);
            }

            Assert.IsNotNull(createOrUpdateReply, "AuthGroupsCreateOrUpdateReplyDC object null");
            Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.StatusReply is null");
            Assert.AreEqual(0, createOrUpdateReply.StatusReply.Errorcode, "createOrUpdateReply.StatusReply.Errorcode is not 0. Instead it is {0}.", createOrUpdateReply.StatusReply.Errorcode);
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorMessage, "createOrUpdateReply.StatusReply.ErrorMessage is not null");
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorGuid, "createOrUpdateReply.StatusReply.ErrorGuid is not null");
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblAuthGroup Table and then cleanup. This will be a create since id is null.
        /// </summary>
        private void VerifyCreateOrUpdateAuthGroupsWithNullIDAndCleanup()
        {
            // Create with null id
            int id = CreateOrUpdateAuthGroupsWithNullId(TEST_FIELD_NAME);

            // Update
            VerifyUpdateAuthGroups(id, TEST_FIELD_NAME);

            // Delete if it was created
            DeleteAuthGroupsForValidIDs(id);
            HardDeleteThisRow(id, TABLE_NAME, TEST_DATABASE);

            GetAuthGroupForInValidIDs(id);
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblAuthGroup Table. This will be a create since id is null.
        /// </summary>
        /// <param name="name">name to be created</param>
        /// <returns>returns the id created</returns>
        private int CreateOrUpdateAuthGroupsWithNullId(string name)
        {
            createOrUpdateRequest = new AuthGroupsCreateOrUpdateRequestDC();

            createOrUpdateReply = null;

            //Populate the request data
            createOrUpdateRequest.Incaller = IN_CALLER;
            createOrUpdateRequest.IncallerVersion = IN_CALLER_VERSION;
            createOrUpdateRequest.InGuid = Guid.NewGuid();
            createOrUpdateRequest.InName = name;
            createOrUpdateRequest.InInsertedByUserAlias = USER;
            createOrUpdateRequest.InUpdatedByUserAlias = USER;

            try
            {
                createOrUpdateReply = proxy.AuthGroupsCreateOrUpdate(createOrUpdateRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblAuthGroups: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblAuthGroups: {0}", ex.Message);
            }

            Assert.IsNotNull(createOrUpdateReply, "AuthGroupsCreateOrUpdateReplyDC object null");
            Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.StatusReply is null");
            Assert.AreEqual(0, createOrUpdateReply.StatusReply.Errorcode, "createOrUpdateReply.StatusReply.Errorcode is not 0. Instead it is {0}.", createOrUpdateReply.StatusReply.Errorcode);
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorMessage, "createOrUpdateReply.StatusReply.ErrorMessage is not null");
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorGuid, "createOrUpdateReply.StatusReply.ErrorGuid is not null");

            int id = GetAuthGroupForValidNameForMaxID(name);
            GetAuthGroupForValidIDs(id);
            Assert.IsNotNull(getReply, "getReplyList is null.");
            Assert.IsNotNull(getReply.List, "getReplyList is null.");
            Assert.AreNotEqual(0, getReply.List.Count, " getReply.List.Count is 0.");
            Assert.AreEqual(name, getReply.List[0].Name, "Name did not get inserted or updated correctly");

            return id;
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblAuthGroup Table for Invalid IDs. id is invalid if it's not 0 or not already in the table.
        /// </summary>
        /// <param name="id">id to try to insert or update</param>
        /// <param name="name">name to try to insert or update</param>
        private void CreateOrUpdateAuthGroupsForInvalidId(int id, string name)
        {
            bool isFaultException = false;

            createOrUpdateRequest = new AuthGroupsCreateOrUpdateRequestDC();

            createOrUpdateReply = null;

            //Populate the request data
            createOrUpdateRequest.Incaller = IN_CALLER;
            createOrUpdateRequest.IncallerVersion = IN_CALLER_VERSION;
            createOrUpdateRequest.InId = id;
            createOrUpdateRequest.InGuid = Guid.NewGuid();
            createOrUpdateRequest.InName = name;
            createOrUpdateRequest.InInsertedByUserAlias = USER;
            createOrUpdateRequest.InUpdatedByUserAlias = USER;

            try
            {
                createOrUpdateReply = proxy.AuthGroupsCreateOrUpdate(createOrUpdateRequest);
            }
            // Task 20943. Add fault exception validation.
            //catch (FaultException<www.microsoft.com.practices.EnterpriseLibrary._2007._01.wcf.validation.ValidationFault> exc)
            //{
            //    Assert.IsNotNull(exc.Detail.Details);
            //    Assert.AreNotEqual(0, exc.Detail.Details.Count);
            //    Assert.IsNotNull(exc.Detail.Details[0].Message);
            //    if (createOrUpdateRequest.InId < 0)
            //    {
            //        Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_INID_MSG, exc.Detail.Details[0].Message);
            //        isFaultException = true;
            //    }
            //    else if (createOrUpdateRequest.InId == 0)
            //    {
            //        if (createOrUpdateRequest.InName == null || createOrUpdateRequest.InName == string.Empty)
            //        {
            //            Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_INNAME_MSG, exc.Detail.Details[0].Message);
            //            isFaultException = true;
            //        }
            //        if (createOrUpdateRequest.InInsertedByUserAlias == null || createOrUpdateRequest.InInsertedByUserAlias == string.Empty)
            //        {
            //            Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_ININSERTEDBYUSERALIAS_MSG, exc.Detail.Details[0].Message);
            //            isFaultException = true;
            //        }
            //        if (createOrUpdateRequest.InGuid == null)
            //        {
            //            Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_INGUID_MSG, exc.Detail.Details[0].Message);
            //            isFaultException = true;
            //        }
            //    }
            //    if (!isFaultException)
            //    {
            //        Assert.Fail("Failed to get data from ltblAuthGroups {0}", exc.Message);
            //    }
            //}
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblAuthGroups: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblAuthGroups: {0}", ex.Message);
            }

            if (!isFaultException)
            {
                int errorConstant = GetErrorConstantInvalidIDForUpdate(id);

                Assert.IsNotNull(createOrUpdateReply, "AuthGroupsCreateOrUpdateReplyDC object null");
                Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.StatusReply is null");
                Assert.AreEqual(createOrUpdateReply.StatusReply.Errorcode, errorConstant, "createOrUpdateReply.StatusReply.Errorcode is not {0}. Instead it is {1}.", errorConstant, createOrUpdateReply.StatusReply.Errorcode);
                Assert.IsNotNull(createOrUpdateReply.StatusReply.ErrorMessage, "createOrUpdateReply.StatusReply.ErrorMessage is null");
            }
        }

        /// <summary>
        /// Verifies that different WCF calls for the ltblAuthGroup Table can be run simultaneously
        /// </summary>
        private void VerifyConcurrencyTestingForAuthGroupsByRunningThreads()
        {
            ThreadStart[] authGroupsThreadStarter = new ThreadStart[10];
            Thread[] authGroupsThread = new Thread[10];

            authGroupsThreadStarter[0] = delegate { GetAuthGroupForValidIDs(1); };
            authGroupsThreadStarter[1] = delegate { GetAuthGroupForValidIDs(3); };
            authGroupsThreadStarter[2] = delegate { VerifyDeleteAuthGroupForValidIDsAndCleanup(2); };
            authGroupsThreadStarter[3] = delegate { VerifyCreateOrUpdateAuthGroupsAndCleanup(); };
            authGroupsThreadStarter[4] = delegate { VerifyCreateOrUpdateAuthGroupsWithNullIDAndCleanup(); };
            authGroupsThreadStarter[5] = delegate { GetAuthGroupForValidIDs(4); };
            authGroupsThreadStarter[6] = delegate { VerifyCreateOrUpdateAuthGroupsAndCleanup(); };
            authGroupsThreadStarter[7] = delegate { VerifyDeleteAuthGroupForValidIDsAndCleanup(5); };
            authGroupsThreadStarter[8] = delegate { VerifyCreateOrUpdateAuthGroupsWithNullIDAndCleanup(); };
            authGroupsThreadStarter[9] = delegate { GetAuthGroupForValidIDs(1); };

            for (int i = 0; i < authGroupsThread.Length; i++)
            {
                authGroupsThread[i] = new Thread(authGroupsThreadStarter[i]);
            }

            for (int i = 0; i < authGroupsThread.Length; i++)
            {
                authGroupsThread[i].Start();
                authGroupsThread[i].Join();
            }
        }

        #endregion

        #region Test Methods

        [WorkItem(20910)]
        [Description("Verify GET FROM ltblAuthGroups Table")]
        [Owner(TEST_OWNER)]
        //[TestCategory(Common.TestCategory.BVT)]
        [TestCategory(Common.TestCategory.Smoke)]
        [TestMethod]
        public void VerifyGetAuthGroupForValidIDs()
        {
            for (int id = 1; id <= 5; id++)
            {
                GetAuthGroupForValidIDs(id);
            }
        }

        [WorkItem(21038)]
        [Description("Verify GET FROM ltblAuthGroups Table for valid guid")]
        [Owner(TEST_OWNER)]
        [TestCategory(Common.TestCategory.Full)]
        [TestMethod]
        public void VerifyGetAuthGroupForValidGuid()
        {
            GetAuthGroupForValidGuid(new Guid (VALID_GUID));
        }

        [WorkItem(21037)]
        [Description("Verify GET FROM ltblAuthGroups Table using Invalid ID")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetAuthGroupForInValidIDs()
        {
            int[] nonExistingIDs = new int[] 
           {
               Int32.MinValue,
               -1,
               6,
               100,
               Int32.MaxValue         
           };

            foreach (int nonExistingID in nonExistingIDs)
            {
                GetAuthGroupForInValidIDs(nonExistingID);
            }
        }

        [WorkItem(21010)]
        [Description("Verify DELETE FROM ltblAuthGroup Table for valid IDs")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyDeleteAuthGroupForValidIDs()
        {
            int id = 1;

            VerifyDeleteAuthGroupForValidIDsAndCleanup(id);
        }

        [WorkItem(21011)]
        [Description("Verify DELETE FROM ltblAuthGroup Table for valid Name")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyDeleteAuthGroupForValidName()
        {
            VerifyDeleteAuthGroupForValidNameAndCleanup(VALID_NAME);
        }

        [WorkItem(21009)]
        [Description("Verify DELETE FROM ltblAuthGroup Table for Invalid IDs")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyDeleteAuthGroupForInvalidIDs()
        {
            int[] nonExistingIDs = new int[]
            {
                0, 
                100, 
                Int32.MaxValue,
                Int32.MinValue,
                -1
            };

            foreach (int nonExistingID in nonExistingIDs)
            {
                DeleteAuthGroupsForInvalidIDs(nonExistingID);
            }
        }

        [WorkItem(20987)]
        [Description("Verify CreateOrUpdate FROM ltblAuthGroups Table with ID = 0")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyCreateOrUpdateAuthGroups()
        {
            VerifyCreateOrUpdateAuthGroupsAndCleanup();
        }

        [WorkItem(20989)]
        [Description("Verify CreateOrUpdate FROM ltblAuthGroups Table with Null ID")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyCreateOrUpdateAuthGroupsWithNullID()
        {
            VerifyCreateOrUpdateAuthGroupsWithNullIDAndCleanup();
        }

        [WorkItem(20988)]
        [Description("Verify CreateOrUpdate FROM ltblAuthGroups Table For Invalid Ids")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyCreateOrUpdateAuthGroupsForInvalidId()
        {
            int[] invalidIDs = new int[]
            {
                101, 
                Int32.MaxValue,
                Int32.MinValue,
                -1
            };

            foreach (int id in invalidIDs)
            {
                // Check that it's not already there
                GetAuthGroupForInValidIDs(id);

                // Create
                CreateOrUpdateAuthGroupsForInvalidId(id, TEST_FIELD_NAME);

                // Do a get to make sure it worked
                GetAuthGroupForInValidIDs(id);
            }
        }

        [WorkItem(20967)]
        [Description("Verify ConcurrencyTesting FROM ltblAuthGroups Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyConcurrencyTestingForAuthGroups()
        {
            VerifyConcurrencyTestingForAuthGroupsByRunningThreads();
        }

        #endregion
    }
}
