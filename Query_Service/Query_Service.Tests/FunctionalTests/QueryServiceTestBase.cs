//-----------------------------------------------------------------------
// <copyright file="QueryServiceTestBase.cs" company="Microsoft">
// Copyright
// A base test class that provides common functionalities to the derived test classes.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using System.ServiceModel;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.Support.Workflow.Service.DataAccessServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Service.Test.Common;

namespace Query_Service.Tests
{
    /// <summary>
    /// A base test class that provides common functionalities to the derived test classes.
    /// </summary>
    [TestClass]
    public class QueryServiceTestBase
    {
        protected static string IN_CALLER = "cwf.dal";
        protected static string IN_CALLER_VERSION = "1.0.0.0";
        protected static string TEST_DATABASE = ConfigurationManager.AppSettings["DatabaseName"];
        protected static string USER = Environment.UserName;
        protected static int STATUSCODE = 1010;
        protected static string STATUS = "Public";          
        protected static string TEST_STRING = Utility.GenerateRandomString(10);
        protected static string TEST_STRING_2 = Utility.GenerateRandomString(10);
        protected static string TEST_FIELD_NAME = Utility.GenerateRandomString(10);
        protected const string TEST_OWNER = "v-yiabdi";
        protected const string PERF_TESTING_ASSEMBLY_FOLDER = @"\\PQOOASSWS01\QueryServiceTests\Microsoft.VisualStudio.QualityTools.UnitTestFramework.dll";
        protected const string QUERY_SERVICE_ENDPOINT_NAME = "BasicHttpBinding_IWorkflowsQueryService";

        //protected WorkflowsQueryServiceClient proxy;
        protected QueryServiceExtensionClient testProxy;
        protected WorkflowsQueryServiceClientDevBranch devBranchProxy;

        /// <summary>
        /// A test initializer that gets executed before every test method.
        /// </summary>
        [TestInitialize]
        public void TestInit()
        {
            //proxy = new WorkflowsQueryServiceClient(QUERY_SERVICE_ENDPOINT_NAME);
            devBranchProxy = new WorkflowsQueryServiceClientDevBranch(QUERY_SERVICE_ENDPOINT_NAME);
            testProxy = new QueryServiceExtensionClient();
        }

        /// <summary>
        /// A test cleanup method that runs after all test method.
        /// </summary>
        [ClassCleanup]
        public void CleanUp()
        {
            if (testProxy != null)
            {
                testProxy.Close();
            }
            if (devBranchProxy != null)
            {
                devBranchProxy.Close();
            }
        }

        /// <summary>
        /// HARD deletes row given an id and name and database
        /// </summary>
        /// <param name="id">The id of the row to be deleted</param>
        /// <param name="table">The table of the row to be deleted</param>
        /// <param name="database">The database of the row to be deleted. Database has to be in the list of test databases</param>
        protected void HardDeleteThisRow(int id, string table, string database)
        {
            Query_Service.ExtensionForTests.DataProxy.StatusReplyDC deleteReply = null;

            try
            {
                deleteReply = testProxy.DeleteFromId(id, table, database);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to delete data from {0}: {1}", table, ex.Message);
            }

            Assert.IsNotNull(deleteReply, "StatusReplyDC object null");
            Assert.AreEqual(0, deleteReply.Errorcode, "Delete operation not successful.");
        }

        /// <summary>
        /// put the soft delete back in so other tests won't be affected
        /// </summary>
        /// <param name="id"> id of the row to be softdeleted</param>
        /// <param name="table">name of the table to be softdeleted</param>
        protected void UpdateSoftDelete(string id, string table)
        {
            Query_Service.ExtensionForTests.DataProxy.StatusReplyDC deleteReply = null;
            ExtensionForTests.DataProxy.UpdateSoftDeleteRequestDC updateSoftDeleteRequest = new ExtensionForTests.DataProxy.UpdateSoftDeleteRequestDC();
            updateSoftDeleteRequest.Id = id;
            updateSoftDeleteRequest.TableName = table;

            try
            {
                deleteReply = testProxy.UpdateSoftDelete(updateSoftDeleteRequest);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to delete data from {0}: {1}", table, ex.Message);
            }

            Assert.IsNotNull(deleteReply, "StatusReplyDC object null");
            Assert.AreEqual(0, deleteReply.Errorcode, "Delete operation not successful.");
        }

        /// <summary>
        /// Get ErrorCode based on the Id
        /// </summary>
        /// <param name="id">id of row we're trying to get</param>
        /// <returns>the corresponding error constant</returns>
        protected int GetErrorConstantInvalidID(int id)
        {
            return id < 1 ? SprocValues.INVALID_PARMETER_VALUE_INID_ID : SprocValues.GET_INVALID_ID;
        }

        /// <summary>
        /// Get ErrorCode based on the Incode
        /// </summary>
        /// <param name="inCode">incode of row we're trying to get</param>
        /// <returns>the corresponding error constant</returns>
        protected int GetErrorConstantInvalidIncode(int inCode)
        {
            return inCode < 1 ? SprocValues.INVALID_PARMETER_VALUE_INCODE_ID : SprocValues.GET_INVALID_ID;
        }

