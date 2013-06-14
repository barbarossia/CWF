using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using System.Collections.ObjectModel;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using System.ComponentModel;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Microsoft.Support.Workflow.Authoring.Views
{
    /// <summary>
    /// Interaction logic for ImportAssemblyView.xaml
    /// </summary>
    public partial class ImportAssemblyView : Window,INotifyPropertyChanged
    {
        const string IssuesCountTooltipFormat = "{0} items with issues.";
        private CollectionViewSource ActivityCategories;
        public ImportAssemblyView()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(ImportAssemblyView_DataContextChanged);
            Loaded += (s, e) =>
            {
                this.ActivitiesListBox.SelectedIndex = -1;
                this.listBoxLibrary.SelectedIndex = 0;
                //verify library metadata
                var errorConverter = (ErrorSectionConverter)Resources["ErrorSectionConverter"];
                CanImport = this.IsValid && !errorConverter.HasErrors && HasNoIssues;
            };
            this.ActivityCategories = AssetStore.AssetStoreProxy.ActivityCategories;
            this.ActivityCategories.View.CollectionChanged += (s, e) =>
            {
                this.Source = ActivityCategories.Source as ObservableCollection<string>;
            };
            this.Source = ActivityCategories.Source as ObservableCollection<string>;
        }

        private bool libraryHasError;
        private void ImportAssemblyView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Verify activity metadata
            if (null == DataContext) return;
            var context = (ImportAssemblyViewModel)DataContext;

            //verify library metadata
            var errorConverter = (ErrorSectionConverter)Resources["ErrorSectionConverter"];

            if (null != context)
                errorConverter.ViewModelBaseDerivedType = context.SelectedActivityAssemblyItem;
            errorConverter.PropertyChanged += (s, e1) =>
            {
                if (e1.PropertyName == "HasErrors")
                {
                    libraryHasError = errorConverter.HasErrors;
                    CanImport = this.IsValid && !errorConverter.HasErrors && HasNoIssues;
                }
            };

            //verfiy dependencies
            UpdateBoundProperties();
        }

        /// <summary>
        /// Notifies the front end that items it has bound to here have changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private bool isValid = false;
        private ActivityItem selectedActivityItem;

        /// <summary>
        /// Is the data for this page in a valid state?
        /// </summary>
        public virtual bool IsValid
        {
            get { return isValid; }
            set
            {
                isValid = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsValid"));
                CanImport = this.IsValid && !libraryHasError && HasNoIssues;
            }
        }

        private ObservableCollection<string> source;
        public ObservableCollection<string> Source
        {
            get { return this.source; }
            set
            {
                this.source = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Source"));
            }
        }

        /// <summary>
        /// This is the currently selected item in the listbox.
        /// </summary>
        public ActivityItem SelectedActivityItem
        {
            get { return selectedActivityItem; }
            set
            {
                var errorConverter = (ErrorSectionConverter)Resources["ErrorSectionConverterActivity"];

                selectedActivityItem = value;

                if (null != selectedActivityItem)
                {
                    selectedActivityItem.IsReviewed = true;
                    errorConverter.ViewModelBaseDerivedType = selectedActivityItem;
                    selectedActivityItem.PropertyChanged += (sender, e) => Validate();
                }

                Validate();
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedActivityItem"));
                CanImport = this.IsValid && !libraryHasError && HasNoIssues;
            }
        }

        private bool IsCurrentItemValid()
        {
            bool result = true;

            if (null != selectedActivityItem)
            {
                selectedActivityItem.Validate();
                result = selectedActivityItem.IsValid;
            }

            return result;
        }


        bool isValidating = false;
        private void Validate()
        {
            var isValid = true; // is the entire list of items valid? if not, we can't move next.
            var context = (ImportAssemblyViewModel)DataContext;

            if (null == context) return;
            if (isValidating) return; // don't allow re-entry into this routine if a validation is in progress.

            isValidating = true;

            if (null != SelectedActivityItem)
                SelectedActivityItem.Validate();

            context.SelectedActivityAssemblyItem
                .ActivityItems
                .ToList()
                .ForEach(item =>
                {
                    item.Validate();
                    isValid &= item.IsValid;
                });

            IsValid = isValid;

            isValidating = false;
        }

        private bool canImport;
        public bool CanImport
        {
            get { return this.canImport; }
            set
            {
                this.canImport = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CanImport"));
            }
        }

        #region dependencies
        /// <summary>
        /// Returns a string describing the number of items having issues.
        /// </summary>
        public string ItemsWithIssuesMessage { get { return string.Format(IssuesCountTooltipFormat, ItemsWithIssuesCount); } }

        /// <summary>
        /// Returns the number of items that do not have the location set correctly. Intended to be bound to on the front end. 
        /// </summary>
        public int ItemsWithIssuesCount
        {
            get
            {
                var viewModel = DataContext as ImportAssemblyViewModel;
                var result = 0;

                if ((null != viewModel) && (null != viewModel.AssembliesToImport))
                    result = viewModel.AssembliesToImport
                                .Where(assembly => string.IsNullOrEmpty(assembly.Location))
                                .Count();

                return result;
            }
        }

        /// <summary>
        /// Visible if there are items that do not have locations set properly, Collapsed if all locations are set.
        /// </summary>
        public Visibility ItemsWithIssuesVisibility { get { return HasNoIssues ? Visibility.Collapsed : Visibility.Visible; } }

        #endregion

        private void UpdateBoundProperties()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("ItemsWithIssuesMessage"));
            PropertyChanged(this, new PropertyChangedEventArgs("ItemsWithIssuesCount"));
            PropertyChanged(this, new PropertyChangedEventArgs("ItemsWithIssuesVisibility"));
            PropertyChanged(this, new PropertyChangedEventArgs("HasNoIssues"));
            CanImport = this.IsValid && !libraryHasError && HasNoIssues;
        }

        /// <summary>
        /// True if the data on this page is valid.
        /// </summary>
        public bool HasNoIssues { get { return ItemsWithIssuesCount == 0; } }

        private void PickLocationButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var activityAssemblyItem = button.Tag as ActivityAssemblyItemViewModel;
            var viewModel = DataContext as ImportAssemblyViewModel;
            string assemblyFileName = DialogService.ShowOpenFileDialogAndReturnResult("Assembly files (*.dll)|*.dll", "Open Assembly File");

            if (!string.IsNullOrEmpty(assemblyFileName)) // if user didn't cancel
                viewModel.UpdateReferenceLocation(activityAssemblyItem, assemblyFileName);

            e.Handled = true;

            UpdateBoundProperties();
        }

        bool isInSelectionChanged = false;
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActivityItem previousItem;
            ImportAssemblyViewModel context;

            if (null == DataContext) return;
            if (isInSelectionChanged) return;

            isInSelectionChanged = true;

            context = (ImportAssemblyViewModel)DataContext;

            context.SelectedActivityAssemblyItem.ActivityItems
                .ToList()
                .ForEach(item =>
                {
                    item.IsEditing = false;
                    item.IsEdited = item.IsDirty;
                });

            if (e.RemovedItems.Count != 0)
            {
                previousItem = (ActivityItem)e.RemovedItems[0];
                previousItem.Validate();

                if (!previousItem.IsValid)
                {
                    SelectedActivityItem = previousItem;
                    previousItem.IsEditing = true;
                }
            }
            isInSelectionChanged = false;
        }

        private void listLibrary_Selected(object sender, RoutedEventArgs e)
        {
            this.libraryMetaDataPanel.Visibility = System.Windows.Visibility.Visible;
            this.activitiesMetaDataPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void importBtn_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ImportAssemblyViewModel;
            viewModel.ImportCommand.Execute();
            if (viewModel.ImportResult == true)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Verify activity metadata
            if (null == DataContext) return;

            var context = (ImportAssemblyViewModel)DataContext;
            var activityAssemblyItem = context.SelectedActivityAssemblyItem;

            if ((null != activityAssemblyItem) && activityAssemblyItem.ActivityItems.Count > 0)
                SelectedActivityItem = activityAssemblyItem.ActivityItems[0];

            activityAssemblyItem
                .ActivityItems
                .ToList()
                .ForEach(item =>
                {
                    if (string.IsNullOrEmpty(item.Category))
                        item.Category = context.SelectedCategory;
                    item.IsDirty = false;

                }); //TODO put this in the viewmodel once the refactoring task is complete - v-richt 2/13/2012

            Validate();
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.activitiesMetaDataPanel.Visibility != System.Windows.Visibility.Visible)
            {
                this.libraryMetaDataPanel.Visibility = System.Windows.Visibility.Collapsed;
                this.activitiesMetaDataPanel.Visibility = System.Windows.Visibility.Visible;
                this.listBoxLibrary.SelectedIndex = -1;
            }
        }
    }
}
