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
    public class NullToDefaultValueConverterUnitTest
    {
        [WorkItem(321711)]
        [TestMethod]
        [Description("Check if NullToDefaultValueConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestNullToDefaultValueConverterConvert()
        {
            object value;
            object result;
            NullToDefaultValueConverter converter = new NullToDefaultValueConverter();

            value = null;
            converter.DefaultValue = "default";
            result = converter.Convert(value, null, null, null) as string;
            Assert.AreEqual(converter.DefaultValue, result);

            value = null;
            converter.DefaultValue = string.Empty;
            result = converter.Convert(value, null, null, null) as string;
            Assert.AreEqual(converter.DefaultValue, result);

            value = "test";
            result = converter.Convert(value, null, null, null) as string;
            Assert.AreEqual(value, result);

            value = 1;
            result = converter.Convert(value, null, null, null);
            Assert.AreEqual(1, result);
        }

        [WorkItem(321710)]
        [TestMethod]
        [Description("Check NullToDefaultValueConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestNullToDefaultValueConverterConvertBack()
        {
            var converter = new NullToDefaultValueConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }
    }
}
