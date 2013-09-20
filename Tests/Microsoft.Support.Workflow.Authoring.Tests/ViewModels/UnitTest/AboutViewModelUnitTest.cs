using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels
{
    [TestClass]
    public class AboutViewModelUnitTest
    {
        [WorkItem(322374)]
        [TestCategory("Unit-NoDif")]
        [Owner("v-kason")]
        [Description("Verify the Version property is not null")]
        [TestMethod()]
        public void About_VerifyVersionNotNull() 
        {
            var vm = new AboutViewModel();
            Assert.IsFalse(string.IsNullOrEmpty(vm.Version));
        }
    }
}
