using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Views;
using Microsoft.Support.Workflow.Authoring.ViewModels;

namespace Microsoft.Support.Workflow.Authoring.Tests.Services
{
    [TestClass]
    public class ViewViewModelMappingsUnitTest
    {
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void ViewViewModelMappings_TestGetViewTypeFromViewModelType()
        {
            Assert.AreEqual(typeof(AboutView), 
                ViewViewModelMappings.GetViewTypeFromViewModelType(typeof(AboutViewModel)));
            Assert.AreEqual(typeof(ImportAssemblyView), 
                ViewViewModelMappings.GetViewTypeFromViewModelType(typeof(ImportAssemblyViewModel)));
            Assert.AreEqual(typeof(NewWorkflowView), 
                ViewViewModelMappings.GetViewTypeFromViewModelType(typeof(NewWorkflowViewModel)));
            Assert.AreEqual(typeof(OpenWorkflowFromServerView), 
                ViewViewModelMappings.GetViewTypeFromViewModelType(typeof(OpenWorkflowFromServerViewModel)));
            Assert.AreEqual(typeof(ReviewActivityView), 
                ViewViewModelMappings.GetViewTypeFromViewModelType(typeof(ReviewActivityViewModel)));
            Assert.AreEqual(typeof(UploadAssemblyView), 
                ViewViewModelMappings.GetViewTypeFromViewModelType(typeof(UploadAssemblyViewModel)));
            Assert.AreEqual(typeof(ClickableMessage), 
                ViewViewModelMappings.GetViewTypeFromViewModelType(typeof(ClickableMessageViewModel)));
            Assert.AreEqual(typeof(SelectAssemblyAndActivityView), 
                ViewViewModelMappings.GetViewTypeFromViewModelType(typeof(SelectAssemblyAndActivityViewModel)));
        }
    }
}
