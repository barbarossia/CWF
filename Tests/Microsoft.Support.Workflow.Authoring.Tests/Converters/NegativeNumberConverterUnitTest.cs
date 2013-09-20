using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Threading;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class NegativeNumberConverterUnitTest
    {
        [WorkItem(321716)]
        [TestMethod]
        [Description("Check if NegativeNumberConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestNegativeNumberConverterConvert()
        {
            object value;
            double result;
            NegativeNumberConverter converter = new NegativeNumberConverter();

            value = null;
            result = (double)converter.Convert(value, null, null, null);
            Assert.AreEqual(0, result);

            value = 1;
            result = (double)converter.Convert(value, null, null, null);
            Assert.AreEqual(-1D, result);
        }

        [WorkItem(321715)]
        [TestMethod]
        [Description("Check NegativeNumberConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestNegativeNumberConverterConvertBack()
        {
            var converter = new NegativeNumberConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }
    }
}
