//-----------------------------------------------------------------------
// <copyright file="CWF.BAL.cs" company="Microsoft">
// Copyright
// BAL methods
// </copyright>
//-----------------------------------------------------------------------
namespace CWF.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Transactions;
    using Versioning;
    using DataContracts;
    using Microsoft.Support.Workflow.Service.BusinessServices;
    using Microsoft.Support.Workflow.Service.DataAccessServices;

    /// <summary>
    /// All objects and methods for calling BAL methods and accessing objects from the NEW3PrototypeAssetStore DB
    /// </summary>
    public class Services
    {
        private const string ProductionStatusName = "Production"; // This is the string that the StatusCode table will have for items that have been moved to production.

        [Obsolete("only used by test")]
        /// <summary>
        /// DEPRECATED
        /// </summary>
        /// <param name="request">StoreActivityLibrariesDependenciesDC object</param>
        /// <returns>StoreActivityLibrariesDependenciesDC object</returns>
        public static StoreActivityLibrariesDependenciesDC StoreActivityLibraryDependencyList(StoreActivityLibrariesDependenciesDC request)
        {
            var statusReply = new StatusReplyDC();
            //// StoreDependenciesDependentActiveLibrary  dal = null;
            var storeActivityLibrariesDependenciesDC = new StoreActivityLibrariesDependenciesDC();
            storeActivityLibrariesDependenciesDC.StoreDependenciesDependentActiveLibraryList =
                new List<StoreDependenciesDependentActiveLibrary>();
            storeActivityLibrariesDependenciesDC.StoreDependenciesRootActiveLibrary =
                request.StoreDependenciesRootActiveLibrary;
            StoreActivityLibrariesDependenciesDC reply = null;

            using (var transaction = new TransactionScope())
            {
                try
                {
                    if (request == null)
                    {
                        statusReply.Errorcode = SprocValues.REQUEST_OBJECT_IS_NULL_ID;
                        statusReply.ErrorMessage = SprocValues.REQUEST_OBJECT_IS_NULL_MSG;
                        throw new Exception(statusReply.ErrorMessage);
                    }

                    if (request.StoreDependenciesRootActiveLibrary == null)
                    {
                        statusReply.Errorcode =
                            SprocValues.REQUEST_ACTION_STORE_DEPENDENCIES_ROOTACTIVITY_LIBRARY_IS_NULL_ID;
                        statusReply.ErrorMessage =
                            SprocValues.REQUEST_ACTION_STORE_DEPENDENCIES_ROOTACTIVITY_LIBRARY_IS_NULL_MSG;
                        throw new Exception(statusReply.ErrorMessage);
                    }

                    if (request.StoreDependenciesDependentActiveLibraryList == null)
                    {
                        statusReply.Errorcode =
                            SprocValues.REQUEST_ACTION_STORE_DEPENDENCIES_DEPENDENT_ACTIVITY_LIBRARY_IS_NULL_ID;
                        statusReply.ErrorMessage =
                            SprocValues.REQUEST_ACTION_STORE_DEPENDENCIES_DEPENDENT_ACTIVITY_LIBRARY_IS_NULL_MSG;
                        throw new Exception(statusReply.ErrorMessage);
                    }

                    storeActivityLibrariesDependenciesDC.StoreDependenciesRootActiveLibrary =
                        request.StoreDependenciesRootActiveLibrary;
                    //// CreateOrUpdate ActivityLibraryDependencies
                    foreach (var dependentLibrary in request.StoreDependenciesDependentActiveLibraryList)
                    {
                        storeActivityLibrariesDependenciesDC.StoreDependenciesDependentActiveLibraryList.Add(
                            dependentLibrary);
                        storeActivityLibrariesDependenciesDC.Incaller = request.Incaller;
                        storeActivityLibrariesDependenciesDC.IncallerVersion = request.IncallerVersion;
                        storeActivityLibrariesDependenciesDC.InInsertedByUserAlias = request.InInsertedByUserAlias;
                        storeActivityLibrariesDependenciesDC.InUpdatedByUserAlias = request.InUpdatedByUserAlias;
                        reply = ActivityLibraryDependency.StoreActivityLibraryDependenciesCreateOrUpdate(storeActivityLibrariesDependenciesDC);
                        if (reply.StatusReply.Errorcode != 0)
                            throw new Exception(reply.StatusReply.ErrorMessage);
                        storeActivityLibrariesDependenciesDC.StoreDependenciesDependentActiveLibraryList.Remove(dependentLibrary);
                    }
                }
                catch (TransactionAbortedException)
                {
                    //// TODO temp removal of faults tex case
                    //// throw new FaultException(new FaultReason(Convert.ToString(reply.StatusReply.Errorcode) + "|" + reply.StatusReply.ErrorMessage));
                    statusReply.ErrorMessage = reply.StatusReply.ErrorMessage;
                    statusReply.Errorcode = reply.StatusReply.Errorcode;
                }
                catch (Exception)
                {
                    //// TODO temp removal of faults ex case
                    //// throw new FaultException(new FaultReason(Convert.ToString(reply.StatusReply.Errorcode) + "|" + reply.StatusReply.ErrorMessage));
                    statusReply.ErrorMessage = reply.StatusReply.ErrorMessage;
                    statusReply.Errorcode = reply.StatusReply.Errorcode;
                }

                transaction.Complete();
            }

            return reply;
        }

        /// <summary>
        /// Gets the ActivityLibrary and associated StoreActivity entries as a unit. A transaction is used here
        /// to eliminate getting caught in the middle of a multi row update.
        /// </summary>
        /// <param name="request">GetLibraryAndActivitiesDC object</param>
        /// <returns>List<GetLibraryAndActivitiesDC> object</returns>
        public static List<GetLibraryAndActivitiesDC> GetLibraryAndActivities(GetLibraryAndActivitiesDC request)
        {
            var reply = new List<GetLibraryAndActivitiesDC>();
            var statusReply = new StatusReplyDC();
            //// ActivityLibraryDC alDC = request.ActivityLibrary;
            List<ActivityLibraryDC> activityLibraryDClist = null;
            using (var transaction = new TransactionScope())
            {
                try
                {
                    //// Get StoreActivities
                    reply = ActivityRepositoryService.GetActivitiesByActivityLibrary(request, true);
                    if (reply[0].StatusReply.Errorcode != 0)
                        throw new Exception(reply[0].StatusReply.ErrorMessage);
                    //// Get ActivityLibrary
                    activityLibraryDClist = ActivityLibraryRepositoryService.GetActivityLibraries(request.ActivityLibrary, true);
                    if (activityLibraryDClist.Count == 1 && activityLibraryDClist[0].StatusReply.Errorcode == 0)
                        reply[0].ActivityLibrary = activityLibraryDClist[0];
                    else
                        throw new FaultException();
                    transaction.Complete();
                    reply[0].StatusReply = statusReply;
                }
                catch (TransactionAbortedException)
                {
                    statusReply.ErrorMessage = reply[0].StatusReply.ErrorMessage;
                    statusReply.Errorcode = reply[0].StatusReply.Errorcode;
                }
                catch (Exception)
                {
                    statusReply.ErrorMessage = reply[0].StatusReply.ErrorMessage;
                    statusReply.Errorcode = reply[0].StatusReply.Errorcode;
                }
            }

            return reply;
        }

        private static List<StoreActivitiesDC> SetupErrorReply(List<StoreActivitiesDC> reply, StatusReplyDC status)
        {
            reply.Clear();
            reply.Add(new StoreActivitiesDC());
            reply[0].StatusReply = status;
            return reply;
        }

        private static List<TaskActivityDC> SetupErrorReply(List<TaskActivityDC> reply, StatusReplyDC status)
        {
            reply.Clear();
            reply.Add(new TaskActivityDC());
            reply[0].StatusReply = status;
            return reply;
        }

        /// <summary>
        /// Gets the next version number for a workflow.
        /// </summary>
        /// <param name="request">An object describing the workflow for which we need a new version number.</param>
        /// <param name="env">The name of the user making the request.</param>
        /// <returns></returns>
        public static Version GetNextVersion(StoreActivitiesDC request, string env)
        {
            var checkResult = VersionHelper.CheckVersioningRules(request, null, env);
            Version result = Version.Parse(request.Version);

            // Item1 is the boolean entry in the tuple indicating success or failure 
            // If the version number passed in passes the rules, it is OK to use it -- return it unaltered.
            // If not, ask what the next available version number is, and return that, instead.
            if (!checkResult.Item1)
                result = VersionHelper.GetNextVersion(request);

            return result;
        }

        public static List<TaskActivityDC> UploadLibraryAndTaskActivities(StoreLibraryAndTaskActivityRequestDC request)
        {
            using (var scope = new TransactionScope())
            {
                List<TaskActivityDC> reply = SaveStoreLibraryAndTaskActivity(request);
                scope.Complete();
                return reply;
            }
        }


        private static List<TaskActivityDC> SaveStoreLibraryAndTaskActivity(StoreLibraryAndTaskActivityRequestDC request)
        {
            List<TaskActivityDC> reply = new List<TaskActivityDC>();
            reply.Add(new TaskActivityDC());
            StatusReplyDC status = new StatusReplyDC();
            var publishingState = String.Empty;
            Tuple<bool, string> dependencyCheckResult;  // true/false for if the dependencies pass, and a string error message if there are problems
            TaskActivityDC activityDC = new TaskActivityDC()
            {
                Incaller = request.Incaller,
                IncallerVersion = request.IncallerVersion,
                Activity = new StoreActivitiesDC()
                {
                    Incaller = request.Incaller,
                    IncallerVersion = request.IncallerVersion,
                    InAuthGroupNames = request.InAuthGroupNames,
                    Locked = false,
                    LockedBy = null,
                }
            };
            Version nextVersion = null;

            request.ActivityLibrary.Incaller = request.Incaller;
            request.ActivityLibrary.IncallerVersion = request.IncallerVersion;

            //// Check if the library is in production first. If so bail.
            if (IsLibraryInProduction(request.ActivityLibrary))
            {
                status = SetUpReplyStatus(SprocValues.ACTIVITYLIBRARY_MARKED_FOR_PRODUCTION_ID, SprocValues.ACTIVITYLIBRARY_MARKED_FOR_PRODUCTION_MSG, Convert.ToString(Guid.Empty));
                return SetupErrorReply(reply, status);
            }

            if (request.EnforceVersionRules)
            {
                var checkReply = ActivityBusinessService.CheckActivityExists(request.TaskActivitiesList[0].Activity);

                if (checkReply.Errorcode == 0)
                {
                    bool exists = false;

                    if (bool.TryParse(checkReply.Output, out exists) && exists)
                    {
                        nextVersion = GetNextVersion(request.TaskActivitiesList[0].Activity, request.Environment);
                    }
                }

                var saRequest = ConvertStoreLibraryAndTaskActivityRequestDCToStoreLibraryAndActivitiesRequestDC(request);
                dependencyCheckResult = VersionHelper.CheckDependencyRules(saRequest);
                if (!dependencyCheckResult.Item1)
                    throw new VersionException(dependencyCheckResult.Item2, null);

                reply.Clear();
            }
            if (string.IsNullOrEmpty(request.ActivityLibrary.StatusName))
                throw new ValidationException(-1, "'Status Name' for Activity Library records cannot be null.");

            if (request.EnforceVersionRules)
            {
                foreach (var activity in request.TaskActivitiesList)
                {
                    activityDC.Activity.Name = activity.Activity.Name;
                    activityDC.Activity.Version = activity.Activity.OldVersion;
                    activityDC.Activity.Environment = request.Environment;

                    List<StoreActivitiesDC> existingRecords = Activities.StoreActivitiesGetByName(activityDC.Activity.Name, activityDC.Activity.Environment);
                    if (existingRecords.Any())
                    {
                        //Clear the Store Activities lock
                        var lockReply = Activities.StoreActivitiesUpdateLock(activityDC.Activity, DateTime.Now);
                        if (lockReply.StatusReply.Errorcode != 0)
                        {
                            status = SetUpReplyStatus(lockReply.StatusReply.Errorcode, lockReply.StatusReply.ErrorMessage, Convert.ToString(Guid.Empty));
                            return SetupErrorReply(reply, status);
                        }
                    }
                }

                if (nextVersion != null)
                {
                    request.ActivityLibrary.VersionNumber = nextVersion.ToString();
                    request.TaskActivitiesList[0].Activity.Version = nextVersion.ToString();
                    request.StoreActivityLibraryDependenciesGroupsRequestDC.Version = nextVersion.ToString();
                }
            }


            // add the entires
            // Create the ActivityLibrary
            var activityLibraryDCCreate = request.ActivityLibrary;
            activityLibraryDCCreate.Incaller = request.Incaller;
            activityLibraryDCCreate.IncallerVersion = request.IncallerVersion;
            activityLibraryDCCreate.Environment = request.Environment;
            ActivityLibraryDC createALreply = ActivityLibrary.ActivityLibraryCreateOrUpdate(activityLibraryDCCreate);
            if (createALreply.StatusReply.Errorcode != 0)
            {
                status = SetUpReplyStatus(createALreply.StatusReply.Errorcode, createALreply.StatusReply.ErrorMessage, Convert.ToString(Guid.Empty));
                return SetupErrorReply(reply, status);
            }
            // Store the store activities
            foreach (var activity in request.TaskActivitiesList)
            {
                activity.Incaller = request.Incaller;
                activity.IncallerVersion = request.IncallerVersion;
                activity.Activity.Incaller = request.Incaller;
                activity.Activity.IncallerVersion = request.IncallerVersion;
                activity.Activity.ActivityLibraryName = request.ActivityLibrary.Name;
                activity.Activity.ActivityLibraryVersion = request.ActivityLibrary.VersionNumber;
                activity.Activity.AuthGroupName = request.ActivityLibrary.AuthGroupName;
                activity.Activity.Locked = true;
                activity.Activity.LockedBy = request.InUpdatedByUserAlias;
                activity.Environment = request.Environment;
                StoreActivitiesDC createSAreply = null;
                createSAreply = Activities.StoreActivitiesCreateOrUpdate(activity.Activity);
                if (createSAreply.StatusReply.Errorcode != 0)
                {
                    status = SetUpReplyStatus(createSAreply.StatusReply.Errorcode, createSAreply.StatusReply.ErrorMessage, Convert.ToString(Guid.Empty));
                    return SetupErrorReply(reply, status);
                }
                activity.ActivityId = createSAreply.Id;

                TaskActivityDC createTAreply = TaskActivityBusinessService.TaskActivityCreateOrUpdate(activity);
                if (createTAreply.StatusReply.Errorcode != 0)
                {
                    status = SetUpReplyStatus(createTAreply.StatusReply.Errorcode, createTAreply.StatusReply.ErrorMessage, Convert.ToString(Guid.Empty));
                    return SetupErrorReply(reply, status);
                }
                createTAreply.Activity = createSAreply;
                reply.Add(createTAreply);
            }
            // Create the ActivityLibrary dependency list

            // store the 1st entry
            // create the head list
            try
            {
                if (request.StoreActivityLibraryDependenciesGroupsRequestDC != null)
                {
                    if (request.StoreActivityLibraryDependenciesGroupsRequestDC.List != null &&
                        request.StoreActivityLibraryDependenciesGroupsRequestDC.List.Count > 0)
                    {
                        var headList = new ActivityLibraryDependenciesListHeadCreateOrUpdateRequestDC();

                        headList.Name = request.ActivityLibrary.Name;
                        headList.Version = request.ActivityLibrary.VersionNumber;
                        headList.Incaller = request.Incaller;
                        headList.IncallerVersion = request.IncallerVersion;
                        headList.InInsertedByUserAlias = request.InInsertedByUserAlias;
                        headList.InUpdatedByUserAlias = request.InUpdatedByUserAlias;
                        StatusReplyDC replyHeadCreate = ActivityLibraryDependency.ActivityLibraryDependenciesListHeadCreateOrUpdate(headList);

                        if (replyHeadCreate.Errorcode != 0)
                            return SetupErrorReply(reply, status);

                        StatusReplyDC ActivityLibraryDependenciesCreateOrUpdateStatusReply = null;
                        ActivityLibraryDependenciesCreateOrUpdateStatusReply =
                            ActivityLibraryDependenciesCreateOrUpdate(
                                                                        request.StoreActivityLibraryDependenciesGroupsRequestDC.Name,
                                                                        request.StoreActivityLibraryDependenciesGroupsRequestDC.Version,
                                                                        request.StoreActivityLibraryDependenciesGroupsRequestDC.List,
                                                                        request.Incaller,
                                                                        request.IncallerVersion,
                                                                        request.InInsertedByUserAlias,
                                                                        request.InUpdatedByUserAlias
                                                                      );
                        if (ActivityLibraryDependenciesCreateOrUpdateStatusReply.Errorcode != 0)
                            return SetupErrorReply(reply, ActivityLibraryDependenciesCreateOrUpdateStatusReply);
                    }
                }
            }
            catch (TransactionAbortedException tex)
            {
                status = SetUpReplyStatus(SprocValues.GENERIC_CATCH_ID,
                                         "[UploadLibraryAndTaskActivities]" + tex.Message,
                                         Convert.ToString(Guid.Empty));
                return SetupErrorReply(reply, status);
            }
            catch (Exception ex)
            {
                status = SetUpReplyStatus(SprocValues.GENERIC_CATCH_ID,
                                         "[UploadLibraryAndTaskActivities]" + ex.Message,
                                         Convert.ToString(Guid.Empty));
                return SetupErrorReply(reply, status);
            }
            return reply;
        }

        private static StoreLibraryAndActivitiesRequestDC ConvertStoreLibraryAndTaskActivityRequestDCToStoreLibraryAndActivitiesRequestDC(StoreLibraryAndTaskActivityRequestDC request)
        {
            var saRequest = new StoreLibraryAndActivitiesRequestDC();
            saRequest.ActivityLibrary = request.ActivityLibrary;
            saRequest.EnforceVersionRules = request.EnforceVersionRules;
            saRequest.Incaller = request.Incaller;
            saRequest.IncallerVersion = request.IncallerVersion;
            saRequest.InInsertedByUserAlias = request.InInsertedByUserAlias;
            saRequest.InUpdatedByUserAlias = request.InUpdatedByUserAlias;
            saRequest.StoreActivitiesList = new List<StoreActivitiesDC>();
            foreach (var task in request.TaskActivitiesList)
            {
                saRequest.StoreActivitiesList.Add(task.Activity);
            }
            saRequest.StoreActivityLibraryDependenciesGroupsRequestDC = request.StoreActivityLibraryDependenciesGroupsRequestDC;
            return saRequest;
        }

        /// <summary>
        /// Uploads three objects
        /// 1. Activity library
        /// 2. Activities associated with the activity library
        /// 3. Activity Library dependency list
        /// </summary>
        /// <param name="request">StoreLibraryAndActivitiesRequestDC object</param>
        /// <returns>StatusReplyDC object</returns>
        public static List<StoreActivitiesDC> UploadActivityLibraryAndDependentActivities(StoreLibraryAndActivitiesRequestDC request)
        {
            List<StoreActivitiesDC> reply = new List<StoreActivitiesDC>();
            reply.Add(new StoreActivitiesDC());
            StatusReplyDC status = new StatusReplyDC();
            var publishingState = String.Empty;
            Tuple<bool, string> dependencyCheckResult;  // true/false for if the dependencies pass, and a string error message if there are problems
            StoreActivitiesDC activityDC = new StoreActivitiesDC()
            {
                Incaller = request.Incaller,
                IncallerVersion = request.IncallerVersion,
                InAuthGroupNames = request.InAuthGroupNames,
                Locked = false,
                LockedBy = null,
            };
            Version nextVersion = null;

            request.ActivityLibrary.Incaller = request.Incaller;
            request.ActivityLibrary.IncallerVersion = request.IncallerVersion;

            //// Check if the library is in production first. If so bail.
            if (IsLibraryInProduction(request.ActivityLibrary))
            {
                status = SetUpReplyStatus(SprocValues.ACTIVITYLIBRARY_MARKED_FOR_PRODUCTION_ID, SprocValues.ACTIVITYLIBRARY_MARKED_FOR_PRODUCTION_MSG, Convert.ToString(Guid.Empty));
                return SetupErrorReply(reply, status);
            }

            if (request.EnforceVersionRules)
            {
                var checkReply = ActivityBusinessService.CheckActivityExists(request.StoreActivitiesList[0]);

                if (checkReply.Errorcode == 0)
                {
                    bool exists = false;

                    if (bool.TryParse(checkReply.Output, out exists) && exists)
                    {
                        nextVersion = GetNextVersion(request.StoreActivitiesList[0], request.StoreActivitiesList[0].Environment);
                    }
                }

                dependencyCheckResult = VersionHelper.CheckDependencyRules(request);

                if (!dependencyCheckResult.Item1)
                    throw new VersionException(dependencyCheckResult.Item2, null);

                reply.Clear();

            }

            using (var scope = new TransactionScope())
            {

                if (string.IsNullOrEmpty(request.ActivityLibrary.StatusName))
                    throw new ValidationException(-1, "'Status Name' for Activity Library records cannot be null.");

                if (request.EnforceVersionRules)
                {
                    foreach (var activity in request.StoreActivitiesList)
                    {
                        activityDC.Name = activity.Name;
                        activityDC.Version = activity.OldVersion;
                        activityDC.Environment = activity.Environment;
                        List<StoreActivitiesDC> existingRecords = Activities.StoreActivitiesGetByName(activityDC.Name, activityDC.Environment);
                        if (existingRecords.Any())
                        {
                            //Clear the Store Activities lock
                            var lockReply = Activities.StoreActivitiesUpdateLock(activityDC, DateTime.Now);
                            if (lockReply.StatusReply.Errorcode != 0)
                            {
                                status = SetUpReplyStatus(lockReply.StatusReply.Errorcode, lockReply.StatusReply.ErrorMessage, Convert.ToString(Guid.Empty));
                                return SetupErrorReply(reply, status);
                            }
                        }
                    }

                    if (nextVersion != null)
                    {
                        request.ActivityLibrary.VersionNumber = nextVersion.ToString();
                        request.StoreActivitiesList[0].Version = nextVersion.ToString();
                        request.StoreActivityLibraryDependenciesGroupsRequestDC.Version = nextVersion.ToString();
                    }
                }

                // add the entires
                // Create the ActivityLibrary
                var activityLibraryDCCreate = request.ActivityLibrary;
                activityLibraryDCCreate.Incaller = request.Incaller;
                activityLibraryDCCreate.IncallerVersion = request.IncallerVersion;
                ActivityLibraryDC createALreply = ActivityLibrary.ActivityLibraryCreateOrUpdate(activityLibraryDCCreate);
                if (createALreply.StatusReply.Errorcode != 0)
                {
                    status = SetUpReplyStatus(createALreply.StatusReply.Errorcode, createALreply.StatusReply.ErrorMessage, Convert.ToString(Guid.Empty));
                    return SetupErrorReply(reply, status);
                }

                // Store the store activities
                foreach (var activity in request.StoreActivitiesList)
                {
                    activity.Incaller = request.Incaller;
                    activity.IncallerVersion = request.IncallerVersion;
                    activity.ActivityLibraryName = request.ActivityLibrary.Name;
                    activity.ActivityLibraryVersion = request.ActivityLibrary.VersionNumber;
                    activity.AuthGroupName = request.ActivityLibrary.AuthGroupName;
                    activity.Locked = true;
                    activity.LockedBy = request.InUpdatedByUserAlias;
                    StoreActivitiesDC createSAreply = null;
                    createSAreply = Activities.StoreActivitiesCreateOrUpdate(activity);
                    if (createSAreply.StatusReply.Errorcode != 0)
                    {
                        status = SetUpReplyStatus(createSAreply.StatusReply.Errorcode, createSAreply.StatusReply.ErrorMessage, Convert.ToString(Guid.Empty));
                        return SetupErrorReply(reply, status);
                    }
                    reply.Add(createSAreply);
                }

                if (request.TaskActivitiesList != null)
                    //Store TaskActivityDC
                    foreach (var ta in request.TaskActivitiesList)
                    {
                        ta.Incaller = request.Incaller;
                        ta.IncallerVersion = request.IncallerVersion;
                        ta.Environment = request.StoreActivitiesList[0].Environment;
                        if (ta.TaskActivitiesList != null && ta.TaskActivitiesList[0] != null)
                        {
                            if (ta.TaskActivitiesList[0].Status == TaskActivityStatus.Unassigned)
                            {
                                ta.TaskActivitiesList[0].Incaller = request.Incaller;
                                ta.TaskActivitiesList[0].IncallerVersion = request.IncallerVersion;
                                ta.TaskActivitiesList[0].Environment = request.StoreActivitiesList[0].Environment;
                                TaskActivityRepositoryService.TaskActivity_SetStatus(ta.TaskActivitiesList[0]);
                            }
                            else
                            {
                                var statusReply = ActivityRepositoryService.CheckActivityExists(ta.TaskActivitiesList[0].Activity);
                                if (statusReply.Output != Convert.ToString(true))
                                {
                                    ta.EnforceVersionRules = true;
                                    List<TaskActivityDC> taReply = SaveStoreLibraryAndTaskActivity(ta);
                                    if (taReply != null && taReply[0] != null)
                                        if (taReply[0].StatusReply.Errorcode != 0)
                                        {
                                            status = SetUpReplyStatus(taReply[0].StatusReply.Errorcode, taReply[0].StatusReply.ErrorMessage, Convert.ToString(Guid.Empty));
                                            return SetupErrorReply(reply, status);
                                        }
                                }
                            }

                        }
                    }

                // Create the ActivityLibrary dependency list
                // store the 1st entry
                // create the head list
                try
                {
                    if (request.StoreActivityLibraryDependenciesGroupsRequestDC != null)
                    {
                        if (request.StoreActivityLibraryDependenciesGroupsRequestDC.List != null &&
                            request.StoreActivityLibraryDependenciesGroupsRequestDC.List.Count > 0)
                        {
                            var headList = new ActivityLibraryDependenciesListHeadCreateOrUpdateRequestDC();

                            headList.Name = request.ActivityLibrary.Name;
                            headList.Version = request.ActivityLibrary.VersionNumber;
                            headList.Incaller = request.Incaller;
                            headList.IncallerVersion = request.IncallerVersion;
                            headList.InInsertedByUserAlias = request.InInsertedByUserAlias;
                            headList.InUpdatedByUserAlias = request.InUpdatedByUserAlias;
                            StatusReplyDC replyHeadCreate = ActivityLibraryDependency.ActivityLibraryDependenciesListHeadCreateOrUpdate(headList);

                            if (replyHeadCreate.Errorcode != 0)
                                return SetupErrorReply(reply, status);

                            StatusReplyDC ActivityLibraryDependenciesCreateOrUpdateStatusReply = null;
                            ActivityLibraryDependenciesCreateOrUpdateStatusReply =
                                ActivityLibraryDependenciesCreateOrUpdate(
                                                                            request.StoreActivityLibraryDependenciesGroupsRequestDC.Name,
                                                                            request.StoreActivityLibraryDependenciesGroupsRequestDC.Version,
                                                                            request.StoreActivityLibraryDependenciesGroupsRequestDC.List,
                                                                            request.Incaller,
                                                                            request.IncallerVersion,
                                                                            request.InInsertedByUserAlias,
                                                                            request.InUpdatedByUserAlias
                                                                          );
                            if (ActivityLibraryDependenciesCreateOrUpdateStatusReply.Errorcode != 0)
                                return SetupErrorReply(reply, ActivityLibraryDependenciesCreateOrUpdateStatusReply);
                        }
                    }
                }
                catch (TransactionAbortedException tex)
                {
                    status = SetUpReplyStatus(SprocValues.GENERIC_CATCH_ID,
                                             "[UploadActivityLibraryAndDependentActivities]" + tex.Message,
                                             Convert.ToString(Guid.Empty));
                    return SetupErrorReply(reply, status);
                }
                catch (Exception ex)
                {
                    status = SetUpReplyStatus(SprocValues.GENERIC_CATCH_ID,
                                             "[UploadActivityLibraryAndDependentActivities]" + ex.Message,
                                             Convert.ToString(Guid.Empty));
                    return SetupErrorReply(reply, status);
                }

                scope.Complete();
                return reply;
            }
        }

        /// <summary>
        /// ActivityLibraryDependenciesCreateOrUpdate
        /// </summary>
        /// <param name="rootName"></param>
        /// <param name="rootVersion"></param>
        /// <param name="list"></param>
        /// <param name="incaller"></param>
        /// <param name="incallerVersion"></param>
        /// <param name="insertedByUserAlias"></param>
        /// <param name="updatedByUserAlias"></param>
        private static StatusReplyDC ActivityLibraryDependenciesCreateOrUpdate(string rootName,
                                                                               string rootVersion,
                                                                               List<StoreActivityLibraryDependenciesGroupsRequestDC> list,
                                                                               string incaller,
                                                                               string incallerVersion,
                                                                               string insertedByUserAlias,
                                                                               string updatedByUserAlias)
        {
            foreach (StoreActivityLibraryDependenciesGroupsRequestDC l1 in list)
            {
                StoreActivityLibrariesDependenciesDC reply = null;
                var storeActivityLibrariesDependenciesDC = new StoreActivityLibrariesDependenciesDC();
                var storeDependenciesDependentActiveLibrary = new StoreDependenciesDependentActiveLibrary();

                storeActivityLibrariesDependenciesDC.InInsertedByUserAlias = insertedByUserAlias;
                storeActivityLibrariesDependenciesDC.InUpdatedByUserAlias = updatedByUserAlias;
                storeActivityLibrariesDependenciesDC.Incaller = incaller;
                storeActivityLibrariesDependenciesDC.IncallerVersion = incallerVersion;
                storeActivityLibrariesDependenciesDC.StoreDependenciesRootActiveLibrary =
                    new StoreDependenciesRootActiveLibrary();
                storeActivityLibrariesDependenciesDC.StoreDependenciesRootActiveLibrary.ActivityLibraryName = rootName;
                storeActivityLibrariesDependenciesDC.StoreDependenciesRootActiveLibrary.ActivityLibraryVersionNumber =
                    rootVersion;

                storeDependenciesDependentActiveLibrary.ActivityLibraryDependentName = l1.Name;
                storeDependenciesDependentActiveLibrary.ActivityLibraryDependentVersionNumber = l1.Version;
                storeActivityLibrariesDependenciesDC.StoreDependenciesDependentActiveLibraryList =
                    new List<StoreDependenciesDependentActiveLibrary>();
                storeActivityLibrariesDependenciesDC.StoreDependenciesDependentActiveLibraryList.Add(
                    storeDependenciesDependentActiveLibrary);

                reply = ActivityLibraryDependency.StoreActivityLibraryDependenciesCreateOrUpdate(storeActivityLibrariesDependenciesDC);
                if (reply.StatusReply.Errorcode != 0)
                    return reply.StatusReply;
                // Write the dependency table entry
                if (l1.List != null)
                    ActivityLibraryDependenciesCreateOrUpdate(rootName, rootVersion, l1.List, incaller, incallerVersion,
                                                              insertedByUserAlias, updatedByUserAlias);
            }
            return new StatusReplyDC();
        }


        /// <summary>
        /// Sets up the StatusReply object
        /// </summary>
        /// <param name="errorCode">errorCode value</param>
        /// <param name="errorMessager">errorMessager string</param>
        /// <param name="errorGuid">errorGuid object</param>
        /// <returns>StatusReplyDC object</returns>
        public static StatusReplyDC SetUpReplyStatus(int errorCode, string errorMessager, string errorGuid)
        {
            var reply = new StatusReplyDC();
            reply.Errorcode = errorCode;
            reply.ErrorGuid = errorGuid;
            reply.ErrorMessage = errorMessager;
            return reply;
        }


        /// <summary>
        /// check to see if the ActivityLibrary is in production
        /// </summary>
        /// <param name="request">ActivityLibraryDC object</param>
        /// <returns>true if in production else false</returns>
        private static bool IsLibraryInProduction(ActivityLibraryDC request)
        {
            var newRequest = new ActivityLibraryDC();
            newRequest.Name = request.Name;
            newRequest.VersionNumber = request.VersionNumber;
            newRequest.Incaller = request.Incaller;
            newRequest.IncallerVersion = request.IncallerVersion;
            List<ActivityLibraryDC> reply = ActivityLibraryRepositoryService.GetActivityLibraries(newRequest, true);

            var result = (reply.Count == 1)
                && string.Compare(reply[0].StatusName, ProductionStatusName, false) == 0;

            return result;
        }

        /// <summary>
        /// Set lock on StoreActivities
        /// </summary>
        /// <param name="request"></param>
        /// <param name="lockedTime"></param>
        /// <returns></returns>
        public static StatusReplyDC StoreActivitiesUpdateLock(StoreActivitiesDC request, DateTime lockedTime)
        {
            var result = Activities.StoreActivitiesGetByName(request.Name, request.Environment);
            if (result.Any())
            {
                return Activities.StoreActivitiesUpdateLock(request, lockedTime).StatusReply;
            }
            else
            {
                return new StatusReplyDC();
            }
        }

        /// <summary>
        /// Override lock on StoreActivities
        /// </summary>
        /// <param name="request"></param>
        /// <param name="lockedTime"></param>
        /// <returns></returns>
        public static StatusReplyDC StoreActivitiesOverrideLock(StoreActivitiesDC request, DateTime lockedTime)
        {
            var result = Activities.StoreActivitiesGetByName(request.Name, request.Environment);
            if (result.Any())
            {
                return Activities.StoreActivitiesOverrideLock(request, lockedTime).StatusReply;
            }
            else
            {
                return new StatusReplyDC();
            }
        }
    }
}
