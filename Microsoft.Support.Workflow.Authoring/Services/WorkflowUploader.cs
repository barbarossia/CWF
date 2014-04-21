using System.Reflection;

namespace Microsoft.Support.Workflow.Authoring.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Windows;
    using Common;
    using Common.ExceptionHandling;
    using Common.Messages;
    using CWF.DataContracts;
    using Microsoft.Support.Workflow.Authoring.AddIns;
    using Microsoft.Support.Workflow.Authoring.AddIns.Data;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
    using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
    using Microsoft.Support.Workflow.Authoring.Security;
    using Models;
    using Service.Contracts.FaultContracts;
    using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

    public static class WorkflowUploader
    {
        /// <summary>
        /// The main method of WorkflowUploader. Upload the given workflow and its dependencies to server using the default WorkflowsQueryService, as a new activityLibrary.
        /// </summary>;
        /// <param name="workflow">WorkflowItem to save</param>
        /// <returns>true if uploaded succeeded, else CommunicationException</returns>
        /// <exception cref="CommunicationException">WCF layer exceptions and error codes from the workflow query service will be wrapped as CommunicationExceptions</exception>
        public static StatusReplyDC Upload(IWorkflowsQueryService proxy, WorkflowItem workflow)
        {
            var assemblyActivitiyItemsUsed = workflow.WorkflowDesigner.DependencyAssemblies;

            // Database requires that we upload dependencies first so the DB is always in a consistent state
            assemblyActivitiyItemsUsed.ToList().ForEach(al => 
                {
                    al.Env = workflow.Env;
                    al.ActivityItems.ToList().ForEach(a => a.Env = workflow.Env);
                });
            Upload(proxy, assemblyActivitiyItemsUsed);

            // Now, upload the actual workflow
            StoreActivitiesDC result = null;
            if (workflow.TaskActivityGuid.HasValue)
            {
                result = proxy.UploadActivityLibraryAndTaskActivities(
                    DataContractTranslator.WorkflowToStoreLibraryAndTaskActivityRequestDC(workflow, assemblyActivitiyItemsUsed)
                )[0].Activity;
            }
            else
            {
                List<TaskAssignment> tasks = workflow.WorkflowDesigner.Tasks;
                Guid[] newTaskIds = tasks.Where(t => t.TaskStatus == TaskActivityStatus.New).Select(t => t.TaskId).ToArray();
                if (newTaskIds.Any())
                {
                    workflow.WorkflowDesigner.SetNewTasksToAssigned(newTaskIds);
                    workflow.SetXamlCode(); // Publication has removed all tasks from workflow
                }
                try
                {
                    result = proxy.UploadActivityLibraryAndDependentActivities(
                        DataContractTranslator.WorkflowToStoreLibraryAndActivitiesRequestDC(workflow, assemblyActivitiyItemsUsed, tasks)
                    )[0];
                }
                catch (Exception)
                {
                    if (newTaskIds.Any())
                        workflow.WorkflowDesigner.RollbackAssignedTasks(newTaskIds);
                }
            }
            if (result.StatusReply.Errorcode != 0)
                return result.StatusReply;

            // No exception = success. Disable the Save button, clean up, return true to report success.
            workflow.FinishTaskAssigned();

            workflow.OriginalName = result.Name;
            workflow.CreateDateTime = result.InsertedDateTime;
            workflow.UpdateDateTime = result.UpdatedDateTime;
            workflow.Version = result.Version;
            workflow.OldVersion = result.Version;
            workflow.IsOpenFromServer = true;
            workflow.IsSavedToServer = true;
            workflow.IsDataDirty = false;

            return result.StatusReply;
        }

        public static bool CheckActivityExist(IWorkflowsQueryService proxy, StoreActivitiesDC activity)
        {
            var workflowList = proxy.StoreActivitiesGetByName(activity);
            if (workflowList.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method checks if Upload method should be executed.
        /// </summary>
        /// <param name="workflow">WorkflowItem to save</param>
        /// <returns>true if pre-condition check passed</returns>
        public static Tuple<bool, StoreActivitiesDC> CheckCanUpload(IWorkflowsQueryService proxy, WorkflowItem workflow)
        {
            StoreActivitiesDC workflowDC = DataContractTranslator.ActivityItemToStoreActivitiyDC(workflow);
            var workflowList = proxy.StoreActivitiesGetByName(workflowDC);

            //a new created workflow with duplicated name to be saved.
            if ((!workflow.IsOpenFromServer || workflow.Name != workflow.OriginalName) && workflowList.Count > 0)
            {
                MessageBoxService.CannotSaveDuplicatedNameWorkflow(workflow.Name);
                return new Tuple<bool, StoreActivitiesDC>(false, null);
            }

            StoreActivitiesDC latestWorkflow = workflowList.FirstOrDefault();
            bool shouldContinue = false;
            StoreActivitiesDC workflowToOpen = null;

            if (latestWorkflow == null || latestWorkflow.InsertedDateTime <= workflowDC.UpdatedDateTime)
            {
                if (AuthorizationService.Validate(workflowDC.Environment.ToEnv(), Permission.OverrideLock)
                    || (latestWorkflow == null)
                    || (workflowDC.LockedBy == Environment.UserName))
                {
                    shouldContinue = true;
                }
                else
                {
                    MessageBoxService.CannotSaveLockedActivity();
                    shouldContinue = false;
                }
            }
            else // there is a new version saved on server after user checked current one out
            {
                if (AuthorizationService.Validate(workflowDC.Environment.ToEnv(), Permission.OverrideLock))
                {
                    if (MessageBoxService.CreateNewActivityOnSaving() == MessageBoxResult.Yes)
                    {
                        shouldContinue = true;
                    }
                }
                else
                {
                    if (MessageBoxService.DownloadNewActivityOnSaving() == MessageBoxResult.Yes)
                    {
                        workflowToOpen = latestWorkflow;
                    }
                }
            }
            return new Tuple<bool, StoreActivitiesDC>(shouldContinue, workflowToOpen);
        }

        /// <summary>
        /// Using an existing proxy, upload any assemblies which are not already present on server as activity libraries
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="proxy"></param>
        public static void Upload(IWorkflowsQueryService proxy, IEnumerable<ActivityAssemblyItem> assemblies)
        {
            List<ActivityLibraryDC> assembliesMissingInStore;

            //If the assemblies list is blank or all the assemblies are built-in, there is no need to check the data store and upload dependencies.
            if (assemblies.All(item => AssemblyInspectionService.AssemblyIsBuiltIn(item.AssemblyName)))
            {
                return;
            }

            try
            {
                var request = new GetMissingActivityLibrariesRequest();

                request.Incaller = Assembly.GetExecutingAssembly().GetName().Name;
                request.IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                request.ActivityLibrariesList = new List<ActivityLibraryDC>();

                foreach (var assemblyItem in assemblies.Where(item => !AssemblyInspectionService.AssemblyIsBuiltIn(item.AssemblyName)))
                {
                    request.ActivityLibrariesList.Add(DataContractTranslator.AssemblyItemToActivityLibraryDataContract(assemblyItem));
                }

                var reply = proxy.GetMissingActivityLibraries(request.SetIncaller());
                assembliesMissingInStore = reply.MissingActivityLibraries;
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

            //Upload anything not already on the server
            var assembliesToUpload = assemblies.Where(assembly => assembliesMissingInStore.Exists(
                serverItem => serverItem.Name == assembly.Name
                              && serverItem.VersionNumber == assembly.Version.ToString()))
                .ToList();

            // Iteratively upload assemblies with no un-uploaded dependencies until there are no more
            while (assembliesToUpload.Any())
            {
                var assembliesWithNoDependencies = from assembly in assembliesToUpload
                                                   where assembly.ReferencedAssemblies == null
                                                         || !assembly.ReferencedAssemblies.Any(
                                                             dependency =>
                                                             {
                                                                 // if the dependency is in assembliesToUpload
                                                                 // we can't upload assembly yet
                                                                 bool dependencyNotUploadedYet = assembliesToUpload.Any(assembly1 => assembly1.AssemblyName.FullName == dependency.FullName);
                                                                 return dependencyNotUploadedYet;
                                                             })
                                                   select assembly;

                foreach (var assembly in assembliesWithNoDependencies)
                {
                    // Get the dependencies for this particular assembly, by filtering based on ReferencedAssemblies
                    var referencedNames = assembly.ReferencedAssemblies.IfNotNull(refs => refs.Select(assembly1 => assembly1.FullName));
                    var dependencies = from cached in Caching.ActivityAssemblyItems
                                       where referencedNames.IfNotNull(reference => reference.Contains(cached.AssemblyName.FullName))
                                       select cached;

                    // Upload it to the server
                    var request = assembly.ToStoreLibraryAndActivitiesRequestDC(dependencies);
                    request.EnforceVersionRules = false;

                    request.StoreActivitiesList.ForEach(item => item.StatusCodeName = MarketplaceStatus.Public.ToString());
                    request.ActivityLibrary.StatusName = MarketplaceStatus.Public.ToString();
                    var result = proxy.UploadActivityLibraryAndDependentActivities(request);
                    result[0].StatusReply.CheckErrors();
                }

                // assembliesToUpload -= assemblies that were just uploaded
                assembliesToUpload = assembliesToUpload.Except(assembliesWithNoDependencies).ToList();
                if (!assembliesWithNoDependencies.Any())
                {
                    throw new DevFacingException(TextResources.CyclicDependenciesMsg);
                }
            }
        }
    }
}
