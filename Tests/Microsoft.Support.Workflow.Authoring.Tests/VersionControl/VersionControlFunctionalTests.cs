using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Principal;
using Microsoft.Support.Workflow.Authoring.Security;
using System.Threading;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Windows;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;
using Microsoft.Support.Workflow.Authoring.Tests.DataAccess;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;

namespace Microsoft.Support.Workflow.Authoring.Tests.VersionControl
{
    [TestClass]
    public class VersionControlFunctionalTests
    {
        private const string OWNER = "v-yiabdi";
        private const string TestCategory = "FullTest";
        private bool isPopuped = false;

        [Owner(OWNER)]
        [TestMethod]
        [WorkItem(164410)]
        [TestCategory("Func-NoDif1-Full")]
        [Description("Save new Project As Public status")]
        public void SaveProjectAsPublic()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            TestUtilities.RegistLoginUserRole(Role.Admin);
            //Create project or workflow as public activities
            var properties = new WorkFlowProperties
            {
                Status = "Public",
                Version = "0.1.0.1"
            };

            //TestUtilities.MockCreateIntellisenseList(() =>
            //{
            //check Version of store activity and activity library corresponding to 
            //workflow is 0.0.0.1 and status of both SA and AL is "Public"
            WorkFlowActions.CreateSaveAndValidateWorkFlow(properties);
        }


        [Owner(OWNER)]
        [TestMethod]
        [WorkItem(164411)]
        [TestCategory("Func-NoDif2-Full")]
        [Description("Save new Project As Private status")]
        public void SaveProjectAsPrivate()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            TestUtilities.RegistLoginUserRole(Role.Author);
            //Create project or workflow as public activities
            var properties = new WorkFlowProperties
            {
                Status = "Private",
                Version = "0.0.0.1"
            };

