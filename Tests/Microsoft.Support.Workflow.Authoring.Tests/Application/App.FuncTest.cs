using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AuthoringToolTests.Services;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring;
using System.IO;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Security.Principal;
using System.Threading;
using Microsoft.Support.Workflow.Authoring.Security;

namespace Authoring.Tests.Functional
{
    [TestClass]
    public class AppFunctionalTests
    {
        [Description("Verify that Application_Startup empties the Assemblies cache when config flag is set")]
        [Owner("waiip")]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        [Ignore]
        public void VerifyDeleteCacheOnStartupWouldDeleteEntireCache()
        {
            using (new CachingIsolator())
            {
                // ARRANGE
                Assembly testAssembly = Assembly.GetExecutingAssembly();

                // get the "cache" assembly folder and put a test.dll in there
                string assemblyFolder = Utility.GetAssembliesDirectoryPath();
                string testFile = assemblyFolder + "\\" + "test.dll";
                using (var file = File.Create(testFile))
                {
                    Assert.IsTrue(File.Exists(testFile));
                }

                // ACT
                //App.ApplicationInitialize();

                // ASSERT
                Assert.IsFalse(File.Exists(testFile), testFile + " should have been deleted.");
            }
        }

        [WorkItem(321166)]
        [Description("Verify that Application_Startup validates user authentication")]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void ValidateUserAuthorizationOnAppStartup()

        {
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            var currentPrincipal = Thread.CurrentPrincipal as WindowsPrincipal;
            bool bl = currentPrincipal.IsInRole("pqocwfauthors");
          
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.NoPrincipal);
            currentPrincipal = Thread.CurrentPrincipal as WindowsPrincipal;
            bool bl2 = currentPrincipal.IsInRole("pqocwfauthors");
         
            System.Security.Principal.WindowsPrincipal wp =Thread.CurrentPrincipal as System.Security.Principal.WindowsPrincipal;
            wp.IsInRole(WindowsBuiltInRole.Guest);
            Assert.AreEqual(AuthorizationService.EnvPermissionMaps.Count, 4);
           
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.UnauthenticatedPrincipal);
            currentPrincipal = Thread.CurrentPrincipal as WindowsPrincipal; ;
            bool bl3 = currentPrincipal.IsInRole("pqocwfauthors");
            Assert.AreEqual(AuthorizationService.EnvPermissionMaps.Count, 4);
        }

        [Description("This method verifies that the environment variable are set properly for DIF")]
        [Owner("v-toy")]
        [WorkItem(299851)]
        [TestCategory("Func-Dif-Environment")]
        [TestMethod]
        [Ignore]
        public void checkEnvVariablesForDIF()
        {
            Assert.AreEqual("1", Environment.GetEnvironmentVariable("COR_ENABLE_PROFILING"), true);
            Assert.AreEqual("{6243DBC9-21F1-406a-B319-AD4937778564}", Environment.GetEnvironmentVariable("COR_PROFILER"), true);
            Assert.AreEqual("EnableV2Profiler", Environment.GetEnvironmentVariable("COMPLUS_ProfAPI_ProfilerCompatibilitySetting"), true);
            Assert.AreEqual("qtagent32.exe", Environment.GetEnvironmentVariable("DIF_PROFILER_FILTER"), true);
            Assert.AreEqual("qtagent32", System.Diagnostics.Process.GetCurrentProcess().ProcessName, true);
        }
    }
}
