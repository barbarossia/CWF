using System;
using System.Activities.Statements;
using System.IO;
using System.Linq;
using System.Reflection;
using AuthoringToolTests.Services;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;
using System.Security.Principal;
using Microsoft.Support.Workflow.Authoring.Security;
using System.Threading;
using Microsoft.Support.Workflow.Authoring;
using System.Windows;
using Microsoft.Support.Workflow.Authoring.Tests.DataAccess;
using Microsoft.Support.Workflow.Authoring.CompositeActivity;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Activities.Presentation;
using Microsoft.Support.Workflow.Authoring.Behaviors;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;

namespace Authoring.Tests.Functional
{
    /// <summary>
    /// Contains tests for MainWindowViewModel
    /// </summary>
    [TestClass]
    public class MainWindowViewModelFunctionalTest
    {
        private const string TESTOWNER = "v-xtong";
        private string test001dllName = @"TestData\test001.dll";
        private string test001Path;
        private TestContext testContextInstance;
        /// <summary>
        /// Creates a unique workflow name 
        /// </summary>
        private string UniqueWorkflowDisplayName
        {
            get
            {
                return "TestAutomation" + DateTime.Now.ToString("mmddyyyyhhMMss");
            }
        }

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        #region test initiliaze
        [TestInitialize]
        public void TestInitialize()
        {
            test001Path = Path.Combine(testContextInstance.DeploymentDirectory, test001dllName);
        }
        #endregion

        [Description("Needed to use the MainWindowViewModel class")]
        [ClassInitialize]
        public static void TestSetup(TestContext context)
        {
            TestUtilities.LoadSystemActivitesIntoCurrentAppDomain();

        }


        [WorkItem(27738)]
        [Description("Verify a workflow is updated in the database")]
        [Owner("v-billmi")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        [Ignore]
        public void VerifySaveToServerWithUpdate()
        {
            TestUtilities.RegistCreateIntellisenseList();
            using (new CachingIsolator())
            {
                string Xaml = TestUtilities.EmptyWorkFlowTemplateXamlCode;

                string wfName = TestUtilities.CreateUniqueWorkflowName("STSUpdate");
                WorkflowItem createdWorkflowItem = new WorkflowItem(wfName, "STSUpdateWFDispName", Xaml, "Workflow");

                TestUtilities.SaveWorkflow(createdWorkflowItem, Locations.ToServer.ToString());
                Assert.IsTrue(createdWorkflowItem.IsSavedToServer, "IsSavedToServer is not set to True for the workflowItem");

                // Actually checks the StoreActivities table in the database
                Assert.IsTrue(TestUtilities.WorkflowItemExistsInStoreActivitiesTable(createdWorkflowItem),
                                                            String.Format("{0} does not exist in the StoreActivities table", createdWorkflowItem.Name));

                StoreActivitiesDC workflowToOpen =
                    TestUtilities.GetCollectionFromStoreActivitiesTable().FirstOrDefault(saDC => saDC.ShortName == createdWorkflowItem.Name);

                Assert.IsNotNull(workflowToOpen, "Did not find workflowToOpen in Store Activities table.");

                MainWindowViewModel mw = new MainWindowViewModel();
                mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);

                // Create some new XAML
                string newXAML = TestUtilities.GenerateASequenceXmalCodeWithoutActivity(); ;

                Assert.AreNotEqual(mw.FocusedWorkflowItem.XamlCode, newXAML, "The XAML should be different");

                // Set newXAML to the workflow
                mw.FocusedWorkflowItem.XamlCode = newXAML;

                // Save the workflow to the DB
                mw.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());
                Assert.IsTrue(mw.FocusedWorkflowItem.IsSavedToServer, "IsSavedToServer should be true");

                StoreActivitiesDC workflowToCheckXAML =
                    TestUtilities.GetCollectionFromStoreActivitiesTable().FirstOrDefault(saDC => saDC.ShortName == createdWorkflowItem.Name && saDC.Version != createdWorkflowItem.Version);

                Assert.IsNotNull(workflowToCheckXAML, "Did not find workflowToCheckXAML in Store Activities table.");

                Assert.AreEqual(newXAML, workflowToCheckXAML.Xaml);
                mw.WorkflowItems.FirstOrDefault().Close();
            }
        }


