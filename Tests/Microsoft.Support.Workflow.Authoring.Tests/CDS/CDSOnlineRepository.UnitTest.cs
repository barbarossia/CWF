using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.CDS;
using Microsoft.Support.Workflow.Authoring.Common;
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
    public class CDSOnlineRepositoryUnitTest
    {
        private List<CDSRepository> Settings = new List<CDSRepository>();

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-Dif")]
        public void Search()
        {
            int startIndex = 0;
            int pageSize = 10;
            CDSSortByType orderBy = CDSSortByType.PublishedDate;
            string searchTerm = "DefaultUrlUsed";
            string source = "Test Source";
            bool isLatestVersion = true;

            IPackageRepositoryFactory factory = CDSRepositoryHelper.CreatePackageRepositoryFactory();

            using (var manager = new ImplementationOfType(typeof(NugetConfigManager)))
            {
                manager.Register(() => NugetConfigManager.Repositories).Return(Settings);

                var onlineRepository = new Implementation<CDSOnlineRepository>();
                onlineRepository.Register(inst => inst.SourceProvider).Return(CDSRepositoryHelper.GetSourceProvider());
                onlineRepository.Register(inst => inst.RepositoryFactory).Return(factory);

                onlineRepository.Instance.Source = source;
                onlineRepository.Instance.OrderBy = orderBy;
                onlineRepository.Instance.SearchTerm = searchTerm;
                onlineRepository.Instance.IsLatestVersion = isLatestVersion;
                onlineRepository.Instance.Search(startIndex, pageSize);
            }

        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-Dif")]
        public void Install()
        {
            var packageA = PackageUtility.CreatePackage("A");

            using (var manager = new ImplementationOfType(typeof(NugetConfigManager)))
            {
                manager.Register(() => NugetConfigManager.Repositories).Return(Settings);

                using (var onlineRepository = new Implementation<CDSOnlineRepository>())
                {
                    onlineRepository.Register(inst => inst.CreatePackageManager()).Return(CDSRepositoryHelper.GetPackageManager(packageA));
                    onlineRepository.Instance.InstallPackage(packageA.Id, packageA.Version);
                }
            }
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-Dif")]
        public void CreatePackageManager()
        {
            var packageA = PackageUtility.CreatePackage("A");
            IPackageRepositoryFactory factory = CDSRepositoryHelper.CreatePackageRepositoryFactory();

            using (var manager = new ImplementationOfType(typeof(NugetConfigManager)))
            {
                manager.Register(() => NugetConfigManager.Repositories).Return(Settings);

                using (var onlineRepository = new Implementation<CDSOnlineRepository>())
                {
                    onlineRepository.Register(inst => inst.SourceProvider).Return(CDSRepositoryHelper.GetSourceProvider());
                    onlineRepository.Register(inst => inst.RepositoryFactory).Return(factory);

                    onlineRepository.Instance.CreatePackageManager();
                }
            }
        }
    }
}
