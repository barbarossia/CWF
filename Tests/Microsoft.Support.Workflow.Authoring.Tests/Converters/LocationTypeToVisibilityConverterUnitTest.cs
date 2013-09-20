using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Threading;
using System.Windows;
using Microsoft.Support.Workflow.Authoring.Common;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class LocationTypeToVisibilityConverterUnitTest
    {
        [WorkItem(321718)]
        [TestMethod]
        [Description("Check if LocationTypeToVisibilityConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestLocationTypeToVisibilityConverterConvert()
        {
            object value;
            Visibility result;
            LocationTypeToVisibilityConverter converter = new LocationTypeToVisibilityConverter();

            value = LocationType.None;
            result = (Visibility)converter.Convert(value, null, null, null);
            Assert.AreEqual(Visibility.Visible, result);

            value = LocationType.New;
            result = (Visibility)converter.Convert(value, null, null, null);
            Assert.AreEqual(Visibility.Collapsed, result);

            value = string.Empty;
            result = (Visibility)converter.Convert(value, null, null, null);
            Assert.AreEqual(Visibility.Visible, result);

            value = "t";
            result = (Visibility)converter.Convert(value, null, null, null);
            Assert.AreEqual(Visibility.Collapsed, result);

            value = LocationType.None;
            converter.Invert = true;
            result = (Visibility)converter.Convert(value, null, null, null);
            Assert.AreEqual(Visibility.Collapsed, result);

            value = LocationType.New;
            converter.Invert = true;
            result = (Visibility)converter.Convert(value, null, null, null);
            Assert.AreEqual(Visibility.Visible, result);
        }

        [WorkItem(321717)]
        [TestMethod]
        [Description("Check LocationTypeToVisibilityConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestLocationTypeToVisibilityConverterConvertBack()
        {
            var converter = new LocationTypeToVisibilityConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }
    }
}
