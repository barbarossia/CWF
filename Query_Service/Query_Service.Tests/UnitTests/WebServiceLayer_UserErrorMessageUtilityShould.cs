using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.WorkflowQueryService;
using System.IO;
using System.Configuration;
using CWF.WorkflowQueryService.UserError.Config;
using Microsoft.Support.Workflow.QueryService.Common;
using Microsoft.Support.Workflow.Service.Common.Tests;
using Microsoft.Support.Workflow.Service.Test.Common;

namespace Query_Service.Tests.UnitTests
{
    /// <summary>
    /// Before running these tests, copy the Config folder and its contents from this project to C:\Temp.
    /// This is where the test config file get deployed.
    /// </summary>
    [TestClass]
    public class WebServiceLayer_UserErrorMessageUtilityShould
    {
        private TestContext testContextInstance;
        private const int UndefinedErrorCode = -30;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        /// Used to initalize Variation test excution object
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Utility.CopyTestConfigs(Utility.TempLocation);
            Utility.CopyTestConfigs(testContextInstance.TestDeploymentDir);
        }
               

        [Description("Verify whether the error message defined in the UserErrorMessage.config is returned when GetUserErrorInfo is called.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void GetErrorMessageForDefinedErrorCodeWhenGetUserErrorInfoIsCalled()
        {
            using (UserErrorConfigIsolator.GetValidConfiguration())
            {
                UserErrorInfo errorInfo = UserErrorInfo.GetUserErrorInfo(EventCode.DatabaseEvent.Validation.ActivityCategoryNotFound);
                Assert.AreEqual(UnitTestConstant.UserErrorMessage.ActivityCategoryNotFound, errorInfo.Message);
            }
        }

        [Description("Verify whether the general error message is returned for error code undefined in UserErrorMessage.config file when GetUserErrorInfo is called.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void GetDefaultErrorMessageForUndefinedErrorCodeWhenGetUserErrorInfoIsCalled()
        {
            using (UserErrorConfigIsolator.GetValidConfiguration())
            {
                UserErrorInfo errorInfo = UserErrorInfo.GetUserErrorInfo(UndefinedErrorCode);
                Assert.AreEqual(UserErrorInfo.DefaultMessage, errorInfo.Message);
            }

            using (UserErrorConfigIsolator.GetEmptyConfiguration())
            {
                UserErrorInfo errorInfo = UserErrorInfo.GetUserErrorInfo(EventCode.DatabaseEvent.Validation.ActivityCategoryNotFound);
                Assert.AreEqual(UserErrorInfo.DefaultMessage, errorInfo.Message);
            }
        }

        [Description("Verify whether the fault type defined in the UserErrorMessage.config is returned when GetUserErrorInfo is called.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void GetFaultTypeForDefinedErrorCodeWhenGetUserErrorInfoIsCalled()
        {
            using (UserErrorConfigIsolator.GetValidConfiguration())
            {
                UserErrorInfo errorInfo = UserErrorInfo.GetUserErrorInfo(EventCode.DatabaseEvent.Validation.ActivityCategoryNotFound);
                Assert.AreEqual(FaultType.ServiceFault, errorInfo.FaultType);

                errorInfo = UserErrorInfo.GetUserErrorInfo(EventCode.BusinessLayerEvent.Error.WorkflowDefinitionByNameVersionNotFound);
                Assert.AreEqual(FaultType.PublishingFault, errorInfo.FaultType);

                errorInfo = UserErrorInfo.GetUserErrorInfo(EventCode.BusinessLayerEvent.Validation.CallerNameRequired);
                Assert.AreEqual(FaultType.ValidationFault, errorInfo.FaultType);
            }
        }

        [Description("Verify whether the default fault type is returned error code is not defined in UserErrorMessage.config.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void GetDefaultFaultTypeForUndefinedErrorCodeWhenGetUserErrorInfoIsCalled()
        {
            using (UserErrorConfigIsolator.GetValidConfiguration())
            {
                UserErrorInfo errorInfo = UserErrorInfo.GetUserErrorInfo(UndefinedErrorCode);
                Assert.AreEqual(FaultType.ServiceFault, errorInfo.FaultType);
            }
        }

        [Description("Verify whether the default fault type is returned if fault type defined in UserErrorMessage.config is invalid.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void GetDefaultFaultTypeForDefinedErrorCodeWithIncorrectFaultTypeWhenGetUserErrorInfoIsCalled()
        {
            using (new InstanceMethodCallIsolator<UserErrorConfigSection>(UnitTestConstant.SimulationMethodName.UserMessageConfigSectionGetErrorsMethodName,
                delegate
                {
                    // Simulate non-empty configuration with invalid fault type value.
                    UserErrorCollection collection = new UserErrorCollection();
                    collection[EventCode.DatabaseEvent.Validation.ActivityCategoryNotFound] = new UserErrorElement
                    {
                        ErrorCode = EventCode.DatabaseEvent.Validation.ActivityCategoryNotFound,
                        UserMessage = "Unable to update the specified activity category.",
                        FaultType = "UnknownFault"
                    };
                    collection[EventCode.BusinessLayerEvent.Error.WorkflowDefinitionByNameVersionNotFound] = new UserErrorElement
                    {
                        ErrorCode = EventCode.BusinessLayerEvent.Error.WorkflowDefinitionByNameVersionNotFound,
                        UserMessage = "Workflow not found.",
                        FaultType = "InvalidFault"
                    };
                    collection[EventCode.BusinessLayerEvent.Validation.CallerNameRequired] = new UserErrorElement
                    {
                        ErrorCode = EventCode.BusinessLayerEvent.Validation.CallerNameRequired,
                        UserMessage = "Caller name required.",
                        FaultType = "JunkFault"
                    };
                    return collection;
                }))
                {
                UserErrorInfo errorInfo = UserErrorInfo.GetUserErrorInfo(EventCode.BusinessLayerEvent.Error.WorkflowDefinitionByNameVersionNotFound);
                Assert.AreEqual(FaultType.ServiceFault, errorInfo.FaultType);

                errorInfo = UserErrorInfo.GetUserErrorInfo(EventCode.BusinessLayerEvent.Validation.CallerNameRequired);
                Assert.AreEqual(FaultType.ServiceFault, errorInfo.FaultType);
            }
        }

        [Description("Verify whether valid fault type is detected when it is in mixed case letters.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void GetMatchingFaultTypeIfConfiguredInMixedCaseCharsWhenGetUserErrorInfoIsCalled()
        {
            using (new InstanceMethodCallIsolator<UserErrorConfigSection>(UnitTestConstant.SimulationMethodName.UserMessageConfigSectionGetErrorsMethodName,
                           delegate
                           {
                               // Simulate empty configuration with fault type value in mixed case (inconsistent upper/lower casing).
                               UserErrorCollection collection = new UserErrorCollection();
                               collection[EventCode.DatabaseEvent.Validation.ActivityCategoryNotFound] = new UserErrorElement
                               {
                                   ErrorCode = EventCode.DatabaseEvent.Validation.ActivityCategoryNotFound,
                                   UserMessage = "Unable to update the specified activity category.",
                                   FaultType = "servicefault"
                               };
                               collection[EventCode.BusinessLayerEvent.Error.WorkflowDefinitionByNameVersionNotFound] = new UserErrorElement
                               {
                                   ErrorCode = EventCode.BusinessLayerEvent.Error.WorkflowDefinitionByNameVersionNotFound,
                                   UserMessage = "Workflow not found.",
                                   FaultType = "pUbliShingfAuLt"
                               };
                               collection[EventCode.BusinessLayerEvent.Validation.CallerNameRequired] = new UserErrorElement
                               {
                                   ErrorCode = EventCode.BusinessLayerEvent.Validation.CallerNameRequired,
                                   UserMessage = "Caller name required.",
                                   FaultType = "VALIDATIONFAULT"
                               };
                               return collection;
                           }))            
            {
                UserErrorInfo errorInfo = UserErrorInfo.GetUserErrorInfo(EventCode.DatabaseEvent.Validation.ActivityCategoryNotFound);
                Assert.AreEqual(FaultType.ServiceFault, errorInfo.FaultType);

                errorInfo = UserErrorInfo.GetUserErrorInfo(EventCode.BusinessLayerEvent.Error.WorkflowDefinitionByNameVersionNotFound);
                Assert.AreEqual(FaultType.PublishingFault, errorInfo.FaultType);

                errorInfo = UserErrorInfo.GetUserErrorInfo(EventCode.BusinessLayerEvent.Validation.CallerNameRequired);
                Assert.AreEqual(FaultType.ValidationFault, errorInfo.FaultType);
            }
        }
    }
}
