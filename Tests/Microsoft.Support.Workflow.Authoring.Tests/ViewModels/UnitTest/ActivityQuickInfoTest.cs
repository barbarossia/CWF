using CWF.DataContracts.Marketplace;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.Serialization;
using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;
using CWF.DataContracts;
using CWF.WorkflowQueryService.Versioning;
using CWF.BAL.Versioning;
using Microsoft.Support.Workflow.Authoring.Models;
using System.Activities.Statements;
using Microsoft.Support.Workflow.Authoring.Behaviors;
using Microsoft.Support.Workflow.Authoring.UIControls;

namespace Microsoft.Support.Workflow.Authoring.Tests
{
    
    
    /// <summary>
    ///This is a test class for ActivityQuickInfoTest and is intended
    ///to contain all ActivityQuickInfoTest Unit Tests
    ///</summary>
    [TestClass]
    public class ActivityQuickInfoUnitTest
    {       
        /// <summary>
        ///A test for properties in ActivityQuickInfo
        ///</summary>
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void ActivityQuickInfo_PropertiesTest()
        {
            ActivityQuickInfo target = new ActivityQuickInfo(); // TODO: Initialize to an appropriate value
            
            //Id
            long idExpected = 10; // TODO: Initialize to an appropriate value
            long idActual;
            target.Id = idExpected;
            idActual = target.Id;
            Assert.AreEqual(idExpected, idActual);           

            //Name
            string NameExpected = "ExpectData"; // TODO: Initialize to an appropriate value
            string NameActual;
            target.Name = NameExpected;
            NameActual = target.Name;
            Assert.AreEqual(NameExpected, NameActual);
           
            //Version
            string versionExpected = "v 1.0.0.0"; // TODO: Initialize to an appropriate value
            string versionActual;
            target.Version = versionExpected;
            versionActual = target.Version;
            Assert.AreEqual(versionExpected, versionActual);
        }    
    }

    [TestClass]
    public class MarketplaceAssetUnitTest
    {
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MarketplaceAsset_PropertiesTest()
        {
            MarketplaceAsset target = new MarketplaceAsset();

            //UpdatedBy
            string updatedBy_expected = "User";
            string updatedBy_Actual;
            target.UpdatedBy = updatedBy_expected;
            updatedBy_Actual = target.UpdatedBy;
            Assert.AreEqual(updatedBy_expected,updatedBy_Actual);

            //IsPublishingWorkflow
            bool? isPublishingWorkflow_Expected = false;
            bool? isPublishingWorkflow_Actual;
            target.IsPublishingWorkflow = isPublishingWorkflow_Expected;
            isPublishingWorkflow_Actual = target.IsPublishingWorkflow;
            Assert.AreEqual(isPublishingWorkflow_Expected,isPublishingWorkflow_Actual);

            //IsTemplate
            bool? isTemplate_Expected = false;
            bool? isTemplate_Actual;
            target.IsTemplate = isTemplate_Expected;
            isTemplate_Actual = target.IsTemplate;
            Assert.AreEqual(isTemplate_Expected, isTemplate_Actual);
        }
    }

    [TestClass]
    public class MarketplaceSearchDetailUnitTest
    {
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MarketplaceSearchDetail_PropertiesTest()
        {
            MarketplaceSearchDetail target = new MarketplaceSearchDetail();
            //AssetType
            AssetType assetType_expected = AssetType.Activities;
            AssetType assetType_actual;
            target.AssetType = assetType_expected;
            assetType_actual = target.AssetType;
            Assert.AreEqual(assetType_expected,assetType_actual);

            //Id
            long id_expected = 10;
            long id_actual;
            target.Id = id_expected;
            id_actual = target.Id;
            Assert.AreEqual(id_expected,id_actual);
        }  
    }

    [TestClass]
    public class SortCriterionUnitTest
    {
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void SortCriterion_PropertiesTest()
        {
            SortCriterion target = new SortCriterion();
            //FieldName
            string feildName_expected = "expected feildName";
            string feildName_actual;
            target.FieldName = feildName_expected;
            feildName_actual = target.FieldName;
            Assert.AreEqual(feildName_expected,feildName_actual);

            //IsAscending
            bool isAscending_expected = false;
            bool isAscending_actual;
            target.IsAscending = isAscending_expected;
            isAscending_actual = target.IsAscending;
            Assert.AreEqual(isAscending_expected,isAscending_actual);
        }
    }

