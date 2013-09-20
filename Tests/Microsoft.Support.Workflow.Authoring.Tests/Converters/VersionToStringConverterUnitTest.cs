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
    public class VersionToStringConverterUnitTest
    {
        [TestMethod]
        [Description("Check if VersionToStringConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestVersionToStringConverterConvert()
        {
            object value;
            Type targetType = typeof(string);
            string result;
            VersionToStringConverter converter = new VersionToStringConverter();

            value = null;
            result = converter.Convert(value, targetType, null, null) as string;
            Assert.AreEqual(string.Empty, result);

            value = new Version();
            result = converter.Convert(value, null, null, null) as string;
            Assert.AreEqual(string.Empty, result);

            value = new Version(1, 0, 0, 0);
            result = converter.Convert(value, targetType, null, null) as string;
            Assert.AreEqual(value.ToString(), result);
        }

        [TestMethod]
        [Description("Check VersionToStringConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestVersionToStringConverterConvertBack()
        {
            var converter = new VersionToStringConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }
    }
}
