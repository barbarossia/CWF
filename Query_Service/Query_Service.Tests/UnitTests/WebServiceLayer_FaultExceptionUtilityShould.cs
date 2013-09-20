using CWF.WorkflowQueryService;
using CWF.WorkflowQueryService.UserError.Config;
using Microsoft.Support.Workflow.QueryService.Common;
using Microsoft.Support.Workflow.Service.Common.Tests;
using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;
using Microsoft.Support.Workflow.Service.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;

namespace Query_Service.UnitTests
{
    /// <summary>
    /// Defines the unit tests for FaultExceptionUtility class.
    /// </summary>
    [TestClass]
    public class WebServiceLayer_FaultExceptionUtilityShould
    {
        private TestContext testContextInstance;
        
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

        [Description("Verify fault exception type configured in UserErrorMessage.config is returned when GetFaultException is called.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnConfiguredServiceFaultWhenGetFaultExceptionIsCalled()
        {
            using (UserErrorConfigIsolator.GetValidConfiguration())
            {
                FaultException exception = FaultExceptionUtility.GetFaultException(EventCode.DatabaseEvent.Validation.AuthGroupNotFound);
                Assert.IsTrue(exception is FaultException<ServiceFault>);

                exception = FaultExceptionUtility.GetFaultException(EventCode.BusinessLayerEvent.Validation.CallerNameRequired);
                Assert.IsTrue(exception is FaultException<ValidationFault>);

                exception = FaultExceptionUtility.GetFaultException(EventCode.BusinessLayerEvent.Error.WorkflowDefinitionByNameVersionNotFound);
                Assert.IsTrue(exception is FaultException<PublishingFault>);
            }
        }


        [Description("Verify fault exception type configured in UserErrorMessage.config is returned irrespective of the case of the value characters when GetFaultException is called.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnConfiguredServiceFaultWithMixedCaseConfigValueWhenGetFaultExceptionIsCalled()
        {
            using (UserErrorConfigIsolator.GetValidConfigurationWithMixedCaseFaultType())
            {
                FaultException exception = FaultExceptionUtility.GetFaultException(EventCode.BusinessLayerEvent.Validation.CallerNameRequired);
                Assert.IsTrue(exception is FaultException<ValidationFault>);
            }
        }

        [Description("Verify general service fault exception is returned when the fault type is not properly configured in UserErrorMessage.config.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnGeneralServiceIfConfiguredFaultTypeIsInvalidWhenGetFaultExceptionIsCalled()
        {
            using (new InstanceMethodCallIsolator<UserErrorConfigSection>(UnitTestConstant.SimulationMethodName.UserMessageConfigSectionGetErrorsMethodName,
                          delegate
                          {
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
                FaultException exception = FaultExceptionUtility.GetFaultException(EventCode.DatabaseEvent.Validation.ActivityCategoryNotFound);
                Assert.IsTrue(exception is FaultException<ServiceFault>);
                FaultException<ServiceFault> faultException = exception as FaultException<ServiceFault>;
                Assert.AreEqual(UnitTestConstant.UserErrorMessage.ActivityCategoryNotFound, faultException.Detail.ErrorMessage);
                Assert.AreEqual(EventCode.DatabaseEvent.Validation.ActivityCategoryNotFound, faultException.Detail.ErrorCode);
            }
        }

        [Description("Verify general service fault exception is returned when the error code configuration is not given UserErrorMessage.config.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnGeneralFaultIfUserErrorIsNotConfiguredWhenGetFaultExceptionIsCalled()
        {
            using (UserErrorConfigIsolator.GetValidConfiguration())
            {
                FaultException exception = FaultExceptionUtility.GetFaultException(-20);
                Assert.IsTrue(exception is FaultException<ServiceFault>);
                FaultException<ServiceFault> faultException = exception as FaultException<ServiceFault>;
                Assert.AreEqual(UserErrorInfo.DefaultMessage, faultException.Detail.ErrorMessage);
                Assert.AreEqual(-20, faultException.Detail.ErrorCode);
            }

            using (UserErrorConfigIsolator.GetEmptyConfiguration())
            {
                FaultException exception = FaultExceptionUtility.GetFaultException(EventCode.DatabaseEvent.Validation.ActivityCategoryNotFound);
                Assert.IsTrue(exception is FaultException<ServiceFault>);
                FaultException<ServiceFault> faultException = exception as FaultException<ServiceFault>;
                Assert.AreEqual(UserErrorInfo.DefaultMessage, faultException.Detail.ErrorMessage);
                Assert.AreEqual(EventCode.DatabaseEvent.Validation.ActivityCategoryNotFound, faultException.Detail.ErrorCode);
            }
        }    
    }
}
