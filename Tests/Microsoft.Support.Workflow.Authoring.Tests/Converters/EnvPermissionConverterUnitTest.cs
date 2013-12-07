using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Threading;
using Microsoft.Support.Workflow.Authoring.AddIns.Converters;
using Microsoft.Support.Workflow.Authoring.CDS;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Security;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.Tests.Converters
{
    [TestClass]
    public class EnvPermissionConverterUnitTest
    {
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void Aconvert_TestEnvPermissionConverterConvert()
        {
            EnvPermissionConverter converter = new EnvPermissionConverter();

            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
            {
                converter.Convert(null, null, 1, null);
            });

            using (var impl = new ImplementationOfType(typeof(AuthorizationService)))
            {
                impl.Register(() => AuthorizationService.Validate(Env.Dev, Permission.SaveWorkflow)).Return(true);
                impl.Register(() => AuthorizationService.Validate(Env.Stage, Permission.SaveWorkflow)).Return(false);
                impl.Register(() => AuthorizationService.Validate((Env)0, Permission.SaveWorkflow)).Return(false);

                Visibility visibility = (Visibility)converter.Convert(Env.Dev, typeof(Visibility), Permission.SaveWorkflow.ToString(), null);
                Assert.AreEqual(Visibility.Visible, visibility);

                visibility = (Visibility)converter.Convert("Stage", typeof(Visibility), Permission.SaveWorkflow, null);
                Assert.AreEqual(Visibility.Collapsed, visibility);

                bool hasPermission = (bool)converter.Convert(Env.Stage, typeof(bool), Permission.SaveWorkflow, null);
                Assert.IsFalse(hasPermission);
            }
        }

        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void Aconvert_TestEnvPermissionConverterConvertBack()
        {
            EnvPermissionConverter converter = new EnvPermissionConverter();
            TestUtilities.Assert_ShouldThrow<NotImplementedException>(() =>
            {
                converter.ConvertBack("", null, null, null);
            });
        }
    }
}
