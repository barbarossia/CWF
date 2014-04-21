// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReviewActivityViewModel.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    using Practices.Prism.ViewModel;
    using Models;
    using Practices.Prism.Commands;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

    /// <summary>
    /// The review activity view model.
    /// </summary>
    public sealed class ReviewActivityViewModel : NotificationObject
    {
       
        /// <summary>
        /// The activity assembly item.
        /// </summary>
        private ActivityAssemblyItemViewModel itemToReview;

        /// <summary>
        /// The selected activity item.
        /// </summary>
        private ActivityItem selectedActivityItem;

        /// <summary>
        /// The title.
        /// </summary>
        private string title;

         /// <summary>
        /// Gets or sets the command to mark an activity assembly as reviewed.
        /// </summary>
        public DelegateCommand ReviewAssemblyCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewActivityViewModel"/> class.
        /// </summary>
        /// <param name="activityAssemblyItem">
        /// The activity assembly item.
        /// </param>
        public ReviewActivityViewModel(ActivityAssemblyItemViewModel activityAssemblyItem)
        {
            ReviewAssemblyCommand = new DelegateCommand(ReviewAssemblyCommandExecute);
            ActivityAssemblyItem = activityAssemblyItem;
            Title = string.Format(TextResources.ReviewActivitiesFormat, activityAssemblyItem.Name);

            foreach (ActivityItem activityItem in activityAssemblyItem.ActivityItems)
            {
                activityItem.Category = activityAssemblyItem.Category;
                activityItem.Version = activityAssemblyItem.Version;
            }
        }

        /// <summary>
        /// Gets or sets ActivityAssemblyItem.
        /// </summary>
        public ActivityAssemblyItemViewModel ActivityAssemblyItem
        {
            get
            {               
                return itemToReview;
            }

            private set
            {
                itemToReview = value;
                RaisePropertyChanged(() => ActivityAssemblyItem);
            }
        }

        /// <summary>
        /// Gets or sets SelectedActivityItem.
        /// </summary>
        public ActivityItem SelectedActivityItem
        {
            get
            {
                if (selectedActivityItem == null && itemToReview.ActivityItems.Count > 0)
                    selectedActivityItem = itemToReview.ActivityItems[0];
                
                return selectedActivityItem;
            }
            set
            {
                selectedActivityItem = value;
                RaisePropertyChanged(() => SelectedActivityItem);
            }
        }

        /// <summary>
        /// Gets or sets the title of the model
        /// </summary>
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        /// <summary>
        /// Handles the logic for the execution of the ReviewAssemblyCommand. 
        /// Marks the associated assembly item as reviewed.
        /// </summary>
        private void ReviewAssemblyCommandExecute()
        {
            ActivityAssemblyItem.IsReviewed = true;
        }
    }
}
