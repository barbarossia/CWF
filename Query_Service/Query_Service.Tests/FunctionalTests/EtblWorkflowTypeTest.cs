//-----------------------------------------------------------------------
// <copyright file="EtblWorkflowTypeTest.cs" company="Microsoft">
// Copyright
// A test class used to verify service interaction with etblWorkflowType table.
// </copyright>
//-----------------------------------------------------------------------

namespace Query_Service.Tests
{
    using System;
    using System.ServiceModel;
    using CWF.DataContracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Query_Service.Tests.Common;

    /// <summary>
    /// A test class used to verify service interaction with etblWorkflowType table
    /// </summary>
    [TestClass]
    public class EtblWorkflowTypeTest : QueryServiceTestBase
    {
        private const string TABLE_NAME = "etblWorkflowType";
        private const string IN_AUTH_GROUP_NAME = "csswcf";
        private const string IN_CONTEXT_VARIABLE = "InContextVariable";
        private const string IN_HANDLE_VARIABLE = "InHandleVariable";
        private const string IN_NAME = "Contexttype#420b";
        private const string IN_PAGE_VIEW_VARIABLE = "InPageViewVariable";
        private const string IN_PUBLISHING_WORKFLOW = "InPublishingWorkflow";
        private const string IN_SELECTION_WORKFLOW = "InSelectionWorkflow";
        private const string IN_WORKFLOW_GROUP = "InWorkflowGroup";
        private const string IN_WORKFLOW_TEMPLATE = "InWorkflowTemplate";
        private const string IN_WORKFLOW_TYPEID = "99";
        private const string WORKFLOW_TYPE = "OAS Page";
        private const string VERSION = "1.0.0.0";
        private const string WORKFLOWNAME = "TEST#101";
        private const string WORKFLOWNAME2 = "TEST#102";
        private const string WORKFLOWNAME3 = "TestMonu1";
        private const string DEPLOY_PATH = @"\\pqocwfddb01\PocPublishing";
        private const string WORKFLOWTYPE_NAME = "OAS Page";
        private const string INACCESSIBLE_DEPLOY_PATH = @"\\pqocwfddb01\D\TestDeploy";

        private WorkflowTypeGetReplyDC getReply;

        /// <summary>
        /// Verify the WCF call WorkflowTypeGet
        /// </summary>
        private void WorkflowGet()
        {
            getReply = null;

            try
            {
                getReply = devBranchProxy.WorkflowTypeGet();
            }
            catch (FaultException ex)
            {
                Assert.Fail("Caught WCF FaultExceptionException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
            }
            catch (Exception e)
            {
                Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
            }

            Assert.IsNotNull(getReply, "WorkflowTypeGetReplyDC object null");
            Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
            Assert.AreEqual(0, getReply.StatusReply.Errorcode, "getFromNameAndVersionReply.StatusReply. Errorcode is not 0. Instead it is {0}.", getReply.StatusReply.Errorcode);
            Assert.IsNotNull(getReply.WorkflowActivityType);
        }

        [WorkItem(22205)]
        [Description("Verify GET FROM etblWorkflowType Table")]
        [Owner(TEST_OWNER)]
        [TestCategory(TestCategory.Func)]
        [TestMethod]
        public void VerifyWorkflowGet()
        {
            WorkflowGet();
        }
    }
}
