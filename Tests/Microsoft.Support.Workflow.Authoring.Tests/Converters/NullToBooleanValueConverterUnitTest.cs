using Microsoft.Support.Workflow.Authoring.Common.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    /// <summary>
    ///This is a test class for NullToBooleanValueConverterTest and is intended
    ///to contain all NullToBooleanValueConverterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NullToBooleanValueConverterUnitTest
    {
        [TestMethod()]
        [Description("Check to see if we can instantiate the target class correctly  (Bug #86473)")]
        [Owner("v-richt")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_NullToBooleanValueConverterConstructorUnitTest()
        {
            NullToBooleanValueConverter target = new NullToBooleanValueConverter();
            Assert.IsNotNull(target, "An instance of NullToBooleanValueConverter could not be instantiated.");
        }

        [TestMethod()]
        [Description("Test different values as input to see if they convert to true/false correctly  (Bug #86473)")]
        [Owner("v-richt")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestNullToBooleanValueConverterConvert()
        {
            NullToBooleanValueConverter target = new NullToBooleanValueConverter();

            target.Invert = false;
            Assert.IsTrue((bool)target.Convert(null, null, null, null) == false);
            Assert.IsTrue((bool)target.Convert(true, null, null, null) == true);
            Assert.IsTrue((bool)target.Convert(int.MaxValue, null, null, null) == true);
            Assert.IsTrue((bool)target.Convert(String.Empty, null, null, null) == true);
            Assert.IsTrue((bool)target.Convert(DBNull.Value, null, null, null) == true);
            Assert.IsTrue((bool)target.Convert(Guid.Empty, null, null, null) == true);

            target.Invert = true;
            Assert.IsTrue((bool)target.Convert(null, null, null, null) == true);
            Assert.IsTrue((bool)target.Convert(true, null, null, null) == false);
            Assert.IsTrue((bool)target.Convert(int.MaxValue, null, null, null) == false);
            Assert.IsTrue((bool)target.Convert(String.Empty, null, null, null) == false);
            Assert.IsTrue((bool)target.Convert(DBNull.Value, null, null, null) == false);
            Assert.IsTrue((bool)target.Convert(Guid.Empty, null, null, null) == false);

        }

        [WorkItem(321714)]
        [TestMethod]
        [Description("Check NullToBooleanValueConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestNullToBooleanValueConverterConvertBack()
        {
            var converter = new NullToBooleanValueConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }
    }
}
