using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using System.Windows.Data;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Collections;
using Microsoft.DynamicImplementations;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using Microsoft.Support.Workflow.Authoring.Security;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.UnitTest
{
    [TestClass]
    public class DefaultValueSettingsViewModelUnitTest
    {
        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void DefaultValueSettingsViewModel_TestPropertyChanged()
        {
            DefaultValueSettingsViewModel vm = new DefaultValueSettingsViewModel();
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "EnableDownloadDependencies", () => vm.EnableDownloadDependencies = true);
            Assert.AreEqual(vm.EnableDownloadDependencies, true);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "OpenForEditing", () => vm.OpenForEditing = OpenMode.Editing);
            Assert.AreEqual(vm.OpenForEditing, OpenMode.Editing);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "DefaultEnv", () => vm.DefaultEnv = Env.Dev);
            Assert.AreEqual(vm.DefaultEnv, Env.Dev);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "Envs", () => vm.Envs = null);
            Assert.AreEqual(vm.Envs, null);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SearchWorkflowScope", () => vm.SearchWorkflowScope = SearchScope.SearchCurrentWorkflow);
            Assert.AreEqual(vm.SearchWorkflowScope, SearchScope.SearchCurrentWorkflow);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "EnableTaskAssignment", () => vm.EnableTaskAssignment = true);
            Assert.IsTrue(vm.EnableTaskAssignment);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void DefaultValueSettingsViewModel_TestSaveCommand()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {

                DefaultValueSettingsViewModel vm = new DefaultValueSettingsViewModel();
                vm.DefaultEnv = Env.Test;
                vm.EnableTaskAssignment = true;
                vm.EnableDownloadDependencies = false;
                vm.SearchWorkflowScope = SearchScope.SearchCurrentWorkflow;
                vm.OpenForEditing = OpenMode.Readonly;
                vm.Save();
                Assert.IsTrue(vm.EnableTaskAssignment);
                Assert.IsFalse(vm.EnableDownloadDependencies);
                Assert.AreEqual(vm.SearchWorkflowScope, SearchScope.SearchCurrentWorkflow);
                Assert.AreEqual(vm.OpenForEditing, OpenMode.Readonly);
                Assert.AreEqual(vm.DefaultEnv, Env.Test);
            });

        }

    }
}
