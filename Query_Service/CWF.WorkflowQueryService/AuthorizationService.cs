using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using CWF.WorkflowQueryService.Resources;
using CWF.WorkflowQueryService.Authentication;
using Microsoft.Support.Workflow.Service.Contracts;

namespace CWF.WorkflowQueryService
{
    public static class AuthorizationService
    {
        /// <summary>
        /// Default administrators group. 
        /// </summary>
        public const string AdminAuthorizationGroupName = "pqocwfadmin";
        /// <summary>
        /// Default authors group.
        /// </summary>
        public const string AuthorAuthorizationGroupName = "pqocwfauthors";

        /// <summary>
        /// Get the security level of the user account.
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <returns></returns>
        public static SecurityLevel GetSecurityLevel()
        {
            try
            {
                //Get config key from the conf file, to obtain the name of the admin  group we validate.
                if ((ConfigurationManager.AppSettings.Keys.Count > 0) && (!string.IsNullOrEmpty(AppSettings.AuthorGroupName)))
                {
                    //Check if the principal for the request is member of the admin group
                    switch (AppSettings.AuthorGroupName)
                    {
                        case AdminAuthorizationGroupName:
                            return SecurityLevel.Administrator;
                        case AuthorAuthorizationGroupName:
                            return SecurityLevel.Author;
                        default:
                            throw new UnauthorizedAccessException(AuthMessages.ConfigurationMissing);
                    }
                }
                else
                    throw new UnauthorizedAccessException(AuthMessages.ConfigurationMissing);
            }
            catch (AppDomainUnloadedException)
            {
                return SecurityLevel.Offline;
            }
            catch
            {
                return SecurityLevel.NoAccess;
            }
        }
    }
}