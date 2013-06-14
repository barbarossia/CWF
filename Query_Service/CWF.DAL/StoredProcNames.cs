namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    public static class StoredProcNames
    {
        public const string ActivityCheckExists = "Activity_Exists";
        public const string ActivityCreateOrUpdate = "Activity_CreateOrUpdate";
        public const string ActivityDelete = "Activity_Delete";
        public const string ActivityDeleteByNameAndVersion = "Activity_DeleteByNameAndVersion";
        public const string ActivityGet = "Activity_Get";
        public const string ActivityGetByName = "Activity_GetByName";
        public const string ActivityGetByActivityLibrary = "Activity_GetByActivityLibrary";
        public const string ActivityGetByCategoryNameWithoutExecutable = "Activity_GetByCategoryNameWithoutExecutable";
        public const string ActivitySearch = "Activity_Search";
        public const string ActivityUpdateLock = "Activity_UpdateLock";

        public const string PublishingWorkFlowGetByWorkFlowType = "PublishWorkFlow_GetByWorkFlowType";
        public const string PublishWorkflowGetByNameVersion = "PublishWorkflow_GetByNameVersion";

        public const string ActivityLibraryDependencyCreteOrUpdateListHead = "ActivityLibraryDependency_CreateOrUpdateListHead";
        public const string ActivityLibraryDependencyCreateOrUpdate = "ActivityLibraryDependency_CreateOrUpdate";
        public const string ActivityLibraryDependencyGetTree = "ActivityLibraryDependency_GetTree";
        public const string ActivityLibraryDependencyGet = "ActivityLibraryDependency_Get";
        public const string ActivityLibraryDependencyDelete = "ActivityLibraryDependency_Delete";

        public const string ActivityCategoryCreateOrUpdate = "ActivityCategory_CreateOrUpdate";
        public const string ActivityCategoryDelete = "ActivityCategory_Delete";
        public const string ActivityCategoryGet = "ActivityCategory_Get";

        public const string ActivityLibraryExists = "ActivityLibrary_Exists";
        public const string ActivityLibraryGet = "ActivityLibrary_Get";
        public const string ActivityLibraryCreateOrUpdate = "ActivityLibrary_CreateOrUpdate";
        public const string ActivityLibraryDelete = "ActivityLibrary_Delete";
        public const string ActivityLibraryGetWithoutExecutable = "ActivityLibrary_GetWithoutExecutable";
        public const string ActivityLibraryGetMissing = "ActivityLibrary_GetMissing";


        public const string ApplicationCreateOrUpdate = "Application_CreateOrUpdate";
        public const string ApplicationDelete = "Application_Delete";
        public const string ApplicationGet = "Application_Get";

        public const string AuthorizationGroupCreateOrUpdate = "AuthorizationGroup_CreateOrUpdate";
        public const string AuthorizationGroupDelete = "AuthorizationGroup_Delete";
        public const string AuthorizationGroupGet = "AuthorizationGroup_Get";

        public const string StatusCodeCreateOrUpdate = "StatusCode_CreateOrUpdate";
        public const string StatusCodeDelete = "StatusCode_Delete";
        public const string StatusCodeGet = "StatusCode_Get";

        public const string ToolboxTabNameCreateOrUpdate = "ToolboxTabName_CreateOrUpdate";
        public const string ToolboxTabNameDelete = "ToolboxTabName_Delete";
        public const string ToolboxTabNameGet = "ToolboxTabName_Get";

        public const string WorkflowTypeCreateOrUpdate = "WorkflowType_CreateOrUpdate";
        public const string WorkflowTypeGet = "WorkflowType_Get";
        public const string WorkflowType_BindPublishingWorkflow = "WorkflowType_BindPublishingWorkflow";
        public const string WorkflowType_Search = "WorkflowType_Search";

        public const string TaskActivityCreateOrUpdate = "TaskActivity_CreateOrUpdate";
        public const string TaskActivityGet = "TaskActivity_Get";
        public const string TaskActivitySearch = "TaskActivity_Search";
        public const string TaskActivityUpdateStatus = "TaskActivity_UpdateStatus";

        public const string CompositeGet = "Composite_Get";
        public const string CompositeCreate = "Composite_Create";

    }
}