    [TestClass]
    public class MarketplaceSearchQueryUnitTest
    {
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MarketplaceSearchQuery_PropertiesTest()
        {
            MarketplaceSearchQuery target = new MarketplaceSearchQuery();
            //FilterType
            MarketplaceFilter filterType_expected = MarketplaceFilter.Activities;
            MarketplaceFilter filterType_actual;
            target.FilterType = filterType_expected;
            filterType_actual = target.FilterType;
            Assert.AreEqual(filterType_expected,filterType_actual);

            //IsNewest
            bool isnewest_expected = false;
            bool isnewest_actual;
            target.IsNewest = isnewest_expected;
            isnewest_actual = target.IsNewest;
            Assert.AreEqual(isnewest_expected,isnewest_actual);

            //PageNumber
            int pageNumber_expected = 10;
            int pageNumbe_actual;
            target.PageNumber = pageNumber_expected;
            pageNumbe_actual = target.PageNumber;
            Assert.AreEqual(pageNumber_expected,pageNumbe_actual);

            //PageSize
            int pageSize_expected = 10;
            int pageSize_Actual;
            target.PageSize = pageSize_expected;
            pageSize_Actual = target.PageSize;
            Assert.AreEqual(pageSize_expected,pageSize_Actual);

            //SearchText
            string searchText_expected = "search text";
            string searchText_actual;
            target.SearchText = searchText_expected;
            searchText_actual = target.SearchText;
            Assert.AreEqual(searchText_expected,searchText_actual);

            //UserRole
            string userRole_expected = "userrole";
            string userRole_actual;
            target.UserRole = userRole_expected;
            userRole_actual = target.UserRole;
            Assert.AreEqual(userRole_expected,userRole_actual);

        }
    }

    [TestClass]
    public class ServiceFaultUnitTest
    {
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void ServiceFault_CtorTest()
        {
            ServiceFault target = new ServiceFault();
            //ErrorCode
            int errorCode_Expected = 10;
            int errorCode_Actual;
            target.ErrorCode = errorCode_Expected;
            errorCode_Actual = target.ErrorCode;
            Assert.AreEqual(errorCode_Expected,errorCode_Actual);

            //ErrorMessage
            string msg_expected = "error";
            string msg_actual;
            target.ErrorMessage = msg_expected;
            msg_actual = target.ErrorMessage;
            Assert.AreEqual(msg_expected,msg_actual);
        }
    }

