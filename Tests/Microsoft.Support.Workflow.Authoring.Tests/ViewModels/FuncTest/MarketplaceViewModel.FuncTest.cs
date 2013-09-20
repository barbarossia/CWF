using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels.Marketplace;
using CWF.DataContracts.Marketplace;
using System.Collections;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.DynamicImplementations;
using System.Windows;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Security.Principal;
using Microsoft.Support.Workflow.Authoring.Security;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels
{
    /// <summary>
    /// Functional Test for Marketplace Searching
    /// </summary>
    [TestClass]
    public class MarketplaceViewModelFuncationalTest
    {
        [WorkItem(313067)]
        [Description("Verify search feature work well.")]
        [Owner("v-jerzha")]
        [TestCategory("Func-NoDif2-Smoke")]
        [TestMethod()]
        public void VerifySearchMarketplace()
        {
            TestUtilities.RegistLoginUserRole(Role.Admin);
            MarketplaceViewModel model = new MarketplaceViewModel(null);
            model.SearchText = "Publish";
            model.SearchCommand.Execute();

            IWorkflowsQueryService client = new WorkflowsQueryServiceClient();
            MarketplaceAssetDetails details = null;

            foreach (MarketplaceAssetModel assetItem in model.ResultList)
            {
                MarketplaceSearchDetail search = new MarketplaceSearchDetail();
                search.AssetType = assetItem.AssetType;
                search.Id = assetItem.Id;
                details = client.GetMarketplaceAssetDetails(search);
                Assert.IsTrue(details.Name.Contains(model.SearchText));
            }
        }

    }
}
