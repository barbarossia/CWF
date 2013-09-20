using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common;

namespace Microsoft.Support.Workflow.Authoring.Tests.Common
{
    [TestClass]
    public class MultiMapUnitTests
    {
        [WorkItem(348270)]
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void MultiMap_MultiMap()
        {
            var multi = new MultiMap<string, string>();
            //test of AddValues
            multi.AddValue("key1","value1");
            multi.AddValue("key1","value2");
            Assert.AreEqual(multi.Count,1);
            Assert.AreEqual(multi["key1"].ToList().Count,2);

            //test of GetValues
            IEnumerable<string> returnValues=multi.GetValues("key1");
            Assert.AreEqual(returnValues.Count<string>(),2);
        }
    }
}
