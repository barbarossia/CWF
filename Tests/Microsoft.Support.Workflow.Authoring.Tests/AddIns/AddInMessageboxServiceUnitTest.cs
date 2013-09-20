using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.DynamicImplementations;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.Tests.AddIns
{
    [TestClass]
    public class AddInMessageboxServiceUnitTest
    {
        [TestMethod]
        [TestCategory("Unittest")]
        [Owner("v-kason")]
        public void AddInMessageBoxService_TestPrintMessage()
        {
            using (var messageBox = new ImplementationOfType(typeof(AddInMessageBoxService)))
            {
                messageBox.Register(() => AddInMessageBoxService.Show(Argument<string>.Any, Argument<string>.Any,Argument<MessageBoxButton>.Any,Argument<MessageBoxImage>.Any))
                    .Return(MessageBoxResult.Yes);
                AddInMessageBoxService.PrintFailed("");
                AddInMessageBoxService.PrintNoneActivityMessage();
                AddInMessageBoxService.PrintOverflowWorkflow("");
                AddInMessageBoxService.PrintNoneSelectMessage();
                Assert.IsTrue(AddInMessageBoxService.PrintConfirmation(0, "", ""));
                Assert.IsTrue(AddInMessageBoxService.PrintReselectConfirmation());
            }
        }
    }
}