    [TestClass]
    public class AuthorizationGroupGetRequestDCUnitTest
    {
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void AuthorizationGroupGetRequestDC_CtorTest()
        {
            AuthorizationGroupGetRequestDC target = new AuthorizationGroupGetRequestDC();
            //AuthGroupId
            int authGroupId_Expected = 10;
            int authGroupId_Actual;
            target.AuthGroupId = authGroupId_Expected;
            authGroupId_Actual = target.AuthGroupId;
            Assert.AreEqual(authGroupId_Expected,authGroupId_Actual);

            //AuthGroupName
            string authGroupName_expected = "my group";
            string authGroupname_actual;
            target.AuthGroupName = authGroupName_expected;
            authGroupname_actual = target.AuthGroupName;
            Assert.AreEqual(authGroupName_expected,authGroupname_actual);

            //Guid
            string guid_Expected = "guid";
            string guid_Actual;
            target.Guid = guid_Expected;
            guid_Actual = target.Guid;
            Assert.AreEqual(guid_Expected,guid_Actual);
        }

        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void WorkFlowTypeCreateOrUpdateRequestDC_CtorTest()
        {
            WorkFlowTypeCreateOrUpdateRequestDC target = new WorkFlowTypeCreateOrUpdateRequestDC();
            //InAuthGroupId
            target.InAuthGroupId = 10;
            Assert.AreEqual(10,target.InAuthGroupId);
            //InHandleVariable
            target.InHandleVariable = "InHandleVariable";
            Assert.AreEqual("InHandleVariable",target.InHandleVariable);
            //InId
            target.InId = 10;
            Assert.AreEqual(10,target.InId);
            //InPageViewVariable
            target.InPageViewVariable = "InPageViewVariable";
            Assert.AreEqual("InPageViewVariable",target.InPageViewVariable);
            //InPublishingWorkflowId
            target.InPublishingWorkflowId = 10;
            Assert.AreEqual(10,target.InPublishingWorkflowId);
            //InSelectionWorkflowId
            target.InSelectionWorkflowId = 10;
            Assert.AreEqual(10,target.InSelectionWorkflowId);
            //InWorkflowGroup
            target.InWorkflowGroup = 10;
            Assert.AreEqual(10,target.InWorkflowGroup);
            //InWorkflowTemplateId
            target.InWorkflowTemplateId = 10;
            Assert.AreEqual(10,target.InWorkflowTemplateId);
            //InWorkflowTypeid
            target.InWorkflowTypeid = "InWorkflowTypeid";
            Assert.AreEqual("InWorkflowTypeid",target.InWorkflowTypeid);
            //IsDeleted
            target.IsDeleted = true;
            Assert.AreEqual(true,target.IsDeleted);
        }

        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void ActivitySearchRequestDC_PropertyTest()
        {
            ActivitySearchRequestDC target = new ActivitySearchRequestDC();
            //FilterByCreator
            target.FilterByCreator = true;
            Assert.AreEqual(true,target.FilterByCreator);
            //FilterByDescription
            target.FilterByDescription = true;
            Assert.AreEqual(true,target.FilterByDescription);
            //FilterByName
            target.FilterByName = true;
            Assert.AreEqual(true,target.FilterByName);
            //FilterByTags
            target.FilterByTags = true;
            Assert.AreEqual(true,target.FilterByTags);
            //FilterByType
            target.FilterByType = true;
            Assert.AreEqual(true,target.FilterByType);
            //FilterByVersion
            target.FilterByVersion = true;
            Assert.AreEqual(true,target.FilterByVersion);
            //FilterOlder
            target.FilterOlder = true;
            Assert.AreEqual(true,target.FilterOlder);
            //PageNumber
            target.PageNumber = 10;
            Assert.AreEqual(10,target.PageNumber);
            //PageSize
            target.PageSize = 10;
            Assert.AreEqual(10,target.PageSize);
            //SearchText
            target.SearchText = "SearchText";
            Assert.AreEqual("SearchText",target.SearchText);
            //SortAscending
            target.SortAscending = true;
            Assert.AreEqual(true,target.SortAscending);
            //SortColumn
            target.SortColumn = "SortColumn";
            Assert.AreEqual("SortColumn",target.SortColumn);
        }

        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void AuthorizationGroupDC_PropertyTest()
        {
            AuthorizationGroupDC target = new AuthorizationGroupDC();
            //AuthGroupName
            target.AuthGroupName = "AuthGroupName";
            Assert.AreEqual("AuthGroupName",target.AuthGroupName);            
        }

        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void WorkflowTypeSearchRequest_PropertyTest()
        {
            WorkflowTypeSearchRequest target = new WorkflowTypeSearchRequest();
            //PageNumber
            target.PageNumber = 10;
            Assert.AreEqual(10,target.PageNumber);
            //PageSize
            target.PageSize = 10;
            Assert.AreEqual(10,target.PageSize);
            //SearchText
            target.SearchText = "SearchText";
            Assert.AreEqual("SearchText",target.SearchText);
            //SortAscending
            target.SortAscending = true;
            Assert.AreEqual(true,target.SortAscending);
            //SortColumn
            target.SortColumn = "SortColumn";
            Assert.AreEqual("SortColumn", target.SortColumn);
        }

        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void VersionFault_PropertyTest()
        {
            VersionFault target = new VersionFault();
            target.Message = "Message";
            Assert.AreEqual("Message",target.Message);
            target.Rule =new CWF.BAL.Versioning.Rule();
            Assert.IsNotNull(target.Rule);
        }

        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void Rule_PropertyTest()
        {
            Rule target = new Rule();
            //ActionType
            target.ActionType = RequestedOperation.AddNew;
            Assert.AreEqual(RequestedOperation.AddNew,target.ActionType);
            //BuildRequiredChange
            target.BuildRequiredChange = RequiredChange.MustChange;
            Assert.AreEqual(RequiredChange.MustChange,target.BuildRequiredChange);
            //IsPublic
            target.IsPublic = false;
            Assert.AreEqual(false,target.IsPublic);
            //IsRetired
            target.IsRetired = false;
            Assert.AreEqual(false,target.IsRetired);
            //MajorRequiredChange
            target.MajorRequiredChange = RequiredChange.MustIncrement;
            Assert.AreEqual(RequiredChange.MustIncrement,target.MajorRequiredChange);
            //MinorRequiredChange
            target.MinorRequiredChange = RequiredChange.MustReset;
            Assert.AreEqual(RequiredChange.MustReset,target.MinorRequiredChange);
            //NameRequiredChange
            target.NameRequiredChange = RequiredChange.NoActionRequired;
            Assert.AreEqual(RequiredChange.NoActionRequired,target.NameRequiredChange);
            //RevisionRequiredChange
            target.RevisionRequiredChange = RequiredChange.NoActionRequired;
            Assert.AreEqual(RequiredChange.NoActionRequired,target.RevisionRequiredChange);
        }

        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MarketplaceAssetModel_PropertyTest()
        {
            MarketplaceAssetModel target = new MarketplaceAssetModel();
            target.Location = "Location";
            Assert.AreEqual("Location",target.Location);
        }

        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void WorkflowItem_PropertyTest()
        {
            WorkflowItem target = new WorkflowItem("MyWorkFlow", "MyWorkFlow", new Sequence().ToXaml(), string.Empty);
            //IsLoadingDesigner
            target.IsLoadingDesigner = false;
            Assert.AreEqual(false,target.IsLoadingDesigner);
            //HasMajorChanged
            target.HasMajorChanged = false;
            Assert.AreEqual(false,target.HasMajorChanged);
            //PrintState
            target.PrintState = PrintAction.NoneAction;
            Assert.AreEqual(PrintAction.NoneAction,target.PrintState);
        }

        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void DataPaging_PropertyTest()
        {
            DataPaging target = new DataPaging();
            //AvailablePagesCount
            target.AvailablePagesCount = 10;
            Assert.AreEqual(10,target.AvailablePagesCount);
            //PageSize
            target.PageSize = 10;
            Assert.AreEqual(10,target.PageSize);
        }

        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MarketplaceAssetDetails_PropertyTest()
        {
            MarketplaceAssetDetails target = new MarketplaceAssetDetails();
            //ID
            target.Id = 10;
            Assert.AreEqual(10,target.Id);
            //ThumbnailUrl
            target.ThumbnailUrl = "ThumbnailUrl";
            Assert.AreEqual("ThumbnailUrl", target.ThumbnailUrl);
        }
    }