        [WorkItem(266390)]
        [Description("Verify saving a workflow does not wipe out the tool box")]
        [Owner("v-beriva")]
        [TestCategory("Func-Dif-Full")]
        [TestMethod()]
        [Ignore]
        public void VerifySaveToServerWithDependenciesDoesntWipeOutToolbox()
        {
            using (new CachingIsolator())
            {
                using (var principal = new Implementation<WindowsPrincipal>())
                {
                    //principal.Register(p => p.IsInRole(AuthorizationService.AdminAuthorizationGroupName))
                    //          .Return(false);
                    //principal.Register(p => p.IsInRole(AuthorizationService.AuthorAuthorizationGroupName))
                    //    .Return(true);
                    Thread.CurrentPrincipal = principal.Instance;
                    string assembliesPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\TestData\\Assemblies\\";
                    string fullPath = assembliesPath + "OASP.Activities.dll";
                    Assert.AreEqual(0, Caching.ActivityAssemblyItems.Count, "Assembly Cache is not empty.");
                    Assert.IsTrue(File.Exists(fullPath));

                    MainWindowViewModel vmMainWindow = new MainWindowViewModel();

                    var vmImportAssembly = new ImportAssemblyViewModel(fullPath);


                    foreach (var assembly in vmImportAssembly.AssembliesToImport)
                    {
                        if (String.IsNullOrEmpty(assembly.Location))
                        {
                            assembly.Location = assembliesPath + assembly.Name + ".dll";
                        }
                    }
                    using (var imple = new ImplementationOfType(typeof(ExpressionEditorHelper)))
                    {
                        imple.Register(() => ExpressionEditorHelper.CreateIntellisenseList()).Execute<TreeNode>(() => { return null; });
                        vmImportAssembly.ImportCommandExecute();

                        //check for dependencies imported to the toolbox
                        Assert.AreEqual(vmImportAssembly.AssembliesToImport.Count, Caching.ActivityAssemblyItems.Count,
                                                                String.Format("Failed to import all the assemblies, Expected:{0} Actual:{1}", vmImportAssembly.AssembliesToImport.Count, Caching.ActivityAssemblyItems.Count));
                    }
                    using (var vm = new Implementation())
                    {
                        Application application = new Application();
                        application.MainWindow = new Window();
                        application.MainWindow.DataContext = new MainWindowViewModel();

                        vm.Register(() => App.Current).Return(application);
                        var vmNewWorkflow = new NewWorkflowViewModel();


                        vmNewWorkflow.IsCreatingBlank = true;
                        vmNewWorkflow.WorkflowName = UniqueWorkflowDisplayName;
                        vmNewWorkflow.WorkflowClassName = UniqueWorkflowDisplayName;
                        vmNewWorkflow.CreateWorkflowItem.Execute();

                        vmNewWorkflow.CreatedItem.OldVersion = vmNewWorkflow.CreatedItem.Version;
                        vmMainWindow.WorkflowItems.Add(vmNewWorkflow.CreatedItem);
                        vmMainWindow.FocusedWorkflowItem = vmMainWindow.WorkflowItems.Last();
                        vmMainWindow.SaveFocusedWorkflowCommand.Execute(Locations.ToServer.ToString());

                        Assert.IsTrue(vmMainWindow.WorkflowItems[0].IsSavedToServer, "IsSavedToServer is not set to True for the workflowItem");

                        Assert.AreEqual(vmImportAssembly.AssembliesToImport.Count, Caching.ActivityAssemblyItems.Count,
                                                                    String.Format("Failed to retain all the assemblies, Expected:{0} Actual:{1}", vmImportAssembly.AssembliesToImport.Count, Caching.ActivityAssemblyItems.Count));
                        try
                        {
                            DataCleanUp.ExecuteSqlQuery(DataCleanUp.StoreActivity, vmNewWorkflow.CreatedItem.Name);
                            DataCleanUp.ExecuteSqlQuery(DataCleanUp.ActivityLibraries, vmNewWorkflow.CreatedItem.Name);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(String.Format("Error during cleanup:{0}", ex.Message));
                        }
                    }

                }
            }
        }

