using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Activities.Statements;
using System.Activities.Presentation.View;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

namespace Authoring.Tests.Unit
{
    [TestClass]
    public class XamlServiceUnitTests
    {
        [WorkItem(321667)]
        [Description("Test XamlService removes sap view state for perf and readability")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void XamlService_SerializeToXaml_RemovesIgnorableNamespace()
        {
            // Arrange
            var seq = new Sequence();
            // Attach useless ViewState object
            WorkflowViewStateService.SetViewState(seq, new Dictionary<string, object>());
            var xaml = XamlService.SerializeToXaml(seq);
            Assert.IsTrue(xaml.Contains("WorkflowViewStateService.ViewState"), "sap:WorkflowViewStateService.ViewState should not appear in generated XAML");
        }

        [WorkItem(321668)]
        [Description("Test XamlService.SerializeToXaml can write out full CLR namespaces")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void XamlService_SerializeToXaml_SerializesFullClrNamespaceIffRequested()
        {
            // Arrange
            var input = new XamlInput { Number = 42 };
            var xaml = XamlService.SerializeToXaml(input, fullyQualifiedClrNamespaces:false);
            Assert.AreEqual(@"<XamlInput Number=""42"" xmlns=""clr-namespace:Authoring.Tests.Unit;assembly=Microsoft.Support.Workflow.Authoring.Tests"" />", xaml);
            xaml = XamlService.SerializeToXaml(input, fullyQualifiedClrNamespaces:true);
            Assert.AreEqual(@"<XamlInput Number=""42"" xmlns=""clr-namespace:Authoring.Tests.Unit;assembly=Microsoft.Support.Workflow.Authoring.Tests, Version=1.3.0.7, Culture=neutral, PublicKeyToken=null"" />", xaml);
        }

    }

    public class XamlInput
    {
        public int Number { get; set; }
    }
}
