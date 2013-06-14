// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentSelectorViewModel.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Data;
    using Practices.Prism.ViewModel;
    using Models;
    using Services;

    /// <summary>
    /// The content selector view model.
    /// </summary>
    public sealed class ContentSelectorViewModel : NotificationObject
    {
        #region Constants and Fields

        private const string defaultGroupName = "OASPLocalization.xml";

        /// <summary>
        /// The content file items.
        /// </summary>
        private ObservableCollection<ContentFileItem> contentFileItems;

        /// <summary>
        /// The content items.
        /// </summary>
        private ObservableCollection<ContentItem> contentItems;

        private CollectionViewSource itemsView;

        /// <summary>
        /// The search key.
        /// </summary>
        private string searchFilter;

        private ContentFileItem selectedContentFileItem;

        private CollectionViewGroup selectedGroup;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSelectorViewModel"/> class.
        /// </summary>
        public ContentSelectorViewModel()
        {
            //Load existing content files in collection
            LoadContentItemFiles();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets ContentFileItems.
        /// </summary>
        public ObservableCollection<ContentFileItem> ContentFileItems
        {
            get
            {
                return contentFileItems;
            }

            set
            {
                contentFileItems = value;
                RaisePropertyChanged(() => ContentFileItems);
            }
        }

        /// <summary>
        /// Gets or sets ContentItems.
        /// </summary>
        public ObservableCollection<ContentItem> ContentItems
        {
            get
            {
                return contentItems;
            }

            set
            {
                contentItems = value;
                RaisePropertyChanged(() => ContentItems);
            }
        }

        /// <summary>
        /// Gets or sets SearchFilter.
        /// </summary>
        public string SearchFilter
        {
            get
            {
                return searchFilter;
            }

            set
            {
                searchFilter = value;
                RaisePropertyChanged("SearchFilter");
                RefreshContentItems();
            }
        }

        /// <summary>
        /// Gets or sets the selected content file.
        /// </summary>
        public ContentFileItem SelectedContentFileItem
        {
            get
            {
                return selectedContentFileItem;
            }
            set
            {
                selectedContentFileItem = value;
                RaisePropertyChanged(() => SelectedContentFileItem);
                RefreshContentItems();
            }
        }

        public CollectionViewSource ItemsView
        {
            get
            {
                return itemsView;
            }
            set
            {
                itemsView = value;
                RaisePropertyChanged(() => ItemsView);
            }
        }

        public CollectionViewGroup SelectedGroup
        {
            get
            {
                return selectedGroup;
            }
            set
            {
                selectedGroup = value;
                RaisePropertyChanged(() => SelectedGroup);
               
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        public void LoadContentItemFiles()
        {
            if (ContentFileItems == null)
            {
                ContentFileItems = new ObservableCollection<ContentFileItem>();
            }

            ContentFileItems = ContentManager.GetContentFileItems();
            ContentItems = ContentManager.GetContentItems(ContentFileItems);
            itemsView = new CollectionViewSource();
            itemsView.Source = ContentItems;


            if (ItemsView.View.CanGroup)
            {
                var groupDescription = new PropertyGroupDescription("Section");
                ItemsView.GroupDescriptions.Add(groupDescription);

                if (SelectedGroup == null)
                {
                    var group = from item in itemsView.View.Groups
                                where (item as CollectionViewGroup).Name != null && (item as CollectionViewGroup).Name.ToString() == defaultGroupName
                                select item;

                    if (group.Any())
                    {
                        SelectedGroup = (CollectionViewGroup)group.First();
                    }
                }
            }

            RefreshContentItems();

        }

        /// <summary>
        /// Refreshes the ContentItems based on the selected criteria
        /// </summary>
        public void RefreshContentItems()
        {

            //User typed a search filter, so we filter the results that show in the list
            ItemsView.View.Filter = item =>
            {
                var content = item as ContentItem;

                if (SearchFilter != null && content != null)
                {
                    if (content.Key.ToLower().Contains(SearchFilter.ToLower()) || 
                        (content.Value != null && content.Value.ToLower().Contains(SearchFilter.ToLower())))
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            };
        }
        
        #endregion
    }
}