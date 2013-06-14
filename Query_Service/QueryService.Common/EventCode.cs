using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.QueryService.Common
{
    /// <summary>
    /// Defines all the event codes associated with this application.  
    /// </summary>
    public static class EventCode
    {
        /// <summary>
        /// Defines database event codes.
        /// </summary>
        public static class DatabaseEvent
        {
            /// <summary>
            /// Defines database error event codes.
            /// </summary>
            public static class Error
            {
                /// <summary>
                /// The default error number returned when a database stored proc executes RAISERROR.
                /// </summary>
                public const int DefaultSqlError = 50000;
            }

            /// <summary>
            /// Defines database validation event codes.
            /// </summary>
            public static class Validation
            {
                /// <summary>
                /// Activity category to update was not found in the database.
                /// </summary>
                public const int ActivityCategoryNotFound = 50500;

                /// <summary>
                /// Auth group to update was not found in the database.
                /// </summary>
                public const int AuthGroupNotFound = 50501;

                /// <summary>
                /// Workflow type to update was not found in the database.
                /// </summary>
                public const int WorkflowTypeNotFound = 50502;

                /// <summary>
                /// Status code to update was not found in the database.
                /// </summary>
                public const int StatusCodeNotFound = 50503;

                /// <summary>
                /// Icon to update was not found in the database.
                /// </summary>
                public const int IconNotFound = 50504;

                /// <summary>
                /// Store activity to update was not found in the database.
                /// </summary>
                public const int StoreActivityNotFound = 50505;

                /// <summary>
                /// Toolbox tab to update was not found in the database.
                /// </summary>
                public const int ToolboxTabNotFound = 50506;

                /// <summary>
                /// Activity library to update was not found in the database.
                /// </summary>
                public const int ActivityLibraryNotFound = 50507;
            }
        }

        /// <summary>
        /// Defines business layer event codes.
        /// </summary>
        public static class BusinessLayerEvent
        {
            /// <summary>
            /// Defines business layer error event codes.
            /// </summary>
            public static class Error
            {
                /// <summary>
                /// Workflow definition was not found by workflow name and version.
                /// </summary>
                public const int WorkflowDefinitionByNameVersionNotFound = 52000;

                /// <summary>
                /// Workflow XAML was not found by activity name and version.
                /// </summary>
                public const int WorkflowXamlByActivityNameVersionNotFound = 52001;

                /// <summary>
                /// Associated publishing workflow was not found by workflow name and version.
                /// </summary>
                public const int PublishingWorkflowXamlByNameVersionNotFound = 52002;
            }

            /// <summary>
            /// Defines business layer validation event codes.
            /// </summary>
            public static class Validation
            {
                /// <summary>
                /// Request data contract object is null.
                /// </summary>
                public const int RequestIsNull = 52500;

                /// <summary>
                /// Caller name is null or empty.
                /// </summary>
                public const int CallerNameRequired = 52501;

                /// <summary>
                /// Caller version is null or empty.
                /// </summary>
                public const int CallerVersionRequired = 52502;

                /// <summary>
                /// Activity category ID is invalid (negative).
                /// </summary>
                public const int ActivityCategoryIdNegative = 52520;

                /// <summary>
                /// Activity category GUID is null.
                /// </summary>
                public const int ActivityCategoryGuidRequired = 52521;

                /// <summary>
                /// Activity category inserted by is null or empty.
                /// </summary>
                public const int ActivityCategoryInsertedByRequired = 52522;

                /// <summary>
                /// Activity category description is null or empty.
                /// </summary>
                public const int ActivityCategoryDescriptionRequired = 52523;
                
                /// <summary>
                /// Activity category name is null or empty.
                /// </summary>
                public const int ActivityCategoryNameRequired = 52524;

                /// <summary>
                /// Activity category meta tags value is null or empty.
                /// </summary>
                public const int ActivityCategoryMetaTagsRequired = 52525;

                /// <summary>
                /// Activity category auth group name is null or empty.
                /// </summary>
                public const int ActivityCategoryAuthGroupNameRequired = 52526;

                /// <summary>
                /// Activity category updated by is null or empty.
                /// </summary>
                public const int ActivityCategoryUpdatedByRequired = 52527;

                /// <summary>
                /// Store activity name is null or empty.
                /// </summary>
                public const int StoreActivityNameRequired = 52540;

                /// <summary>
                /// Store activity GUID is null.
                /// </summary>
                public const int StoreActivityGuidRequired = 52541;
                
                /// <summary>
                /// Store activity updated by is null or empty.
                /// </summary>
                public const int StoreActivityUpdatedByRequired = 52542;

                /// <summary>
                /// Store activity short name is null or empty.
                /// </summary>
                public const int StoreActivityShortNameRequired = 52543;

                /// <summary>
                /// Store activity description is null or empty.
                /// </summary>
                public const int StoreActivityDescriptionRequired = 52544;

                /// <summary>
                /// Store activity meta tags value is null or empty.
                /// </summary>
                public const int StoreActivityMetaTagsRequired = 52545;

                /// <summary>
                /// Store activity 'is switch' value is null.
                /// </summary>
                public const int StoreActivityIsSwitchRequired = 52546;

                /// <summary>
                /// Store activity 'is service' value is null.
                /// </summary>
                public const int StoreActivityIsServiceRequired = 52547;

                /// <summary>
                /// Store activity 'is UX activity' value is null.
                /// </summary>
                public const int StoreActivityIsUXActivityRequired = 52548;

                /// <summary>
                /// Store activity category is null or empty.
                /// </summary>
                public const int StoreActivityCategoryRequired = 52550;

                /// <summary>
                /// Store activity 'is toolbox activity' value is null.
                /// </summary>
                public const int StoreActivityIsToolboxActivityRequired = 52552;

                /// <summary>
                /// Store activity version is null or empty.
                /// </summary>
                public const int StoreActivityVersionRequired = 52553;

                /// <summary>
                /// Store activity created by is null or empty.
                /// </summary>
                public const int StoreActivityCreatedByRequired = 52554;

                /// <summary>
                /// Store activity auth group name is null or empty.
                /// </summary>
                public const int StoreActivityAuthGroupNameRequired = 52555;

                /// <summary>
                /// Activity library created by is null or empty.
                /// </summary>
                public const int ActivityLibraryCreatedByRequired = 52560;

                /// <summary>
                /// Activity library updated by is null or empty.
                /// </summary>
                public const int ActivityLibraryUpdatedByRequired = 52561;

                /// <summary>
                /// Activity library GUID is null.
                /// </summary>
                public const int ActivityLibraryGuidRequired = 52562;

                /// <summary>
                /// Activity library name is null or empty.
                /// </summary>
                public const int ActivityLibraryNameRequired = 52563;

                /// <summary>
                /// Activity library version is null or empty.
                /// </summary>
                public const int ActivityLibraryVersionRequired = 52564;

                /// <summary>
                /// Activity library auth group name is null or empty.
                /// </summary>
                public const int ActivityLibraryAuthGroupRequired = 52565;

                /// <summary>
                /// Activity library dependency created by is null or empty.
                /// </summary>
                public const int ActivityLibraryDependencyCreatedByRequired = 52580;

                /// <summary>
                /// Activity library dependency updated by is null or empty.
                /// </summary>
                public const int ActivityLibraryDependencyUpdatedByRequired = 52581;

                /// <summary>
                /// Activity library dependencies contain a recursive loop.
                /// </summary>
                public const int ActivityLibraryDependencyRecursiveLoopFound = 52582;

                /// <summary>
                /// The list of activity libraries is null or empty
                /// </summary>
                public const int ActivityLibrariesListRequired = 52583;

                /// <summary>
                /// Workflow type name is null or empty.
                /// </summary>
                public const int WorkflowTypeNameRequired = 52600;

               
            }
        }

        /// <summary>
        /// Defines web service layer event codes.
        /// </summary>
        public static class WebServiceLayerEvent
        {
            /// <summary>
            /// Defines web service layer error event codes.
            /// </summary>
            public static class Error
            {
                /// <summary>
                /// Unhandled exception was found.
                /// </summary>
                public const int UnhandledException	= 53000;

                /// <summary>
                /// Error code to event category mapping configuration was not found.
                /// </summary>
                public const int ErrorCodeToEventCategoryMappingConfigNotFound = 53001;
            }

            /// <summary>
            /// Defines web service layer information event codes.
            /// </summary>
            public static class Information
            {   
                /// <summary>
                /// A user request was authenticated.
                /// </summary>
                public const int RequestAuthenticated = 53900;
            }
        }
    }
}