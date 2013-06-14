//-----------------------------------------------------------------------
// <copyright file="StoreActivityLibraryDependencies.cs" company="Microsoft">
// Copyright
// StoreActivityLibraryDependencies class
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;
    using CWF.DataContracts;

    /// <summary>
    /// The StoreActivityLibraryDependencies class is used to maintain the activityLibraries dependency list. This list consists of pair
    /// entries in the mtblActivityLibraryDependencies table. The pairs consist of a library FK and a dependent library FK. The list can
    /// be created, the entire tree obtained, and a specific node obtained. The creation of the list is a BAL layer method under the control
    /// of a transaction. The get operations are DAL methods.
    /// </summary>
    public class StoreActivityLibraryDependencies
    {
        /// <summary>
        /// Stores the list of library dependencies
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>StoreActivityLibrariesDependenciesDC with StatusReply</returns>
        public static StoreActivityLibrariesDependenciesDC StoreActivityLibraryDependencyList(StoreActivityLibrariesDependenciesDC request)
        {
            StatusReplyDC statusReply = new StatusReplyDC();
            //// StoreDependenciesDependentActiveLibrary  dal = null;
            StoreActivityLibrariesDependenciesDC storeActivityLibrariesDependenciesDC = new StoreActivityLibrariesDependenciesDC();
            storeActivityLibrariesDependenciesDC.StoreDependenciesDependentActiveLibraryList = new List<StoreDependenciesDependentActiveLibrary>();
            storeActivityLibrariesDependenciesDC.StoreDependenciesRootActiveLibrary = request.StoreDependenciesRootActiveLibrary;
            StoreActivityLibrariesDependenciesDC reply = null;

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    if (request == null)
                    {
                        statusReply.Errorcode = Constants.SprocValues.REQUEST_OBJECT_IS_NULL_ID;
                        statusReply.ErrorMessage = Constants.SprocValues.REQUEST_OBJECT_IS_NULL_MSG;
                        throw new Exception(statusReply.ErrorMessage);
                    }

                    if (request.StoreDependenciesRootActiveLibrary == null)
                    {
                        statusReply.Errorcode = Constants.SprocValues.REQUEST_ACTION_STORE_DEPENDENCIES_ROOTACTIVITY_LIBRARY_IS_NULL_ID;
                        statusReply.ErrorMessage = Constants.SprocValues.REQUEST_ACTION_STORE_DEPENDENCIES_ROOTACTIVITY_LIBRARY_IS_NULL_MSG;
                        throw new Exception(statusReply.ErrorMessage);
                    }

                    if (request.StoreDependenciesDependentActiveLibraryList == null)
                    {
                    statusReply.Errorcode = Constants.SprocValues.REQUEST_ACTION_STORE_DEPENDENCIES_DEPENDENT_ACTIVITY_LIBRARY_IS_NULL_ID;
                        statusReply.ErrorMessage = Constants.SprocValues.REQUEST_ACTION_STORE_DEPENDENCIES_DEPENDENT_ACTIVITY_LIBRARY_IS_NULL_MSG;
                        throw new Exception(statusReply.ErrorMessage);
                    }

                    storeActivityLibrariesDependenciesDC.StoreDependenciesRootActiveLibrary = request.StoreDependenciesRootActiveLibrary;
                    //// CreateOrUpdate ActivityLibraryDependencies
                    foreach (var dependentLibrary in request.StoreDependenciesDependentActiveLibraryList)
                    {
                        storeActivityLibrariesDependenciesDC.StoreDependenciesDependentActiveLibraryList.Add(dependentLibrary);
                        storeActivityLibrariesDependenciesDC.Incaller = request.Incaller;
                        storeActivityLibrariesDependenciesDC.IncallerVersion = request.IncallerVersion;
                        storeActivityLibrariesDependenciesDC.InsertedByUserAlias = request.InsertedByUserAlias;
                        storeActivityLibrariesDependenciesDC.UpdatedByUserAlias = request.UpdatedByUserAlias;
                        reply = DAL.Activities.StoreActivityLibraryDependenciesCreateOrUpdate(storeActivityLibrariesDependenciesDC);
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

                ts.Complete();
            }

            return reply;
        }
    }
}
