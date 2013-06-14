using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Service.Common.Logging.Config;
using CWF.WorkflowQueryService.UserError.Config;
using Microsoft.Support.Workflow.QueryService.Common;
using Query_Service.Tests.UnitTests;
using Microsoft.Support.Workflow.Service.Test.Common;

namespace Microsoft.Support.Workflow.Service.Common.Tests
{
    /// <summary>
    /// Defines dynamic implementations to isolate the reading of UserError.config and to dynamically
    /// provide different configuration values to verify the behavior of code that reads from this configuration.
    /// </summary>
    internal class UserErrorConfigIsolator : Implementation, IDisposable
    {
        internal static InstanceMethodCallIsolator<UserErrorConfigSection> GetValidConfiguration()
        {
            return new InstanceMethodCallIsolator<UserErrorConfigSection>(UnitTestConstant.SimulationMethodName.UserMessageConfigSectionGetErrorsMethodName,
                           delegate
                           {
                               // Simulate valid non-empty configuration.
                               UserErrorCollection collection = new UserErrorCollection();
                               collection[EventCode.DatabaseEvent.Validation.ActivityCategoryNotFound] = new UserErrorElement
                               {
                                   ErrorCode = EventCode.DatabaseEvent.Validation.ActivityCategoryNotFound,
                                   UserMessage = UnitTestConstant.UserErrorMessage.ActivityCategoryNotFound,
                                   FaultType = "ServiceFault"
                               };
                               collection[EventCode.BusinessLayerEvent.Error.WorkflowDefinitionByNameVersionNotFound] = new UserErrorElement
                               {
                                   ErrorCode = EventCode.BusinessLayerEvent.Error.WorkflowDefinitionByNameVersionNotFound,
                                   UserMessage = UnitTestConstant.UserErrorMessage.WorkflowDefinitionByNameVersionNotFound,
                                   FaultType = "PublishingFault"
                               };
                               collection[EventCode.BusinessLayerEvent.Validation.CallerNameRequired] = new UserErrorElement
                               {
                                   ErrorCode = EventCode.BusinessLayerEvent.Validation.CallerNameRequired,
                                   UserMessage = UnitTestConstant.UserErrorMessage.CallerNameRequired,
                                   FaultType = "ValidationFault"
                               };
                               return collection;
                           });
        }

        internal static InstanceMethodCallIsolator<UserErrorConfigSection> GetEmptyConfiguration()
        {
            return new InstanceMethodCallIsolator<UserErrorConfigSection>(UnitTestConstant.SimulationMethodName.UserMessageConfigSectionGetErrorsMethodName,
                          delegate
                          {
                              UserErrorCollection collection = new UserErrorCollection();
                              return collection;
                          });
        }

        internal static InstanceMethodCallIsolator<UserErrorConfigSection> GetValidConfigurationWithMixedCaseFaultType()
        {
            return new InstanceMethodCallIsolator<UserErrorConfigSection>(UnitTestConstant.SimulationMethodName.UserMessageConfigSectionGetErrorsMethodName,
                          delegate
                          {
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
                          });
        }
    }
}
