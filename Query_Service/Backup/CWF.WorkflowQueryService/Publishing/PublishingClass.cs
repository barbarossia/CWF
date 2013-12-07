//-----------------------------------------------------------------------
// <copyright file="PublishingClass.cs" company="Microsoft">
// Copyright
// Publishing class
// </copyright>
//-----------------------------------------------------------------------
namespace CWF.Publishing
{
    #region References
    using System;
    using System.Activities;
    using System.Activities.XamlIntegration;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xaml;

    using CWF.DataContracts;
    using CWF.WorkflowQueryService;
    using Microsoft.Support.Workflows.Publishing.PublishingInfo;

    #endregion References

    #region Publishing class

    /// <summary>
    /// Class that is used to Publish workflows 
    /// </summary>
    public static class PublishingClass
    {
        #region Private constants
        /// <summary>
        /// Publish workflow missing error message 
        /// </summary>
        private const string NO_PUBLISHING_WF_FOUND = "A publishing workflow is unavailable for this workflow";
        #endregion Private constants

        #region Public methods
        /// <summary>
        /// Workflow publish call
        /// </summary>
        /// <param name="client">Query Service client</param>
        /// <param name="workflowName">Name of the Workflow to Publish</param>
        /// <param name="workflowVersion">Version of the Workflow to Publish</param>
        public static PublishingReply PublishWorkflow(IWorkflowsQueryService client, string workflowName, string workflowVersion)
        {
            // Get the Extension Uri
            string extensionUri = client.GetExtensionUri();
            // Get the publishing XAML
            string publishingXaml = GetPublishingWorkflow(client, workflowName, workflowVersion);

            if (string.IsNullOrEmpty(publishingXaml))
            {
                throw new InvalidOperationException(NO_PUBLISHING_WF_FOUND);
            }

            string xamlFile = Path.GetTempFileName();
            File.WriteAllText(xamlFile, publishingXaml);
            Activity publishWorkflow = ActivityXamlServices.Load(xamlFile);

            var wf = new WorkflowInvoker(publishWorkflow);
            wf.Extensions.Add(new DeployActivity.QueryServiceURIExtension(extensionUri));
            wf.Extensions.Add(DownloadWorkflowInformation(client, workflowName, workflowVersion));

            // create the in arguments
            Dictionary<string, object> inArguments = new Dictionary<string, object>()
            {
                { "ActivityName", workflowName },
                { "ActivityVersion", workflowVersion }
            };

            // Invoke the Publishing workflow
            IDictionary<string, object> outArguments = wf.Invoke(inArguments);
            // Create the Reply
            var publishReply = new PublishingReply();
            // Get the version we published
            if (outArguments.ContainsKey("PublishedVersion"))
            {
                publishReply.PublishedVersion = outArguments["PublishedVersion"].IfNotNull(pv => pv.ToString());
            }
            // Get the location where we published
            if (outArguments.ContainsKey("PublishedLocation"))
            {
                publishReply.PublishedLocation = outArguments["PublishedLocation"].IfNotNull(location => location.ToString());
            }
            // If there are Errors we report them
            if (outArguments.ContainsKey("PublishError"))
            {
                publishReply.PublishErrors = outArguments["PublishError"].IfNotNull(pe => pe.ToString());
            }

            return publishReply;
        }

        #endregion Public methods

        #region Extension methods
        /// <summary>
        /// Eliminate chained nullchecks a la LINQ "Select"
        /// </summary>
        public static TResult IfNotNull<T, TResult>(this T val, Func<T, TResult> select)
                where T : class
        {
            if (val == null)
                return default(TResult);
            else
                return select(val);
        }

        #endregion Extension methods

        #region Private helpers

        /// <summary>
        /// Gets the Publishing Workflow XAML
        /// </summary>
        /// <param name="client">Interface for the Query Service Calls</param>
        /// <param name="workflowName">Name of the workflow to get the Publishing Workflow for</param>
        /// <param name="workflowVersion">Workflow version</param>
        /// <returns>The XAML that matches the Workflow Publishing </returns>
        private static string GetPublishingWorkflow(IWorkflowsQueryService client, string workflowName, string workflowVersion)
        {
            string publishingXaml = null;
            // Get the Store Activities for this workflow
            var activityItem = GetDefinition(client, workflowName, workflowVersion);
            // Gt all of the workflow types
            var workflowTypeGetReply = client.WorkflowTypeGet();
            // Make sure we got them
            if (workflowTypeGetReply.IfNotNull(wfGetType => 0 == wfGetType.StatusReply.Errorcode))
            {
                // Should find the workflow type
                var workflowType = workflowTypeGetReply.WorkflowActivityType.Where(wfAt => activityItem.WorkflowTypeName == wfAt.Name).First();
                // Get the Publishing workflow for the type
                if (null != workflowType && !string.IsNullOrEmpty(workflowType.PublishingWorkflow))
                {
                    var publishWorkflow = client.StoreActivitiesGet(new StoreActivitiesDC()
                    {
                        Id = workflowType.PublishingWorkflowId,
                        Incaller = Environment.UserName,
                        IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString()
                    }).FirstOrDefault();

                    foreach (var dependency in DownloadDependencies(client, ComputeDependencies(client, publishWorkflow)))
                    {
                        var nameVersion = dependency.Key;
                        if (!AppDomain.CurrentDomain.GetAssemblies().Any(asm => asm.GetName().Name == nameVersion.Name && asm.GetName().Version == nameVersion.Version))
                        {
                            var tmp = Path.GetTempFileName();
                            File.WriteAllBytes(tmp, dependency.Value);
                            Assembly.LoadFrom(tmp);
                        }
                    }

                    // Get the XAML
                    publishingXaml = publishWorkflow.IfNotNull(pWf => pWf.Xaml);
                }
            }

            return publishingXaml;
        }


