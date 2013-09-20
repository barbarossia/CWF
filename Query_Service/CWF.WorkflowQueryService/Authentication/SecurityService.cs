using CWF.DataContracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Web;

namespace CWF.WorkflowQueryService.Authentication
{
    public static class SecurityService
    {
        public static string[] GetSecurityIdentifierArray(string[] authorGroups)
        {
            ServiceSecurityContext securityContext = ServiceSecurityContext.Current;
            var principal = new WindowsPrincipal(securityContext.WindowsIdentity);
            return authorGroups.Where(a => principal.IsInRole(a)).ToArray();             
        }
    }
}