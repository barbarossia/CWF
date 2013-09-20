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

namespace Query_Service.UnitTests
{
    /// <summary>
    /// Unit tests for QueryService BAl and DAL layer
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "This not required")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Not required fot const/unit tests")]
    [TestClass]
    public class ActivityUnitTest
    {
        [Description("Get The entire StoreActivities table")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void StoreActivitiesGet()
        {
            CWF.DataContracts.StoreActivitiesDC request = new CWF.DataContracts.StoreActivitiesDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.Name = "PublishingWorkflow";
            request.Version = "1.0.1.0";
            request.Environment = UnitTestConstant.TOENVIRONMENT;
            List<CWF.DataContracts.StoreActivitiesDC> reply = null;

            try
            {
                reply = Activities.StoreActivitiesGet(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.Activities.StoreActivitiesGet(request);");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply[0].StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }

        [Description("Get all records for a workflow name")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void StoreActivitiesGetByName()
        {
            CWF.DataContracts.StoreActivitiesDC request = new CWF.DataContracts.StoreActivitiesDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.Name = "PublishingWorkflow";
            List<CWF.DataContracts.StoreActivitiesDC> reply = null;

            try
            {
                reply = Activities.StoreActivitiesGetByName(request.Name, "test");
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.Activities.StoreActivitiesGetByName('PublishingWorkflow','');");
            }

            Assert.IsNotNull(reply);
            Assert.IsTrue(reply.Any());
            Assert.IsNotNull(reply[0].InsertedDateTime);

        }

        [Description("Set lock for the activity.")]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        [TestMethod]
        public void TestBalStoreActivitiesUpdateLock()
        {
            CWF.DataContracts.StoreActivitiesDC request = new CWF.DataContracts.StoreActivitiesDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.Name = "PublishingWorkflow";
            request.Version = "1.0.1.0";
            request.Locked = true;
            request.LockedBy = "v-ery";
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
            request.Environment = UnitTestConstant.TOENVIRONMENT;
            DateTime lockedTime = DateTime.Now;
            CWF.DataContracts.StatusReplyDC result = null;

            try
            {
                result = Services.StoreActivitiesUpdateLock(request, lockedTime);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.Activities.StoreActivitiesGet(request);");
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(SprocValues.REPLY_ERRORCODE_VALUE_OK, result.Errorcode);
        }

        [Description("Set lock for the activity.")]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        [TestMethod]
        public void TestBalStoreActivitiesOverrideLock()
        {
            CWF.DataContracts.StoreActivitiesDC request = new CWF.DataContracts.StoreActivitiesDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.Name = "PublishingWorkflow";
            request.Version = "1.0.1.0";
            request.Locked = true;
            request.LockedBy = "v-ery";
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
            request.Environment = UnitTestConstant.TOENVIRONMENT;
            DateTime lockedTime = DateTime.Now;
            CWF.DataContracts.StatusReplyDC result = null;

            try
            {
                result = Services.StoreActivitiesOverrideLock(request, lockedTime);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.Activities.StoreActivitiesGet(request);");
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(SprocValues.REPLY_ERRORCODE_VALUE_OK, result.Errorcode);
        }

        [Description("Set StoreActivity locked")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void StoreActivitiesUpdateLock()
        {
            CWF.DataContracts.StoreActivitiesDC request = new CWF.DataContracts.StoreActivitiesDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.Name = "PublishingWorkflow";
            request.Version = "1.0.1.0";
            request.Locked = true;
            request.LockedBy = UnitTestConstant.OWNER;
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
            request.Environment = UnitTestConstant.TOENVIRONMENT;
            CWF.DataContracts.StoreActivitiesDC reply = null;

            try
            {
                reply = Activities.StoreActivitiesUpdateLock(request, DateTime.Now);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.Activities.StoreActivityUpdateLock(request);");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }

        [Description("Set StoreActivity locked")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void StoreActivitiesOverrideLock()
        {
            CWF.DataContracts.StoreActivitiesDC request = new CWF.DataContracts.StoreActivitiesDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.Name = "PublishingWorkflow";
            request.Version = "1.0.1.0";
            request.Locked = true;
            request.LockedBy = UnitTestConstant.OWNER;
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
            request.Environment = UnitTestConstant.TOENVIRONMENT;
            CWF.DataContracts.StoreActivitiesDC reply = null;

            try
            {
                reply = Activities.StoreActivitiesOverrideLock(request, DateTime.Now);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.Activities.StoreActivityUpdateLock(request);");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }

        [Description("Searches for activities in the StoreActivities table")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void SearchActivities()
        {
            ActivitySearchRequestDC request = new ActivitySearchRequestDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
            request.Environments = new string[] { "dev", "Test" };

            ActivitySearchReplyDC reply = null;

            try
            {
                reply = ActivityRepositoryService.SearchActivities(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply =  ActivityRepositoryService.SearchActivities(request);");
            }
        }

        [Description("Searches for activities in the StoreActivities table")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void BALSearchActivities()
        {
            ActivitySearchRequestDC request = new ActivitySearchRequestDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
            request.Environments = new string[] { "dev", "Test" };

            ActivitySearchReplyDC reply = null;

            try
            {
                reply = ActivityBusinessService.SearchActivities(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply =  ActivityRepositoryService.SearchActivities(request);");
            }
        }

        [Description("Change Author")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void ChangeAuthor()
        {
            ChangeAuthorRequest request = new ChangeAuthorRequest();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
            request.Environment = UnitTestConstant.TOENVIRONMENT;
            request.Name = "PublishingWorkflow";
            request.Version = "1.0.1.0";
            request.AuthorAlias = "v-bobzh1";
            request.InUpdatedByUserAlias = "v-bobzh";

            ChangeAuthorReply reply = null;

            try
            {
                reply = Activities.ChangeAuthor(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply =  ActivityRepositoryService.SearchActivities(request);");
            }
        }

        [Description("Copy Activity")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void BALActivityCopy()
        {
            ActivityCopyRequest request = GetActivityCopyRequest("dev", "Test");
            StoreActivitiesDC reply = null;

            try
            {
                reply = ActivityBusinessService.ActivityCopy(request);
                Assert.IsTrue(reply.StatusReply.Errorcode == 0);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply =  ActivityRepositoryService.SearchActivities(request);");
            }
        }

        [Description("Delete Activity")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityDelete()
        {
            ActivityCopyRequest requestCopy = GetActivityCopyRequest("dev", "Test");
            var replyCopy = ActivityBusinessService.ActivityCopy(requestCopy);

            StoreActivitiesDC request = new StoreActivitiesDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
            request.Environment = "dev";
            request.Name = requestCopy.Name;
            request.Version = replyCopy.Version;
            request.InInsertedByUserAlias = "v-bobzh";
            request.InUpdatedByUserAlias = "v-bobzh";

            StoreActivitiesDC reply = null;

            try
            {
                reply = Activities.StoreActivitiesDelete(request);
                Assert.IsTrue(reply.StatusReply.Errorcode == 0);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply =  ActivityRepositoryService.SearchActivities(request);");
            }
        }

        [Description("Move Activity")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityMove()
        {
            ActivityCopyRequest requestCopy = GetActivityCopyRequest("dev", "Test");
            var replyCopy = ActivityBusinessService.ActivityCopy(requestCopy);

            ActivityMoveRequest requestMove = new ActivityMoveRequest()
            {
                Incaller = UnitTestConstant.INCALLER,
                IncallerVersion = UnitTestConstant.INCALLERVERSION,
                InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME },
                Environment = "test",
                EnvironmentTarget = "stage",
                Name = requestCopy.Name,
                WorkflowTypeId = GetWorkflowTypeId("stage"),
                Version = replyCopy.Version,
                InInsertedByUserAlias = "v-bobzh",
                InUpdatedByUserAlias = "v-bobzh",
            };

            ActivityMoveReply reply = null;

            try
            {
                reply = Activities.ActivityMove(requestMove);
                Assert.IsTrue(reply.StatusReply.Errorcode == 0);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply =  ActivityRepositoryService.SearchActivities(request);");
            }
        }

        private ActivityCopyRequest GetActivityCopyRequest(string env, string target)
        {
            ActivityCopyRequest request = new ActivityCopyRequest();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
            request.Environment = env;
            request.EnvironmentTarget = target;
            request.WorkflowTypeId = GetWorkflowTypeId(request.EnvironmentTarget);
            request.Name = "PublishingWorkflow";
            request.Version = "1.0.1.0";
            request.InInsertedByUserAlias = "v-bobzh";
            request.InUpdatedByUserAlias = "v-bobzh";

            return request;
        }

        private int GetWorkflowTypeId(string env)
        {
            //get WorkflowType id
            WorkflowTypesGetRequestDC workflowTypeRequest = new WorkflowTypesGetRequestDC();
            workflowTypeRequest.Environment = env;
            workflowTypeRequest.Name = "Workflow";
            var workflowTypeReply = WorkflowTypeRepositoryService.GetWorkflowTypes(workflowTypeRequest);
            return workflowTypeReply.WorkflowActivityType.First().Id;
        }
    }
}
