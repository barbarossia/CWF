using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.UnitTest
{
    [TestClass]
    public class DataPagingViewModelUnitTest
    {
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void DataPaging_PropertyChangedNotificationsAreRaised() 
        {
            DataPagingViewModel vm = new DataPagingViewModel();
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "PageSize", () => vm.PageSize = 1);
            Assert.AreEqual(vm.PageSize,1);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "ResetPageIndex", () => vm.ResetPageIndex =true);
            Assert.AreEqual(vm.ResetPageIndex,true);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "PageIndex", () => vm.PageIndex = 1);
            Assert.AreEqual(vm.PageIndex, 1);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "ResultsLength", () => vm.ResultsLength = 36);
            Assert.AreEqual(vm.ResultsLength, 36);
            TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "TotalPages", () => vm.TotalPages = 2);
            Assert.AreEqual(vm.TotalPages, 2);
        }

        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void DataPaging_CheckCommandExecute()
        {
            //Can FirstPageCommand execute
            DataPagingViewModel vm = new DataPagingViewModel();
            vm.TotalPages = 3;
            vm.PageIndex = 2;
            vm.FirstPageCommand.Execute();
            Assert.AreEqual(1, vm.PageIndex);

            //Can PrevioustPageCommand execute
            
            vm.PageIndex = 3;
            vm.PreviousPageCommand.Execute();
            Assert.IsTrue(vm.PageIndex == 2);

            //Can NextPageCommand execute
            vm.TotalPages = 4;
            vm.PageIndex = 2;
            vm.NextPageCommand.Execute();
            Assert.AreEqual(vm.PageIndex, 3);

            //Can LastPageCommand execute
            vm.TotalPages = 3;
            vm.LastPageCommand.Execute();
            Assert.AreEqual(vm.PageIndex, 3);
        }

        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void DataPaging_CheckCommandCanExecute()
        {
            //Can FirstPageCommand execute
            DataPagingViewModel vm = new DataPagingViewModel();
            vm.TotalPages = 3;
            vm.PageIndex = 2;
            Assert.IsTrue(vm.FirstPageCommand.CanExecute());

            //Can PrevioustPageCommand execute
            vm.PageIndex = 3;
            Assert.IsTrue(vm.PreviousPageCommand.CanExecute());

            //Can NextPageCommand execute
            vm.TotalPages = 2;
            vm.PageIndex = 1;
            Assert.IsTrue(vm.NextPageCommand.CanExecute());

            //Can LastPageCommand execute
            vm.TotalPages = 3;
            Assert.IsTrue(vm.LastPageCommand.CanExecute());
        }

        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void DataPaging_CheckCalculatePages()
        {
            //Check pages when resetPageIndex = true
            DataPagingViewModel vm = new DataPagingViewModel();
            vm.PageSize = 5;
            vm.ResetPageIndex = true;
            vm.ResultsLength = 16;
            PrivateObject po = new PrivateObject(vm);
            po.Invoke("CalculatePages");
            Assert.AreEqual(vm.TotalPages, 4);
            Assert.AreEqual(vm.PageIndex, 1);
            Assert.IsFalse(vm.ResetPageIndex);

            //check pages when totalresults = 0
            vm.ResultsLength = 0;
            po.Invoke("CalculatePages");
            Assert.AreEqual(vm.PageIndex, 0);
        }

    }
}
