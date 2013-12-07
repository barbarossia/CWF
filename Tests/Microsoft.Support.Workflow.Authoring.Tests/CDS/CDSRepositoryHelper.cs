using Microsoft.DynamicImplementations;
using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.Tests.CDS
{
    public static class CDSRepositoryHelper
    {
        private static string DefaultRepoUrl = "https://go.microsoft.com/fwlink/?LinkID=206669";

        public static IPackageRepository GetPackageRepository()
        {
            //Default Repository
            MockPackageRepository defaultPackageRepository = new MockPackageRepository();
            var packageA = PackageUtility.CreatePackage("DefaultUrlUsed", "1.0");
            defaultPackageRepository.AddPackage(packageA);
            var packageC = PackageUtility.CreatePackage("SearchPackage", "1.0");
            defaultPackageRepository.AddPackage(packageC);
            var packageD = PackageUtility.CreatePackage("AnotherTerm", "1.0");
            defaultPackageRepository.AddPackage(packageD);

            return defaultPackageRepository;
        }

        public static IPackageRepositoryFactory CreatePackageRepositoryFactory()
        {
            var defaultPackageRepository = GetPackageRepository();
            //Setup Factory
            var packageRepositoryFactory = new Implementation<IPackageRepositoryFactory>();
            packageRepositoryFactory.Register(inst => inst.CreateRepository(Argument<string>.Any)).Return(defaultPackageRepository);

            //Return the Factory
            return packageRepositoryFactory.Instance;
        }

        public static IPackageSourceProvider GetSourceProvider(params string[] sources)
        {
            var provider = new Mock<IPackageSourceProvider>();
            if (sources == null || !sources.Any())
            {
                sources = new[] { DefaultRepoUrl };
            }
            provider.Register(c => c.LoadPackageSources()).Return(sources.Select(c => new PackageSource(c)));

            return provider.Instance;
        }

        public static IPackageManager GetPackageManager(IPackage package)
        {
            var packageManager = new Mock<IPackageManager>();
            var repository = new MockPackageRepository { package };
            packageManager.Register(inst => inst.SourceRepository).Return(repository);
            packageManager.Register(inst => inst.InstallPackage(package.Id, package.Version, false, true)).Return();
            packageManager.Register(inst => inst.UninstallPackage(package.Id, package.Version, true, false)).Return();
            packageManager.Register(inst => inst.UpdatePackage(package.Id, package.Version, true, true)).Return();
            return packageManager.Instance;
        }
    }
}
