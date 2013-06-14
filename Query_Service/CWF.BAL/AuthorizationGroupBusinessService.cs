using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Service.Contracts;
using Microsoft.Support.Workflow.Service.DataAccessServices;
using CWF.DAL;
using CWF.DataContracts;

namespace Microsoft.Support.Workflow.Service.BusinessServices
{
    public class AuthorizationGroupBusinessService
    {
        /// <summary>
        /// Get AuthorizationGroup
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static AuthorizationGroupGetReplyDC GetAuthorizationGroups(AuthorizationGroupGetRequestDC request)
        {
            AuthorizationGroupGetReplyDC reply = new AuthorizationGroupGetReplyDC();
            try
            {
                reply = AuthorizationGroup.GetAuthorizationGroups(request);
            }
            catch (ValidationException e)
            {
                e.HandleException();
            }
            catch (DataAccessException e)
            {
                e.HandleException();
            }
            return reply;
        }
    }
}
