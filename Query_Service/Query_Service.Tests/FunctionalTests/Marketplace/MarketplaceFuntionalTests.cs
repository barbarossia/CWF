using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts.Marketplace;
using Query_Service.Tests.Common;

namespace Query_Service.Tests.FunctionalTests.Marketplace
{
    /// <summary>
    /// Summary description for MarketplaceFuntionalTests
    /// </summary>
    [TestClass]
    public class MarketplaceFuntionalTests : QueryServiceTestBase
    {
        private MarketplaceSearchQuery searchRequest;
        private MarketplaceSearchDetail assetSearchRequest;

        #region Private Methods
        /// <summary>
        /// Generate MarketplaceSearch Query instance
        /// </summary>
        /// <param name="searchText">Text for searching</param>
        /// <param name="filterType">Filter type</param>
        /// <param name="userRole">User Role (Admin / Author)</param>
        /// <param name="pageNumber">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>MarketplaceSearchQuery</returns>
        private MarketplaceSearchQuery GenerateSearchRequest(string searchText, MarketplaceFilter filterType, string userRole = "Admin", int pageNumber = 1, int pageSize = 15)
        {
            MarketplaceSearchQuery query = new MarketplaceSearchQuery();

            SortCriterion sortCriterion = new SortCriterion();
            sortCriterion.FieldName = "Name";
            sortCriterion.IsAscending = true;

            query.UserRole = userRole;
            query.FilterType = filterType;
            query.SearchText = searchText;
            query.PageNumber = pageNumber;
            query.PageSize = pageSize;
            query.IsNewest = true;
            query.SortCriteria = new List<SortCriterion>() { sortCriterion };

            return query;
        }

        /// <summary>
        /// Generate MarketplaceAssetSearchDetail
        /// </summary>
        /// <param name="activityLibraryId">The id of the activity or project</param>
        /// <param name="assetType">Asset Type</param>
        /// <returns>MarketplaceSearchDetail</returns>
        private MarketplaceSearchDetail GenerateAssetSearchRequest(long id, AssetType assetType = AssetType.Activities)
        {
            MarketplaceSearchDetail request = new MarketplaceSearchDetail();
            request.Id = id;
            request.AssetType = assetType;

            return request;
        }

        /// <summary>
        /// Calling Query service for searching marketplace
        /// </summary>
        /// <param name="request">MarketplaceSearchQuery</param>
        /// <returns>MarketplaceSearchResult</returns>
        private MarketplaceSearchResult SearchMarketplace(MarketplaceSearchQuery request)
        {
            return devBranchProxy.SearchMarketplace(request);
        }

        /// <summary>
        /// Get the marketplace asset details
        /// </summary>
        /// <param name="assetSearchRequest">MarketplaceSearchDetail</param>
        /// <returns>MarketplaceAssetDetails</returns>
        private MarketplaceAssetDetails GetMarketplaceAssetDetails(MarketplaceSearchDetail assetSearchRequest)
        {
            return devBranchProxy.GetMarketplaceAssetDetails(assetSearchRequest);
        }

        #endregion

        [WorkItem(313114)]
        [TestMethod()]
        [Owner("v-toy")]
        [Description("Verify searching projects or activities for all from marketplace")]
        [TestCategory(TestCategory.Full)]
        public void VerifySearchMarketplaceForAll()
        {
            string searchText = "Publishing";

            //Create request
            searchRequest = GenerateSearchRequest(searchText, MarketplaceFilter.None);
            //Send request
            MarketplaceSearchResult result = SearchMarketplace(searchRequest);
            //Verification
            Assert.AreEqual(result.Items.Count, 3, "Search result incorrect.");
            Assert.AreEqual(result.Items[0].Name, "Publishing", "Failed to search Marketplace.");
            Assert.AreEqual(result.Items[1].Name, "PublishingInfo", "Failed to search Marketplace.");
            Assert.AreEqual(result.Items[2].Name, "PublishingWorkflow_WorkflowLibrary", "Failed to search Marketplace.");
        }

