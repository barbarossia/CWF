
namespace Query_Service.UnitTests
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
    /// Unit tests for Query service TaskActivity Bal layer
    /// </summary>
    [TestClass]
    public class TaskActivityBALUnitTest
    {

        private static int activityId;

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            string nameModifier = "Test_" + Guid.NewGuid().ToString();
            CWF.DataContracts.StoreLibraryAndActivitiesRequestDC request = new CWF.DataContracts.StoreLibraryAndActivitiesRequestDC();
            List<CWF.DataContracts.StoreActivitiesDC> reply = null;

            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.Incaller = UnitTestConstant.INCALLER;
            request.InInsertedByUserAlias = UnitTestConstant.INCALLER;
            request.InUpdatedByUserAlias = UnitTestConstant.INCALLER;
            request.EnforceVersionRules = true;
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };

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
                        IncallerVersion = UnitTestConstant.INCALLERVERSION,
                        Incaller = UnitTestConstant.INCALLER,
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
                Incaller = UnitTestConstant.INCALLER,
                IncallerVersion = UnitTestConstant.INCALLERVERSION,
                AssignedTo = UnitTestConstant.OWNER,
                InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME },
                Environment = UnitTestConstant.TOENVIRONMENT,
            };
            try
            {

                reply1 = TaskActivityBusinessService.TaskActivityCreateOrUpdate(request1);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch exception in reply = TaskActivityBusinessService.CreateOrUpdate(request)");
            }
            Assert.IsNotNull(reply1);
            Assert.IsTrue(reply1.Id > 0);
            Assert.AreEqual(SprocValues.REPLY_ERRORCODE_VALUE_OK, reply1.StatusReply.Errorcode);
        }

        [TestCategory("Unit")]
        [Owner("v-kason")]
        [TestMethod]
        public void TestSearchTaskActivity()
        {
            TaskActivityGetRequestDC request = new TaskActivityGetRequestDC();
            request.AssignedTo = UnitTestConstant.OWNER;
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.SearchText = "Test";
            TaskActivityGetReplyDC reply = null;
            try
            {
                reply = TaskActivityBusinessService.GetTaskActivities(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch exception in reply = TaskActivityBusinessService.GetTaskActivities(request)");
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
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.ActivityId = activityId;
            TaskActivityDC reply = null;
            try
            {
                reply = TaskActivityBusinessService.TaskActivityGet(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch exception in reply = TaskActivityBusinessService.SearchTaskActivities(request)");
            }
            Assert.IsNotNull(reply);
            Assert.IsTrue(reply.Id >= 1);
        }

        [TestCategory("Unit")]
        [Owner("v-kason")]
        [TestMethod]
        public void TestTaskActivity_UpdateStatus()
        {
            TaskActivityDC request = new TaskActivityDC();
            request.ActivityId = activityId;
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
            request.Environment = UnitTestConstant.TOENVIRONMENT;
            TaskActivityDC reply = null;
            try
            {
                reply = TaskActivityRepositoryService.TaskActivityGet(request);
                request.Id = reply.Id;
                request.Status = TaskActivityStatus.CheckedIn;
                reply = TaskActivityBusinessService.TaskActivityUpdateStatus(request);
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
        public void TestGetTaskActivityList()
        {
            TaskActivityGetListRequest request = new TaskActivityGetListRequest();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.List = new List<TaskActivityDC>()
            { 
                new TaskActivityDC()
                {
                    ActivityId = activityId,
                }
            };

            TaskActivityGetListReply reply = null;
            try
            {
                reply = TaskActivityBusinessService.TaskActivityGetList(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch exception in reply = TaskActivityBusinessService.SearchTaskActivities(request)");
            }
            Assert.IsNotNull(reply);
            Assert.IsTrue(reply.List.Count == 1);
        }

        [TestCategory("Unit")]
        [Owner("v-kason")]
        [TestMethod]
        public void TestUploadTaskActivity()
        {
            string nameModifier = "Test_" + Guid.NewGuid().ToString();
            CWF.DataContracts.StoreLibraryAndTaskActivityRequestDC request = new CWF.DataContracts.StoreLibraryAndTaskActivityRequestDC();
            List<CWF.DataContracts.TaskActivityDC> reply = null;

            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.Incaller = UnitTestConstant.INCALLER;
            request.InInsertedByUserAlias = UnitTestConstant.INCALLER;
            request.InUpdatedByUserAlias = UnitTestConstant.INCALLER;
            request.EnforceVersionRules = true;
            request.Environment = UnitTestConstant.TOENVIRONMENT;
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };

            // Create ActivityLibrary object and add to request object
            CWF.DataContracts.ActivityLibraryDC activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();

            // create storeActivitiesDC list and individual objects and add to request
            List<CWF.DataContracts.StoreActivitiesDC> storeActivitiesDCList = new List<CWF.DataContracts.StoreActivitiesDC>();
            CWF.DataContracts.StoreActivitiesDC storeActivitiesDC = new CWF.DataContracts.StoreActivitiesDC();
            DALUnitTest.CreateActivityLibraryAndStoreActivities(out activityLibraryDC, out storeActivitiesDCList);
            request.ActivityLibrary = activityLibraryDC;
            storeActivitiesDCList.ForEach(record =>
            {
                record.Name += nameModifier;
                record.ActivityLibraryName += nameModifier;
                record.ShortName += nameModifier;

            });
            List<TaskActivityDC> taskActivityList = storeActivitiesDCList.Select(sa => new TaskActivityDC()
            {
                Activity = sa,
                AssignedTo = UnitTestConstant.OWNER,
                Guid = Guid.NewGuid(),
                Incaller = UnitTestConstant.INCALLER,
                IncallerVersion = UnitTestConstant.INCALLERVERSION,
                Environment = UnitTestConstant.TOENVIRONMENT,
                InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME },
            }).ToList();
            request.TaskActivitiesList = taskActivityList;

            activityLibraryDC.Name += nameModifier;

            request.StoreActivityLibraryDependenciesGroupsRequestDC = new StoreActivityLibraryDependenciesGroupsRequestDC()
            {
                Name = activityLibraryDC.Name,
                Version = activityLibraryDC.VersionNumber,
                List = new List<StoreActivityLibraryDependenciesGroupsRequestDC>
                {
                    new StoreActivityLibraryDependenciesGroupsRequestDC
                    {
                        IncallerVersion = UnitTestConstant.INCALLERVERSION,
                        Incaller = UnitTestConstant.INCALLER,
                        Name = "PublishingInfo",
                        Version = "1.0.0.1"
                    },
                }
            };
            reply = CWF.BAL.Services.UploadLibraryAndTaskActivities(request);
            Assert.IsNotNull(reply);
        }
    }
}
