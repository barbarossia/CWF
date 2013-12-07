using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.CompositeActivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.AddIns.CompositeActivity;
using Microsoft.DynamicImplementations;
using System.Activities.Presentation.Model;
using CMP = Microsoft.Support.Workflow.Authoring.CompositeActivity;
using CWF.DataContracts;
using Microsoft.VisualBasic.Activities;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using System.Threading;

namespace Microsoft.Support.Workflow.Authoring.Tests.MultipleAuthor
{
    [TestClass]
    public class MultipleAuthorServiceUnitTest
    {
        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-bobzh")]
        public void TestCanAssign()
        {
            var model = new MockModelItem(new TaskActivity());
            MultipleAuthorService.CanAssign(model);
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-bobzh")]
        public void TestCanUnassign()
        {
            var task = new TaskActivity()
            {
                Status = TaskActivityStatus.Editing,
            };

            var model = new MockModelItem(task);
            MultipleAuthorService.CanUnassign(model);
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-bobzh")]
        public void TestCanMerge()
        {
            var task = new TaskActivity()
            {
                Status = TaskActivityStatus.CheckedIn,
            };

            var model = new MockModelItem(task);
            MultipleAuthorService.CanMerge(model);
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-bobzh")]
        public void TestAssign()
        {
            var activity = new Sequence();

            var task = new TaskActivity()
            {
                TaskBody = activity,
            };

            var model = new MockModelItem(activity);
            using (var service = new ImplementationOfType(typeof(CMP.CompositeService)))
            using (var ts = new ImplementationOfType(typeof(TaskService)))
            {
                service.Register(() => CMP.CompositeService.UpdateModelItem(Argument<ModelItem>.Any, Argument<Activity>.Any)).Return(null);
                ts.Register(() => TaskService.CreateTaskActivity(Argument<Activity>.Any)).Return(task);
                MultipleAuthorService.Assign(model);
            }
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-bobzh")]
        public void FindTaskIsNull()
        {
            using (var wd = new Implementation<WorkflowDesigner>())
            using (var e = new ImplementationOfType(typeof(CMP.ExtensionMethods)))
            {
                e.Register(() => CMP.ExtensionMethods.GetRoot(wd.Instance)).Return(null);

                MultipleAuthorService.FindTaskActivity(wd.Instance);
            }
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-bobzh")]
        public void GetTasks()
        {
            var taskBody = new Sequence();

            var task = new TaskActivity()
            {
                Alias = "v-bobzh",
                DisplayName = "Test",
                Status = TaskActivityStatus.Assigned,
                TaskId = Guid.NewGuid(),
                TaskBody = taskBody
            };

            using (var wd = new Implementation<WorkflowDesigner>())
            using (var service = new ImplementationOfType(typeof(ModelItemService)))
            using (var e = new ImplementationOfType(typeof(CMP.ExtensionMethods)))
            using (var cs = new ImplementationOfType(typeof(CompositeService)))
            using (var ar = new ImplementationOfType(typeof(ArgumentService)))
            using (var xamlService = new ImplementationOfType(typeof(XamlService)))
            {
                var model = new MockModelItem(task);

                service.Register(() => ModelItemService.Find(Argument<ModelItem>.Any, Argument<Predicate<Type>>.Any)).Return(new List<ModelItem>() { model });

                //mock GetXamlOfTaskBody
                var ab = new ActivityBuilder() { Name = "Test" };
                var root = new MockModelItem(ab);

                e.Register(() => CMP.ExtensionMethods.GetRoot(wd.Instance)).Return(root);
                var bodyWd = new Implementation<WorkflowDesigner>();

                cs.Register(() => CompositeService.CreateWorkflowDesigner(taskBody, ab.Name)).Return(bodyWd.Instance);

                var body = new MockModelItem(null);
                e.Register(() => CMP.ExtensionMethods.GetRoot(wd.Instance)).Return(root);
                e.Register(() => CMP.ExtensionMethods.GetRoot(bodyWd.Instance)).Return(body);

                ar.Register(() => ArgumentService.AddArguments(Argument<ModelItem>.Any, Argument<ModelItem>.Any)).Return();

                var va = new Variable<string>("Argument");

                var vList = new List<Variable>() { va };
                ar.Register(() => ArgumentService.GetAvailableVariables(Argument<ModelItem>.Any)).Return(vList);
                ar.Register(() => ArgumentService.AddArguments(body, vList.Select(v => new DynamicActivityProperty()
                {
                    Name = v.Name,
                    Type = typeof(InOutArgument<>).MakeGenericType(v.Type),
                }))).Return();

                string xaml = "XAML";
                xamlService.Register(() => XamlService.CompilableXaml(bodyWd.Instance)).Return(xaml);

                MultipleAuthorService.GetTasks(wd.Instance);
            }
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-bobzh")]
        public void TestSetNewTasksToAssigned()
        {
            Guid id = Guid.NewGuid();
            var task = new TaskActivity()
            {
                TaskId = id,
            };

            var taskModel = CreateTaskAndPropertyModelItem(task);

            var tasks = new List<ModelItem>() { taskModel };

            UsingFindTask(
                tasks, 
                (wd) => MultipleAuthorService.SetNewTasksToAssigned(wd, new[] { id }));
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-bobzh")]
        public void TestRollbackAssignedTasks()
        {
            Guid id = Guid.NewGuid();
            var task = new TaskActivity()
            {
                TaskId = id,
            };

            var taskModel = CreateTaskAndPropertyModelItem(task);
            var tasks = new List<ModelItem>() { taskModel };

            UsingFindTask(
                tasks,
                (wd) => MultipleAuthorService.RollbackAssignedTasks(wd, new[] { id }));
       }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-bobzh")]
        public void TestGetTaskItems()
        {
            var taskModel = new MockModelItem(new TaskActivity());
            var tasks = new List<ModelItem>() { taskModel };

            UsingFindTask(
                tasks,
                (wd) => MultipleAuthorService.GetTaskItems(wd));

        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-bobzh")]
        public void TestFinishTaskAssigned()
        {
            var task = new TaskActivity()
            {
                Alias = "v-bobzh",
                DisplayName = "Test",
                Status = TaskActivityStatus.New,
                TaskId = Guid.NewGuid(),
                TaskBody = new Sequence()
            };

            var taskModel = CreateTaskAndPropertyModelItem(task);

            var tasks = new List<ModelItem>() { taskModel };

            using (var wd = new Implementation<WorkflowDesigner>())
            using (var e = new ImplementationOfType(typeof(CMP.ExtensionMethods)))
            using (var service = new ImplementationOfType(typeof(ModelItemService)))
            using (var cs = new ImplementationOfType(typeof(CompositeService)))
            {
                var root = new MockModelItem(null);
                e.Register(() => CMP.ExtensionMethods.GetRoot(wd.Instance)).Return(root);
                service.Register(() => ModelItemService.Find(Argument<ModelItem>.Any, Argument<Predicate<Type>>.Any)).Return(tasks);

                cs.Register(() => CompositeService.UpdateModelItem(Argument<ModelItem>.Any, Argument<object>.Any)).Return(null);
                MultipleAuthorService.FinishTaskAssigned(wd.Instance);
            }
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-bobzh")]
        public void TestGetLastVersion()
        {
            var task = new TaskActivity()
            {
                TaskId = Guid.NewGuid(),
            };

            var sourceModel = CreateTaskAndPropertyModelItem(task);
            var lvAb = new ActivityBuilder();
            lvAb.Implementation = new Sequence();

            var rootAb = new ActivityBuilder();
            var taskModel = new MockModelItem(lvAb);
            var rootModel = new MockModelItem(rootAb);

            TaskActivityDC dc = new TaskActivityDC()
            {
                Guid = task.TaskId,
                Activity = new StoreActivitiesDC()
                {
                    Xaml = "XAML"
                }
            };
            var dcList = new[] { dc }.ToList();
            DynamicActivityProperty arg = new DynamicActivityProperty() { Name = "Argument" };
            var argList = new[] { arg }.ToList();

            var va = new Variable<string>("Argument");
            var vaList = new[] { va }.ToList();

            using (var wd = new Implementation<WorkflowDesigner>())
            using (var vm = new Implementation<WorkflowEditorViewModel>())
            using (var e = new ImplementationOfType(typeof(CMP.ExtensionMethods)))
            using (var taskService = new ImplementationOfType(typeof(TaskService)))
            using (var compositeService = new ImplementationOfType(typeof(CompositeService)))
            using (var arService = new ImplementationOfType(typeof(ArgumentService)))
            {
                e.Register(() => CMP.ExtensionMethods.GetRoot(wd.Instance)).Return(rootModel);
                vm.Register(inst => inst.WorkflowDesigner).Return(wd.Instance);
                vm.Register(inst => inst.DownloadTaskDependency(dcList)).Return();

                //GetLastVersionActivity
                taskService.Register(() => TaskService.GetLastVersionTaskActivityDC(Argument<Guid[]>.Any)).Return(dcList);
                compositeService.Register(() => CompositeService.CreateActivity(Argument<string>.Any)).Return(taskModel);

                //MergeTaskArgmentToParent
                arService.Register(() => ArgumentService.GetArgument(Argument<ActivityBuilder>.Any)).Return(argList);
                arService.Register(() => ArgumentService.GetAvailableVariables(sourceModel)).Return(vaList);

                //DoSameNameVariables
                arService.Register(() => ArgumentService.GetAvailableParent(sourceModel)).Return(null);
                compositeService.Register(() => CompositeService.UpdateModelItem(Argument<ModelItem>.Any, Argument<object>.Any)).Return(null);

                MultipleAuthorService.GetLastVersion(sourceModel, vm.Instance);
                MultipleAuthorService.GetAllLastVersion(new[] { sourceModel }.ToList(), vm.Instance);
            }
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-bobzh")]
        public void TestUnassignTask()
        {
            var task = new TaskActivity()
            {
                TaskBody = new Sequence()
            };

            var source = CreateTaskAndPropertyModelItem(task);
            using (var service = new ImplementationOfType(typeof(CompositeService)))
            {
                service.Register(() => CompositeService.UpdateModelItem(source, task.TaskBody)).Return(null);

                MultipleAuthorService.UnassignTask(source);
            }
        }

        [TestMethod]
        [TestCategory("Unit-Dif")]
        [Owner("v-bobzh")]
        public void TestUnassignTaskIfNull()
        {
            var task = new TaskActivity()
            {
                TaskBody = null
            };

            var source = CreateTaskAndPropertyModelItem(task);
            using (var service = new ImplementationOfType(typeof(CompositeService)))
            {
                service.Register(() => CompositeService.DeleteModelItem(source)).Return();

                MultipleAuthorService.UnassignTask(source);
            }
        }

        private void UsingFindTask(List<ModelItem> tasks, Action<WorkflowDesigner> action)
        {
            using (var wd = new Implementation<WorkflowDesigner>())
            using (var e = new ImplementationOfType(typeof(CMP.ExtensionMethods)))
            using (var service = new ImplementationOfType(typeof(ModelItemService)))
            {
                var root = new MockModelItem(null);
                e.Register(() => CMP.ExtensionMethods.GetRoot(wd.Instance)).Return(root);
                service.Register(() => ModelItemService.Find(Argument<ModelItem>.Any, Argument<Predicate<Type>>.Any)).Return(tasks);

                action(wd.Instance);
            }
        }

        private MockModelItem CreateTaskAndPropertyModelItem(TaskActivity task)
        {
            var prop = new MockModelProperty();

            var taskModel = new MockModelItem(task);
            var propCollection = new MockModelPropertyCollection(taskModel);
            propCollection.CreateModelPropery("Status", prop);

            taskModel.SetProperty(propCollection);
            return taskModel;
        }

    }
}
