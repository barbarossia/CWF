using Microsoft.Support.Workflow.Authoring.CDS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Support.Workflow.Authoring.Tests.CDS
{
    /// <summary>
    ///This is a test class for NugetConfigManagerTest and is intended
    ///to contain all NugetConfigManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NugetConfigManagerTest
    {
        [TestInitialize]
        public void Init()
        {
            if (File.Exists(NugetConfigManager_Accessor.configPath))
            {
                File.Delete(NugetConfigManager_Accessor.configPath);
            }
        }

        /// <summary>
        ///A test for Load and Save
        ///</summary>
        [TestMethod()]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void SaveAndLoadTest()
        {
            NugetConfigManager_Accessor.repositories = null;
            CollectionAssert.AreEquivalent(new List<CDSRepository>(), NugetConfigManager.Repositories);

            List<CDSRepository> repos = new List<CDSRepository>()
            {
                new CDSRepository() {
                    IsEnabled = true,
                    Name = "repo",
                    Source = "http://source"
                },
                new CDSRepository() {
                    IsEnabled = false,
                    Name = "repoDisabled",
                    Source = "http://source"
                },
            };
            NugetConfigManager_Accessor.Save(repos);
            Assert.AreEqual(repos.Count, NugetConfigManager_Accessor.Load().Count);
            Assert.AreEqual(repos.Count, NugetConfigManager.Repositories.Count);
            Assert.AreEqual(repos.Where(r => r.IsEnabled).Count(), NugetConfigManager.EnabledRepositories.Count());
        }

        /// <summary>
        ///A test for configPath
        ///</summary>
        [TestMethod()]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void configPathTest()
        {
            string actual;
            actual = NugetConfigManager_Accessor.configPath;
            Assert.AreEqual(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nuget.config"), actual);
        }
    }
}
