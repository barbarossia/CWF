using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts;
using System.ServiceModel;

namespace Query_Service.Tests
{
    /// <summary>
    /// A test class that verifies service interaction with mtblActivityContext table
    /// </summary>
    [TestClass]
    public class MtblActivityContextTest : QueryServiceTestBase
    {
        // Eric todo: These tests may be deleted. Steve may delete these sprocs.
        #region Request Objects

        private mtblActivityContextGetRequestDC getRequest;
        private mtblActivityContextCreateOrUpdateRequestDC createOrUpdateRequest;
        private mtblActivityContextByIdDeleteRequestDC deleteRequest;

        #endregion

        #region Reply Objects

        private mtblActivityContextGetReplyDC getReply;
        private mtblActivityContextCreateOrUpdateReplyDC createOrUpdateReply;
        private mtblActivityContextByIdDeleteReplyDC deleteReply;

        #endregion

        [Description("Verify GET FROM mtblActivityContext Table for Valid IDs")]
        [Owner(TEST_OWNER)]
        [TestCategory("Full")]
        [TestMethod]
        public void VerifyGetActivityContextForValidIDs()
        {
            getRequest = new mtblActivityContextGetRequestDC();
            getReply = null;

            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            int[] validIDs = new int[]
            {
                1, 2                   
            };

            foreach (int validID in validIDs)
            {
                getRequest.InId = validID;

                try
                {
                    getReply = proxy.mtblActivityContextGet(getRequest);
                }
                catch (FaultException ex)
                {
                    Assert.Fail("Caught WCF FaultExceptionException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
                }
                catch (Exception e)
                {
                    Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
                }

                Assert.IsNotNull(getReply, "ApplicationsGetReplyDC object null");
                Assert.IsNotNull(getReply.List, "getReply.List is null");
                Assert.AreEqual(1, getReply.List.Count, "Get returned the wrong number of entries.");
                Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
                Assert.AreEqual(getRequest.InId, getReply.List[0].Id, "Service returned the wrong record");
            }
        }

        [Description("Verify GET FROM mtblActivityContext Table for Valid IDs")]
        [Owner(TEST_OWNER)]
        [TestCategory("Full")]
        [TestMethod]
        public void VerifyGetActivityContextForInvalidIDs()
        {
            getRequest = new mtblActivityContextGetRequestDC();
            getReply = null;

            getRequest.Incaller = IN_CALLER;
            getRequest.IncallerVersion = IN_CALLER_VERSION;

            int[] invalidIDs = new int[]
            {
               Int32.MinValue,
               -1, 
               0, 
               100, 
               Int32.MaxValue
            };

            foreach (int invalidID in invalidIDs)
            {
                getRequest.InId = invalidID;

                try
                {
                    getReply = proxy.mtblActivityContextGet(getRequest);
                }
                catch (FaultException ex)
                {
                    Assert.Fail("Caught WCF FaultExceptionException: Message: {0} \n Stack Trace: {1}", ex.Message, ex.StackTrace);
                }
                catch (Exception e)
                {
                    Assert.Fail("Caught Exception Invoking the Service. Message: {0} \n Stack Trace: {1}", e.Message, e.StackTrace);
                }

                int errorConstant = GetErrorConstantInvalidID(invalidID);

                Assert.IsNotNull(getReply, "ApplicationsGetReplyDC object null");
                Assert.IsNotNull(getReply.List, "getReply.List is null");
                Assert.AreEqual(0, getReply.List.Count, "Get returned the wrong number of entries."); 
                Assert.IsNotNull(getReply.StatusReply, "getReply.StatusReply is null");
                Assert.IsNotNull(getReply.StatusReply.ErrorMessage, "Error Message is null"); 
                Assert.IsNotNull(getReply.StatusReply.ErrorGuid, "ErrorGuid is null.");
                Assert.AreEqual(errorConstant, getReply.StatusReply.Errorcode, "Service returned unexpected error code.");
            }
        }


    }
}

