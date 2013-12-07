using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.CDS;
using System.Threading;
using System.Collections.Specialized;
using System.Collections;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.UnitTest
{
    [TestClass]
    public class CDSIntegrationViewModelUnitTest
    {
        [TestMethod()]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void TestCDSRepository()
        {
            CDSRepository repo = new CDSRepository()
            {
                IsEnabled = true,
                Name = "repo1",
                Source = "source",
            };
            Assert.AreEqual(true, repo.IsEnabled);
            Assert.AreEqual("repo1", repo.Name);
            Assert.AreEqual("source", repo.Source);
            Assert.AreEqual(repo.Name, repo.ToString());
        }

        [TestMethod()]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void TestCDSIntegrationViewModelSave()
        {
            CDSRepository repo = new CDSRepository() {
                IsEnabled = true,
                Name = "repo1",
                Source = "source",
            };
            NugetConfigManager_Accessor.repositories = new List<CDSRepository>() { repo };

            CDSIntegrationViewModel_Accessor vm = new CDSIntegrationViewModel_Accessor();
            Assert.IsTrue(vm.HasSaved);

            vm.RepositoryPropertyChanged(null, null);
            Assert.IsFalse(vm.HasSaved);

            vm.Save();
            Assert.IsTrue(vm.HasSaved);

            vm.RepositoriesCollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<CDSRepository> { repo }));
            Assert.IsFalse(vm.HasSaved);

            vm.Save();
            vm.RepositoriesCollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<CDSRepository> { repo }));
            Assert.IsFalse(vm.HasSaved);

            vm.Repositories.Add(repo);
            using (var service = new ImplementationOfType(typeof(MessageBoxService)))
            {
                bool hasError = false;
                service.Register(() => MessageBoxService.ShowError(Argument<string>.Any)).Execute<MessageBoxResult>(() =>
                {
                    hasError = true;
                    return MessageBoxResult.OK;
                });
                vm.Save();
                Assert.IsTrue(hasError);
                Assert.IsFalse(vm.HasSaved);
            }
        }

        [TestMethod()]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void TestCDSIntegrationViewModelCommands()
        {
            NugetConfigManager_Accessor.repositories = new List<CDSRepository>();
            CDSIntegrationViewModel vm = new CDSIntegrationViewModel();
            Assert.IsTrue(vm.AddCommand.CanExecute());
            Assert.IsFalse(vm.RemoveCommand.CanExecute());
            Assert.IsFalse(vm.UpCommand.CanExecute());
            Assert.IsFalse(vm.DownCommand.CanExecute());
            Assert.IsFalse(vm.UpdateCommand.CanExecute());

            vm.AddCommand.Execute();
            Assert.AreEqual(1, vm.Repositories.Count);
            Assert.IsTrue(vm.RemoveCommand.CanExecute());
            Assert.IsTrue(vm.UpCommand.CanExecute());
            Assert.IsTrue(vm.DownCommand.CanExecute());
            Assert.IsTrue(vm.UpdateCommand.CanExecute());

            vm.AddCommand.Execute();
            Assert.AreEqual(2, vm.Repositories.Count);
            vm.UpCommand.Execute();
            vm.DownCommand.Execute();
            vm.UpdateCommand.Execute();
            vm.RemoveCommand.Execute();
            Assert.AreEqual(1, vm.Repositories.Count);
            Assert.IsNull(vm.SelectedRepository);

            vm.SelectedRepository = vm.Repositories.Single();
            Assert.AreEqual(vm.SelectedRepository.IsEnabled, vm.DisplayingRepository.IsEnabled);
            Assert.AreEqual(vm.SelectedRepository.Name, vm.DisplayingRepository.Name);
            Assert.AreEqual(vm.SelectedRepository.Source, vm.DisplayingRepository.Source);

            using (var service = new ImplementationOfType(typeof(MessageBoxService)))
            {
                bool hasError = false;
                service.Register(() => MessageBoxService.ShowError(Argument<string>.Any)).Execute<MessageBoxResult>(() =>
                {
                    hasError = true;
                    return MessageBoxResult.OK;
                });

                vm.DisplayingRepository.Source = "invalidUri";
                vm.UpdateCommand.Execute();
                Assert.IsTrue(hasError);
            }
        }
    }
}
