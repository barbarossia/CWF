using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;

namespace Authoring.Tests.Functional
{
    [TestClass]
    public class CDSIntegrationViewModelTest
    {
        #region test methods

        [WorkItem(503249)]
        [Description("Verify the add button of CDS Integration")]
        [Owner("v-demisu")]
        [TestMethod()]
        public void VerifyCDSAddPackageSource()
        {
            //Click the "Add" button and the default package source will be added.
            CDSIntegrationViewModel cdsvm = new CDSIntegrationViewModel();
            Assert.IsTrue(cdsvm.AddCommand.CanExecute());
            int currentCount=cdsvm.Repositories.Count();
            cdsvm.AddCommand.Execute();
            Assert.IsTrue(cdsvm.Repositories.Count == currentCount + 1);
            Assert.IsTrue(cdsvm.SelectedRepository.IsEnabled);
            Assert.IsTrue(cdsvm.SelectedRepository.Name.Equals("Package Source"));
            Assert.IsTrue(cdsvm.SelectedRepository.Source.Equals("http://packagesource"));
        }

        [WorkItem(503250)]
        [Description("Verify the Remove button of CDS Integration")]
        [Owner("v-demisu")]
        [TestMethod()]
        public void VerifyCDSRemovePackageSource()
        {
            //Select a package source and click "Remove" button, the selected package source will be remove.
            CDSIntegrationViewModel cdsvm = new CDSIntegrationViewModel();
            cdsvm.AddCommand.Execute();
            Assert.IsTrue(cdsvm.RemoveCommand.CanExecute());
            int currentCount = cdsvm.Repositories.Count();
            cdsvm.RemoveCommand.Execute();
            Assert.IsTrue(cdsvm.Repositories.Count == currentCount - 1);
        }

        [WorkItem(503262)]
        [Description("Verify the Up button of CDS Integration")]
        [Owner("v-demisu")]
        [TestMethod()]
        public void VerifyCDSMoveUpPackageSource()
        {
            //Select a package source and click "Up" button, the selected package source will be move up one index
            CDSIntegrationViewModel cdsvm = new CDSIntegrationViewModel();
            cdsvm.AddCommand.Execute();
            cdsvm.AddCommand.Execute();
            Assert.IsTrue(cdsvm.UpCommand.CanExecute());
            int currentIndex = cdsvm.Repositories.IndexOf(cdsvm.SelectedRepository);
            cdsvm.UpCommand.Execute();
            Assert.IsTrue(cdsvm.Repositories.IndexOf(cdsvm.SelectedRepository) == currentIndex - 1);
        }

        [WorkItem(503263)]
        [Description("Verify the Down button of CDS Integration")]
        [Owner("v-demisu")]
        [TestMethod()]
        public void VerifyCDSMoveDownPackageSource()
        {
            //Select a package source and click "Down" button, the selected package source will be move down one index
            CDSIntegrationViewModel cdsvm = new CDSIntegrationViewModel();
            cdsvm.AddCommand.Execute();
            cdsvm.AddCommand.Execute();
            cdsvm.UpCommand.Execute();
            Assert.IsTrue(cdsvm.DownCommand.CanExecute());
            int currentIndex = cdsvm.Repositories.IndexOf(cdsvm.SelectedRepository);
            cdsvm.DownCommand.Execute();
            Assert.IsTrue(cdsvm.Repositories.IndexOf(cdsvm.SelectedRepository) == currentIndex + 1);
        }

        [WorkItem(503273)]
        [Description("Verify that the name and source can be updated on CDS Integration")]
        [Owner("v-demisu")]
        [TestMethod()]
        public void VerifyCDSUpdatePackageSource()
        {
            //Select a package and modify "Name" and "Source" fields, and click "Update" button.
            CDSIntegrationViewModel cdsvm = new CDSIntegrationViewModel();
            cdsvm.AddCommand.Execute();
            Assert.IsTrue(cdsvm.SelectedRepository.IsEnabled);
            Assert.IsTrue(cdsvm.SelectedRepository.Name.Equals("Package Source"));
            Assert.IsTrue(cdsvm.SelectedRepository.Source.Equals("http://packagesource"));
            Assert.IsTrue(cdsvm.UpdateCommand.CanExecute());
            cdsvm.DisplayingRepository.Name = "MyPackage 123";
            cdsvm.DisplayingRepository.Source = "http://MyPackageSource123";
            cdsvm.UpdateCommand.Execute();
            Assert.IsTrue(cdsvm.SelectedRepository.IsEnabled);
            Assert.IsTrue(cdsvm.SelectedRepository.Name.Equals("MyPackage 123"));
            Assert.IsTrue(cdsvm.SelectedRepository.Source.Equals("http://MyPackageSource123"));
        }
        #endregion
    }
}
