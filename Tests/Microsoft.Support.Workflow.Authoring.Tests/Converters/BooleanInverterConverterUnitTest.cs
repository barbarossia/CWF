using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Threading;
using Microsoft.Support.Workflow.Authoring.AddIns.Converters;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class BooleanInverterConverterUnitTest
    {
        [WorkItem(321687)]
        [TestMethod]
        [Description("Check if BooleanInverterConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestBooleanInverterConverterConvert()
        {
            object value;
            object result;
            BooleanInverterConverter converter = new BooleanInverterConverter();

            value = null;
            result = converter.Convert(value, typeof(bool), null, null);
            Assert.IsNull(result);

            value = 1;
            TestUtilities.Assert_ShouldThrow<InvalidOperationException>(() =>
            {
                converter.Convert(value, null, null, null);
            });

            value = true;
            result = (bool)converter.Convert(value, typeof(bool), null, null);
            Assert.AreEqual(false, result);
        }

        [WorkItem(321689)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestBooleanInverterConverterConvertBack()
        {
            object value;
            object result;
            BooleanInverterConverter converter = new BooleanInverterConverter();

            value = null;
            result = converter.ConvertBack(value, typeof(bool), null, null);
            Assert.IsNull(result);

            value = 1;
            TestUtilities.Assert_ShouldThrow<InvalidOperationException>(() =>
            {
                converter.ConvertBack(value, null, null, null);
            });

            value = true;
            result = (bool)converter.ConvertBack(value, typeof(bool), null, null);
            Assert.AreEqual(false, result);
        }
    }
}
