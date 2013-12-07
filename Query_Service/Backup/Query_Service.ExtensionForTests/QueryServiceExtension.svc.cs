//-----------------------------------------------------------------------
// <copyright file="QueryServiceExtension.svc.cs" company="Microsoft">
// Copyright
// QueryServiceExtension implementation of iQueryServiceExtension
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
using Query_Service.ExtensionForTests.DAL;

namespace Query_Service.ExtensionForTests
{
    /// <summary>
    /// QueryServiceExtension exposes a DAL providing CRUD access to test stored procedures in PrototypeAssetStoreDBTest
    /// </summary>
    public class QueryServiceExtension : IQueryServiceExtension
    {
        /// <summary>
        /// Put the soft delete back in so other tests won't be affected
        /// </summary>
        /// <param name="request">UpdateSoftDeleteRequestDC contains the id and table</param>
        /// <returns>StatusReplyDC contains information about the database command execution</returns>
        public StatusReplyDC UpdateSoftDelete(DataProxy.UpdateSoftDeleteRequestDC request)
        {
            return DAL.DAL.UpdateSoftDelete(request);
        }

        /// <summary>
        /// Hard deletes row given an id and name and database
        /// </summary>
        /// <param name="id">The id of the row to be deleted</param>
        /// <param name="table">The table of the row to be deleted</param>
        /// <param name="database">The database of the row to be deleted. Database has to be in the list of test databases</param>
        /// <returns>StatusReplyDC contains information about the database command execution</returns>
        public StatusReplyDC DeleteFromId(int id, string table, string database)
        {
            return DAL.DAL.DeleteFromId(id, table, database);
        }

        /// <summary>
        /// Clear the lock for activites with name and version
        /// </summary>
        /// <param name="request">The name&version of activity</param>
        /// <returns>StatusReplyDC contains information about the database command execution</returns>
        public StatusReplyDC ClearLock(ClearLockRequestDC request)
        {
            return DAL.DAL.ClearLock(request);
        }
    }
}
