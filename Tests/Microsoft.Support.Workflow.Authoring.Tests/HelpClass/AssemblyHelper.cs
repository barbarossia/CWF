using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.AccessControl;
using System.IO;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Collections.ObjectModel;
using CWF.DataContracts;
using System.ServiceModel;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Microsoft.Support.Workflow.Authoring.Tests
{
    public static class AssemblyHelper
    {
        /// <summary>
        /// Set an ACL entry on the specified file for the specified account.
        /// </summary>
        /// <param name="fileName">the file name</param>
        /// <param name="account">domain/username</param>
        /// <param name="rights">FileSystemRights</param>
        /// <param name="controlType">AccessControlType: Allow, Deny</param>
        public static void SetFileUserAccessControl(string fileName, string account,
            FileSystemRights rights, AccessControlType controlType)
        {
            // Get current AccessControl settings.
            FileSecurity fSecurity = File.GetAccessControl(fileName);

            // Add the FileSystemAccessRule to the security settings.
            fSecurity.AddAccessRule(new FileSystemAccessRule(account, rights, controlType));

            // Set new AccessControl settings.
            File.SetAccessControl(fileName, fSecurity);
        }


        /// <summary>
        /// Removes an ACL entry on the specified file for the specified account.
        /// </summary>
        /// <param name="fileName">the file name</param>
        /// <param name="account">domain/username</param>
        /// <param name="rights">FileSystemRights</param>
        /// <param name="controlType">AccessControlType: Allow, Deny</param>
        public static void RemoveFileUserAccessControl(string fileName, string account,
            FileSystemRights rights, AccessControlType controlType)
        {
            // Get current AccessControl settings.
            FileSecurity fSecurity = File.GetAccessControl(fileName);

            // Remove the FileSystemAccessRule from the security settings.
            fSecurity.RemoveAccessRule(new FileSystemAccessRule(account, rights, controlType));

            // Set new AccessControl settings.
            File.SetAccessControl(fileName, fSecurity);
        }


        /// <summary>
        /// Check if the assembly is in local caching by given an assembly name
        /// </summary>
        /// <param name="fileName">teh assembly file path</param>
        /// <returns">true if exist, otherwise false</returns>
        public static bool IsAssemblyInLocalCaching(string assemblyFileName)
        {
            AssemblyName assemblyName;
            try
            {
                assemblyName = AssemblyName.GetAssemblyName(assemblyFileName);
            }
            catch (Exception)
            {
                throw new ArgumentException(string.Format("The file '{0}' is not a .NET assembly", assemblyFileName));
            }

            ActivityAssemblyItem ignore;
            if (Caching.LoadCachedAssembly(assemblyName, out ignore))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the assembly is in local caching by given an assembly name
        /// </summary>
        /// <param name="fileName">teh assembly file path</param>
        /// <returns">true if exist, otherwise false</returns>
        public static bool IsAssemblyInLocalCaching(Assembly assemblyIn)
        {
            if (assemblyIn == null || string.IsNullOrEmpty(assemblyIn.FullName))
            {
                throw new ArgumentNullException(string.Format("The assembly '{0}' is null or empty", assemblyIn.FullName));
            }

            ActivityAssemblyItem ignore;
            if (Caching.LoadCachedAssembly(assemblyIn.GetName(), out ignore))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieve categories from server 
        /// </summary>
        /// <param name="client">IWorkflowsQueryService</param>
        /// <returns">ObservableCollection<string> object</returns>
        public static ObservableCollection<string> GetCategoriesFromServer()
        {          
            ObservableCollection<string> activityCategories = new ObservableCollection<string>();
            using (var client = new WorkflowsQueryServiceClient())
            {
                var request = new ActivityCategoryByNameGetRequestDC
                {
                    Incaller = Assembly.GetExecutingAssembly().GetName().Name,
                    IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    InInsertedByUserAlias = Environment.UserName,
                    InUpdatedByUserAlias = Environment.UserName
                };

                var categories = client.ActivityCategoryGet(request);

                activityCategories.Assign(from category in categories select category.Name);
            }
            return activityCategories;
        }


        /// <summary>
        /// Retrieve a file from Resource and deploy it to testResult out folder 
        /// </summary>
        /// <param name="filename">file name</param>
        /// <returns>string filepath</returns>
        public static string GetAFileFromResourceAndDeployToTest(string filename, byte[] fileContent)
        {
            try
            {
                string currentFolder = Directory.GetCurrentDirectory();
                string filePath = Path.Combine(currentFolder, filename);
                File.WriteAllBytes(filePath, fileContent);
                bool f = File.Exists(filePath);
                return filePath;
            }
            catch (Exception e)
            {
                throw new UserFacingException(e.Message);
            }
        }
    }
}
