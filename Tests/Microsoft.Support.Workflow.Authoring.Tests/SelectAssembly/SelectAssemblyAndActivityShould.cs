using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using System.Collections.ObjectModel;
using System.Reflection;
using AuthoringToolTests.Services;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Microsoft.Support.Workflow.Authoring.Tests.SaveToServer
{
    [TestClass]
    public class SelectAssemblyAndActivityShould
    {
        private const string OWNER = "v-yiabdi";
        private TestContext testContextInstance;
        private const string TestCategory = "FullTest";
        private SelectAssemblyAndActivityViewModel select = null;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            select = TestUtilities.SelectAssemblyAndActivity;
        }
        #endregion

        [Description("Add Select Assembly And Activity To Caching")]
        [TestMethod]
        [Owner(OWNER)]
        [WorkItem(164776)]
        [TestCategory("Func-NoDif1-Full")]
        [Ignore]
        public void AddSelectAssemblyAndActivityToCaching()
        {            
            //Modify select status
            foreach (ActivityAssemblyItem activityAssemblyItem in select.ActivityAssemblyItemCollection)
            {
                if (activityAssemblyItem != null)
                {
                    bool isSelected = TestUtilities.GetRandomBoolean;
                    foreach (ActivityItem item in activityAssemblyItem.ActivityItems)
                    {
                        if (isSelected){ item.UserSelected = isSelected; }
                    }
                    //Activity
                    if (isSelected){ activityAssemblyItem.UserSelected = isSelected; }
                }
            }

            using (new CachingIsolator())
            {
                //OkCommand Excute
                WorkflowsQueryServiceUtility.UsingClient(select.OkCommandExecute);
            }

            //Modify select statys
            foreach (ActivityAssemblyItem activityAssemblyItem in select.ActivityAssemblyItemCollection)
            {
                if (activityAssemblyItem != null)
                {
                    //Compare cached Item
                    if(activityAssemblyItem.UserSelected)
                    {
                        ActivityAssemblyItem cachedActivityAssemblyItem;
                        Assert.IsTrue(Caching.LoadCachedAssembly(
                            activityAssemblyItem.AssemblyName, out cachedActivityAssemblyItem));
                   }
                }
            }
        }

        [Description("Not Add If UnSelected Assembly And Activity To Caching")]
        [TestMethod]
        [Owner(OWNER)]
        [WorkItem(164777)]
        [TestCategory("Func-NoDif1-Full")]
        public void NotAddIfUnSelectedAssemblyAndActivityToCaching()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                //OkCommand Excute
                WorkflowsQueryServiceUtility.UsingClient(select.OkCommandExecute);
            }

            //Modify select statys
            foreach (ActivityAssemblyItem activityAssemblyItem in select.ActivityAssemblyItemCollection)
            {
                if (activityAssemblyItem != null)
                {
                    //Compare cached Item
                    if (activityAssemblyItem.UserSelected)
                    {
                        ActivityAssemblyItem cachedActivityAssemblyItem;
                        Assert.IsFalse(Caching.LoadCachedAssembly(
                            activityAssemblyItem.AssemblyName, out cachedActivityAssemblyItem));
                    }
                }
            }
        }
    }
}