        /// <summary>
        /// Gets the workflow definition from the database by name and version. Stolen from DeployToIIS activity (code smell = duplication).
        /// </summary>
        /// <param name="client">The client to use for the Query Service calls</param>
        /// <param name="activityName">The name of the workflow to get</param>
        /// <param name="activityVersion">The version of the workflow to get</param>
        /// <returns>StoreActivitiesDC</returns>
        private static StoreActivitiesDC GetDefinition(IWorkflowsQueryService client, string activityName, string activityVersion)
        {
            StoreActivitiesDC activityDC = null;
            var workflows = client.StoreActivitiesGet(new StoreActivitiesDC()
            {
                Name = activityName,
                Version = activityVersion,
                Incaller = Environment.UserName,
                IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            });
            
            if (workflows != null && workflows.Any())
                activityDC = workflows.FirstOrDefault();
            else
            {
                throw new InvalidOperationException("No definition found for that Name/Version");
            }

            if (activityDC.StatusReply.Errorcode != 0)
            {
                throw new InvalidOperationException(activityDC.StatusReply.ErrorMessage);
            }

            if (activityDC.Xaml == null)
            {
                throw new InvalidOperationException("No XAML found for that Name/Version");
            }
            return activityDC;
        }

        static PublishingInfoExtension DownloadWorkflowInformation(IWorkflowsQueryService client, string workflowName, string workflowVersion)
        {
            var activity = GetDefinition(
                        client, workflowName, workflowVersion);
            var xaml = activity.Xaml;
            Version version = null;
            Version.TryParse(workflowVersion, out version);
            var dependencies = DownloadDependencies(client, ComputeDependencies(client, activity));
            return new PublishingInfoExtension
            {
                WorkflowName = workflowName,
                WorkflowVersion = version,
                WorkflowType = activity.WorkflowTypeName,
                DependencyNames = dependencies.Keys.ToList(),
                Dependencies = dependencies,
                WorkflowXaml = xaml,
                Tags = activity.MetaTags,
            };
        }

        /// <summary>
        /// Compute dependencies for PublishingInfoExtension to download
        /// </summary>
        static StoreActivityLibrariesDependenciesDC ComputeDependencies(IWorkflowsQueryService client, StoreActivitiesDC activity)
        {
            var dependenciesByID = client.StoreActivityLibraryDependenciesTreeGet(
                new StoreActivityLibrariesDependenciesDC
                {
                    StoreDependenciesRootActiveLibrary =
                    new StoreDependenciesRootActiveLibrary()
                    {
                        ActivityLibraryId = activity.ActivityLibraryId,
                        ActivityLibraryName = activity.ActivityLibraryName,
                        ActivityLibraryVersionNumber = activity.ActivityLibraryVersion
                    },
                    Incaller = "DeployToIIS Activity",
                    IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString()
                });
            return dependenciesByID.Single();
        }

        /// <summary>
        /// Download dependencies for PublishingInfoExtension
        /// </summary>
        /// <param name="client"></param>
        /// <param name="destinationDirectoryPath"></param>
        /// <param name="dependencies"></param>
        static Dictionary<NameVersion, byte[]> DownloadDependencies(IWorkflowsQueryService client, StoreActivityLibrariesDependenciesDC dependencies)
        {
            Dictionary<NameVersion, byte[]> dependencyBytes = new Dictionary<NameVersion, byte[]>();
            foreach (var dependentAssembly in dependencies.StoreDependenciesDependentActiveLibraryList)
            {
                var reply = client.ActivityLibraryGet(new ActivityLibraryDC
                {
                    Incaller = "DeployToIIS Activity",
                    IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    Id = dependentAssembly.ActivityLibraryDependentId
                }).Single();
                Version replyVersion;
                if (!Version.TryParse(reply.VersionNumber, out replyVersion)) // Could be "None"
                {
                    replyVersion = null;
                }
                dependencyBytes[NameVersion.Create(reply.Name, replyVersion)] = reply.Executable;
            }
            return dependencyBytes;
        }

        class PublishingInfoExtension : IPublishingInfo
        {
            public string WorkflowName { get; set; }

            public Version WorkflowVersion { get; set; }

            public string WorkflowType { get; set; }

            public string WorkflowXaml { get; set; }

            public string Tags { get; set; }

            public byte[] DownloadCompiledWorkflow() { return null; }

            public List<NameVersion> DependencyNames { get; set; }

            public Dictionary<NameVersion, byte[]> Dependencies { get; set; }

            public byte[] DownloadDependency(NameVersion dependency)
            {
                return this.Dependencies[dependency];
            }
        }

        #endregion Private helpers
    }

    #endregion Publishing class
}
