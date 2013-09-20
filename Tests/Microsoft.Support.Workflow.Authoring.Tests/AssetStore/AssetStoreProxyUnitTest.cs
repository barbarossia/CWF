using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Views;
using Microsoft.DynamicImplementations;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.AssetStore;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;

namespace Microsoft.Support.Workflow.Authoring.Tests.AssetStore
{
    [TestClass]
    public class AssetStoreProxyUnitTest
    {
        [WorkItem(325730)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void AssetStore_VerifyAssetStoreProxy()
        {
            using (var client = new Implementation<WorkflowsQueryServiceClient>())
            {
                client.Register(inst => inst.ActivityCategoryCreateOrUpdate(Argument<ActivityCategoryCreateOrUpdateRequestDC>.Any)).Execute(() =>
                {
                    ActivityCategoryCreateOrUpdateReplyDC reply = new ActivityCategoryCreateOrUpdateReplyDC();
                    reply.StatusReply = new StatusReplyDC() { Errorcode = 0 };
                    return reply;
                });
                client.Register(inst => inst.ActivityCategoryGet(Argument<ActivityCategoryByNameGetRequestDC>.Any)).Execute(() =>
                {

                    var reply = new List<ActivityCategoryByNameGetReplyDC>();
                    reply.Add(new ActivityCategoryByNameGetReplyDC { Name = "Admin" });
                    reply.Add(new ActivityCategoryByNameGetReplyDC { Name = "tool" });
                    return reply;
                });
                client.Register(inst => inst.WorkflowTypeGet(Argument<WorkflowTypesGetRequestDC>.Any)).Execute(() =>
                {
                    WorkflowTypeGetReplyDC reply = new WorkflowTypeGetReplyDC();
                    reply.WorkflowActivityType = new List<WorkflowTypesGetBase>();
                    return reply;
                });
                client.Register(inst => inst.TenantGet()).Return("test");

                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                //verify get woekflow types
                AssetStoreProxy.GetWorkflowTypes(Env.Test);
                Assert.IsTrue(AssetStoreProxy.WorkflowTypes.Count == 0);

                //verify get categories
                AssetStoreProxy.GetActivityCategories();
                ObservableCollection<string> source = AssetStoreProxy.ActivityCategories.Source as ObservableCollection<string>;
                Assert.IsTrue(source.Count == 3);

                //verify add new workflow category
                string categoryName = string.Empty;

                try { AssetStoreProxy.ActivityCategoryCreateOrUpdate(categoryName); }
                
                catch (ArgumentNullException expect) 
                {
                    Assert.AreEqual(expect.ParamName, "categoryName");
                }

                Assert.IsTrue(AssetStoreProxy.ActivityCategoryCreateOrUpdate("newcategory"));
                Assert.AreEqual(AssetStoreProxy.TenantName, "test");
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
            }

        }

        [TestCleanup]
        public void TestCleanup() { System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); }
    }
}
