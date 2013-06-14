//-----------------------------------------------------------------------
// <copyright file="GetLibrary.cs" company="Microsoft">
// Copyright
// Get library BAL class controlled by a transaction
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.BAL
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Transactions;
    using CWF.DataContracts;

    /// <summary>
    /// Get library BAL class controlled by a transaction
    /// </summary>
    public class GetLibrary
    {
        /// <summary>
        /// Gets the ActivityLibrary and associated StoreActivity entries as a unit. A transaction is used here
        /// to eliminate getting caught in the middle of a multi row update.
        /// </summary>
        /// <param name="request">GetLibraryAndActivitiesDC</param>
        /// <returns>either List<GetLibraryAndActivitiesDC>, or a fault</returns>
        public static List<GetLibraryAndActivitiesDC> GetLibraryAndActivities(GetLibraryAndActivitiesDC request)
        {
            List<GetLibraryAndActivitiesDC> reply = new List<GetLibraryAndActivitiesDC>();
            StatusReplyDC statusReply = new StatusReplyDC();
            //// ActivityLibraryDC alDC = request.ActivityLibrary;
            List<ActivityLibraryDC> activityLibraryDClist = null;
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    //// Get StoreActivities
                    reply = DAL.Activities.GetActivityLibraryAndStoreActivities(request);
                    if (reply[0].StatusReply.Errorcode != 0)
                        throw new Exception(reply[0].StatusReply.ErrorMessage);
                    //// Get ActivityLibrary
                    activityLibraryDClist = DAL.Activities.ActivityLibraryGet(request.ActivityLibrary);
                    if (activityLibraryDClist.Count == 1 && activityLibraryDClist[0].StatusReply.Errorcode == 0)
                        reply[0].ActivityLibrary = activityLibraryDClist[0];
                    else
                       throw new FaultException();
                    ts.Complete();
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
    }
}
