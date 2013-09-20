using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Models;
using System.Collections.ObjectModel;

namespace Microsoft.Support.Workflow.Authoring.Tests.Models
{
    [TestClass]
    public class CommonDataUnitTests
    {
       
        [TestMethod()]
        [Description("Check to see if we can instantiate the target class correctly (Bug #86473)")]        
        [TestCategory("Unit")]
        public void ActivityAssemblyItemViewModelConstructorTest()
        {
            CommonData target = CommonData.Instance;
            Assert.IsNotNull(target, "An instance of CommonData could not be instantiated.");
        }

       
        [TestMethod()]
        [Description("Test to see if we can retrieve the (static, non database sourced) list of categories (Bug #86473)")]        
        [TestCategory("Unit")]
        public void CategoriesTest()
        {
            CommonData target = CommonData.Instance;
            ObservableCollection<string> actual = target.ActivityCategories;

            Assert.IsTrue(actual.Count != 0, "No categories could be retrieved."); // this is from a static list and not loaded from a database - there should always be entries
        }
    }
}
