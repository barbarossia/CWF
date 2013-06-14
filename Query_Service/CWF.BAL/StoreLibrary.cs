//-----------------------------------------------------------------------
// <copyright file="StoreLibrary.cs" company="Microsoft">
// Copyright
// StoreLibrary class
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;
    using CWF.DataContracts;
    using Microsoft.Support.Workflow;

    /// <summary>
    /// Store library class
    /// </summary>
    public class StoreLibrary 
    {
        /// <summary>
        /// Stores the ActivityLibrary and associated StoreActivity entries as a unit.
        /// </summary>
        /// <param name="request">List of ActivityLibrary and StoreActivity members to remove and add</param>
        /// <returns>either a null List, or a fault</returns>
        public static StoreLibraryAndActivitiesDC StoreLibraryAndActivities(List<StoreLibraryAndActivitiesDC> request)
        {
            StoreLibraryAndActivitiesDC reply = new StoreLibraryAndActivitiesDC();

            StatusReplyDC statusReply = new StatusReplyDC();
            reply.StatusReply = statusReply;

            StoreActivitiesDC removedSALreply = null;
            ActivityLibraryDC removedALreply = null;
            ActivityLibraryDC createALreply = null;
            StoreActivitiesDC createSAreply = null;

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    List<StoreActivitiesDC> currentSAL = request[0].StoreActivitiesList;
                    ActivityLibraryDC currentAL = request[0].ActivityLibrary;
                    //// Check if the library is in production first. If so bail.
                    if (IsLibraryInProduction(request[0].ActivityLibrary))
                    {
                        reply.StatusReply = SetUpReplyStatus(Constants.SprocValues.ACTIVITYLIBRARY_MARKED_FOR_PRODUCTION_ID, Constants.SprocValues.ACTIVITYLIBRARY_MARKED_FOR_PRODUCTION_MSG, Convert.ToString(Guid.Empty));
                        throw new Exception(Constants.SprocValues.ACTIVITYLIBRARY_MARKED_FOR_PRODUCTION_MSG);
                    }
                    //// Check if all StoreActivity entries point to the ActivityLibrary being created. If not bail.
                    ////if (request[0].StoreActivitiesList.Count > 0)
                    ////{
                    ////    if (CheckActivityLibraryAndStoreActivityRelationship(request).Count != 0)
                    ////    {
                    ////        reply.StatusReply = SetUpReplyStatus(Constants.Constants.ACTIVITYLIBRARY_NAME_DOES_NOT_MATCH_STOREACTIVITY_ID, Constants.Constants.ACTIVITYLIBRARY_NAME_DOES_NOT_MATCH_STOREACTIVITY_MSG, Convert.ToString(Guid.Empty));
                    ////        throw (new Exception(Constants.Constants.ACTIVITYLIBRARY_NAME_DOES_NOT_MATCH_STOREACTIVITY_MSG));
                    ////    }
                    ////}
                    //// Remove all activities associated with this library GUID
                    foreach (var storeActivity in currentSAL)
                    {
                        storeActivity.ActivityLibraryName = request[0].ActivityLibrary.Name;
                        storeActivity.ActivityLibraryVersion = request[0].ActivityLibrary.VersionNumber;
                        removedSALreply = DAL.Activities.StoreActivitiesDelete(storeActivity);
                    }
                    //// remove the library
                    removedALreply = DAL.Activities.ActivityLibraryDelete(currentAL);
                    //// Call the DAL and create a library from the requestDC
                    createALreply = DAL.Activities.ActivityLibraryCreateOrUpdate(currentAL);
                    if (createALreply.StatusReply.Errorcode < 0)
                    {
                        reply.StatusReply = SetUpReplyStatus(createALreply.StatusReply.Errorcode, createALreply.StatusReply.ErrorMessage, Convert.ToString(Guid.Empty));
                        throw new Exception(createALreply.StatusReply.ErrorMessage);
                    }
                    //// Call the DAL multiple times exhausting the list of Activities and do an activities create
                    foreach (var activity in currentSAL)
                    {
                        activity.ActivityLibraryName = request[0].ActivityLibrary.Name;
                        activity.ActivityLibraryVersion = request[0].ActivityLibrary.VersionNumber;
                        activity.AuthgroupName = request[0].ActivityLibrary.AuthGroupName;
                        createSAreply = DAL.Activities.StoreActivitiesCreateOrUpdate(activity);
                        if (createSAreply.StatusReply.Errorcode < 0)
                        {
                            reply.StatusReply = SetUpReplyStatus(createALreply.StatusReply.Errorcode, createALreply.StatusReply.ErrorMessage, Convert.ToString(Guid.Empty));
                            throw new Exception(createSAreply.StatusReply.ErrorMessage);
                        }
                    }

                    statusReply.Errorcode = 0;
                    statusReply.ErrorMessage = string.Empty;
                    ts.Complete();
                }
                catch (TransactionAbortedException tex)
                {
                    statusReply.ErrorMessage = tex.Message;
                }
                catch (Exception ex)
                {
                    statusReply.ErrorMessage = ex.Message;
                }
            }

            reply.StatusReply = statusReply;
            return reply;
        }

        #region [ Helpers ]

        #region [ SetupReplyStatus ]

        /// <summary>
        /// Sets up the StatusReply object
        /// </summary>
        /// <param name="errorCode">errorCode value</param>
        /// <param name="errorMessager">errorMessager string</param>
        /// <param name="errorGuid">errorGuid object</param>
        /// <returns>StatusReplyDC object</returns>
        private static StatusReplyDC SetUpReplyStatus(int errorCode, string errorMessager, string errorGuid)
        {
            StatusReplyDC reply = new StatusReplyDC();
            reply.Errorcode = errorCode;
            reply.ErrorGuid = errorGuid;
            reply.ErrorMessage = errorMessager;
            return reply;
        }

        #endregion

        #region [ CheckActivityLibraryAndStoreActivityRelationship ]
        /// <summary>
        /// Insures that each activityStoreEntry.ActivityLibraryName and activityStoreEntry.ActivityLibraryVersion values match 
        /// the ActivityLibrary.Name and ActivityLibrary.VersionNumber
        /// </summary>
        /// <param name="request">List<StoreLibraryAndActivitiesDC></param>
        /// <returns>List<string> of errors</returns>
        private static List<string> CheckActivityLibraryAndStoreActivityRelationship(List<StoreLibraryAndActivitiesDC> request)
        {
            string errorFormat = "ActivityLibraryName/StoreActivities.ActivityLibraryName/({0}/{1}) does not match activityLibraryVersion/activityStoreEntry.ActivityLibraryVersion({2}/{3})";
            string activityLibraryName = request[0].ActivityLibrary.Name;
            string activityLibraryVersion = request[0].ActivityLibrary.VersionNumber;
            List<string> errorList = new List<string>();
            foreach (var activityStoreEntry in request[0].StoreActivitiesList)
            {
                if (!activityStoreEntry.ActivityLibraryName.Equals(activityLibraryName) ||
                   (!activityStoreEntry.ActivityLibraryVersion.Equals(activityLibraryVersion)))
                 errorList.Add(string.Format(errorFormat, activityLibraryName, activityStoreEntry.ActivityLibraryName, activityLibraryVersion, activityStoreEntry.ActivityLibraryVersion));
            }

            return errorList;
        }
        #endregion

        #region [ IsLibraryInProduction ]

        /// <summary>
        /// check to see if the ActivityLibrary is in production
        /// </summary>
        /// <param name="request">ActivityLibraryDC object</param>
        /// <returns>true if in production else false</returns>
        private static bool IsLibraryInProduction(ActivityLibraryDC request)
        {
            ActivityLibraryDC newRequest = new ActivityLibraryDC();
            newRequest.Name = request.Name;
            newRequest.VersionNumber = request.VersionNumber;
            newRequest.Incaller = request.Incaller;
            newRequest.IncallerVersion = request.IncallerVersion;
            List<ActivityLibraryDC> reply = DAL.Activities.ActivityLibraryGet(newRequest);
            if (reply.Count == 1)
            {
                if (reply[0].StatusReply.Errorcode == 0)
                {
                    if (reply[0].Status == Convert.ToInt32(Status.Production))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }
        #endregion

        #endregion
    }
}
