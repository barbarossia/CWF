//-----------------------------------------------------------------------
// <copyright file="MtblActivityLibraryDependenciesTest.cs" company="Microsoft">
// Copyright
// A Test Class for Service Operations on mtblActivityLibraryDependencies table.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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
    /// A Test Class for Service Operations on mtblActivityLibraryDependencies table.
    /// </summary>
    [TestClass]
    public class MtblActivityLibraryDependenciesTest : QueryServiceTestBase
    {
        private const string VERSION = "1.0.1.0";
        private const string VERSIONDEPENEDNT = "1.0.0.1"; 

        private const string ACTIVITY_NAME = "PublishingWorkflow";
        private const string TABLE_NAME = "ActivityLibraryDependency";
        private const string ACTIVITY_LIBRARY_DEPENDENT_NAME_2 = "Publishing";
        private const string ACTIVITY_LIBRARY_DEPENDENT_NAME = "PublishingInfo";
        private const int ACTIVITY_LIBRARY_Id = 2;
        private const int ACTIVITY_LIBRARY_DEPENDENT_Id = 15;
        private const string ACTIVITY_LIBRARY_NAME = "PublishingWorkflow_WorkflowLibrary";
        private const string ACTIVITY_LIBRARY_NAME_TREE_GET = "PublishingWorkflow_WorkflowLibrary";

        StoreActivityLibrariesDependenciesDC createOrUpdateRequest;
        StoreActivityLibrariesDependenciesDC getRequest;

        StoreActivityLibrariesDependenciesDC createOrUpdateReply;
        List<StoreActivityLibrariesDependenciesDC> getReplyList;

        /// <summary>
        /// Verify GET FROM mtblActivityLibraryDependencies Table
        /// </summary>
        /// <param name="activitiyLibraryName">activitiyLibraryName to be used in the request</param>
        /// <param name="version">version to be used in the request</param>
        /// <param name="treeGet">if treeGet is true, a TreeGet is called, otherwise just a Get</param>
        /// <returns>returns the id of this row</returns>
        private int VerifyGetActivityLibraryDependencies(string activitiyLibraryName, string version, bool treeGet)
        {
            getRequest = new StoreActivityLibrariesDependenciesDC();
            getReplyList = new List<StoreActivityLibrariesDependenciesDC>();

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.UpdatedByUserAlias = USER;
            getRequest.InsertedByUserAlias = USER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            getRequest.StoreDependenciesRootActiveLibrary = new StoreDependenciesRootActiveLibrary();

            getRequest.StoreDependenciesRootActiveLibrary.ActivityLibraryVersionNumber = version;
            getRequest.StoreDependenciesRootActiveLibrary.ActivityLibraryName = activitiyLibraryName;

            try
            {
                getReplyList = new List<StoreActivityLibrariesDependenciesDC>(devBranchProxy.StoreActivityLibraryDependenciesTreeGet(getRequest));
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from mtblActivityLibraryDependencies: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from mtblActivityLibraryDependencies: {0}", ex.Message);
            }

            Assert.IsNotNull(getReplyList, "getReply.List is null");
            Assert.AreEqual(1, getReplyList.Count, "Get returned the wrong number of entries. activitiyLibraryName: {0}. It should have returned 1 but instead returned {1}.", activitiyLibraryName, getReplyList.Count);
            Assert.IsNotNull(getReplyList[0].StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(0, getReplyList[0].StatusReply.Errorcode, "StatusReply.Errorcode returned the wrong value. Errorcode: {0}", getReplyList[0].StatusReply.Errorcode);
            
            //bug 14840 for the next sprint
            //Assert.AreEqual(activitiyLibraryName, getReplyList[0].StoreDependenciesRootActiveLibrary.ActivityLibraryName, "Get returned the wrong ActivityLibraryName. Expected activitiyLibraryName: {0}. Actual ActivityLibraryName", activitiyLibraryName, getReplyList[0].StoreDependenciesRootActiveLibrary.ActivityLibraryName);
            //Assert.AreEqual(version, getReplyList[0].StoreDependenciesRootActiveLibrary.ActivityLibraryVersionNumber, "Get returned the wrong version. Expected version: {0}. Actual ActivityLibraryName", version, getReplyList[0].StoreDependenciesRootActiveLibrary.ActivityLibraryVersionNumber);
            Assert.IsNotNull(getReplyList[0].StoreDependenciesDependentActiveLibraryList, "StoreDependenciesDependentActiveLibraryList is null");
            Assert.AreNotEqual(0, getReplyList[0].StoreDependenciesDependentActiveLibraryList.Count, "StoreDependenciesDependentActiveLibraryList is empty");

            int id = getReplyList[0].StoreDependenciesDependentActiveLibraryList[0].ActivityLibraryParentId;
            return id;
        }

        /// <summary>
        /// Verify GET FROM mtblActivityLibraryDependencies Table
        /// </summary>
        /// <param name="activitiyLibraryName">activitiyLibraryName to be used in the request</param>
        /// <param name="version">version to be used in the request</param>
        /// <param name="treeGet">if treeGet is true, a TreeGet is called, otherwise just a Get</param>
        private void GetActivityLibraryDependenciesForInvalidNameOrVersion(string activitiyLibraryName, string version, bool isTreeGet)
        {
            bool isFaultException = false;

            getRequest = new StoreActivityLibrariesDependenciesDC();
            getReplyList = new List<StoreActivityLibrariesDependenciesDC>();

            //Populate the request data
            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;
            getRequest.InsertedByUserAlias = USER;
            getRequest.UpdatedByUserAlias = USER;

            getRequest.StoreDependenciesRootActiveLibrary = new StoreDependenciesRootActiveLibrary();

            getRequest.StoreDependenciesRootActiveLibrary.ActivityLibraryName = activitiyLibraryName;
            getRequest.StoreDependenciesRootActiveLibrary.ActivityLibraryVersionNumber = version;

            try
            {
                if (isTreeGet)
                {
                    getReplyList = new List<StoreActivityLibrariesDependenciesDC>(devBranchProxy.StoreActivityLibraryDependenciesTreeGet(getRequest));
                }
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to get data from mtblActivityLibraryDependencies: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from mtblActivityLibraryDependencies: {0}", ex.Message);
            }

            if (!isFaultException)
            {
                int errorCode = GetErrorCodeForActivityLibraryDependencies(activitiyLibraryName, version);
                Assert.IsNotNull(getReplyList, "getReply.List is null");
                Assert.AreEqual(1, getReplyList.Count, "Get returned the wrong number of entries. activitiyLibraryName: {0}. It should have returned 1 but instead returned {1}.", activitiyLibraryName, getReplyList.Count);
                Assert.IsNotNull(getReplyList[0].StatusReply, "getReply.StatusReply is null");
                Assert.AreEqual(errorCode, getReplyList[0].StatusReply.Errorcode, "StatusReply.Errorcode returned the wrong value. Errorcode: {0}", getReplyList[0].StatusReply.Errorcode);
            }
        }


        /// <summary>
        /// Verify CreateActivityLibrariesDependencyList FROM mtblActivityLibraryDependencies Table..
        /// </summary>
        /// <param name="activitiyLibraryName">activitiyLibraryName to do a create or update on.</param>
        /// <param name="version">version to do a create or update on.</param>
        /// <param name="activityLibraryDependentName">activityLibraryDependentName to do a create or update on.</param>
        /// <returns>returns the id created</returns>
        private int CreateActivityLibrariesDependencyList(int activitiyLibraryId, string activitiyLibraryName, string version, 
                                                                int activityLibraryDependentId, string activityLibraryDependentName, string versionDependent)
        {
            createOrUpdateReply = null;
            
            //Populate the request data
            createOrUpdateRequest = new StoreActivityLibrariesDependenciesDC();
            createOrUpdateRequest.Incaller = IN_CALLER;
            createOrUpdateRequest.UpdatedByUserAlias = USER;
            createOrUpdateRequest.InsertedByUserAlias = USER;
            createOrUpdateRequest.IncallerVersion = IN_CALLER_VERSION;

            var storeDependenciesActivityLib = new StoreDependenciesRootActiveLibrary();
            storeDependenciesActivityLib.ActivityLibraryId = activitiyLibraryId;
            storeDependenciesActivityLib.ActivityLibraryName = activitiyLibraryName;
            storeDependenciesActivityLib.ActivityLibraryVersionNumber = version;

            //Set Request StoreDependenciesRootActiveLibrary object
            createOrUpdateRequest.StoreDependenciesRootActiveLibrary = storeDependenciesActivityLib;
            
            var storeDependenciesDependentActiveLibraryList = new List<StoreDependenciesDependentActiveLibrary>();
            StoreDependenciesDependentActiveLibrary storeDependenciesDependentActiveLibrary = new StoreDependenciesDependentActiveLibrary();
            storeDependenciesDependentActiveLibrary.ActivityLibraryDependentId = activityLibraryDependentId;
            storeDependenciesDependentActiveLibrary.ActivityLibraryDependentName = activityLibraryDependentName;
            storeDependenciesDependentActiveLibrary.ActivityLibraryDependentVersionNumber = versionDependent;
            storeDependenciesDependentActiveLibraryList.Add(storeDependenciesDependentActiveLibrary);

            //set StoreDependenciesDependentActiveLibraryList object
            createOrUpdateRequest.StoreDependenciesDependentActiveLibraryList = storeDependenciesDependentActiveLibraryList;
            
            try
            {
                createOrUpdateReply = devBranchProxy.StoreActivityLibraryDependencyList(createOrUpdateRequest);
            }
            catch (FaultException e)
            {
                Assert.Fail("Failed to createOrUpdate data from mtblActivityLibraryDependencies: {0}", e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to createOrUpdate data from mtblActivityLibraryDependencies: {0}", ex.Message);
            }

            Assert.IsNotNull(createOrUpdateReply, "StoreActivityLibrariesDependenciesDC object null");
            Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.StatusReply is null");
            Assert.AreEqual(0, createOrUpdateReply.StatusReply.Errorcode, "createOrUpdateReply.StatusReply.Errorcode is not 0. Instead it is {0}.", createOrUpdateReply.StatusReply.Errorcode);

            int activityLibraryId = VerifyGetActivityLibraryDependencies(activitiyLibraryName, version, false);
            Assert.IsNotNull(getReplyList, "getReplyList is null.");
            Assert.AreNotEqual(0, getReplyList.Count, " getReply.List.Count is 0.");

            return activityLibraryId;
        }

        /////// <summary>
        /////// Verify CreateActivityLibrariesDependencyList FROM mtblActivityLibraryDependencies Table when recursion occurs.
        /////// </summary>
        /////// <param name="activitiyLibraryName">activitiyLibraryName to do a create or update on.</param>
        /////// <param name="version">version to do a create or update on.</param>
        /////// <param name="activityLibraryDependentName">activityLibraryDependentName to do a create or update on.</param>
        ////private void CreateActivityLibrariesDependencyListWithRecursion(int activitiyLibraryId, string activitiyLibraryName, string version, string activityLibraryDependentName)
        ////{
        ////    createOrUpdateRequest = new StoreActivityLibrariesDependenciesDC();

        ////    createOrUpdateReply = null;

        ////    //Populate the request data
        ////    createOrUpdateRequest.Incaller = IN_CALLER;
        ////    createOrUpdateRequest.IncallerVersion = IN_CALLER_VERSION;
        ////    createOrUpdateRequest.InsertedByUserAlias = USER;
        ////    createOrUpdateRequest.UpdatedByUserAlias = USER;

        ////    createOrUpdateRequest.StoreDependenciesRootActiveLibrary = new StoreDependenciesRootActiveLibrary();
        ////    createOrUpdateRequest.StoreDependenciesRootActiveLibrary.ActivityLibraryId = activitiyLibraryId;
        ////    createOrUpdateRequest.StoreDependenciesRootActiveLibrary.ActivityLibraryName = activitiyLibraryName;
        ////    createOrUpdateRequest.StoreDependenciesRootActiveLibrary.ActivityLibraryVersionNumber = version;

        ////    createOrUpdateRequest.StoreDependenciesDependentActiveLibraryList = new List<StoreDependenciesDependentActiveLibrary>();
        ////    StoreDependenciesDependentActiveLibrary storeDependenciesDependentActiveLibrary = new StoreDependenciesDependentActiveLibrary();
        ////    storeDependenciesDependentActiveLibrary.ActivityLibraryDependentName = activityLibraryDependentName;
        ////    storeDependenciesDependentActiveLibrary.ActivityLibraryDependentVersionNumber = version;
        ////    createOrUpdateRequest.StoreDependenciesDependentActiveLibraryList.Add(storeDependenciesDependentActiveLibrary);

        ////    try
        ////    {
        ////        createOrUpdateReply = proxy.StoreActivityLibraryDependencyList(createOrUpdateRequest);
        ////    }
        ////    catch (FaultException e)
        ////    {
        ////        Assert.Fail("Failed to createOrUpdate data from mtblActivityLibraryDependencies: {0}", e.Message);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Assert.Fail("Failed to createOrUpdate data from mtblActivityLibraryDependencies: {0}", ex.Message);
        ////    }

        ////    Assert.IsNotNull(createOrUpdateReply, "StoreActivityLibrariesDependenciesDC object null");
        ////    Assert.IsNotNull(createOrUpdateReply.StatusReply, "createOrUpdateReply.StatusReply is null");
        ////    Assert.AreEqual(SprocValues.INVALID_PARMETER_VALUE_ACTIVITYLIBRARYDEPENDENCY_ID,
        ////                    createOrUpdateReply.StatusReply.Errorcode,
        ////                    "createOrUpdateReply.StatusReply.Errorcode is not {0}. Instead it is {1}.",
        ////                    SprocValues.INVALID_PARMETER_VALUE_ACTIVITYLIBRARYDEPENDENCY_ID,
        ////                    createOrUpdateReply.StatusReply.Errorcode);
        ////}

        /// <summary>
        /// Gets the correct error code depending on the activitiyLibraryName and version 
        /// </summary>
        /// <param name="activitiyLibraryName"></param>
        /// <param name="version"></param>
        /// <returns>correct error code</returns>
        private int GetErrorCodeForActivityLibraryDependencies(string activitiyLibraryName, string version)
        {
            int errorCode = 0;

            if (string.IsNullOrEmpty(activitiyLibraryName))
                errorCode = SprocValues.INVALID_PARMETER_VALUE_INNAME_ID;
            else if (string.IsNullOrEmpty(version))
                errorCode = SprocValues.INVALID_PARMETER_VALUE_INVERSION_ID;
            else
                errorCode = SprocValues.INVALID_PARMETER_VALUE_INNAMEINVERSION_ID;

            return errorCode;
        }

        /// <summary>
        /// Gets the correct error code when deleting, creating, or updating, depending on the activitiyLibraryName and version 
        /// </summary>
        /// <param name="activitiyLibraryName"></param>
        /// <param name="version"></param>
        /// <returns>correct error code</returns>
        private int GetInvalidDeleteOrCrudErrorCodeForActivityLibraryDependencies(string activitiyLibraryName, string version)
        {
            int errorCode = 0;

            if (string.IsNullOrEmpty(activitiyLibraryName))
                errorCode = SprocValues.INVALID_PARMETER_VALUE_INACTIVITYLIBRARYNAME_ID;
            else if (string.IsNullOrEmpty(version))
                errorCode = -129; //not defined yet in ErrorConstants.cs
            else
                errorCode = SprocValues.INVALID_PARMETER_VALUE_INACTIVITYLIBRARYVERSIONNUMBER_ID;

            return errorCode;
        }

        [WorkItem(20960)]
        [Description("Verify TREEGET FROM mtblActivityLibraryDependencies Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyActivityLibraryDependenciesTreeGet()
        {
            VerifyGetActivityLibraryDependencies(ACTIVITY_LIBRARY_NAME_TREE_GET, VERSION, true);
        }

        [WorkItem(20957)]
        [Description("Verify GET FROM mtblActivityLibraryDependencies Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyActivityLibraryDependenciesGet()
        {
            VerifyGetActivityLibraryDependencies(ACTIVITY_LIBRARY_NAME, VERSION, false);
        }

        [WorkItem(20959)]
        [Description("Verify GET FROM mtblActivityLibraryDependencies Table for Invalid Name or Version")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        [Ignore]
        public void VerifyActivityLibraryDependenciesGetForInvalidNameOrVersion()
        {
            GetActivityLibraryDependenciesForInvalidNameOrVersion(TEST_STRING, VERSION, false);

            GetActivityLibraryDependenciesForInvalidNameOrVersion(ACTIVITY_LIBRARY_NAME, TEST_STRING, false);

            GetActivityLibraryDependenciesForInvalidNameOrVersion(TEST_STRING, TEST_STRING, false);

            GetActivityLibraryDependenciesForInvalidNameOrVersion(ACTIVITY_LIBRARY_NAME, string.Empty, false);

            GetActivityLibraryDependenciesForInvalidNameOrVersion(ACTIVITY_LIBRARY_NAME, null, false);

            GetActivityLibraryDependenciesForInvalidNameOrVersion(string.Empty, VERSION, false);

            GetActivityLibraryDependenciesForInvalidNameOrVersion(null, VERSION, false);
        }

        
        [WorkItem(20973)]
        [Description("Verify CreateActivityLibrariesDependencyList FROM mtblActivityLibraryDependencies Table ")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Full)]
        [TestMethod]
        public void VerifyCreateActivityLibrariesDependencyList()
        {
            var name = ACTIVITY_LIBRARY_NAME;// +nameExtender;           
            var dependentName = ACTIVITY_LIBRARY_DEPENDENT_NAME;// + nameExtender;

            int id1 = CreateActivityLibrariesDependencyList(ACTIVITY_LIBRARY_Id, name, VERSION, ACTIVITY_LIBRARY_DEPENDENT_Id, dependentName ,VERSIONDEPENEDNT);
            int id2 = VerifyGetActivityLibraryDependencies(name, VERSION, false);

            Assert.AreEqual(id1, id2, "Not getting the right id from the newly created row. Correct: {0}. Actual: {1}", id1, id2);

            // Update
            id1 = CreateActivityLibrariesDependencyList(ACTIVITY_LIBRARY_Id, name, VERSION, ACTIVITY_LIBRARY_DEPENDENT_Id, dependentName, VERSIONDEPENEDNT);
            id2 = VerifyGetActivityLibraryDependencies(name, VERSION, false);
            Assert.AreEqual(id1, id2, "Not getting the right id from the newly updated row. Correct: {0}. Actual: {1}", id1, id2);
        }
    }
}
