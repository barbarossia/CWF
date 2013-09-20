//-----------------------------------------------------------------------
// <copyright file="EtblActivityLibrariesTest.cs" company="Microsoft">
// Copyright
// A Test Class for Service Operations on etblActivityLibraries table.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;
using Query_Service.Tests.Common;
using CWF.BAL.Versioning;
using Microsoft.Support.Workflow.Service.DataAccessServices;

namespace Query_Service.Tests
{
    /// <summary>
    /// A Test Class for Service Operations on etblActivityLibraries table.
    /// </summary>
    [TestClass]
    public class EtblActivityLibrariesTest : QueryServiceTestBase
    {
        private const string TABLE_NAME = "etblActivityLibraries";
        private const string VERSION = "2.2.108.0";
        private const string VERSION2 = "1.0.0.1";
        private const string AUTH_GROUP_NAME = "pqocwfauthors";
        private const string CATEGORY_NAME = "Administration";
        private const string ACTIVITY_LIBRARY_NAME = "OASP.Core";
        private const string ACTIVITY_LIBRARY_NAME2 = "PublishingInfo";
        private const string AUTH_GROUP_NAME2 = "pqocwfauthors";
        private const string CATEGORY_NAME2 = "OAS Basic Controls";
        private const string VERSIONNUMBER1 = "1.0.7.8";
        private const string VERSIONNUMBER2 = "1.1.6.0";
        private const string AUTH_GROUP_NAME3 = "pqocwfadmin";
        private const string WORKFLOWTYPENAME = "OAS Page";
        private const string XAML = "<XamlBeginTag></XamlBeginTag>";
        private const int DELETE_ID = 2;
        private const string VALID_GUID = "CB46BC99-84DB-4E27-AAAC-B62FE801B392";
        private const string STOREACTIVITYLIBRARYDEPENDENCIESGROUPSREQUESTDCNAME = "TEST#200";
        private const string STOREACTIVITYNAME = "TEST#312";
        private const string STATUSCODENAME = "Draft";

        ActivityLibraryDC getRequest;
        ActivityLibraryDC createOrUpdateRequest;
        GetActivitiesByActivityLibraryNameAndVersionRequestDC getActivitiesByActivityLibraryNameAndVersionRequestDC;
        GetAllActivityLibrariesRequestDC getAllActivityLibrariesRequestDC;
        StoreLibraryAndActivitiesRequestDC storeLibraryAndActivitiesRequest;

        List<ActivityLibraryDC> getReplyList;
        ActivityLibraryDC createOrUpdateReply;
        GetActivitiesByActivityLibraryNameAndVersionReplyDC getActivitiesByActivityLibraryNameAndVersionReplyDC;
        GetAllActivityLibrariesReplyDC getAllActivityLibrariesReplyDC;
       // StatusReplyDC reply;
        List<StoreActivitiesDC> replyList;

