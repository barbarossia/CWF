using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.CDS;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.Tests.CDS
{
    [TestClass]
    public class CDSServiceUnitTest
    {
        private List<CDSRepository> Settings = new List<CDSRepository>();

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-Dif")]
        public void CDSServiceSearchOnline()
        {
            int startIndex = 0;
            int pageSize = 10;
            CDSSortByType orderBy = CDSSortByType.PublishedDate;
            string searchTerm = "aaa";
            string source = "Test Source";
            bool isLatestVersion = true;
            var result = new [] { PackageUtility.CreatePackage("A") }.ToList();

            using (var repository = new Implementation<CDSOnlineRepository>())
            {
                using (var manager = new ImplementationOfType(typeof(NugetConfigManager)))
                {
                    manager.Register(() => NugetConfigManager.Repositories).Return(Settings);
                    repository.Register(
                        inst => inst.Search(startIndex, pageSize)).Execute(() =>
                            new PagedList<IPackage>(result, 0, 7, 1));

                    CDSService_Accessor.onlineRepository = repository.Instance;
                    CDSService.SearchOnline(startIndex, pageSize, source, orderBy, searchTerm, isLatestVersion);
                }
            }

        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-Dif")]
        public void CDSServiceSearchLocal()
        {
            int startIndex = 0;
            int pageSize = 10;
            CDSSortByType orderBy = CDSSortByType.PublishedDate;
            string searchTerm = "aaa";
            var result = new[] { PackageUtility.CreatePackage("A") }.ToList();

            using (var repository = new Implementation<CDSInstalledRepository>())
            {
                using (var manager = new ImplementationOfType(typeof(NugetConfigManager)))
                {
                    manager.Register(() => NugetConfigManager.Repositories).Return(Settings);
                    repository.Register(
                        inst => inst.Search(startIndex, pageSize)).Execute(() =>
                            new PagedList<IPackage>(result, 0, 7, 1));

                    CDSService_Accessor.installedRepository = repository.Instance;
                    CDSService.SearchLocal(startIndex, pageSize, orderBy, searchTerm);
                }
            }
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-Dif")]
        public void CDSServiceSearchUpdate()
        {
            int startIndex = 0;
            int pageSize = 10;
            CDSSortByType orderBy = CDSSortByType.PublishedDate;
            string source = "Test Source";
            string searchTerm = "aaa";
            var result = new[] { PackageUtility.CreatePackage("A") }.ToList();

            using (var repository = new Implementation<CDSUpdateRepository>())
            {
                using (var manager = new ImplementationOfType(typeof(NugetConfigManager)))
                {
                    manager.Register(() => NugetConfigManager.Repositories).Return(Settings);
                    repository.Register(
                        inst => inst.Search(startIndex, pageSize)).Execute(() =>
                            new PagedList<IPackage>(result, 0, 7, 1));

                    CDSService_Accessor.updateRepository = repository.Instance;
                    CDSService.SearchUpdate(startIndex, pageSize, source, orderBy, searchTerm);
                }
            }
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-Dif")]
        public void CDSServiceInstall()
        {
            string source = "Test Source";
            var packageA = PackageUtility.CreatePackage("A");

            using (var repository = new Implementation<CDSInstalledRepository>())
            {
                using (var manager = new ImplementationOfType(typeof(NugetConfigManager)))
                {
                    manager.Register(() => NugetConfigManager.Repositories).Return(Settings);
                    repository.Register(
                        inst => inst.InstallPackage(packageA.Id, packageA.Version)).Return(true);

                    CDSService_Accessor.installedRepository = repository.Instance;
                    CDSService.Install(source, packageA.Id, packageA.Version);
                }
            }
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-Dif")]
        public void CDSServiceIsInstall()
        {
            var packageA = PackageUtility.CreatePackage("A");

            using (var repository = new Implementation<CDSInstalledRepository>())
            {
                using (var manager = new ImplementationOfType(typeof(NugetConfigManager)))
                {
                    manager.Register(() => NugetConfigManager.Repositories).Return(Settings);
                    repository.Register(
                        inst => inst.IsInstalled(packageA.Id, packageA.Version)).Return(true);

                    CDSService_Accessor.installedRepository = repository.Instance;
                    CDSService.IsInstalled(packageA);
                }
            }
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-Dif")]
        public void CDSServiceIsPackageDependentOn()
        {
            var packageA = PackageUtility.CreatePackage("A");

            using (var repository = new Implementation<CDSInstalledRepository>())
            {
                using (var manager = new ImplementationOfType(typeof(NugetConfigManager)))
                {
                    manager.Register(() => NugetConfigManager.Repositories).Return(Settings);
                    repository.Register(
                        inst => inst.IsPackageDependentOn(packageA.Id, packageA.Version)).Return(true);

                    CDSService_Accessor.installedRepository = repository.Instance;
                    CDSService.IsPackageDependentOn(packageA);
                }
            }
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-Dif")]
        public void CDSServiceUnInstall()
        {
            var packageA = PackageUtility.CreatePackage("A");

            using (var repository = new Implementation<CDSInstalledRepository>())
            {
                using (var manager = new ImplementationOfType(typeof(NugetConfigManager)))
                {
                    manager.Register(() => NugetConfigManager.Repositories).Return(Settings);
                    repository.Register(
                        inst => inst.UninstallPackage(packageA.Id, packageA.Version, false)).Return(true);

                    CDSService_Accessor.installedRepository = repository.Instance;
                    CDSService.Uninstall(packageA.Id, packageA.Version);
                }
            }
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-Dif")]
        public void CDSServiceUpdate()
        {
            string source = "Test Source";
            var packageA = PackageUtility.CreatePackage("A");

            using (var repository = new Implementation<CDSUpdateRepository>())
            {
                using (var manager = new ImplementationOfType(typeof(NugetConfigManager)))
                {
                    manager.Register(() => NugetConfigManager.Repositories).Return(Settings);
                    repository.Register(
                        inst => inst.UpdatePackage(packageA.Id, packageA.Version)).Return(true);

                    CDSService_Accessor.updateRepository = repository.Instance;
                    CDSService.Update(source, packageA.Id, packageA.Version);
                }
            }
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-Dif")]
        [ExpectedException(typeof(CDSPackageException))]
        public void CDSServiceException()
        {
            string source = "Test Source";
            var packageA = PackageUtility.CreatePackage("A");

            using (var repository = new Implementation<CDSInstalledRepository>())
            {
                using (var manager = new ImplementationOfType(typeof(NugetConfigManager)))
                {
                    manager.Register(() => NugetConfigManager.Repositories).Return(Settings);
                    repository.Register(
                        inst => inst.InstallPackage(packageA.Id, packageA.Version)).Throw(new Exception("err"));

                    CDSService_Accessor.installedRepository = repository.Instance;
                    CDSService.Install(source, packageA.Id, packageA.Version);
                }
            }
        }
    }
}
