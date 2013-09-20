using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Services;
using System.IO;
using AuthoringToolTests.Services;
using System.Reflection;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Microsoft.Support.Workflow.Authoring.Tests.Services
{
    [TestClass]
    public class AssemblyInspectionServiceUnitTest
    {
        [WorkItem(323355)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void AssemblyInspection_TestInspect()
        {
            AssemblyInspectionService service = new AssemblyInspectionService();
            bool result;

            try
            {
                service.Inspect(null);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentNullException);
            }

            result = service.Inspect("test.dll");
            Assert.IsFalse(result);
            Assert.IsTrue(service.OperationException is AssemblyInspectionException);

            result = service.Inspect("microsoft.support.workflow.authoring.tests.dll");
            Assert.IsTrue(result);
        }

        [WorkItem(323357)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void AssemblyInspection_TestGetNameFromAssemblyFullName()
        {
            Assert.AreEqual(string.Empty, AssemblyInspectionService.GetNameFromAssemblyFullName(null));

            Assert.AreEqual(TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1.Name,
                AssemblyInspectionService.GetNameFromAssemblyFullName(TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1.FullName));
        }

        [WorkItem(323356)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void AssemblyInspection_TestGetPublicTokenFromAssemblyFullName()
        {
            Assert.AreEqual(new string(TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1.AssemblyName.GetPublicKeyToken().SelectMany(b => b.ToString("x2")).ToArray()),
                AssemblyInspectionService.GetPublicTokenFromAssemblyFullName(TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1.FullName));
        }

        [WorkItem(323358)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        public void AssemblyInspection_TestCheckAssemblyPath()
        {
                string assemblyPath;

                assemblyPath = null;
                AssertCheckAssemblyPathException<ArgumentNullException>(assemblyPath);

                assemblyPath = string.Empty;
                AssertCheckAssemblyPathException<ArgumentNullException>(assemblyPath);

                assemblyPath = "test.exe";
                AssertCheckAssemblyPathException<ArgumentOutOfRangeException>(assemblyPath);

                assemblyPath = "NotExist.dll";
                AssertCheckAssemblyPathException<FileNotFoundException>(assemblyPath);

                using (var assembly = new ImplementationOfType(typeof(Assembly)))
                {
                    assembly.Register(() => Assembly.ReflectionOnlyLoadFrom(Argument<string>.Any))
                        .Throw(new FileLoadException());

                    assemblyPath = TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1.Location;
                    AssertCheckAssemblyPathException<AssemblyInspectionException>(assemblyPath);
                }

                using (new CachingIsolator())
                {
                    Caching.CacheAssembly(new List<ActivityAssemblyItem>() { TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1 });

                    assemblyPath = TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1.Location;
                    AssertCheckAssemblyPathException<AssemblyInspectionException>(assemblyPath);
                }

                using (new CachingIsolator())
                {
                    assemblyPath = TestInputs.TestInputs.Assemblies.TestInput_LibraryA.Location;
                    AssertCheckAssemblyPathException<AssemblyInspectionException>(assemblyPath);
                }
        }

        private void AssertCheckAssemblyPathException<T>(string argument) where T : Exception
        {
            try
            {
                AssemblyInspectionService.CheckAssemblyPath(argument);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is T);
            }
        }

        [TestCleanup]
        public void TestCleanup() { System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); }
    }
}
