using System;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using AuthoringToolTests.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.DataAccess;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Microsoft.Support.Workflow.Authoring.Security;
using Microsoft.DynamicImplementations;
using System.Security.Principal;
using System.Threading;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;

namespace Authoring.Tests.Functional
{
    [TestClass]
    public class UploadAssemblyViewModelFunctionalTest
    {
        #region test methods

        [WorkItem(16278)]
        [Description("Test the UploadAssembly function")]
        [Owner("v-ertang")]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyUploadAssembly()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            var stringBuilder = new StringBuilder();
            // mock  the “upload successful” dialog message box 
            MessageBoxService.ShowFunc = ((msg, a, b, c, d) => { stringBuilder.Append(msg); return MessageBoxResult.OK; });
            string executingAssemblyPath = TestUtilities.GetExecutingAssemblyPath();
            var vmUploadAssembly = new UploadAssemblyViewModel();
            vmUploadAssembly.Initialize(new[] { new ActivityAssemblyItem(Assembly.LoadFrom((executingAssemblyPath))) });
            vmUploadAssembly.UploadCommand.Execute();
            Assert.AreEqual("Upload successful", stringBuilder.ToString());
        }


        [Description("Verify assembly with dependency are also uploaded")]
        [Owner("v-shgudi")]
        [TestCategory("Func-NoDif3-Smoke")]
        [TestMethod()]
        public void VerifyAssemblywithDependencyUpload()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            TestUtilities.RegistLoginUserRole(Role.Author);
            using (new CachingIsolator(
                            TestInputs.ActivityAssemblyItems.TestInput_Lib3,
                            TestInputs.ActivityAssemblyItems.TestInput_Lib2,
                            TestInputs.ActivityAssemblyItems.TestInput_Lib1))
            {
                // Delete the test input libraries if they are already in the database.
                TestDataUtility.DeleteTestInputLibrariesDataFromDB();

                // tests uploading the assemblies with dependencies(TestInput_Library3 is dependent on Library2 which is dependent on Library1) 
                try
                {
                    ActivityAssemblyItem activityAssemblyItem3 = TestInputs.ActivityAssemblyItems.TestInput_Lib3;
                    ActivityAssemblyItem activityAssemblyItem2 = TestInputs.ActivityAssemblyItems.TestInput_Lib2;
                    ActivityAssemblyItem activityAssemblyItem1 = TestInputs.ActivityAssemblyItems.TestInput_Lib1;

                    var stringBuilder = new StringBuilder();

                    // mock  the “upload successful” dialog message box 
                    MessageBoxService.ShowFunc = ((msg, a, b, c, d) => { stringBuilder.Append(msg); return MessageBoxResult.OK; });
                    string executingAssemblyPath = TestUtilities.GetExecutingAssemblyPath();


                    var vmUploadAssembly = new UploadAssemblyViewModel();
                    vmUploadAssembly.Initialize(new[] { activityAssemblyItem3, activityAssemblyItem2, activityAssemblyItem1 });

                    activityAssemblyItem3.UserWantsToUpload = true;
                    activityAssemblyItem2.UserWantsToUpload = true;
                    activityAssemblyItem1.UserWantsToUpload = true;

                    //Upload
                    if (vmUploadAssembly.UploadCommand.CanExecute())
                    {
                        vmUploadAssembly.UploadCommand.Execute();
                    }

                    //Checking for Upload Successful
                    Assert.AreEqual("Upload successful", stringBuilder.ToString());

                    // Verify that the test input libraries made to database.
                    Assert.IsTrue(TestDataUtility.VerifyTestInputLibrariesAreAvailableInDB());
                }
                finally
                {
                    // Delete the test input libraries that are added to database by this test.
                    TestDataUtility.DeleteTestInputLibrariesDataFromDB();
                }
            }
        }

        [Description("Verify dependency of selecting assembly are also automatically selected")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void VerifyAssemblywithDependencyAutoSelectedAndUnselected()
        {
            using (new CachingIsolator(
                            TestInputs.ActivityAssemblyItems.TestInput_Lib3,
                            TestInputs.ActivityAssemblyItems.TestInput_Lib2,
                            TestInputs.ActivityAssemblyItems.TestInput_Lib1))
            {
                TestDataUtility.DeleteTestInputLibrariesDataFromDB();

                try
                {
                    ActivityAssemblyItem activityAssemblyItem3 = TestInputs.ActivityAssemblyItems.TestInput_Lib3;
                    ActivityAssemblyItem activityAssemblyItem2 = TestInputs.ActivityAssemblyItems.TestInput_Lib2;
                    ActivityAssemblyItem activityAssemblyItem1 = TestInputs.ActivityAssemblyItems.TestInput_Lib1;

                    var vmUploadAssembly = new UploadAssemblyViewModel();
                    vmUploadAssembly.Initialize(new[] { activityAssemblyItem3, activityAssemblyItem2, activityAssemblyItem1 });

                    activityAssemblyItem3.UserWantsToUpload = true;
                    vmUploadAssembly.NotifyUploadAssemblyItemChange(activityAssemblyItem3, true);

                    var selected = from assemblies in vmUploadAssembly.DisplayActivityAssemblyItems
                                   where assemblies.UserWantsToUpload == true
                                   select assemblies;

                    Assert.AreEqual(3, selected.Count());

                    Assert.IsTrue(activityAssemblyItem2.UserWantsToUpload);
                    Assert.IsTrue(activityAssemblyItem1.UserWantsToUpload);

                    activityAssemblyItem1.UserWantsToUpload = false;
                    vmUploadAssembly.NotifyUploadAssemblyItemChange(activityAssemblyItem1, false);

                    Assert.IsFalse(activityAssemblyItem2.UserWantsToUpload);
                    Assert.IsFalse(activityAssemblyItem3.UserWantsToUpload);

                }
                finally
                {
                    TestDataUtility.DeleteTestInputLibrariesDataFromDB();
                }
            }
        }


        #endregion
    }
}
