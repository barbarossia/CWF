using System;

namespace Query_Service.UnitTests
{    
    public static class UnitTestConstant
    {
        public const string INCALLER = "v-stska";
        public const string INCALLERVERSION = "1.0.0.0";
        public const string OWNER = "v-stska";
        public const string UPDATEDBYUSERALIAS = "v-stska";
        public const string INSERTEDBYUSERALIAS = "v-stska";
        public const string TOENVIRONMENT = "TEST";
        public const string AUTHORGROUPNAME = "pqocwfadmin";

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
