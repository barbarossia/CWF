using System;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Support.Workflow.Authoring.Models;
using CWF.DataContracts.Marketplace;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;

namespace Microsoft.Support.Workflow.Authoring.ViewModels.Marketplace
{
    public class FieldValue
    {
        public string Field { get; set; }
        public string Value { get; set; }

        public override bool Equals(object obj)
        {
            var fieldValue = obj as FieldValue;

            if (fieldValue != null && this.Field == fieldValue.Value)
                return true;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

    public class MarketplaceAssetDetailsViewModel : ViewModelBase
    {
        private MarketplaceAssetModel assetItem;
        private MarketplaceAssetDetails details = null;
        private ObservableCollection<ActivityQuickInfo> includedActivities = new ObservableCollection<ActivityQuickInfo>();
        private ObservableCollection<FieldValue> marketplaceAssetFieldValues = new ObservableCollection<FieldValue>();
        private string iconUrl;
        private string rightPaneTitle;
        private string assetName;

        /// <summary>
        /// Get a value indicate if the AssetType is activities
        /// </summary>
        public bool IsActivityType
        {
            get { return this.assetItem.AssetType == AssetType.Activities ? true : false; }
        }

        /// <summary>
        /// Gets user selected marketplace asset 
        /// </summary>
        public MarketplaceAssetModel SelectedMarketplaceAsset
        {
            get { return this.assetItem; }
        }

        /// <summary>
        ///MarketplaceAsset's Icon Address 
        /// </summary>
        public string IconUrl
        {
            get { return this.iconUrl; }
            set
            {
                this.iconUrl = value;
                RaisePropertyChanged(() => IconUrl);
            }
        }

        /// <summary>
        /// Current marketplace asset
        /// </summary>
        public MarketplaceAssetModel CurrentAssetItem
        {
            get { return this.assetItem; }
            set
            {
                this.assetItem = value;
                RaisePropertyChanged(() => this.CurrentAssetItem);
                RaisePropertyChanged(() => IsActivityType);
            }
        }

        /// <summary>
        ///MarketplaceAsset's Name 
        /// </summary>
        public string AssetName
        {
            get { return this.assetName; }
            set
            {
                this.assetName = value;
                RaisePropertyChanged(() => AssetName);
            }
        }

        /// <summary>
        /// Get or set RightPaneTitle according to the included activities's count
        /// </summary>
        public string RightPaneTitle
        {
            get { return this.rightPaneTitle; }
            set
            {
                this.rightPaneTitle = value;
                RaisePropertyChanged(() => RightPaneTitle);
            }
        }

        /// <summary>
        /// Get or set details matedata value with field-value pairs
        /// </summary>
        public ObservableCollection<FieldValue> MarketplaceAssetFieldValues
        {
            get { return this.marketplaceAssetFieldValues; }
            set
            {
                this.marketplaceAssetFieldValues = value;
                RaisePropertyChanged(() => MarketplaceAssetFieldValues);
            }
        }

        /// <summary>
        /// Gets a value indicates if LeftPane(MetaDataPane) should be visible
        /// </summary>
        public bool MetaDataVisible
        {
            get { return details == null ? false : true; }
        }

        /// <summary>
        /// Get a value indicates if the right pane(IncludedActivities) shoud be visible
        /// </summary>
        public bool ActivitiesVisible
        {
            get { return this.includedActivities.Count > 0; }
        }

        /// <summary>
        /// Get or set IncludedActivities according to the AssetType.
        /// </summary>
        public ObservableCollection<ActivityQuickInfo> IncludedActivities
        {
            get { return this.includedActivities; }
            set
            {
                this.includedActivities = value;
                RaisePropertyChanged(() => IncludedActivities);
                RaisePropertyChanged(() => ActivitiesVisible);
            }
        }

        public MarketplaceAssetDetailsViewModel(MarketplaceAssetModel assetItem)
        {
            this.CurrentAssetItem = assetItem;
            
            WorkflowsQueryServiceUtility.UsingClient(this.SearchMarketplaceAssertDetails);
        }

        /// <summary>
        /// load marketplace details data for UI displaying
        /// </summary>
        /// <param name="client"></param>
        public void SearchMarketplaceAssertDetails(IWorkflowsQueryService client)
        {
            if (assetItem == null)
                return;
            MarketplaceSearchDetail search = new MarketplaceSearchDetail();
            search.AssetType = assetItem.AssetType;
            search.Id = assetItem.Id;
            details = client.GetMarketplaceAssetDetails(search);
            RaisePropertyChanged(() => MetaDataVisible);
            RaisePropertyChanged(() => ActivitiesVisible);

            if (details != null)
            {
                this.MarketplaceAssetFieldValues.Clear();
                AssetName = this.details.Name;
                this.MarketplaceAssetFieldValues.Add(new FieldValue() { Field = "Type", Value = this.assetItem.AssetType.ToString() });
                this.MarketplaceAssetFieldValues.Add(new FieldValue() { Field = "Version", Value = this.assetItem.Version, });

                if (assetItem.AssetType == AssetType.Activities)
                {
                    this.MarketplaceAssetFieldValues.Add(new FieldValue() { Field = "Category", Value = details.CategoryName, });
                    if (details.Activities != null && details.Activities.Count > 0)
                    {
                        this.MarketplaceAssetFieldValues.Add(new FieldValue() { Field = "Activities", Value = details.Activities.Count.ToString(), });
                        this.IncludedActivities = new ObservableCollection<ActivityQuickInfo>(details.Activities);
                        this.RightPaneTitle = "INCLUDED ACTIVITIES(" + this.IncludedActivities.Count + ")";
                    }
                    else
                    {
                        this.RightPaneTitle = "INCLUDED ACTIVITIES(0)";
                        this.MarketplaceAssetFieldValues.Add(new FieldValue() { Field = "Activities", Value = "None", });
                    }
                }
                this.MarketplaceAssetFieldValues.Add(new FieldValue() { Field = "Description", Value = details.Description, });
                this.MarketplaceAssetFieldValues.Add(new FieldValue() { Field = "Tags", Value = details.MetaTages, });
                this.IconUrl = details.ThumbnailUrl;
            }
        }
    }
}
