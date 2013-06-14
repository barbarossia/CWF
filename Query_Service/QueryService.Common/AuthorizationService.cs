using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Microsoft.Support.Workflow.Service.Common
{
    /// <summary>
    /// Contains methodos to do authorization and authentication in the application.
    /// </summary>
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
                if ((ConfigurationManager.AppSettings.Keys.Count > 0) && (!string.IsNullOrEmpty(AppSettings.AuthorGroupName)))
                {
                    //Check if the principal for the request is member of the admin group
                    var principal = new WindowsPrincipal(securityContext.WindowsIdentity);
                    if (!principal.IsInRole(AppSettings.AuthorGroupName))
                    {
                        string message = string.Format(
                            AuthMessages.InvalidCredentials,
                            securityContext.WindowsIdentity.Name,
                            AppSettings.AuthorGroupName);
                        var ex = new UnauthorizedAccessException(message);
                        Logging.Log(LoggingValues.InvalidCredentials, System.Diagnostics.EventLogEntryType.Error, null, ex);
                        throw ex;
                    }
                }
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
