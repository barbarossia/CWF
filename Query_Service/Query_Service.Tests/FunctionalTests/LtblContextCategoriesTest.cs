//-----------------------------------------------------------------------
// <copyright file="LtblContextCategoriesTest.cs" company="Microsoft">
// Copyright
// A Test Class for ltblContextCategories table.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts;
using System.ServiceModel;
using Query_Service.Tests.Common;

namespace Query_Service.Tests
{
    /// <summary>
    /// A Test Class for ltblContextCategories table
    /// </summary>
    [TestClass]
    public class LtblContextCategoriesTest : QueryServiceTestBase
    {
        #region Constants

        private const string TABLE_NAME = "ltblContextCategories";
        private const string AUTH_GROUP_NAME = "Unassigned";
        private const string VALID_GUID = "737D6643-A14C-4B06-AC54-41AC89E4FD38";
        private const string VALID_NAME = "OAS";

        #endregion

        #region Request Objects

        private ContextCategoriesGetRequestDC getRequest;
        private ContextCategoriesDeleteRequestDC deleteRequest;
        private ContextCategoriesCreateOrUpdateRequestDC createOrUpdateRequest;

        #endregion

        #region Reply Objects

        ContextCategoriesGetReplyDC getReply;
        ContextCategoriesCreateOrUpdateReplyDC createOrUpdateReply;
        ContextCategoriesDeleteReplyDC deleteReply;

        #endregion

        #region Private Methods

