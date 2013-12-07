using Microsoft.Support.Workflow.Authoring.Common;
using NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.CDS
{
    public class CDSInstalledRepository : CDSOnlineRepository
    {
        private IPackageRepository localPakcageRepository;
        public IPackageRepository LocalPakcageRepository
        {
            get
            {
                if (localPakcageRepository == null)
                {
                    localPakcageRepository = GetRepository();
                }
                return localPakcageRepository;
            }
        }

        public override PagedList<IPackage> Search(int startIndex, int pageSize)
        {
            IQueryable<IPackage> query = LocalPakcageRepository.Search(SearchTerm, false);

            totalCount = query.Count();
            IQueryable<IPackage> orderedQuery = ApplyOrdering(query);

            buffer = orderedQuery.AsBufferedEnumerable(bufferSize);

            IList<IPackage> packages = buffer.Skip(startIndex).Take(pageSize).ToList();
            return new PagedList<IPackage>(packages, startIndex, pageSize, totalCount);
        }

        protected override IPackageRepository GetRepository()
        {
            return GetLocalRepository();
        }

        public bool UninstallPackage(string packageId, SemanticVersion version, bool removeDependencies)
        {
            if (IsPackageDependentOn(packageId, version))
                return false;

            var packageManager = CreatePackageManager();
            packageManager.UninstallPackage(packageId, version, forceRemove: true, removeDependencies: removeDependencies);
            return true;
        }

        public bool IsInstalled(string packageId, SemanticVersion version)
        {
            return null != FindLocalPackage(packageId, version);
        }

        public bool IsPackageDependentOn(string packageId, SemanticVersion version)
        {
            IPackage uninstallPackage = FindLocalPackage(packageId, version);
            PagedList<IPackage> installedPackages = Search(0, int.MaxValue);
            foreach (var package in installedPackages)
            {
                foreach (var dependent in GetDependents(package))
                {
                    if (PackageEqualityComparer.IdAndVersion.Equals(uninstallPackage, LocalPakcageRepository.ResolveDependency(dependent, allowPrereleaseVersions: true, preferListedPackages: false)))
                        return true;
                }
            }

            return false;
        }

        private IPackage FindLocalPackage(string packageId, SemanticVersion version)
        {
            return LocalPakcageRepository.FindPackage(packageId, version);
        }

        private IEnumerable<PackageDependency> GetDependents(IPackage package)
        {
            foreach (var d in package.DependencySets)
            {
                foreach (var ds in d.Dependencies)
                {
                    yield return ds;
                }
            }
        }
    }
}
