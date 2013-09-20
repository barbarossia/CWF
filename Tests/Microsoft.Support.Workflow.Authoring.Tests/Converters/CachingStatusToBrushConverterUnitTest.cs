using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Threading;
using Microsoft.Support.Workflow.Authoring.Common;
using System.Windows.Media;
using Microsoft.Support.Workflow.Authoring.AddIns;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class CachingStatusToBrushConverterUnitTest
    {
        [WorkItem(321698)]
        [TestMethod]
        [Description("Check if CachingStatusToBrushConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestCachingStatusToBrushConverterConvert()
        {
            CachingStatus value;
            Brush result;
            CachingStatusToBrushConverter converter = new CachingStatusToBrushConverter();

            value = CachingStatus.Server;
            result = (SolidColorBrush)converter.Convert(value, null, null, null);
            Assert.AreEqual(Brushes.Red, result);

            value = CachingStatus.Latest;
            result = (SolidColorBrush)converter.Convert(value, null, null, null);
            Assert.AreEqual(Brushes.LawnGreen, result);

            value = CachingStatus.CachedOld;
            result = (SolidColorBrush)converter.Convert(value, null, null, null);
            Assert.AreEqual(Brushes.Yellow, result);

            value = CachingStatus.None;
            result = (SolidColorBrush)converter.Convert(value, null, null, null);
            Assert.AreEqual(Brushes.Red, result);
        }

        [WorkItem(321699)]
        [TestMethod]
        [Description("Check CachingStatusToBrushConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestCachingStatusToBrushConverterConvertBack()
        {
            var converter = new CachingStatusToBrushConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }
    }
}
