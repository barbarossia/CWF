// -----------------------------------------------------------------------
// <copyright file="MarketplaceViewModel.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.ViewModels.Marketplace
{
    using System;
    using System.Collections.ObjectModel;
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.ViewModel;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.Support.Workflow.Authoring.Services;
    using CWF.DataContracts.Marketplace;
    using Microsoft.Support.Workflow.Authoring.Models;
    using CWF.DataContracts;
    using System.Reflection;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using System.Net;
    using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
    using Microsoft.Support.Workflow.Authoring.Security;
    using System.Threading;
    using System.Diagnostics;
    using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
    using Microsoft.Support.Workflow.Authoring.AddIns.Data;

    /// <summary>
    /// Viewmodel for the Marketplace download wizard.
    /// </summary>
    public class MarketplaceViewModel :ViewModelBase
    {
        private const string loadAssetCaption = "Loading marketplace assets...";
        private string currentUserRole;
        private string searchText = string.Empty;   // the text we will send to the Marketplace Service on search.
        private string filter;        // "All", "Projects", "Activities", "Templates" or "Publishing" -- sent to the back end on search.
        private bool isOnlyShowLatestVersion = true;
        private int currentPage = 1;          // The page we are currently on.
        private int resultsPerPage = 15;      // The page size for the result set.
        private int pageCount = 0;// The page count for the result set.
        private string downloadProgress;
        private bool isDownloadCompleted;
        private bool isBeginDownload;
        private Dictionary<long, MarketplaceAssetModel> activitiesSelections = new Dictionary<long, MarketplaceAssetModel>();   // The set of items that we will download. this is the database ID. On paging, these need to be interleaved with the page data from the back end.
        private Dictionary<long, MarketplaceAssetModel> projectsSelections = new Dictionary<long, MarketplaceAssetModel>();   // The set of items that we will download. this is the database ID. On paging, these need to be interleaved with the page data from the back end.

        private MarketplaceAssetDownloader downloader;
        private MarketplaceSearchQuery query = new MarketplaceSearchQuery();
        private MarketplaceAssetModel selectedAssetItem;
        private ObservableCollection<MarketplaceAssetModel> resultList = new ObservableCollection<MarketplaceAssetModel>();        // The results of calling Search on the back end.
        private ObservableCollection<string> filterListEntries = new ObservableCollection<string>(); // The list of entries to show in the filter list.
        private string sortMemberPath;
        private bool isAscending;
        private WorkflowItem currentWorkflow;

        /// <summary>
        /// Begin the pagechanged process.
        /// </summary>
        public DelegateCommand PageChangedCommand { get; private set; }

        /// <summary>
        /// Send a search request to the back end, and bind the results to ResultList.
        /// </summary>
        public DelegateCommand SearchCommand { get; private set; }

        /// <summary>
        /// Begin the download process.
        /// </summary>
        public DelegateCommand DownloadCommand { get; private set; }

        /// <summary>
        /// Stop the download process.
        /// </summary>
        public DelegateCommand CancelDownloadCommand { get; private set; }

        /// <summary>
        /// Open asset details command
        /// </summary>
        public DelegateCommand OpenAssetCommand { get; private set; }


        /// <summary>
        /// Gets a Dictionary value that contains user selected Activity type marketplaceasset
        /// </summary>
        public Dictionary<long, MarketplaceAssetModel> ActivitySelections
        {
            get { return this.activitiesSelections; }
        }

        /// <summary>
        /// Gets a Dictionary value that contains user selected Project type marketplaceasset
        /// </summary>
        public Dictionary<long, MarketplaceAssetModel> ProjectSelections
        {
            get { return this.projectsSelections; }
        }

        /// <summary>
        /// Gets a MarketplaceDownloader value in the current marketplace
        /// </summary>
        public MarketplaceAssetDownloader AssetDownloader
        {
            get { return this.downloader; }
            set { this.downloader = value; }
        }

        /// <summary>
        /// Gets or sets a value of DownloadProgress
        /// </summary>
        public string DownloadProgress
        {
            get { return this.downloadProgress; }
            set
            {
                this.downloadProgress = value;
                RaisePropertyChanged(() => DownloadProgress);
            }
        }

        /// <summary>
        /// flag for show MarketplaceAsset's latest version
        /// </summary>
        public bool IsOnlyShowLatestVersion
        {
            get { return this.isOnlyShowLatestVersion; }
            set
            {
                this.isOnlyShowLatestVersion = value;
                RaisePropertyChanged(() => IsOnlyShowLatestVersion);
                SearchCommand.Execute();
            }
        }

        /// <summary>
        /// Gets or sets a value indicates if the current download process is finished
        /// </summary>
        public bool IsDownloadCompleted
        {
            get { return this.isDownloadCompleted; }
            set
            {
                this.isDownloadCompleted = value;
                RaisePropertyChanged(() => IsDownloadCompleted);
            }
        }

        /// <summary>
        /// The list of results from a search on the server.
        /// </summary>
        public ObservableCollection<MarketplaceAssetModel> ResultList
        {
            get { return resultList; }
            set
            {
                if (value == null)
                    resultList = new ObservableCollection<MarketplaceAssetModel>();
                else
                    resultList = value;
                RaisePropertyChanged(() => ResultList);
                RaisePropertyChanged(() => NoResultsTextVisible);
            }
        }

        /// <summary>
        /// The list of elements to show in the filter list.
        /// </summary>
        public ObservableCollection<string> FilterListEntries
        {
            get { return filterListEntries; }
            set
            {
                filterListEntries = value;
                RaisePropertyChanged(() => FilterListEntries);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MarketplaceAssetModel SelectedAssetItem
        {
            get { return this.selectedAssetItem; }
            set
            {
                this.selectedAssetItem = value;
                RaisePropertyChanged(() => SelectedAssetItem);
            }
        }

        /// <summary>
        /// The current page being displayed to the user.
        /// </summary>
        public int CurrentPage
        {
            get { return currentPage; }
            set
            {
                currentPage = value; // Keep it in the boundaries of the available pages.
                RaisePropertyChanged(() => CurrentPage);
            }
        }

        /// <summary>
        /// The size of the pages we want back from the server.
        /// </summary>
        public int ResultsPerPage
        {
            get { return resultsPerPage; }
            set
            {
                resultsPerPage = value;
                RaisePropertyChanged(() => ResultsPerPage);
            }
        }

        /// <summary>
        /// The total Pages.
        /// </summary>
        public int PageCount
        {
            get { return this.pageCount; }
            set
            {
                pageCount = value;
                RaisePropertyChanged(() => PageCount);
            }
        }

        /// <summary>
        /// The search text we will send to the server.
        /// </summary>
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                RaisePropertyChanged(() => SearchText);
            }
        }

        /// <summary>
        ///  "All", "Projects", "Activities", "Templates" or "Publishing" -- sent to the back end on search.
        /// </summary>
        public string Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                RaisePropertyChanged(() => Filter);
                RaisePropertyChanged(() => FilterDescription);
                SearchCommand.Execute();
            }
        }

        /// <summary>
        /// Get a FilterDescription value when Filter changed
        /// </summary>
        public string FilterDescription
        {
            get
            {
                string result = String.Empty;
                if (Filter == "ALL")
                    result = "ALL PROJECTS AND ACTIVITIES";
                else if (Filter == "PROJECTS")
                    result = "PROJECTS";
                else if (Filter == "ACTIVITIES")
                    result = "ACTIVITIES";
                else if (Filter == "TEMPLATES")
                    result = "PROJECT TEMPLATES";
                else if (Filter == "PUBLISHING")
                    result = "PUBLISHING WORKFLOW PROJECTS";
                return result;
            }
        }

        /// <summary>
        /// Get or sets a value indicates if the download process is begun.
        /// </summary>
        public bool IsBeginDownload
        {
            get { return this.isBeginDownload; }
            set
            {
                this.isBeginDownload = value;
                RaisePropertyChanged(() => IsBeginDownload);
                CancelDownloadCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets a value that indicates if display the NoResultsText
        /// </summary>
        public bool NoResultsTextVisible
        {
            get { return this.ResultList.Count > 0 ? false : true; }
        }

        /// <summary>
        /// sort column name that user selected
        /// </summary>
        public string SortMemberPath
        {
            get { return this.sortMemberPath; }
            set
            {
                this.sortMemberPath = value;
                RaisePropertyChanged(() => SortMemberPath);
            }
        }

        /// <summary>
        /// SortCriterion that user selected
        /// </summary>
        public bool IsAscending
        {
            get { return this.isAscending; }
            set
            {
                this.isAscending = value;
                RaisePropertyChanged(() => IsAscending);
            }
        }

        public MarketplaceViewModel()
            : this(null)
        {
        }

        /// <summary>
        /// Set up commands, do an initial search to populate the list the first time.
        /// </summary>
        public MarketplaceViewModel(WorkflowItem focusedWorkflow)
        {
            downloader = new MarketplaceAssetDownloader(SynchronizationContext.Current, focusedWorkflow);
            this.currentWorkflow = focusedWorkflow;
            SearchCommand = new DelegateCommand((Action)(() =>
            {
                CurrentPage = 1;
                WorkflowsQueryServiceUtility.UsingClient(SearchMarketplaceAssets);
            }));

            DownloadCommand = new DelegateCommand(DownloadExecute,
                () =>
                {
                    return this.ResultList.Any(i => i.IsMarkedForDownload == true) && (!IsBeginDownload);
                });

            CancelDownloadCommand = new DelegateCommand(CancelDownloadExecute, () => { return IsBeginDownload; });

            OpenAssetCommand = new DelegateCommand(this.OpenAssetDetailsCommandExecute, () => { return this.SelectedAssetItem != null; });

            PageChangedCommand = new DelegateCommand(() =>
            {
                WorkflowsQueryServiceUtility.UsingClient(SearchMarketplaceAssets);
            });

            SetDefaultValue();
        }


        /// <summary>
        /// search marketplace asset from queryservice
        /// </summary>
        public void SearchMarketplaceAssets(IWorkflowsQueryService client)
        {
            try
            {
                this.BuildMarketplaceSearchQuery();
                SetDownloadSelections();
                Utility.DoTaskWithBusyCaption(loadAssetCaption, () =>
                {
                    MarketplaceSearchResult result = client.SearchMarketplace(this.query);
                    result.StatusReply.CheckErrors();
                    LoadMarketplace(result);
                });
            }
            catch (Exception ex)
            {
                MarketplaceExceptionHandler.HandleSearchException(ex);
                IsBusy = false;
            }
        }

        /// <summary>
        /// Raise Commands CanExecute
        /// </summary>
        public void RaiseCommandsCanExecute()
        {
            DownloadCommand.RaiseCanExecuteChanged();
            CancelDownloadCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Open file where downloaded activities projects is located
        /// </summary>
        [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]
        public void OpenDownloadLocation(string location)
        {
            if (!string.IsNullOrEmpty(location) && File.Exists(location))
            {
                Process.Start("explorer.exe", "/select, " + location);
            }
        }

        public bool CloseMarketplace()
        {
            bool shouldClose = true;
            if (this.IsBeginDownload)
            {
                shouldClose = MessageBoxService.ShoudExitWithMarketplaceDownloading();
                if (shouldClose)
                    this.CancelDownloadExecute();
            }
            return shouldClose;
        }

        /// <summary>
        /// Open asset details window
        /// </summary>
        private void OpenAssetDetailsCommandExecute()
        {
            Marketplace.MarketplaceAssetDetailsViewModel vm = new MarketplaceAssetDetailsViewModel(this.SelectedAssetItem);
            DialogService.ShowDialog(vm);
        }

        /// <summary>
        /// Set default value for filter, datapaging ...
        /// </summary>
        private void SetDefaultValue()
        {
            //Get current user role
            this.FilterListEntries.Clear();
            if (AuthorizationService.Validate(Env.All, Permission.ViewMarketplace))
            {
                new[] { "ALL", "PROJECTS", "ACTIVITIES", "TEMPLATES", "PUBLISHING", }
                                   .ToList()
                                   .ForEach(item => FilterListEntries.Add(item));
                currentUserRole = "Admin";
            }
            else
            {
                new[] { "ALL", "PROJECTS", "ACTIVITIES", }.ToList().ForEach(item => FilterListEntries.Add(item));
                currentUserRole = "Author";
            }

            this.filter = "ALL";
            this.SortMemberPath = "UpdatedDate";
            this.IsAscending = false;
            this.resultsPerPage = int.Parse(System.Configuration.ConfigurationManager.AppSettings["RowsPerPage"]);
        }

        /// <summary>
        /// create MarketplaceSearchQuery
        /// </summary>
        private void BuildMarketplaceSearchQuery()
        {
            if (string.IsNullOrEmpty(this.Filter))
                this.Filter = MarketplaceFilter.None.ToString();
            if (this.Filter == "ALL")
                query.FilterType = MarketplaceFilter.None;
            else if (Filter == "PROJECTS")
                query.FilterType = MarketplaceFilter.Projects;
            else if (Filter == "ACTIVITIES")
                query.FilterType = MarketplaceFilter.Activities;
            else if (Filter == "TEMPLATES")
                query.FilterType = MarketplaceFilter.Templates;
            else if (Filter == "PUBLISHING")
                query.FilterType = MarketplaceFilter.PublishingWorkflows;
            query.IsNewest = IsOnlyShowLatestVersion;
            query.PageNumber = this.CurrentPage;
            query.PageSize = ResultsPerPage;
            query.SearchText = this.SearchText;
            query.SortCriteria = new List<SortCriterion>() 
            {  
                new SortCriterion
              { 
                  FieldName=this.SortMemberPath,
                  IsAscending = IsAscending,
             } 
            };
            query.UserRole = currentUserRole;
        }

        /// <summary>
        /// load marketplace data for UI displaying
        /// </summary>
        /// <param name="results"></param>
        private void LoadMarketplace(MarketplaceSearchResult results)
        {
            if (results == null || results.Items == null)
            {
                PageCount = 0;
                CurrentPage = 1;
                ResultList = null;
                return;
            }

            SetDownloadSelections();
            this.PageCount = results.PageCount;
            this.CurrentPage = results.PageNumber;
            this.ResultList = new ObservableCollection<MarketplaceAssetModel>(results.Items.ToList().Select(a =>
            {
                MarketplaceAssetModel m = (MarketplaceAssetModel)a;
                if (activitiesSelections.ContainsKey(m.Id) || projectsSelections.ContainsKey(m.Id))
                    m.IsMarkedForDownload = true;

                if (m.AssetType == AssetType.Project)
                    m.IsDownloaded = IsProjectExists(m);
                else if (Caching.ActivityAssemblyItems.Any(i => i.Name == m.Name && i.Version.ToString() == m.Version))
                {
                    m.IsDownloaded = true;
                    m.Location = Caching.ActivityAssemblyItems.Where(i => i.Name == m.Name && i.Version.ToString() == m.Version).FirstOrDefault().Location;
                }
                return m;
            }));
        }

        /// <summary>
        ///for download assemblies, record assets which are MarkedForDownload into  activitiesSelections and projectsSelections
        /// </summary>
        /// <returns></returns>
        private void SetDownloadSelections()
        {
            //record markfordownload item
            if (this.ResultList != null)
            {
                this.ResultList.ToList().ForEach(i =>
                {
                    if (i.IsMarkedForDownload)
                    {
                        if (i.AssetType == AssetType.Activities && !activitiesSelections.ContainsKey(i.Id))
                            this.activitiesSelections.Add(i.Id, i);
                        if (i.AssetType == AssetType.Project && !projectsSelections.ContainsKey(i.Id))
                            this.projectsSelections.Add(i.Id, i);
                    }
                });
                RaiseCommandsCanExecute();
            }
        }

        /// <summary>
        /// clear asset's MarkedForDownload
        /// invoked when finish download
        /// </summary>
        private void ClearDownloadSelections()
        {
            this.activitiesSelections.Clear();
            this.projectsSelections.Clear();
            foreach (MarketplaceAssetModel m in this.ResultList)
            {
                m.IsMarkedForDownload = false;
                if (m.AssetType == AssetType.Project)
                    m.IsDownloaded = IsProjectExists(m);
                else if (Caching.ActivityAssemblyItems.Any(i => i.Name == m.Name && i.Version.ToString() == m.Version))
                {
                    m.IsDownloaded = true;
                    m.Location = Caching.ActivityAssemblyItems.Where(i => i.Name == m.Name && i.Version.ToString() == m.Version).FirstOrDefault().Location;
                }
            }
            RaiseCommandsCanExecute();
        }

        /// <summary>
        /// download marketplace command execute
        /// </summary>
        private void DownloadExecute()
        {
            if (IsBeginDownload)
                return;
            SetDownloadSelections();
            IsDownloadCompleted = false;
            var downloadList = this.activitiesSelections.Values.ToList();
            downloadList.AddRange(this.projectsSelections.Values.ToList());

            downloader.MarketpalceAssetsToDownload = new ObservableCollection<MarketplaceAssetModel>(downloadList);
            downloader.IsCancelDownload = false;
            downloader.DownloadedPercentChanged += new DownloadedPercentChangedEventHandler(DownloadedPercentChanged);
            downloader.DownloadCompleted += new EventHandler(DownloadCompleted);
            IsBeginDownload = true;


            downloader.StartDownload();
        }

        /// <summary>
        ///Marketplaceasset download complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadCompleted(object sender, EventArgs e)
        {
            IsBeginDownload = false;
            if (!downloader.IsCancelDownload)
            {
                ClearDownloadSelections();
                IsDownloadCompleted = true;
            }
            else
            {
                ClearDownloadSelections();
                IsDownloadCompleted = false;
            }
        }

        /// <summary>
        /// Cancel download process
        /// </summary>
        private void CancelDownloadExecute()
        {
            IsDownloadCompleted = false;
            IsBeginDownload = false;
            this.downloader.IsCancelDownload = true;
            this.ClearDownloadSelections();
        }

        /// <summary>
        /// DownloadedPercentChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadedPercentChanged(object sender, DownloadPercentEventArgs e)
        {
            DownloadProgress = "Downloading... (" + e.DownloadPercent + ")";
        }

        /// <summary>
        /// validate if the project already exist
        /// </summary>
        /// <returns></returns>
        private bool IsProjectExists(MarketplaceAssetModel model)
        {
            string targetFileName = Utility.GetProjectsDirectoryPath() + "\\" + model.Name + "_" + model.Version + ".wf";
            model.Location = targetFileName;
            return File.Exists(targetFileName);
        }
    }
}
