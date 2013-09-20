using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.DynamicImplementations;
using System.Linq;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Microsoft.Support.Workflow.Authoring.Tests.AddIns
{
    [TestClass]
    public class AddInCachingUnitTest
    {
        private static Version One = new Version(1, 0, 0, 0);
        private static Version Two = new Version(2, 0, 0, 0);
        private static Version Three = new Version(3, 0, 0, 0);
        private static AssemblyName asmA1 = new AssemblyName() { Name = "A", Version = One };
        private static AssemblyName asmB1 = new AssemblyName() { Name = "B", Version = One };
        private static AssemblyName asmC1 = new AssemblyName() { Name = "C", Version = One };
        private static AssemblyName asmD1 = new AssemblyName() { Name = "D", Version = One };
        private static AssemblyName asmB2 = new AssemblyName() { Name = "B", Version = Two };
        private static AssemblyName asmB3 = new AssemblyName() { Name = "B", Version = Three };

        private static ActivityAssemblyItem A1 = new ActivityAssemblyItem(asmA1) { NotSafeForTypeLoad = false, Location = TestUtilities.GetExecutingAssemblyPath()};
        private static ActivityAssemblyItem B1 = new ActivityAssemblyItem(asmB1) { NotSafeForTypeLoad = false, Location = TestUtilities.GetExecutingAssemblyPath() };
        private static ActivityAssemblyItem C1 = new ActivityAssemblyItem(asmC1) { NotSafeForTypeLoad = false, Location = TestUtilities.GetExecutingAssemblyPath() };
        private static ActivityAssemblyItem D1 = new ActivityAssemblyItem(asmD1) { NotSafeForTypeLoad = false, Location = TestUtilities.GetExecutingAssemblyPath() };
        private static ActivityAssemblyItem B2 = new ActivityAssemblyItem(asmB2) { NotSafeForTypeLoad = false, Location = TestUtilities.GetExecutingAssemblyPath() };
        private static ActivityAssemblyItem B3 = new ActivityAssemblyItem(asmB3) { NotSafeForTypeLoad = false, Location = TestUtilities.GetExecutingAssemblyPath() };

        [TestInitialize()]
        public void Initialize()
        {
            AddInCaching.ActivityAssemblyItems.Clear();
            AddInCaching.Conflict.Clear();
            AddInCaching.Used.Clear();
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void AddInCaching_TestMethod1()
        {
            List<ActivityAssemblyItem> c1 = new List<ActivityAssemblyItem>() { A1, B1, C1, D1 };
            List<ActivityAssemblyItem> c2 = new List<ActivityAssemblyItem>() { A1, B2, C1, D1 };
            List<ActivityAssemblyItem> c3 = new List<ActivityAssemblyItem>() { B1 };

            bool result;
            using (var impl = new ImplementationOfType(typeof(Assembly)))
            {
                impl.Register(() => Assembly.LoadFrom(Argument<string>.Any)).Return(null);

                result = AddInCaching.ImportAssemblies(c1);
                result = AddInCaching.ImportAssemblies(c2);
                result = AddInCaching.ImportAssemblies(c3);
                Assert.IsFalse(result);

                Assert.IsTrue(AddInCaching.ActivityAssemblyItems.Contains(B1));

            }
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void AddInCaching_TestMethod2()
        {
            List<ActivityAssemblyItem> c1 = new List<ActivityAssemblyItem>() { A1, B1, C1, D1 };
            List<ActivityAssemblyItem> c2 = new List<ActivityAssemblyItem>() { A1, B1, B2, C1, D1 };
            List<ActivityAssemblyItem> c3 = new List<ActivityAssemblyItem>() { B1 };

            bool result;
            using (var impl = new ImplementationOfType(typeof(Assembly)))
            {
                impl.Register(() => Assembly.LoadFrom(Argument<string>.Any)).Return(null);

                result = AddInCaching.ImportAssemblies(c1);
                result = AddInCaching.ImportAssemblies(c2);
                result = AddInCaching.ImportAssemblies(c3);
                Assert.IsFalse(result);

                Assert.IsTrue(AddInCaching.ActivityAssemblyItems.Contains(B1));

            }
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void AddInCaching_TestMethod3()
        {
            List<ActivityAssemblyItem> c1 = new List<ActivityAssemblyItem>() { A1, B2, C1, D1 };
            List<ActivityAssemblyItem> c2 = new List<ActivityAssemblyItem>() { A1, B1, B2, C1, D1 };
            List<ActivityAssemblyItem> c3 = new List<ActivityAssemblyItem>() { B2 };

            bool result;
            using (var impl = new ImplementationOfType(typeof(Assembly)))
            {
                impl.Register(() => Assembly.LoadFrom(Argument<string>.Any)).Return(null);

                result = AddInCaching.ImportAssemblies(c1);
                result = AddInCaching.ImportAssemblies(c2);
                result = AddInCaching.ImportAssemblies(c3);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void AddInCaching_TestMethod4()
        {
            List<ActivityAssemblyItem> c1 = new List<ActivityAssemblyItem>() { A1, B2, C1, D1 };
            List<ActivityAssemblyItem> c2 = new List<ActivityAssemblyItem>() { A1, B1, C1, D1 };
            List<ActivityAssemblyItem> c3 = new List<ActivityAssemblyItem>() { B2 };

            bool result;
            using (var impl = new ImplementationOfType(typeof(Assembly)))
            {
                impl.Register(() => Assembly.LoadFrom(Argument<string>.Any)).Return(null);

                result = AddInCaching.ImportAssemblies(c1);
                result = AddInCaching.ImportAssemblies(c2);
                result = AddInCaching.ImportAssemblies(c3);
                Assert.IsFalse(result);

                Assert.IsTrue(AddInCaching.ActivityAssemblyItems.Contains(B2));

            }
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void AddInCaching_TestMethod5()
        {
            List<ActivityAssemblyItem> c1 = new List<ActivityAssemblyItem>() { A1, B1, B2, C1, D1 };
            List<ActivityAssemblyItem> c2 = new List<ActivityAssemblyItem>() { A1, B1, C1, D1 };
            List<ActivityAssemblyItem> c3 = new List<ActivityAssemblyItem>() { B2 };

            bool result;
            using (var impl = new ImplementationOfType(typeof(Assembly)))
            {
                impl.Register(() => Assembly.LoadFrom(Argument<string>.Any)).Return(null);

                result = AddInCaching.ImportAssemblies(c1);
                result = AddInCaching.ImportAssemblies(c2);
                result = AddInCaching.ImportAssemblies(c3);
                Assert.IsFalse(result);

                Assert.IsTrue(AddInCaching.ActivityAssemblyItems.Contains(B2));

            }
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void AddInCaching_TestMethod6()
        {
            List<ActivityAssemblyItem> c1 = new List<ActivityAssemblyItem>() { A1, B1, B2, B3, C1, D1 };

            using (var impl = new ImplementationOfType(typeof(Assembly)))
            {
                impl.Register(() => Assembly.LoadFrom(Argument<string>.Any)).Return(null);

                Assert.IsTrue(AddInCaching.ImportAssemblies(c1));
            }
        }

        [TestMethod]
        [Owner("v-bobo")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddInCaching_TestMethod7()
        {
            AddInCaching.ImportAssemblies(null);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddInCaching_TestMethod8()
        {
            AddInCaching.ImportAssemblies(new List<ActivityAssemblyItem>() { null });
        }

        [TestMethod]
        [Owner("v-toy")]
        [TestCategory("Unit-Dif")]
        public void AddInCaching_ImportAssembliesArgumentNullTest()
        {
            TestUtilities.Assert_ShouldThrow<ArgumentNullException>(() =>
                        AddInCaching.ImportAssemblies(null));
        }

        [TestMethod]
        [Owner("v-toy")]
        [TestCategory("Unit-Dif")]
        public void AddInCaching_ImportAssembliesImportAssembliesNullTest()
        {
            List<ActivityAssemblyItem> activityAssemblyItems = new List<ActivityAssemblyItem>();
            activityAssemblyItems.Add(null);
            TestUtilities.Assert_ShouldThrow<ArgumentNullException>(() =>
                        AddInCaching.ImportAssemblies(activityAssemblyItems));
        }

        [TestCleanup]
        public void TestCleanup() { System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); }
    }
}
