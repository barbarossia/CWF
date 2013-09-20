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
    public class AllMatchToVisibilityConverterUnitTest
    {
        [WorkItem(321682)]
        [TestMethod]
        [Description("Check if AllMatchToVisibilityConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestAllMatchToVisibilityConverterConvert()
        {
            object[] values;
            Visibility result;
            AllMatchToVisibilityConverter converter = new AllMatchToVisibilityConverter();
            
            values = new object[] { 1, 1, 1, 1 };
            converter.Invert = false;
            result = (Visibility)converter.Convert(values, null, null, null);
            Assert.AreEqual(Visibility.Visible, result);
            converter.Invert = true;
            result = (Visibility)converter.Convert(values, null, null, null);
            Assert.AreEqual(Visibility.Collapsed, result);

            values = new object[] { 1, 1, 1, 2 };
            converter.Invert = false;
            result = (Visibility)converter.Convert(values, null, null, null);
            Assert.AreEqual(Visibility.Collapsed, result);
            converter.Invert = true;
            result = (Visibility)converter.Convert(values, null, null, null);
            Assert.AreEqual(Visibility.Visible, result);
        }

        [WorkItem(321683)]
        [TestMethod]
        [Description("Check AllMatchToVisibilityConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestAllMatchToVisibilityConverterConvertBack()
        {
            var converter = new AllMatchToVisibilityConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }
    }
}
