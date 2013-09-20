using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Threading;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.AddIns;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class CachingStatusToBoolConverterUnitTest
    {
        [WorkItem(321694)]
        [TestMethod]
        [Description("Check if CachingStatusToBoolConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestCachingStatusToBoolConverterConvert()
        {
            CachingStatus value;
            bool result;
            CachingStatusToBoolConverter converter = new CachingStatusToBoolConverter();

            value = CachingStatus.Latest;
            converter.Invert = false;
            result = (bool)converter.Convert(value, null, null, null);
            Assert.IsTrue(result);

            value = CachingStatus.Latest;
            converter.Invert = true;
            result = (bool)converter.Convert(value, null, null, null);
            Assert.IsFalse(result);
        }

        [WorkItem(321696)]
        [TestMethod]
        [Description("Check CachingStatusToBoolConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestCachingStatusToBoolConverterConvertBack()
        {
            var converter = new CachingStatusToBoolConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }
    }
}
