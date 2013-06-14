
namespace Query_Service.Testsproject
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using CWF.BAL.Versioning;
    using CWF.DataContracts;
    using Microsoft.Support.Workflow.Service.BusinessServices;
    using Microsoft.Support.Workflow.Service.DataAccessServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Support.Workflow.Service.Test.Common;
    using CWF.BAL;
    /// <summary>
    /// Unit tests for Query service TaskActivity DAL layer
    /// </summary>
    [TestClass]
    public class TaskActivityDALUnitTest
    {
        private const string INCALLER = "v-kason";
        private const string INCALLERVERSION = "1.0.0.0";
        private const string OWNER = "v-kason";
        private const string UPDATEDBYUSERALIAS = "v-kason";
        private const string INSERTEDBYUSERALIAS = "v-kason";
        private static int activityId;

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            string nameModifier = "Test_" + Guid.NewGuid().ToString();
            CWF.DataContracts.StoreLibraryAndActivitiesRequestDC request = new CWF.DataContracts.StoreLibraryAndActivitiesRequestDC();
            List<CWF.DataContracts.StoreActivitiesDC> reply = null;

            request.IncallerVersion = INCALLERVERSION;
            request.Incaller = INCALLER;
            request.InInsertedByUserAlias = INCALLER;
            request.InUpdatedByUserAlias = INCALLER;
            request.EnforceVersionRules = true;

            // Create ActivityLibrary object and add to request object
            CWF.DataContracts.ActivityLibraryDC activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();

            // create storeActivitiesDC list and individual objects and add to request
            List<CWF.DataContracts.StoreActivitiesDC> storeActivitiesDCList = new List<CWF.DataContracts.StoreActivitiesDC>();
            CWF.DataContracts.StoreActivitiesDC storeActivitiesDC = new CWF.DataContracts.StoreActivitiesDC();
            DALUnitTest.CreateActivityLibraryAndStoreActivities(out activityLibraryDC, out storeActivitiesDCList);
            request.ActivityLibrary = activityLibraryDC;
            request.StoreActivitiesList = storeActivitiesDCList;
            request.StoreActivitiesList.ForEach(record =>
            {
                record.Name += nameModifier;
                record.ActivityLibraryName += nameModifier;
                record.ShortName += nameModifier;

            });

            activityLibraryDC.Name += nameModifier;

            request.StoreActivityLibraryDependenciesGroupsRequestDC = new StoreActivityLibraryDependenciesGroupsRequestDC()
            {
                Name = activityLibraryDC.Name,
                Version = activityLibraryDC.VersionNumber,
                List = new List<StoreActivityLibraryDependenciesGroupsRequestDC>
                {
                    new StoreActivityLibraryDependenciesGroupsRequestDC
                    {
                        IncallerVersion = INCALLERVERSION,
                        Incaller = INCALLER,
                        Name = "PublishingInfo",
                        Version = "1.0.0.1"
                    },
                }
            };
            reply = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(request);
            Assert.IsNotNull(reply);
            activityId = reply[0].Id;
            Assert.IsTrue(activityId > 0);

            //Test Create or Update
            Guid taskGuid = Guid.NewGuid();
            TaskActivityDC reply1 = null;
            TaskActivityDC request1 = new TaskActivityDC()
            {
                ActivityId = activityId,
                Guid = taskGuid,
                Incaller = INCALLER,
                IncallerVersion = INCALLERVERSION,
                AssignedTo = OWNER
            };
            try
            {
                reply1 = TaskActivityRepositoryService.TaskActivitiesCreateOrUpdate(request1);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch exception in reply = TaskActivityRepository.CreateOrUpdate(request)");
            }
            Assert.IsNotNull(reply1);
            Assert.IsTrue(reply1.Id > 0);
            Assert.AreEqual(SprocValues.REPLY_ERRORCODE_VALUE_OK, reply1.StatusReply.Errorcode);
        }

        [TestCategory("Unit")]
        [Owner("v-kason")]
        [TestMethod]
        public void TestTaskActivity_UpdateStatus()
        {
            TaskActivityDC request = new TaskActivityDC();
            request.ActivityId = activityId;
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
            TaskActivityDC reply = null;
            try
            {
                reply = TaskActivityRepositoryService.TaskActivityGet(request);
                request.Id = reply.Id;
                request.Status = TaskActivityStatus.CheckedIn;
                reply = TaskActivityRepositoryService.TaskActivity_SetStatus(request);
                Assert.AreEqual(reply.StatusReply.Errorcode, 0);
                reply = TaskActivityRepositoryService.TaskActivityGet(request);
                Assert.AreEqual(reply.Status, TaskActivityStatus.CheckedIn);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch exception in reply = TaskActivityRepository.SearchTaskActivities(request)");
            }
            Assert.IsNotNull(reply);
            Assert.IsTrue(reply.Id >= 1);
        }

        [TestCategory("Unit")]
        [Owner("v-kason")]
        [TestMethod]
        public void TestSearchTaskActivity()
        {
            TaskActivityGetRequestDC request = new TaskActivityGetRequestDC();
            request.AssignedTo = OWNER;
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
            request.SearchText = "Test";
            request.IncludeDetails = true;
            TaskActivityGetReplyDC reply = null;
            try
            {
                reply = TaskActivityRepositoryService.SearchTaskActivities(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch exception in reply = TaskActivityRepository.SearchTaskActivities(request)");
            }
            Assert.IsNotNull(reply);
            Assert.IsTrue(reply.ServerResultsLength >= 1);
        }

        [TestCategory("Unit")]
        [Owner("v-kason")]
        [TestMethod]
        public void TestGetTaskActivity()
        {
            TaskActivityDC request = new TaskActivityDC();
            request.ActivityId = activityId;
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
            TaskActivityDC reply = null;
            try
            {
                reply = TaskActivityRepositoryService.TaskActivityGet(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch exception in reply = TaskActivityRepository.SearchTaskActivities(request)");
            }
            Assert.IsNotNull(reply);
            Assert.IsTrue(reply.Id >= 1);
        }


    }
}
