//-----------------------------------------------------------------------
// <copyright file="WorkflowsQueryService.svc.cs" company="Microsoft">
// Copyright
// QueryService implementation of iWorkFlowsQueryService
// </copyright>
//-----------------------------------------------------------------------
namespace CWF.WorkflowQueryService
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using BAL.Versioning;
    using DataContracts;
    using Publishing;
    using Versioning;
    using Microsoft.Support.Workflow.Service.BusinessServices;
    using Microsoft.Support.Workflow.Service.DataAccessServices;
    using CWF.DataContracts.Marketplace;
    using CWF.WorkflowQueryService.Authentication;
    using System.Configuration;
    using CWF.WorkflowQueryService.Resources;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Workflow query service exposes a DAL providing CRUD access to the PrototypeAssetStore DB
    /// It also exposes a BAL which uses the DAL.
    /// </summary>
    public class WorkflowQueryService : IWorkflowsQueryService
    {
        /// <summary>
        /// Implementation of getting the current Extension Uri
        /// </summary>
        /// <returns>Extension Uri</returns>
        public string GetExtensionUri()
        {
            return OperationContext.Current.Channel.LocalAddress.Uri.ToString();
        }

        /// <summary>
        /// Publish workflow method
        /// </summary>
        public PublishingReply PublishWorkflow(PublishingRequest request)
        {
            PublishingReply reply = null;

            if (request == null)
            {
                reply = new PublishingReply();
                reply.StatusReply.Errorcode = SprocValues.REQUEST_OBJECT_IS_NULL_ID;
                reply.StatusReply.ErrorMessage = SprocValues.REQUEST_OBJECT_IS_NULL_MSG;
                return reply;
            }

            try
            {
                request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
                reply = PublishingClass.PublishWorkflow(this, request);
                reply.StatusReply.Errorcode = SprocValues.REPLY_ERRORCODE_VALUE_OK;
                return reply;
            }
            catch (Exception ex)
            {
                reply = new PublishingReply();
                reply.StatusReply.Errorcode = SprocValues.GENERIC_CATCH_ID;
                reply.StatusReply.ErrorMessage = "Error during publishing\n" + ex.ToString();
                return reply;
            }

        }

        /// <summary>
        /// Get ActivityLibrary row(s)
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>ActivityLibraryDC object</returns>
        public IList<StoreActivitiesDC> UploadActivityLibraryAndDependentActivities(StoreLibraryAndActivitiesRequestDC request)
        {
            List<StoreActivitiesDC> reply = new List<StoreActivitiesDC>();
            StatusReplyDC status = null;

            if (request == null)
            {
                status = CWF.BAL.Services.SetUpReplyStatus(SprocValues.REQUEST_OBJECT_IS_NULL_ID,
                                                            string.Format(SprocValues.REQUEST_OBJECT_IS_NULL_MSG,
                                                            "request",
                                                            "UploadActivityLibrarieAndDependentActivities"),
                                                            "");

            }
            if (request.ActivityLibrary == null)
            {
                status = CWF.BAL.Services.SetUpReplyStatus(SprocValues.REQUEST_OBJECT_IS_NULL_ID,
                                                           string.Format(SprocValues.REQUEST_OBJECT_IS_NULL_MSG,
                                                           "request.ActivityLibrary",
                                                           "UploadActivityLibrarieAndDependentActivities"),
                                                           "");
            }
            if (request.StoreActivitiesList == null)
            {
                status = CWF.BAL.Services.SetUpReplyStatus(SprocValues.REQUEST_OBJECT_IS_NULL_ID,
                                                          string.Format(SprocValues.REQUEST_OBJECT_IS_NULL_MSG,
                                                          "request.StoreActivitiesList",
                                                          "UploadActivityLibrarieAndDependentActivities"),
                                                          "");
            }

            if (status != null)
            {
                reply.Add(new StoreActivitiesDC() { StatusReply = status });
                return reply;
            }

            try
            {
                request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
                reply = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(request);
            }
            catch (VersionException ex)
            {
                throw new FaultException<VersionFault>(new VersionFault { Message = ex.Message, Rule = ex.Rule, },
                                                       new FaultReason(SprocValues.VersionIncorrectFaultReasonMessage + "\r\n" + ex.Message));
            }
            catch (BusinessException e)
            {
                e.HandleException();
            }
            catch (Exception e)
            {
                // Handles unhandled exception.
                e.HandleException();
            }
            return reply;
        }

        /// <summary>
        /// Get ActivityLibrary row(s)
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>ActivityLibraryDC object</returns>
        public GetAllActivityLibrariesReplyDC GetAllActivityLibraries(GetAllActivityLibrariesRequestDC request)
        {
            GetAllActivityLibrariesReplyDC reply = null;
            try
            {
                reply = ActivityLibraryBusinessService.GetActivityLibrariesWithoutDlls(request);
            }
            catch (BusinessException e)
            {
                e.HandleException();
            }
            catch (Exception e)
            {
                // Handles unhandled exception.
                e.HandleException();
            }
            return reply;
        }

        /// <summary>
        /// Check if the specified activity libraries exist in the data store.
        /// </summary>
        /// <param name="request"></param>
        public GetMissingActivityLibrariesReply GetMissingActivityLibraries(GetMissingActivityLibrariesRequest request)
        {
            GetMissingActivityLibrariesReply reply = null;

            try
            {
                reply = ActivityLibraryBusinessService.GetMissingActivityLibraries(request);
            }
            catch (BusinessException e)
            {
                e.HandleException();
            }
            catch (Exception e)
            {
                // Handles unhandled exception.
                e.HandleException();
            }
            return reply;
        }

        /// <summary>
        /// Gets activities by activity library ID or Name & Version combination while handling any exceptions to return
        /// them as service faults to the caller.
        /// </summary>
        /// <param name="request">Request that specifies activity library identifier info.</param>
        /// <returns>Response that contains a list of activities.</returns>
        public GetActivitiesByActivityLibraryNameAndVersionReplyDC GetActivitiesByActivityLibraryNameAndVersion(GetActivitiesByActivityLibraryNameAndVersionRequestDC request)
        {
            GetActivitiesByActivityLibraryNameAndVersionReplyDC reply = null;

            try
            {
                reply = ActivityBusinessService.GetActivitiesByActivityLibrary(request, false);
            }
            catch (BusinessException e)
            {
                e.HandleException();
            }
            catch (Exception e)
            {
                // Handles unhandled exception.
                e.HandleException();
            }

            return reply;
        }

        /// <summary>
        /// Stores the ActivityLibrary dependency list pairs (ActivityLibrary and dependent ActivityLibrary)
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>StoreActivityLibrariesDependenciesDC object</returns>
        public StoreActivityLibrariesDependenciesDC StoreActivityLibraryDependencyList(StoreActivityLibrariesDependenciesDC request)
        {
            //// Eliminate with validation pipeline implementation
            if (request == null)
            {
                StoreActivityLibrariesDependenciesDC reply = new StoreActivityLibrariesDependenciesDC();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            return BAL.Services.StoreActivityLibraryDependencyList(request);
        }

        /// <summary>
        /// Gets the list of all the dependencies in the dependency hierarchy for an an activity library while handling any exceptions to return
        /// them as service faults to the caller.
        /// </summary>
        /// <param name="request">Request that defines the root activity library for which the dependencies are to be found.</param>
        /// <returns>Response that contains a list of dependencies.</returns>
        public List<StoreActivityLibrariesDependenciesDC> StoreActivityLibraryDependenciesTreeGet(StoreActivityLibrariesDependenciesDC request)
        {
            List<StoreActivityLibrariesDependenciesDC> reply = null;

            try
            {
                reply = ActivityLibraryDependencyBusinessService.GetActivityLibraryDependencyTree(request);
            }
            catch (BusinessException e)
            {
                e.HandleException();
            }
            catch (Exception e)
            {
                // Handles unhandled exception.
                e.HandleException();
            }
            return reply;
        }

        /// <summary>
        /// Creates or updates ActivityCategory row
        /// </summary>
        /// <param name="request">ActivityCategoryCreateOrUpdateRequestDC object</param>
        /// <returns>ActivityCategoryCreateOrUpdateReplyDC object</returns>
        public ActivityCategoryCreateOrUpdateReplyDC ActivityCategoryCreateOrUpdate(ActivityCategoryCreateOrUpdateRequestDC request)
        {
            //// Eliminate with validation pipeline implementation
            if (request == null)
            {
                ActivityCategoryCreateOrUpdateReplyDC reply = new ActivityCategoryCreateOrUpdateReplyDC();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            string[] authorGroups = SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb());
            request.InAuthGroupName = authorGroups.First();
            return ActivityCategory.ActivityCategoryCreateOrUpdate(request);
        }

        /// <summary>
        /// Gets activity categories by ID, Name or by ID & Name while handling any exceptions to return
        /// them as service faults to the caller.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>List of activity categories found.</returns>
        public List<ActivityCategoryByNameGetReplyDC> ActivityCategoryGet(ActivityCategoryByNameGetRequestDC request)
        {
            List<ActivityCategoryByNameGetReplyDC> reply = null;
            try
            {
                reply = ActivityCategoryBusinessService.GetActivityCategories(request);
            }
            catch (BusinessException e)
            {
                e.HandleException();
            }
            catch (Exception e)
            {
                // Handles unhandled exception.
                e.HandleException();
            }

            return reply;
        }

        /// <summary>
        /// Gets an activity library by ID if a positive ID value is provided.  
        /// Otherwise, gets by GUID if a non-empty GUID is provided.  If the ID is not positive and the GUID 
        /// is empty, then a list of activity libraries matching the name and version combination.
        /// </summary>
        /// <param name="request">ActivityLibraryDC request object.</param>
        /// <returns>List of ActivityLibraryDC.</returns>
        public List<ActivityLibraryDC> ActivityLibraryGet(ActivityLibraryDC request)
        {
            List<ActivityLibraryDC> reply = null;
            try
            {
                reply = ActivityLibraryBusinessService.GetActivityLibraries(request);
            }
            catch (BusinessException e)
            {
                e.HandleException();
            }
            catch (Exception e)
            {
                // Handles unhandled exception.
                e.HandleException();
            }
            return reply;
        }

        /// <summary>
        /// Get Applications row(s)
        /// </summary>
        /// <param name="request">ApplicationsGetRequestDC object</param>
        /// <returns>ApplicationsGetReplyDC List object</returns>
        public ApplicationsGetReplyDC ApplicationsGet(ApplicationsGetRequestDC request)
        {
            //// Eliminate with validation pipeline implementation
            if (request == null)
            {
                var reply = new ApplicationsGetReplyDC();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            return Applications.ApplicationsGet(request);
        }

        /// <summary>
        /// Get StatusCodes row(s)
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>StatusCodeGetReplyDC object</returns>
        public StatusCodeGetReplyDC StatusCodeGet(StatusCodeGetRequestDC request)
        {
            //// Eliminate with validation pipeline implementation
            if (request == null)
            {
                var reply = new StatusCodeGetReplyDC();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            return StatusCode.StatusCodeGet(request);
        }

        /// <summary>
        /// Get StoreActivities row(s)
        /// </summary>
        /// <param name="request">StoreActivitiesDC object</param>
        /// <returns>reply object</returns>
        public List<StoreActivitiesDC> StoreActivitiesGet(StoreActivitiesDC request)
        {
            //// Eliminate with validation pipeline implementation
            if (request == null)
            {
                var reply = new List<StoreActivitiesDC>();
                reply.Add(new StoreActivitiesDC());
                reply[0].StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            return Activities.StoreActivitiesGet(request);
        }

        /// <summary>
        /// Gets all workflow types  while handling any exceptions to return
        /// them as service faults to the caller.
        /// </summary>
        /// <returns>List of workflow types found.</returns>
        public WorkflowTypeGetReplyDC WorkflowTypeGet(WorkflowTypesGetRequestDC request)
        {
            WorkflowTypeGetReplyDC reply = null;
            try
            {
                reply = WorkflowTypeBusinessService.GetWorkflowTypes(request);
            }
            catch (BusinessException e)
            {
                e.HandleException();
            }
            catch (Exception e)
            {
                // Handles unhandled exception.
                e.HandleException();
            }
            return reply;
        }

        /// <summary>
        /// Populates the statusResult object with the request is null ErrorCode and ErrorMessage
        /// </summary>
        /// <returns>StatusReplyDC object</returns>
        private static CWF.DataContracts.StatusReplyDC SetupStatusreplyNullRequestError()
        {
            var reply = new StatusReplyDC();

            reply.Errorcode = SprocValues.REQUEST_OBJECT_IS_NULL_ID;
            reply.ErrorMessage = SprocValues.REQUEST_OBJECT_IS_NULL_MSG;

            return reply;
        }

        /// <summary>
        /// Performs input validation and gets activities by searching with some params
        /// </summary>
        /// <param name="request">Request that specifies the search parameters</param>
        /// <returns>Response that contains a list of activities.</returns>
        public ActivitySearchReplyDC SearchActivities(ActivitySearchRequestDC request)
        {
            ActivitySearchReplyDC reply = null;
            try
            {
                request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
                reply = ActivityBusinessService.SearchActivities(request);
            }
            catch (BusinessException e)
            {
                e.HandleException();
            }
            catch (Exception e)
            {
                // Handles unhandled exception.
                e.HandleException();
            }
            return reply;
        }

        public MarketplaceSearchResult SearchMarketplace(MarketplaceSearchQuery request)
        {
            request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
            return MarketplaceRepositoryService.SearchMarketplace(request);
        }

        /// <summary>
        /// This method invokes MarketplaceBusinessService.GetAssetDetails operation.
        /// </summary>
        /// <returns></returns>
        public MarketplaceAssetDetails GetMarketplaceAssetDetails(MarketplaceSearchDetail request)
        {
            return MarketplaceRepositoryService.GetMarketplaceAssetDetails(request);
        }

        /// <summary>
        /// Set lock on StoreActivities
        /// </summary>
        /// <param name="request"></param>
        /// <param name="lockedTime"></param>
        /// <returns></returns>
        public StatusReplyDC StoreActivitiesUpdateLock(StoreActivitiesDC request, DateTime lockedTime)
        {
            if (request == null)
            {
                return SetupStatusreplyNullRequestError();
            }

            request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
            return CWF.BAL.Services.StoreActivitiesUpdateLock(request, lockedTime);
        }

        /// <summary>
        /// Override lock on StoreActivities
        /// </summary>
        /// <param name="request"></param>
        /// <param name="lockedTime"></param>
        /// <returns></returns>
        public StatusReplyDC StoreActivitiesOverrideLock(StoreActivitiesDC request, DateTime lockedTime)
        {
            if (request == null)
            {
                return SetupStatusreplyNullRequestError();
            }

            request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
            return CWF.BAL.Services.StoreActivitiesOverrideLock((request), lockedTime);
        }

        /// <summary>
        /// Gets the next version number for a workflow.
        /// </summary>
        /// <param name="request">An object describing the workflow for which we need a new version number.</param>
        /// <param name="env">The name of the user making the request.</param>
        /// <returns></returns>
        public Version GetNextVersion(StoreActivitiesDC request, string env)
        {
            return CWF.BAL.Services.GetNextVersion(request, env);
        }

        /// <summary>
        /// Get all records for a workflow name that are public or retired, plus all workflows with that 
        /// name that are private and owned by the specified user.
        /// </summary>
        /// <param name="request">StoreActivitiesDC object</param>
        /// <returns></returns>
        public IList<StoreActivitiesDC> StoreActivitiesGetByName(StoreActivitiesDC request)
        {
            if (request == null)
            {
                var reply = new List<StoreActivitiesDC>();
                reply.Add(new StoreActivitiesDC());
                reply[0].StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }
            return Activities.StoreActivitiesGetByName(request.Name, request.Environment);
        }

        [Obsolete("Used only by func test")]
        /// <summary>
        /// Create or Update ActivityLibrary row
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>ActivityLibraryDC object</returns>
        public ActivityLibraryDC ActivityLibraryCreateOrUpdate(ActivityLibraryDC request)
        {
            //// Eliminate with validation pipeline implementation
            if (request == null)
            {
                var reply = new ActivityLibraryDC();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
            return ActivityLibrary.ActivityLibraryCreateOrUpdate(request);
        }

        /// <summary>
        /// Create or Update WorkflowType row
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public WorkFlowTypeCreateOrUpdateReplyDC WorkflowTypeCreateOrUpdate(WorkFlowTypeCreateOrUpdateRequestDC request)
        {
            if (request == null)
            {
                WorkFlowTypeCreateOrUpdateReplyDC reply = new WorkFlowTypeCreateOrUpdateReplyDC();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
            return WorkflowTypeBusinessService.WorkflowTypeCreateOrUpdate(request);
        }

        /// <summary>
        /// Search workflow types
        /// If none is specified, returns all the workflow types.
        /// </summary>
        /// <returns>Reply that contains the list of workflow types found.</returns>
        public WorkflowTypeSearchReply SearchWorkflowTypes(WorkflowTypeSearchRequest request)
        {
            if (request == null)
            {
                WorkflowTypeSearchReply reply = new WorkflowTypeSearchReply();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
            return WorkflowTypeBusinessService.SearchWorkflowTypes(request);
        }

        /// <summary>
        /// Get AuthorizationGroup
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AuthorizationGroupGetReplyDC GetAuthorizationGroups(AuthorizationGroupGetRequestDC request)
        {
            if (request == null)
            {
                AuthorizationGroupGetReplyDC reply = new AuthorizationGroupGetReplyDC();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            return AuthorizationGroupBusinessService.GetAuthorizationGroups(request);
        }

        /// <summary>
        /// Create Or Update AuthorizationGroup
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AuthGroupsCreateOrUpdateReplyDC AuthorizationGroupCreateOrUpdate(AuthGroupsCreateOrUpdateRequestDC request)
        {
            if (request == null)
            {
                AuthGroupsCreateOrUpdateReplyDC reply = new AuthGroupsCreateOrUpdateReplyDC();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
            return AuthorizationGroup.AuthGroupsCreateOrUpdate(request);
        }

        /// <summary>
        /// Enable Or Disable AuthorizationGroup
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AuthGroupsEnableOrDisableReplyDC AuthorizationGroupEnableOrDisable(AuthGroupsEnableOrDisableRequestDC request)
        {
            if (request == null)
            {
                AuthGroupsEnableOrDisableReplyDC reply = new AuthGroupsEnableOrDisableReplyDC();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
            return AuthorizationGroup.AuthGroupsEnableOrDisable(request);
        }

        /// <summary>
        /// Create or update TaskActivity
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IList<TaskActivityDC> UploadActivityLibraryAndTaskActivities(StoreLibraryAndTaskActivityRequestDC request)
        {
            List<TaskActivityDC> reply = new List<TaskActivityDC>();
            StatusReplyDC status = null;

            if (request == null)
            {
                status = CWF.BAL.Services.SetUpReplyStatus(SprocValues.REQUEST_OBJECT_IS_NULL_ID,
                                                            string.Format(SprocValues.REQUEST_OBJECT_IS_NULL_MSG,
                                                            "request",
                                                            "UploadActivityLibraryAndTaskActivities"),
                                                            "");

            }

            if (request.ActivityLibrary == null)
            {
                status = CWF.BAL.Services.SetUpReplyStatus(SprocValues.REQUEST_OBJECT_IS_NULL_ID,
                                                           string.Format(SprocValues.REQUEST_OBJECT_IS_NULL_MSG,
                                                           "request.ActivityLibrary",
                                                           "UploadActivityLibraryAndTaskActivities"),
                                                           "");
            }

            if (request.TaskActivitiesList == null)
            {
                status = CWF.BAL.Services.SetUpReplyStatus(SprocValues.REQUEST_OBJECT_IS_NULL_ID,
                                                          string.Format(SprocValues.REQUEST_OBJECT_IS_NULL_MSG,
                                                          "request.StoreActivitiesList",
                                                          "UploadActivityLibraryAndTaskActivities"),
                                                          "");
            }

            if (status != null)
            {
                reply.Add(new TaskActivityDC() { StatusReply = status });
                return reply;
            }

            try
            {
                request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
                reply = CWF.BAL.Services.UploadLibraryAndTaskActivities(request);
            }
            catch (VersionException ex)
            {
                throw new FaultException<VersionFault>(new VersionFault { Message = ex.Message, Rule = ex.Rule, },
                                                       new FaultReason(SprocValues.VersionIncorrectFaultReasonMessage + "\r\n" + ex.Message));
            }
            catch (BusinessException e)
            {
                e.HandleException();
            }
            catch (Exception e)
            {
                // Handles unhandled exception.
                e.HandleException();
            }
            return reply;
        }

        /// <summary>
        /// Get TaskActivity
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TaskActivityGetReplyDC SearchTaskActivities(TaskActivityGetRequestDC request)
        {
            //// Eliminate with validation pipeline implementation
            if (request == null)
            {
                var reply = new TaskActivityGetReplyDC();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
            return TaskActivityBusinessService.GetTaskActivities(request);
        }

        /// <summary>
        /// Get latest ActivityGet
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TaskActivityDC TaskActivityGet(TaskActivityDC request)
        {
            //// Eliminate with validation pipeline implementation
            if (request == null)
            {
                var reply = new TaskActivityDC();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }
            return TaskActivityBusinessService.TaskActivityGet(request);
        }

        public TaskActivityDC TaskActivityUpdateStatus(TaskActivityDC request)
        {
            //// Eliminate with validation pipeline implementation
            if (request == null)
            {
                var reply = new TaskActivityDC();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
            return TaskActivityBusinessService.TaskActivityUpdateStatus(request);
        }

        public TaskActivityGetListReply TaskActivityGetList(TaskActivityGetListRequest request) 
        {
            if (request == null)
            {
                var reply = new TaskActivityGetListReply();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
            return TaskActivityBusinessService.TaskActivityGetList(request);
        }

        public PermissionGetListReply PermissionGetList(RequestHeader request)
        {
            return Permission.PermissionGet(request);
        }

        public string TenantGet()
        {
            if ((ConfigurationManager.AppSettings.Keys.Count > 0) && (!string.IsNullOrEmpty(AppSettings.TenantName)))
            {
                return AppSettings.TenantName;
            }

            return string.Empty;
        }

        public ChangeAuthorReply ChangeAuthor(ChangeAuthorRequest request)
        {
            if (request == null)
            {
                var reply = new ChangeAuthorReply();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
            return Activities.ChangeAuthor(request);
        }

        public StoreActivitiesDC ActivityCopy(ActivityCopyRequest request)
        {
            if (request == null)
            {
                var reply = new StoreActivitiesDC();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
            return ActivityBusinessService.ActivityCopy(request);
        }

        public ActivityMoveReply ActivityMove(ActivityMoveRequest request)
        {
            if (request == null)
            {
                var reply = new ActivityMoveReply();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
            return Activities.ActivityMove(request);
        }

        public StoreActivitiesDC ActivityDelete(StoreActivitiesDC request)
        {
            if (request == null)
            {
                var reply = new StoreActivitiesDC();
                reply.StatusReply = SetupStatusreplyNullRequestError();
                return reply;
            }

            request.AddAuthGroupOnRequest(SecurityService.GetSecurityIdentifierArray(GetAuthorizationGroupsInDb()));
            return Activities.StoreActivitiesDelete(request);
        }

        private string[] GetAuthorizationGroupsInDb()
        {
            AuthorizationGroupGetRequestDC request = new AuthorizationGroupGetRequestDC()
            {
                Incaller = Environment.UserName,
                IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
            };

            var result = GetAuthorizationGroups(request);
            if (result.StatusReply.Errorcode != 0)
                throw new UnauthorizedAccessException();

            return result.AuthorizationGroups.Select(a => a.AuthGroupName).ToArray();
        }
    }
}
