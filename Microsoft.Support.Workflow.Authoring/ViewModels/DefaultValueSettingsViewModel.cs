using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using Microsoft.Support.Workflow.Authoring.Security;
using Microsoft.Support.Workflow.Authoring.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{

    public class DefaultValueSettingsViewModel : NotificationObject
    {
        private const string SaveFailedMsg = "Save the default value settings failed, please check the settings and try again later.";
        private ObservableCollection<string> categories;

        public bool TaskAssignmentFeatureChanged { get; set; }

        private bool enableDownloadDependencies;
        public bool EnableDownloadDependencies
        {
            get { return this.enableDownloadDependencies; }
            set
            {
                this.enableDownloadDependencies = value;
                RaisePropertyChanged(() => EnableDownloadDependencies);
                RaisePropertyChanged(() => HasChanged);
            }
        }

        private OpenMode openForEditing;
        public OpenMode OpenForEditing
        {
            get { return this.openForEditing; }
            set
            {
                this.openForEditing = value;
                RaisePropertyChanged(() => OpenForEditing);
                RaisePropertyChanged(() => HasChanged);
            }
        }

        private Env defaultEnv;
        public Env DefaultEnv
        {
            get { return this.defaultEnv; }
            set
            {
                this.defaultEnv = value;
                RaisePropertyChanged(() => DefaultEnv);
                RaisePropertyChanged(() => HasChanged);
            }
        }

        private IList<Env> envs;
        public IList<Env> Envs
        {
            get { return this.envs; }
            set
            {
                this.envs = value;
                RaisePropertyChanged(() => Envs);
            }
        }

        private SearchScope enableSearchWholeWorkflow;
        public SearchScope SearchWorkflowScope
        {
            get { return this.enableSearchWholeWorkflow; }
            set
            {
                this.enableSearchWholeWorkflow = value;
                RaisePropertyChanged(() => SearchWorkflowScope);
                RaisePropertyChanged(() => HasChanged);
            }
        }

        private bool enableTaskAssignment;
        public bool EnableTaskAssignment
        {
            get { return this.enableTaskAssignment; }
            set
            {
                this.enableTaskAssignment = value;
                RaisePropertyChanged(() => EnableTaskAssignment);
                RaisePropertyChanged(() => HasChanged);
                if (this.EnableTaskAssignment != DefaultValueSettings.EnableTaskAssignment)
                    TaskAssignmentFeatureChanged = true;
                else
                    TaskAssignmentFeatureChanged = false;
                RaisePropertyChanged(() => TaskAssignmentFeatureChanged);
            }
        }

        private string defaultCategory;
        public string DefaultCategory
        {
            get { return this.defaultCategory; }
            set
            {
                this.defaultCategory = value;
                RaisePropertyChanged(() => this.DefaultCategory);
                RaisePropertyChanged(() => HasChanged);
            }
        }

        public ObservableCollection<string> Categories
        {
            get { return this.categories; }
            set
            {
                this.categories = value;
                RaisePropertyChanged(() => Categories);
            }
        }

        public bool HasSaved
        {
            get;
            private set;
        }

        public bool HasChanged
        {
            get
            {
                return this.EnableDownloadDependencies != DefaultValueSettings.EnableDownloadDependecies
                     || (this.SearchWorkflowScope == SearchScope.SearchWholeWorkflow) != DefaultValueSettings.SearchWholeWorkflow
                     || this.DefaultEnv != DefaultValueSettings.Environment
                     || this.EnableTaskAssignment != DefaultValueSettings.EnableTaskAssignment
                     || (this.OpenForEditing == OpenMode.Editing) != DefaultValueSettings.OpenForEditingMode
                     || this.DefaultCategory != DefaultValueSettings.DefaultCategory;

            }
        }

        public DelegateCommand SaveCommand { get; set; }

        public DefaultValueSettingsViewModel()
        {
            this.enableDownloadDependencies = DefaultValueSettings.EnableDownloadDependecies;
            this.SearchWorkflowScope = DefaultValueSettings.SearchWholeWorkflow ? SearchScope.SearchWholeWorkflow : SearchScope.SearchCurrentWorkflow;
            this.enableTaskAssignment = DefaultValueSettings.EnableTaskAssignment;
            this.defaultEnv = DefaultValueSettings.Environment;
            this.defaultCategory = DefaultValueSettings.DefaultCategory;
            this.Categories = AssetStore.AssetStoreProxy.Categories;
            this.OpenForEditing = DefaultValueSettings.OpenForEditingMode ? OpenMode.Editing : OpenMode.Readonly;
            this.Envs = new List<Env> { Env.Dev, Env.Test, Env.Stage, Env.Prod };
            this.SaveCommand = new DelegateCommand(this.SaveCommandExecute, () => HasChanged);
        }

        private void SaveCommandExecute()
        {
            try
            {
                HasSaved = false;
                Utility.DoTaskWithBusyCaption("Saving", () =>
                {
                    DefaultValueSettings.SetConfigValue(DefaultValueSettings.EnableDownloadDependeciesKey, EnableDownloadDependencies.ToString());
                    DefaultValueSettings.SetConfigValue(DefaultValueSettings.OpenForEditingModeKey, (this.OpenForEditing == OpenMode.Editing).ToString());
                    DefaultValueSettings.SetConfigValue(DefaultValueSettings.SearchWholeWorkflowKey, (this.SearchWorkflowScope == SearchScope.SearchWholeWorkflow).ToString());
                    DefaultValueSettings.SetConfigValue(DefaultValueSettings.EnvKey, this.DefaultEnv.ToString());
                    DefaultValueSettings.SetConfigValue(DefaultValueSettings.EnableTaskAssignmentKey, this.EnableTaskAssignment.ToString());
                    DefaultValueSettings.SetConfigValue(DefaultValueSettings.DefaultCategoryKey, this.DefaultCategory);
                    DefaultValueSettings.RefreshConfigValues();
                   
                    HasSaved = true;
                });
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowException(ex, SaveFailedMsg);
            }
        }


    }
}
