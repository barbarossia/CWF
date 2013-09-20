using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Threading;
using System.Windows.Data;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class CountConverterUnitTest
    {
        [WorkItem(321731)]
        [TestMethod]
        [Description("Check if CountConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestCountConverterConvert()
        {
            object value;
            string result;
            CountConverter converter = new CountConverter();

            value = new List<int>();
            result = converter.Convert(value, null, null, null) as string;
            Assert.AreEqual(converter.ZeroOrNullDisplayValue, result);

            value = new TestList<int>();
            result = converter.Convert(value, null, null, null) as string;
            Assert.AreEqual(converter.ZeroOrNullDisplayValue, result);

            value = new int[0];
            result = converter.Convert(value, null, null, null) as string;
            Assert.AreEqual(converter.ZeroOrNullDisplayValue, result);
        }

        [WorkItem(321730)]
        [TestMethod]
        [Description("Check CountConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestCountConverterConvertBack()
        {
            var converter = new CountConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }

        public class TestList<T>
        {
            public int Count()
            {
                return 0;
            }
        }
    }
}
