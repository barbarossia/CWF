// -----------------------------------------------------------------------
// <copyright file="AuthorizationService.cs" company="Microsoft">
//  Copyright (c) Microsoft Corporation 2012.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Security
{
    using System;
    using System.Security;
    using System.Security.Principal;
    using System.Threading;
    using System.Collections.Generic;
    using System.Configuration;

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
        /// Production authors group.
        /// </summary>
        public const string ProductionAuthorAuthorizationGroupName = "ecocwfauthors_prod";

        public static readonly Dictionary<string, SecurityLevel> SecurityLevelMaps = new Dictionary<string, SecurityLevel>()
        {
            { AdminAuthorizationGroupName, SecurityLevel.Administrator },
            { AuthorAuthorizationGroupName, SecurityLevel.Author },
            { ProductionAuthorAuthorizationGroupName, SecurityLevel.Author }
        };

        private static readonly string firstPriorityRole = ConfigurationManager.AppSettings["FirstPriorityRole"];

        /// <summary>
        /// Get security level of current user
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <returns></returns>
        public static SecurityLevel GetSecurityLevel(WindowsPrincipal currentPrincipal)
        {
            try
            {
                if (currentPrincipal != null)
                {
                    if (!string.IsNullOrEmpty(firstPriorityRole) && PrincipalIsInRoleFunc(firstPriorityRole,currentPrincipal))
                    {
                        return SecurityLevelMaps[firstPriorityRole];
                    }
                    else
                    {
                        foreach (var p in SecurityLevelMaps)
                        {
                            if (PrincipalIsInRoleFunc(p.Key,currentPrincipal))
                            {
                                return p.Value;
                            }
                        }
                    }
                }
                return SecurityLevel.NoAccess;
            }
            catch (AppDomainUnloadedException)
            {
                return SecurityLevel.Offline;
            }
            catch (SecurityException)
            {
                return SecurityLevel.NoAccess;
            }
            catch
            {
                return SecurityLevel.NoAccess;
            }
        }

        /// <summary>
        /// verify if the current  is an administator
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <returns></returns>
        public static bool IsAdministrator(IPrincipal currentPrincipal)
        {
            if (currentPrincipal == null)
            {
                throw new ArgumentNullException("currentPrincipal");
            }

            return GetSecurityLevel(currentPrincipal as WindowsPrincipal) == SecurityLevel.Administrator;
        }

        /// <summary>
        /// Gets the workflow author role name if the current user is in that role.
        /// </summary>
        /// <returns>Workflow author role name.</returns>
        public static string GetWorkflowAuthGroupName()
        {
            string authGroupName = String.Empty;
            IPrincipal principal = CurrentPrincipalFunc();
            if (principal != null)
            {
                if (PrincipalIsInRoleFunc(AuthorAuthorizationGroupName, principal))
                {
                    authGroupName = AuthorAuthorizationGroupName;
                }
                else if (PrincipalIsInRoleFunc(AdminAuthorizationGroupName, principal))
                {
                    authGroupName = AdminAuthorizationGroupName;
                }
                else if (PrincipalIsInRoleFunc(ProductionAuthorAuthorizationGroupName, principal))
                {
                    authGroupName = ProductionAuthorAuthorizationGroupName;
                }
                else if (PrincipalIsInRoleFunc(AdminAuthorizationGroupName, principal))
                {
                    authGroupName = AdminAuthorizationGroupName;
                }
            }
            return authGroupName;
        }

        //user for Automachine test
        public static Func<string, IPrincipal, bool> PrincipalIsInRoleFunc = ((authGroupName, currentPrincipal) =>
        {
            return currentPrincipal.IsInRole(authGroupName);
        });

        //user for Automachine test
        public static Func<IPrincipal> CurrentPrincipalFunc = () =>
        {
            return Thread.CurrentPrincipal;
        };

    }
}
