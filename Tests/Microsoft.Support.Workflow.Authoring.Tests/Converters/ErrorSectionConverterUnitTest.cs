using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Threading;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class ErrorSectionConverterUnitTest
    {
        [WorkItem(321728)]
        [TestMethod]
        [Description("Check if ErrorSectionConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestErrorSectionConverterConvert()
        {
            object result;

            ActivityItem item = new ActivityItem();
            item.ErrorSectionsDictionary.Add("test", "error");
            ErrorSectionConverter converter = new ErrorSectionConverter();
            converter.ViewModelBaseDerivedType = item;

            item.Description = string.Empty;
            Thread.Sleep(2000);
            Assert.AreNotEqual(item.IsValid, converter.HasErrors);

            result = converter.Convert(null, null, null, null);
            Assert.AreEqual(typeof(string), result.GetType());
            Assert.AreEqual(string.Empty, (string)result);

            result = converter.Convert(null, null, "test;integer;2;3", null);
            Assert.AreEqual(typeof(int), result.GetType());
            Assert.AreEqual(2, (int)result);

            result = converter.Convert(null, null, "test2;integer;2;3", null);
            Assert.AreEqual(typeof(int), result.GetType());
            Assert.AreEqual(3, (int)result);
        }

        [WorkItem(321727)]
        [TestMethod]
        [Description("Check ErrorSectionConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestErrorSectionConverterConvertBack()
        {
            var converter = new ErrorSectionConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }
    }
}
