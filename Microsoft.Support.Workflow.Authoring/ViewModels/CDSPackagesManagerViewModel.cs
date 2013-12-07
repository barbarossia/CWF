using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.CDS;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using NuGet;

namespace Microsoft.Support.Workflow.Authoring.ViewModels {
    public class CDSPackagesManagerViewModel : ViewModelBase {
        private const int pageSize = 7;

        private DataPagingViewModel dpvm;
        private IPackage selectedPackage;
        private PagedList<IPackage> packages;
        private IEnumerable<CDSRepository> repos;
        private CDSSortByType selectedSortByOption = CDSSortByType.PublishedDate;
        private string searchText;
        private bool latestOnly = true;
        private PackageSearchType searchType;
        private string source;

        public DelegateCommand SettingsCommand { get; private set; }
        public DelegateCommand PackageCommand { get; private set; }

        public string DateHeader {
            get { return SearchType == PackageSearchType.Local ? "Installed:" : "Published:"; }
        }
        public bool IsLatestOnlyVisible {
            get { return SearchType == PackageSearchType.Online; }
        }
        public string SelectedPackageDependencies {
            get {
                var dependencies = SelectedPackage.DependencySets.SelectMany(d => d.Dependencies).Select(d => d.ToString());
                return dependencies.Any() ? string.Join(Environment.NewLine, dependencies) : "No dependencies";
            }
        }
        public string SelectedPackageAuthors {
            get {
                return string.Join(", ", SelectedPackage.Authors);
            }
        }
        public EnumOnUI<CDSSortByType>[] SortByOptions {
            get { return EnumOnUI<CDSSortByType>.GetUIModels(); }
        }
        public CDSSortByType SelectedSortByOption {
            get { return selectedSortByOption; }
            set {
                if (selectedSortByOption != value) {
                    selectedSortByOption = value;
                    RaisePropertyChanged(() => SelectedSortByOption);

                    SearchWithReset();
                }
            }
        }
        public DataPagingViewModel DataPagingVM {
            get { return dpvm; }
            set {
                dpvm = value;
                RaisePropertyChanged(() => DataPagingVM);
            }
        }
        public IPackage SelectedPackage {
            get { return selectedPackage; }
            set {
                selectedPackage = value;
                RaisePropertyChanged(() => SelectedPackage);

                RaisePropertyChanged(() => SelectedPackageDependencies);
                RaisePropertyChanged(() => SelectedPackageAuthors);
                PackageCommand.RaiseCanExecuteChanged();
            }
        }
        public PagedList<IPackage> Packages {
            get { return packages; }
            set {
                packages = value;
                RaisePropertyChanged(() => Packages);
            }
        }
        public IEnumerable<CDSRepository> Repositories {
            get { return repos; }
            set {
                repos = value;
                RaisePropertyChanged(() => Repositories);
            }
        }
        public string SearchText {
            get { return searchText; }
            set {
                searchText = value;
                RaisePropertyChanged(() => SearchText);
            }
        }
        public bool LatestOnly {
            get { return latestOnly; }
            set {
                latestOnly = value;
                RaisePropertyChanged(() => LatestOnly);

                SearchWithReset();
            }
        }
        public PackageSearchType SearchType {
            get { return searchType; }
            set {
                searchType = value;
                RaisePropertyChanged(() => SearchType);

                RaisePropertyChanged(() => IsLatestOnlyVisible);
                RaisePropertyChanged(() => DateHeader);
            }
        }
        public string Source {
            get { return source; }
            set {
                source = value;
                RaisePropertyChanged(() => Source);
            }
        }

        public CDSPackagesManagerViewModel() {
            DataPagingVM = new DataPagingViewModel();
            DataPagingVM.PageSize = pageSize;
            DataPagingVM.SearchExecute = Search;
            Repositories = NugetConfigManager.EnabledRepositories;

            SettingsCommand = new DelegateCommand(() => {
                OptionsViewModel vm = new OptionsViewModel("CDS Integration");
                DialogService.ShowDialog(vm);
                Repositories = NugetConfigManager.EnabledRepositories;

                if (!Repositories.Any(r => r.Name == Source))
                    Packages = null;
            });
            PackageCommand = new DelegateCommand(() => {
                string id = SelectedPackage.Id;
                var version = SelectedPackage.Version;
                try
                {
                    if (CDSService.IsInstalled(SelectedPackage))
                        Utility.DoTaskWithBusyCaption("Uninstalling...", () =>
                        {
                            CDSService.Uninstall(id, version);
                        });
                    else
                        Utility.DoTaskWithBusyCaption("Installing...", () =>
                        {
                            CDSService.Install(Source, id, version);
                        });
                    Search();
                    if (Packages != null)
                        SelectedPackage = Packages.SingleOrDefault(p => p.Id == id && p.Version == version);
                }
                catch (CDSPackageException ex)
                {
                    MessageBoxService.ShowError(ex.Message);
                }
            }, () => {
                if (SelectedPackage == null)
                    return false;

                if (CDSService.IsInstalled(SelectedPackage))
                    return !CDSService.IsPackageDependentOn(SelectedPackage);
                else
                    return true;
            });
        }

        public void Search() {
            int startIndex = DataPagingVM.ResetPageIndex ? 0 : (DataPagingVM.PageIndex - 1) * pageSize;

            Utility.DoTaskWithBusyCaption("Searching...", () => {
                try {
                    switch (SearchType) {
                        case PackageSearchType.Local:
                            Packages = CDSService.SearchLocal(startIndex, pageSize, SelectedSortByOption, SearchText);
                            break;
                        case PackageSearchType.Online:
                            Packages = CDSService.SearchOnline(startIndex, pageSize, Source, SelectedSortByOption, SearchText, LatestOnly);
                            break;
                        case PackageSearchType.Update:
                            Packages = CDSService.SearchUpdate(startIndex, pageSize, Source, SelectedSortByOption, SearchText);
                            break;
                    }
                    DataPagingVM.ResultsLength = Packages.TotalCount;
                }
                catch (CDSPackageException ex) {
                    Packages = null;
                    DataPagingVM.ResultsLength = 0;
                    MessageBoxService.ShowError(ex.Message);
                }
                catch {
                    Packages = null;
                    DataPagingVM.ResultsLength = 0;
                    MessageBoxService.ShowError(string.Format("Failed to visit \"{0}\" due to network issue.", Source));
                }
            });
        }

        public void SearchWithReset() {
            DataPagingVM.ResetPageIndex = true;
            Search();
        }
    }

    public enum PackageSearchType {
        Local,
        Online,
        Update
    }
}
