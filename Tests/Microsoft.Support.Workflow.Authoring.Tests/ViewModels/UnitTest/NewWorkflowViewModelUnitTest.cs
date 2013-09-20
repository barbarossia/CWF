using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Common;
using CWF.DataContracts;
using System.Activities.Statements;
using System.ServiceModel;
using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels
{
    [TestClass]
    public class NewWorkflowViewModelUnitTest
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [WorkItem(322312)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void NewWorkflow_PropertyChangedNotificationsAreRaised()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var client = new Implementation<WorkflowsQueryServiceClient>())
                {
                    client.Register(inst => inst.WorkflowTypeGet(Argument<WorkflowTypesGetRequestDC>.Any)).Execute(() =>
                    {
                        WorkflowTypeGetReplyDC replyDC = new WorkflowTypeGetReplyDC();
                        replyDC.WorkflowActivityType = new List<WorkflowTypesGetBase>() 
                    {
                        new WorkflowTypesGetBase(){WorkflowTemplateId=1,Name="myworkflow"},
                    };
                        return replyDC;

                    });
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                    var vm = new NewWorkflowViewModel();
                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "WorkflowName", () => vm.WorkflowName = "myworkflow");
                    Assert.AreEqual(vm.WorkflowName, "myworkflow");
                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "WorkflowClassName", () => vm.WorkflowClassName = "workflowclassname");
                    Assert.AreEqual(vm.WorkflowClassName, "workflowclassname");
                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "CreatedItem", () => vm.CreatedItem = null);
                    Assert.AreEqual(vm.CreatedItem, null);
                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectWorkflowTemplates", () => vm.SelectWorkflowTemplates = null);
                    Assert.AreEqual(vm.SelectWorkflowTemplates, null);
                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedWorkflowTemplateItem", () => vm.SelectedWorkflowTemplateItem = null);
                    Assert.AreEqual(vm.SelectedWorkflowTemplateItem, null);
                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "IsCreatingBlank", () => vm.IsCreatingBlank = true);
                    Assert.AreEqual(vm.IsCreatingBlank, true);
                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "IsInitialized", () => vm.IsInitialized = true);
                    Assert.AreEqual(vm.IsInitialized, true);
                    vm.IsCreatingBlank = false;
                    vm.Validate();
                    Assert.IsFalse(vm.IsValid);
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                }
            });
        }

        [WorkItem(322331)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void NewWorkflow_VerifyCreateWorkflowCanExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
               {
                   using (var client = new Implementation<WorkflowsQueryServiceClient>())
                   {
                       client.Register(inst => inst.WorkflowTypeGet(Argument<WorkflowTypesGetRequestDC>.Any)).Execute(() =>
                       {
                           WorkflowTypeGetReplyDC replyDC = new WorkflowTypeGetReplyDC();
                           replyDC.WorkflowActivityType = new List<WorkflowTypesGetBase>() 
                            {
                                new WorkflowTypesGetBase(){WorkflowTemplateId=1,Name="myworkflow"},
                            };
                           return replyDC;
                       });
                       WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                       var vm = new NewWorkflowViewModel();
                       vm.IsInitialized = true;
                       vm.IsValid = true;
                       vm.SelectedWorkflowTemplateItem = new WorkflowTemplateItem(1, "MetaData");
                       vm.IsCreatingBlank = true;
                       vm.WorkflowName = "myworkflow";
                       PrivateObject pv = new PrivateObject(vm);
                       object result = pv.Invoke("CanExecuteCreateWorkflowItem");
                       Assert.IsTrue(Convert.ToBoolean(result));
                       WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                   }
               });
        }

        [WorkItem(325751)]
        [Owner("v-kason1")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void NewWorkflow_VerifyGetWorkflowTemplateActivityWithException()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var client = new Implementation<WorkflowsQueryServiceClient>())
                {
                    client.Register(inst => inst.WorkflowTypeGet(Argument<WorkflowTypesGetRequestDC>.Any)).Execute(() =>
                    {
                        WorkflowTypeGetReplyDC replyDC = new WorkflowTypeGetReplyDC();
                        replyDC.WorkflowActivityType = new List<WorkflowTypesGetBase>() 
                    {
                        new WorkflowTypesGetBase(){WorkflowTemplateId=1,Name="myworkflow"},
                    };
                        replyDC.StatusReply = new StatusReplyDC() { Errorcode = 0 };
                        return replyDC;

                    });
                    List<StoreActivitiesDC> storeActivityDC = new List<StoreActivitiesDC> 
                    {
                        new StoreActivitiesDC()
                        {
                            Name="mytemplate",
                            StatusReply = new StatusReplyDC(){Errorcode =0},
                            ActivityLibraryId=1,
                            Version = "1.0.0.0",
                        },
                    };
                    client.Register(inst => inst.StoreActivitiesGet(Argument<StoreActivitiesDC>.Any)).Return(storeActivityDC);
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                    UIHelper.IsInTesting = true;

                    var vm = new NewWorkflowViewModel();
                    vm.SelectedWorkflowTemplateItem = null;

                    try { vm.GetWorkflowTemplateActivity(client.Instance); }
                    catch (Exception ex)
                    {
                        Assert.IsTrue(ex is ArgumentNullException);
                    }

                    vm.SelectedWorkflowTemplateItem = new WorkflowTemplateItem(1, "mytemplate");

                    //verify exception when get template
                    client.Register(inst => inst.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any)).Execute(() =>
                    {
                        List<ActivityLibraryDC> dcs = null;
                        if (dcs == null)
                            throw new FaultException<ServiceFault>(new ServiceFault(), "reason");
                        return dcs;
                    });
                    //WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                    try { vm.GetWorkflowTemplateActivity(client.Instance); }
                    catch (Exception ex)
                    {
                        Assert.IsTrue(ex is CommunicationException);
                    }

                    client.Register(inst => inst.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any)).Execute(() =>
                    {
                        List<ActivityLibraryDC> dcs = null;
                        if (dcs == null)
                            throw new FaultException<ValidationFault>(new ValidationFault(), "reason");
                        return dcs;
                    });
                    //WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                    try { vm.GetWorkflowTemplateActivity(client.Instance); }
                    catch (Exception ex)
                    {
                        Assert.IsTrue(ex is BusinessValidationException);
                    }

                    client.Register(inst => inst.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any)).Execute(() =>
                    {
                        List<ActivityLibraryDC> dcs = null;
                        if (dcs == null)
                            throw new Exception("reason");
                        return dcs;
                    });
                    //WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                    try { vm.GetWorkflowTemplateActivity(client.Instance); }
                    catch (Exception ex)
                    {
                        Assert.IsTrue(ex is CommunicationException);
                    }
                }
            });
        }

        [WorkItem(325750)]
        [Owner("v-kason1")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void NewWorkflow_VerifyGetListOfTemplates()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var client = new Implementation<WorkflowsQueryServiceClient>())
                {
                    client.Register(inst => inst.WorkflowTypeGet(Argument< WorkflowTypesGetRequestDC>.Any)).Execute(() =>
                    {
                        WorkflowTypeGetReplyDC replyDC = new WorkflowTypeGetReplyDC();
                        replyDC.WorkflowActivityType = new List<WorkflowTypesGetBase>() 
                    {
                        new WorkflowTypesGetBase(){WorkflowTemplateId=1,Name="myworkflow"},
                    };
                        replyDC.StatusReply = new StatusReplyDC() { Errorcode = 0 };
                        return replyDC;

                    });
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;
                    var vm = new NewWorkflowViewModel();
                    Assert.AreEqual(vm.SelectWorkflowTemplates.Count, 1);
                }
                WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
            });
        }

        // Create WorkflowItems lazily to avoid creating lots of WorkflowDesigners. 
        private WorkflowItem validWorkflowItem;
        private WorkflowItem ValidWorkflowItem
        {
            get
            {
                if (validWorkflowItem == null)
                    validWorkflowItem = new WorkflowItem("MyWorkflow", "WF", (new Sequence()).ToXaml(), string.Empty);
                validWorkflowItem.Env = Authoring.AddIns.Data.Env.Dev;
                Assert.IsTrue(validWorkflowItem.IsValid, "Setup error! validWorkflowItem should be valid XAML for workflow.");
                return validWorkflowItem;
            }
        }

        [WorkItem(322333)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void NewWorkflow_VerifyCreateWorkflowCommandExecute()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var client = new Implementation<WorkflowsQueryServiceClient>())
                {
                    List<StoreActivitiesDC> storeActivityDC = new List<StoreActivitiesDC> 
                    {
                        new StoreActivitiesDC()
                        {
                            Name="mytemplate",
                            StatusReply = new StatusReplyDC(){Errorcode =0},
                            ActivityLibraryId=1,
                            Version = "1.0.0.0",
                            Environment = "dev"
                        },
                    };
                    TestUtilities.RegistLoginUserRole(Role.Admin);
                    ActivityLibraryDC library = new ActivityLibraryDC();
                    library.VersionNumber = "1.0.0.0";
                    library.Name = "mytemplate";
                    library.Environment = "dev";

                    client.Register(inst => inst.WorkflowTypeGet(Argument< WorkflowTypesGetRequestDC>.Any)).Execute(() =>
                    {
                        WorkflowTypeGetReplyDC replyDC = new WorkflowTypeGetReplyDC();
                        replyDC.WorkflowActivityType = new List<WorkflowTypesGetBase>() 
                        {
                            new WorkflowTypesGetBase(){WorkflowTemplateId=1,Name="myworkflow"},
                        };
                        return replyDC;

                    });
                    client.Register(inst => inst.StoreActivitiesGet(Argument<StoreActivitiesDC>.Any)).Return(storeActivityDC);
                    client.Register(inst => inst.ActivityLibraryGet(Argument<ActivityLibraryDC>.Any)).Return(new List<ActivityLibraryDC> { library });
                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => client.Instance;

                    //creat blank workflow
                    var vm = new NewWorkflowViewModel();
                    vm.IsCreatingBlank = true;
                    vm.WorkflowName = "myworkflow";
                    vm.WorkflowClassName = "myclassname";
                    vm.SelectedLocation = Env.Dev;
                    vm.CreateWorkflowItem.Execute();
                    Assert.AreEqual(vm.CreatedItem.IsSavedToServer, false);
                    Assert.AreEqual(vm.CreatedItem.CachingStatus, CachingStatus.None);

                    //throw exception when no workflowType is selected
                    vm.IsCreatingBlank = false;
                    vm.SelectedWorkflowTemplateItem = null;
                    try { vm.CreateWorkflowItem.Execute(); }
                    catch (Exception ex)
                    {
                        Assert.IsTrue(ex is ArgumentNullException);
                    }

                    //create new workflow by one template
                    vm.SelectedWorkflowTemplateItem = new WorkflowTemplateItem(1, "mytemplate");
                    using (var cach = new ImplementationOfType(typeof(Caching)))
                    {
                        cach.Register(() => Caching.ComputeDependencies(client.Instance, Argument<ActivityAssemblyItem>.Any))
                            .Return(new List<ActivityAssemblyItem>());

                        using (var translator = new ImplementationOfType(typeof(DataContractTranslator)))
                        {
                            bool result = false;
                            translator.Register(() => DataContractTranslator.StoreActivitiyDCToWorkflowItem(Argument<StoreActivitiesDC>.Any, Argument<ActivityAssemblyItem>.Any, null, false)).Execute(() =>
                            {
                                result = true;
                                return this.ValidWorkflowItem;
                            });
                            vm.CreateWorkflowItem.Execute();
                            Assert.IsFalse(result);
                            Assert.AreEqual(vm.CreatedItem.IsOpenFromServer, false);
                            Assert.AreEqual(vm.CreatedItem.WorkflowType, "mytemplate");
                        }
                    }

                    WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
                }
            });
        }
    }
}
