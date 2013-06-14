using System;

namespace Query_Service.Tests.UnitTests
{    
    public static class UnitTestConstant
    {
        /// <summary>
        /// Defines the simulated user error messages to be used when defining the 
        /// configuration isolations using DIF framework.
        /// </summary>
        public class UserErrorMessage
        {
            /// <summary>
            /// Activity category not found message.
            /// </summary>
            public const string ActivityCategoryNotFound = "Unable to update the specified activity category.";

            /// <summary>
            /// Workflow definition by name and version not found message.
            /// </summary>
            public const string WorkflowDefinitionByNameVersionNotFound = "Workflow not found.";

            /// <summary>
            /// Caller name required message.
            /// </summary>
            public const string CallerNameRequired = "Caller name required.";
        }

        /// <summary>
        /// Defines the method names that are simulated using the dynamic implementation framework.
        /// </summary>
        public class SimulationMethodName
        {
            /// <summary>
            /// UserMessageConfigSection.Errors get method.
            /// </summary>
            public const string UserMessageConfigSectionGetErrorsMethodName = "get_Errors";
        }
    }
}
