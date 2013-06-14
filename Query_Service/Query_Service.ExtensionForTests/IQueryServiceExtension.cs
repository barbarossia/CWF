//-----------------------------------------------------------------------
// <copyright file="IQueryServiceExtension.cs" company="Microsoft">
// Copyright
// Interface definition for QueryServiceExtension
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using Query_Service.ExtensionForTests.DataProxy;

namespace Query_Service.ExtensionForTests
{
    /// <summary>
    /// These methods call stored procedures that are used to reset table values in the database after a testcase runs, or do a get to verify an insert.
    /// </summary>
    [ServiceContract]
    public interface IQueryServiceExtension
    {
        /// <summary>
        /// Put the soft delete back in so other tests won't be affected
        /// </summary>
        /// <param name="request">UpdateSoftDeleteRequestDC contains the id and table</param>
        /// <returns>StatusReplyDC contains information about the database command execution</returns>
        [OperationContract]
        StatusReplyDC UpdateSoftDelete(UpdateSoftDeleteRequestDC request);

        /// <summary>
        /// HARD deletes row given an id and name and database
        /// Use this method with caution
        /// </summary>
        /// <param name="id">The id of the row to be deleted</param>
        /// <param name="table">The table of the row to be deleted</param>
        /// <param name="database">The database of the row to be deleted. Database has to be in the list of test databases</param>
        /// <returns>StatusReplyDC contains information about the database command execution</returns>
        [OperationContract]
        StatusReplyDC DeleteFromId(int id, string table, string database);

        /// <summary>
        /// Clear the lock for activites with name and version
        /// </summary>
        /// <param name="request">The name&version of activity</param>
        /// <returns>StatusReplyDC contains information about the database command execution</returns>
        [OperationContract]
        StatusReplyDC ClearLock(ClearLockRequestDC request);
    }
}
