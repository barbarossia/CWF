using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Threading;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class InvertedNullToVisibilityConverterUnitTest
    {
        [WorkItem(321723)]
        [TestMethod]
        [Description("Check if InvertedNullToVisibilityConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestInvertedNullToVisibilityConverterConvert()
        {
            object value;
            Visibility result;
            InvertedNullToVisibilityConverter converter = new InvertedNullToVisibilityConverter();

            value = null;
            result = (Visibility)converter.Convert(null, null, null, null);
            Assert.AreEqual(Visibility.Collapsed, result);

            value = null;
            result = (Visibility)converter.Convert(value, typeof(Visibility), null, null);
            Assert.AreEqual(Visibility.Visible, result);

            value = new object();
            result = (Visibility)converter.Convert(value, typeof(Visibility), null, null);
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [WorkItem(321722)]
        [TestMethod]
        [Description("Check InvertedNullToVisibilityConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestInvertedNullToVisibilityConverterConvertBack()
        {
            var converter = new InvertedNullToVisibilityConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }
    }
}
