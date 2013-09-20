using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using System.Collections.ObjectModel;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;
using Microsoft.Support.Workflow.Authoring.Views;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels
{
    [TestClass]
    public class OpenActivityConfirmationViewModelUnitTest
    {
        [WorkItem(322377)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void OpenActivity_PropertyChangedNotificationsAreRaised()
        {
            var vm = new OpenActivityConfirmationViewModel(new OpenActivityConfirmation(), "title", "message");
            vm.DontSaveClickedCommand.Execute();
            vm.SaveClickedCommand.Execute();
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Title", () => vm.Title = "title");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Message", () => vm.Message = "message");
            Assert.AreEqual(vm.Title, "title");
            Assert.AreEqual(vm.Message, "message");
        }
    }
}