        /// <summary>
        /// Verify GET FROM etblActivityLibraries Table for Valid IDs
        /// </summary>
        /// <param name="id">id of row to do a get on</param>
        private void GetActivityLibrariesForValidID(int id)
        {
            getRequest = new ActivityLibraryDC();
            getReplyList = new List<ActivityLibraryDC>();

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.InInsertedByUserAlias = USER;
            getRequest.InUpdatedByUserAlias = USER;
            getRequest.Id = id;

            try
            {
                getReplyList = new List<ActivityLibraryDC>(devBranchProxy.ActivityLibraryGet(getRequest));
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from etblActivityLibraries: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from etblActivityLibraries: {0}", ex.Message);
            }

            Assert.IsNotNull(getReplyList, "getReply.List is null");
            Assert.AreEqual(1, getReplyList.Count, "Get returned the wrong number of entries. InId: {0}. It should have returned 1 but instead returned {1}.", id, getReplyList.Count);
            Assert.IsNotNull(getReplyList[0].StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(0, getReplyList[0].StatusReply.Errorcode, "Errorcode is not 0. Instead it is {0}", getReplyList[0].StatusReply.Errorcode);
            Assert.AreEqual(getRequest.Id, getReplyList[0].Id, "Get returned wrong data");
        }

        /// <summary>
        /// Verify GET ALL FROM etblActivityLibraries Table
        /// </summary>
        private void GetAllActivityLibraries()
        {
            getAllActivityLibrariesRequestDC = new GetAllActivityLibrariesRequestDC();
            getAllActivityLibrariesReplyDC = null;

            //Populate the request data
            getAllActivityLibrariesRequestDC.Incaller = IN_CALLER;
            getAllActivityLibrariesRequestDC.IncallerVersion = IN_CALLER_VERSION;
            getAllActivityLibrariesRequestDC.InInsertedByUserAlias = USER;
            getAllActivityLibrariesRequestDC.InUpdatedByUserAlias = USER;

            getAllActivityLibrariesReplyDC = devBranchProxy.GetAllActivityLibraries(getAllActivityLibrariesRequestDC);

            Assert.IsNotNull(getAllActivityLibrariesReplyDC, "getAllActivityLibrariesReplyDC is null");
            Assert.IsNotNull(getAllActivityLibrariesReplyDC.List, "getAllActivityLibrariesReplyDC.List is null");
            Assert.AreNotEqual(0, getAllActivityLibrariesReplyDC.List.Count, "getAllActivityLibrariesReplyDC.List is empty");
            Assert.IsTrue(getAllActivityLibrariesReplyDC.List.Count > 1);  // Expecting many items to return.  Better to perform precise initialization and assert the expected count dynamically.
        }

        /// <summary>
        /// Verify GET ALL FROM etblActivityLibraries Table for invalid request object
        /// </summary>
        private void VerifyGetAllActivityLibrariesForInvalidRequestObject()
        {
            getAllActivityLibrariesRequestDC = new GetAllActivityLibrariesRequestDC();
            getAllActivityLibrariesReplyDC = null;

            getAllActivityLibrariesReplyDC = devBranchProxy.GetAllActivityLibraries(getAllActivityLibrariesRequestDC);
        }

        /// <summary>
        /// Verify GetActivitiesByActivityLibraryNameAndVersion FROM etblActivityLibraries Table
        /// </summary>
        /// <param name="name">name to do a get on</param>
        /// <param name="version">version to do a get on</param>
        private void VerifyGetActivitiesByActivityLibraryNameAndVersion(string name, string version)
        {
            getActivitiesByActivityLibraryNameAndVersionRequestDC = new GetActivitiesByActivityLibraryNameAndVersionRequestDC();
            getActivitiesByActivityLibraryNameAndVersionReplyDC = null;

            //Populate the request data
            getActivitiesByActivityLibraryNameAndVersionRequestDC.Incaller = IN_CALLER;
            getActivitiesByActivityLibraryNameAndVersionRequestDC.IncallerVersion = IN_CALLER_VERSION;
            getActivitiesByActivityLibraryNameAndVersionRequestDC.InInsertedByUserAlias = USER;
            getActivitiesByActivityLibraryNameAndVersionRequestDC.InUpdatedByUserAlias = USER;
            getActivitiesByActivityLibraryNameAndVersionRequestDC.Name = name;
            getActivitiesByActivityLibraryNameAndVersionRequestDC.VersionNumber = version;
            
            try
            {
                getActivitiesByActivityLibraryNameAndVersionReplyDC = devBranchProxy.GetActivitiesByActivityLibraryNameAndVersion(getActivitiesByActivityLibraryNameAndVersionRequestDC);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from etblActivityLibraries: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from etblActivityLibraries: {0}", ex.Message);
            }

            Assert.IsNotNull(getActivitiesByActivityLibraryNameAndVersionReplyDC, "getActivitiesByActivityLibraryNameAndVersionReplyDC is null");
            Assert.IsNotNull(getActivitiesByActivityLibraryNameAndVersionReplyDC.StatusReply, "getActivityLibrariesByNameReply.StatusReply is null");
            Assert.AreEqual(0, getActivitiesByActivityLibraryNameAndVersionReplyDC.StatusReply.Errorcode, "StatusReply returned the wrong error code. Expected: 0. Actual: {0}", getActivitiesByActivityLibraryNameAndVersionReplyDC.StatusReply.Errorcode);
            Assert.IsNotNull(getActivitiesByActivityLibraryNameAndVersionReplyDC.List, "getActivitiesByActivityLibraryNameAndVersionReplyDC.List is null");
            Assert.AreNotEqual(0, getActivitiesByActivityLibraryNameAndVersionReplyDC.List.Count, "getActivitiesByActivityLibraryNameAndVersionReplyDC.List is empty");
            Assert.AreEqual(name, getActivitiesByActivityLibraryNameAndVersionReplyDC.List[0].ActivityLibraryName, "Reply returned the wrong ActivityLibraryName");
        }

        /// <summary>
        /// Verify GET FROM etblActivityLibraries Table for valid name and version
        /// </summary>
        /// <param name="name">name of row to do a get on</param>
        /// <param name="version">version of name to do a get on</param>
        private void VerifyGetActivityLibrariesForValidNameAndVersion(string name, string version)
        {
            getRequest = new ActivityLibraryDC();
            getReplyList = new List<ActivityLibraryDC>();

            VerifyGetForValidNameAndVersion(name, version, getRequest, getReplyList);
        }

        /// <summary>
        /// Verify GET FROM etblActivityLibraries Table for valid name and invalid version
        /// </summary>
        /// <param name="name">name of row to do a get on</param>
        /// <param name="version">version of name to do a get on</param>
        private void VerifyGetActivityLibrariesForValidNameAndInvalidVersion(string name, string version)
        {
            getRequest = new ActivityLibraryDC();
            getReplyList = null;

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.Name = name;
            getRequest.VersionNumber = version;

            getReplyList = new List<ActivityLibraryDC>(devBranchProxy.ActivityLibraryGet(getRequest));
            Assert.IsNotNull(getReplyList, "getReply.List is null");
            Assert.AreEqual(0, getReplyList.Count, "Get returned the wrong number of entries. name: {0}, version: {1}. It should have returned 0 but instead returned {2}.", name, version, getReplyList.Count);
        }

        /// <summary>
        /// Verify GET FROM etblActivityLibraries Table for Valid guid
        /// </summary>
        /// <param name="guid">guid of row to do a get on</param>
        private void VerifyGetActivityLibrariesForValidGuid(Guid guid)
        {
            getRequest = new ActivityLibraryDC();
            getReplyList = null;

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.InInsertedByUserAlias = USER;
            getRequest.InUpdatedByUserAlias = USER;
            getRequest.Guid = guid;

            try
            {
                getReplyList = new List<ActivityLibraryDC>(devBranchProxy.ActivityLibraryGet(getRequest));
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from etblActivityLibraries: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from etblActivityLibraries: {0}", ex.Message);
            }

            Assert.IsNotNull(getReplyList, "getReply.List is null");
            Assert.AreEqual(1, getReplyList.Count, "Get returned the wrong number of entries. guid: {0}. It should have returned 1 but instead returned {1}.", guid, getReplyList.Count);
            Assert.IsNotNull(getReplyList[0].StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(0, getReplyList[0].StatusReply.Errorcode, "StatusReply returned the wrong error code. Expected: 0. Actual: {0}", getReplyList[0].StatusReply.Errorcode);
            Assert.AreEqual(getRequest.Guid, getReplyList[0].Guid, "Get returned wrong data");
        }

        /// <summary>
        /// Verify GET FROM etblActivityLibraries Table for a valid name
        /// </summary>
        /// <param name="name">name from row to do a get on</param>
        /// <returns>returns the id of this row</returns>
        private int GetActivityLibrariesForValidNameAndValidateForMaxID(string name)
        {
            getRequest = new ActivityLibraryDC();
            getReplyList = new List<ActivityLibraryDC>();

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.Name = name;

            try
            {
                getReplyList = new List<ActivityLibraryDC>(devBranchProxy.ActivityLibraryGet(getRequest));
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from etblActivityLibraries: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from etblActivityLibraries: {0}", ex.Message);
            }

            Assert.IsNotNull(getReplyList, "getReplyList is null");
            Assert.AreNotEqual(0, getReplyList.Count, "Get returned the wrong number of entries. InName: {0}. It should not have returned 0.", name);
            Assert.IsNotNull(getReplyList[0].StatusReply, "getReplyList[0].StatusReply is null");

            int index = getReplyList.Count - 1;
            int id = getReplyList[index].Id;

            return id;
        }

        /// <summary>
        /// Verify GET FROM etblActivityLibraries Table for Invalid IDs
        /// </summary>
        /// <param name="nonExistingID">id of row to do a get on</param>
        private void VerifyGetActivityLibrariesForInvalidIDs(int nonExistingID)
        {
            getRequest = new ActivityLibraryDC();

            // Populate Request 
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.InInsertedByUserAlias = USER;
            getRequest.InUpdatedByUserAlias = USER;
            getRequest.Id = nonExistingID;

            getReplyList = new List<ActivityLibraryDC>();

            try
            {
                getReplyList = new List<ActivityLibraryDC>(devBranchProxy.ActivityLibraryGet(getRequest));
            }
            catch (FaultException ex)
            {
                Assert.Fail("Caught WCF FaultExceptionException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
            }
            catch (Exception e)
            {
                Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
            }

            int errorConstant = 0; // No error expected

            Assert.IsNotNull(getReplyList, "getReplyList is null.");
            Assert.AreEqual(1, getReplyList.Count, "Service returned wrong number of records. InId= {0}. It should have returned 1 but instead returned {1}.", nonExistingID, getReplyList.Count);
            Assert.IsNotNull(getReplyList[0].StatusReply, "getReply.StatusReply is null");
            Assert.IsNull(getReplyList[0].StatusReply.ErrorMessage, "Error Message is null");
            Assert.AreEqual(errorConstant, getReplyList[0].StatusReply.Errorcode, "Service returned unexpected error code. Expected: {0}, Returned: {1}", errorConstant, getReplyList[0].StatusReply.Errorcode);
        }

        /// <summary>
        /// Verify GET FROM etblActivityLibraries Table for softDeleted IDs
        /// </summary>
        /// <param name="softDeletedID">id of row to do a get on</param>
        private void GetActivityLibrariesForSoftDeletedIDs(int softDeletedID)
        {
            getRequest = new ActivityLibraryDC();
            getReplyList = new List<ActivityLibraryDC>();

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.Id = softDeletedID;

            try
            {
                getReplyList = new List<ActivityLibraryDC>(devBranchProxy.ActivityLibraryGet(getRequest));
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from etblActivityLibraries: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from etblActivityLibraries: {0}", ex.Message);
            }

            int errorConstant = GetErrorConstantSoftDeletedID();

            Assert.IsNotNull(getReplyList, "getReplyList is null");
            Assert.AreEqual(1, getReplyList.Count, "Get returned the wrong number of entries. InId: {0}. It should have returned 1 but instead returned {1}.", softDeletedID, getReplyList.Count);
            Assert.IsNotNull(getReplyList[0].StatusReply, "getReply.Status is null");
            Assert.AreEqual(errorConstant, getReplyList[0].StatusReply.Errorcode, "Returned the wrong status error code. InId: {0}.", softDeletedID);
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM etblActivityLibraries Table. This will be an insert as id is set to 0.
        /// </summary>
        /// <param name="name">name to do a create or update on.</param>
        /// <param name="description">description to do a create or update on.</param>
        /// <param name="version">version to do a create or update on.</param>
        /// <param name="authGroupName">authGroupName to do a create or update on.</param>
        /// <param name="categoryName">categoryName to do a create or update on.</param>
        /// <returns>returns the id of this row</returns>
        private int CreateActivityLibrariesWithIdIs0(string name, string description, string version, string authGroupName, string categoryName)
        {
            int id = 0;

            CreateOrUpdateActivityLibraries(id, name, description, version, authGroupName, categoryName);

            int newId = GetActivityLibrariesForValidNameAndValidateForMaxID(name);
            GetActivityLibrariesForValidID(newId);

            Assert.IsNotNull(getReplyList, "getReplyList is null.");
            Assert.AreNotEqual(0, getReplyList.Count, "getReply.List.Count is 0.");
            Assert.AreEqual(description, getReplyList[0].Description, "Description did not get inserted or updated correctly");
            Assert.AreEqual(name, getReplyList[0].Name, "Name did not get inserted or updated correctly");

            return newId;
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM etblActivityLibraries Table. This will be an update as id != 0.
        /// </summary>
        /// <param name="id"> id to do a create or update on. id is 0 if it will be a create.</param>
        /// <param name="name">name to do a create or update on.</param>
        /// <param name="description">description to do a create or update on.</param>
        /// <param name="version">version to do a create or update on.</param>
        /// <param name="authGroupName">authGroupName to do a create or update on.</param>
        /// <param name="categoryName">categoryName to do a create or update on.</param>
        private void UpdateActivityLibraries(int id, string name, string description, string version, string authGroupName, string categoryName)
        {
            Assert.AreNotEqual(0, id, "id = 0 should not be passed into this method, as it is an insert instead of an update");

            CreateOrUpdateActivityLibraries(id, name, description, version, authGroupName, categoryName);

            GetActivityLibrariesForValidID(id);
            Assert.IsNotNull(getReplyList, "getReplyList is null.");
            Assert.AreNotEqual(0, getReplyList.Count, " getReply.List.Count is 0.");
            Assert.AreEqual(description, getReplyList[0].Description, "Description did not get inserted or updated correctly");
            Assert.AreEqual(name, getReplyList[0].Name, "Name did not get inserted or updated correctly");
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM etblActivityLibraries Table
        /// </summary>
        /// <param name="id"> id to do a create or update on. id is 0 if it will be a create.</param>
        /// <param name="name">name to do a create or update on.</param>
        /// <param name="description">description to do a create or update on.</param>
        /// <param name="version">version to do a create or update on.</param>
        /// <param name="authGroupName">authGroupName to do a create or update on.</param>
        /// <param name="categoryName">categoryName to do a create or update on.</param>
        private void CreateOrUpdateActivityLibraries(int id, string name, string description, string version, string authGroupName, string categoryName)
        {
            createOrUpdateRequest = new ActivityLibraryDC();

            createOrUpdateReply = null;

            //Populate the request data
            createOrUpdateRequest.Incaller = IN_CALLER;
            createOrUpdateRequest.IncallerVersion = IN_CALLER_VERSION;
            createOrUpdateRequest.Id = id;
            createOrUpdateRequest.Guid = Guid.NewGuid();
            createOrUpdateRequest.Name = name;
            createOrUpdateRequest.Description = description;
            createOrUpdateRequest.InInsertedByUserAlias = USER;
            createOrUpdateRequest.InUpdatedByUserAlias = USER;
            createOrUpdateRequest.VersionNumber = version;
            createOrUpdateRequest.AuthGroupName = authGroupName;
            createOrUpdateRequest.CategoryName = categoryName;
            createOrUpdateRequest.StatusName = STATUS;
            createOrUpdateRequest.Status = STATUSCODE;
            
            try
            {
                createOrUpdateReply = devBranchProxy.ActivityLibraryCreateOrUpdate(createOrUpdateRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from etblActivityLibraries: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from etblActivityLibraries: {0}", ex.Message);
            }

            Assert.IsNotNull(createOrUpdateReply, "ActivityLibraryDC object null");
            Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.StatusReply is null");
            Assert.AreEqual(0, createOrUpdateReply.StatusReply.Errorcode, "createOrUpdateReply.StatusReply.Errorcode is not 0. Instead it is {0}.", createOrUpdateReply.StatusReply.Errorcode);
            Assert.IsTrue(string.IsNullOrEmpty(createOrUpdateReply.StatusReply.ErrorMessage), "createOrUpdateReply.StatusReply.ErrorMessage is not null");
            Assert.IsTrue(string.IsNullOrEmpty(createOrUpdateReply.StatusReply.ErrorGuid), "createOrUpdateReply.StatusReply.ErrorGuid is not null");
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM etblActivityLibraries Table. This will be a create since id is null.
        /// </summary>
        /// <param name="name">name to be created</param>
        /// <param name="description">description to do a create or update on.</param>
        /// <param name="version">version to do a create or update on.</param>
        /// <param name="authGroupName">authGroupName to do a create or update on.</param>
        /// <param name="categoryName">categoryName to do a create or update on.</param>
        /// <returns>returns the id created</returns>
        private int CreateActivityLibrariesWithNullID(string name, string description, string version, string authGroupName, string categoryName)
        {
            createOrUpdateRequest = new ActivityLibraryDC();

            createOrUpdateReply = null;

            //Populate the request data
            createOrUpdateRequest.Incaller = IN_CALLER;
            createOrUpdateRequest.IncallerVersion = IN_CALLER_VERSION;
            createOrUpdateRequest.Guid = Guid.NewGuid();
            createOrUpdateRequest.Name = name;
            createOrUpdateRequest.Description = description;
            createOrUpdateRequest.InInsertedByUserAlias = USER;
            createOrUpdateRequest.InUpdatedByUserAlias = USER;
            createOrUpdateRequest.VersionNumber = version;
            createOrUpdateRequest.AuthGroupName = authGroupName;
            createOrUpdateRequest.CategoryName = categoryName;
            createOrUpdateRequest.StatusName = STATUS;
            createOrUpdateRequest.Status = STATUSCODE;

            try
            {
                createOrUpdateReply = devBranchProxy.ActivityLibraryCreateOrUpdate(createOrUpdateRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from etblActivityLibraries: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from etblActivityLibraries: {0}", ex.Message);
            }

            Assert.IsNotNull(createOrUpdateReply, "ActivityLibraryDC object null");
            Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.StatusReply is null");
            Assert.AreEqual(SprocValues.REPLY_ERRORCODE_VALUE_OK, createOrUpdateReply.StatusReply.Errorcode, "createOrUpdateReply.StatusReply.Errorcode is not 0. Instead it is {0}.", createOrUpdateReply.StatusReply.Errorcode);
            Assert.IsTrue(string.IsNullOrEmpty(createOrUpdateReply.StatusReply.ErrorMessage), "createOrUpdateReply.StatusReply.ErrorMessage is not null");
            Assert.IsTrue(string.IsNullOrEmpty(createOrUpdateReply.StatusReply.ErrorGuid), "createOrUpdateReply.StatusReply.ErrorGuid is not null");

            int id = GetActivityLibrariesForValidNameAndValidateForMaxID(name);
            GetActivityLibrariesForValidID(id);
            Assert.IsNotNull(getReplyList, "getReplyList is null.");
            Assert.AreNotEqual(0, getReplyList.Count, " getReply.List.Count is 0.");
            Assert.AreEqual(description, getReplyList[0].Description, "Description did not get inserted or updated correctly");
            Assert.AreEqual(name, getReplyList[0].Name, "Name did not get inserted or updated correctly");

            return id;
        }

        /// <summary>
        /// Verify CreateOrUpdate FROM etblActivityLibraries Table for Invalid IDs. id is invalid if it's not 0 or not already in the table.
        /// </summary>
        /// <param name="id">id to try to insert or update</param>
        /// <param name="name">name to try to insert or update</param>
        /// <param name="description">description to do a create or update on.</param>
        /// <param name="version">version to do a create or update on.</param>
        /// <param name="authGroupName">authGroupName to do a create or update on.</param>
        /// <param name="categoryName">categoryName to do a create or update on.</param>
        private void CreateOrUpdateActivityLibrariesForInvalidIds(int id, string name, string description, string version, string authGroupName, string categoryName)
        {
            createOrUpdateRequest = new ActivityLibraryDC();

            createOrUpdateReply = null;

            //Populate the request data
            createOrUpdateRequest.Incaller = IN_CALLER;
            createOrUpdateRequest.IncallerVersion = IN_CALLER_VERSION;
            createOrUpdateRequest.Id = id;
            createOrUpdateRequest.Guid = Guid.NewGuid();
            createOrUpdateRequest.Name = name;
            createOrUpdateRequest.Description = description;
            createOrUpdateRequest.InInsertedByUserAlias = USER;
            createOrUpdateRequest.InUpdatedByUserAlias = USER;
            createOrUpdateRequest.VersionNumber = version;
            createOrUpdateRequest.AuthGroupName = authGroupName;
            createOrUpdateRequest.CategoryName = categoryName;
            createOrUpdateRequest.StatusName = STATUS;
            createOrUpdateRequest.Status = STATUSCODE;

            try
            {
                createOrUpdateReply = devBranchProxy.ActivityLibraryCreateOrUpdate(createOrUpdateRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from etblActivityLibraries: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from etblActivityLibraries: {0}", ex.Message);
            }

            int errorConstant = SprocValues.UPDATE_INVALID_ID;

            Assert.IsNotNull(createOrUpdateReply, "ActivityLibraryDC object null");
            Assert.IsNotNull(createOrUpdateReply.StatusReply, "getReply.Status is null");
            Assert.AreEqual(errorConstant, createOrUpdateReply.StatusReply.Errorcode, "Returned the wrong status error code. InId: {0}", id);
            Assert.IsNotNull(createOrUpdateReply.StatusReply.ErrorMessage, "createOrUpdateReply.StatusReply.ErrorMessage is null");
        }

        /// <summary>
        /// Verify UploadActivityLibrariesAndDependentActivities FROM etblActivityLibraries Table
        /// </summary>
        private void VerifyUploadActivityLibraryAndDependentActivities(string incaller, int errorCode)
        {
            storeLibraryAndActivitiesRequest = new StoreLibraryAndActivitiesRequestDC();
          //  reply = null;

            storeLibraryAndActivitiesRequest.IncallerVersion = IN_CALLER_VERSION;
            storeLibraryAndActivitiesRequest.Incaller = incaller;
            storeLibraryAndActivitiesRequest.InInsertedByUserAlias = USER;
            storeLibraryAndActivitiesRequest.InUpdatedByUserAlias = USER;

            // Create ActivityLibrary object and add to request object
            ActivityLibraryDC activityLibraryDC = new ActivityLibraryDC();

            // create storeActivitiesDC list and individual objects and add to request
            List<StoreActivitiesDC> storeActivitiesDCList = new List<StoreActivitiesDC>();
            StoreActivitiesDC storeActivitiesDC = new StoreActivitiesDC();

            CreateActivityLibraryAndStoreActivities(out activityLibraryDC, out storeActivitiesDCList);

            storeLibraryAndActivitiesRequest.ActivityLibrary = activityLibraryDC;
            storeLibraryAndActivitiesRequest.StoreActivityLibraryDependenciesGroupsRequestDC = CreateStoreActivityLibraryDependenciesGroupsRequestDC();
            storeLibraryAndActivitiesRequest.StoreActivitiesList = storeActivitiesDCList;

            storeActivitiesDCList.ForEach(record => record.Version = VersionHelper.GetNextVersion(record).ToString());

            try
            {
                replyList = new List<StoreActivitiesDC>(devBranchProxy.UploadActivityLibraryAndDependentActivities(storeLibraryAndActivitiesRequest));
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to upload from etblActivityLibraries: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to upload from etblActivityLibraries: {0}", ex.Message);
            }

            Assert.IsNotNull(replyList, "Reply is null");
            Assert.AreEqual(errorCode, replyList[0].StatusReply.Errorcode, "UploadActivityLibraryAndDependentActivities not successful.");
        }

        /// <summary>
        /// Create activity library entry and store activity entries
        /// </summary>
        /// <param name="activityLibraryDC"></param>
        /// <param name="storeActivityDC"></param>
        private void CreateActivityLibraryAndStoreActivities(out ActivityLibraryDC activityLibraryDC, out List<StoreActivitiesDC> storeActivityDC)
        {
            List<StoreActivitiesDC> storeActivityRequestList = new List<StoreActivitiesDC>();
            ActivityLibraryDC activityLibraryDCTemp = new ActivityLibraryDC();

            // create activity library entry
            activityLibraryDCTemp.Incaller = IN_CALLER;
            activityLibraryDCTemp.IncallerVersion = IN_CALLER_VERSION;
            activityLibraryDCTemp.Guid = Guid.NewGuid();
            activityLibraryDCTemp.Name = TEST_STRING;
            activityLibraryDCTemp.AuthGroupName = AUTH_GROUP_NAME2;
            activityLibraryDCTemp.CategoryName = CATEGORY_NAME2;
            activityLibraryDCTemp.Category = Guid.Empty;
            activityLibraryDCTemp.Executable = new byte[4];
            activityLibraryDCTemp.HasActivities = true;
            activityLibraryDCTemp.ImportedBy = USER;
            activityLibraryDCTemp.Description = TEST_STRING;
            activityLibraryDCTemp.InInsertedByUserAlias = USER;
            activityLibraryDCTemp.VersionNumber = VERSIONNUMBER1;
            activityLibraryDCTemp.InUpdatedByUserAlias = USER;
            activityLibraryDCTemp.Status = STATUSCODE;
            activityLibraryDCTemp.StatusName = STATUS;

            // Create 40 store activities and let it crank
            for (int i = 0; i < 40; i++)
            {
                storeActivityRequestList.Add(CreateStoreActivity(STOREACTIVITYNAME, "3.1.0." + i, new Guid(), activityLibraryDCTemp.Name, activityLibraryDCTemp.VersionNumber));
            }

            activityLibraryDC = activityLibraryDCTemp;
            storeActivityDC = storeActivityRequestList;
        }

        /// <summary>
        /// Creates a StoreActivityLibraryDependenciesGroupsRequestDC object
        /// </summary>
        /// <returns>StoreActivityLibraryDependenciesGroupsRequestDC</returns>
        private static StoreActivityLibraryDependenciesGroupsRequestDC CreateStoreActivityLibraryDependenciesGroupsRequestDC()
        {
            // Create StoreActivityLibraryDependenciesGroupsRequestDC and add it to request
            StoreActivityLibraryDependenciesGroupsRequestDC storeActivityLibraryDependenciesGroupsRequestDC = new StoreActivityLibraryDependenciesGroupsRequestDC();
            storeActivityLibraryDependenciesGroupsRequestDC.Name = STOREACTIVITYLIBRARYDEPENDENCIESGROUPSREQUESTDCNAME;
            storeActivityLibraryDependenciesGroupsRequestDC.Version = "1.0.0.3";
            storeActivityLibraryDependenciesGroupsRequestDC.List = new List<StoreActivityLibraryDependenciesGroupsRequestDC>();

            StoreActivityLibraryDependenciesGroupsRequestDC StoreActivityLibraryDependenciesGroupsRequestDC1 = new StoreActivityLibraryDependenciesGroupsRequestDC();
            StoreActivityLibraryDependenciesGroupsRequestDC1.Name = STOREACTIVITYLIBRARYDEPENDENCIESGROUPSREQUESTDCNAME;
            StoreActivityLibraryDependenciesGroupsRequestDC1.Version = "2.9.0.1";
            storeActivityLibraryDependenciesGroupsRequestDC.List.Add(StoreActivityLibraryDependenciesGroupsRequestDC1);

            StoreActivityLibraryDependenciesGroupsRequestDC StoreActivityLibraryDependenciesGroupsRequestDC2 = new StoreActivityLibraryDependenciesGroupsRequestDC();
            StoreActivityLibraryDependenciesGroupsRequestDC2.Name = STOREACTIVITYLIBRARYDEPENDENCIESGROUPSREQUESTDCNAME;
            StoreActivityLibraryDependenciesGroupsRequestDC2.Version = "1.0.0.5";
            storeActivityLibraryDependenciesGroupsRequestDC.List.Add(StoreActivityLibraryDependenciesGroupsRequestDC2);

            StoreActivityLibraryDependenciesGroupsRequestDC StoreActivityLibraryDependenciesGroupsRequestDC3 = new StoreActivityLibraryDependenciesGroupsRequestDC();
            StoreActivityLibraryDependenciesGroupsRequestDC3.Name = STOREACTIVITYLIBRARYDEPENDENCIESGROUPSREQUESTDCNAME;
            StoreActivityLibraryDependenciesGroupsRequestDC3.Version = "1.0.0.8";
            storeActivityLibraryDependenciesGroupsRequestDC.List.Add(StoreActivityLibraryDependenciesGroupsRequestDC3);

            return storeActivityLibraryDependenciesGroupsRequestDC;
        }

        /// <summary>
        /// Creates a store activity for test automation
        /// </summary>
        /// <param name="name">name to be insterted</param>
        /// <param name="version">version to be insterted</param>
        /// <param name="guid">guid to be insterted</param>
        /// <param name="activityLibraryName">activityLibraryName to be insterted</param>
        /// <param name="activityLibraryVersion">activityLibraryVersion to be insterted</param>
        /// <returns>StoreActivitiesDC object</returns>
        private static StoreActivitiesDC CreateStoreActivity(string name, string version, Guid guid, string activityLibraryName, string activityLibraryVersion)
        {
            StoreActivitiesDC storeActivityDC = new StoreActivitiesDC();
            storeActivityDC.ActivityLibraryName = activityLibraryName;
            storeActivityDC.ActivityLibraryVersion = activityLibraryVersion;
            storeActivityDC.ActivityCategoryName = CATEGORY_NAME2;
            storeActivityDC.AuthGroupName = AUTH_GROUP_NAME3;
            storeActivityDC.Description = TEST_STRING;
            storeActivityDC.Incaller = IN_CALLER;
            storeActivityDC.IncallerVersion = IN_CALLER_VERSION;
            storeActivityDC.InInsertedByUserAlias = USER;
            storeActivityDC.IsCodeBeside = true;
            storeActivityDC.IsService = true;
            storeActivityDC.Locked = true;
            storeActivityDC.LockedBy = USER;
            storeActivityDC.MetaTags = TEST_STRING;
            storeActivityDC.Name = name;
            storeActivityDC.Namespace = TEST_STRING;
            storeActivityDC.StatusCodeName = STATUSCODENAME;
            storeActivityDC.Guid = Guid.NewGuid();
            storeActivityDC.ToolBoxtab = 1;
            storeActivityDC.InUpdatedByUserAlias = USER;
            storeActivityDC.Version = version;
            storeActivityDC.WorkflowTypeName = WORKFLOWTYPENAME;
            storeActivityDC.Xaml = XAML;
            storeActivityDC.ShortName = TEST_STRING;

            return storeActivityDC;
        }

        [WorkItem(21030)]
        [Description("Verify GET FROM etblActivityLibraries Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetActivityLibrariesForValidIDs()
        {
            int[] validIDs = { 1, 2, 3 };

            foreach (int validID in validIDs)
            {
                GetActivityLibrariesForValidID(validID);
            }
        }

        [WorkItem(21033)]
        [Description("Verify GET ALL FROM etblActivityLibraries Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetAllActivityLibraries()
        {
            GetAllActivityLibraries();
        }

        [WorkItem(22215)]
        [Description("Verify GET ALL FROM etblActivityLibraries Table for invalid request object")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        [ExpectedException(typeof(FaultException<Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ValidationFault>))]
        public void VerifyGetAllActivityLibrariesForInvalidRequest()
        {
            VerifyGetAllActivityLibrariesForInvalidRequestObject();
        }


        [WorkItem(21025)]
        [Description("Verify GetActivitiesByActivityLibraryNameAndVersion FROM etblActivityLibraries Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetActivitiesByActivityLibraryNameAndVersion()
        {
            string name = Guid.NewGuid().ToString();
            string description = "Description for " + name;
            string version = "1.0.0.0";
            string categoryName = "Administration";

            CreateOrUpdateActivityLibraries(0, name, description, version, AUTH_GROUP_NAME, categoryName);
        }

        [WorkItem(21032)]
        [Description("Verify GET FROM etblStoreActivities Table for Valid name and version")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetActivityLibrariesForValidNameAndVersion()
        {
            string name = ACTIVITY_LIBRARY_NAME2;
            string version = VERSION2;
            VerifyGetActivityLibrariesForValidNameAndVersion(name, version);
        }

        [WorkItem(21031)]
        [Description("Verify GET FROM etblStoreActivities Table for Valid name and Invalid version")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetActivityLibrariesForValidNameAndInvalidVersion()
        {
            string name = ACTIVITY_LIBRARY_NAME;
            string version = VERSIONNUMBER1;
            VerifyGetActivityLibrariesForValidNameAndInvalidVersion(name, version);
        }

        [WorkItem(21029)]
        [Description("Verify GET FROM etblStoreActivities Table for Valid guid")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyGetActivityLibrariesForValidGuid()
        {
            Guid guid = new Guid(VALID_GUID);
            VerifyGetActivityLibrariesForValidGuid(guid);
        }

        [WorkItem(20978)]
        [Description("Verify CreateOrUpdate FROM etblActivityLibraries Table with ID = 0")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyCreateOrUpdateActivityLibraries()
        {
            string testFieldName = TEST_FIELD_NAME + Guid.NewGuid();

            // Create
            int id = CreateActivityLibrariesWithIdIs0(testFieldName, TEST_STRING, VERSION, AUTH_GROUP_NAME, CATEGORY_NAME);

            // Update
            UpdateActivityLibraries(id, testFieldName, TEST_STRING_2, VERSION, AUTH_GROUP_NAME, CATEGORY_NAME);
        }

        [WorkItem(20980)]
        [Description("Verify CreateOrUpdate FROM etblActivityLibraries Table with Null ID")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyCreateOrUpdateActivityLibrariesWithNullID()
        {
            string testFieldName = TEST_FIELD_NAME + Guid.NewGuid();
            // Create with null id
            int id = CreateActivityLibrariesWithNullID(testFieldName, TEST_STRING, VERSION, AUTH_GROUP_NAME, CATEGORY_NAME);

            // Update
            UpdateActivityLibraries(id, testFieldName, TEST_STRING_2, VERSION, AUTH_GROUP_NAME, CATEGORY_NAME);
        }

        [WorkItem(22203)]
        [Description("Verify UploadActivityLibraryAndDependentActivities FROM etblActivityLibraries Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyUploadActivityLibrariesAndDependentActivities()
        {
            //VerifyUploadActivityLibraryAndDependentActivities(IN_CALLER, CWF.Constants.SprocValues.REPLY_ERRORCODE_VALUE_OK);

            // invalid case:
            VerifyUploadActivityLibraryAndDependentActivities(null, SprocValues.INVALID_PARMETER_VALUE_INCALLER_ID);
        }

    }
}
