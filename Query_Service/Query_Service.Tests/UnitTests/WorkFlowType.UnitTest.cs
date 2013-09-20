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
    public class WorkFlowTypeUnitTest
    {
        [Description("Search workflow type")]
        [Owner("v-kason")]
        [TestCategory("Unit")]
        [TestMethod]
        public void WorkflowTypeSearch()
        {
            //Test create a new workflow type
            WorkFlowTypeCreateOrUpdateRequestDC request = new WorkFlowTypeCreateOrUpdateRequestDC();
            WorkFlowTypeCreateOrUpdateReplyDC reply = null;
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.InGuid = Guid.NewGuid();
            request.InName = "TestType_" + request.InGuid.ToString();
            request.InInsertedByUserAlias = UnitTestConstant.INSERTEDBYUSERALIAS;
            request.InAuthGroupId = 2;
            request.Environment = UnitTestConstant.TOENVIRONMENT;
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
            try
            {
                reply = WorkflowTypeBusinessService.WorkflowTypeCreateOrUpdate(request);
                Assert.IsNotNull(reply);
                Assert.AreEqual(reply.StatusReply.Errorcode, 0);

                WorkflowTypeSearchRequest searchRequest = new WorkflowTypeSearchRequest();
                searchRequest.Incaller = UnitTestConstant.INCALLER;
                searchRequest.IncallerVersion = UnitTestConstant.INCALLERVERSION;
                searchRequest.SearchText = "TestType";
                searchRequest.PageSize = 10;
                searchRequest.PageNumber = 1;
                searchRequest.Environments = new string[] { UnitTestConstant.TOENVIRONMENT };
                searchRequest.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
                WorkflowTypeSearchReply searchReply = WorkflowTypeBusinessService.SearchWorkflowTypes(searchRequest);

                Assert.IsNotNull(searchReply);
                Assert.AreEqual(searchReply.StatusReply.Errorcode, 0);
                Assert.IsTrue(searchReply.ServerResultsLength >= 1);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.BAL.WorkflowTypeBusinessService.WorkflowTypeCreateOrUpdate();");
            }

        }

        [Description("Get all workflow types")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void WorkFlowTypesGet()
        {
            CWF.DataContracts.WorkflowTypeGetReplyDC reply = null;

            try
            {
                WorkflowTypesGetRequestDC request = new WorkflowTypesGetRequestDC();
                request.Environment = UnitTestConstant.TOENVIRONMENT;
                reply = WorkflowTypeRepositoryService.GetWorkflowTypes(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.WorkflowTypesGet();");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, 0);
        }

        [Description("Get all workflow types")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void BALWorkFlowTypesGet()
        {
            CWF.DataContracts.WorkflowTypeGetReplyDC reply = null;

            try
            {
                WorkflowTypesGetRequestDC request = new WorkflowTypesGetRequestDC();
                request.Environment = UnitTestConstant.TOENVIRONMENT;
                reply = WorkflowTypeBusinessService.GetWorkflowTypes(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.WorkflowTypesGet();");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, 0);
        }
    }
}
