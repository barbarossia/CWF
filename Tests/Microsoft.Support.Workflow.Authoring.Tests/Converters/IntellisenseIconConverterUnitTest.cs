using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Threading;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.IO;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.DynamicImplementations;
using System.Security;
using Microsoft.Support.Workflow.Authoring.AddIns.Converters;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class IntellisenseIconConverterUnitTest
    {
        [WorkItem(321725)]
        [TestMethod]
        [Description("Check if IntellisenseIconConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestIntellisenseIconConverterConvert()
        {
            TreeNodeType value;
            object result;
            IntellisenseIconConverter converter = new IntellisenseIconConverter();

            value = (TreeNodeType)20;
            result = converter.Convert(value, null, null, null);
            Assert.IsNull(result);

            value = TreeNodeType.Class;
            result = converter.Convert(value, null, null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is BitmapSource);
        }

        [WorkItem(321724)]
        [TestMethod]
        [Description("Check IntellisenseIconConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotSupportedException))]
        public void Aconvert_TestIntellisenseIconConverterConvertBack()
        {
            var converter = new IntellisenseIconConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }
        [TestCleanup]
        public void TestCleanup() { System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); }
    }
}
