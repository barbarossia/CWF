using System.Activities.Presentation.Services;
using System.Activities.Statements;
using System.IO;
using Microsoft.Build.Execution;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Enumerable = System.Linq.Enumerable;
using Microsoft.DynamicImplementations;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using System.Diagnostics;
using System.Security;

namespace Microsoft.Support.Workflow.Authoring.Tests.Models
{
    /// <summary>
    ///This is a test class for CompilerTest and is intended
    ///to contain all CompilerTest Unit Tests
    ///</summary>
    [TestClass]
    public class CompilerUnitTests
    {
        private const string RootPath = @"D:\";
        private static CompileProject GetValidCompileProject()
        {
            return TestUtilities.CreateCompileProject("MyWorkflow", new Sequence());
        }

        /// <summary>
        ///A test for CreateProjectFiles
        ///</summary>
        [WorkItem(321155)]
        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        public void aaCompiler_CompileCreateProjectFilesTest()
        {
            string veryLongPath = string.Join("a", Enumerable.Range(0, 500));
            const string path = "test";
            CompileProject project = GetValidCompileProject();
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                         Compiler_Accessor.CreateProjectFiles(project, null));
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                         Compiler_Accessor.CreateProjectFiles(null, path));
            TestUtilities.Assert_ShouldThrow<CompileException>(() =>
                         Compiler_Accessor.CreateProjectFiles(project, veryLongPath));
            Compiler_Accessor.CreateProjectFiles(project, RootPath);
        }


        /// <summary>
        ///A test for GeneratePrivateKeyFile
        ///</summary>
        [WorkItem(321157)]
        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        public void Compiler_GeneratePrivateKeyFileTest()
        {
            string veryLongPath = string.Join("a", Enumerable.Range(0, 500));

            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                         Compiler_Accessor.GeneratePrivateKeyFile(string.Empty));

            TestUtilities.Assert_ShouldThrow<PathTooLongException>(() =>
                         Compiler_Accessor.GeneratePrivateKeyFile(veryLongPath));
        }

        /// <summary>
        ///A test for GenerateXamlFile
        ///</summary>
        [WorkItem(321160)]
        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        public void Compiler_GenerateXamlFileTest()
        {
            const string projectName = "test";
            const string path = "test";
            const string xaml = "test";
            string veryLongPath = string.Join("a", Enumerable.Range(0, 500));

            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                        Compiler_Accessor.GenerateXamlFile(null, path, xaml));
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                        Compiler_Accessor.GenerateXamlFile(projectName, null, xaml));
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                        Compiler_Accessor.GenerateXamlFile(null, null, xaml));
            TestUtilities.Assert_ShouldThrow<PathTooLongException>(() =>
                       Compiler_Accessor.GenerateXamlFile(projectName, veryLongPath, xaml));

        }

        /// <summary>
        ///A test for GenerateAssemblyInfoFile
        ///</summary>
        [WorkItem(321156)]
        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        public void Compiler_GenerateAssemblyInfoFileTest()
        {
            const string projectName = "test";
            const string path = "test";
            const string version = "1.0.0.0";
            const string wrongVersion = "TestVersion";
            string veryLongPath = string.Join("a", Enumerable.Range(0, 500));

            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                        Compiler_Accessor.GenerateAssemblyInfoFile(null, path, version));
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                        Compiler_Accessor.GenerateAssemblyInfoFile(projectName, null, version));
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                        Compiler_Accessor.GenerateAssemblyInfoFile(projectName, path, null));
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                        Compiler_Accessor.GenerateAssemblyInfoFile(projectName, path, wrongVersion));
            TestUtilities.Assert_ShouldThrow<PathTooLongException>(() =>
                       Compiler_Accessor.GenerateXamlFile(projectName, veryLongPath, version));

        }

        /// <summary>
        ///A test for GenerateVisualStudioProjectFile
        ///</summary>
        [WorkItem(321158)]
        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        public void aaCompile_GenerateVisualStudioProjectFileTest()
        {
            string veryLongPath = string.Join("a", Enumerable.Range(0, 500));
            const string path = "Test";
            CompileProject project = GetValidCompileProject();

            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                         Compiler_Accessor.GenerateVisualStudioProjectFile(null, path));
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                         Compiler_Accessor.GenerateVisualStudioProjectFile(project, null));
            TestUtilities.Assert_ShouldThrow<PathTooLongException>(() =>
                        Compiler_Accessor.GenerateVisualStudioProjectFile(project, veryLongPath));
        }

        /// <summary>
        ///A test for CreateBuildDirectory
        ///</summary>
        [WorkItem(321154)]
        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        public void Compile_CreateBuildDirectoryTest()
        {
            string path = Compiler_Accessor.CreateBuildDirectory();
            Assert.IsNotNull(path);
            Assert.IsTrue(Directory.Exists(path));

            using (var directory = new ImplementationOfType(typeof(Directory)))
            {
                directory.Register(()=>Directory.CreateDirectory(Argument<string>.Any)).Throw(new PathTooLongException());
                TestUtilities.Assert_ShouldThrow<CompileException>(() =>
                    Compiler_Accessor.CreateBuildDirectory());

                directory.Register(()=>Directory.CreateDirectory(Argument<string>.Any)).Throw(new IOException());
                TestUtilities.Assert_ShouldThrow<CompileException>(() =>
                    Compiler_Accessor.CreateBuildDirectory());

                directory.Register(()=>Directory.CreateDirectory(Argument<string>.Any)).Throw(new UnauthorizedAccessException());
                TestUtilities.Assert_ShouldThrow<CompileException>(() =>
                    Compiler_Accessor.CreateBuildDirectory());
                
                directory.Register(()=>Directory.CreateDirectory(Argument<string>.Any)).Throw(new NotSupportedException());
                TestUtilities.Assert_ShouldThrow<CompileException>(() =>
                    Compiler_Accessor.CreateBuildDirectory());

                directory.Register(()=>Directory.CreateDirectory(Argument<string>.Any)).Throw(new ArgumentException());
                TestUtilities.Assert_ShouldThrow<CompileException>(() =>
                    Compiler_Accessor.CreateBuildDirectory());

                directory.Register(()=>Directory.CreateDirectory(Argument<string>.Any)).Throw(new SecurityException());
                TestUtilities.Assert_ShouldThrow<CompileException>(() =>
                    Compiler_Accessor.CreateBuildDirectory());
            }
        }

        /// <summary>
        ///A test for AddToCaching
        ///</summary>
        [WorkItem(367491)]
        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        public void Compile_AddToCachingTest()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            var index = path.IndexOf("DEV");
            path = path.Substring(0, index);
            path = path + @"DEV\Microsoft.Support.Workflow.Authoring\References\System.Windows.Interactivity.dll";
            Compiler.AddToCaching(path);
            Assert.IsNotNull(path);
            Assert.IsTrue(Caching.ActivityAssemblyItems.Count >0);
        }

        /// <summary>
        ///A test for GetReferencedAssemblies
        ///</summary>
        [WorkItem(321163)]
        [TestMethod()]
        [TestCategory("Unit-NoDif")]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        public void aaCompiler_GetReferencedAssembliesTest()
        {
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                        Compiler_Accessor.GetReferencedAssemblies(null));

            CompileProject project = GetValidCompileProject();
            Compiler_Accessor.GetReferencedAssemblies(project);
            Assert.IsTrue(project.ReferencedAssemblies.Count > 0);
            Assert.IsTrue(project.ReferencedTypes.Count > 0);
        }

        /// <summary>
        ///A test for Compile
        ///</summary>
        [WorkItem(321161)]
        [TestMethod()]
        [TestCategory("Unit-Dif")]
        public void aaCompiler_CompileTest()
        {
            CompileResult result;
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                Compiler_Accessor.Compile(null));
            CompileProject project = GetValidCompileProject();

            result = Compiler_Accessor.Compile(project);
            Assert.IsTrue(result.BuildResultCode == BuildResultCode.Success);

            using (var buildManager = new ImplementationOfType(typeof(BuildManager)))
            {
                buildManager.Register(() => BuildManager.DefaultBuildManager.Build(Argument<BuildParameters>.Any, Argument<BuildRequestData>.Any)).Return(null);
                result = Compiler_Accessor.Compile(project);
                Assert.IsTrue(result.BuildResultCode == BuildResultCode.Failure);

                buildManager.Register(() => BuildManager.DefaultBuildManager.Build(Argument<BuildParameters>.Any, Argument<BuildRequestData>.Any)).
                    Throw(new CompileException("CompileException"));
                result = Compiler_Accessor.Compile(project);
                Assert.IsTrue(result.BuildResultCode == BuildResultCode.Failure);

                buildManager.Register(() => BuildManager.DefaultBuildManager.Build(Argument<BuildParameters>.Any, Argument<BuildRequestData>.Any)).
                    Throw(new AggregateException("AggregateException"));
                result = Compiler_Accessor.Compile(project);
                Assert.IsTrue(result.BuildResultCode == BuildResultCode.Failure);

                buildManager.Register(() => BuildManager.DefaultBuildManager.Build(Argument<BuildParameters>.Any, Argument<BuildRequestData>.Any)).
                    Throw(new ArgumentException("ArgumentException"));
                result = Compiler_Accessor.Compile(project);
                Assert.IsTrue(result.BuildResultCode == BuildResultCode.Failure);

                buildManager.Register(() => BuildManager.DefaultBuildManager.Build(Argument<BuildParameters>.Any, Argument<BuildRequestData>.Any)).
                   Throw(new InvalidOperationException("ArgumentException"));
                result = Compiler_Accessor.Compile(project);
                Assert.IsTrue(result.BuildResultCode == BuildResultCode.Failure);
            }          
        }

        /// <summary>
        /// A test for compileException ctor
        /// </summary>
        [Owner("v-jillhu")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void CompileException_ConstructorTest()
        {
            CompileException target = new CompileException("test error message");
            Assert.AreEqual("test error message", target.Message);
        }

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        /// <summary>
        /// A test for AddToCaching
        /// </summary>
        [Owner("v-jillhu")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Compiler_AddToCachingTest()
        {

            string path = Path.Combine(testContextInstance.DeploymentDirectory, @"TestData\test001.dll");
            using (var assemblicheck = new ImplementationOfType(typeof(AssemblyInspectionService)))
            {
                assemblicheck.Register(() => AssemblyInspectionService.CheckAssemblyPath(Argument<string>.Any)).Execute(() => { });
                using (var caching = new ImplementationOfType(typeof(Caching)))
                {
                    bool status = false;
                    caching.Register(() => Caching.Refresh()).Execute(() => { status = true; });
                    Compiler.AddToCaching(path);
                    Assert.IsTrue(status);
                }
            }
        }

        [TestCleanup]
        public void TestCleanup() { System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); }
    }
}
