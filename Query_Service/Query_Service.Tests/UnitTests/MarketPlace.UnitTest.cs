using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CWF.BAL.Versioning;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Service.BusinessServices;
using Microsoft.Support.Workflow.Service.DataAccessServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Service.Test.Common;
using CWF.BAL;

namespace Query_Service.UnitTests
{
    /// <summary>
    /// Unit tests for QueryService BAl and DAL layer
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "This not required")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Not required fot const/unit tests")]
    [TestClass]
    public class MarketPlaceUnitTest
    {
        [Description("Get the marketplace details")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void MarketplaceAssetDetailsGet()
        {
            var request = new CWF.DataContracts.Marketplace.MarketplaceSearchDetail();

            CWF.DataContracts.Marketplace.MarketplaceAssetDetails reply = null;

            request.Id = 0;
            request.AssetType = CWF.DataContracts.Marketplace.AssetType.Project;

            //// Get
            try
            {
                reply = MarketplaceRepositoryService.GetMarketplaceAssetDetails(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.MarketplaceRepositoryService.GetMarketplaceAssetDetails(request, get)");
            }

            Assert.IsNull(reply);
        }

        [Description("Get the marketplace result")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void SearchMarketplaceWithFilterNone()
        {
            var request = new CWF.DataContracts.Marketplace.MarketplaceSearchQuery();

            CWF.DataContracts.Marketplace.MarketplaceSearchResult reply = null;

            request.SearchText = "microsoft";
            request.FilterType = CWF.DataContracts.Marketplace.MarketplaceFilter.None;
            request.PageSize = 15;
            request.PageNumber = 1;
            request.UserRole = "Admin";
            request.SortCriteria = new List<CWF.DataContracts.Marketplace.SortCriterion>
                                    {
                                        new CWF.DataContracts.Marketplace.SortCriterion()
                                        {
                                            FieldName="Name",
                                            IsAscending=true,
                                    }};
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };

            //// Get
            try
            {
                reply = MarketplaceRepositoryService.SearchMarketplace(request);
                var detailRequest = new CWF.DataContracts.Marketplace.MarketplaceSearchDetail();
                CWF.DataContracts.Marketplace.MarketplaceAssetDetails detailReply = null;
                Assert.IsNotNull(reply);
                foreach (var item in reply.Items)
                {
                    detailRequest.Id = item.Id;
                    detailRequest.AssetType = item.AssetType;
                    detailReply = MarketplaceRepositoryService.GetMarketplaceAssetDetails(detailRequest);
                    Assert.IsNotNull(detailReply);
                }
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.MarketplaceRepositoryService.GetMarketplaceAssetDetails(request, get)");
            }

        }

        [Description("Get the marketplace result")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void SearchMarketplaceWithFilterActivities()
        {
            var request = new CWF.DataContracts.Marketplace.MarketplaceSearchQuery();

            CWF.DataContracts.Marketplace.MarketplaceSearchResult reply = null;

            request.SearchText = "microsoft";
            request.FilterType = CWF.DataContracts.Marketplace.MarketplaceFilter.Activities;
            request.PageSize = 15;
            request.PageNumber = 1;
            request.UserRole = "Admin";
            request.SortCriteria = new List<CWF.DataContracts.Marketplace.SortCriterion>
                                    {
                                        new CWF.DataContracts.Marketplace.SortCriterion()
                                        {
                                            FieldName="Name",
                                            IsAscending=true,
                                    }};
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };

            //// Get
            try
            {
                reply = MarketplaceRepositoryService.SearchMarketplace(request);
                Assert.IsNotNull(reply);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.MarketplaceRepositoryService.GetMarketplaceAssetDetails(request, get)");
            }

        }

        [Description("Get the marketplace result")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void SearchMarketplaceWithFilterTemplates()
        {
            var request = new CWF.DataContracts.Marketplace.MarketplaceSearchQuery();

            CWF.DataContracts.Marketplace.MarketplaceSearchResult reply = null;

            request.SearchText = "microsoft";
            request.FilterType = CWF.DataContracts.Marketplace.MarketplaceFilter.Templates;
            request.PageSize = 15;
            request.PageNumber = 1;
            request.UserRole = "Author";
            request.SortCriteria = new List<CWF.DataContracts.Marketplace.SortCriterion>
                                    {
                                        new CWF.DataContracts.Marketplace.SortCriterion()
                                        {
                                            FieldName="Name",
                                            IsAscending=true,
                                    }};
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };

            //// Get
            try
            {
                reply = MarketplaceRepositoryService.SearchMarketplace(request);
                Assert.IsNotNull(reply);
                Assert.IsTrue(reply.StatusReply.Errorcode == SprocValues.INVALID_PARMETER_VALUE_INCODE_ID);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.MarketplaceRepositoryService.GetMarketplaceAssetDetails(request, get)");
            }

        }

        [Description("Get the marketplace result")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void SearchMarketplaceWithFilterPublish()
        {
            var request = new CWF.DataContracts.Marketplace.MarketplaceSearchQuery();

            CWF.DataContracts.Marketplace.MarketplaceSearchResult reply = null;

            request.SearchText = "microsoft";
            request.FilterType = CWF.DataContracts.Marketplace.MarketplaceFilter.PublishingWorkflows;
            request.PageSize = 15;
            request.PageNumber = 1;
            request.UserRole = "Author";
            request.SortCriteria = new List<CWF.DataContracts.Marketplace.SortCriterion>
                                    {
                                        new CWF.DataContracts.Marketplace.SortCriterion()
                                        {
                                            FieldName="Name",
                                            IsAscending=true,
                                    }};
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };

            //// Get
            try
            {
                reply = MarketplaceRepositoryService.SearchMarketplace(request);
                Assert.IsNotNull(reply);
                Assert.IsTrue(reply.StatusReply.Errorcode == SprocValues.INVALID_PARMETER_VALUE_INCODE_ID);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.MarketplaceRepositoryService.GetMarketplaceAssetDetails(request, get)");
            }
        }

        [Description("Get the marketplace result")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void SearchMarketplaceWithFilterWrongCritieria()
        {
            var request = new CWF.DataContracts.Marketplace.MarketplaceSearchQuery();

            CWF.DataContracts.Marketplace.MarketplaceSearchResult reply = null;

            request.SearchText = "microsoft";
            request.FilterType = CWF.DataContracts.Marketplace.MarketplaceFilter.PublishingWorkflows;
            request.PageSize = 15;
            request.PageNumber = 1;
            request.UserRole = "";
            request.SortCriteria = new List<CWF.DataContracts.Marketplace.SortCriterion>
                                    {
                                        new CWF.DataContracts.Marketplace.SortCriterion()
                                        {
                                            FieldName="Name",
                                            IsAscending=true,
                                    }};
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };

            //// Get
            reply = MarketplaceRepositoryService.SearchMarketplace(request);
            Assert.IsTrue(reply.StatusReply.Errorcode == SprocValues.INVALID_PARMETER_VALUE_INCODE_ID);
        }
    }
}
