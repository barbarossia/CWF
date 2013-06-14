// -----------------------------------------------------------------------
// <copyright file="ActivityItemViewModel.cs" company="Microsoft">
//  Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    using System.ComponentModel;
    using AssetStore;
    using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;

    /// <summary>
    /// View Model for the Activity item view. Provides values for combo boxes (etc), and monitors selected values for the same
    /// </summary>
    public class ActivityItemViewModel : ViewModelBase
    {
        private string selectedStatus;
        private string selectedCategory;

        public string SelectedStatus
        {
            get { return selectedStatus; }
            set
            {
                selectedStatus = value;
                RaisePropertyChanged(() => SelectedStatus);
            }
        }

        public ICollectionView Categories { get { return AssetStoreProxy.ActivityCategories.View; } }

        public string SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                RaisePropertyChanged(() => SelectedCategory);
            }
        }

    }
}
