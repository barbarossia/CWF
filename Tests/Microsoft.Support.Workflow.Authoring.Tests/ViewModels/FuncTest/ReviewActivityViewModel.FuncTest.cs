using System;
using System.Reflection;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Authoring.Tests.Functional
{
    [TestClass]
    public class ReviewActivityViewModelFunctionalTest
    {
        #region test methods

        [WorkItem(16243)]
        [Description("Test the ReviewActivity function")]
        [Owner("v-ertang")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void VerifyReviewActivity()
        {
            try
            {
                // Tests that ReviewActivityViewModel can be created
                string executingAssemblyPath = TestUtilities.GetExecutingAssemblyPath();
                string assemblyName = Path.GetFileNameWithoutExtension(executingAssemblyPath);
                // creates datacontext for the ReviewActivityView
                var vmReviewActivity = new ReviewActivityViewModel(new ActivityAssemblyItemViewModel(new ActivityAssemblyItem(Assembly.LoadFrom(executingAssemblyPath))));
                Assert.AreEqual("Review Activities in " + assemblyName, vmReviewActivity.Title);
            }
            catch (Exception ex)
            {
                Assert.Fail("ReviewActivityViewModel failed with this exception: {0}", ex.Message);
            }
        }

        #endregion
    }
}
