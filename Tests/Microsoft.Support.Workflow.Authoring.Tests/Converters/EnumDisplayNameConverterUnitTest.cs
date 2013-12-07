using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Threading;
using Microsoft.Support.Workflow.Authoring.AddIns.Converters;
using Microsoft.Support.Workflow.Authoring.CDS;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class EnumDisplayNameConverterUnitTest
    {
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestEnumDisplayNameConverterConvert()
        {
            CDSSortByType value;
            string result;
            EnumDisplayNameConverter converter = new EnumDisplayNameConverter();

            value = CDSSortByType.PublishedDate;
            result = converter.Convert(value, typeof(string), null, null) as string;
            Assert.AreEqual("Published Date", result);
        }

        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestEnumDisplayNameConverterConvertBack()
        {
            EnumDisplayNameConverter converter = new EnumDisplayNameConverter();
            TestUtilities.Assert_ShouldThrow<NotImplementedException>(() =>
            {
                converter.ConvertBack("", null, null, null);
            });
        }
    }
}
