// -----------------------------------------------------------------------
// <copyright file="AuthorizationService.cs" company="Microsoft">
//  Copyright (c) Microsoft Corporation 2012.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Security {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using System.Security;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;
    using CWF.DataContracts;
    using Microsoft.Support.Workflow.Authoring.AddIns.Data;
    using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
    using Microsoft.Support.Workflow.Authoring.Services;

    /// <summary>
    /// Contains methodos to do authorization and authentication in the application.
    /// </summary>
    public static class AuthorizationService {
        private static object initLock = new object();
        private static Dictionary<Env, Permission> envPermissionMaps;
        private static object groupLock = new object();
        private static Dictionary<string, List<Principal>> groupCache = new Dictionary<string, List<Principal>>(StringComparer.OrdinalIgnoreCase);
        private static List<PermissionGetReplyDC> permissionList;
        private static bool directoryServiceFailure = false;

        public static Dictionary<Env, Permission> EnvPermissionMaps {
            get {
                lock (initLock) {
                    if (envPermissionMaps == null) {
                        try
                        {
                            envPermissionMaps = new Dictionary<Env, Permission>() {
                                { Env.Dev, Permission.None },
                                { Env.Test, Permission.None },
                                { Env.Stage, Permission.None },
                                { Env.Prod, Permission.None },
                            };
                            permissionList = WorkflowsQueryServiceUtility.UsingClientReturn(client =>
                            {
                                return client.PermissionGetList(new RequestHeader().SetIncaller());
                            }).List;
                            foreach (PermissionGetReplyDC dc in permissionList)
                            {
                                if (Thread.CurrentPrincipal.IsInRole(dc.AuthorGroupName))
                                {
                                    Env env = dc.EnvironmentName.ToEnv();
                                    if (!envPermissionMaps.ContainsKey(env))
                                        envPermissionMaps.Add(env, Permission.None);
                                    envPermissionMaps[env] |= (Permission)dc.Permission;
                                }
                            }
                        }
                        catch (EndpointNotFoundException)
                        {
                            throw new UserFacingException("Failed to connect with QueryService.");
                        }
                        catch (ActionNotSupportedException)
                        {
                        }
                    }
                    return envPermissionMaps;
                }
            }
        }

        /// <summary>
        /// Get users who has specified permission in the environments
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static List<Principal> GetAuthorizedPrincipals(Permission permission, params Env[] envs) {
            lock (initLock) {
                return permissionList
                    .Where(p => envs.Contains(p.EnvironmentName.ToEnv()) && ((Permission)p.Permission).HasFlag(permission))
                    .Select(p => p.AuthorGroupName).Distinct()
                    .SelectMany(g => ListGroupsUsers(g)).Distinct(new PrincipalComparer())
                    .OrderBy(p => p.DisplayName)
                    .ToList();
            }
        }

        /// <summary>
        /// Check if the user has required permission in specified environment
        /// </summary>
        /// <param name="env"></param>
        /// <param name="require"></param>
        /// <returns></returns>
        public static bool Validate(Env env, Permission require) {
            if (!Enum.IsDefined(typeof(Env), env))
                throw new ArgumentException("Env is invalid.");

            if (env == Env.All) {
                return EnvPermissionMaps.Any() && EnvPermissionMaps.All(p => p.Value.HasFlag(require));
            }
            else if (env == Env.Any) {
                return EnvPermissionMaps.Any(p => p.Value.HasFlag(require));
            }
            else {
                return EnvPermissionMaps[env].HasFlag(require);
            }
        }

        public static Env ToEnv(this string env) {
            return (Env)Enum.Parse(typeof(Env), env, true);
        }

        /// <summary>
        /// Get environments which user has specified permission
        /// </summary>
        /// <param name="require"></param>
        /// <returns></returns>
        public static Env[] GetAuthorizedEnvs(Permission require) {
            return EnvPermissionMaps.Where(p => p.Value.HasFlag(require)).Select(p => p.Key).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static bool GroupExists(string groupName) {
            Contract.Requires(!string.IsNullOrWhiteSpace(groupName));

            string domain = ConfigurationManager.AppSettings["domain"];
            using (PrincipalContext insPrincipalContext = new PrincipalContext(ContextType.Domain, domain, GetDomainContainer(domain))) {
                using (GroupPrincipal insGroupPrincipal = GroupPrincipal.FindByIdentity(insPrincipalContext, groupName)) {
                    return insGroupPrincipal != null;
                }
            }
        }

        /// <summary>
        /// Add System.DirectoryServices.AccountManagement as reference
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static List<Principal> ListGroupsUsers(string groupName) {
            Contract.Requires(!string.IsNullOrWhiteSpace(groupName));

            lock (groupLock) {
                if (!groupCache.ContainsKey(groupName)) {
                    //Connecting to Active Directory
                    string domain = ConfigurationManager.AppSettings["domain"];
                    using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, domain, GetDomainContainer(domain))) {
                        return ListGroupsUsers(groupName, principalContext);
                    }
                }
                return groupCache[groupName];
            }
        }

        private static List<Principal> ListGroupsUsers(string groupName, PrincipalContext context) {
            if (groupCache.ContainsKey(groupName))
                return groupCache[groupName];

            List<Principal> principals = new List<Principal>();
            using (GroupPrincipal groupPrincipal = GroupPrincipal.FindByIdentity(context, groupName)) {
                if (groupPrincipal != null) {
                    try {
                        foreach (Principal p in groupPrincipal.Members) {
                            if (p is GroupPrincipal)
                                principals.AddRange(ListGroupsUsers(p.SamAccountName, context));
                            else if (p is UserPrincipal)
                                principals.Add(p);
                        }
                    }
                    catch {
                        Guid guid;
                        if (!directoryServiceFailure && !Guid.TryParse(AppDomain.CurrentDomain.FriendlyName, out guid)) {
                            directoryServiceFailure = true;
                            AddInMessageBoxService.DirectoryServiceFailure();
                        }
                    }
                }
            }
            principals = principals.Distinct(new PrincipalComparer()).ToList();
            groupCache.Add(groupName, principals);
            return principals;
        }

        private static string GetDomainContainer(string domain) {
            return string.Join(",", domain.Split('.').Select(p => string.Format("DC={0}", p)));
        }

        private class PrincipalComparer : IEqualityComparer<Principal> {
            public bool Equals(Principal x, Principal y) {
                return x.SamAccountName.Equals(y.SamAccountName, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(Principal obj) {
                return obj.SamAccountName.GetHashCode();
            }
        }
    }
}
