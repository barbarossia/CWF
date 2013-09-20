using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;

namespace Authoring.Tests.Services
{
    /// <summary>
    /// Summary description for Utility
    /// </summary>
    [TestClass]
    [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
    public class Utility
    {

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        private TestContext testContextInstance;

        [TestCategory("Full")]
        [TestMethod()]
        [Description("Verify active directory is working to enforce windows forms authentication")]
        [TestCategory("Security")]
        public void VerifyWindowsAuthenticationIsRunning()
        {
            // Create the security Context

            SafeTokenHandle safeTokenHandle;

            string userName, domainName, pwd;
            IntPtr myToken = IntPtr.Zero;
            domainName = "TESTCORP";
            userName = "dummyUser";
            pwd = "Pass123";

            const int LOGON32_PROVIDER_DEFAULT = 0;
            //This parameter causes LogonUser to create a primary token.
            const int LOGON32_LOGON_INTERACTIVE = 2;

            // Call LogonUser to obtain a handle to an access token.
            Assert.IsFalse(LogonUser(userName, domainName, pwd,
                LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                out safeTokenHandle));

            //free resources           
            safeTokenHandle.Close();

        }

        [TestCategory("Full")]
        [TestMethod()]
        [Description("Verify active directory is working to enforce windows forms authentication")]
        [TestCategory("Security")]
        public void VerifyValidateCredentials()
        {
            //Check User Level to grant/deny access to app
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.UnauthenticatedPrincipal);
            var currentPrincipal = System.Threading.Thread.CurrentPrincipal as WindowsPrincipal;


            Assert.AreEqual(Microsoft.Support.Workflow.Authoring.Services.Utility.GetSecurityLevel(currentPrincipal),
            Microsoft.Support.Workflow.Authoring.Common.SecurityLevel.NoAccess, "User not logged in has access to app");

            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.NoPrincipal);
            currentPrincipal = System.Threading.Thread.CurrentPrincipal as WindowsPrincipal;
            Assert.AreEqual(Microsoft.Support.Workflow.Authoring.Services.Utility.GetSecurityLevel(currentPrincipal),
           Microsoft.Support.Workflow.Authoring.Common.SecurityLevel.NoAccess, "User not logged in has access to app");

            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            currentPrincipal = System.Threading.Thread.CurrentPrincipal as WindowsPrincipal;
            if (currentPrincipal.IsInRole("pqocwfauthors"))
            {
                Assert.AreEqual(Microsoft.Support.Workflow.Authoring.Services.Utility.GetSecurityLevel(currentPrincipal),
               Microsoft.Support.Workflow.Authoring.Common.SecurityLevel.Author, "Authenticated user doesnt have access to app");
            }
            else
            {
                Assert.AreNotEqual(Microsoft.Support.Workflow.Authoring.Services.Utility.GetSecurityLevel(currentPrincipal),
              Microsoft.Support.Workflow.Authoring.Common.SecurityLevel.Author, "User with no author rights can access the app");
            }

        }
    }

    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeTokenHandle()
            : base(true)
        {
        }

        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        protected override bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }
    }
}