    [TestClass]
    public class ViewUnitTest
    {
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void SearchTextBox_IconBorder_MouseLeftButtonDownTest()
        {
            SearchTextBox target = new SearchTextBox();
            PrivateObject po = new PrivateObject(target);
            //po.Invoke("IconBorder_MouseLeftButtonDown", null, null);
            SearchTextBox_Accessor accessor = new SearchTextBox_Accessor(po);
            accessor.IconBorder_MouseLeftButtonDown(null, null);
            Assert.AreEqual(true,target.IsMouseLeftButtonDown);
        }

        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void SearchTextBox_IconBorder_MouseLeftButtonUpTest()
        {
            SearchTextBox target = new SearchTextBox();
            target.IsMouseLeftButtonDown = false;
            PrivateObject po = new PrivateObject(target);            
            SearchTextBox_Accessor accessor = new SearchTextBox_Accessor(po);
            accessor.IconBorder_MouseLeftButtonUp(null,null);

            target.HasText = true;
            target.SearchMode = SearchMode.Instant;
            accessor.IconBorder_MouseLeftButtonUp(null,null);
            Assert.AreEqual("",target.Text);

            target.SearchMode = SearchMode.Delayed;
            accessor.IconBorder_MouseLeftButtonUp(null,null);
            Assert.AreEqual(false,target.IsMouseLeftButtonDown);
        }

        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void SearchTextBox_IconBorder_MouseLeaveTest()
        {
            SearchTextBox target = new SearchTextBox();
            target.IsMouseLeftButtonDown = true;
            PrivateObject po = new PrivateObject(target);
            SearchTextBox_Accessor accessor = new SearchTextBox_Accessor(po);
            accessor.IconBorder_MouseLeave(null,null);
            Assert.AreEqual(false,target.IsMouseLeftButtonDown);
            
        }
    }
}
