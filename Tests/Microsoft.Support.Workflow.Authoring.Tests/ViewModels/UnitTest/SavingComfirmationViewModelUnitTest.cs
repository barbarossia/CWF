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
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using System.Windows;
namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels
{
    [TestClass]
    public class SavingConfirmationViewModelUnitTest
    {
        [WorkItem(322307)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void SavingConfirmation_PropertyChangedNotificationsAreRaised()
        {
            var vm = new SavingConfirmationViewModel(new SavingConfirmation(), "title", "message",false,true,false);
            vm.DontSaveClickedCommand.Execute();
            vm.SaveClickedCommand.Execute();
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Title", () => vm.Title = "title");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Message", () => vm.Message = "message");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "UnlockVisibility", () => vm.UnlockVisibility = Visibility.Visible);

            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "ShouldUnlock", () => vm.ShouldUnlock = true);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SaveButtonContent", () => vm.SaveButtonContent = "button");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "DontSaveButtonContent", () => vm.DontSaveButtonContent = "button");
            Assert.AreEqual(vm.Title,"title");
            Assert.AreEqual(vm.Message, "message");
            Assert.AreEqual(vm.UnlockVisibility,"Visible");
            Assert.AreEqual(vm.CanKeepLocked, false);
            Assert.AreEqual(vm.ShouldUnlock, true);
            Assert.AreEqual(vm.SaveButtonContent, "button");
            Assert.AreEqual(vm.DontSaveButtonContent, "button");
            vm.ShouldUnlock = false;
            vm.SaveClickedCommand.Execute();

            vm = new SavingConfirmationViewModel(new SavingConfirmation(), "title", "message", true, true, false);
            vm.SaveClickedCommand.Execute();
            vm.DontSaveClickedCommand.Execute();
            Assert.AreEqual(vm.ShouldUnlock, true);
        }
    }
}
