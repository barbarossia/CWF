using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using System.Reflection;
using CWF.DataContracts;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.Support.Workflow.Authoring.Common;
using System.IO;
using System.Activities;
using Microsoft.Support.Workflow.Authoring.Tests.HelpClass;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using System.Activities.Statements;
using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;


namespace Microsoft.Support.Workflow.Authoring.Tests.Services
{
    [TestClass]
    public class DataCrontractTranslatorUnitTest
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        // Create WorkflowItems lazily to avoid creating lots of WorkflowDesigners. 
        private WorkflowItem validWorkflowItem;
        private WorkflowItem ValidWorkflowItem
        {
            get
            {
                if (validWorkflowItem == null)
                {
                    validWorkflowItem = new WorkflowItem("MyWorkflow", "WF", (new Sequence()).ToXaml(), string.Empty);
                }
                validWorkflowItem.Env = Authoring.AddIns.Data.Env.Dev;
                Assert.IsTrue(validWorkflowItem.IsValid, "Setup error! validWorkflowItem should be valid XAML for workflow.");
                return validWorkflowItem;
            }
        }

        [WorkItem(322376)]
        [TestCategory("Unit-NoDif")]
        [Owner("v-kason")]
        [TestMethod()]
        public void DataCrontractTranslator_VerifyWorkflowToStoreLibraryAndActivitiesRequestDC()
        {
            MarketplaceDataHelper helper = new MarketplaceDataHelper(TestContext);
            var workflow = ValidWorkflowItem;
            var assemblyItemsUsed = new List<ActivityAssemblyItem> { 
                new ActivityAssemblyItem(){
                Name=workflow.Name,
                Version=new System.Version(workflow.Version),
                Env = Authoring.AddIns.Data.Env.Dev
                },
            };
            StoreLibraryAndActivitiesRequestDC storeLibraryAndActivitiesRequestDC = null;
            try
            {
                storeLibraryAndActivitiesRequestDC =
                    DataContractTranslator.WorkflowToStoreLibraryAndActivitiesRequestDC(workflow, assemblyItemsUsed, new List<TaskAssignment>());
            }
            catch (NullReferenceException) { }
            Assert.AreEqual(storeLibraryAndActivitiesRequestDC.StoreActivitiesList.Count, 1);
            Assert.AreEqual(storeLibraryAndActivitiesRequestDC.ActivityLibrary.Name, workflow.Name);
            Assert.AreEqual(storeLibraryAndActivitiesRequestDC.ActivityLibrary.VersionNumber, workflow.Version);
        }

        [WorkItem(322368)]
        [TestCategory("Unit-NoDif")]
        [Owner("v-kason")]
        [TestMethod()]
        public void DataCrontractTranslator_VerifyToStoreLibraryAndActivitiesRequestDC()
        {
            MarketplaceDataHelper helper = new MarketplaceDataHelper(TestContext);
            var assembly = helper.GetTestActivities().FirstOrDefault();

            StoreLibraryAndActivitiesRequestDC storeLibraryAndActivitiesRequestDC =
                DataContractTranslator.ToStoreLibraryAndActivitiesRequestDC(assembly);

            Assert.AreEqual(storeLibraryAndActivitiesRequestDC.StoreActivitiesList.Count, 1);
            Assert.AreEqual(storeLibraryAndActivitiesRequestDC.ActivityLibrary.Name, assembly.Name);
            Assert.AreEqual(storeLibraryAndActivitiesRequestDC.ActivityLibrary.VersionNumber, assembly.Version.ToString());
        }
    }
}
