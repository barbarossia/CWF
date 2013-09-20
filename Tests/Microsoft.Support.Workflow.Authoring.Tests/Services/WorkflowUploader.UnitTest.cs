// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowUploaderUnitTest.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Tests
{
    #region References

    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.DynamicImplementations;

    using CWF.DataContracts;

    using Microsoft.Support.Workflow.Authoring.Models;
    using Microsoft.Support.Workflow.Authoring.Services;
    using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
    using Microsoft.Support.Workflow.Authoring.ViewModels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Testinput_Lib1;
    using TestInput_Lib2;
    using Testinput_Lib3;
    using AuthoringToolTests.Services;
    using System.Security.Principal;
    using System.Windows;
    using Microsoft.Support.Workflow.Authoring.Security;
    using System.Threading;
    using System.Activities.Statements;
    using System.Activities.Presentation;
    using System.Activities.Presentation.Hosting;
    using System.ServiceModel.Activities;
    using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
    using Microsoft.Support.Workflow.Authoring.AddIns;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
    using Microsoft.Support.Workflow.Authoring.AddIns.Data;


    #endregion References

    /// <summary>
    /// Unit tests for the WorkflowUploader class
    ///</summary>
    [TestClass()]
    public class WorkflowUploaderUnitTest
    {
        #region Private fields and constants
        private const string WF_UNDER_TEST = "OAS Page";
        private const string TEST_INPUT_LIB1 = "TestInput_Lib1";
        private const string TEST_INPUT_LIB2 = "TestInput_Lib2";
        private const string TEST_INPUT_LIB3 = "TestInput_Lib3";
        private const string TEST_LIB1 = "TestInput_Lib1";
        private const string TEST_LIB2 = "TestInput_Lib2";
        private const string TEST_LIB3 = "TestInput_Lib3";
        private const string TEST_ACTIVITY1 = "Activity1";
        private const string TEST_ACTIVITY2 = "Activity1";
        private const string TEST_ACTIVITY3 = "Activity1";
        private const string STRING_DELIMITER = ", ";

        #endregion Private fields and constants

        // Create WorkflowItems lazily to avoid creating lots of WorkflowDesigners.
        private WorkflowItem validWorkflowItem;
        private WorkflowItem ValidWorkflowItem
        {
            get
            {
                if (validWorkflowItem == null)
                {
                    validWorkflowItem = new WorkflowItem("MyWorkflow", "WF", TestUtilities.GetEmptyWorkFlowTemplateXamlCode(), string.Empty);
                    validWorkflowItem.Env = Authoring.AddIns.Data.Env.Dev;
                }

                return validWorkflowItem;
            }
        }

        #region Unit Test Methods

        [WorkItem(322375)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void WorkflowUploader_VerifyWorkflowPropertyAfterUpload()
        {
            var wf = ValidWorkflowItem;
            using (var client = new Implementation<WorkflowsQueryServiceClient>())
            {
                client.Register(inst => inst.GetMissingActivityLibraries(Argument<GetMissingActivityLibrariesRequest>.Any))
                    .Execute(() =>
                    {
                        GetMissingActivityLibrariesReply reply = new GetMissingActivityLibrariesReply();
                        reply.MissingActivityLibraries = new System.Collections.Generic.List<ActivityLibraryDC>();
                        return reply;
                    });
                client.Register(inst => inst.UploadActivityLibraryAndDependentActivities(Argument<StoreLibraryAndActivitiesRequestDC>.Any)).Execute
                 (() =>
                 {
                     List<StoreActivitiesDC> dc = new List<StoreActivitiesDC>() 
                        {
                            new StoreActivitiesDC()
                            {
                                StatusReply = new StatusReplyDC(){Errorcode=0},
                                Version = "1.0.0.1",
                            },
                        };
                     return dc;
                 });
                    
                using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                {
                    List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                    workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                    workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                    workflowDesigner.Register(inst => inst.XamlCode).Return(string.Empty);
                    workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                    workflowDesigner.Register(inst => inst.Tasks).Return(new List<TaskAssignment>());
                    workflowDesigner.Register(inst => inst.FinishTaskAssigned()).Execute(() => { });
                          
                    wf.WorkflowDesigner = workflowDesigner.Instance;
                    WorkflowUploader.Upload(client.Instance, wf);
                    Assert.IsTrue(wf.IsOpenFromServer);
                    Assert.IsTrue(wf.IsSavedToServer);
                    Assert.IsFalse(wf.IsDataDirty);
                    Assert.AreEqual(wf.Version, "1.0.0.1");
                }
            }
        }

        [WorkItem(322321)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void WorkflowUploader_VerifyCanUploadWorkflow()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
           {
               TestUtilities.RegistLoginUserRole(Role.Admin);
               using (var dc = new ImplementationOfType(typeof(DataContractTranslator)))
               {
                   dc.Register(() => DataContractTranslator.ActivityItemToStoreActivitiyDC(Argument<ActivityItem>.Any)).Execute(() =>
                   {
                       return new StoreActivitiesDC() 
                       { 
                           LockedBy = "admin", 
                           UpdatedDateTime = new DateTime(1990, 10, 10), 
                           Environment = "dev"
                       };
                   });

                   using (var service = new ImplementationOfType(typeof(MessageBoxService)))
                   {
                       service.Register(() => MessageBoxService.CannotSaveDuplicatedNameWorkflow(Argument<string>.Any)).Execute(() => { });
                       service.Register(() => MessageBoxService.CannotSaveLockedActivity()).Return(MessageBoxResult.OK);
                       service.Register(() => MessageBoxService.CreateNewActivityOnSaving()).Return(MessageBoxResult.Yes);
                       service.Register(() => MessageBoxService.DownloadNewActivityOnSaving()).Return(MessageBoxResult.Yes);
                       using (var client = new Implementation<WorkflowsQueryServiceClient>())
                       {
                           //can save duplicated name
                           var wf = ValidWorkflowItem;
                           wf.IsOpenFromServer = false;
                           client.Register(inst => inst.StoreActivitiesGetByName(Argument<StoreActivitiesDC>.Any))
                               .Return(new List<StoreActivitiesDC>() { new StoreActivitiesDC(), });
                           var result1 = WorkflowUploader.CheckCanUpload(client.Instance, wf);
                           Assert.IsFalse(result1.Item1);
                           Assert.IsNull(result1.Item2);

                           using (var principal = new Implementation<WindowsPrincipal>())
                           {
                               // TODO: setup permissions
                               Thread.CurrentPrincipal = principal.Instance;

                               //no latest workflow
                               client.Register(inst => inst.StoreActivitiesGetByName(Argument<StoreActivitiesDC>.Any))
                                   .Return(new List<StoreActivitiesDC>());
                               var result2 = WorkflowUploader.CheckCanUpload(client.Instance, wf);
                               Assert.IsTrue(result2.Item1);
                               Assert.IsNull(result2.Item2);

                               wf.IsOpenFromServer = true;
                               //have latest workflow and insertedTime <= updateTime
                               client.Register(inst => inst.StoreActivitiesGetByName(Argument<StoreActivitiesDC>.Any))
                              .Return(new List<StoreActivitiesDC>()
                               { 
                                   new StoreActivitiesDC(){ InsertedDateTime = new DateTime(1990,10,9),}
                               });

                               wf.UpdateDateTime = DateTime.Now;

                               // TODO: setup permissions

                               Thread.CurrentPrincipal = principal.Instance;
                               var result3 = WorkflowUploader.CheckCanUpload(client.Instance, wf);
                               Assert.IsTrue(result3.Item1);
                               Assert.IsNull(result3.Item2);


                               //have latest workflow insertedTime > updateTime
                               client.Register(inst => inst.StoreActivitiesGetByName(Argument<StoreActivitiesDC>.Any))
                                .Return(new List<StoreActivitiesDC>() { new StoreActivitiesDC() { InsertedDateTime = DateTime.Now, }, });

                               //admin
                               // TODO: setup permissions
                               Thread.CurrentPrincipal = principal.Instance;

                               var result4 = WorkflowUploader.CheckCanUpload(client.Instance, wf);
                               Assert.IsTrue(result4.Item1);
                               Assert.IsNull(result4.Item2);

                               //author
                               // TODO: setup permissions

                               var result5 = WorkflowUploader.CheckCanUpload(client.Instance, wf);
                               Assert.IsTrue(result5.Item1);
                               Assert.IsNull(result5.Item2);
                           }
                       }
                   }
               }
           });
        }


        /// <summary>
        /// A test to make sure assemblies are uploaded 
        ///</summary>
        [TestMethod()]
        [Description("Unit test for Uploading a workflow.")]
        [Owner("v-mimcin")]
        [TestCategory("Unit-Dif")]
        public void WorkflowUploader_UploadTest()
        {
            // Arrange
            var lib3 = TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib3;
            var lib2 = TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib2;
            var lib1 = TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1;

            // Step up Cacing: stub out database, create query service under test
            using (new CachingIsolator(lib1, lib2, lib3))
            {
                var wfUnderTest = ValidWorkflowItem;

                // Create a new workflow from Activity3 and compute it's dependencies
                var dependecy = new List<ActivityAssemblyItem>
                {
                    TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1,
                    TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib2,
                    TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib3
                };
                dependecy.ForEach(i => i.Env = Authoring.AddIns.Data.Env.Dev);
                using (var stubClient = new Implementation<IWorkflowsQueryService>())
                {
                    stubClient.Register(inst => inst.GetMissingActivityLibraries(Argument<GetMissingActivityLibrariesRequest>.Any))
                    .Execute(() =>
                    {
                        GetMissingActivityLibrariesReply reply = new GetMissingActivityLibrariesReply();
                        reply.MissingActivityLibraries = new List<ActivityLibraryDC>()
                        {
                            new ActivityLibraryDC() {
                                Name = lib1.Name, 
                                VersionNumber = lib1.Version.ToString(),
                                Environment = "dev"
                            },
                            new ActivityLibraryDC() {
                                Name = lib2.Name, 
                                VersionNumber = lib2.Version.ToString(),
                                Environment = "dev"
                            },
                            new ActivityLibraryDC() {
                                Name = lib3.Name,
                                VersionNumber = lib3.Version.ToString(),
                                Environment = "dev"
                            },
                        };
                        return reply;
                    });

                    stubClient.Register(inst => inst.UploadActivityLibraryAndDependentActivities(Argument<StoreLibraryAndActivitiesRequestDC>.Any)).Execute
                      (() =>
                      {
                          List<StoreActivitiesDC> dc = new List<StoreActivitiesDC>() 
                        {
                            new StoreActivitiesDC()
                            {
                                StatusReply = new StatusReplyDC(){Errorcode=0},
                                Version = "1.0.0.1",
                                Environment = "dev"
                            },
                        };
                          return dc;
                      });

                    using (var workflowDesigner = new Implementation<DesignerHostAdapters>())
                    {
                        workflowDesigner.Register(inst => inst.View).Return(new FrameworkElement());
                        workflowDesigner.Register(inst => inst.ToolboxView).Return(new FrameworkElement());
                        workflowDesigner.Register(inst => inst.ProjectExplorerView).Return(new FrameworkElement());
                        workflowDesigner.Register(inst => inst.PropertyInspectorView).Return(new FrameworkElement());
                        workflowDesigner.Register(inst => inst.DependencyAssemblies).Return(dependecy);
                        workflowDesigner.Register(inst => inst.XamlCode).Return(string.Empty);
                        workflowDesigner.Register(inst => inst.IsWorkflowService).Return(false);
                        workflowDesigner.Register(inst => inst.Tasks).Return(new List<TaskAssignment>());
                        workflowDesigner.Register(inst => inst.FinishTaskAssigned()).Execute(() => { });
                        wfUnderTest.WorkflowDesigner = workflowDesigner.Instance;

                        // ASSERT
                        StatusReplyDC actualResult = WorkflowUploader.Upload(stubClient.Instance, wfUnderTest);
                        Assert.IsTrue(actualResult.Errorcode == 0);
                    }
                }
            }
        }

        #endregion Unit Test Methods
    }
}
