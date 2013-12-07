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
using Microsoft.DynamicImplementations;
using NuGet;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class PackageTitleConverterUnitTest
    {
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void Aconvert_TestPackageTitleConverterConvert()
        {
            string result;
            PackageTitleConverter converter = new PackageTitleConverter();

            result = converter.Convert(null, typeof(string), null, null) as string;
            Assert.IsNull(result);

            using (var package = new Implementation<IPackage>())
            {
                package.Register(p => p.Id).Return("Id");

                package.Register(p => p.Title).Return(null);
                result = converter.Convert(package.Instance, typeof(string), null, null) as string;
                Assert.AreEqual("Id", result);

                package.Register(p => p.Title).Return("Title");
                result = converter.Convert(package.Instance, typeof(string), null, null) as string;
                Assert.AreEqual("Title", result);
            }
        }

        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void Aconvert_TestPackageTitleConverterConvertBack()
        {
            PackageTitleConverter converter = new PackageTitleConverter();
            TestUtilities.Assert_ShouldThrow<NotImplementedException>(() =>
            {
                converter.ConvertBack("", null, null, null);
            });
        }
    }
}
