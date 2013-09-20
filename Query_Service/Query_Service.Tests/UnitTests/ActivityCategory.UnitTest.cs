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
    public class ActivityCategoryUnitTest
    {
        [Description("Get a row from the activityCategory table")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityCategoryGet()
        {
            List<CWF.DataContracts.ActivityCategoryByNameGetReplyDC> reply = null;
            try
            {
                CWF.DataContracts.ActivityCategoryByNameGetRequestDC request = new CWF.DataContracts.ActivityCategoryByNameGetRequestDC();
                request.Incaller = UnitTestConstant.INCALLER;
                request.IncallerVersion = "1.0.0.0";
                request.InName = "OAS Basic Controls";

                try
                {
                    reply = ActivityCategoryRepositoryService.GetActivityCategories(request);
                }
                catch (Exception ex)
                {
                    string faultMessage = ex.Message;
                    Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.ActivityCategoryGet(request);");
                }
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply[0].StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }


        [Description("Get a row from the activityCategory table")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void BALActivityCategoryGet()
        {
            List<CWF.DataContracts.ActivityCategoryByNameGetReplyDC> reply = null;
            try
            {
                CWF.DataContracts.ActivityCategoryByNameGetRequestDC request = new CWF.DataContracts.ActivityCategoryByNameGetRequestDC();
                request.Incaller = UnitTestConstant.INCALLER;
                request.IncallerVersion = "1.0.0.0";
                request.InName = "OAS Basic Controls";

                try
                {
                    reply = ActivityCategoryBusinessService.GetActivityCategories(request);
                }
                catch (Exception ex)
                {
                    string faultMessage = ex.Message;
                    Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.ActivityCategoryGet(request);");
                }
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply[0].StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }

        [Description("Create or update a row in the activityCategory table")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityCategoryCreateOrUpdate()
        {
            CWF.DataContracts.ActivityCategoryCreateOrUpdateRequestDC request = new CWF.DataContracts.ActivityCategoryCreateOrUpdateRequestDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.InDescription = Utility.GenerateRandomString(250);
            request.InGuid = Guid.NewGuid();
            request.InId = 0;
            request.InMetaTags = "Meta Data";
            request.InAuthGroupName = "pqocwfadmin";
            request.InName = "TESTHarness100" + Guid.NewGuid();
            request.InUpdatedByUserAlias = UnitTestConstant.UPDATEDBYUSERALIAS;
            request.InInsertedByUserAlias = UnitTestConstant.INSERTEDBYUSERALIAS;
            CWF.DataContracts.ActivityCategoryCreateOrUpdateReplyDC reply = null;

            try
            {
                reply = ActivityCategory.ActivityCategoryCreateOrUpdate(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.ActivityCategoryCreateOrUpdate(request);");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(SprocValues.REPLY_ERRORCODE_VALUE_OK, reply.StatusReply.Errorcode);
        }

        [Description("Get Applications")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ApplicationsGet()
        {
            CWF.DataContracts.ApplicationsGetRequestDC request = new CWF.DataContracts.ApplicationsGetRequestDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.InId = 0;
            request.InName = string.Empty;

            CWF.DataContracts.ApplicationsGetReplyDC reply = null;

            try
            {
                reply = Applications.ApplicationsGet(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.Applications.ApplicationsGet(request);");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }

        [Description("Get StatusCode")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void StatusCodeGet()
        {
            CWF.DataContracts.StatusCodeGetRequestDC request = new CWF.DataContracts.StatusCodeGetRequestDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;

            CWF.DataContracts.StatusCodeGetReplyDC reply = null;

            try
            {
                reply = StatusCode.StatusCodeGet(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.StatusCode.StatusCodeGet(request);");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }
    }
}
