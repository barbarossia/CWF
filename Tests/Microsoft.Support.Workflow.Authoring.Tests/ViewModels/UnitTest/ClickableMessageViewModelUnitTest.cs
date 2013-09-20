using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.DynamicImplementations;
using System.Diagnostics;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels
{
    [TestClass]
    public class ClickableMessageViewModelUnitTest
    {
        [WorkItem(322379)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void ClickableMessage_PropertyChangedNotificationsAreRaised()
        {
            var vm = new ClickableMessageViewModel();
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Title", () => vm.Title = "Url");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Url", () => vm.Url = "mainwindow");
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Message", () => vm.Message = "Message");
            vm = new ClickableMessageViewModel("", "", "");
            vm.UrlClickedCommand.Execute();
            Assert.IsTrue(string.IsNullOrEmpty(vm.Message));
            Assert.IsTrue(string.IsNullOrEmpty(vm.Title));
            Assert.IsTrue(string.IsNullOrEmpty(vm.Url));
        }

        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void ClickableMessage_VerifyUrlClickCommandExecute()
        {
            using (var mock = new ImplementationOfType(typeof(Process)))
            {
                bool isStart = false;
                mock.Register(() => Process.Start(Argument<string>.Any)).Execute(() =>
                {
                    isStart = true;
                    Process process = null;
                    return process;
                });
                var vm = new ClickableMessageViewModel();
                vm.Url = "url";
                vm.UrlClickedCommand.Execute();
                Assert.IsTrue(isStart);
            }
        }

        [TestCleanup]
        public void TestCleanup() { System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); }
    }
}
