using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.Support.Workflow.Authoring.Services;
using AuthoringToolTests.Services;
using System.IO;
using Microsoft.Support.Workflow.Authoring.Tests.Services;
using System.Activities;
using Microsoft.Support.Workflow.Authoring.Models;

namespace Authoring.Tests.Unit
{
    /// <summary>
    /// Unit tests for assembly resolution. The actual methods live on the Utility class, 
    /// but the tests need special setup so they get their own test suite class in C#")]
    /// </summary>
    [TestClass]
    public class Utility_AssemblyResolutionUnitTests
    {       
        /// <summary>
        /// Verifies whether an assembly in cache is loaded to an app domain when Utlity.Resolve() is called.
        /// </summary>
        [WorkItem(321736)]
        [Description("Verifies whether an assembly in cache is loaded to an app domain when Utlity.Resolve() is called.")]
        [Owner("v-sanja")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void Utility_AssemblyResolution_LoadCachedAssemblyWhenResolveCalled()
        {
            using (new CachingIsolator(
                 TestInputs.ActivityAssemblyItems.TestInput_Lib3,
                 TestInputs.ActivityAssemblyItems.TestInput_Lib2,
                 TestInputs.ActivityAssemblyItems.TestInput_Lib1))
            {
                var expectedAssembly = TestInputs.ActivityAssemblyItems.TestInput_Lib3;

                // Resolve the assembly.
                Assert.AreSame(expectedAssembly.Assembly, Utility.Resolve(expectedAssembly.FullName));
            }
        }
    }
}
