using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Threading;
using System.Windows.Media;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class NullToBrushConverterUnitTest
    {
        [WorkItem(321713)]
        [TestMethod]
        [Description("Check if NullToBrushConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestNullToBrushConverterConvert()
        {
            string value;
            SolidColorBrush result;
            NullToBrushConverter converter = new NullToBrushConverter();

            value = null;
            result = converter.Convert(value, null, null, null) as SolidColorBrush;
            Assert.AreEqual(Colors.Red, result.Color);

            value = "t";
            result = converter.Convert(value, null, null, null) as SolidColorBrush;
            Assert.AreEqual(Colors.Transparent, result.Color);
        }

        [WorkItem(321712)]
        [TestMethod]
        [Description("Check NullToBrushConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestNullToBrushConverterConvertBack()
        {
            var converter = new NullToBrushConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }
    }
}
