using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.DataAccess;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Authoring.Tests.Functional
{
    [TestClass]
    public class VerifyRolePermissionFunctionalTest
    {
        #region test initiliaze
        [TestInitialize]
        public void TestInitialize()
        {
           //// test001Path = Path.Combine(testContextInstance.DeploymentDirectory, test001dllName);
        }
        #endregion

        [Description("Needed to use the MainWindowViewModel class")]
        [ClassInitialize]
        public static void TestSetup(TestContext context)
        {
            TestUtilities.LoadSystemActivitesIntoCurrentAppDomain();

        }

        [WorkItem(405138)]
        [Description("Verify viewer user login AL the menu permission")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void VerifyViewerUserMenuPermission()
        {
            string workFlowItem = string.Empty;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.Viewer);
                MainWindowViewModel mw = new MainWindowViewModel();
                Assert.IsFalse(mw.NewWorkflowCommand.CanExecute());
                Assert.IsFalse(mw.SaveFocusedWorkflowCommand.CanExecute("ToServer"));
                Assert.IsFalse(mw.CopyProjectCommand.CanExecute());
                Assert.IsFalse(mw.DeleteProjectCommand.CanExecute());
                Assert.IsFalse(mw.ChangeAuthorCommand.CanExecute());
                Assert.IsTrue(mw.OpenWorkflowCommand.CanExecute("ToServer"));
                Assert.IsFalse(mw.UploadAssemblyCommand.CanExecute());
                Assert.IsTrue(mw.SelectAssemblyAndActivityCommand.CanExecute());
                Assert.IsFalse(mw.PublishCommand.CanExecute());
                Assert.IsFalse(mw.CompileCommand.CanExecute());
                Assert.IsTrue(mw.OpenMarketplaceCommand.CanExecute());
                Assert.IsFalse(mw.OpenEnvSecurityOptionsCommand.CanExecute());
                Assert.IsFalse(mw.OpenTenantSecurityOptionsCommand.CanExecute());

                workFlowItem = TestDataUtility.CreateWorkFlowItemTestData(false);
                var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();

                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.RegistCreateIntellisenseList();
                MainWindowViewModel mwvm = new MainWindowViewModel();
                TestUtilities.OpenWorkflowFromServer(workflowToOpen, mwvm);

                Assert.IsFalse(mwvm.NewWorkflowCommand.CanExecute());
                Assert.IsFalse(mwvm.SaveFocusedWorkflowCommand.CanExecute("ToServer"));
                Assert.IsFalse(mwvm.CopyProjectCommand.CanExecute());
                Assert.IsFalse(mwvm.DeleteProjectCommand.CanExecute());
                Assert.IsFalse(mwvm.ChangeAuthorCommand.CanExecute());
                Assert.IsTrue(mwvm.OpenWorkflowCommand.CanExecute("ToServer"));
                Assert.IsFalse(mwvm.UploadAssemblyCommand.CanExecute());
                Assert.IsTrue(mwvm.SelectAssemblyAndActivityCommand.CanExecute());
                Assert.IsFalse(mwvm.PublishCommand.CanExecute());
                Assert.IsFalse(mwvm.CompileCommand.CanExecute());
                Assert.IsTrue(mwvm.OpenMarketplaceCommand.CanExecute());
                Assert.IsFalse(mwvm.OpenEnvSecurityOptionsCommand.CanExecute());
                Assert.IsFalse(mwvm.OpenTenantSecurityOptionsCommand.CanExecute());
                mwvm.WorkflowItems.FirstOrDefault().Close();
            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(workFlowItem);
            }
        }

        [WorkItem(405139)]
        [Description("Verify author user login AL the menu permission")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void VerifyAuthorUserMenuPermission()
        {
            string workFlowItem = string.Empty;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.Author);
                MainWindowViewModel mw = new MainWindowViewModel();
                Assert.IsTrue(mw.NewWorkflowCommand.CanExecute());
                Assert.IsFalse(mw.SaveFocusedWorkflowCommand.IsActive);
                Assert.IsFalse(mw.CopyProjectCommand.CanExecute());
                Assert.IsFalse(mw.DeleteProjectCommand.CanExecute());
                Assert.IsFalse(mw.ChangeAuthorCommand.CanExecute());
                Assert.IsTrue(mw.OpenWorkflowCommand.CanExecute("ToServer"));
                Assert.IsTrue(mw.UploadAssemblyCommand.CanExecute());
                Assert.IsTrue(mw.SelectAssemblyAndActivityCommand.CanExecute());
                Assert.IsFalse(mw.PublishCommand.CanExecute());
                Assert.IsFalse(mw.CompileCommand.CanExecute());
                Assert.IsTrue(mw.OpenMarketplaceCommand.CanExecute());
                Assert.IsFalse(mw.OpenEnvSecurityOptionsCommand.CanExecute());
                Assert.IsFalse(mw.OpenTenantSecurityOptionsCommand.CanExecute());

                workFlowItem = TestDataUtility.CreateWorkFlowItemTestData(false);
                var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();

                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.RegistCreateIntellisenseList();
                MainWindowViewModel mwvm = new MainWindowViewModel();
                TestUtilities.OpenWorkflowFromServer(workflowToOpen, mwvm);

                Assert.IsTrue(mwvm.NewWorkflowCommand.CanExecute());
                Assert.IsFalse(mwvm.SaveFocusedWorkflowCommand.CanExecute("ToServer"));
                Assert.IsTrue(mwvm.CopyProjectCommand.CanExecute());
                Assert.IsFalse(mwvm.DeleteProjectCommand.CanExecute());
                Assert.IsFalse(mwvm.ChangeAuthorCommand.CanExecute());
                Assert.IsTrue(mwvm.OpenWorkflowCommand.CanExecute("ToServer"));
                Assert.IsTrue(mwvm.UploadAssemblyCommand.CanExecute());
                Assert.IsTrue(mwvm.SelectAssemblyAndActivityCommand.CanExecute());
                Assert.IsTrue(mwvm.PublishCommand.CanExecute());
                Assert.IsTrue(mwvm.CompileCommand.CanExecute());
                Assert.IsTrue(mwvm.OpenMarketplaceCommand.CanExecute());
                Assert.IsFalse(mwvm.OpenEnvSecurityOptionsCommand.CanExecute());
                Assert.IsFalse(mwvm.OpenTenantSecurityOptionsCommand.CanExecute());
                mwvm.WorkflowItems.FirstOrDefault().Close();
            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(workFlowItem);
            }
        }

        [WorkItem(405140)]
        [Description("Verify admin user login AL the menu permission")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void VerifyAdminUserMenuPermission()
        {
            string workFlowItem = string.Empty;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.Admin);
                MainWindowViewModel mw = new MainWindowViewModel();
                Assert.IsTrue(mw.NewWorkflowCommand.CanExecute());
                Assert.IsFalse(mw.SaveFocusedWorkflowCommand.CanExecute("ToServer"));
                Assert.IsFalse(mw.CopyProjectCommand.CanExecute());
                Assert.IsFalse(mw.DeleteProjectCommand.CanExecute());
                Assert.IsFalse(mw.ChangeAuthorCommand.CanExecute());
                Assert.IsTrue(mw.OpenWorkflowCommand.CanExecute("ToServer"));
                Assert.IsTrue(mw.UploadAssemblyCommand.CanExecute());
                Assert.IsTrue(mw.SelectAssemblyAndActivityCommand.CanExecute());
                Assert.IsFalse(mw.PublishCommand.CanExecute());
                Assert.IsFalse(mw.CompileCommand.CanExecute());
                Assert.IsTrue(mw.OpenMarketplaceCommand.CanExecute());
                Assert.IsFalse(mw.OpenEnvSecurityOptionsCommand.CanExecute());
                Assert.IsFalse(mw.OpenTenantSecurityOptionsCommand.CanExecute());

                workFlowItem = TestDataUtility.CreateWorkFlowItemTestData(false);
                var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();

                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.RegistCreateIntellisenseList();
                MainWindowViewModel mwvm = new MainWindowViewModel();
                TestUtilities.OpenWorkflowFromServer(workflowToOpen, mwvm);

                Assert.IsTrue(mwvm.NewWorkflowCommand.CanExecute());
                Assert.IsFalse(mwvm.SaveFocusedWorkflowCommand.CanExecute("ToServer"));
                Assert.IsTrue(mwvm.CopyProjectCommand.CanExecute());
                Assert.IsTrue(mwvm.DeleteProjectCommand.CanExecute());
                Assert.IsTrue(mwvm.ChangeAuthorCommand.CanExecute());
                Assert.IsTrue(mwvm.OpenWorkflowCommand.CanExecute("ToServer"));
                Assert.IsTrue(mwvm.UploadAssemblyCommand.CanExecute());
                Assert.IsTrue(mwvm.SelectAssemblyAndActivityCommand.CanExecute());
                Assert.IsTrue(mwvm.PublishCommand.CanExecute());
                Assert.IsTrue(mwvm.CompileCommand.CanExecute());
                Assert.IsTrue(mwvm.OpenMarketplaceCommand.CanExecute());
                Assert.IsFalse(mwvm.OpenEnvSecurityOptionsCommand.CanExecute());
                Assert.IsFalse(mwvm.OpenTenantSecurityOptionsCommand.CanExecute());
                mwvm.WorkflowItems.FirstOrDefault().Close();
            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(workFlowItem);
            }
        }

        [WorkItem(405142)]
        [Description("Verify En admin user login AL the menu permission")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void VerifyEnvAdminUserMenuPermission()
        {
            string workFlowItem = string.Empty;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.CWFEnvAdmin);
                MainWindowViewModel mw = new MainWindowViewModel();
                Assert.IsFalse(mw.NewWorkflowCommand.CanExecute());
                Assert.IsFalse(mw.SaveFocusedWorkflowCommand.CanExecute("ToServer"));
                Assert.IsFalse(mw.CopyProjectCommand.CanExecute());
                Assert.IsFalse(mw.DeleteProjectCommand.CanExecute());
                Assert.IsFalse(mw.ChangeAuthorCommand.CanExecute());
                Assert.IsTrue(mw.OpenWorkflowCommand.CanExecute("ToServer"));
                Assert.IsFalse(mw.UploadAssemblyCommand.CanExecute());
                Assert.IsTrue(mw.SelectAssemblyAndActivityCommand.CanExecute());
                Assert.IsFalse(mw.PublishCommand.CanExecute());
                Assert.IsFalse(mw.CompileCommand.CanExecute());
                Assert.IsTrue(mw.OpenMarketplaceCommand.CanExecute());
                Assert.IsFalse(mw.OpenEnvSecurityOptionsCommand.CanExecute());
                Assert.IsTrue(mw.OpenTenantSecurityOptionsCommand.CanExecute());

                workFlowItem = TestDataUtility.CreateWorkFlowItemTestData(false);
                var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();

                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.RegistCreateIntellisenseList();
                MainWindowViewModel mwvm = new MainWindowViewModel();
                TestUtilities.OpenWorkflowFromServer(workflowToOpen, mwvm);

                Assert.IsFalse(mwvm.NewWorkflowCommand.CanExecute());
                Assert.IsFalse(mwvm.SaveFocusedWorkflowCommand.CanExecute("ToServer"));
                Assert.IsFalse(mwvm.CopyProjectCommand.CanExecute());
                Assert.IsFalse(mwvm.DeleteProjectCommand.CanExecute());
                Assert.IsFalse(mwvm.ChangeAuthorCommand.CanExecute());
                Assert.IsTrue(mwvm.OpenWorkflowCommand.CanExecute("ToServer"));
                Assert.IsFalse(mwvm.UploadAssemblyCommand.CanExecute());
                Assert.IsTrue(mwvm.SelectAssemblyAndActivityCommand.CanExecute());
                Assert.IsFalse(mwvm.PublishCommand.CanExecute());
                Assert.IsFalse(mwvm.CompileCommand.CanExecute());
                Assert.IsTrue(mwvm.OpenMarketplaceCommand.CanExecute());
                Assert.IsFalse(mwvm.OpenEnvSecurityOptionsCommand.CanExecute());
                Assert.IsTrue(mwvm.OpenTenantSecurityOptionsCommand.CanExecute());
                mwvm.WorkflowItems.FirstOrDefault().Close();
            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(workFlowItem);
            }
        }


        [WorkItem(405143)]
        [Description("Verify CWF admin user login AL the menu permission")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void VerifyCWFAdminUserMenuPermission()
        {
            string workFlowItem = string.Empty;
            try
            {
                TestUtilities.RegistLoginUserRole(Role.CWFAdmin);
                MainWindowViewModel mw = new MainWindowViewModel();
                Assert.IsFalse(mw.NewWorkflowCommand.CanExecute());
                Assert.IsFalse(mw.SaveFocusedWorkflowCommand.CanExecute("ToServer"));
                Assert.IsFalse(mw.CopyProjectCommand.CanExecute());
                Assert.IsFalse(mw.DeleteProjectCommand.CanExecute());
                Assert.IsFalse(mw.ChangeAuthorCommand.CanExecute());
                Assert.IsTrue(mw.OpenWorkflowCommand.CanExecute("ToServer"));
                Assert.IsFalse(mw.UploadAssemblyCommand.CanExecute());
                Assert.IsTrue(mw.SelectAssemblyAndActivityCommand.CanExecute());
                Assert.IsFalse(mw.PublishCommand.CanExecute());
                Assert.IsFalse(mw.CompileCommand.CanExecute());
                Assert.IsTrue(mw.OpenMarketplaceCommand.CanExecute());
                Assert.IsTrue(mw.OpenEnvSecurityOptionsCommand.CanExecute());
                Assert.IsFalse(mw.OpenTenantSecurityOptionsCommand.CanExecute());

                workFlowItem = TestDataUtility.CreateWorkFlowItemTestData(false);
                var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();

                TestUtilities.RegistMessageBoxServiceOfCommonOperate();
                TestUtilities.RegistCreateIntellisenseList();
                MainWindowViewModel mwvm = new MainWindowViewModel();
                TestUtilities.OpenWorkflowFromServer(workflowToOpen, mwvm);

                Assert.IsFalse(mwvm.NewWorkflowCommand.CanExecute());
                Assert.IsFalse(mwvm.SaveFocusedWorkflowCommand.CanExecute("ToServer"));
                Assert.IsFalse(mwvm.CopyProjectCommand.CanExecute());
                Assert.IsTrue(mwvm.DeleteProjectCommand.CanExecute());
                Assert.IsFalse(mwvm.ChangeAuthorCommand.CanExecute());
                Assert.IsTrue(mwvm.OpenWorkflowCommand.CanExecute("ToServer"));
                Assert.IsFalse(mwvm.UploadAssemblyCommand.CanExecute());
                Assert.IsTrue(mwvm.SelectAssemblyAndActivityCommand.CanExecute());
                Assert.IsFalse(mwvm.PublishCommand.CanExecute());
                Assert.IsFalse(mwvm.CompileCommand.CanExecute());
                Assert.IsTrue(mwvm.OpenMarketplaceCommand.CanExecute());
                Assert.IsTrue(mwvm.OpenEnvSecurityOptionsCommand.CanExecute());
                Assert.IsFalse(mwvm.OpenTenantSecurityOptionsCommand.CanExecute());
                mwvm.WorkflowItems.FirstOrDefault().Close();
            }
            catch (Exception e)
            {
                Assert.Fail("Fail Method:{0} \n Fail Info:{1}", MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                TestDataUtility.ClearTestWorkflowFromDB(workFlowItem);
            }
        }

        [WorkItem(405144)]
        [Description("Verify delete workflow")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void VerifyDeleteWorkflow()
        {
            TestUtilities.RegistLoginUserRole(Role.Admin);
            string workFlowItem = string.Empty;
            workFlowItem = TestDataUtility.CreateWorkFlowItemTestData(false);
            var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();

            TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            TestUtilities.RegistCreateIntellisenseList();
            MainWindowViewModel mwvm = new MainWindowViewModel();
            TestUtilities.OpenWorkflowFromServer(workflowToOpen, mwvm);
            Assert.IsTrue(mwvm.WorkflowItems.Count > 0);
            
            mwvm.DeleteProjectCommand.Execute();
            Assert.IsTrue(mwvm.WorkflowItems.Count == 0);
            workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();
            Assert.IsNull(workflowToOpen);

        }

        [WorkItem(405145)]
        [Description("Verify copy workflow")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void VerifyCopyWorkflow()
        {
            TestUtilities.RegistLoginUserRole(Role.Author);
            string workFlowItem = string.Empty;
            workFlowItem = TestDataUtility.CreateWorkFlowItemTestData(false);
            var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();

            TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            TestUtilities.RegistCreateIntellisenseList();
            MainWindowViewModel mwvm = new MainWindowViewModel();
            TestUtilities.OpenWorkflowFromServer(workflowToOpen, mwvm);
            Assert.IsTrue(mwvm.WorkflowItems.Count > 0);

            var viewModel = new CopyCurrentProjectViewModel(mwvm.FocusedWorkflowItem);
            //viewModel.NewName = workFlowItem+ "_Copy";
            viewModel.CopyTo = Env.Test;
            viewModel.WorkflowTemplate = viewModel.WorkflowTemplates.FirstOrDefault();
            viewModel.CopyProjectCommand.Execute();
            Assert.IsTrue(mwvm.WorkflowItems.Count == 1);
            int count = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem+"_Copy",Env.Test).Count;
            Assert.IsTrue(count==1);
            mwvm.WorkflowItems.FirstOrDefault().Close();
        }

        [WorkItem(405147)]
        [Description("Verify change workflow author")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void VerifyChangeWFAuthor()
        {
            TestUtilities.RegistLoginUserRole(Role.Admin);
            string workFlowItem = string.Empty;
            workFlowItem = TestDataUtility.CreateWorkFlowItemTestData(false);
            var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();
            string oldauthor = workflowToOpen.InInsertedByUserAlias;
            TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            TestUtilities.RegistCreateIntellisenseList();
            MainWindowViewModel mwvm = new MainWindowViewModel();
            TestUtilities.OpenWorkflowFromServer(workflowToOpen, mwvm);
            Assert.IsTrue(mwvm.WorkflowItems.Count > 0);

            var viewModel = new ChangeAuthorViewModel(mwvm.FocusedWorkflowItem);
            System.DirectoryServices.AccountManagement.Principal author = viewModel.AvaliableAuthors.FirstOrDefault();
            viewModel.TargetAuthor = author;
           // viewModel.ChangeAuthorCommand.Execute();
            WorkflowsQueryServiceUtility.UsingClient(client =>
            {
                ChangeAuthorRequest request = new ChangeAuthorRequest();
                request.SetIncaller();
                request.Name = workFlowItem;
                request.Version = workflowToOpen.Version;
                request.AuthorAlias = author.SamAccountName;
                request.Environment = workflowToOpen.Environment;
                ChangeAuthorReply reply = null;
                reply = client.ChangeAuthor(request);
            });
           var workflowToOpen2 = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();
           Assert.IsTrue(workflowToOpen2.InInsertedByUserAlias!=workflowToOpen.InInsertedByUserAlias);
           mwvm.WorkflowItems.FirstOrDefault().Close();
        }

        [WorkItem(405149)]
        [Description("Verify move project")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        public void VerifyMoveProject()
        {
            TestUtilities.RegistLoginUserRole(Role.Admin);
            string workFlowItem = string.Empty;
            workFlowItem = TestDataUtility.CreateWorkFlowItemTestData(false);
            var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();

            TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            TestUtilities.RegistCreateIntellisenseList();
            MainWindowViewModel mwvm = new MainWindowViewModel();
            TestUtilities.OpenWorkflowFromServer(workflowToOpen, mwvm);
            Assert.IsTrue(mwvm.WorkflowItems.Count > 0);
            
            var viewModel = new MoveProjectViewModel(mwvm.FocusedWorkflowItem);
            Assert.IsTrue(viewModel.CurrentStatus==Env.Dev);
            viewModel.NextStatus = Env.Test;
            viewModel.WorkflowTemplate = viewModel.WorkflowTemplates.FirstOrDefault();
            viewModel.MoveProjectCommand.Execute();
            Assert.IsTrue(mwvm.WorkflowItems.Count == 1);
            var workflowToOpen2= TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();
            Assert.IsTrue(mwvm.WorkflowItems.First().Env==Env.Test);
            mwvm.WorkflowItems.FirstOrDefault().Close();
        }

    }
}
