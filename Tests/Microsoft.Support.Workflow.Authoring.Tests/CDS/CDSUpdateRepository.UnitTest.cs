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
    public class CDSUpdateRepositoryUnitTest
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

            var localRepository = CDSRepositoryHelper.GetPackageRepository();
            using (var manager = new ImplementationOfType(typeof(NugetConfigManager)))
            {
                manager.Register(() => NugetConfigManager.Repositories).Return(Settings);

                var repository = new Implementation<CDSUpdateRepository>();

                repository.Register(inst => inst.GetLocalRepository()).Return(localRepository);
                repository.Instance.OrderBy = orderBy;
                repository.Instance.SearchTerm = searchTerm;
                repository.Instance.Search(startIndex, pageSize);
            }

        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-Dif")]
        public void Update()
        {
            var packageA = PackageUtility.CreatePackage("DefaultUrlUsed", "1.0");

            using (var manager = new ImplementationOfType(typeof(NugetConfigManager)))
            {
                manager.Register(() => NugetConfigManager.Repositories).Return(Settings);

                using (var repository = new Implementation<CDSUpdateRepository>())
                {
                    repository.Register(inst => inst.CreatePackageManager()).Return(CDSRepositoryHelper.GetPackageManager(packageA));
                    repository.Instance.UpdatePackage(packageA.Id, packageA.Version);
                }
            }
        }
    }
}
