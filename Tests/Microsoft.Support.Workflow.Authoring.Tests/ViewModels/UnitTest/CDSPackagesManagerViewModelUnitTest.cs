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
using NuGet;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.Models;

namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.UnitTest
{
    [TestClass]
    public class CDSPackagesManagerViewModelUnitTest
    {
        [TestMethod()]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void TestPackagesManagerViewModel()
        {
            CDSPackagesManagerViewModel vm = new CDSPackagesManagerViewModel();
            
            Assert.IsNotNull(vm.DataPagingVM);
            Assert.AreEqual(CDSPackagesManagerViewModel_Accessor.pageSize, vm.DataPagingVM.PageSize);
            CollectionAssert.AreEqual(NugetConfigManager.EnabledRepositories.ToList(), vm.Repositories.ToList());
        }

        [TestMethod()]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void TestPackagesManagerViewModelProperties()
        {
            CDSPackagesManagerViewModel vm = new CDSPackagesManagerViewModel();

            vm.SearchType = PackageSearchType.Local;
            Assert.AreEqual("Installed:", vm.DateHeader);
            Assert.IsFalse(vm.IsLatestOnlyVisible);

            vm.SearchType = PackageSearchType.Online;
            Assert.AreEqual("Published:", vm.DateHeader);
            Assert.IsTrue(vm.IsLatestOnlyVisible);

            Assert.IsNotNull(vm.SortByOptions);

            using (var package = GetPackage())
            {
                vm.SelectedPackage = package.Instance;

                Assert.AreEqual("author", vm.SelectedPackageAuthors);
                Assert.AreEqual("No dependencies", vm.SelectedPackageDependencies);
            }
        }
        
        [TestMethod()]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        public void TestPackagesManagerViewModelCommands()
        {
            NugetConfigManager_Accessor.repositories = new List<CDSRepository>() {
                new CDSRepository() {
                    IsEnabled = true,
                    Name = "name",
                    Source = "source"
                }
            };

            CDSPackagesManagerViewModel vm = new CDSPackagesManagerViewModel();

            Assert.IsTrue(vm.SettingsCommand.CanExecute());
            using (var dialogService = new ImplementationOfType(typeof(DialogService)))
            {
                dialogService.Register(() => DialogService.ShowDialog(Argument<object>.Any)).Execute<Nullable<bool>>(() =>
                {
                    NugetConfigManager_Accessor.repositories = new List<CDSRepository>();
                    return null;
                });
                vm.SettingsCommand.Execute();
                Assert.IsNull(vm.Packages);
            }

            vm.SelectedPackage = null;
            Assert.IsFalse(vm.PackageCommand.CanExecute());
            using (var package = GetPackage())
            {
                vm.Packages = new Authoring.Common.PagedList<IPackage>(new List<IPackage>() { package.Instance }, 0, CDSPackagesManagerViewModel_Accessor.pageSize, 1);
                vm.SelectedPackage = package.Instance;
                vm.Source = "name";
                vm.SearchText = "";

                using (var messageService = new ImplementationOfType(typeof(MessageBoxService)))
                {
                    bool hasError = false;
                    messageService.Register(() => MessageBoxService.ShowError(Argument<string>.Any)).Execute<MessageBoxResult>(() =>
                    {
                        hasError = true;
                        return MessageBoxResult.OK;
                    });

                    using (var cdsService = new ImplementationOfType(typeof(CDSService)))
                    {
                        cdsService.Register(() => CDSService.IsInstalled(package.Instance)).Return(true);
                        cdsService.Register(() => CDSService.IsPackageDependentOn(package.Instance)).Return(false);
                        cdsService.Register(() => CDSService.Uninstall(package.Instance.Id, package.Instance.Version)).Return(true);
                        cdsService.Register(() => CDSService.SearchLocal(Argument<int>.Any, Argument<int>.Any, Argument<CDSSortByType>.Any, Argument<string>.Any)).Return(vm.Packages);
                        cdsService.Register(() => CDSService.SearchOnline(Argument<int>.Any, Argument<int>.Any, Argument<string>.Any, Argument<CDSSortByType>.Any, Argument<string>.Any, Argument<bool>.Any)).Execute<PagedList<IPackage>>(() =>
                        {
                            throw new CDSPackageException();
                        });
                        cdsService.Register(() => CDSService.SearchUpdate(Argument<int>.Any, Argument<int>.Any, Argument<string>.Any, Argument<CDSSortByType>.Any, Argument<string>.Any, Argument<bool>.Any)).Execute<PagedList<IPackage>>(() =>
                        {
                            throw new Exception();
                        });

                        Assert.IsTrue(vm.PackageCommand.CanExecute());

                        vm.SearchType = PackageSearchType.Local;
                        vm.PackageCommand.Execute();
                        Assert.AreEqual(1, vm.DataPagingVM.ResultsLength);

                        vm.SearchType = PackageSearchType.Online;
                        vm.PackageCommand.Execute();
                        Assert.IsTrue(hasError);

                        hasError = false;
                        vm.SearchType = PackageSearchType.Update;
                        vm.PackageCommand.Execute();
                        Assert.IsTrue(hasError);

                        hasError = false;
                        vm.SelectedSortByOption = CDSSortByType.NameAscending;
                        Assert.IsTrue(hasError);

                        hasError = false;
                        vm.LatestOnly = true;
                        Assert.IsTrue(hasError);
                    }

                    using (var service = new ImplementationOfType(typeof(CDSService)))
                    {
                        service.Register(() => CDSService.IsInstalled(package.Instance)).Return(false);
                        service.Register(() => CDSService.Install(Argument<string>.Any, package.Instance.Id, package.Instance.Version)).Execute<bool>(() =>
                        {
                            throw new CDSPackageException();
                        });

                        hasError = false;
                        Assert.IsTrue(vm.PackageCommand.CanExecute());
                        vm.PackageCommand.Execute();
                        Assert.IsTrue(hasError);
                    }
                }
            }
        }

        private Implementation<IPackage> GetPackage()
        {
            var package = new Implementation<IPackage>();
            package.Register(p => p.Id).Return("Id");
            package.Register(p => p.Version).Return(new SemanticVersion("1.0.0"));
            package.Register(p => p.Authors).Return(new string[] { "author" });
            package.Register(p => p.DependencySets).Return(new List<PackageDependencySet>());
            return package;
        }
    }
}
