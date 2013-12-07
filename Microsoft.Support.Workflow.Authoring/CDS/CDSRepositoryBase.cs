using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Support.Workflow.Authoring.Common;
using NuGet;
using System;
using System.ComponentModel;

namespace Microsoft.Support.Workflow.Authoring.CDS
{
    public abstract class CDSRepositoryBase
    {
        protected IEnumerable<IPackage> buffer;
        protected int totalCount;
        protected const int bufferSize = 30;
        private List<string> sources;
        public IFileSystem FileSystem { get; set; }
        public IList<string> Arguments { get; private set; }
        public IMachineWideSettings MachineWideSettings { get; set; }
        private List<string> Sources
        {
            get
            {
                return sources;
            }
            set
            {
                var setting = NugetConfigManager.EnabledRepositories.Select(p => p.Name);
                if (value.Any(v => !string.IsNullOrEmpty(v)))
                {
                    sources = (from all in setting
                               join s in value on all equals s
                               select all).ToList();
                }
                else
                {
                    sources = setting.ToList();
                }
            }
        }

        private string source;
        public string Source
        {
            get
            {
                return source;
            }
            set 
            {
                IsSearchOptionChanged = true;
                source = value;
                Sources = new[] { source }.ToList();
                SourceProvider = null;
                InitNuGetRepository();
            }
        }

        protected internal ISettings Settings { get; set; }
        public IPackageSourceProvider SourceProvider { get; set; }
        public IPackageRepositoryFactory RepositoryFactory { get; set; }
        private string searchTerm;
        private CDSSortByType orderBy;
        private bool isLatestVersion;
        private bool isSearchOptionChanged;

        protected bool IsSearchOptionChanged
        {
            get
            {
                return isSearchOptionChanged;
            }
            set
            {
                isSearchOptionChanged = value;
                if (isSearchOptionChanged)
                    ResetQuery();
            }
        }

        public string SearchTerm
        {
            get
            {
                return searchTerm;
            }
            set
            {
                if (value != searchTerm)
                    IsSearchOptionChanged = true;

                searchTerm = value;
            }
        }

        public CDSSortByType OrderBy
        {
            get
            {
                return orderBy;
            }
            set
            {
                if (value != orderBy)
                    IsSearchOptionChanged = true;

                orderBy = value;
            }
        }

        public bool IsLatestVersion
        {
            get
            {
                return isLatestVersion;
            }
            set
            {
                if (value != isLatestVersion)
                    IsSearchOptionChanged = true;

                isLatestVersion = value;
            }
        }

        protected CDSRepositoryBase()
        {
            InitNuGetRepository();
        }

        public virtual PagedList<IPackage> Search(int startIndex, int pageSize)
        {
            if (buffer == null)
            {
                IPackageRepository packageRepository = GetRepository();
                IQueryable<IPackage> query = packageRepository.Search(SearchTerm, allowPrereleaseVersions: false);

                if (IsLatestVersion)
                {
                    query = query.Where(p => p.IsLatestVersion);
                }

                totalCount = query.Count();
                IQueryable<IPackage> orderedQuery = ApplyOrdering(query);

                buffer = orderedQuery.AsBufferedEnumerable(bufferSize);
            }

            IList<IPackage> packages = buffer.Skip(startIndex).Take(pageSize).ToList();
            return new PagedList<IPackage>(packages, startIndex, pageSize, totalCount);
        }


        protected void ResetQuery()
        {
            this.buffer = null;
        }

        protected virtual IQueryable<IPackage> ApplyOrdering(IQueryable<IPackage> query)
        {
            // If the default sort is null then fall back to download count
            IOrderedQueryable<IPackage> result;
            switch (OrderBy)
            {
                case CDSSortByType.PublishedDate:
                    result = query.OrderByDescending(p => p.Published);
                    break;
                case CDSSortByType.NameAscending:
                case CDSSortByType.NameDescending:
                    result = query.SortBy(new string[] { "Title", "Id" }, orderBy == CDSSortByType.NameAscending ? ListSortDirection.Ascending : ListSortDirection.Descending);
                    break;
                default:
                    result = query.OrderByDescending(p => p.DownloadCount);
                    break;
            }
            return result.ThenBy(p => p.Id);
        }

        protected virtual IPackageRepository GetRepository()
        {
            var repository = AggregateRepositoryHelper.CreateAggregateRepositoryFromSources(RepositoryFactory, SourceProvider, Sources);
            return repository;
        }

        public IPackageRepository GetLocalRepository()
        {
            if (!Directory.Exists(CDSConstants.CDSLocalPath))
            {
                Directory.CreateDirectory(CDSConstants.CDSLocalPath);
            }

            var nugetFileSystem = CreateFileSystem(CDSConstants.CDSLocalPath);
            var pathResolver = new DefaultPackagePathResolver(nugetFileSystem, useSideBySidePaths: true);
            IPackageRepository localRepository = new LocalPackageRepository(pathResolver, nugetFileSystem);
            return localRepository;
        }

        protected IFileSystem CreateFileSystem(string path)
        {
            path = Path.GetFullPath(path);
            return new PhysicalFileSystem(path);
        }

        private void InitNuGetRepository()
        {
            if (RepositoryFactory == null)
                RepositoryFactory = new PackageRepositoryFactory();

            var directory = Path.GetDirectoryName(Path.GetFullPath(CDSConstants.ConfigFile));
            var configFileName = Path.GetFileName(CDSConstants.ConfigFile);
            var configFileSystem = new PhysicalFileSystem(directory);
            Settings = NuGet.Settings.LoadDefaultSettings(
                configFileSystem,
                configFileName,
                MachineWideSettings);

            if (SourceProvider == null)
                SourceProvider = PackageSourceBuilder.CreateSourceProvider(Settings);
        }
    }
}
