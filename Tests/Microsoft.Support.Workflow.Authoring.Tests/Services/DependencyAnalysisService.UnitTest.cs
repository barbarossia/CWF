using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activities;
using AuthoringToolTests.Services;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using System.Threading;

namespace Authoring.Tests.Unit
{
    [TestClass]
    public class DependencyAnalysisServiceUnitTests
    {
        private const string TEST_INPUT_LIB1 = "TestInput_Lib1";
        private const string TEST_INPUT_LIB2 = "TestInput_Lib2";
        private const string TEST_INPUT_LIB3 = "TestInput_Lib3";

        /// <summary>
        ///A test for AddReferencesInApplicationDomain
        ///</summary>
        [TestMethod()]
        [WorkItem(321130)]
        [TestCategory("Unit-NoDif")]
        public void DependencyAnalysis_AddReferencesInApplicationDomainTest()
        {
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                        DependencyAnalysisService_Accessor.AddReferencesInApplicationDomain(null));
        }

        /// <summary>
        ///A test for GetBaseAndArgumentTypes
        ///</summary>
        [WorkItem(321162)]
        [TestMethod()]
        [TestCategory("Unit-NoDif")]
        public void DependencyAnalysis_GetBaseAndArgumentTypesTest()
        {
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                      DependencyAnalysisService_Accessor.GetBaseAndArgumentTypes(null));
        }

        /// <summary>
        /// Unit Test for ComputeDependencies
        ///</summary>
        [TestMethod()]
        [Description("Unit test for Computing Dependencies for a workflow.")]
        [Owner("v-mimcin")]
        [TestCategory("Unit")]
        public void aaaDependencyAnalysis_VerifyComputeDependenciesTest()
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            // Make sure Caching shows our assemblies in the cache               
            using (new CachingIsolator(
                TestInputs.ActivityAssemblyItems.TestInput_Lib1,
                TestInputs.ActivityAssemblyItems.TestInput_Lib2,
                TestInputs.ActivityAssemblyItems.TestInput_Lib3))
            {
                // Create a new workflow from Activity3 and compute it's dependencies
                var wfToUpload = new Testinput_Lib3.Activity1();
                var wfWorkflowDesinger = new WorkflowDesigner();
                var safeImports = new List<ActivityAssemblyItem>
                {
                    TestInputs.ActivityAssemblyItems.TestInput_Lib1,
                    TestInputs.ActivityAssemblyItems.TestInput_Lib2,
                    TestInputs.ActivityAssemblyItems.TestInput_Lib3
                };

                wfWorkflowDesinger.Context.Items.SetValue(new AssemblyContextControlItem()
                {
                    ReferencedAssemblyNames =
                        new[] { 
                            typeof(int).Assembly.GetName(), // mscorlib
                            typeof(Uri).Assembly.GetName(), // System
                            typeof(Activity).Assembly.GetName(), // System.Activities
                            typeof(System.ServiceModel.BasicHttpBinding).Assembly.GetName(), // System.ServiceModel 
                            typeof(CorrelationHandle).Assembly.GetName(), // System.ServiceModel.Activities
                        }
                        .Union
                        (safeImports.Select(c => c.AssemblyName)).ToList()
                });

                wfWorkflowDesinger.Load(new ActivityBuilder() { Implementation = wfToUpload });
                
                WorkflowEditorViewModel vm = new WorkflowEditorViewModel(cancellationToken);
                vm.WorkflowDesigner = wfWorkflowDesinger;
                var compileProject = DependencyAnalysisService.GetCompileProject(vm);
                // ASSERT
                Assert.IsNotNull(compileProject);
                Assert.IsTrue(compileProject.ReferencedTypes.Contains(typeof(Testinput_Lib3.Activity1)));
                Assert.IsTrue(compileProject.ReferencedAssemblies.Contains(TestInputs.Assemblies.TestInput_Lib3));
            }
        }
    }
}
