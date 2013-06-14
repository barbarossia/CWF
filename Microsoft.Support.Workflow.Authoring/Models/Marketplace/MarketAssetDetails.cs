using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.Support.Workflow.Authoring.Models.Marketplace
{
    public class MarketAssetDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaTags { get; set; }
        public string ThumbnailUrl { get; set; }
        public ObservableCollection<ActivityQuickInfo> Activities = new ObservableCollection<ActivityQuickInfo>();
    }
}
