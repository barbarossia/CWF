using CWF.DataContracts;
using Microsoft.Support.Workflow.Service.BusinessServices;
using Microsoft.Support.Workflow.Service.DataAccessServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Query_Service.UnitTests
{
    /// <summary>
    /// Unit tests for QueryService BAl and DAL layer
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "This not required")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Not required fot const/unit tests")]
    [TestClass]
    public class AuthorizationGroupUnitTest
    {
        [Description("Get The entire AuthorizationGroup table")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void AuthorizationGroupGet()
        {
            CWF.DataContracts.AuthorizationGroupGetRequestDC request = new CWF.DataContracts.AuthorizationGroupGetRequestDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            AuthorizationGroupGetReplyDC reply = null;

            try
            {
                reply = AuthorizationGroup.GetAuthorizationGroups(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }

        [Description("Get The entire AuthorizationGroup table")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void BALAuthorizationGroupGet()
        {
            CWF.DataContracts.AuthorizationGroupGetRequestDC request = new CWF.DataContracts.AuthorizationGroupGetRequestDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            AuthorizationGroupGetReplyDC reply = null;

            try
            {
                reply = AuthorizationGroupBusinessService.GetAuthorizationGroups(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }

        [Description("Create The entire AuthorizationGroup table")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void AuthorizationGroupCreateOrUpdate()
        {
            CWF.DataContracts.AuthGroupsCreateOrUpdateRequestDC request = GetAuthGroupsCreateRequest(Guid.NewGuid().ToString());
            AuthGroupsCreateOrUpdateReplyDC reply = null;

            try
            {
                reply = AuthorizationGroup.AuthGroupsCreateOrUpdate(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }

        private AuthGroupsCreateOrUpdateRequestDC GetAuthGroupsCreateRequest(string name)
        {
            TestDataAccessUtility.CreateAuthoriztionGroup("cwfAdmin", 5);
            CWF.DataContracts.AuthGroupsCreateOrUpdateRequestDC request = new CWF.DataContracts.AuthGroupsCreateOrUpdateRequestDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.InAuthGroupNames = new string[] { "cwfAdmin" };
            request.InAuthGroups = new string[] { name };
            request.RoleId = 1;
            request.InInsertedByUserAlias = UnitTestConstant.OWNER;
            request.InUpdatedByUserAlias = UnitTestConstant.OWNER;

            return request;
        }

        [Description("Enable The entire AuthorizationGroup table")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void AuthorizationGroupEnable()
        {
            string name = "Test_" + Guid.NewGuid().ToString();
            CWF.DataContracts.AuthGroupsCreateOrUpdateRequestDC createDC = GetAuthGroupsCreateRequest(name);
            AuthorizationGroup.AuthGroupsCreateOrUpdate(createDC);

            TestDataAccessUtility.CreateAuthoriztionGroup("cwfDevAdmin", 4);
            CWF.DataContracts.AuthGroupsEnableOrDisableRequestDC request = new CWF.DataContracts.AuthGroupsEnableOrDisableRequestDC();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.InAuthGroupNames = new string[] { "cwfDevAdmin" };
            request.InAuthGroups = new string[] { name };
            request.InEnabled = true;
            request.InInsertedByUserAlias = UnitTestConstant.OWNER;
            request.InUpdatedByUserAlias = UnitTestConstant.OWNER;

            AuthGroupsEnableOrDisableReplyDC reply = null;

            try
            {
                reply = AuthorizationGroup.AuthGroupsEnableOrDisable(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }
    }
}
