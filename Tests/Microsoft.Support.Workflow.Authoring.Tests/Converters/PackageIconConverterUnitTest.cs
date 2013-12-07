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
    public class PackageIconConverterUnitTest
    {
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void Aconvert_TestPackageIconConverterConvert()
        {
            Uri value;
            Uri result;
            PackageIconConverter converter = new PackageIconConverter();

            // to register pack scheme
            if (!UriParser.IsKnownScheme("pack"))
                new Application();

            value = null;
            result = converter.Convert(value, typeof(Uri), null, null) as Uri;
            Assert.AreEqual("pack://application:,,,/Resources/Images/packageicon.png", result.AbsoluteUri);

            value = new Uri("http://packageSource");
            result = converter.Convert(value, typeof(Uri), null, null) as Uri;
            Assert.AreEqual(value, result);
        }

        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void Aconvert_TestPackageIconConverterConvertBack()
        {
            PackageIconConverter converter = new PackageIconConverter();
            TestUtilities.Assert_ShouldThrow<NotImplementedException>(() =>
            {
                converter.ConvertBack("", null, null, null);
            });
        }
    }
}
