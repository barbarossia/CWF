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
{    /// <summary>
    /// Unit tests for QueryService BAl and DAL layer
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "This not required")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Not required fot const/unit tests")]
    [TestClass]
    public class PermissionUnitTest
    {
        [Description("Get The entire Permission table")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void PermissionGet()
        {
            CWF.DataContracts.RequestHeader request = new CWF.DataContracts.RequestHeader();
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            PermissionGetListReply reply = null;

            try
            {
                reply = Permission.PermissionGet(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.Activities.StoreActivitiesGet(request);");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }
    }
}