        [WorkItem(313115)]
        [TestMethod()]
        [Owner("v-toy")]
        [Description("Verify searching projects from marketplace")]
        [TestCategory(TestCategory.Full)]
        public void VerifySearchMarketplaceForProjects()
        {
            string searchText = "Publishing";

            //Create request
            searchRequest = GenerateSearchRequest(searchText, MarketplaceFilter.Projects);
            //Send request
            MarketplaceSearchResult result = SearchMarketplace(searchRequest);
            //Verification
            Assert.AreEqual(result.Items[0].Name, "PublishingWorkflow_WorkflowLibrary", "Failed to search Marketplace.");
        }

        [WorkItem(313116)]
        [TestMethod()]
        [Owner("v-toy")]
        [Description("Verify searching activities from marketplace")]
        [TestCategory(TestCategory.Full)]
        public void VerifySearchMarketplaceForActivities()
        {
            string searchText = "Publishing";

            //Create request
            searchRequest = GenerateSearchRequest(searchText, MarketplaceFilter.Activities);
            //Send request
            MarketplaceSearchResult result = SearchMarketplace(searchRequest);
            //Verification
            Assert.AreEqual(result.Items.Count, 2, "Search result incorrect.");
            Assert.AreEqual(result.Items[0].Name, "Publishing", "Failed to search Marketplace.");
            Assert.AreEqual(result.Items[1].Name, "PublishingInfo", "Failed to search Marketplace.");
        }

        [WorkItem(313117)]
        [TestMethod()]
        [Owner("v-toy")]
        [Description("Verify searching templates from marketplace")]
        [TestCategory(TestCategory.Full)]
        public void VerifySearchMarketplaceForTemplates()
        {
            string searchText = "PageTemplate_WorkflowLibrary";

            //Create request
            searchRequest = GenerateSearchRequest(searchText, MarketplaceFilter.Templates);
            //Send request
            MarketplaceSearchResult result = SearchMarketplace(searchRequest);
            //Verification
            Assert.AreEqual(result.Items.Count, 1, "Search result incorrect.");
            Assert.AreEqual(result.Items[0].Name, "PageTemplate_WorkflowLibrary", "Failed to search Marketplace.");
            Assert.AreEqual(result.Items[0].IsTemplate, true, "Search result incorrect.");
        }

        [WorkItem(313118)]
        [TestMethod()]
        [Owner("v-toy")]
        [Description("Verify searching publishing from marketplace")]
        [TestCategory(TestCategory.Full)]
        public void VerifySearchMarketplaceForPublishing()
        {
            string searchText = "Publishing";

            //Create request
            searchRequest = GenerateSearchRequest(searchText, MarketplaceFilter.PublishingWorkflows);
            //Send request
            MarketplaceSearchResult result = SearchMarketplace(searchRequest);
            //Verification
            Assert.AreEqual(result.Items.Count, 1, "Search result incorrect.");
            Assert.AreEqual(result.Items[0].Name, "PublishingWorkflow_WorkflowLibrary", "Failed to search Marketplace.");
            Assert.AreEqual(result.Items[0].IsPublishingWorkflow, true, "Search result incorrect.");
        }

        [WorkItem(313119)]
        [TestMethod()]
        [Owner("v-toy")]
        [Description("Verify searching not exist item from marketplace")]
        [TestCategory(TestCategory.Full)]
        public void VerifySearchMarketplaceForNotExist()
        {
            string searchText = "Not_Exist_Workflow";

            //Create request
            searchRequest = GenerateSearchRequest(searchText, MarketplaceFilter.PublishingWorkflows);
            //Send request
            MarketplaceSearchResult result = SearchMarketplace(searchRequest);
            //Verification
            Assert.IsNull(result, "Search result incorrect.");
        }

        [WorkItem(313120)]
        [TestMethod()]
        [Owner("v-toy")]
        [Description("Verify geting asset details from marketplace")]
        [TestCategory(TestCategory.Full)]
        public void VerifyGetMarketplaceAssetDetails()
        {
            //This is the id of the publishing activity
            long assetId = 16;

            assetSearchRequest = GenerateAssetSearchRequest(assetId);
            MarketplaceAssetDetails result = GetMarketplaceAssetDetails(assetSearchRequest);

            Assert.IsNotNull(result, "Failed to get asset details");
            Assert.AreEqual(result.Id, assetId, "Failed to get publishing activity.");
            Assert.AreEqual(result.Name, "Publishing", "Failed to get publishing activity.");
            Assert.AreEqual(result.Activities.Count, 2, "Failed to get the contained activities.");
        }
    }
}
