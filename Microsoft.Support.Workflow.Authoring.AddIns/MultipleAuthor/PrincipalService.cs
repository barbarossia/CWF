using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System;
using System.Text;
using Microsoft.Support.Workflow.Authoring.Security;

namespace Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor
{
    public static class PrincipalService
    {
        private static object groupLock = new object();
        private static Dictionary<string, List<Principal>> groupCache = new Dictionary<string, List<Principal>>(StringComparer.OrdinalIgnoreCase);

        public static void Init()
        {
            lock (groupLock)
            {
                foreach (string group in AuthorizationService.SecurityLevelMaps.Keys)
                {
                    ListGroupsUsers(group);
                }
            }
        }

        /// <summary>
        /// Add System.DirectoryServices.AccountManagement as reference
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static IEnumerable<Principal> ListGroupsUsers(string groupName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(groupName));

            lock (groupLock)
            {
                if (!groupCache.ContainsKey(groupName))
                {
                    //Connecting to Active Directory
                    string domain = ConfigurationManager.AppSettings["domain"];
                    using (PrincipalContext insPrincipalContext = new PrincipalContext(ContextType.Domain, domain, GetDomainContainer(domain)))
                    {
                        using (GroupPrincipal insGroupPrincipal = GroupPrincipal.FindByIdentity(insPrincipalContext, groupName))
                        {
                            groupCache.Add(groupName, insGroupPrincipal == null ? new List<Principal>() : insGroupPrincipal.Members.ToList());
                        }
                    }
                }
                return groupCache[groupName];
            }
        }

        private static string GetDomainContainer(string domain)
        {
            return string.Join(",", domain.Split('.').Select(p => string.Format("DC={0}", p)));
        }
    }
}