            //TestUtilities.MockCreateIntellisenseList(() =>
            //{
            //check Version of store activity and activity library corresponding to workflow 
            //workflow is 0.1.0.1 and status of both SA and AL is "Public"
            WorkFlowActions.CreateSaveAndValidateWorkFlow(properties);
        }


        [Owner(OWNER)]
        [TestMethod]
        [WorkItem(164413)]
        [TestCategory("Func-NoDif3-Full")]
        [Description("Save Existing Name As New Project")]
        public void SaveExistingNameAsNewProject()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            TestUtilities.RegistLoginUserRole(Role.Author);
            //Create project or workflow as public activities
            var properties = new WorkFlowProperties
            {
                Status = "Private",
                Version = "0.0.0.1"
            };

            //Create WorkFlow with some name "xyz"
            //Save WF as either private/public
            var workFlowItem = WorkFlowActions.CreateSaveAndValidateWorkFlow(properties);
            WorkflowItem recreatedWorkFlow = null;
            try
            {
                //Create new workflow with same name "xyz"
                recreatedWorkFlow = WorkFlowActions.CreateWorkFlowItem(
                            workFlowItem.Name, TestUtilities.GenerateRandomString(10));

                //Verify Work flow in the DB
                TestUtilities.VerifyWorkflowPropertiesInDB(properties, recreatedWorkFlow);
            }
            catch (Exception ex)
            {
                //Should not re-crate workflow with the same name
                Assert.IsNotNull(recreatedWorkFlow, "Should not Create workflow with existing Workflow.Name " + ex.Message);
            }
        }

        //Bug Id: 164436
        [Ignore]
        [Owner(OWNER)]
        [TestMethod]
        [WorkItem(164415)]
        [TestCategory("Func-NoDif-Full")]
        [Description("Save Existing Retired Name As New Project")]
        public void SaveExistingRetiredNameAsNewProject()
        {
            TestUtilities.RegistCreateIntellisenseList();
            //Create project or workflow as public activities
            var properties = new WorkFlowProperties
            {
                Status = "Retired",
                Version = "0.0.0.1"
            };

            TestUtilities.RegistCreateIntellisenseList();
            //TestUtilities.MockCreateIntellisenseList(() =>
            //{
                //Create WorkFlow with some name "xyz"
                //Save WF as Retired
                var workFlowItem = WorkFlowActions.CreateSaveAndValidateWorkFlow(properties);
                WorkflowItem recreatedWorkFlow = null;
                try
                {
                    //Create New WorkFlow
                    recreatedWorkFlow = WorkFlowActions.AutoGeneratedWorkFlowItem;
                    //Change worflowProperty
                    recreatedWorkFlow.Name = workFlowItem.Name;
                    //Verify Work flow in the DB
                    TestUtilities.VerifyWorkflowPropertiesInDB(properties, recreatedWorkFlow);
                    workFlowItem.Close();
                }
                catch (Exception ex)
                {
                    //Should not re-crate workflow with retired status
                    Assert.IsNotNull(recreatedWorkFlow, "Should not Create workflow with existing " +
                                                          "Workflow.Name even if it has Retired Status " + ex.Message);
                }
            //});
        }

        [Owner(OWNER)]
        [TestMethod]
        [WorkItem(164417)]
        [TestCategory("Func-NoDif2-Full")]
        [Description("Save Compiled Project As Public")]
        public void SaveCompiledProjectAsPublic()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            TestUtilities.RegistLoginUserRole(Role.Author);
            //Create project or workflow as public activities
            var properties = new WorkFlowProperties
            {
                Name = TestUtilities.GenerateRandomString(25),
                Status = "Public",
                Version = "0.1.0.1"
            };

            //Create New WorkFlow
            var publicWorkFlow = WorkFlowActions.AutoGeneratedWorkFlowItem;
            //Change worflowProperty
            publicWorkFlow.Name = properties.Name;
            publicWorkFlow.Status = properties.Status;
            publicWorkFlow.Version = properties.Version;

            int random = new Random().Next(2, 5);
            WorkFlowActions.CompileSaveAndVerifyWorkFlowVersion(random, publicWorkFlow);
            publicWorkFlow.Close();
        }

        [Owner(OWNER)]
        [TestMethod]
        [WorkItem(164418)]
        [TestCategory("Func-NoDif2-Full")]
        [Description("Save Compiled Project As Private")]
        public void SaveCompiledProjectAsPrivate()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            TestUtilities.RegistLoginUserRole(Role.Author);
            TestUtilities.RegistCreateIntellisenseList();
            //Create project or workflow as public activities
            var properties = new WorkFlowProperties
            {
                Name = TestUtilities.GenerateRandomString(50),
                Status = "Private",
                Version = "0.0.0.1"
            };

            //check Version of store activity and activity library corresponding to 
            //workflow is 0.0.0.1 and status of both SA and AL is "Public"
            var privateWorkFlow = WorkFlowActions.AutoGeneratedWorkFlowItem;

            //Change worflowProperty
            privateWorkFlow.Name = properties.Name;
            privateWorkFlow.Status = properties.Status;
            privateWorkFlow.Version = properties.Version;

            int random = new Random().Next(2, 5);
            WorkFlowActions.CompileSaveAndVerifyWorkFlowVersion(random, privateWorkFlow);
            privateWorkFlow.Close();
        }


        [Owner(OWNER)]
        [TestMethod]
        [WorkItem(164419)]
        [TestCategory("Func-NoDif1-Full")]
        [Description("Save Project As Private With Public Activities")]
        public void SaveProjectAsPrivateWithPublicActivities()
        {
            TestUtilities.RegistCreateIntellisenseList();
            //Create project or workflow as public activities
            var properties = new WorkFlowProperties
            {
                Name = TestUtilities.GenerateRandomString(25),
                Status = "Private",
                Version = "0.0.0.1"
            };
            //check Version of store activity and activity library corresponding to 
            //workflow is 0.0.0.1 and status of both SA and AL is "Public"
            var privateWorkFlow = WorkFlowActions.AutoGeneratedWorkFlowItem;

            //Change worflowProperty
            privateWorkFlow.Name = properties.Name;
            privateWorkFlow.Status = properties.Status;
            privateWorkFlow.Version = properties.Version;

            //Save Actvity as Private
            if (privateWorkFlow.ParentAssemblyItem != null &&
                    privateWorkFlow.ParentAssemblyItem.ActivityItems != null)
            {
                privateWorkFlow.ParentAssemblyItem.ActivityItems[0].Status = "Public";
            }

            try
            {
                int random = new Random().Next(2, 5);
                WorkFlowActions.CompileSaveAndVerifyWorkFlowVersion(random, privateWorkFlow);
                privateWorkFlow.Close();
            }
            catch (Exception ex)
            {
                //Should not re-crate workflow with retired status
                Assert.IsNotNull(privateWorkFlow, "Should not Create public workflow with private Acticities " + ex.Message);
            }

        }

        [Owner(OWNER)]
        [TestMethod]
        [WorkItem(164421)]
        [TestCategory("Func-NoDif1-Full")]
        [Description("Save Project As Public With Private Activities")]
        public void SaveProjectAsPublicWithPrivateActivities()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            TestUtilities.RegistCreateIntellisenseList();
            //Create project or workflow as public activities
            var properties = new WorkFlowProperties
            {
                Name = TestUtilities.GenerateRandomString(25),
                Status = "Public",
                Version = "0.0.0.1"
            };

            //check Version of store activity and activity library corresponding to 
            //workflow is 0.0.0.1 and status of both SA and AL is "Public"
            var privateWorkFlow = WorkFlowActions.AutoGeneratedWorkFlowItem;

            //Change worflowProperty
            privateWorkFlow.Name = properties.Name;
            privateWorkFlow.Status = properties.Status;
            privateWorkFlow.Version = properties.Version;

            var select = TestUtilities.SelectAssemblyAndActivity;
            //Modify select status
            foreach (ActivityAssemblyItem activityAssemblyItem in select.ActivityAssemblyItemCollection)
            {
                if (activityAssemblyItem != null)
                {
                    activityAssemblyItem.Status = "Private";
                    privateWorkFlow.ParentAssemblyItem = activityAssemblyItem;
                    break;
                }
            }

            try
            {
                int random = new Random().Next(2, 5);
                WorkFlowActions.CompileSaveAndVerifyWorkFlowVersion(random, privateWorkFlow);
                privateWorkFlow.Close();
            }
            catch (Exception ex)
            {
                //Should not re-crate workflow with retired status
                Assert.IsNotNull(privateWorkFlow, "Should not Create p workflow with private Acticities " + ex.Message);
            }
        }

        [Owner(OWNER)]
        [TestMethod]
        [WorkItem(164422)]
        [TestCategory("Func-NoDif2-Full")]
        [Description("Update Private Project And Make It Public")]
        public void UpdatePrivateProjectAndMakeItPublic()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            TestUtilities.RegistLoginUserRole(Role.Admin);
            TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            //TestUtilities.MockWinPrincipal(AuthorizationService.AdminAuthorizationGroupName, () =>
            //{
            //Create project or workflow as public activities
            var properties = new WorkFlowProperties
            {
                Name = TestUtilities.GenerateRandomString(25),
                Status = "Private",
                Version = "0.0.0.1"
            };
            //TestUtilities.MockCreateIntellisenseList(() =>
            //{
            //    TestUtilities.MockMessageBoxServiceOfCommonOperate(() =>
            //    {
            // Set
            //check Version of store activity and activity library corresponding to 
            //workflow is 0.0.0.1 and status of both SA and AL is "Public"
            var publicWorkFlow = WorkFlowActions.CreateSaveAndValidateWorkFlow(properties, null, false);

            //Modify Expected Stats to be 0.0.0.2
            publicWorkFlow.Status = "Public";
            //Modify Expected Version to be 0.0.0.2
            properties.Version = "0.0.0.2";
            //Save after Update
            WorkFlowActions.CreateSaveAndValidateWorkFlow(properties, publicWorkFlow);
            publicWorkFlow.Close();
            //        });
            //    });
            //});
        }

        [Owner(OWNER)]
        [TestMethod]
        [WorkItem(164424)]
        [TestCategory("Func-NoDif2-Full")]
        [Description("Update Public Project And Make It Private")]
        public void UpdatePublicProjectAndMakeItPrivate()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            TestUtilities.RegistLoginUserRole(Role.Author);
            //Create project or workflow as public activities
            var properties = new WorkFlowProperties
            {
                Name = TestUtilities.GenerateRandomString(25),
                Status = "Public",
                Version = "0.1.0.1"
            };


            // Set
            //check Version of store activity and activity library corresponding to 
            //workflow is 0.0.0.1 and status of both SA and AL is "Public"
            var publicWorkFlow = WorkFlowActions.CreateSaveAndValidateWorkFlow(properties, null, false);

            //Modify Expected Stats to be 0.0.0.2
            publicWorkFlow.Status = "Private";
            //Modify Expected Version to be 0.0.0.2
            properties.Version = "0.2.0.1";

            //Save after Update
            WorkFlowActions.CreateSaveAndValidateWorkFlow(properties, publicWorkFlow);
            publicWorkFlow.Close();
        }
    }
}
