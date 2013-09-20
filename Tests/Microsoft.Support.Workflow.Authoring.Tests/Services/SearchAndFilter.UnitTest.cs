using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.ViewModels;

namespace Microsoft.Support.Workflow.Authoring.Tests.Services
{
    [TestClass]
    public class SearchAndFilter
    {
        [TestMethod]
        public void Verify_ContainingSearchWithLatestVersion()
        {
            StoreActivitiesDC[] input = new StoreActivitiesDC[4];
            input[0] = new StoreActivitiesDC(){ Name="test$WA*search", MetaTags="Test$WA*SearchTag", Description="Test*searchDescription", Version="1.2.3.4"};
            input[1] = new StoreActivitiesDC(){ Name="search*WA$test", MetaTags="TagSearch*WA$Test", Description="Descriptionsearch*WA$Test", Version="42.34.24.56"};
            input[2] = new StoreActivitiesDC(){ Name="search*WA$test", MetaTags="TagSearch*WA$Test", Description="Descriptionsearch*WA$Test", Version = "42.34.25.56" };
            input[3] = new StoreActivitiesDC() { Name = "test*search", MetaTags = "Test$WA*SearchTag", Description = "Test$WA*searchDescription", Version = "1.2.3.5" };

            OpenWorkflowFromServerViewModel vm = new OpenWorkflowFromServerViewModel();
            vm.wholeWorkflows = input;
            SearchType type = SearchType.LatestVersionOnly;
            string searchValue = "";      
            vm.RunWorkflowFilter(searchValue, type);
            Assert.AreEqual(vm.WorkflowsOnServer.Count, 3);

            vm = new OpenWorkflowFromServerViewModel();
            vm.wholeWorkflows = input;
            type = SearchType.Name;
            searchValue = "WA$";
            vm.RunWorkflowFilter(searchValue, type);
            Assert.AreEqual(vm.WorkflowsOnServer.Count, 2);

            vm = new OpenWorkflowFromServerViewModel();
            vm.wholeWorkflows = input;
            type = SearchType.Name;
            type |= SearchType.Description;
            searchValue = "$WA";
            vm.RunWorkflowFilter(searchValue, type);
            Assert.AreEqual(vm.WorkflowsOnServer.Count, 2);

        }

        [TestMethod]
        public void Verify_ContainingSearchWithWildCard()
        {
            StoreActivitiesDC[] input = new StoreActivitiesDC[4];
            input[0] = new StoreActivitiesDC() { Name = "test$WAsearch", MetaTags = "Test$WA*SearchTag", Description = "Test*searchDescription", Version = "1.2.3.4" };
            input[1] = new StoreActivitiesDC() { Name = "search*WA$test", MetaTags = "TagSearch*WA$Test", Description = "DescriptionsearchWA$Test", Version = "42.34.24.56" };
            input[2] = new StoreActivitiesDC() { Name = "search*WA$test", MetaTags = "TagSearch*WA$Te", Description = "DescriptionsearchWA$Test", Version = "42.34.25.56" };
            input[3] = new StoreActivitiesDC() { Name = "test*search", MetaTags = "Test$WA*SearchTag", Description = "Test$WA*searchDescription", Version = "1.2.3.5" };

            OpenWorkflowFromServerViewModel vm = new OpenWorkflowFromServerViewModel();
            vm.wholeWorkflows = input;
            SearchType type = SearchType.Tags;
            string searchValue = "Tag*st";
            vm.RunWorkflowFilter(searchValue, type);
            Assert.AreEqual(vm.WorkflowsOnServer.Count, 1);

            vm = new OpenWorkflowFromServerViewModel();
            vm.wholeWorkflows = input;
            type = SearchType.Description;           
            searchValue = "Des*WA*st";
            vm.RunWorkflowFilter(searchValue, type);
            Assert.AreEqual(vm.WorkflowsOnServer.Count, 2);

        }
    }
}
