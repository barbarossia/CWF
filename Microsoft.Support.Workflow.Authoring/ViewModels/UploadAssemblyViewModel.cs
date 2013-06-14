// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadAssemblyViewModel.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using Models;
    using Practices.Prism.Commands;
    using Practices.Prism.ViewModel;
    using Services;
    using System;
    using Microsoft.Support.Workflow.Authoring.Common;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;

    /// <summary>
    /// The upload assembly view model.
    /// </summary>
    public sealed class UploadAssemblyViewModel : NotificationObject
    {
        private ObservableCollection<ActivityAssemblyItem> displayActivityAssemblyItems;
        private bool selectAllAssemblies;
        private Dictionary<ActivityAssemblyItem, List<ActivityAssemblyItem>> referencingAssemblies;    //Temp storage for all selected assembly and their references

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadAssemblyViewModel"/> class.
        /// </summary>
        public UploadAssemblyViewModel()
        {
            UploadCommand = new DelegateCommand(UploadCommandExecute, UploadCommandCanExecute);
            referencingAssemblies = new Dictionary<ActivityAssemblyItem, List<ActivityAssemblyItem>>();
        }

        /// <summary>
        /// Initialize the viewmodel with the list of assemblies to upload
        /// </summary>
        /// <param name="activityAssemblyItems">List of assemblies to upload</param>
        public void Initialize(IEnumerable<ActivityAssemblyItem> activityAssemblyItems)
        {
            if (activityAssemblyItems == null)
            {
                throw new ArgumentNullException("activityAssemblyItems");
            }

            DisplayActivityAssemblyItems = new ObservableCollection<ActivityAssemblyItem>(activityAssemblyItems);

            foreach (var item in activityAssemblyItems)
                item.UserWantsToUpload = false; // view state is embedded in object, needs to be reset when we have a new UploadAssemblyViewModel
        }

        public bool SelectAllAssemblies
        {
            get { return selectAllAssemblies; }
            set
            {
                selectAllAssemblies = value;
                UpdateMultipleSelection();
                UploadCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => SelectAllAssemblies);
            }
        }

        /// <summary>
        /// Gets or sets DisplayAssemblyLocationItems. User can switch display locations between intial and all cached.
        /// </summary>
        public ObservableCollection<ActivityAssemblyItem> DisplayActivityAssemblyItems
        {
            get { return displayActivityAssemblyItems; }
            set
            {
                displayActivityAssemblyItems = value;
                RaisePropertyChanged(() => DisplayActivityAssemblyItems);
            }
        }

        /// <summary>
        /// Gets or sets UploadCommand.
        /// </summary>
        public DelegateCommand UploadCommand { get; set; }

        private void UpdateMultipleSelection()
        {
            foreach (var item in DisplayActivityAssemblyItems)
            {
                item.UserWantsToUpload = selectAllAssemblies;
            }
        }


        /// <summary>
        /// To upload selected assemblies to database.
        /// </summary>
        /// <param name="activityAssemblyItems">
        /// The locations.
        /// </param>
        public void UploadAssemblies(IEnumerable<ActivityAssemblyItem> activityAssemblyItems)
        {
            Utility.WithContactServerUI(() => WorkflowsQueryServiceUtility.UsingClient(client => WorkflowUploader.Upload(client, activityAssemblyItems)));
            MessageBoxService.NotifyUploadResult("Upload successful", isSucceed: true);
        }

        /// <summary>
        /// Called when UploadCommand execute.
        /// </summary>
        private void UploadCommandExecute()
        {
            var assemblyLocationItemsShouldBeUploaded = DisplayActivityAssemblyItems.Where(item => item.UserWantsToUpload);

            assemblyLocationItemsShouldBeUploaded
                .ToList()
                .ForEach(item =>
                {
                    item.Status = MarketplaceStatus.Public.ToString();
                });


            UploadAssemblies(assemblyLocationItemsShouldBeUploaded);
        }

        private bool UploadCommandCanExecute()
        {
            var selected = from assemblies in DisplayActivityAssemblyItems
                           where assemblies.UserWantsToUpload
                           select assemblies;

            return selected.Any();
        }

        /// <summary>
        /// Get notified when one activity assembly item checked or unchecked for uploading, return the number of assembly items affected.
        /// </summary>
        /// <param name="activityAssemblyItem"></param>
        /// <param name="isChecked"></param>
        /// <returns></returns>
        public void NotifyUploadAssemblyItemChange(ActivityAssemblyItem activityAssemblyItem, bool isChecked)
        {
            if (activityAssemblyItem.ReferencedAssemblies.Any())
            {
                var allReferencedAssemblies = FindAllReferencedAssemblies(activityAssemblyItem.ReferencedAssemblies, activityAssemblyItem);
                allReferencedAssemblies.ForEach(referencedItems => referencedItems.UserWantsToUpload = isChecked);
            }

            foreach (ActivityAssemblyItem referencer in FindAllReferencers(activityAssemblyItem))
            {
                if (referencer.UserWantsToUpload && !isChecked)
                {
                    referencer.UserWantsToUpload = false;
                }

                if (referencingAssemblies.ContainsKey(referencer))
                {
                    referencingAssemblies.Remove(referencer);
                }
            }

            UploadCommand.RaiseCanExecuteChanged();
        }


        private List<ActivityAssemblyItem> FindAllReferencers(ActivityAssemblyItem activityItem)
        {
            var result = new List<ActivityAssemblyItem>();

            var dependentActivities = (from item in DisplayActivityAssemblyItems
                                       where item.ReferencedAssemblies.Any(item2 => item2.Name == activityItem.Name)
                                       select item)
                                      .ToList();

            result.AddRange(dependentActivities);

            dependentActivities.ForEach(item => result.AddRange(FindAllReferencers(item)));

            return result
                    .Distinct()
                    .ToList();
        }

        //Find all all referencing assembly of one assembly
        private List<ActivityAssemblyItem> FindAllReferencedAssemblies(IEnumerable<AssemblyName> referencedAssemblies, ActivityAssemblyItem referencer)
        {
            var result = new List<ActivityAssemblyItem>();

            foreach (var assemblyName in referencedAssemblies.Distinct())
            {
                var items = DisplayActivityAssemblyItems
                                .Where(assemblyItem => assemblyItem.AssemblyName.ToString() == assemblyName.ToString());

                if (items != null)
                {
                    foreach (ActivityAssemblyItem item in items)
                    {
                        if (item.ReferencedAssemblies.Any())
                        {
                            foreach (ActivityAssemblyItem activityItem in FindAllReferencedAssemblies(item.ReferencedAssemblies, item))
                            {
                                result.Add(activityItem);
                            }
                        }

                        AddToReferencingAssemblies(item, referencer);
                        result.Add(item);
                    }
                }
            }

            return result
                .Distinct()
                .ToList();
        }

        //Sets up the assembly/assembly's referencers collection
        private void AddToReferencingAssemblies(ActivityAssemblyItem activityItem, ActivityAssemblyItem referencer)
        {
            List<ActivityAssemblyItem> referencers;

            if (referencingAssemblies.TryGetValue(activityItem, out referencers))
                referencers.Add(referencer);
            else
                referencingAssemblies.Add(activityItem, new List<ActivityAssemblyItem> { referencer });
        }

    }
}
