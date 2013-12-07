using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Threading;
using Microsoft.Support.Workflow.Authoring.AddIns.Converters;
using Microsoft.Support.Workflow.Authoring.CDS;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class EnumBooleanConverterUnitTest
    {
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void Aconvert_TestEnumBooleanConverterConvert()
        {
            CDSSortByType value;
            object result;
            EnumBooleanConverter converter = new EnumBooleanConverter();

            value = (CDSSortByType)0;
            result = converter.Convert(value, typeof(bool), null, null);
            Assert.AreEqual(DependencyProperty.UnsetValue, result);
            result = converter.Convert(value, typeof(bool), "", null);
            Assert.AreEqual(DependencyProperty.UnsetValue, result);

            value = CDSSortByType.PublishedDate;
            result = converter.Convert(value, typeof(bool), "PublishedDate", null);
            Assert.IsTrue((bool)result);
        }

        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void Aconvert_TestEnumBooleanConverterConvertBack()
        {
            bool value;
            object result;
            EnumBooleanConverter converter = new EnumBooleanConverter();

            value = false;
            result = (CDSSortByType)converter.ConvertBack(value, typeof(CDSSortByType), null, null);
            Assert.AreEqual(DependencyProperty.UnsetValue, result);

            value = false;
            result = (CDSSortByType)converter.ConvertBack(value, typeof(CDSSortByType), "anything", null);
            Assert.AreEqual(DependencyProperty.UnsetValue, result);
    
            value = true;
            result = (CDSSortByType)converter.ConvertBack(value, typeof(CDSSortByType), CDSSortByType.NameAscending.ToString(), null);
            Assert.AreEqual(CDSSortByType.NameAscending, (CDSSortByType)result);
        }
    }
}
