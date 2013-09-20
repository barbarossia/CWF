using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
namespace Microsoft.Support.Workflow.Authoring.Tests.Common
{
    [TestClass]
    public class ExtensionMethodsUnitTest
    {
        [WorkItem(322881)]
        [TestMethod]
        [Description("Check if Remove works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void ExtensionMethods_TestRemove()
        {
            ObservableCollection<int> numbers = new ObservableCollection<int>() { 1, 2, 3, 4, 5, 6, 4, 4, 3, 2 };
            numbers.Remove(n => n > 3);
            Assert.IsTrue(numbers.All(n => n <= 3));
        }
    }
}
