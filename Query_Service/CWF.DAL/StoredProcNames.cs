namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    public static class StoredProcNames
    {
        public const string ActivityCheckExists = "Activity_Exists";
        public const string ActivityCreateOrUpdate = "Activity_CreateOrUpdate";
        public const string ActivityDelete = "Activity_Delete";
        public const string ActivityGet = "Activity_Get";
        public const string ActivityGetByName = "Activity_GetByName";
        public const string ActivityGetByActivityLibrary = "Activity_GetByActivityLibrary";
        public const string ActivitySearch = "Activity_Search";
        public const string ActivityUpdateLock = "Activity_UpdateLock";
        public const string ActivityOverrideLock = "Activity_OverrideLock";
        public const string ActivityChangeAuthor = "Activity_ChangeAuthor";
        public const string ActivityCopy = "Activity_Copy";
        public const string ActivityMove = "Activity_Move";

        public const string ActivityLibraryDependencyCreteOrUpdateListHead = "ActivityLibraryDependency_CreateOrUpdateListHead";
        public const string ActivityLibraryDependencyCreateOrUpdate = "ActivityLibraryDependency_CreateOrUpdate";
        public const string ActivityLibraryDependencyGetTree = "ActivityLibraryDependency_GetTree";

        public const string ActivityCategoryCreateOrUpdate = "ActivityCategory_CreateOrUpdate";
        public const string ActivityCategoryGet = "ActivityCategory_Get";

        public const string ActivityLibraryGet = "ActivityLibrary_Get";
        public const string ActivityLibraryCreateOrUpdate = "ActivityLibrary_CreateOrUpdate";
        public const string ActivityLibraryGetWithoutExecutable = "ActivityLibrary_GetWithoutExecutable";
        public const string ActivityLibraryGetMissing = "ActivityLibrary_GetMissing";

        public const string ApplicationGet = "Application_Get";

        public const string AuthorizationGroupCreateOrUpdate = "AuthorizationGroup_CreateOrUpdate";
        public const string AuthorizationGroupGet = "AuthorizationGroup_Get";
        public const string AuthorizationGroupEnableOrDisable = "AuthorizationGroup_EnableOrDisable";

        public const string StatusCodeGet = "StatusCode_Get";

        public const string WorkflowTypeCreateOrUpdate = "WorkflowType_CreateOrUpdate";
        public const string WorkflowTypeGet = "WorkflowType_Get";
        public const string WorkflowType_Search = "WorkflowType_Search";

        public const string TaskActivityCreateOrUpdate = "TaskActivity_CreateOrUpdate";
        public const string TaskActivityGet = "TaskActivity_Get";
        public const string TaskActivitySearch = "TaskActivity_Search";
        public const string TaskActivityUpdateStatus = "TaskActivity_UpdateStatus";

        public const string PermissionGet = "Permission_Get";

    }
}
