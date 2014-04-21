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
using NuGet;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class PackageStatusConverterUnitTest
    {
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void Aconvert_TestPackageStatusConverterConvert()
        {
            object[] values;
            string result;
            PackageStatusConverter converter = new PackageStatusConverter();

            values = new object[] { };
            result = converter.Convert(values, typeof(string), null, null) as string;
            Assert.IsNull(result);

            using (var package = new Implementation<IPackage>())
            {
                using (var service = new ImplementationOfType(typeof(CDSService)))
                {
                    service.Register(() => CDSService.IsInstalled(package.Instance)).Return(true);
                    values = new object[] { package.Instance, PackageSearchType.Local };
                    result = converter.Convert(values, typeof(string), null, null) as string;
                    Assert.AreEqual(TextResources.Uninstall, result);

                    service.Register(() => CDSService.IsInstalled(package.Instance)).Return(false);
                    values = new object[] { package.Instance, PackageSearchType.Online };
                    result = converter.Convert(values, typeof(string), null, null) as string;
                    Assert.AreEqual(TextResources.Install, result);

                    values = new object[] { package.Instance, PackageSearchType.Update };
                    result = converter.Convert(values, typeof(string), null, null) as string;
                    Assert.AreEqual(TextResources.Update, result);

                    TestUtilities.Assert_ShouldThrow<NotImplementedException>(() =>
                    {
                        values = new object[] { package.Instance, (PackageSearchType)int.MaxValue };
                        result = converter.Convert(values, typeof(string), null, null) as string;
                    });
                }
            }
        }

        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void Aconvert_TestPackageStatusConverterConvertBack()
        {
            PackageStatusConverter converter = new PackageStatusConverter();
            TestUtilities.Assert_ShouldThrow<NotImplementedException>(() =>
            {
                converter.ConvertBack("", null, null, null);
            });
        }
    }
}
