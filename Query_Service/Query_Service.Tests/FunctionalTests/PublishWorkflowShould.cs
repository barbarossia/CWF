using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Support.Workflow.Service.DataAccessServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts;
using Query_Service.Tests.Common;

namespace Query_Service.Tests.FunctionalTests
{

    [TestClass]
    public class PublishWorkflowShould
    {
        private const string ALIAS = "v-billmi";
        private static string VERSION = "1.0.0.0";
        private static string BAD_VERSION = "2.0.0.0";
        private static string WORKFLOWNAME = "manPublishtest";
        private static string BAD_WORKFLOWNAME = "manPublishtest1";

        WorkflowsQueryServiceClient client;
        PublishingRequest pubReq;
        PublishingReply pubReply;
               
        [WorkItem(97609)]
        [Description("Verify that ")]
        [Owner(ALIAS)]
        [TestCategory(TestCategory.Func)]
        [TestMethod]
        public void ReturnErrorPublishAWorkflowForValidWorkflowServiceNameAndInvalidVersion()
        {
            client = new WorkflowsQueryServiceClient();

            pubReq = new PublishingRequest();
            pubReq.Incaller = ALIAS;
            pubReq.IncallerVersion = VERSION;
            pubReq.WorkflowName = WORKFLOWNAME;
            pubReq.WorkflowVersion = BAD_VERSION;

            pubReply = client.PublishWorkflow(pubReq);

            Assert.IsNotNull(pubReply, "pubReply is Null");
            Assert.AreEqual(SprocValues.GENERIC_CATCH_ID, pubReply.StatusReply.Errorcode, string.Format("Errorcode: {0}", pubReply.StatusReply.Errorcode));
            Assert.AreNotEqual(string.Empty, pubReply.StatusReply.ErrorMessage, string.Format("ErrorMessage: {0}", pubReply.StatusReply.ErrorMessage));

            Console.WriteLine();
        }

        [WorkItem(97610)]
        [Description("Verify that ")]
        [Owner(ALIAS)]
        [TestCategory(TestCategory.Func)]
        [TestMethod]
        public void ReturnErrorPublishAWorkflowForInvalidWorkflowServiceNameAndValidVersion()
        {
            client = new WorkflowsQueryServiceClient();

            pubReq = new PublishingRequest();
            pubReq.Incaller = ALIAS;
            pubReq.IncallerVersion = VERSION;
            pubReq.WorkflowName = BAD_WORKFLOWNAME;
            pubReq.WorkflowVersion = VERSION;

            pubReply = client.PublishWorkflow(pubReq);

            Assert.IsNotNull(pubReply, "pubReply is Null");
            Assert.AreEqual(SprocValues.GENERIC_CATCH_ID, pubReply.StatusReply.Errorcode, string.Format("Errorcode: {0}", pubReply.StatusReply.Errorcode));
            Assert.AreNotEqual(string.Empty, pubReply.StatusReply.ErrorMessage, string.Format("ErrorMessage: {0}", pubReply.StatusReply.ErrorMessage));

            Console.WriteLine();
        }
    }
}
