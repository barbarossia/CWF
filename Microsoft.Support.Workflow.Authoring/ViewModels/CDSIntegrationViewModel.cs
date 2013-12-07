using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Support.Workflow.Authoring.CDS;
using Microsoft.Support.Workflow.Authoring.Services;

namespace Microsoft.Support.Workflow.Authoring.ViewModels {
    public class CDSIntegrationViewModel : NotificationObject {
        private bool hasSaved = true;
        private ObservableCollection<CDSRepository> repositories;
        private CDSRepository selectedRepository;
        private CDSRepository displayingRepository;

        public ObservableCollection<CDSRepository> Repositories {
            get { return repositories; }
            set {
                repositories = value;
                RaisePropertyChanged(() => Repositories);
            }
        }
        public CDSRepository SelectedRepository {
            get { return selectedRepository; }
            set {
                selectedRepository = value;
                RaisePropertyChanged(() => SelectedRepository);

                if (SelectedRepository != null)
                    DisplayingRepository = SelectedRepository.Clone();
                RemoveCommand.RaiseCanExecuteChanged();
                UpCommand.RaiseCanExecuteChanged();
                DownCommand.RaiseCanExecuteChanged();
                UpdateCommand.RaiseCanExecuteChanged();
            }
        }
        public CDSRepository DisplayingRepository {
            get { return displayingRepository; }
            set {
                displayingRepository = value;
                RaisePropertyChanged(() => DisplayingRepository);
            }
        }

        public bool HasSaved {
            get { return hasSaved; }
            private set {
                hasSaved = value;
                RaisePropertyChanged(() => HasSaved);
            }
        }

        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand RemoveCommand { get; private set; }
        public DelegateCommand UpCommand { get; private set; }
        public DelegateCommand DownCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }

        public CDSIntegrationViewModel() {
            Repositories = new ObservableCollection<CDSRepository>(NugetConfigManager.Repositories.Select(r => r.Clone()));
            foreach (CDSRepository repo in Repositories) {
                repo.PropertyChanged += RepositoryPropertyChanged;
            }
            Repositories.CollectionChanged += RepositoriesCollectionChanged;

            AddCommand = new DelegateCommand(() => {
                CDSRepository newRepo = new CDSRepository() {
                    IsEnabled = true,
                    Name = "Package Source",
                    Source = "http://packagesource",
                };
                Repositories.Add(newRepo);
                SelectedRepository = newRepo;
            });
            RemoveCommand = new DelegateCommand(() => {
                Repositories.Remove(SelectedRepository);
                SelectedRepository = null;
            }, () => { return SelectedRepository != null; });
            UpCommand = new DelegateCommand(() => {
                int index = Repositories.IndexOf(SelectedRepository);
                if (index > 0)
                    Repositories.Move(index, index - 1);
            }, () => { return SelectedRepository != null; });
            DownCommand = new DelegateCommand(() => {
                int index = Repositories.IndexOf(SelectedRepository);
                if (index < (Repositories.Count - 1))
                    Repositories.Move(index, index + 1);
            }, () => { return SelectedRepository != null; });
            UpdateCommand = new DelegateCommand(() => {
                Uri uri;
                if (!Uri.TryCreate(DisplayingRepository.Source, UriKind.Absolute, out uri) || !uri.IsWellFormedOriginalString())
                    MessageBoxService.ShowError("The source specified is invalid. Please provide a valid source.");
                else {
                    SelectedRepository.Name = DisplayingRepository.Name;
                    SelectedRepository.Source = DisplayingRepository.Source;
                }
            }, () => { return SelectedRepository != null; });
        }

        public void Save() {
            if (Repositories.GroupBy(r => r.Name, StringComparer.OrdinalIgnoreCase).Any(g => g.Count() > 1))
                MessageBoxService.ShowError("The name specified has already been added to the list of available package sources. Please provide a unique name.");
            else {
                NugetConfigManager.Save(Repositories.ToList());
                HasSaved = true;
            }
        }

        private void RepositoryPropertyChanged(object sender, PropertyChangedEventArgs e) {
            HasSaved = false;
        }

        private void RepositoriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            HasSaved = false;
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems) {
                        ((CDSRepository)item).PropertyChanged += RepositoryPropertyChanged;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems) {
                        ((CDSRepository)item).PropertyChanged -= RepositoryPropertyChanged;
                    }
                    break;
            }
        }
    }

    public class CDSRepository : NotificationObject {
        private bool isEnabled;
        private string name;
        private string source;

        public bool IsEnabled {
            get { return isEnabled; }
            set {
                isEnabled = value;
                RaisePropertyChanged(() => IsEnabled);
            }
        }
        public string Name {
            get { return name; }
            set {
                name = value;
                RaisePropertyChanged(() => Name);
            }
        }
        public string Source {
            get { return source; }
            set {
                source = value;
                RaisePropertyChanged(() => Source);
            }
        }

        public CDSRepository Clone() {
            return (CDSRepository)MemberwiseClone();
        }

        public override string ToString() {
            return Name;
        }
    }
}
