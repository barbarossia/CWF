using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DataContracts.Marketplace;
using Microsoft.Support.Workflow.Authoring.Models;
using CWF.DataContracts;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using System.Net;
using System.ComponentModel;
using System.Threading;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.Security;

namespace Microsoft.Support.Workflow.Authoring.Services
{
    public class DownloadPercentEventArgs : EventArgs
    {
        public string DownloadPercent { get; set; }
        public DownloadPercentEventArgs(string percent)
        {
            this.DownloadPercent = percent;
        }
    }

    public delegate void DownloadedPercentChangedEventHandler(object sender, DownloadPercentEventArgs e);

    public class MarketplaceAssetDownloader
    {
        private int marketplaceCount;//the count wanted to be downloaded
        private int currentDownloadingNumber;//the current number that is downloading
        private string downloadPercent;
        private bool isCancelDownload;
        private ObservableCollection<MarketplaceAssetModel> marketplaceToDownload;
        private List<MarketplaceAssetModel> projects;
        private List<MarketplaceAssetModel> activities;
        private event EventHandler downloadCompleted;
        private SynchronizationContext uiContext = null;
        private WorkflowItem currentWorkflow;

        /// <summary>
        /// DownloadedPercentChanged event
        /// </summary>
        public event DownloadedPercentChangedEventHandler DownloadedPercentChanged;

        /// <summary>
        ///DownloadCompleted Event
        /// </summary>
        public event EventHandler DownloadCompleted
        {
            add { downloadCompleted += value; }
            remove { downloadCompleted -= value; }
        }

        /// <summary>
        /// Gets a value indicates the number that have downloaded asset.
        /// </summary>
        public int DownloadedNumber
        {
            get { return this.currentDownloadingNumber; }
        }

        /// <summary>
        /// Gets a value indicates the download asset's total count 
        /// </summary>
        public int ToDownloadCount
        {
            get { return this.marketplaceCount; }
        }

        /// <summary>
        /// Gets or sets a value indicate if Download process is canceled.
        /// </summary>
        public bool IsCancelDownload
        {
            get { return this.isCancelDownload; }
            set
            {
                this.isCancelDownload = value;
            }
        }

        /// <summary>
        /// Get or set the collection wanted to download 
        /// </summary>
        public ObservableCollection<MarketplaceAssetModel> MarketpalceAssetsToDownload
        {
            get { return this.marketplaceToDownload; }
            set
            {
                this.marketplaceToDownload = value;
                if (this.marketplaceToDownload != null)
                {
                    this.marketplaceCount = this.marketplaceToDownload.Count;
                    this.projects = this.marketplaceToDownload.Where(i => i.AssetType == AssetType.Project).ToList();
                    this.activities = this.marketplaceToDownload.Where(i => i.AssetType == AssetType.Activities).ToList();
                }
            }
        }