        [WorkItem(266412)]
        [Description("Verify that opening a workflow from server does not wipe out the tool box")]
        [Owner("v-beriva")]
        [TestCategory("Func-NoDif-Smoke")]
        [TestMethod()]
        public void VerifyOpenFromServerWithDependenciesDoesntWipeOutToolbox()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            TestUtilities.RegistCreateIntellisenseList();
            using (new CachingIsolator())
            {
                string assembliesPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Data\\TestData\\Assemblies\\";
                string fullPath = assembliesPath + "OASP.Activities.dll";
                Assert.AreEqual(0, Caching.ActivityAssemblyItems.Count, "Assembly Cache is not empty.");
                Assert.IsTrue(File.Exists(fullPath));

                MainWindowViewModel vmMainWindow = new MainWindowViewModel();
                var vmImportAssembly = new ImportAssemblyViewModel(fullPath);

                foreach (var assembly in vmImportAssembly.AssembliesToImport)
                {
                    if (String.IsNullOrEmpty(assembly.Location))
                    {
                        assembly.Location = assembliesPath + assembly.Name + ".dll";
                    }
                }
                //TestUtilities.MockCreateIntellisenseList(() =>
                //{
                    vmImportAssembly.ImportCommandExecute();
                    var vmOpen = new OpenWorkflowFromServerViewModel();
                    WorkflowsQueryServiceUtility.UsingClient(client => vmOpen.LoadLiveData(client));

                    if (vmOpen.ExistingWorkflows.Count > 0)
                    {
                        vmOpen.SelectedWorkflow = vmOpen.ExistingWorkflows[0];
                        vmOpen.OpenSelectedWorkflowCommand.Execute();
                        var assembly = new ActivityAssemblyItem { Name = vmOpen.SelectedWorkflow.ActivityLibraryName, Version = Version.Parse(vmOpen.SelectedWorkflow.ActivityLibraryVersion) };
                        vmMainWindow.WorkflowItems.Add(DataContractTranslator.StoreActivitiyDCToWorkflowItem(vmOpen.SelectedWorkflow, assembly));
                    }
               // });

                Assert.AreEqual(vmImportAssembly.AssembliesToImport.Count, Caching.ActivityAssemblyItems.Count,
                                               String.Format("Failed to retain all the assemblies, Expected:{0} Actual:{1}", vmImportAssembly.AssembliesToImport.Count, Caching.ActivityAssemblyItems.Count));
            }
        }

        [WorkItem(330257)]
        [Description("Verify that project exproler loads a tree of activities when open a workflow")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void VerifyProjectExprolerLoadsTreeViewOpenWorkFlow()
        {
            var properties = new WorkFlowProperties
            {
                Status = "Private",
                Version = "0.0.0.1"
            };
            TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            TestUtilities.RegistLoginUserRole(Role.Author);
            TestUtilities.RegistCreateIntellisenseList();

            var workFlowItem = TestDataUtility.CreateWorkFlowItemTestData(false);
            var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();
            Assert.IsNotNull(workflowToOpen, "Did not find workflow in Store Activities table.");
            MainWindowViewModel mw = new MainWindowViewModel();
            mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
          
            //Assert.IsTrue(mw.FocusedWorkflowItem.WorkflowOutlineNodes.Count > 0);
            mw.WorkflowItems.FirstOrDefault().Close();

        }

        [WorkItem(330255)]
        [Description("Verify that project exproler loads a tree of activities when open a workflow")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        [Ignore]
        public void VerifyProjectExprolerTreeNodeRename()
        {
            TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            TestUtilities.RegistCreateIntellisenseList();
            var workFlowItem = TestDataUtility.CreateWorkFlowItemTestData();
            var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();
            Assert.IsNotNull(workflowToOpen, "Did not find workflow in Store Activities table.");
            MainWindowViewModel mw = new MainWindowViewModel();
            mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
            var peVM = (ProjectExplorerViewModel)mw.FocusedWorkflowItem.PEView.DataContext;
            var peVM2 = (ProjectExplorerViewModel)mw.FocusedWorkflowItem.WorkflowDesigner.ProjectExplorerView.DataContext;

            Assert.IsTrue(peVM.WorkflowOutlineNodes.Count > 0);
            peVM.SelectedWorkflowOutlineNode = peVM.WorkflowOutlineNodes.FirstOrDefault();
            string newname = TestUtilities.GenerateRandomString(10);
            peVM.SelectedWorkflowOutlineNode.NodeName = newname;
            //mw.FocusedWorkflowItem.SelectedWorkflowOutlineNode.Model.Properties["DisplayName"].SetValue(newname);
            //Assert.AreEqual(newname, mw.FocusedWorkflowItem.SelectedWorkflowOutlineNode.Model.Properties["DisplayName"].Value);
            Assert.AreEqual(newname, peVM.SelectedWorkflowOutlineNode.NodeName);
        //
        }

        [WorkItem(330256)]
        [Description("Verify that project exproler search fun")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        [Ignore]
        public void VerifyProjectExprolerSearch()
        {
            TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            TestUtilities.RegistCreateIntellisenseList();
            var workFlowItem = TestDataUtility.CreateWorkFlowItemTestData();
            var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();
            Assert.IsNotNull(workflowToOpen, "Did not find workflow in Store Activities table.");
            MainWindowViewModel mw = new MainWindowViewModel();
            mw = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw);
            //Assert.IsTrue(mw.FocusedWorkflowItem.WorkflowOutlineNodes.Count > 0);
            //mw.FocusedWorkflowItem.SelectedWorkflowOutlineNode = mw.FocusedWorkflowItem.WorkflowOutlineNodes.FirstOrDefault();
            //mw.FocusedWorkflowItem.IsSearchType = true;
            //mw.FocusedWorkflowItem.IsSearchTitle = true;
            //mw.FocusedWorkflowItem.IsSearchParameter = true;
            //mw.FocusedWorkflowItem.IsSearchWholeWorkflow = true;
            //mw.FocusedWorkflowItem.SearchText = "WriteLine";
            //mw.FocusedWorkflowItem.ExecuteSearch();
            //mw.FocusedWorkflowItem.SelectedWorkflowOutlineNode.NodeName = "WriteLine";
           // mw.FocusedWorkflowItem.PEView.DataContext.
        }


        [WorkItem(332316)]
        [Description("Verify user can expand the included activity and edit its parameters")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif3-Full")]
        [TestMethod()]
        [Ignore]
        public void VerifyAPEExpandIncludeActivityAndEdit()
        {
            //TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            //TestUtilities.Loadtest001Assembly();
            //var workFlowItem = TestDataUtility.CreateWorkFlowItemTestData();
            //var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();

            //MainWindowViewModel mw_Accessor = new MainWindowViewModel();
            //mw_Accessor = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw_Accessor);
            //mw_Accessor.FocusedWorkflowItem.XamlCode = TestUtilities.CreateCompositeWorkFlowTemplateXamlCode(workflowToOpen.Name);
            //mw_Accessor.FocusedWorkflowItem.WorkflowDesigner.RefreshDesignerFromXamlCode();
            //ModelItem root = mw_Accessor.FocusedWorkflowItem.GetModelService().Root;
            //ModelItem oldModelItem = root.GetModelService().Find(root, typeof(Activity)).FirstOrDefault(m => m.ItemType.Assembly.GetName().Name == "test001");
            //object newItem = oldModelItem.GetActivity();
            //CompositeService.UpdateModelItem(oldModelItem, newItem);

            //Assert.IsNotNull(mw_Accessor.FocusedWorkflowItem);
            //string xamlCode = mw_Accessor.FocusedWorkflowItem.XamlCode;
            //Assert.IsTrue(xamlCode.Contains("sap:WorkflowViewStateService.ViewState"));
            //Assert.IsTrue(xamlCode.Contains("mswac:ModelKey"));
            //Assert.IsTrue(xamlCode.Contains("Node"));
            ////clear the test data
            //TestDataUtility.ClearTestWorkflowFromDB(workflowToOpen.Name);
        }

        [WorkItem(332344)]
        [Description("Verify user can add reference activity and edit its parameters")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif3-Full")]
        [TestMethod()]
        [Ignore]
        public void VerifyAPEEditActivityParameters()
        {
            //TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            //TestUtilities.Loadtest001Assembly();
            //var workFlowItem = TestDataUtility.CreateWorkFlowItemTestData();
            //var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();

            //MainWindowViewModel mw_Accessor = new MainWindowViewModel();
            //mw_Accessor = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw_Accessor);

            //mw_Accessor.FocusedWorkflowItem.XamlCode = TestUtilities.CreateCompositeWorkFlowTemplateXamlCode(workflowToOpen.Name);
            //mw_Accessor.FocusedWorkflowItem.RefreshDesignerFromXamlCode();

            ////add test001_1
            //ModelItem root = mw_Accessor.FocusedWorkflowItem.WorkflowDesigner.GetModelService().Root;
            //ModelItem oldModelItem = root.GetModelService().Find(root, typeof(Activity)).FirstOrDefault(m => m.ItemType.Assembly.GetName().Name == "test001");
            //object newItem = oldModelItem.GetActivity();
            //CompositeService.UpdateModelItem(oldModelItem, newItem);

            ////add test001_2
            //mw_Accessor.FocusedWorkflowItem.XamlCode = TestUtilities.ReplacefirstMatch(mw_Accessor.FocusedWorkflowItem.XamlCode, "</Sequence>", "</Sequence><local:test001 />");
            //mw_Accessor.FocusedWorkflowItem.RefreshDesignerFromXamlCode();
            //ModelItem root2 = mw_Accessor.FocusedWorkflowItem.WorkflowDesigner.GetModelService().Root;
            //ModelItem oldModelItem2 = root2.GetModelService().Find(root2, typeof(Activity)).FirstOrDefault(m => m.ItemType.Assembly.GetName().Name == "test001");
            //object newItem2 = oldModelItem2.GetActivity();
            //CompositeService.UpdateModelItem(oldModelItem2, newItem2);

            ////edit test001_1
            //mw_Accessor.FocusedWorkflowItem.XamlCode = TestUtilities.ReplacefirstMatch(mw_Accessor.FocusedWorkflowItem.XamlCode, "<WriteLine>", "<WriteLine sap:VirtualizedContainerService.HintSize='211,61' Text='9999999999' TextWriter='{x:Null}'>");
            //mw_Accessor.FocusedWorkflowItem.RefreshDesignerFromXamlCode();
            //Assert.IsTrue(TestUtilities.CountStringAContainsStringB(mw_Accessor.FocusedWorkflowItem.XamlCode, "9999999999") == 1);
            ////update test001_1
            //ModelItem root3 = mw_Accessor.FocusedWorkflowItem.WorkflowDesigner.GetModelService().Root;
            //ModelItem selected = root3.GetModelService().Find(root3, typeof(Activity)).Where(k => k.GetActivity().DisplayName == "test001").FirstOrDefault(); ;


            //mw_Accessor.FocusedWorkflowItem.CompositeWorkflow.UpdateReference(selected);

            //Assert.IsNotNull(mw_Accessor.FocusedWorkflowItem);
            //string xamlCode = mw_Accessor.FocusedWorkflowItem.XamlCode;
            //Assert.IsTrue(xamlCode.Contains("sap:WorkflowViewStateService.ViewState"));
            //Assert.IsTrue(xamlCode.Contains("mswac:ModelKey"));
            //Assert.IsTrue(xamlCode.Contains("Node"));
            //Assert.IsTrue(TestUtilities.CountStringAContainsStringB(xamlCode, "9999999999") == 2);
            ////clear the test data
            //TestDataUtility.ClearTestWorkflowFromDB(workflowToOpen.Name);
        }

        [WorkItem(332534)]
        [Description("Verify saving workflow and don't change activity")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif3-Full")]
        [TestMethod()]
        [Ignore]
        public void VerifyAPESaveWorkFlowNotChangeActivity()
        {
            //TestUtilities.RegistUtilityGetCurrentWindow();
            ////TestUtilities.RegistWinPrincipalFunc(AuthorizationService.AuthorAuthorizationGroupName);
            //TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            //TestUtilities.Loadtest001Assembly();
            //TestDataUtility.CreateWorkFlowItemTestData(true, "test001");
            //var workFlowItem = TestDataUtility.CreateWorkFlowItemTestData();
            //var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();
            //Assert.AreEqual("1.0.0.0", workflowToOpen.Version);
            //var test001Pre = TestUtilities.GetStoreActivitiesFromeServerByName("test001").FirstOrDefault();

            //MainWindowViewModel mw_Accessor = new MainWindowViewModel();
            //mw_Accessor = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw_Accessor);
            //Assert.IsNotNull(mw_Accessor.FocusedWorkflowItem);
            //mw_Accessor.FocusedWorkflowItem.XamlCode = TestUtilities.CreateCompositeWorkFlowTemplateXamlCode(workflowToOpen.Name);
            //mw_Accessor.FocusedWorkflowItem.RefreshDesignerFromXamlCode();

            ////add test001_1
            //ModelItem root = mw_Accessor.FocusedWorkflowItem.WorkflowDesigner.GetModelService().Root;
            //ModelItem oldModelItem = root.GetModelService().Find(root, typeof(Activity)).FirstOrDefault(m => m.ItemType.Assembly.GetName().Name == "test001");
            //object newItem = oldModelItem.GetActivity();
            //CompositeService.UpdateModelItem(oldModelItem, newItem);

            //mw_Accessor.SaveFocusedWorkflowCommandExecute(Locations.ToServer.ToString());
            //var newworkflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).OrderByDescending(a=>a.ActivityLibraryId).FirstOrDefault();
            //Assert.AreEqual("1.0.0.1", newworkflowToOpen.Version);

            ////test001 dont'general new version or save.
            //var test001Aft = TestUtilities.GetStoreActivitiesFromeServerByName("test001").FirstOrDefault();
            //if (test001Pre == null)
            //    Assert.IsNull(test001Aft);
            //else
            //    Assert.AreEqual(test001Pre.Version, test001Aft.Version);
            ////clear the test data
            //TestDataUtility.ClearTestWorkflowFromDB(workflowToOpen.Name);
            //TestDataUtility.ClearTestWorkflowFromDB("test001");
        }

        [WorkItem(332539)]
        [Description("Verify saving workflow and changed activity")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif3-Full")]
        [TestMethod()]
        [Ignore]
        public void VerifyAPESaveWorkFlowAndChangedActivity()
        {
            //TestUtilities.RegistUtilityGetCurrentWindow();
            //TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            ////TestUtilities.RegistWinPrincipalFunc(AuthorizationService.AuthorAuthorizationGroupName);
            //TestUtilities.Loadtest001Assembly();
            //TestDataUtility.CreateWorkFlowItemTestData(true, "test001");
            //var workFlowItem = TestDataUtility.CreateWorkFlowItemTestData();
            //var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();
            //Assert.AreEqual("1.0.0.0", workflowToOpen.Version);
            //var test001Pre = TestUtilities.GetStoreActivitiesFromeServerByName("test001").FirstOrDefault();

            //MainWindowViewModel mw_Accessor = new MainWindowViewModel();
            //mw_Accessor = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw_Accessor);
            //Assert.IsNotNull(mw_Accessor.FocusedWorkflowItem);
            //mw_Accessor.FocusedWorkflowItem.XamlCode = TestUtilities.CreateCompositeWorkFlowTemplateXamlCode(workflowToOpen.Name);
            //mw_Accessor.FocusedWorkflowItem.RefreshDesignerFromXamlCode();

            ////add test001_1
            //ModelItem root = mw_Accessor.FocusedWorkflowItem.WorkflowDesigner.GetModelService().Root;
            //ModelItem oldModelItem = root.GetModelService().Find(root, typeof(Activity)).FirstOrDefault(m => m.ItemType.Assembly.GetName().Name == "test001");
            //object newItem = oldModelItem.GetActivity();
            //CompositeService.UpdateModelItem(oldModelItem, newItem);

            ////edit test001_
            //string newxamlCode = TestUtilities.ReplacefirstMatch(mw_Accessor.FocusedWorkflowItem.XamlCode, "<WriteLine>", "<WriteLine sap:VirtualizedContainerService.HintSize='211,61' Text='9999999999' TextWriter='{x:Null}'>");
            //newxamlCode = TestUtilities.RemovefirstMatch(newxamlCode, "<x:Null />");
            //newxamlCode = TestUtilities.RemovefirstMatch(newxamlCode, "<WriteLine.TextWriter>");
            //newxamlCode = TestUtilities.RemovefirstMatch(newxamlCode, "<x:Null />");
            //newxamlCode = TestUtilities.RemovefirstMatch(newxamlCode, "</WriteLine.TextWriter>");
            //mw_Accessor.FocusedWorkflowItem.XamlCode = newxamlCode;
            //mw_Accessor.FocusedWorkflowItem.RefreshDesignerFromXamlCode();
            //Assert.IsTrue(TestUtilities.CountStringAContainsStringB(mw_Accessor.FocusedWorkflowItem.XamlCode, "9999999999") == 1);

            //mw_Accessor.SaveFocusedWorkflowCommandExecute(Locations.ToServer.ToString());
            //var newworkflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).OrderByDescending(a => a.ActivityLibraryId).FirstOrDefault();
            //Assert.AreEqual("1.0.0.1", newworkflowToOpen.Version);

            ////test001 dont'general new version or save.
            //var test001Aft = TestUtilities.GetStoreActivitiesFromeServerByName("test001").FirstOrDefault();
            //if (test001Pre == null)
            //    Assert.IsNull(test001Aft);
            //else
            //    Assert.AreEqual(test001Pre.Version, test001Aft.Version);
            ////clear the test data
            //TestDataUtility.ClearTestWorkflowFromDB(workflowToOpen.Name);
            //TestDataUtility.ClearTestWorkflowFromDB("test001");
        }

        [WorkItem(332544)]
        [Description("Verify compile workflow and don't change activity")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        [Ignore]
        public void VerifyAPECompileWorkFlowNotChangeActivity()
        {
            ////TestUtilities.RegistWinPrincipalFunc(AuthorizationService.AuthorAuthorizationGroupName);
            //TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            //TestUtilities.Loadtest001Assembly();
            //TestDataUtility.CreateWorkFlowItemTestData(true, "test001");
            //var workFlowItem = TestDataUtility.CreateWorkFlowItemTestData();
            //var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();
            //Assert.AreEqual("1.0.0.0", workflowToOpen.Version);
            //var test001Pre = TestUtilities.GetStoreActivitiesFromeServerByName("test001").FirstOrDefault();

            //MainWindowViewModel mw_Accessor = new MainWindowViewModel();
            //mw_Accessor = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw_Accessor);
            //Assert.IsNotNull(mw_Accessor.FocusedWorkflowItem);
            //mw_Accessor.FocusedWorkflowItem.XamlCode = TestUtilities.CreateCompositeWorkFlowTemplateXamlCode(workflowToOpen.Name);
            //mw_Accessor.FocusedWorkflowItem.RefreshDesignerFromXamlCode();

            ////add test001_1
            //ModelItem root = mw_Accessor.FocusedWorkflowItem.WorkflowDesigner.GetModelService().Root;
            //ModelItem oldModelItem = root.GetModelService().Find(root, typeof(Activity)).FirstOrDefault(m => m.ItemType.Assembly.GetName().Name == "test001");
            //object newItem = oldModelItem.GetActivity();
            //CompositeService.UpdateModelItem(oldModelItem, newItem);

            //mw_Accessor.CompileCommand.Execute();
            //var newworkflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();
            //Assert.AreEqual("1.0.0.0", newworkflowToOpen.Version);


            ////test001 dont'general new version or save.
            //var test001Aft = TestUtilities.GetStoreActivitiesFromeServerByName("test001").FirstOrDefault();
            //if (test001Pre == null)
            //    Assert.IsNull(test001Aft);
            //else
            //    Assert.AreEqual(test001Pre.Version, test001Aft.Version);
            ////clear the test data
            //TestDataUtility.ClearTestWorkflowFromDB(workflowToOpen.Name);
            //TestDataUtility.ClearTestWorkflowFromDB("test001");
        }

        [WorkItem(332545)]
        [Description("Verify compile workflow and changed activity")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        [Ignore]
        public void VerifyAPECompileWorkFlowAndChangedActivity()
        {
            //TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            ////TestUtilities.RegistWinPrincipalFunc(AuthorizationService.AuthorAuthorizationGroupName);
            //TestUtilities.Loadtest001Assembly();
            //TestDataUtility.CreateWorkFlowItemTestData(true, "test001");
            //var workFlowItem = TestDataUtility.CreateWorkFlowItemTestData();
            //var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();
            //Assert.AreEqual("1.0.0.0", workflowToOpen.Version);
            //var test001Pre = TestUtilities.GetStoreActivitiesFromeServerByName("test001").FirstOrDefault();

            //MainWindowViewModel mw_Accessor = new MainWindowViewModel();
            //mw_Accessor = TestUtilities.OpenWorkflowFromServer(workflowToOpen, mw_Accessor);
            //Assert.IsNotNull(mw_Accessor.FocusedWorkflowItem);
            //mw_Accessor.FocusedWorkflowItem.XamlCode = TestUtilities.CreateCompositeWorkFlowTemplateXamlCode(workflowToOpen.Name);
            //mw_Accessor.FocusedWorkflowItem.RefreshDesignerFromXamlCode();

            ////add test001_1
            //ModelItem root = mw_Accessor.FocusedWorkflowItem.WorkflowDesigner.GetModelService().Root;
            //ModelItem oldModelItem = root.GetModelService().Find(root, typeof(Activity)).FirstOrDefault(m => m.ItemType.Assembly.GetName().Name == "test001");
            //object newItem = oldModelItem.GetActivity();
            //CompositeService.UpdateModelItem(oldModelItem, newItem);

            ////edit test001_
            //string newxamlCode = TestUtilities.ReplacefirstMatch(mw_Accessor.FocusedWorkflowItem.XamlCode, "<WriteLine>", "<WriteLine sap:VirtualizedContainerService.HintSize='211,61' Text='9999999999' TextWriter='{x:Null}'>");
            //newxamlCode = TestUtilities.RemovefirstMatch(newxamlCode, "<x:Null />");
            //newxamlCode = TestUtilities.RemovefirstMatch(newxamlCode, "<WriteLine.TextWriter>");
            //newxamlCode = TestUtilities.RemovefirstMatch(newxamlCode, "<x:Null />");
            //newxamlCode = TestUtilities.RemovefirstMatch(newxamlCode, "</WriteLine.TextWriter>");
            //mw_Accessor.FocusedWorkflowItem.XamlCode = newxamlCode;
            //mw_Accessor.FocusedWorkflowItem.RefreshDesignerFromXamlCode();
            //Assert.IsTrue(TestUtilities.CountStringAContainsStringB(mw_Accessor.FocusedWorkflowItem.XamlCode, "9999999999") == 1);

            //mw_Accessor.CompileCommand.Execute();
            //var newworkflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();
            //Assert.AreEqual("1.0.0.0", newworkflowToOpen.Version);

            ////test001 dont'general new version or save.
            //var test001Aft = TestUtilities.GetStoreActivitiesFromeServerByName("test001").FirstOrDefault();
            //if (test001Pre == null)
            //    Assert.IsNull(test001Aft);
            //else
            //    Assert.AreEqual(test001Pre.Version, test001Aft.Version);
            ////clear the test data
            //TestDataUtility.ClearTestWorkflowFromDB(workflowToOpen.Name);
            //TestDataUtility.ClearTestWorkflowFromDB("test001");
        }

        [WorkItem(332545)]
        [Description("Verify Foundry designer's color will change to gray when user click Print")]
        [Owner("v-allen")]
        [TestCategory("Func-NoDif2-Full")]
        [TestMethod()]
        [Ignore]
        public void VerifyDesignerChangeTograyWhenUserClickPrint()
        {
            TestUtilities.RegistMessageBoxServiceOfCommonOperate();
            TestUtilities.RegistCreateIntellisenseList();
            var workFlowItem = TestDataUtility.CreateWorkFlowItemTestData(false);
            var workflowToOpen = TestUtilities.GetStoreActivitiesFromeServerByName(workFlowItem).FirstOrDefault();

            MainWindowViewModel mwvm = new MainWindowViewModel();
            TestUtilities.OpenWorkflowFromServer(workflowToOpen, mwvm);
            Assert.IsNotNull(mwvm.FocusedWorkflowItem);
            mwvm.PrintCommand.Execute();
            //mwvm.FocusedWorkflowItem.PrintAll();
            Assert.AreEqual(PrintAction.PrintUserSelection, mwvm.FocusedWorkflowItem.PrintState);       
        }

      

    }
}