        /// <summary>
        /// Verify GET FROM ltblContextCategories Table for Valid IDs
        /// </summary>
        /// <param name="id">id of row to do a get on</param>
        private void GetContextCategoriesForValidIDs(int id)
        {
            getRequest = new ContextCategoriesGetRequestDC();

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            getRequest.InId = id;

            try
            {
                getReply = proxy.ContextCategoriesGet(getRequest);
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
            Assert.AreEqual(1, getReply.List.Count, "Service returned wrong number of records. InId = {0}. It should have returned 1 but instead returned {1}.", id, getReply.List.Count);
            Assert.IsNull(getReply.StatusReply.ErrorMessage, "Error Message is not null. Error Message: {0}", getReply.StatusReply.ErrorMessage);
            Assert.IsNull(getReply.StatusReply.ErrorGuid, "ErrorGuid is not null. ErrorGuid: {0}", getReply.StatusReply.ErrorGuid);
            Assert.AreEqual(getRequest.InId, getReply.List[0].Id, "Service returned the wrong record. Expected Id: {0}, Actual Id: {1}", getRequest.InId, getReply.List[0].Id);
        }

        /// <summary>
        /// Verify GET FROM ltblContextCategories Table for Valid Guid
        /// </summary>
        /// <param name="guid">guid of row to do a get on</param>
        private void GetContextCategoriesForValidGuid(Guid guid)
        {
            getRequest = new ContextCategoriesGetRequestDC();

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            getRequest.InGuid = guid;

            try
            {
                getReply = proxy.ContextCategoriesGet(getRequest);
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
            Assert.AreEqual(1, getReply.List.Count, "Service returned wrong number of records. InGuid = {0}. It should have returned 1 but instead returned {1}.", guid, getReply.List.Count);
            Assert.IsNull(getReply.StatusReply.ErrorMessage, "Error Message is not null. Error Message: {0}", getReply.StatusReply.ErrorMessage);
            Assert.IsNull(getReply.StatusReply.ErrorGuid, "ErrorGuid is not null. ErrorGuid: {0}", getReply.StatusReply.ErrorGuid);
            Assert.AreEqual(getRequest.InGuid, getReply.List[0].Guid, "Service returned the wrong record. Expected Guid: {0}, Actual Guid: {1}", getRequest.InGuid, getReply.List[0].Guid);
        }

        /// <summary>
        /// Verify GET FROM ltblContextCategories Table for a valid name
        /// </summary>
        /// <param name="name">name from row to do a get on</param>
        /// <returns>returns the id of this row</returns>
        private int GetContextCategoriesForValidNamesForMaxID(string name)
        {
            getRequest = new ContextCategoriesGetRequestDC();

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            getRequest.InName = name;

            try
            {
                getReply = proxy.ContextCategoriesGet(getRequest);
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
            Assert.AreEqual(1, getReply.List.Count, "Service returned wrong number of records. InName = {0}. It should have returned 1 but instead returned {1}.", name, getReply.List.Count);
            Assert.IsNull(getReply.StatusReply.ErrorMessage, "Error Message is not null. Error Message: {0}", getReply.StatusReply.ErrorMessage);
            Assert.IsNull(getReply.StatusReply.ErrorGuid, "ErrorGuid is not null. ErrorGuid: {0}", getReply.StatusReply.ErrorGuid);
            int index = getReply.List.Count - 1;
            int id = getReply.List[index].Id;

            return id;
        }

        /// <summary>
        /// Verify GET FROM ltblContextCategories Table for Invalid IDs
        /// </summary>
        /// <param name="nonExistingID">id of row to do a get on</param>
        private void GetContextCategoriesForInValidIDs(int nonExistingID)
        {
            bool isFaultException = false;
            getRequest = new ContextCategoriesGetRequestDC();

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            getRequest.InId = nonExistingID;

            try
            {
                getReply = proxy.ContextCategoriesGet(getRequest);
            }
            // Task 20943. Add fault exception validation.
            //catch (FaultException<www.microsoft.com.practices.EnterpriseLibrary._2007._01.wcf.validation.ValidationFault> exc)
            //{
            //    if (nonExistingID < 0)
            //    {
            //        Assert.IsNotNull(exc.Detail.Details);
            //        Assert.AreEqual(1, exc.Detail.Details.Count);
            //        Assert.IsNotNull(exc.Detail.Details[0].Message);
            //        Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_INID_MSG, exc.Detail.Details[0].Message);
            //        isFaultException = true;
            //    }
            //    else
            //    {
            //        Assert.Fail("Failed to get data from ltblContextCategories: {0}", exc.Message);
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
                Assert.AreEqual(0, getReply.List.Count, "Service returned wrong number of records. InId: {0}. It should have returned 0 but instead returned {1}.", nonExistingID, getReply.List.Count);
                Assert.IsNotNull(getReply.StatusReply.ErrorMessage, "Error Message is null");
                Assert.AreEqual(errorConstant, getReply.StatusReply.Errorcode, "Service returned unexpected error code.");
            }
        }

        /// <summary>
        /// Verify GET FROM ltblContextCategories Table for softDeleted IDs
        /// </summary>
        /// <param name="softDeletedID">id of row to do a get on</param>
        private void GetContextCategoriesForSoftDeletedID(int softDeletedID)
        {
            getRequest = new ContextCategoriesGetRequestDC();

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            getRequest.InId = softDeletedID;

            try
            {
                getReply = proxy.ContextCategoriesGet(getRequest);
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
            Assert.AreEqual(0, getReply.List.Count, "Service returned wrong number of records. InId: {0}. It should have returned 0 but instead returned {1}.", softDeletedID, getReply.List.Count);
            Assert.IsNotNull(getReply.StatusReply.ErrorMessage, "Error Message is null");
            Assert.AreEqual(errorConstant, getReply.StatusReply.Errorcode, "Service returned unexpected error code.");
        }

        /// <summary>
        /// Verify DELETE FROM ltblContextCategories Table for Valid IDs. Will set softdelete to 1. Afterwards will do a cleanup.
        /// </summary>
        /// <param name="id">id of row to do a delete on</param>
        private void VerifyDeleteContextCategoriesForValidIDsAndCleanup(int id)
        {
            DeleteContextCategoriesForValidIDs(id);

            // put the soft delete back in so other tests won't be affected
            UpdateSoftDelete(id.ToString(), TABLE_NAME);

            GetContextCategoriesForValidIDs(id);
        }

        /// <summary>
        /// Verify DELETE FROM ltblContextCategories Table for Valid Names. Will set softdelete to 1. Afterwards will do a cleanup.
        /// </summary>
        /// <param name="name">name of row to do a delete on</param>
        private void VerifyDeleteContextCategoriesForValidNamesAndCleanup(string name)
        {
            int id = DeleteContextCategoriesForValidNames(name);

            // put the soft delete back in so other tests won't be affected
            UpdateSoftDelete(id.ToString(), TABLE_NAME);

            GetContextCategoriesForValidIDs(id);
        }

        /// <summary>
        /// Verify DELETE FROM ltblContextCategories Table for Valid IDs. Will set softdelete to 1.
        /// </summary>
        /// <param name="id">id of row to do a delete on</param>
        private void DeleteContextCategoriesForValidIDs(int id)
        {
            deleteRequest = new ContextCategoriesDeleteRequestDC();
            deleteReply = null;

            deleteRequest.Incaller = IN_CALLER;
            deleteRequest.IncallerVersion = IN_CALLER_VERSION;
            deleteRequest.InId = id;

            try
            {
                deleteReply = proxy.ContextCategoriesDelete(deleteRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to delete data from ltblContextCategories: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to delete data from ltblContextCategories: {0}", ex.Message);
            }

            Assert.IsNotNull(deleteReply, "etblContextDeleteReplyDC object null");
            Assert.IsNotNull(deleteReply.StatusReply, "deleteReply.StatusReply is null");
            Assert.AreEqual(0, deleteReply.StatusReply.Errorcode, "Delete operation not successful.");

            // Now check to see if we don't have that record in the table
            GetContextCategoriesForSoftDeletedID(id);
        }

        /// <summary>
        /// Verify DELETE FROM ltblContextCategories Table for Valid Name. Will set softdelete to 1.
        /// </summary>
        /// <param name="name">name of row to do a delete on</param>
        /// <returns>returns the id of this row</returns>
        private int DeleteContextCategoriesForValidNames(string name)
        {
            int id = GetContextCategoriesForValidNamesForMaxID(name);
            deleteRequest = new ContextCategoriesDeleteRequestDC();
            deleteReply = null;

            deleteRequest.Incaller = IN_CALLER;
            deleteRequest.IncallerVersion = IN_CALLER_VERSION;
            deleteRequest.InName = name;

            try
            {
                deleteReply = proxy.ContextCategoriesDelete(deleteRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to delete data from ltblContextCategories: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to delete data from ltblContextCategories: {0}", ex.Message);
            }

            Assert.IsNotNull(deleteReply, "etblContextDeleteReplyDC object null");
            Assert.IsNotNull(deleteReply.StatusReply, "deleteReply.StatusReply is null");
            Assert.AreEqual(0, deleteReply.StatusReply.Errorcode, "Delete operation not successful.");

            // Now check to see if we don't have that record in the table
            GetContextCategoriesForSoftDeletedID(id);

            return id;
        }

        /// <summary>
        /// Verify DELETE FROM ltblContextCategories Table for Invalid IDs
        /// </summary>
        /// <param name="id">id of row to do a delete on. This id does not exist in the table</param>
        private void DeleteContextCategoriesForInvalidIDs(int id)
        {
            bool isFaultException = false;
            deleteRequest = new ContextCategoriesDeleteRequestDC();
            deleteReply = null;

            deleteRequest.Incaller = IN_CALLER;
            deleteRequest.IncallerVersion = IN_CALLER_VERSION;
            deleteRequest.InId = id;

            try
            {
                deleteReply = proxy.ContextCategoriesDelete(deleteRequest);
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
            //        Assert.Fail("Failed to delete data from ltblContextCategories: {0}", exc.Message);
            //    }
            //}
            catch (FaultException e)
            {
                Assert.Fail("Failed to delete data from ltblContextCategories: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to delete data from ltblContextCategories: {0}", ex.Message);
            }

            if (!isFaultException)
            {
                int errorcode = GetErrorConstantDeleteInvalidID(id);

                // special case if id = 0
                if (id == 0)
                {
                    errorcode = CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_INIDINNAMEINGUIDCANNOTALLBENULL_ID;
                }

                Assert.IsNotNull(deleteReply, "ContextCategoriesGetReplyDC object null");
                Assert.IsNotNull(deleteReply.StatusReply, "deleteReply.StatusReply is null");
                Assert.AreEqual(errorcode, deleteReply.StatusReply.Errorcode, "Delete operation not successful.");
                Assert.IsNotNull(deleteReply.StatusReply.ErrorMessage, "Error Message is null");
            }
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblContextCategories table and then does a cleanup. This will be an insert as id is set to 0.
        /// </summary>
        private void VerifyCreateOrUpdateContextCategoriesAndCleanup()
        {
            string testFieldName = TEST_FIELD_NAME + Guid.NewGuid();

            // Create
            int id = CreateContextCategoriesWithIdIsZero(testFieldName, TEST_STRING);

            // Update
            UpdateContextCategories(id, testFieldName, TEST_STRING_2);

            // Delete if it was created
            DeleteContextCategoriesForValidIDs(id);
            HardDeleteThisRow(id, TABLE_NAME, TEST_DATABASE);

            GetContextCategoriesForInValidIDs(id);
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblContextCategories table. This will be an insert as id is set to 0.
        /// </summary>
        /// <param name="name">name to do a create or update on.</param>
        /// <param name="description">description to do a create or update on.</param>
        /// <returns>returns the id of this row</returns>
        private int CreateContextCategoriesWithIdIsZero(string name, string description)
        {
            int id = 0;

            CreateOrUpdateContextCategories(id, name, description);

            int newId = GetContextCategoriesForValidNamesForMaxID(name);
            GetContextCategoriesForValidIDs(newId);
            Assert.IsNotNull(getReply, "getReplyList is null.");
            Assert.IsNotNull(getReply.List, "getReplyList is null.");
            Assert.AreNotEqual(0, getReply.List.Count, " getReply.List.Count is 0.");
            Assert.AreEqual(description, getReply.List[0].Description, "Description did not get inserted or updated correctly");
            Assert.AreEqual(name, getReply.List[0].Name, "Name did not get inserted or updated correctly");

            return newId;
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblContextCategories Table. This will be an update as id != 0.
        /// </summary>
        /// <param name="id"> id to do a create or update on. id is 0 if it will be a create.</param>
        /// <param name="name">name to do a create or update on.</param>
        /// <param name="description">description to do a create or update on.</param>
        private void UpdateContextCategories(int id, string name, string description)
        {
            Assert.AreNotEqual(0, id, "id = 0 should not be passed into this method, as it is an insert instead of an update");

            CreateOrUpdateContextCategories(id, name, description);

            GetContextCategoriesForValidIDs(id);
            Assert.IsNotNull(getReply, "getReplyList is null.");
            Assert.IsNotNull(getReply.List, "getReplyList is null.");
            Assert.AreNotEqual(0, getReply.List.Count, " getReply.List.Count is 0.");
            Assert.AreEqual(description, getReply.List[0].Description, "Description did not get inserted or updated correctly");
            Assert.AreEqual(name, getReply.List[0].Name, "Name did not get inserted or updated correctly");
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblContextCategories Table and then cleanup. This will be a create since id is null.
        /// </summary>
        private void VerifyCreateOrUpdateContextCategoriesWithNullIDAndCleanup()
        {
            /// Create with null id
            int id = CreateOrUpdateContextCategoriesForNullId(TEST_FIELD_NAME, TEST_STRING);

            // Update
            UpdateContextCategories(id, TEST_FIELD_NAME, TEST_STRING_2);

            // Delete if it was created
            DeleteContextCategoriesForValidIDs(id);
            HardDeleteThisRow(id, TABLE_NAME, TEST_DATABASE);

            GetContextCategoriesForInValidIDs(id);
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblContextCategories Table. This will be a create since id is null.
        /// </summary>
        /// <param name="name">name to be created</param>
        /// <param name="description">description to be created</param>
        /// <returns>returns the id created</returns>
        private int CreateOrUpdateContextCategoriesForNullId(string name, string description)
        {
            createOrUpdateRequest = new ContextCategoriesCreateOrUpdateRequestDC();

            createOrUpdateReply = null;

            //Populate the request data
            createOrUpdateRequest.Incaller = IN_CALLER;
            createOrUpdateRequest.IncallerVersion = IN_CALLER_VERSION;
            createOrUpdateRequest.InGuid = Guid.NewGuid();
            createOrUpdateRequest.InName = name;
            createOrUpdateRequest.InDescription = description;
            createOrUpdateRequest.InInsertedByUserAlias = USER;
            createOrUpdateRequest.InUpdatedByUserAlias = USER;
            createOrUpdateRequest.InAuthGroupName = AUTH_GROUP_NAME;

            try
            {
                createOrUpdateReply = proxy.ContextCategoriesCreateOrUpdate(createOrUpdateRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblContextCategories: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblContextCategories: {0}", ex.Message);
            }

            Assert.IsNotNull(createOrUpdateReply, "ApplicationsCreateOrUpdateReplyDC object null");
            Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.StatusReply is null");
            Assert.AreEqual(0, createOrUpdateReply.StatusReply.Errorcode, "createOrUpdateReply.StatusReply.Errorcode is not 0. Instead it is {0}.", createOrUpdateReply.StatusReply.Errorcode);
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorMessage, "createOrUpdateReply.StatusReply.ErrorMessage is not null");
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorGuid, "createOrUpdateReply.StatusReply.ErrorGuid is not null");

            int id = GetContextCategoriesForValidNamesForMaxID(name);
            GetContextCategoriesForValidIDs(id);
            Assert.IsNotNull(getReply, "getReplyList is null.");
            Assert.IsNotNull(getReply.List, "getReplyList is null.");
            Assert.AreNotEqual(0, getReply.List.Count, " getReply.List.Count is 0.");
            Assert.AreEqual(description, getReply.List[0].Description, "Description did not get inserted or updated corrected");
            Assert.AreEqual(name, getReply.List[0].Name, "Description did not get inserted or updated corrected");

            return id;
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblContextCategories Table
        /// </summary>
        /// <param name="id"> id to do a create or update on. id is 0 if it will be a create.</param>
        /// <param name="name">name to do a create or update on.</param>
        /// <param name="name">description to do a create or update on.</param>
        private void CreateOrUpdateContextCategories(int id, string name, string description)
        {
            createOrUpdateRequest = new ContextCategoriesCreateOrUpdateRequestDC();

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
            createOrUpdateRequest.InAuthGroupName = AUTH_GROUP_NAME;

            try
            {
                createOrUpdateReply = proxy.ContextCategoriesCreateOrUpdate(createOrUpdateRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblContextCategories: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblContextCategories: {0}", ex.Message);
            }

            Assert.IsNotNull(createOrUpdateReply, "ApplicationsCreateOrUpdateReplyDC object null");
            Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.StatusReply is null");
            Assert.AreEqual(0, createOrUpdateReply.StatusReply.Errorcode, "createOrUpdateReply.StatusReply.Errorcode is not 0. Instead it is {0}.", createOrUpdateReply.StatusReply.Errorcode);
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorMessage, "createOrUpdateReply.StatusReply.ErrorMessage is not null");
            Assert.IsNull(createOrUpdateReply.StatusReply.ErrorGuid, "createOrUpdateReply.StatusReply.ErrorGuid is not null");
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM ltblContextCategories Table for Invalid IDs. id is invalid if it's not 0 or not already in the table.
        /// </summary>
        /// <param name="id">id to try to insert or update</param>
        /// <param name="name">name to try to insert or update</param>
        private void CreateOrUpdateContextCategoriesForInvalidID(int id, string name)
        {
            bool isFaultException = false;

            createOrUpdateRequest = new ContextCategoriesCreateOrUpdateRequestDC();

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
            createOrUpdateRequest.InAuthGroupName = AUTH_GROUP_NAME;

            try
            {
                createOrUpdateReply = proxy.ContextCategoriesCreateOrUpdate(createOrUpdateRequest);
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
            //        if (createOrUpdateRequest.InDescription == null || createOrUpdateRequest.InDescription == string.Empty)
            //        {
            //            Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_INDESCRIPTION_MSG, exc.Detail.Details[0].Message);
            //            isFaultException = true;
            //        }
            //        if (createOrUpdateRequest.InAuthGroupName == null || createOrUpdateRequest.InAuthGroupName == string.Empty)
            //        {
            //            Assert.AreEqual(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_InAuthGroupName_MSG, exc.Detail.Details[0].Message);
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
            //        Assert.Fail("Failed to get data from ltblContextCategories {0}", exc.Message);
            //    }
            //}
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblContextCategories: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from ltblContextCategories: {0}", ex.Message);
            }

            if (!isFaultException)
            {
                int errorConstant = GetErrorConstantInvalidIDForUpdate(id);

                Assert.IsNotNull(createOrUpdateReply, "ContextCategoriesCreateOrUpdateRequestDC object null");
                Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.StatusReply is null");
                Assert.AreEqual(createOrUpdateReply.StatusReply.Errorcode, errorConstant, "createOrUpdateReply.StatusReply.Errorcode is not {0}. Instead it is {1}.", errorConstant, createOrUpdateReply.StatusReply.Errorcode);
                Assert.IsNotNull(createOrUpdateReply.StatusReply.ErrorMessage, "createOrUpdateReply.StatusReply.ErrorMessage is null");
            }
        }

        /// <summary>
        /// Verifies that different WCF calls for the ltblContextCategories Table can be run simultaneously
        /// </summary>
        private void VerifyConcurrencyTestingForContextCategoriesByRunningThreads()
        {
            ThreadStart[] contextCategoriesThreadStarter = new ThreadStart[10];
            Thread[] contextCategoriesThread = new Thread[10];

            contextCategoriesThreadStarter[0] = delegate { GetContextCategoriesForValidIDs(1); };
            contextCategoriesThreadStarter[1] = delegate { GetContextCategoriesForValidIDs(3); };
            contextCategoriesThreadStarter[2] = delegate { VerifyDeleteContextCategoriesForValidIDsAndCleanup(2); };
            contextCategoriesThreadStarter[3] = delegate { VerifyCreateOrUpdateContextCategoriesAndCleanup(); };
            contextCategoriesThreadStarter[4] = delegate { VerifyCreateOrUpdateContextCategoriesWithNullIDAndCleanup(); };
            contextCategoriesThreadStarter[5] = delegate { GetContextCategoriesForValidIDs(4); };
            contextCategoriesThreadStarter[6] = delegate { VerifyCreateOrUpdateContextCategoriesAndCleanup(); };
            contextCategoriesThreadStarter[7] = delegate { VerifyDeleteContextCategoriesForValidIDsAndCleanup(5); };
            contextCategoriesThreadStarter[8] = delegate { VerifyCreateOrUpdateContextCategoriesWithNullIDAndCleanup(); };
            contextCategoriesThreadStarter[9] = delegate { GetContextCategoriesForValidIDs(1); };

            for (int i = 0; i < contextCategoriesThread.Length; i++)
            {
                contextCategoriesThread[i] = new Thread(contextCategoriesThreadStarter[i]);
            }

            for (int i = 0; i < contextCategoriesThread.Length; i++)
            {
                contextCategoriesThread[i].Start();
                contextCategoriesThread[i].Join();
            }
        }

        #endregion

        #region Test Methods

        [WorkItem(20911)]
        [Description("Verify GET FROM ltblContextCategories Table")]
        [Owner(TEST_OWNER)]
        //[TestCategory(TestCategory.BVT)]
        [TestCategory(Common.TestCategory.Smoke)]
        [TestMethod]
        public void VerifyGetContextCategoriesForValidIDs()
        {
            for (int id = 1; id <= 5; id++)
            {
                GetContextCategoriesForValidIDs(id);
            }
        }

        [WorkItem(21040)]
        [Description("Verify GET FROM ltblContextCategories Table for valid Guid")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetContextCategoriesForValidGuid()
        {
            GetContextCategoriesForValidGuid(new Guid(VALID_GUID));
        }

        [WorkItem(21039)]
        [Description("Verify GET FROM ltblContextCategories Table using Invalid ID")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetContextCategoriesForInValidIDs()
        {
            int[] nonExistingIDs = new int[]
            {
               -1, 
                Int32.MinValue,
                Int32.MaxValue,
                6, 
                100

            };

            foreach (int nonExistingID in nonExistingIDs)
            {
                GetContextCategoriesForInValidIDs(nonExistingID);
            }
        }

        [WorkItem(21014)]
        [Description("Verify DELETE FROM ltblContextCategories Table for valid IDs")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyDeleteContextCategoriesForValidIDs()
        {
            int[] validIDs = { 1, 2, 3, 4, 5 };

            foreach (int id in validIDs)
            {
                VerifyDeleteContextCategoriesForValidIDsAndCleanup(id);
            }
        }

        [WorkItem(21015)]
        [Description("Verify DELETE FROM ltblContextCategories Table for valid Names")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyDeleteContextCategoriesForValidNames()
        {
            VerifyDeleteContextCategoriesForValidNamesAndCleanup(VALID_NAME);
        }

        [WorkItem(21012)]
        [Description("Verify DELETE FROM ltblContextCategories Table for invalid IDs")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyDeleteContextCategoriesForInvalidIDs()
        {
            int[] nonExistingIDs = new int[]
            {
            0,
            12, 
            100, 
            Int32.MaxValue, 
            Int32.MinValue,
            -1
            };

            foreach (int nonExistingID in nonExistingIDs)
            {
                DeleteContextCategoriesForInvalidIDs(nonExistingID);
            }
        }

        [WorkItem(20990)]
        [Description("Verify CreateOrUpdate FROM ltblContextCategories Table with ID = 0")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyCreateOrUpdateContextCategories()
        {
            VerifyCreateOrUpdateContextCategoriesAndCleanup();
        }

        [WorkItem(20993)]
        [Description("Verify CreateOrUpdate FROM ltblContextCategories Table with Null ID")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyCreateOrUpdateContextCategoriesWithNullID()
        {
            VerifyCreateOrUpdateContextCategoriesWithNullIDAndCleanup();
        }

        [WorkItem(20992)]
        [Description("Verify CreateOrUpdate FROM ltblContextCategories Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyCreateOrUpdateContextCategoriesForInvalidID()
        {
            int[] invalidIDs = new int[]
            {
                15,
                Int32.MaxValue,
                Int32.MinValue,
                -1
            };

            foreach (int invalidID in invalidIDs)
            {
                // Check that it's not already there
                GetContextCategoriesForInValidIDs(invalidID);

                // Create
                CreateOrUpdateContextCategoriesForInvalidID(invalidID, TEST_FIELD_NAME);

                // Do a get to make sure it worked
                GetContextCategoriesForInValidIDs(invalidID);
            }
        }

        [WorkItem(20968)]
        [Description("Verify ConcurrencyTesting FROM ltblContextCategories Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyConcurrencyTestingForContextCategories()
        {
            VerifyConcurrencyTestingForContextCategoriesByRunningThreads();
        }

        #endregion
    }
}
