using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Threading;
using System.Windows;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Security;
using System.Security.Principal;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class BooleanToVisibilityConverterUnitTest
    {
        [WorkItem(321690)]
        [TestMethod]
        [Description("Check if BooleanToVisibilityConverter works properly.")]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestBooleanToVisibilityConverterConvert()
        {
            object value;
            object result;
            BooleanToVisibilityConverter converter = new BooleanToVisibilityConverter();

            using (var principal = new Implementation<WindowsPrincipal>())
            {
                Thread.CurrentPrincipal = principal.Instance;

                value = true;
                converter.VisibleWhen = true;
                result = converter.Convert(value, typeof(Visibility), null, null);
                Assert.AreEqual(typeof(Visibility), result.GetType());
                Assert.AreEqual(Visibility.Visible, (Visibility)result);

                value = null;
                converter.VisibleWhen = true;
                converter.CollapseWhenInvisible = true;
                result = converter.Convert(value, typeof(Visibility), null, null);
                Assert.AreEqual(typeof(Visibility), result.GetType());
                Assert.AreEqual(Visibility.Collapsed, (Visibility)result);

                value = null;
                converter.VisibleWhen = true;
                converter.CollapseWhenInvisible = false;
                result = converter.Convert(value, typeof(Visibility), null, null);
                Assert.AreEqual(typeof(Visibility), result.GetType());
                Assert.AreEqual(Visibility.Hidden, (Visibility)result);

                value = true;
                converter.VisibleWhen = true;
                result = converter.Convert(value, typeof(double), null, null);
                Assert.AreEqual(typeof(double), result.GetType());
                Assert.AreEqual(1D, (double)result);

                value = null;
                converter.VisibleWhen = true;
                converter.PartialHideWhenInvisible = true;
                result = converter.Convert(value, typeof(double), null, null);
                Assert.AreEqual(typeof(double), result.GetType());
                Assert.AreEqual(0.4D, (double)result);

                value = null;
                converter.VisibleWhen = true;
                converter.PartialHideWhenInvisible = false;
                result = converter.Convert(value, typeof(double), null, null);
                Assert.AreEqual(typeof(double), result.GetType());
                Assert.AreEqual(0D, (double)result);

                value = null;
                converter.Convert(value, typeof(int), null, null);
            }
        }

        [WorkItem(321692)]
        [TestMethod]
        [Description("Check BooleanToVisibilityConverter doesn't implement back conversion.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Aconvert_TestBooleanToVisibilityConverterConvertBack()
        {
            var converter = new BooleanToVisibilityConverter();
            var result = converter.ConvertBack(null, null, null, Thread.CurrentThread.CurrentCulture);
        }
    }
}
