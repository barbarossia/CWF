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
            bool result;
            BooleanInverterConverter converter = new BooleanInverterConverter();

            try
            {
                value = null;
                converter.Convert(value, typeof(bool), null, null);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(typeof(ArgumentNullException), ex.GetType());
            }

            try
            {
                value = 1;
                converter.Convert(value, null, null, null);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(typeof(InvalidOperationException), ex.GetType());
            }

            value = true;
            result = (bool)converter.Convert(value, typeof(bool), null, null);
            Assert.AreEqual(false, result);
        }

        [WorkItem(321689)]
        [TestMethod]
        [Description("Check BooleanInverterConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestBooleanInverterConverterConvertBack()
        {
            var converter = new BooleanInverterConverter();
            var result = (bool)converter.Convert(true, typeof(bool), null, null);
            Assert.AreEqual(false, result);
        }
    }
}
