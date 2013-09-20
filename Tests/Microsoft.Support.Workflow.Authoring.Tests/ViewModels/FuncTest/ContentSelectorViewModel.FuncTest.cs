using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
namespace Authoring.Tests.Functional
{
    [TestClass]
    public class ContentSelectorViewModelTests
    {
        #region test methods

        [WorkItem(27569)]
        [Description("Test the Content Selector ViewModel")]
        [Owner("v-ertang")]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyContentSelectorViewModel()
        {
            var vmContentSelector = new ContentSelectorViewModel();
            vmContentSelector.SearchFilter = "Test";            
            ContentFileItem contentFile = new ContentFileItem();
            contentFile.FileName = "Test";
            contentFile.FileShortName = "Test";
            vmContentSelector.ContentFileItems.Add(contentFile);
        }

        #endregion
    }
}
