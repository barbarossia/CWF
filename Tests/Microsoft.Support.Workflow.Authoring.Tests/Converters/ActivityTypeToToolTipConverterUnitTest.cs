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
    public class ActivityTypeToToolTipConverterUnitTest
    {
        [WorkItem(321679)]
        [TestMethod]
        [Description("Check if ActivityTypeToToolTipConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestActivityTypeToToolTipConverterConvert()
        {
            Type type;
            string result;
            ActivityTypeToToolTipConverter converter = new ActivityTypeToToolTipConverter();
  
            type = null;
            result = converter.Convert(type, null, null, null) as string;
            Assert.IsNull(result);

            type = typeof(int);
            ActivityTypeToToolTipConverter.ToolTipDictionary.Add(type, type.ToString());
            result = converter.Convert(type, null, null, null) as string;
            Assert.AreEqual(ActivityTypeToToolTipConverter.ToolTipDictionary[type], result);

            type = typeof(bool);
            result = converter.Convert(type, null, null, null) as string;
            Assert.AreEqual(string.Format("v{0}, Built-in .NET activity", type.Assembly.GetName().Version), result);
        }

        [WorkItem(321681)]
        [TestMethod]
        [Description("Check ActivityTypeToToolTipConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestActivityTypeToToolTipConverterConvertBack()
        {
            var converter = new ActivityTypeToToolTipConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }
    }
}
