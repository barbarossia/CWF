using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Models;
using System.Xaml;
using System.Activities;
using System.Activities.Statements;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.DynamicImplementations;
using CWF.DataContracts;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

namespace AuthoringToolTests.Services
{
    [TestClass]
    public class WorkflowUploaderFunctionalTests
    {
        [Description("Verify the WorkflowUploader's UploadWorkflow() function in detail on a mock database")]
        [Owner("v-maxw")]
        [TestCategory("Func-Dif-Full")]
        [TestMethod()]
        [Ignore]
        public void TestUploadingWithDependencies()
        {
            // Arrange
            var wf = new Testinput_Lib3.Activity1();

            using (var cache = new CachingIsolator(TestInputs.ActivityAssemblyItems.TestInput_Lib3, TestInputs.ActivityAssemblyItems.TestInput_Lib2, TestInputs.ActivityAssemblyItems.TestInput_Lib1))
            using (var mock = new Mock<IWorkflowsQueryService>())
            {

                // Act + Assert: workflows will be uploaded in order of dependency graph (most dependent last, i.e. lib1 lib2 lib3)
                mock.Expect(inst => inst.GetAllActivityLibraries(Argument<GetAllActivityLibrariesRequestDC>.Any))
                    .Return(new GetAllActivityLibrariesReplyDC() { List = new List<ActivityLibraryDC>() });

                mock
                    .Expect(inst => inst.UploadActivityLibraryAndDependentActivities(Argument<StoreLibraryAndActivitiesRequestDC>.Any))
                    .Execute((StoreLibraryAndActivitiesRequestDC request) =>
                        {
                            Assert.AreEqual("Activity1", request.StoreActivitiesList.FirstOrDefault().IfNotNull(a => a.ShortName));
                            Assert.AreEqual(0, request.StoreActivityLibraryDependenciesGroupsRequestDC.List.Count);
                            return new StatusReplyDC();
                        });

                mock
                    .Expect(inst => inst.UploadActivityLibraryAndDependentActivities(Argument<StoreLibraryAndActivitiesRequestDC>.Any))
                    .Execute((StoreLibraryAndActivitiesRequestDC request) =>
                        {
                            Assert.AreEqual("Activity2", request.StoreActivitiesList.FirstOrDefault().IfNotNull(a => a.ShortName));
                            Assert.AreEqual("TestInput_Lib1", request.StoreActivityLibraryDependenciesGroupsRequestDC.List.FirstOrDefault().IfNotNull(dep => dep.Name));
                            return new StatusReplyDC();
                        });

                mock
                    .Expect(inst => inst.UploadActivityLibraryAndDependentActivities(Argument<StoreLibraryAndActivitiesRequestDC>.Any))
                    .Execute((StoreLibraryAndActivitiesRequestDC request) =>
                        {
                            Assert.AreEqual("Activity3", request.StoreActivitiesList.FirstOrDefault().IfNotNull(a => a.ShortName));
                            Assert.AreEqual("TestInput_Lib2", request.StoreActivityLibraryDependenciesGroupsRequestDC.List.FirstOrDefault().IfNotNull(dep => dep.Name));
                            return new StatusReplyDC();
                        });

                mock
                    .Expect(inst => inst.UploadActivityLibraryAndDependentActivities(Argument<StoreLibraryAndActivitiesRequestDC>.Any))
                    .Return(new StatusReplyDC());

                var client = mock.Instance;

                // actually kick off the mocking process
                WorkflowUploader.Upload(client, new WorkflowItem(TestUtilities.GenerateRandomString(25), 
                                            TestUtilities.GenerateRandomString(15), wf.ToXaml(), workflowType: "N/A"));

                mock.Verify();
            }
        }
   }
}
