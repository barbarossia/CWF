using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using CWF.DataContracts.Marketplace;
using Microsoft.Practices.Prism.ViewModel;

namespace Microsoft.Support.Workflow.Authoring.Models
{
    public class MarketplaceAssetModel : NotificationObject
    {
        public long Id { get; set; }
        public AssetType AssetType { get; set; }
        public string CreatedBy { get; set; }
        public bool? IsPublishingWorkflow { get; set; }
        public bool? IsTemplate { get; set; }
        public string Name { get; set; }
        public DateTime UpdatedDate { get; set; }

        private bool isAddToToolbox;
        public bool IsAddToToolbox
        {
            get { return this.isAddToToolbox; }
            set
            {
                this.isAddToToolbox = value;
                RaisePropertyChanged(() => IsAddToToolbox);
            }
        }


        private bool isDownloaded;
        public bool IsDownloaded
        {
            get { return this.isDownloaded; }
            set
            {
                this.isDownloaded = value;
                RaisePropertyChanged(() => IsDownloaded);
            }
        }

        private bool isMarkedForDownload;
        public bool IsMarkedForDownload
        {
            get { return this.isMarkedForDownload; }
            set
            {
                this.isMarkedForDownload = value;
                RaisePropertyChanged(() => IsMarkedForDownload);
            }
        }

        private string location;
        public string Location
        {
            get { return this.location; }
            set
            {
                this.location = value;
                RaisePropertyChanged(() => Location);
            }
        }

        public string Version { get; set; }

        public static explicit operator MarketplaceAssetModel(MarketplaceAsset asset)
        {
            return new MarketplaceAssetModel()
            {
                Id = asset.Id,
                Name = asset.Name,
                AssetType = asset.AssetType,
                IsPublishingWorkflow = asset.IsPublishingWorkflow,
                IsTemplate = asset.IsTemplate,
                Version = asset.Version,
                CreatedBy = asset.CreatedBy,
                UpdatedDate = asset.UpdatedDate,
                IsAddToToolbox = asset.AssetType == AssetType.Activities ? true : false
            };
        }
    }
}