        /// <summary>
        /// Get ErrorCode based on the Id
        /// </summary>
        /// <param name="id">id of row we're trying to create or update</param>
        /// <returns>the corresponding error constant</returns>
        protected int GetErrorConstantInvalidIDForUpdate(int id)
        {
            return id < 1 ? SprocValues.INVALID_PARMETER_VALUE_INID_ID : SprocValues.UPDATE_INVALID_ID;
        }

        /// <summary>
        /// Get ErrorCode based on the Id for get. This row has been soft deleted.
        /// </summary>
        /// <returns>the corresponding error constant</returns>
        protected int GetErrorConstantSoftDeletedID()
        {
            return SprocValues.GET_INVALID_GETID_ON_SOFTDELETEDROW_ID;
        }

        /// <summary>
        /// Get ErrorCode based on the Incode for Delete
        /// </summary>
        /// <param name="inCode">incode of row we're trying to delete</param>
        /// <returns>the corresponding error constant</returns>
        protected int GetErrorConstantDeleteInvalidIncode(int inCode)
        {
            return inCode < 1 ? SprocValues.INVALID_PARMETER_VALUE_INCODE_ID : SprocValues.DELETE_INVALID_ID;
        }

        /// <summary>
        /// Get ErrorCode based on the Id for Delete
        /// </summary>
        /// <param name="id">id of row we're trying to soft delete</param>
        /// <returns>the corresponding error constant</returns>
        protected int GetErrorConstantDeleteInvalidID(int id)
        {
            return id < 1 ? SprocValues.INVALID_PARMETER_VALUE_INID_ID : SprocValues.DELETE_INVALID_ID;
        }

        /// <summary>
        /// Verify GET for valid name and version
        /// </summary>
        /// <param name="name">name of row to do a get on</param>
        /// <param name="version">version of name to do a get on</param>
        /// <param name="getRequest">the request object that we'll use reflection on</param>
        /// <param name="getReplyList">the reply object we'll use reflection on</param>
        public void VerifyGetForValidNameAndVersion(string name, string version, object getRequest, object getReplyList)
        {
            Type typeRequest = getRequest.GetType();
            typeRequest.GetProperty("Incaller").SetValue(getRequest, IN_CALLER, null);
            typeRequest.GetProperty("IncallerVersion").SetValue(getRequest, IN_CALLER_VERSION, null);
            typeRequest.GetProperty("Name").SetValue(getRequest, name, null);
            typeRequest.GetProperty("Environment").SetValue(getRequest, "Dev", null);
            if (typeRequest.Name == "ActivityLibraryDC")
                typeRequest.GetProperty("VersionNumber").SetValue(getRequest, version, null);
            else
                typeRequest.GetProperty("Version").SetValue(getRequest, version, null);

            try
            {
                if (typeRequest.Name == "ActivityLibraryDC")
                    getReplyList = devBranchProxy.ActivityLibraryGet((ActivityLibraryDC)getRequest);
                else if (typeRequest.Name == "StoreActivitiesDC")
                    getReplyList = devBranchProxy.StoreActivitiesGet((StoreActivitiesDC)getRequest);
                else
                    Assert.Fail("Request object is not the correct type");
            }
            catch (FaultException e)
            {
                Assert.Fail("FaultException. Failed to get data from {0}: {1}", typeRequest.Name, e.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to get data from {0}: {1}", typeRequest.Name, ex.Message);
            }

            Type typeReply = getReplyList.GetType();
            Assert.IsTrue(typeReply.IsArray);
            Type typeParameters = typeReply.GetElementType();
            Assert.IsNotNull(typeParameters, "getReplyList is null");

            if (typeParameters.Name == "ActivityLibraryDC")
            {
                ActivityLibraryDC[] getReplyListCasted = getReplyList as ActivityLibraryDC[];
                Assert.IsNotNull(getReplyListCasted);
                Assert.AreEqual(1, getReplyListCasted.Length);
                var a = typeParameters.GetProperty("Name").GetValue(getReplyListCasted[0], null);
                Assert.AreEqual(name, typeParameters.GetProperty("Name").GetValue(getReplyListCasted[0], null).ToString(), "Get returned wrong data for name");
                Assert.AreEqual(version, typeParameters.GetProperty("VersionNumber").GetValue(getReplyListCasted[0], null).ToString(), "Get returned wrong data for version");
            }
            else if (typeParameters.Name == "StoreActivitiesDC")
            {
                StoreActivitiesDC[] getReplyListCasted = getReplyList as StoreActivitiesDC[];
                Assert.IsNotNull(getReplyListCasted);
                Assert.AreEqual(1, getReplyListCasted.Length);
                Assert.AreEqual(name, typeParameters.GetProperty("Name").GetValue(getReplyListCasted[0], null).ToString(), "Get returned wrong data for name");
                Assert.AreEqual(version, typeParameters.GetProperty("Version").GetValue(getReplyListCasted[0], null).ToString(), "Get returned wrong data for version");
            }
            else
                Assert.Fail("Reply object is not the correct type");
        }

        /// <summary>
        /// This method is used in perf testing. It is called by reflection from TestController.cs.
        /// </summary>
        public void LoadUnitTestFrameworkAssembly()
        {
            Assembly unitTestFrameworkAssembly = Assembly.LoadFile(PERF_TESTING_ASSEMBLY_FOLDER);
        }
    }
}
