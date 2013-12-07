using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.Models;
using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.CDS
{
    public class CDSUpdateRepository : CDSOnlineRepository
    {
        public override PagedList<IPackage> Search(int startIndex, int pageSize)
        {
            IPackageRepository packageRepository = GetRepository();
            var localPackages = GetLocalPackages(SearchTerm);

            var query = packageRepository.GetUpdates(
                localPackages,
                includePrerelease: true,
                includeAllVersions: false).AsQueryable();

            totalCount = query.Count();
            IQueryable<IPackage> orderedQuery = ApplyOrdering(query);

            buffer = orderedQuery.AsBufferedEnumerable(bufferSize);
            IList<IPackage> packages = buffer.Skip(startIndex).Take(pageSize).ToList();

            return new PagedList<IPackage>(packages, startIndex, pageSize, totalCount);
        }

        public bool UpdatePackage(string packageId, SemanticVersion version)
        {
            var packageManager = CreatePackageManager();

            using (packageManager.SourceRepository.StartOperation(RepositoryOperationNames.Update, packageId))
            {
                packageManager.UpdatePackage(packageId, version, updateDependencies: true, allowPrereleaseVersions: true);
                return true;
            }
        }

        private IQueryable<IPackage> GetLocalPackages(string searchTerm)
        {
            IPackageRepository lcoalRepository = GetLocalRepository();

            return lcoalRepository.Search(searchTerm, true);

        }
    }
}
