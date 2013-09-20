using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.UIControls;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.DynamicImplementations;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Support.Workflow.Authoring.AddIns;

namespace Microsoft.Support.Workflow.Authoring.Tests.UIControl
{
    [TestClass]
    public class CustomToolTip_UnitTest
    {
        [TestMethod]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        public void CustomToolTip_CheckCanExecuteMethod()
        {
            using (var provider = new ImplementationOfType(typeof(HelpProvider)))
            {
                CustomToolTip tip = new CustomToolTip();
                PrivateObject po = new PrivateObject(tip);
                FrameworkElement fe = new FrameworkElement();
                bool returnValue = Convert.ToBoolean(po.Invoke("CanExecute", fe));
                Assert.IsFalse(returnValue);

                provider.Register(() => HelpProvider.GetHelpTitle(Argument<DependencyObject>.Any)).Return("Title");
                provider.Register(() => HelpProvider.GetHelpAction(Argument<DependencyObject>.Any)).Return("Action");
                returnValue = Convert.ToBoolean(po.Invoke("CanExecute", fe));
                Assert.IsTrue(returnValue);
               
            }
        }

        [TestMethod]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        public void CustomToolTip_CheckMyToolTip_OpenedMethod()
        {
            using (var provider = new ImplementationOfType(typeof(HelpProvider)))
            {
                CustomToolTip tip = new CustomToolTip();
                PrivateObject po = new PrivateObject(tip);
                ToolTip tip1 = new ToolTip();
                tip1.PlacementTarget = new UIElement();
                RoutedEventArgs args = new RoutedEventArgs();
                provider.Register(() => HelpProvider.GetHelpTitle(Argument<DependencyObject>.Any)).Return("Title");
                provider.Register(() => HelpProvider.GetHelpAction(Argument<DependencyObject>.Any)).Return("Action");
                po.Invoke("MyToolTip_Opened", tip1 as object, args);
                Assert.AreEqual(tip.Action, "Action");
                Assert.AreEqual(tip.Title, "Title");
            }
        }
    }
}
