using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.Models;
using NuGet;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Support.Workflow.Authoring.CDS
{
    public class CDSOnlineRepository : CDSRepositoryBase
    {
        public bool NoCache { get; set; }

        /// <remarks>
        /// Meant for unit testing.
        /// </remarks>
        protected IPackageRepository CacheRepository { get; private set; }

        public CDSOnlineRepository()
            : this(MachineCache.Default)
        {
        }

        protected internal CDSOnlineRepository(IPackageRepository cacheRepository)
        {
            CacheRepository = cacheRepository;
        }

        protected IPackageRepository CreateRepository()
        {
            IPackageRepository repository = base.GetRepository();

            if (NoCache)
            {
                return repository;
            }
            else
            {
                return new PriorityPackageRepository(CacheRepository, repository);
            }
        }

        public bool InstallPackage(string packageId, SemanticVersion version)
        {
            var packageManager = CreatePackageManager();

            using (packageManager.SourceRepository.StartOperation(RepositoryOperationNames.Install, packageId))
            {
                packageManager.InstallPackage(packageId, version, ignoreDependencies: false, allowPrereleaseVersions: true);
                return true;
            }
        }

        public IPackageManager CreatePackageManager()
        {
            var cwfFilwSystem = CreateCWFFileSystem();
            var repository = CreateRepository();
            LocalPackageRepository localRepository = GetLocalRepository() as LocalPackageRepository;
            return new PackageManager(repository, localRepository.PathResolver, cwfFilwSystem, localRepository);
        }

        protected CWFFileSystem CreateCWFFileSystem()
        {
            var path = Path.GetFullPath(CDSConstants.AssemblyPath);
            return new CWFFileSystem(path);
        }
    }
}
