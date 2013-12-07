//-----------------------------------------------------------------------
// <copyright file="IWorkflowsQueryService.cs" company="Microsoft">
// Copyright
// Interface definition for QueryService
// </copyright>
//-----------------------------------------------------------------------
namespace CWF.WorkflowQueryService
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using CWF.DataContracts;
    using CWF.Publishing;
    using CWF.BAL;
    using CWF.BAL.Versioning;
    using CWF.WorkflowQueryService.Versioning;
    using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;
using CWF.DataContracts.Marketplace;

    /// <summary>
    /// Workflow query service exposes a DAL providing CRUD access to the PrototypeAssetStore DB
    /// It also exposes a BAL which uses the DAL.
    /// </summary>
    [ServiceContract(Namespace = "http://cwf.microsoft.com")]
    //// [XmlSerializerFormat]
    //// [ValidationBehavior]
    public interface IWorkflowsQueryService
    {
        /// <summary>
        /// Get ActivityLibrary row(s)
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>ActivityLibraryDC object</returns>
        [OperationContract]
        [FaultContract(typeof(VersionFault))]
        IList<StoreActivitiesDC> UploadActivityLibraryAndDependentActivities(StoreLibraryAndActivitiesRequestDC request);

        /// <summary>
        /// Get ActivityLibrary row(s)
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>ActivityLibraryDC object</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        [FaultContract(typeof(ValidationFault))]
        GetAllActivityLibrariesReplyDC GetAllActivityLibraries(GetAllActivityLibrariesRequestDC request);

        /// <summary>
        /// Publish workflow method
        /// </summary>
        /// <param name="request">Publishing request</param>
        /// <returns>ReplyHeader with the result of the Publish</returns>
        [OperationContract]
        PublishingReply PublishWorkflow(PublishingRequest request);

        /// <summary>
        /// Interface definition to get the Extension Uri
        /// </summary>
        /// <returns>Returns the Uri that the current Query Service is executing in</returns>
        [OperationContract]
        string GetExtensionUri();

        /// <summary>
        /// BAL Version - Gets the ActivityLibrary and associated StoreActivities method by Library Id or (name and version). One Library and n StoreActivity entries are returned, under
        /// transaction control based on the ActivityLibrary name and version If their is a failure, a fault is thrown.
        /// </summary>
        /// <param name="request">GetLibraryAndActivitiesDC object</param>
        /// <returns>Fault or reply object</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        [FaultContract(typeof(ValidationFault))]
        GetActivitiesByActivityLibraryNameAndVersionReplyDC GetActivitiesByActivityLibraryNameAndVersion(GetActivitiesByActivityLibraryNameAndVersionRequestDC request);

        /// <summary>
        /// Stores the ActivityLibrary dependency list pairs (ActivityLibrary and dependent ActivityLibrary)
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>StoreActivityLibrariesDependenciesDC object</returns>
        [OperationContract]
        StoreActivityLibrariesDependenciesDC StoreActivityLibraryDependencyList(StoreActivityLibrariesDependenciesDC request);

        /// <summary>
        /// Gets the entire tree from the mtblActivityLibraryDependencies table
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>StatusReplyDC object</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        [FaultContract(typeof(ValidationFault))]
        List<StoreActivityLibrariesDependenciesDC> StoreActivityLibraryDependenciesTreeGet(StoreActivityLibrariesDependenciesDC request);

        /// <summary>
        /// Creates or updates ActivityCategory row
        /// </summary>
        /// <param name="request">ActivityCategoryCreateOrUpdateRequestDC object</param>
        /// <returns>ActivityCategoryCreateOrUpdateReplyDC object</returns>
        [OperationContract]
        ActivityCategoryCreateOrUpdateReplyDC ActivityCategoryCreateOrUpdate(ActivityCategoryCreateOrUpdateRequestDC request);

        /// <summary>
        /// Gets ActivityCategory row(s)
        /// </summary>
        /// <param name="request">ActivityCategoryByNameGetRequestDC object</param>
        /// <returns>ActivityCategoryByNameGetReplyDC object</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        [FaultContract(typeof(ValidationFault))]
        List<ActivityCategoryByNameGetReplyDC> ActivityCategoryGet(ActivityCategoryByNameGetRequestDC request);

        /// <summary>
        /// Get ActivityLibrary row(s)
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>ActivityLibraryDC object</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        [FaultContract(typeof(ValidationFault))]
        List<ActivityLibraryDC> ActivityLibraryGet(ActivityLibraryDC request);

        /// <summary>
        /// Get Applications row(s)
        /// </summary>
        /// <param name="request">ApplicationsGetRequestDC object</param>
        /// <returns>ApplicationsGetReplyDC List object</returns>
        [OperationContract]
        ApplicationsGetReplyDC ApplicationsGet(ApplicationsGetRequestDC request);

        /// <summary>
        /// Get StatusCodes row(s)
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>StatusCodeGetReplyDC object</returns>
        [OperationContract]
        StatusCodeGetReplyDC StatusCodeGet(StatusCodeGetRequestDC request);

        /// <summary>
        /// Get StoreActivities row(s)
        /// </summary>
        /// <param name="request">StoreActivitiesDC object</param>
        /// <returns>reply object</returns>
        [OperationContract]
        List<StoreActivitiesDC> StoreActivitiesGet(StoreActivitiesDC request);

        /// <summary>
        /// Get WorkflowTypes row(s)
        /// </summary>
        /// <returns>WorkflowTypeGetReplyDC object</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        WorkflowTypeGetReplyDC WorkflowTypeGet();

        [OperationContract]
        Version GetNextVersion(StoreActivitiesDC request, string userName);

        /// <summary>
        /// Check if the specified activity libraries exist in the data store.
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>ActivityLibrariesCheckExistReply object</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        [FaultContract(typeof(ValidationFault))]
        GetMissingActivityLibrariesReply GetMissingActivityLibraries(GetMissingActivityLibrariesRequest request);

       /// <summary>
        /// Check for activities in the data store.
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>ActivitySearchReplyDC object</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        [FaultContract(typeof(ValidationFault))]
        ActivitySearchReplyDC SearchActivities(ActivitySearchRequestDC request);
        
        /// <summary>
        /// This method invokes MarketplaceBusinessService.Search operation to 
        /// retrieve the workflow assets based on the search criteria.  
        /// This operation is used to search both Marketplace items and user specific items.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        [FaultContract(typeof(ValidationFault))]
        MarketplaceSearchResult SearchMarketplace(MarketplaceSearchQuery request);

        /// <summary>
        /// This method invokes MarketplaceBusinessService.GetAssetDetails operation.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        [FaultContract(typeof(ValidationFault))]
        MarketplaceAssetDetails GetMarketplaceAssetDetails(MarketplaceSearchDetail request);

        /// <summary>
        /// Set lock on StoreActivities
        /// </summary>
        /// <param name="request"></param>
        /// <param name="lockedTime"></param>
        /// <returns></returns>
        [OperationContract]
        StatusReplyDC StoreActivitiesSetLock(StoreActivitiesDC request, DateTime lockedTime);

        /// <summary>
        /// Get StoreActivities row(s)
        /// </summary>
        /// <param name="request">StoreActivitiesDC object</param>
        /// <returns>list of reply object</returns>
        [OperationContract]
        IList<StoreActivitiesDC> StoreActivitiesGetByName(StoreActivitiesDC request);

        [Obsolete("Used only by func test")]
        /// <summary>
        /// Create or Update ActivityLibrary row
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>ActivityLibraryDC object</returns>
        [OperationContract]
        ActivityLibraryDC ActivityLibraryCreateOrUpdate(ActivityLibraryDC request);

    }
}