        public MarketplaceAssetDownloader()
        {
            this.currentDownloadingNumber = 0;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public MarketplaceAssetDownloader(SynchronizationContext uiContext, WorkflowItem focusedWorkflow)
        {
            this.currentDownloadingNumber = 0;
            this.uiContext = uiContext;
            this.currentWorkflow = focusedWorkflow;
        }


        /// <summary>
        /// Begin the download process
        /// </summary>
        public void StartDownload()
        {
            currentDownloadingNumber = 0;
            SetDownloadProgress();

            if (ShouldStopDownload())
            {
                isCancelDownload = true;
                return;
            }

            try
            {
                using (BackgroundWorker _backgroundWorker = new BackgroundWorker())
                {
                    _backgroundWorker.DoWork += (s, e) =>
                    {
                        WorkflowsQueryServiceUtility.UsingClient(DownloadProjects);
                        WorkflowsQueryServiceUtility.UsingClient(DownloadActivities);
                    };
                    _backgroundWorker.RunWorkerCompleted += (s, e) =>
                    {
                        RaiseDownloadCompleted();
                    };
                    _backgroundWorker.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is WebException)
                    MarketplaceExceptionHandler.HandleDownloadException(ex);
            }
        }

        /// <summary>
        /// for test,check if should complete the download
        /// </summary>
        /// <returns></returns>
        public bool ShouldStopDownload()
        {
            if (this.marketplaceToDownload == null || this.marketplaceToDownload.Count <= 0 || IsCancelDownload)
            {
                this.RaiseDownloadCompleted();
                return true;
            }
            return false;
        }



        /// <summary>
        /// Download MarketplaceAsset that AssetType is equal to Activities
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="assetType"></param>
        public void DownloadActivities(IWorkflowsQueryService client)
        {
            var downloadedAssemblies = new List<ActivityAssemblyItem>();
            foreach (MarketplaceAssetModel model in this.activities)
            {
                if (IsCancelDownload)
                {
                    CancelDownload();
                    return;
                }

                ActivityAssemblyItem toDownloadAssembly = new ActivityAssemblyItem 
                { 
                    Name = model.Name,
                    Version = System.Version.Parse(model.Version) ,
                    Env = model.Env.ToEnv()
                };
                AssemblyDownloader.GetActivityItemsByActivityAssemblyItem(toDownloadAssembly, client);

                //download dependendies
                var dependecies = Caching.ComputeDependencies(client, toDownloadAssembly);
                dependecies.Add(toDownloadAssembly);

                var toCachedAssembly = Caching.DownloadAssemblies(client, dependecies);
                toCachedAssembly.ToList().ForEach(i => i.UserSelected = model.IsAddToToolbox);

                downloadedAssemblies.AddRange(toCachedAssembly);

                //set download progress
                currentDownloadingNumber++;
                SetDownloadProgress();
            }

            if (!IsCancelDownload)
            {
                Caching.CacheAssembly(downloadedAssemblies);
                Caching.Refresh();
                if (this.currentWorkflow != null)
                {
                    ObservableCollection<ActivityAssemblyItem> importAssemblies = new ObservableCollection<ActivityAssemblyItem>(this.currentWorkflow.WorkflowDesigner.DependencyAssemblies);
                    downloadedAssemblies.ForEach(i =>
                    {
                        if (importAssemblies.SingleOrDefault(item => item.FullName != i.FullName) == null)
                            importAssemblies.Add(i);
                    });
                    this.currentWorkflow.WorkflowDesigner.ImportAssemblies(importAssemblies.ToList());
                }
                this.activities.ForEach(
                    m => m.Location = Caching.ActivityAssemblyItems
                        .Where(i => i.Name == m.Name && i.Version.ToString() == m.Version)
                        .First().Location);
            }
        }

        /// <summary>
        /// Download MarketplaceAsset assettype = Projects
        /// </summary>
        /// <param name="assets"></param>
        /// <returns></returns>
        public void DownloadProjects(IWorkflowsQueryService client)
        {
            List<StoreActivitiesDC> request = this.projects.Select(item =>
            {
                return new StoreActivitiesDC
                {
                    Name = item.Name,
                    Version = item.Version,
                    Environment = item.Env,
                    Incaller = Environment.UserName,
                    IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                };
            }).ToList<StoreActivitiesDC>();

            foreach (StoreActivitiesDC item in request)
            {
                if (IsCancelDownload)
                {
                    CancelDownload();
                    return;
                }
                var result = client.StoreActivitiesGet(item);
                if (result.Any())
                {
                    StoreActivitiesDC dc = result[0];
                    ActivityAssemblyItem assembly = new ActivityAssemblyItem 
                    {   
                        Name = dc.ActivityLibraryName, 
                        Version = System.Version.Parse(dc.Version),
                        Env = dc.Environment.ToEnv()
                    };
                    List<ActivityAssemblyItem> references = Caching.CacheAndDownloadAssembly(client, Caching.ComputeDependencies(client, assembly));
                    this.SaveProjectsToLocal(dc, references);
                    currentDownloadingNumber++;
                    SetDownloadProgress();
                }
            }
        }

        /// <summary>
        /// Save downloaded projects to local machine
        /// </summary>
        /// <param name="wfs"></param>
        /// <param name="filePath"></param>
        public void SaveProjectsToLocal(StoreActivitiesDC activity, List<ActivityAssemblyItem> references)
        {
            if (activity == null)
                return;
            try
            {
                string targetFileName = Utility.GetProjectsDirectoryPath() + "\\" + activity.ActivityLibraryName + "_" + activity.ActivityLibraryVersion + ".wf";
                using (var stream = File.Open(targetFileName, FileMode.Create))
                {
                    var formatter = new BinaryFormatter();
                    WorkflowItem wfItem = null;
                    ActivityAssemblyItem assembly = new ActivityAssemblyItem { Name = activity.Name, Version = System.Version.Parse(activity.Version) };
                    wfItem = DataContractTranslator.StoreActivitiyDCToWorkflowItem(activity, assembly, references);
                    formatter.Serialize(stream, wfItem);
                }
            }
            catch (Exception ex)//net work exception
            {
                MarketplaceExceptionHandler.HandleSaveProjectsException(ex);
            }
        }

        /// <summary>
        /// Clear download activities and projects from disk and caching
        /// </summary>
        public void CancelDownload()
        {
            if (!this.IsCancelDownload)
                return;

            List<string> toDeleteFiles = new List<string>();

            //clear activities downloaded to localmachine and remove them from caching
            if (this.activities.Count > 0)
            {
                foreach (var model in this.activities)
                {
                    var assemblyItem = Caching.ActivityAssemblyItems.Where(i => i.Name == model.Name && i.Version.ToString() == model.Version).FirstOrDefault();
                    if (assemblyItem != null)
                    {
                        //delete assembly and activity in caching
                        assemblyItem.ActivityItems.ToList().ForEach(i =>
                        {
                            Caching.ActivityItems.Remove(Caching.ActivityItems.FirstOrDefault(item => i.Name == i.Name && i.Version == item.Version));
                        });
                        Caching.ActivityAssemblyItems.Remove(assemblyItem);

                        //delete version file which contains the assembly dll
                        string rootFile = string.Format(@"{0}\{1}", Utility.GetAssembliesDirectoryPath(), assemblyItem.AssemblyName.Name);
                        string versionFile = string.Format(@"{0}\{1}", rootFile, assemblyItem.AssemblyName.Version.IfNotNull(v => v.ToString()) ?? "None");
                        if (Directory.Exists(versionFile))
                            toDeleteFiles.Add(versionFile);
                    }
                }
                Caching.Refresh();
                toDeleteFiles.ForEach(i => { FileService.DeleteDirectory(i); });
            }

            //clear projects download to local machine
            if (this.projects.Any())
            {
                string targetFileName = string.Empty;
                foreach (var model in this.projects)
                {
                    targetFileName = Utility.GetProjectsDirectoryPath() + "\\" + model.Name + "_" + model.Version + ".wf";
                    if (File.Exists(targetFileName))
                        File.Delete(targetFileName);
                }
            }
        }

        /// <summary>
        /// Raise DownloadCompleted event
        /// </summary>
        public void RaiseDownloadCompleted()
        {
            if (this.downloadCompleted != null)
            {
                downloadCompleted(this, new EventArgs());
            }
        }

        /// <summary>
        /// Set the download percent and raise the DownloadPercentChanged event;
        /// </summary>
        private void SetDownloadProgress()
        {
            downloadPercent = this.currentDownloadingNumber + "/" + marketplaceCount;
            if (this.DownloadedPercentChanged != null)
                DownloadedPercentChanged(this, new DownloadPercentEventArgs(downloadPercent));
        }
    }
}
