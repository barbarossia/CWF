namespace DeployActivity
{
    #region References

    using System;
    using System.Activities;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using DeployActivity.DataContracts;
    using Microsoft.Web.Administration;

    #endregion References
    
    /// <summary>
    /// Deloy Activity
    /// </summary>
    public class DeployToIIS : CodeActivity
    {
        #region Fields and constants

        /// <summary>
        /// Error message for failing to create the IIS virtual directory
        /// </summary>
        private const string ERROR_MESSAGE_NOIIS_VIRTUAL_PATH = "Unable to create an IIS virtual directory at {0}, please contact your systems administrator.";
        /// <summary>
        /// The name of the default web site to use
        /// </summary>
        private const string DEFAULT_WEB_SITE = "Default Web Site";
        /// <summary>
        /// The format to use for naming the virtual path
        /// </summary>
        private const string VIRTUAL_PATH_FORMAT = "/{0}_{1}";
        /// <summary>
        /// The format of the Version(3) + next build number string
        /// </summary>
        private const string VERSION_FORMAT = "{0}.{1}";
        /// <summary>
        /// The file extension to use for the workflow service
        /// </summary>
        private const string XAMLX_EXTENSION = ".xamlx";

        #endregion Fields and constants

        #region In Arguments
        /// <summary>
        /// The base location for all workflows
        /// </summary>
        [RequiredArgument]
        public InArgument<string> FilePath { get; set; }

        /// <summary>
        /// The name of the workflow to publish
        /// </summary>
        [RequiredArgument]
        public InArgument<string> ActivityName { get; set; }

        /// <summary>
        /// The versionm of the workflow to publish as a string
        /// </summary>
        [RequiredArgument]
        public InArgument<string> ActivityVersion { get; set; }

        #endregion In Arguments

        #region Out Arguments
        /// <summary>
        /// The version that was published to the server
        /// </summary>
        public OutArgument<Version> PublishedVersion { get; set; }

        /// <summary>
        /// The location on the Server that we published 
        /// </summary>
        public OutArgument<string> PublishedLocation { get; set; }

        /// <summary>
        /// Any errors that happened in the Publish
        /// </summary>
        public OutArgument<string> PublishError { get; set; }

        #endregion Out Arguments

        #region Execute Method
        /// <summary>
        /// Execute 
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(CodeActivityContext context)
        {
            // Get the InArguments
            string publishPath = this.FilePath.Get(context);
            string workflowName = this.ActivityName.Get(context);
            string workflowVersion = this.ActivityVersion.Get(context);

            UsingClient(
                // Get the appropriate query service URI from the host
                context.GetExtension<QueryServiceURIExtension>().IfNotNull(ext => ext.QueryServiceURI),
                (client) =>
                {
                    var activity = GetDefinition(
                        client,
                        this.ActivityName.Get(context), this.ActivityVersion.Get(context));

                    // Have to check and see if we have already published this name and version of the workflow
                    string workflowPath = Path.Combine(publishPath, workflowName, workflowVersion);
                    Version thisVersion = new Version(workflowVersion);
                    int nextBuild = thisVersion.Build;
                    while (Directory.Exists(workflowPath))
                    {
                        workflowVersion = string.Format(VERSION_FORMAT, thisVersion.ToString(3), ++nextBuild);
                        workflowPath = Path.Combine(publishPath, workflowName, workflowVersion);
                    }
                    // Get the version that is to be published
                    Version newVersion = Version.Parse(workflowVersion);
                    // Download the dependencies                
                    DownloadDependencies(client, workflowPath, ComputeDependencies(client, activity));
                    // Get the XAML
                    var xaml = activity.XAML;
                    // If the activity is a workflow service then we write out XAMLX
                    if (activity.IsService == true)
                    {
                        // Write the XAML as XAMLX
                        File.WriteAllText(Path.Combine(workflowPath, activity.ShortName + XAMLX_EXTENSION), xaml);
                    }

                    // Create the IIS Virtual directory
                    string creationError = CreateIISVirtual(workflowName, newVersion, workflowPath);

                    // Set the output to the version we published and the location it went to
                    this.PublishedVersion.Set(context, newVersion);
                    this.PublishedLocation.Set(context, workflowPath);
                    this.PublishError.Set(context, creationError);
                });
        }

        #endregion Execute Method

        #region Public methods

        /// <summary>
        /// Compute ActivityAssemblyItem dependencies
        /// </summary>
        /// <param name="client"></param>
        /// <param name="workflowAssembly"></param>
        /// <returns></returns>
        public static StoreActivityLibrariesDependenciesDC ComputeDependencies(IWorkflowsQueryService client, StoreActivitiesDC activity)
        {
            var dependenciesByID = client.StoreActivityLibraryDependenciesTreeGet(
                new StoreActivityLibrariesDependenciesDC
                {
                    StoreDependenciesRootActiveLibrary =
                    new StoreDependenciesRootActiveLibrary()
                    {
                        activityLibraryId = activity.ActivityLibraryId,
                        activityLibraryName = activity.ActivityLibraryName,
                        activityLibraryVersionNumber = activity.ActivityLibraryVersion
                    },
                    Incaller = "DeployToIIS Activity",
                    IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString()
                });
            return dependenciesByID.Single();
        }

        /// <summary>
        /// Download dependencies and copy them to the deploy directory's \bin subdirectory
        /// </summary>
        /// <param name="client"></param>
        /// <param name="destinationDirectoryPath"></param>
        /// <param name="dependencies"></param>
        public static void DownloadDependencies(IWorkflowsQueryService client, string destinationDirectoryPath, StoreActivityLibrariesDependenciesDC dependencies)
        {
            // IIS needs assemblies to go under bin subdirectory whereas .xamlx goes in the root
            string binPath = Path.Combine(destinationDirectoryPath, "bin");
            Directory.CreateDirectory(binPath);
        
            foreach (var dependentAssembly in dependencies.StoreDependenciesDependentActiveLibraryList)
            {
                var reply = client.ActivityLibraryGet(new ActivityLibraryDC
                {
                    Incaller = "DeployToIIS Activity",
                    IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    Id = dependentAssembly.activityLibraryDependentId
                }).Single();
                string destFile = Path.Combine(binPath, reply.Name + ".dll");
                File.WriteAllBytes(destFile, reply.Executable);
            }
        }

        #endregion Public methods

        #region Private helpers
        /// <summary>
        /// Create an IIS Directory
        /// </summary>
        /// <param name="workflowName">Workflow name</param>
        /// <param name="workflowVersion">Workflow version as a Version</param>
        /// <param name="pathToCreate">The path that we created for the workflow</param>
        private string CreateIISVirtual(string workflowName, Version workflowVersion, string pathToCreate)
        {
            string resultString = null;
            
            try
            {
                using (var mgr = new ServerManager())
                {
                    mgr.Sites[DEFAULT_WEB_SITE].Applications.Add(string.Format(VIRTUAL_PATH_FORMAT, workflowName, workflowVersion.ToString()), pathToCreate);
                    mgr.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                resultString = string.Format(ERROR_MESSAGE_NOIIS_VIRTUAL_PATH, pathToCreate, ex);
            }

            return resultString;
        }
        
        private void UsingClient(Uri endpointUri, Action<WorkflowsQueryServiceClient> a)
        {
            // Set up binding explicitly to decouple from the app.config of host.
            // Alternatively we could get it from the extension but there's no
            // scenario for that right now since all QueryServices use the same bindings.
            var binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;

            var client = new WorkflowsQueryServiceClient(
                binding,
                new EndpointAddress(endpointUri)
                );
            bool success = false;
            try
            {
                a(client);
                client.Close();
                success = true;
            }
            finally
            {
                if (!success)
                {
                    client.Abort();
                }
            }
        }

        /// <summary>
        /// Gets the workflow definition from the database by name and version
        /// </summary>
        /// <param name="client">The client to use for the Query Service calls</param>
        /// <param name="activityName">The name of the workflow to get</param>
        /// <param name="activityVersion">The version of the workflow to get</param>
        /// <returns>StoreActivitiesDC</returns>
        private StoreActivitiesDC GetDefinition(WorkflowsQueryServiceClient client, string activityName, string activityVersion)
        {

            var workflows = client.StoreActivitiesGet(new StoreActivitiesDC()
            {
                Incaller = Environment.UserName,
                IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            });

            var activityDC = workflows.FirstOrDefault(wf => wf.ShortName.ToLower() == activityName.ToLower() && wf.Version == activityVersion);

            if (activityDC == null)
            {
                throw new InvalidOperationException("No definition found for that Name/Version");
            }
            if (activityDC.XAML == null)
            {
                throw new InvalidOperationException("No XAML found for that Name/Version");
            }
            return activityDC;
        }

        #endregion Private helpers
        
    }
}
