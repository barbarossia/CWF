using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Threading;
using System.Windows.Media;
using Microsoft.Support.Workflow.Authoring.Common;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class LocationTypeToBrushConverterUnitTest
    {
        [WorkItem(321720)]
        [TestMethod]
        [Description("Check if LocationTypeToBrushConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestLocationTypeToBrushConverterConvert()
        {
            LocationType value;
            SolidColorBrush result;
            LocationTypeToBrushConverter converter = new LocationTypeToBrushConverter();

            value = LocationType.New;
            result = (SolidColorBrush)converter.Convert(value, null, null, null);
            Assert.AreEqual(Colors.Transparent, result.Color);

            value = LocationType.Cached;
            result = (SolidColorBrush)converter.Convert(value, null, null, null);
            Assert.AreEqual(Colors.Transparent, result.Color);

            value = LocationType.None;
            result = (SolidColorBrush)converter.Convert(value, null, null, null);
            Assert.AreEqual(Colors.Red, result.Color);

            value = (LocationType)10;
            result = (SolidColorBrush)converter.Convert(value, null, null, null);
            Assert.AreEqual(Colors.Transparent, result.Color);
        }

        [WorkItem(321719)]
        [TestMethod]
        [Description("Check LocationTypeToBrushConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestLocationTypeToBrushConverterConvertBack()
        {
            var converter = new LocationTypeToBrushConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }

        [TestCleanup]
        public void TestCleanup() { System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); }
    }
}
