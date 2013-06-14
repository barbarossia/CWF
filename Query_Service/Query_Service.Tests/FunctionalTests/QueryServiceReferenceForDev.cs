﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Query_Service.Tests
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://cwf.microsoft.com", ConfigurationName = "IWorkflowsQueryService")]
    public interface IWorkflowsQueryServiceDevBranch
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/UploadActivityLibraryAndDependent" +
            "Activities", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/UploadActivityLibraryAndDependent" +
            "ActivitiesResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(CWF.WorkflowQueryService.Versioning.VersionFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/UploadActivityLibraryAndDependent" +
            "ActivitiesVersionFaultFault", Name = "VersionFault", Namespace = "http://schemas.datacontract.org/2004/07/CWF.WorkflowQueryService.Versioning")]
        CWF.DataContracts.StoreActivitiesDC[] UploadActivityLibraryAndDependentActivities(CWF.DataContracts.StoreLibraryAndActivitiesRequestDC request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/GetAllActivityLibraries", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/GetAllActivityLibrariesResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/GetAllActivityLibrariesServiceFau" +
            "ltFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ValidationFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/GetAllActivityLibrariesValidation" +
            "FaultFault", Name = "ValidationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        CWF.DataContracts.GetAllActivityLibrariesReplyDC GetAllActivityLibraries(CWF.DataContracts.GetAllActivityLibrariesRequestDC request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/PublishWorkflow", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/PublishWorkflowResponse")]
        CWF.DataContracts.PublishingReply PublishWorkflow(CWF.DataContracts.PublishingRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/GetExtensionUri", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/GetExtensionUriResponse")]
        string GetExtensionUri();

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/GetActivitiesByActivityLibraryNam" +
            "eAndVersion", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/GetActivitiesByActivityLibraryNam" +
            "eAndVersionResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ValidationFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/GetActivitiesByActivityLibraryNam" +
            "eAndVersionValidationFaultFault", Name = "ValidationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/GetActivitiesByActivityLibraryNam" +
            "eAndVersionServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        CWF.DataContracts.GetActivitiesByActivityLibraryNameAndVersionReplyDC GetActivitiesByActivityLibraryNameAndVersion(CWF.DataContracts.GetActivitiesByActivityLibraryNameAndVersionRequestDC request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/GetPublishingWorkFlowByWorkFlowTy" +
            "pe", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/GetPublishingWorkFlowByWorkFlowTy" +
            "peResponse")]
        CWF.DataContracts.GetPublishingWorkFlowByWorkFlowTypeReplyDC GetPublishingWorkFlowByWorkFlowType(CWF.DataContracts.GetPublishingWorkFlowByWorkFlowTypeRequestDC request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/StoreActivityLibraryDependencyLis" +
            "t", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/StoreActivityLibraryDependencyLis" +
            "tResponse")]
        CWF.DataContracts.StoreActivityLibrariesDependenciesDC StoreActivityLibraryDependencyList(CWF.DataContracts.StoreActivityLibrariesDependenciesDC request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/StoreActivityLibraryDependenciesT" +
            "reeGet", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/StoreActivityLibraryDependenciesT" +
            "reeGetResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/StoreActivityLibraryDependenciesT" +
            "reeGetServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ValidationFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/StoreActivityLibraryDependenciesT" +
            "reeGetValidationFaultFault", Name = "ValidationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        CWF.DataContracts.StoreActivityLibrariesDependenciesDC[] StoreActivityLibraryDependenciesTreeGet(CWF.DataContracts.StoreActivityLibrariesDependenciesDC request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/ActivityCategoryCreateOrUpdate", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/ActivityCategoryCreateOrUpdateRes" +
            "ponse")]
        CWF.DataContracts.ActivityCategoryCreateOrUpdateReplyDC ActivityCategoryCreateOrUpdate(CWF.DataContracts.ActivityCategoryCreateOrUpdateRequestDC request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/ActivityCategoryGet", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/ActivityCategoryGetResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ValidationFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/ActivityCategoryGetValidationFaul" +
            "tFault", Name = "ValidationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/ActivityCategoryGetServiceFaultFa" +
            "ult", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        CWF.DataContracts.ActivityCategoryByNameGetReplyDC[] ActivityCategoryGet(CWF.DataContracts.ActivityCategoryByNameGetRequestDC request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/ActivityLibraryCreateOrUpdate", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/ActivityLibraryCreateOrUpdateResp" +
            "onse")]
        CWF.DataContracts.ActivityLibraryDC ActivityLibraryCreateOrUpdate(CWF.DataContracts.ActivityLibraryDC request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/ActivityLibraryGet", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/ActivityLibraryGetResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/ActivityLibraryGetServiceFaultFau" +
            "lt", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ValidationFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/ActivityLibraryGetValidationFault" +
            "Fault", Name = "ValidationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        CWF.DataContracts.ActivityLibraryDC[] ActivityLibraryGet(CWF.DataContracts.ActivityLibraryDC request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/ApplicationsGet", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/ApplicationsGetResponse")]
        CWF.DataContracts.ApplicationsGetReplyDC ApplicationsGet(CWF.DataContracts.ApplicationsGetRequestDC request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/StatusCodeGet", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/StatusCodeGetResponse")]
        CWF.DataContracts.StatusCodeGetReplyDC StatusCodeGet(CWF.DataContracts.StatusCodeGetRequestDC request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/StoreActivitiesGet", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/StoreActivitiesGetResponse")]
        CWF.DataContracts.StoreActivitiesDC[] StoreActivitiesGet(CWF.DataContracts.StoreActivitiesDC request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/WorkflowTypeGet", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/WorkflowTypeGetResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/WorkflowTypeGetServiceFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        CWF.DataContracts.WorkflowTypeGetReplyDC WorkflowTypeGet();

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/GetNextVersion", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/GetNextVersionResponse")]
        System.Version GetNextVersion(CWF.DataContracts.StoreActivitiesDC request, string userName);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/GetMissingActivityLibraries", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/GetMissingActivityLibrariesRespon" +
            "se")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ValidationFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/GetMissingActivityLibrariesValida" +
            "tionFaultFault", Name = "ValidationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/GetMissingActivityLibrariesServic" +
            "eFaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        CWF.DataContracts.GetMissingActivityLibrariesReply GetMissingActivityLibraries(CWF.DataContracts.GetMissingActivityLibrariesRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/CreateWorkflowTemplate", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/CreateWorkflowTemplateResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/CreateWorkflowTemplateServiceFaul" +
            "tFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        void CreateWorkflowTemplate(string workflowName, string version, string publishingWorkflowName, string publishingWorkflowVersion, string authGroupName, string insertedBy);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/IsWorkflowTemplate", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/IsWorkflowTemplateResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/IsWorkflowTemplateServiceFaultFau" +
            "lt", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        bool IsWorkflowTemplate(string workflowName, string version);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/SearchActivities", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/SearchActivitiesResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ValidationFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/SearchActivitiesValidationFaultFa" +
            "ult", Name = "ValidationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/SearchActivitiesServiceFaultFault" +
            "", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        CWF.DataContracts.ActivitySearchReplyDC SearchActivities(CWF.DataContracts.ActivitySearchRequestDC request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/SearchMarketplace", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/SearchMarketplaceResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/SearchMarketplaceServiceFaultFaul" +
            "t", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ValidationFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/SearchMarketplaceValidationFaultF" +
            "ault", Name = "ValidationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        CWF.DataContracts.Marketplace.MarketplaceSearchResult SearchMarketplace(CWF.DataContracts.Marketplace.MarketplaceSearchQuery request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/GetMarketplaceAssetDetails", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/GetMarketplaceAssetDetailsRespons" +
            "e")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ValidationFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/GetMarketplaceAssetDetailsValidat" +
            "ionFaultFault", Name = "ValidationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        [System.ServiceModel.FaultContractAttribute(typeof(Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault), Action = "http://cwf.microsoft.com/IWorkflowsQueryService/GetMarketplaceAssetDetailsService" +
            "FaultFault", Name = "ServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Support.Workflow.Service.Contra" +
            "cts.FaultContracts")]
        CWF.DataContracts.Marketplace.MarketplaceAssetDetails GetMarketplaceAssetDetails(CWF.DataContracts.Marketplace.MarketplaceSearchDetail request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/StoreActivitiesSetLock", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/StoreActivitiesSetLockResponse")]
        CWF.DataContracts.StatusReplyDC StoreActivitiesSetLock(CWF.DataContracts.StoreActivitiesDC request, System.DateTime lockedTime);

        [System.ServiceModel.OperationContractAttribute(Action = "http://cwf.microsoft.com/IWorkflowsQueryService/StoreActivitiesGetByName", ReplyAction = "http://cwf.microsoft.com/IWorkflowsQueryService/StoreActivitiesGetByNameResponse")]
        CWF.DataContracts.StoreActivitiesDC[] StoreActivitiesGetByName(CWF.DataContracts.StoreActivitiesDC request);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IWorkflowsQueryServiceChannelDevBranch : IWorkflowsQueryServiceDevBranch, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WorkflowsQueryServiceClientDevBranch : System.ServiceModel.ClientBase<IWorkflowsQueryServiceDevBranch>, IWorkflowsQueryServiceDevBranch
    {

        public WorkflowsQueryServiceClientDevBranch()
        {
        }

        public WorkflowsQueryServiceClientDevBranch(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public WorkflowsQueryServiceClientDevBranch(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public WorkflowsQueryServiceClientDevBranch(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public WorkflowsQueryServiceClientDevBranch(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public CWF.DataContracts.StoreActivitiesDC[] UploadActivityLibraryAndDependentActivities(CWF.DataContracts.StoreLibraryAndActivitiesRequestDC request)
        {
            return base.Channel.UploadActivityLibraryAndDependentActivities(request);
        }

        public CWF.DataContracts.GetAllActivityLibrariesReplyDC GetAllActivityLibraries(CWF.DataContracts.GetAllActivityLibrariesRequestDC request)
        {
            return base.Channel.GetAllActivityLibraries(request);
        }

        public CWF.DataContracts.PublishingReply PublishWorkflow(CWF.DataContracts.PublishingRequest request)
        {
            return base.Channel.PublishWorkflow(request);
        }

        public string GetExtensionUri()
        {
            return base.Channel.GetExtensionUri();
        }

        public CWF.DataContracts.GetActivitiesByActivityLibraryNameAndVersionReplyDC GetActivitiesByActivityLibraryNameAndVersion(CWF.DataContracts.GetActivitiesByActivityLibraryNameAndVersionRequestDC request)
        {
            return base.Channel.GetActivitiesByActivityLibraryNameAndVersion(request);
        }

        public CWF.DataContracts.GetPublishingWorkFlowByWorkFlowTypeReplyDC GetPublishingWorkFlowByWorkFlowType(CWF.DataContracts.GetPublishingWorkFlowByWorkFlowTypeRequestDC request)
        {
            return base.Channel.GetPublishingWorkFlowByWorkFlowType(request);
        }

        public CWF.DataContracts.StoreActivityLibrariesDependenciesDC StoreActivityLibraryDependencyList(CWF.DataContracts.StoreActivityLibrariesDependenciesDC request)
        {
            return base.Channel.StoreActivityLibraryDependencyList(request);
        }

        public CWF.DataContracts.StoreActivityLibrariesDependenciesDC[] StoreActivityLibraryDependenciesTreeGet(CWF.DataContracts.StoreActivityLibrariesDependenciesDC request)
        {
            return base.Channel.StoreActivityLibraryDependenciesTreeGet(request);
        }

        public CWF.DataContracts.ActivityCategoryCreateOrUpdateReplyDC ActivityCategoryCreateOrUpdate(CWF.DataContracts.ActivityCategoryCreateOrUpdateRequestDC request)
        {
            return base.Channel.ActivityCategoryCreateOrUpdate(request);
        }

        public CWF.DataContracts.ActivityCategoryByNameGetReplyDC[] ActivityCategoryGet(CWF.DataContracts.ActivityCategoryByNameGetRequestDC request)
        {
            return base.Channel.ActivityCategoryGet(request);
        }

        public CWF.DataContracts.ActivityLibraryDC ActivityLibraryCreateOrUpdate(CWF.DataContracts.ActivityLibraryDC request)
        {
            return base.Channel.ActivityLibraryCreateOrUpdate(request);
        }

        public CWF.DataContracts.ActivityLibraryDC[] ActivityLibraryGet(CWF.DataContracts.ActivityLibraryDC request)
        {
            return base.Channel.ActivityLibraryGet(request);
        }

        public CWF.DataContracts.ApplicationsGetReplyDC ApplicationsGet(CWF.DataContracts.ApplicationsGetRequestDC request)
        {
            return base.Channel.ApplicationsGet(request);
        }

        public CWF.DataContracts.StatusCodeGetReplyDC StatusCodeGet(CWF.DataContracts.StatusCodeGetRequestDC request)
        {
            return base.Channel.StatusCodeGet(request);
        }

        public CWF.DataContracts.StoreActivitiesDC[] StoreActivitiesGet(CWF.DataContracts.StoreActivitiesDC request)
        {
            return base.Channel.StoreActivitiesGet(request);
        }

        public CWF.DataContracts.WorkflowTypeGetReplyDC WorkflowTypeGet()
        {
            return base.Channel.WorkflowTypeGet();
        }

        public System.Version GetNextVersion(CWF.DataContracts.StoreActivitiesDC request, string userName)
        {
            return base.Channel.GetNextVersion(request, userName);
        }

        public CWF.DataContracts.GetMissingActivityLibrariesReply GetMissingActivityLibraries(CWF.DataContracts.GetMissingActivityLibrariesRequest request)
        {
            return base.Channel.GetMissingActivityLibraries(request);
        }

        public void CreateWorkflowTemplate(string workflowName, string version, string publishingWorkflowName, string publishingWorkflowVersion, string authGroupName, string insertedBy)
        {
            base.Channel.CreateWorkflowTemplate(workflowName, version, publishingWorkflowName, publishingWorkflowVersion, authGroupName, insertedBy);
        }

        public bool IsWorkflowTemplate(string workflowName, string version)
        {
            return base.Channel.IsWorkflowTemplate(workflowName, version);
        }

        public CWF.DataContracts.ActivitySearchReplyDC SearchActivities(CWF.DataContracts.ActivitySearchRequestDC request)
        {
            return base.Channel.SearchActivities(request);
        }

        public CWF.DataContracts.Marketplace.MarketplaceSearchResult SearchMarketplace(CWF.DataContracts.Marketplace.MarketplaceSearchQuery request)
        {
            return base.Channel.SearchMarketplace(request);
        }

        public CWF.DataContracts.Marketplace.MarketplaceAssetDetails GetMarketplaceAssetDetails(CWF.DataContracts.Marketplace.MarketplaceSearchDetail request)
        {
            return base.Channel.GetMarketplaceAssetDetails(request);
        }

        public CWF.DataContracts.StatusReplyDC StoreActivitiesSetLock(CWF.DataContracts.StoreActivitiesDC request, System.DateTime lockedTime)
        {
            return base.Channel.StoreActivitiesSetLock(request, lockedTime);
        }

        public CWF.DataContracts.StoreActivitiesDC[] StoreActivitiesGetByName(CWF.DataContracts.StoreActivitiesDC request)
        {
            return base.Channel.StoreActivitiesGetByName(request);
        }
    }
}
