// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectAssemblyAndActivityViewModel.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using Common;
    using CWF.DataContracts;
    using Common.ExceptionHandling;
    using Service.Contracts.FaultContracts;
    using Models;
    using Practices.Prism.Commands;
    using Practices.Prism.ViewModel;
    using Services;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    using Microsoft.Support.Workflow.Authoring.AddIns;
    using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

    /// <summary>
    /// The select assembly and activity view model.
    /// </summary>
    public class SelectAssemblyAndActivityViewModel : NotificationObject
    {
        private ActivityAssemblyItem currentActivityAssemblyItem;  // The current activity assembly item.
        private ActivityItem currentActivityItem;                  // The current activity item.

        /// <summary>
        /// Gets all ActivityAssemblyItems from local caching.
        /// </summary>
        public ObservableCollection<ActivityAssemblyItem> ActivityAssemblyItemCollection { get; set; }

        /// <summary>
        /// Gets or sets the ActivityAssemblyItem user current select.
        /// </summary>
        public ActivityAssemblyItem CurrentActivityAssemblyItem
        {
            get { return currentActivityAssemblyItem; }
            set
            {
                currentActivityAssemblyItem = value;
                RaisePropertyChanged(() => CurrentActivityAssemblyItem);
                currentActivityAssemblyItem.PropertyChanged += CurrentActivityAssemblyItem_PropertyChanged;
            }
        }

        /// <summary>
        /// Gets or sets ActivityItem user current select.
        /// </summary>
        public ActivityItem CurrentActivityItem
        {
            get { return currentActivityItem; }
            set
            {
                currentActivityItem = value;
                RaisePropertyChanged(() => CurrentActivityItem);
            }
        }

        /// <summary>
        /// Gets or sets OkCommand.
        /// </summary>
        public DelegateCommand OkCommand { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="SelectAssemblyAndActivityViewModel"/> class.
        /// </summary>
        public SelectAssemblyAndActivityViewModel()
        {
            OkCommand = new DelegateCommand(() => WorkflowsQueryServiceUtility.UsingClient(OkCommandExecute));
            ActivityAssemblyItemCollection = new ObservableCollection<ActivityAssemblyItem>();
            Utility.WithContactServerUI(new Action(() => { WorkflowsQueryServiceUtility.UsingClient(LoadLiveData); }));
        }


        /// <summary>
        /// The get activity items by activity assembly item.
        /// </summary>
        /// <param name="client"> </param>
        /// <param name="activityAssemblyItem">
        /// The activity assembly item.
        /// </param>
        public void GetActivityItemsByActivityAssemblyItem(IWorkflowsQueryService client, ActivityAssemblyItem activityAssemblyItem)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            if (activityAssemblyItem == null)
            {
                throw new ArgumentNullException("activityAssemblyItem");
            }

            CurrentActivityAssemblyItem = activityAssemblyItem;

            if (activityAssemblyItem.ActivityItems != null && activityAssemblyItem.ActivityItems.Count == 0)
            {
                AssemblyDownloader.GetActivityItemsByActivityAssemblyItem(activityAssemblyItem, client);
                WorkflowsQueryServiceUtility.UsingClient(LoadLiveData);
            }
        }

        /// <summary>
        /// The event handler for CurrentActivityAssemblyItem's PropertyChannged event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void CurrentActivityAssemblyItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UserSelected")
            {
                var activityAssemblyItem = sender as ActivityAssemblyItem;

                if (activityAssemblyItem != null)
                {
                    foreach (ActivityItem item in activityAssemblyItem.ActivityItems)
                    {
                        item.UserSelected = activityAssemblyItem.UserSelected;
                    }
                }
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        public void LoadLiveData(IWorkflowsQueryService client)
        {
            GetAllActivityLibrariesReplyDC reply;

            // gets the version for an ActivityAssemblyItem, or an empty string if it is null
            Func<ActivityAssemblyItem, string> getVersion = item => null == item.Version ? string.Empty : item.Version.ToString();

            // checks to see if an ActivityAssemblyItem is already in the ActivityAssemblyItemCollection 
            Func<ActivityLibraryDC, bool> isAlreadyInCollection =
                activityLibrary => ActivityAssemblyItemCollection
                                       .Any(activityAssemblyItem => activityAssemblyItem.Name == activityLibrary.Name
                                                                    && getVersion(activityAssemblyItem) == activityLibrary.VersionNumber);

            // Load from local caching
            Caching
                .ActivityAssemblyItems
                .ToList()
                .ForEach(activityAssemblyItem => ActivityAssemblyItemCollection.Add(activityAssemblyItem));

            // Get the list of libraries that are on the server   
            try
            {
                var request = new GetAllActivityLibrariesRequestDC
                             {
                                 Incaller = Environment.UserName,
                                 IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString()
                             };

                reply = client.GetAllActivityLibraries(request);
            }
            catch (FaultException<ServiceFault> ex)
            {
                throw new CommunicationException(ex.Detail.ErrorMessage);
            }
            catch (FaultException<ValidationFault> ex)
            {
                throw new BusinessValidationException(ex.Detail.ErrorMessage);
            }
            catch (Exception ex)
            {
                throw new CommunicationException(ex.Message);
            }

            // Require a pre-check since the list is not reliably instantiated 
            // by the WCF service
            if ((null != reply) && (null != reply.List))
            {
                // Each library which isn't already locally cached
                // should show up on the Select screen as a library
                // available on the server
                reply
                    .List
                    .Where(activityLibrary => !string.IsNullOrEmpty(activityLibrary.Name)
                                               && activityLibrary.HasExecutable
                                               && !isAlreadyInCollection(activityLibrary))
                    .ToList()
                    .ForEach(activityLibrary =>
                            {
                                var item = DataContractTranslator.ActivityLibraryDCToActivityAssemblyItem(activityLibrary);

                                item.CachingStatus = CachingStatus.Server;
                                ActivityAssemblyItemCollection.Add(item);
                            });
            }
        }




        /// <summary>
        /// Loads assemblies from the server, sets display names, categories, determines dependencies, and so forth.
        /// </summary>
        public void OkCommandExecute(IWorkflowsQueryService client)
        {
            string[] sections;  // When we get a human readable name for an activity, we use this to find the name of the activity without namespaces.

            var selected = ActivityAssemblyItemCollection
                              .Where(activityAssemblyItem => activityAssemblyItem.UserSelected
                                                             && activityAssemblyItem.CachingStatus != CachingStatus.Latest).ToList();


            // Make sure activities (etc.) have the category of their parent.
            // Since we don't have a display name available at this point and need to make one, determine
            // what might be an appropriate display name and set it.
            selected
                .ForEach(assembly => assembly
                                         .ActivityItems
                                         .ToList()
                                         .ForEach(item =>
                                                      {
                                                          item.Category = assembly.Category;
                                                          sections = item.Name.Split(".".ToCharArray());
                                                          item.DisplayName = ToolboxControlService.GetHumanReadableName(sections[sections.Length - 1]);
                                                      }));


            if (selected.Any())
            {
                var assemblies = Utility.WithContactServerUI(() =>
                {
                    var dependencies = Caching.ComputeDependencies(client, selected);
                    // When a selected assembly is also a dependency of another selected assembly,
                    // be careful to always prefer the ActivityAssemblyItem from the ViewModel (selected) 
                    // over the one newly downloaded from the database (dependencies) so client-only properties 
                    // like UserSelected and CacheState are maintained.
                    var assembliesAndDependencies = selected
                                                        .Union(dependencies
                                                                  .Where(dependency => !selected.Any(selectedItem => selectedItem.Matches(dependency.AssemblyName)))).ToList();

                    return Caching.DownloadAssemblies(client, assembliesAndDependencies);
                });

                Caching.CacheAssembly(assemblies);
                Caching.Refresh();
            }
            else
            {
                Caching.Refresh(); // force toolbox re-load
            }
        }

    }
}
