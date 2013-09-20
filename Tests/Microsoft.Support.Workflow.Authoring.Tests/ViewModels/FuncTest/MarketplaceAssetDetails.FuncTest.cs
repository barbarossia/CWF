using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels.Marketplace;
using CWF.DataContracts.Marketplace;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Security.Principal;
using Microsoft.Support.Workflow.Authoring.Security;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels
{
    [TestClass]
    public class MarketplaceAssetDetailsFuncationalTest
    {
        [WorkItem(313066)]
        [Description("Verify detail view load successfully.")]
        [Owner("v-jerzha")]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyLaunchAssetDetailView()
        {
            TestUtilities.RegistLoginUserRole(Role.Admin);
            MarketplaceViewModel model = new MarketplaceViewModel(null);
            model.SearchText = "Publish";

            model.SearchCommand.Execute();
            if (model.ResultList.Count > 0)
            {
                IWorkflowsQueryService client = new WorkflowsQueryServiceClient();
                MarketplaceAssetDetails details = null;
                MarketplaceSearchDetail search = new MarketplaceSearchDetail();

                search.AssetType = model.ResultList[0].AssetType;
                search.Id = model.ResultList[0].Id;
                details = client.GetMarketplaceAssetDetails(search);
                Assert.IsTrue(details.Name.Contains(model.SearchText));
            }

        }
    }
}
