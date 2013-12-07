using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.CDS
{
    public static class CDSService
    {
        private static CDSOnlineRepository onlineRepository;
        private static CDSInstalledRepository installedRepository;
        private static CDSUpdateRepository updateRepository;

        private static CDSOnlineRepository OnlineRepository
        {
            get
            {
                if (onlineRepository == null)
                    onlineRepository = new CDSOnlineRepository();
                return onlineRepository;
            }
        }

        private static CDSInstalledRepository InstalledRepository
        {
            get
            {
                if (installedRepository == null)
                    installedRepository = new CDSInstalledRepository();
                return installedRepository;
            }
        }

        private static CDSUpdateRepository UpdateRepository
        {
            get
            {
                if (updateRepository == null)
                    updateRepository = new CDSUpdateRepository();
                return updateRepository;
            }
        }

        public static PagedList<IPackage> SearchOnline(int startIndex,
            int pageSize,
            string source,
            CDSSortByType orderBy,
            string searchTerm,
            bool isLatestVersion = true)
        {
            return ExceptionHandler(() =>
            {
                OnlineRepository.Source = source;
                OnlineRepository.OrderBy = orderBy;
                OnlineRepository.SearchTerm = searchTerm;
                OnlineRepository.IsLatestVersion = isLatestVersion;

                return OnlineRepository.Search(startIndex, pageSize);
            });
        }

        public static PagedList<IPackage> SearchLocal(int startIndex,
            int pageSize,
            CDSSortByType orderBy,
            string searchTerm)
        {
            return ExceptionHandler(() =>
            {
                InstalledRepository.OrderBy = orderBy;
                InstalledRepository.SearchTerm = searchTerm;
                return InstalledRepository.Search(startIndex, pageSize);
            });
        }

        public static PagedList<IPackage> SearchUpdate(int startIndex,
            int pageSize,
            string source,
            CDSSortByType orderBy,
            string searchTerm,
            bool isLatestVersion = true)
        {
            return ExceptionHandler(() =>
            {
                UpdateRepository.Source = source;
                UpdateRepository.OrderBy = orderBy;
                UpdateRepository.SearchTerm = searchTerm;
                UpdateRepository.IsLatestVersion = isLatestVersion;
                return UpdateRepository.Search(startIndex, pageSize);
            });
        }

        public static bool Install(string source, string packageId, SemanticVersion version)
        {
            return ExceptionHandler(() =>
            {
                return InstalledRepository.InstallPackage(packageId, version);
            });
        }

        public static bool Update(string source, string packageId, SemanticVersion version)
        {
            return ExceptionHandler(() =>
            {
                UpdateRepository.Source = source;
                return UpdateRepository.UpdatePackage(packageId, version);
            });
        }

        public static bool Uninstall(string packageId, SemanticVersion version)
        {
            return ExceptionHandler(() =>
            {
                return InstalledRepository.UninstallPackage(packageId, version, removeDependencies: false);
            });
        }

        /// <summary>
        /// The package is installed
        /// </summary>
        /// <param name="package"></param>
        /// <returns>True: installed</returns>
        /// <returns>False: Not installed</returns>
        public static bool IsInstalled(IPackage package)
        {
            return ExceptionHandler(() =>
            {
                return InstalledRepository.IsInstalled(package.Id, package.Version);
            });
        }

        public static bool IsPackageDependentOn(IPackage package)
        {
            return ExceptionHandler(() =>
            {
                return InstalledRepository.IsPackageDependentOn(package.Id, package.Version);
            });
        }

        private static T ExceptionHandler<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                throw new CDSPackageException(ex.Message);
            }
        }
    }
}
