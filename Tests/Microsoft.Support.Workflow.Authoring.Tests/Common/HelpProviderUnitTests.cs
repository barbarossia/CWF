using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Common;
using System.Windows;
using System.Windows.Input;
using Microsoft.Support.Workflow.Authoring.AddIns;

namespace Microsoft.Support.Workflow.Authoring.Tests.Common
{
    [TestClass]
    public class HelpProviderUnitTests
    {
        [WorkItem(348269)]
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void HelpProvider_Help()
        {
            DependencyObject obj = new DependencyObject();

            //setHelpKey,GetHelpKey
            Assert.IsTrue(HelpProvider.GetHelpKey(null).Equals(string.Empty));           
            HelpProvider.SetHelpKey(obj,"Call for help");
            Assert.AreEqual(HelpProvider.GetHelpKey(obj), "Call for help");

            //SetHelpTitle,GetHelpTitle
            Assert.IsTrue(HelpProvider.GetHelpTitle(null).Equals(string.Empty));
            HelpProvider.SetHelpTitle(obj,"Set help title");
            Assert.AreEqual(HelpProvider.GetHelpTitle(obj), "Set help title");

            //SetHelpAction,GetHelpAction
            Assert.IsTrue(HelpProvider.GetHelpAction(null).Equals(string.Empty));
            HelpProvider.SetHelpAction(obj,"Set help Action");
            Assert.AreEqual(HelpProvider.GetHelpAction(obj), "Set help Action");   
        }
    }
}