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
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels
{
    [TestClass]
    public class IntellisensePopupViewModelUnitTest
    {
        [WorkItem(322309)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void IntellisensePopup_PropertyChangedNotificationsAreRaised()
        {
            var vm = new IntellisensePopupViewModel();
            ObservableCollection<TreeNode> tree = new ObservableCollection<TreeNode>();
            TreeNode node = new TreeNode();
            tree.Add(node);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "TreeNodes", () => vm.TreeNodes = tree);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedItem", () => vm.SelectedItem = node);
            Assert.AreEqual(vm.ItemsCount, 1);
            vm.SelectedIndex = 0;
            Assert.AreEqual(vm.SelectedItem, node);
            vm.SelectedItem = null;
            Assert.IsTrue(vm.SelectedIndex < 0);
        }
    }
}
