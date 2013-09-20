using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.Common;
using CWF.DataContracts;
using CWF.DataContracts.Marketplace;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Windows.Forms;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Microsoft.Support.Workflow.Authoring.Tests.HelpClass
{
    /// <summary>
    /// The class is responsible for preparing test data for marketplace unit test
    /// </summary>
    public class MarketplaceDataHelper
    {
        private List<WorkflowItem> wfs;
        private List<ActivityAssemblyItem> acts;
        private TestContext context;

        public MarketplaceDataHelper(TestContext contextParam)
        {
            context = contextParam;
            wfs = GetTestProjects();
            acts = GetTestActivities();
        }

        public List<WorkflowItem> GetTestProjects()
        {
            var projects = new List<WorkflowItem>();
            string wf1Path2 = Path.Combine(context.DeploymentDirectory, @"MarketplaceTestData\MKPUT1.wf");
            //string wf3Path = Path.Combine(context.DeploymentDirectory, @"MarketplaceTestData\MKPUT3.wf"); ;

            string wf1Path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Data", "MarketplaceTestData", "MKPUT1.wf");
            if (!File.Exists(wf1Path))
                wf1Path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "MarketplaceTestData", "MKPUT1.wf");
            string wf3Path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Data", "MarketplaceTestData", "MKPUT3.wf");
            if (!File.Exists(wf3Path))
                wf3Path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "MarketplaceTestData", "MKPUT3.wf");
            //load projects from local
            var recoverdWorkflow1 = (WorkflowItem)Utility.DeserializeSavedContent(wf1Path);
            recoverdWorkflow1.IsDataDirty = false;
            recoverdWorkflow1.Status = MarketplaceStatus.Public.ToString();
            recoverdWorkflow1.Env = Authoring.AddIns.Data.Env.Dev;

            var recoverdWorkflow3 = (WorkflowItem)Utility.DeserializeSavedContent(wf3Path);
            recoverdWorkflow3.IsDataDirty = false;
            recoverdWorkflow3.Status = MarketplaceStatus.Public.ToString();
            recoverdWorkflow3.Env = Authoring.AddIns.Data.Env.Dev;

            projects.Add(recoverdWorkflow1);
            projects.Add(recoverdWorkflow3);

            return projects;
        }

        public List<ActivityAssemblyItem> GetTestActivities()
        {
            var activities = new List<ActivityAssemblyItem>();
            // string activitiesPath = Path.Combine(context.DeploymentDirectory, @"MarketplaceTestData\Assemblies\ActivityAssemblyCatalog.txt");
            string activitiesPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"Data\MarketplaceTestData\Assemblies\ActivityAssemblyCatalog.txt");
            if(!File.Exists(activitiesPath))
                activitiesPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"MarketplaceTestData\Assemblies\ActivityAssemblyCatalog.txt");
            //load activities
            var deserializedAssembly = Utility.DeserializeSavedContent(activitiesPath) as IEnumerable<ActivityAssemblyItem>;

            //load assemblies from local
            deserializedAssembly.ToList().ForEach(item =>
            {
                item.Status = MarketplaceStatus.Public.ToString();
                item.Env = Authoring.AddIns.Data.Env.Dev;
                item.UserWantsToUpload = true;
                item.Location = item.Location.Replace(".\\Assemblies", ".\\MarketplaceTestData\\Assemblies");
            });
            return deserializedAssembly.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MarketplaceSearchResult GetMarketplaceSearchResult()
        {
            var result = new MarketplaceSearchResult();
            result.Items = new List<MarketplaceAsset>();
            result.PageCount = 1;
            result.PageNumber = 1;
            result.PageSize = 15;
            int id = 0;

            this.acts.ForEach(item =>
            {
                result.Items.Add(new MarketplaceAsset
                {
                    Name = item.Name,
                    Version = item.Version.ToString(),
                    AssetType = CWF.DataContracts.Marketplace.AssetType.Activities,
                    CreatedBy = "v-kason",
                    UpdatedDate = DateTime.Now,
                    Id = ++id,
                    Environment = "dev",
                });
            });

            this.wfs.ForEach(item =>
            {
                result.Items.Add(new MarketplaceAsset
                {
                    Name = item.Name,
                    Version = item.Version.ToString(),
                    AssetType = CWF.DataContracts.Marketplace.AssetType.Project,
                    CreatedBy = "v-kason",
                    UpdatedDate = DateTime.Now,
                    Id = ++id,
                    Environment = "dev",
                });
            });
            return result;
        }

        public MarketplaceAssetDetails Activities_GetMarketplaceAssetDetailsIncludeActivityItems()
        {
            var details = new MarketplaceAssetDetails();
            var asset = this.acts[0];

            details.Description = "woooooooooooooo";
            details.Name = asset.Name;
            details.MetaTages = "cmnerjm.bcfiefneifoje";
            details.Activities = new List<ActivityQuickInfo>();
            if (asset.ActivityItems != null && asset.ActivityItems.Count > 0)
            {
                asset.ActivityItems.ToList().ForEach(item =>
                {
                    details.Activities.Add(new ActivityQuickInfo { Name = item.Name, Version = item.Version });
                });
            }
            details.CategoryName = "Unassigned";

            return details;
        }

        public MarketplaceAssetDetails Activities_GetMarketplaceAssetDetailsNoActivityItems()
        {
            var details = new MarketplaceAssetDetails();
            var asset = this.acts[0];

            details.Description = "woooooooooooooo";
            details.Name = asset.Name;
            details.MetaTages = "cmnerjm.bcfiefneifoje";
            details.Activities = null;
            details.CategoryName = "Unassigned";

            return details;
        }

        public MarketplaceAssetDetails Project_GetMarketplaceAssetDetails()
        {
            var details = new MarketplaceAssetDetails();
            var asset = this.wfs[0];

            details.Description = "woooooooooooooo";
            details.Name = asset.Name;
            details.MetaTages = "cmnerjm.bcfiefneifoje";
            details.Activities = null;
            return details;
        }

        public byte[] GetActivityData(string file)
        {
            byte[] array = null;
            using (Stream stream = File.Open(file, FileMode.Open))
            {
                array = new byte[stream.Length];
                stream.Read(array, 0, array.Length);
            }
            return array;
        }
    }
}
