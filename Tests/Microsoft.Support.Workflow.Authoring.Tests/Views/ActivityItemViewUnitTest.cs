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
using Microsoft.Support.Workflow.Authoring.AddIns.Data;

namespace Microsoft.Support.Workflow.Authoring.Tests.Views
{
    [TestClass]
    public class ActivityItemViewUnitTest
    {
        [WorkItem(325744)]
        [Owner("v-kason1")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Views_VerifyDataContext()
        {
            using (var store = new ImplementationOfType(typeof(AssetStoreProxy)))
            {
                store.Register(() => AssetStoreProxy.GetActivityCategories()).Return(true);
                store.Register(() => AssetStoreProxy.GetWorkflowTypes(Env.Test)).Return(true);
                store.Register(() => AssetStoreProxy.ActivityCategoryCreateOrUpdate(Argument<string>.Any)).Return(true);
                store.Register(() => AssetStoreProxy.ActivityCategories).Return(new CollectionViewSource());

                var view = new ActivityItemView();
                PrivateObject po = new PrivateObject(view);
                var viewModel = po.GetProperty("ViewModel") as ActivityItemViewModel;
                Assert.IsNotNull(viewModel);

                view.SelectedCategory = "category";
                Assert.AreEqual(view.SelectedCategory, viewModel.SelectedCategory);

                view.SelectedStatus = "status";
                Assert.AreEqual(view.SelectedStatus, viewModel.SelectedStatus);

            }
        }
    }
}
