using System;
using System.Activities;
using System.Activities.Presentation.Services;
using System.Activities.Statements;
using System.IO;
using AuthoringToolTests.Services;
using Microsoft.Build.Execution;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.VisualBasic.Activities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Activities.Presentation;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Tests.DataAccess;
using CWF.DataContracts;
using System.Linq;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Authoring.Tests.Functional
{
    [TestClass]
    public class WorkflowCompilerFunctionalTests
    {
        private const string TESTOWNER = "v-xtong";
        private const string WORKFLOWVALIDVERSION = "1.0.0.0";
        private const string WORKFLOWWRONGVERSION = "test";
        private const string WORKFLOWINVALIDXMAL = "test";
        private const string WORKFLOWTYPE = "Page";
        private const int MAX_PATH = 260;
        private const int EXTRAPATH = 8; // '\' + ".csproj"

        // workflow name
        private const string WORKFLOWNAMEWITHWHITESPACE = " Workflow name";
        private const string WORKFLOWNAMEWITHCHINESECHARS = "测试";
        private const string WORKFLOWNAMEWITHSPECIALCHARS = "#Aa&b*@$%^()B";

        [TestMethod()]
        [WorkItem(135056)]
        [Owner(TESTOWNER)]
        [Description("Compile a null CompileProject")]
        [TestCategory("Func-NoDif1-Smoke")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void VerifyArgumentNullExceptionWhenCompileProjectIsNull()
        {
            // Arrange
            CompileProject compileProjectObj = null;

            // Set
            CompileResult result = Compiler.Compile(compileProjectObj);

            // Verify
            Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
        }

        [TestMethod]
        [WorkItem(135107)]
        [Owner(TESTOWNER)]
        [Description("Compile a workflow with null name")]
        [TestCategory("Func-NoDif1-Smoke")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void VerifyArgumentNullExceptionWhenNameOfWorkflowIsNull()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();

            // Set
            CompileResult result = Compiler.Compile(new CompileProject(null, workflowItemObj.Version, workflowItemObj.XamlCode));

            // Verify
            Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
        }

        [TestMethod]
        [WorkItem(135109)]
        [Owner(TESTOWNER)]
        [Description("Compile a workflow with empty name")]
        [TestCategory("Func-NoDif1-Smoke")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void VerifyArgumentNullExceptionWhenNameOfWorkflowIsEmpty()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();

            // Set
            CompileResult result = Compiler.Compile(new CompileProject(string.Empty, workflowItemObj.Version, workflowItemObj.XamlCode));

            // Verify
            Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
        }

        [TestMethod]
        [WorkItem(135110)]
        [Owner(TESTOWNER)]
        [Description("Compile a workflow with Asian chars in name")]
        [TestCategory("Func-NoDif1-Smoke")]
        public void VerifyCompileWorkflowWithAsianCharsInName()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();

            // Set
            CompileResult result = Compiler.Compile(new CompileProject(WORKFLOWNAMEWITHCHINESECHARS, workflowItemObj.Version, workflowItemObj.XamlCode));

            // Verify
            Assert.AreEqual(BuildResultCode.Success, result.BuildResultCode);
        }

        [TestMethod]
        [WorkItem(135111)]
        [Owner(TESTOWNER)]
        [Description("Compile a workflow with white spaces in name")]
        [TestCategory("Func-NoDif1-Smoke")]
        public void VerifyCompileWorkflowWithWhiteSpacesInName()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();

            // Set
            CompileResult result = Compiler.Compile(new CompileProject(WORKFLOWNAMEWITHWHITESPACE, workflowItemObj.Version, workflowItemObj.XamlCode));

            // Verify
            Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
        }

        [TestMethod]
        [WorkItem(135113)]
        [Owner(TESTOWNER)]
        [Description("Compile workflow with special chars in name")]
        [TestCategory("Func-NoDif1-Smoke")]
        public void VerifyWorkflowWithSpecialCharsInName()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();

            // Set
            CompileResult result = Compiler.Compile(new CompileProject(WORKFLOWNAMEWITHSPECIALCHARS, workflowItemObj.Version, workflowItemObj.XamlCode));

            // Verify
            Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
        }

        [TestMethod]
        [WorkItem(135115)]
        [Owner(TESTOWNER)]
        [Description("Compile a workflow whose file name path is equal or greater than 260 chars")]
        [TestCategory("Func-NoDif1-Smoke")]
        public void VerifyCompileWorkflowWhenFileNamePathIsEqualOrGreaterThan260Chars()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            int lengthOfDirPath = (Path.GetFullPath(Path.Combine(Path.Combine(@".", "Output"), Guid.NewGuid().ToString()))).Length;

            int lengthOfFilePath = MAX_PATH - lengthOfDirPath - EXTRAPATH;
            string wfName = TestUtilities.GetRandomStringOfLength(lengthOfFilePath);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();

            // Set
            CompileResult result = Compiler.Compile(new CompileProject(wfName, workflowItemObj.Version, workflowItemObj.XamlCode));

            // Verify
            Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
        }


        [TestMethod]
        [WorkItem(135184)]
        [Owner(TESTOWNER)]
        [Description("Compile a workflow whose display name contains 260 chars")]
        [TestCategory("Func-NoDif1-Smoke")]
        public void VerifyCompileWorkflowHas260CharsInDisplayName()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(20);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(500);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();

            // Set
            CompileResult result = Compiler.Compile(new CompileProject(wfName, workflowItemObj.Version, workflowItemObj.XamlCode));

            // Verify
            Assert.AreEqual(BuildResultCode.Success, result.BuildResultCode);
        }


        [WorkItem(135118)]
        [Description("Test the WorkflowCompiler function to compile an activity with long namespace")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod]
        public void VerifyWorkflowCompilerForActivityWithLongNamespace()
        {
            // ARRANGE
            MessageBoxService.ShowFunc = ((msg, caption, btn, img, defaultResult) => { throw new AssertFailedException(msg); });

            // create a workflow with different variables and activities in it
            string wfName = TestUtilities.GetRandomStringOfLength(10);
            Sequence testSequence = new Sequence
            {
                Activities = 
                    {
                        // this CodeActivitLibrary with long namespace takes two in arguments
                        new CodeActivityLibraryaaaaaaaaaaaaaaaaaaaaaaaaaaaaa50aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa100aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa150aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa200aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa240.CodeActivityWithLongNamespace
                        {
                            Number1 = 10,
                            Number2 = 11
                        }
                    }
            };

            //var wf = new WorkflowItem(wfName, wfName, testSequence.ToXaml(), WORKFLOWTYPE);
            //var project = new CompileProject(wf.Name, wf.Version, wf.XamlCode);


            CompileProject project = TestUtilities.CreateCompileProject(wfName, testSequence);
            // ACT
            CompileResult result = Compiler.Compile(project);

            // VERIFY
            // the resulting string after compiling should not be empty and should contain the assembly name
            Assert.AreNotEqual(null, result);

            // VERIFY
            TestUtilities.VerifyTheCompiledAssemblyHelper(result.FileName, wfName, testSequence);
        }

        [TestMethod]
        [WorkItem(135121)]
        [Owner(TESTOWNER)]
        [Description("Compile a workflow which contains only one activity")]
        [TestCategory("Func-NoDif1-Full")]
        public void VerifyCompileAWorkflowWithOneActivity()
        {
            //// Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);

            Sequence sequenceObj = TestUtilities.GenerateASequenceWithOneActivity();

            CompileProject compileProject = TestUtilities.CreateCompileProject(wfName, sequenceObj);
            CompileResult result = Compiler.Compile(compileProject);
            // Verify
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(BuildResultCode.Success, result.BuildResultCode);
            Assert.IsTrue(result.FileName.Contains(wfName));
            TestUtilities.VerifyTheCompiledAssemblyHelper(result.FileName, wfName, sequenceObj);

        }



        [WorkItem(135122)]
        [Description("Test the WorkflowCompiler function for complex workflow")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod]
        public void VerifyWorkflowCompilerForComplexWorkflow()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            var longNamespace = new ActivityAssemblyItem(TestInputs.Assemblies.TestInput_LibraryWithLongNamespace);
            // Manually add XAML reference so that compiled workflow can parse XAML successfully
            longNamespace.ReferencedAssemblies.Add(TestInputs.Assemblies.TestInput_Lib1.GetName());

            using (new CachingIsolator(longNamespace, TestInputs.ActivityAssemblyItems.TestInput_Lib2, TestInputs.ActivityAssemblyItems.TestInput_Lib1))
            {
                MessageBoxService.ShowFunc = ((msg, caption, btn, img, defaultResult) => { throw new AssertFailedException(msg); });
                string wfName = TestUtilities.GetRandomStringOfLength(20);
                string sequenceDisplayName = TestUtilities.GetRandomStringOfLength(10);
                Sequence complexSequence = new Sequence
                        {
                            DisplayName = sequenceDisplayName,

                            Variables = {   
                                        new Variable<string> { Default = "default1", Name = "greeting1" }, 
                                        new Variable<string> { Default = "default2", Name = "greeting2" } 
                                    },

                            Activities = {  
                                        new WriteLine { Text = "Hello1" }, 
                                        new WriteLine { Text = "Hello2" }, 
                                        new WriteLine { Text = new VisualBasicValue<string>("greeting3 + \"World\"")},
                                        new Delay { Duration = TimeSpan.FromSeconds(2) }, 
                                        new Parallel // another parallel
                                        {
                                            Variables = 
                                            { 
                                                new Variable<string> { Default = "Hello4", Name = "greeting4" }, 
                                                new Variable<int> { Default = 9, Name = "greeting5" }, 
                                                //new Variable<CodeActivityLibraryaaaaaaaaaaaaaaaaaaaaaaaaaaaaa50aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa100aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa150aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa200aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa240.CodeActivityWithLongNamespace>() // CodeActivityWithLongNamespace depends on TestInput_Library1.Activity1
                                              //new Variable<CodeActivityLibraryaaaaaaaaaaaaaaaaaaaaaaaaaaaaa50aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa100aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa150aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa200aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa240.CodeActivityWithLongNamespace>()
                                            },                                            
                                        },
                                        new Sequence // another sequence
                                        {                                            
                                            Activities = 
                                            {   
                                                new WriteLine { Text = "Hello5" },                                           
                                            }     
                                        }
                                    }
                        };

              
                CompileProject compileProject = TestUtilities.CreateCompileProject(wfName, complexSequence);

                CompileResult result = Compiler.Compile(compileProject);

                // VERIFY
                // the resulting string after compiling should not be empty and should contain the assembly name
                Assert.AreNotEqual(null, result);
                Assert.AreEqual(BuildResultCode.Success, result.BuildResultCode);
                Assert.IsTrue(result.FileName.Contains(wfName));
                TestUtilities.VerifyTheCompiledAssemblyHelper(result.FileName, wfName, complexSequence);
            }
        }

        [TestMethod]
        [WorkItem(135124)]
        [Owner(TESTOWNER)]
        [Description("Compile a workflow contains mutiple activities")]
        [TestCategory("Func-NoDif1-Full")]
        public void VerifyCompileAWorkflowWithMultipleActivities()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);
            Sequence sequenceObj = TestUtilities.GenerateASequenceWithMutipleActivities();
           // WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, sequenceObj.ToXaml(), string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();
            //CompileProject compileProjectObj = new CompileProject(workflowItemObj.Name, workflowItemObj.Version, workflowItemObj.XamlCode);

            CompileProject compileProjectObj = TestUtilities.CreateCompileProject(wfName, sequenceObj);
            // Set
            CompileResult result = Compiler.Compile(compileProjectObj);

            // Verify
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(BuildResultCode.Success, result.BuildResultCode);
            Assert.IsTrue(result.FileName.Contains(wfName));
            TestUtilities.VerifyTheCompiledAssemblyHelper(result.FileName, wfName, sequenceObj);
        }

        [TestMethod]
        [WorkItem(135125)]
        [Owner(TESTOWNER)]
        [Description("Compile a workflow with null version")]
        [TestCategory("Func-NoDif1-Smoke")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void VerifyArgumentNullExceptionWhenVersionOfWorkflowIsNull()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();
            //CompileProject project = TestUtilities.CreateCompileProject(wfName, xmalcode);
            // Set
            CompileResult result = Compiler.Compile(new CompileProject(workflowItemObj.Name, null, workflowItemObj.XamlCode));

            // Verify
            Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
        }

        [TestMethod]
        [WorkItem(135128)]
        [Owner(TESTOWNER)]
        [Description("Compile a workflow with empty version")]
        [TestCategory("Func-NoDif1-Smoke")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void VerifyArgumentNullExceptionWhenVersionOfWorkflowIsEmpty()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();

            // Set
            CompileResult result = Compiler.Compile(new CompileProject(workflowItemObj.Name, string.Empty, workflowItemObj.XamlCode));

            // Verify
            Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
        }

        [TestMethod]
        [WorkItem(135131)]
        [Owner(TESTOWNER)]
        [Description("Compile a workflow with invalid version")]
        [TestCategory("Func-NoDif1-Smoke")]
        [ExpectedException(typeof(System.ArgumentException))]
        public void VerifyArgumentExceptionWhenVersionOfWorkflowIsInvalid()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();

            // Set
            CompileResult result = Compiler.Compile(new CompileProject(workflowItemObj.Name, WORKFLOWWRONGVERSION, workflowItemObj.XamlCode));

            // Verify
            Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
        }

        [TestMethod]
        [WorkItem(135132)]
        [Owner(TESTOWNER)]
        [Description("Compile a workflow with null xaml")]
        [TestCategory("Func-NoDif1-Smoke")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void VerifyArgumentNullExceptionWhenXamlOfWorkflowIsNull()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();

            // Set
            CompileResult result = Compiler.Compile(new CompileProject(workflowItemObj.Name, workflowItemObj.Version, null));

            // Verify
            Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
        }

        [TestMethod]
        [WorkItem(135134)]
        [Owner(TESTOWNER)]
        [Description("Compile a workflow with empty xaml")]
        [TestCategory("Func-NoDif1-Smoke")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void VerifyArgumentNullExceptionWhenXamlOfWorkflowIsEmpty()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();

            // Set
            CompileResult result = Compiler.Compile(new CompileProject(workflowItemObj.Name, workflowItemObj.Version, string.Empty));
            Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
        }

        [TestMethod]
        [WorkItem(135138)]
        [Owner(TESTOWNER)]
        [Description("Compile a workflow with invalid xaml")]
        [TestCategory("Func-NoDif1-Smoke")]
        public void VerifyCompileWorkflowWhenXamlIsInvalid()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();

            // Set
            CompileResult result = Compiler.Compile(new CompileProject(workflowItemObj.Name, workflowItemObj.Version, WORKFLOWINVALIDXMAL));

            // Verify
            Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
        }

        [TestMethod]
        [WorkItem(135142)]
        [Owner(TESTOWNER)]
        [Description("Compile a workflow with null ModelService")]
        [TestCategory("Func-NoDif1-Smoke")]
        [ExpectedException(typeof(System.ArgumentNullException))]
        [Ignore]
        public void VerifyArgumentNullExceptionWhenItsModelServiceIsNull()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, xmalcode, string.Empty);

            // Set
            CompileResult result = Compiler.Compile(new CompileProject(workflowItemObj.Name, workflowItemObj.Version, workflowItemObj.XamlCode));

            // Verify
            Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
        }


        [TestMethod]
        [WorkItem(135144)]
        [Description("Compile a workflow while there is an unsigned assembly in the requiredAssemblies")]
        [TestCategory("Func-Dif-Full")]
        [Owner("DiffRequired")]
        public void VerifyCompileWorkflowWithUnsignedAssembly()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();
            CompileProject compileProjectObj = new CompileProject(workflowItemObj.Name, workflowItemObj.Version, workflowItemObj.XamlCode);

            // Dynamically implement File.WriteAllBytes method
            using (var impl = new ImplementationOfType(typeof(File)))
            {
                impl.Register(() => File.WriteAllBytes(Argument<string>.Any, Argument<byte[]>.Any)).Execute<string, byte[]>(
                    (string privateKeyPath, byte[] privateKeyBytes) =>
                    {
                    });
                // Set
                CompileResult result = Compiler.Compile(compileProjectObj);
                // Verify
                Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
            }
        }


        /*Failed Test*/
        [TestMethod]
        [WorkItem(135145)]
        [Description("Compile a workflow while one of dependency assembly is not available")]
        [TestCategory("Func-Dif-Smoke")]
        [Ignore]
        public void VerifyCompileWorkflowWhileOneDependencyAssemblyIsMissing()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);

            using (new CachingIsolator(Microsoft.Support.Workflow.Authoring.Tests.TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib2,
                    Microsoft.Support.Workflow.Authoring.Tests.TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1))
            {
                var act1 = new Testinput_Lib1.Activity1();
                WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, act1.ToXaml(), WORKFLOWTYPE);
                //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();
                CompileProject compileProjectObj = new CompileProject(workflowItemObj.Name, workflowItemObj.Version, workflowItemObj.XamlCode);

                // Dynamically implement File.WriteAllText method
                using (var impl = new ImplementationOfType(typeof(File)))
                {
                    impl.Register(() => File.WriteAllText(Argument<string>.Any, Argument<string>.Any)).Execute<string, string>(
                        (string filePath, string contents) =>
                        {
                            using (StreamWriter sw = new StreamWriter(filePath))
                            {
                                if (Path.GetExtension(filePath).Contains(".csproj"))
                                {
                                    string newContents = contents.Replace("TestInput_Lib1.dll", "TestInput_Lib5.dll");
                                    sw.Write(newContents);
                                }
                                else
                                {
                                    sw.Write(contents);
                                }
                            }
                        });

                    // Set
                    CompileResult result = Compiler.Compile(compileProjectObj);

                    // Verify
                    Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
                }
            }
        }


        [TestMethod]
        [WorkItem(135167)]
        [Description("Compile a workflow with exception thrown in creating directory")]
        [TestCategory("Func-Dif-Full")]
        [Owner("DiffRequired")]
        public void VerifyCompileWorkflowThrownExceptionInCreatingDirectory()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();
            CompileProject compileProjectObj = new CompileProject(workflowItemObj.Name, workflowItemObj.Version, workflowItemObj.XamlCode);

            // Dynamically implement Directory.CreateDirectory method
            using (var impl = new ImplementationOfType(typeof(Directory)))
            {
                impl.Register(() => Directory.CreateDirectory(Argument<string>.Any)).Throw(new IOException());

                // Set
                CompileResult result = Compiler.Compile(compileProjectObj);

                // Verify
                Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
            }
        }


        [TestMethod]
        [WorkItem(135150)]
        [Description("Compile a workflow while writing to file thrown exception")]
        [TestCategory("Func-Dif-Smoke")]
        [Owner("DiffRequired")]
        public void VerifyCompileWorkflowThrownExceptionInWriteAllText()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();
            CompileProject compileProjectObj = new CompileProject(workflowItemObj.Name, workflowItemObj.Version, workflowItemObj.XamlCode);

            // Dynamically implement a File.WriteAllText method
            using (var impl = new ImplementationOfType(typeof(File)))
            {
                impl.Register(() => File.WriteAllText(Argument<string>.Any, Argument<string>.Any)).Execute<string, string>(
                    (string path, string contents) =>
                    {
                        throw new IOException(path);
                    });

                // Set
                CompileResult result = Compiler.Compile(compileProjectObj);

                // Verify
                Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
            }
        }


        [TestMethod]
        [WorkItem(135154)]
        [Description("Compile a workflow while build throws InvalidOperationException")]
        [TestCategory("Func-Dif-Full")]
        [Owner("DiffRequired")]
        public void VerifyCompileWorkflowWhileBuildThrowInvalidOperationException()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();
            CompileProject compileProjectObj = new CompileProject(workflowItemObj.Name, workflowItemObj.Version, workflowItemObj.XamlCode);


            // Dynamically implement Build method
            using (var impl = new ImplementationOfType(typeof(BuildManager)))
            {
                impl.Register(() => (BuildManager.DefaultBuildManager).Build(Argument<BuildParameters>.Any, Argument<BuildRequestData>.Any)).Throw(new System.InvalidOperationException());

                // Set
                CompileResult result = Compiler.Compile(compileProjectObj);

                // Verify
                Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
            }
        }


        [TestMethod]
        [WorkItem(135156)]
        [Description("Compile a workflow while build failed")]
        [TestCategory("Func-Dif-Full")]
        [Owner("DiffRequired")]
        public void VerifyCompileWorkflowWhileBuildFailed()
        {
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            string wfName = TestUtilities.GetRandomStringOfLength(15);
            string wfDisplayName = TestUtilities.GetRandomStringOfLength(20);
            string xmalcode = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();

            WorkflowItem workflowItemObj = new WorkflowItem(wfName, wfDisplayName, xmalcode, string.Empty);
            //ModelService modelService = workflowItemObj.WorkflowDesigner.Context.Services.GetService<ModelService>();
            CompileProject compileProjectObj = new CompileProject(workflowItemObj.Name, workflowItemObj.Version, workflowItemObj.XamlCode);

            // Dynamically implement Build method
            using (var impl = new ImplementationOfType(typeof(BuildManager)))
            {
                impl.Register(() => (BuildManager.DefaultBuildManager).Build(Argument<BuildParameters>.Any, Argument<BuildRequestData>.Any)).Return(null);
                // Set
                CompileResult result = Compiler.Compile(compileProjectObj);
                // Verify
                Assert.AreEqual(BuildResultCode.Failure, result.BuildResultCode);
            }
        }

    }
}
