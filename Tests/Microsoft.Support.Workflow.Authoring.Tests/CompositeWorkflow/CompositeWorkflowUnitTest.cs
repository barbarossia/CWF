using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation;
using System.Activities.Presentation.Hosting;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Activities;
using System.Windows;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.CompositeActivity;
using Microsoft.VisualBasic.Activities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Tests.HelpClass;
using System.Windows.Threading;

namespace Microsoft.Support.Workflow.Authoring.Tests.CompositeWorkflow
{
    /// <summary>
    /// Summary description for CompositeWorkflowUnitTest
    /// </summary>
    [TestClass]
    public class CompositeWorkflowUnitTest
    {
        private const string key = "Key";

        private Assembly test001;
        public Assembly Test001
        {
            get
            {
                if (test001 == null)
                {
                    test001 = LoadtestAssembly("test001");
                }
                return test001;
            }
        }

        private void ConfigWorkflowDesigner(WorkflowDesigner newWorkflowDesigner, IEnumerable<AssemblyName> referenceAssembly)
        {
            newWorkflowDesigner.Context.Items.SetValue(new AssemblyContextControlItem()
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
                    (referenceAssembly).ToList()
            });

        }

        private ActivityBuilder CreateWorkflow(Activity activity)
        {
            return new ActivityBuilder()
            {
                Name = "Test001",
                Implementation = activity,
            };
        }

        private WorkflowDesigner CreateWorkflowDesigner(string xaml, Assembly reference = null)
        {
            var newWorkflowDesigner = new WorkflowDesigner();
            List<AssemblyName> referenceAssembly = new List<AssemblyName>() { };
            var assembly = reference ?? Test001;

            referenceAssembly.Add(assembly.GetName());

            ConfigWorkflowDesigner(newWorkflowDesigner, referenceAssembly);
            newWorkflowDesigner.Text = xaml;
            newWorkflowDesigner.Load(); // initialize workflow based on Text property

            return newWorkflowDesigner;
        }

        private WorkflowDesigner CreateWorkflowDesigner(Activity activity)
        {
            var newWorkflowDesigner = new WorkflowDesigner();
            List<AssemblyName> referenceAssembly = new List<AssemblyName>() { Test001.GetName() };

            ConfigWorkflowDesigner(newWorkflowDesigner, referenceAssembly);
            newWorkflowDesigner.Load(CreateWorkflow(activity)); // initialize workflow based on Text property

            return newWorkflowDesigner;
        }

        private WorkflowDesigner CreateWorkflowDesigner(ActivityBuilder activity)
        {
            var newWorkflowDesigner = new WorkflowDesigner();
            List<AssemblyName> referenceAssembly = new List<AssemblyName>() { Test001.GetName() };

            ConfigWorkflowDesigner(newWorkflowDesigner, referenceAssembly);
            newWorkflowDesigner.Load(activity); // initialize workflow based on Text property

            return newWorkflowDesigner;
        }

        private Assembly LoadtestAssembly(string name)
        {
            return Assembly.LoadFrom(string.Format(@"testData\{0}.dll", name));
        }

        private ModelItem GetCompositeModel(WorkflowDesigner desinger, Assembly assembly = null)
        {
            Assembly match = assembly ?? Test001;
            return GetAllActivities(desinger.GetModelService().Root).First(mi => mi.ItemType.Assembly == match);
        }

        private List<ModelItem> GetModelsByName(WorkflowDesigner desinger, string name)
        {
            List<ModelItem> result = new List<ModelItem>();
            var atCollection = GetAllActivities(desinger.GetModelService().Root);
            foreach (var mi in atCollection)
            {
                if (mi.GetActivity().DisplayName == name)
                    result.Add(mi);
            }
            return result;
        }

        private ModelItem GetFirstModel(WorkflowDesigner desinger)
        {
            return GetAllActivities(desinger.GetModelService().Root).First();
        }

        private ModelItem GetRoot(WorkflowDesigner desinger)
        {
            return desinger.GetModelService().Root; ;
        }

        private object GetViewState(ModelItem model)
        {
            ViewStateService vss = model.GetEditingContext().Services.GetService<ViewStateService>();
            return vss.RetrieveViewState(model, key);
        }

        private void StoreViewState(ModelItem model, object value)
        {
            ViewStateService vss = model.GetEditingContext().Services.GetService<ViewStateService>();
            vss.StoreViewState(model, key, value);
        }

        private NodeKey CreateKey(AssemblyName assemblyName, CompositeNode parent, bool isSuccessfullyApplied = false)
        {
            return new NodeKey()
            {
                Key = Guid.NewGuid(),
                Name = assemblyName.FullName,
                Parent = parent != null ? parent.Key.Key : Guid.Empty,
                IsSuccessfullyApplied = isSuccessfullyApplied,
            };
        }

        private ModelKey CreateModelKey()
        {
            return new ModelKey()
            {
                Node = Guid.NewGuid(),
                Key = Guid.NewGuid(),
            };
        }

        private Sequence GenerateASequenceWithTwoActivities()
        {
            Sequence sequence = new Sequence
            {
                DisplayName = "Test",
                Activities = {  
                                new WriteLine { DisplayName="Test1", Text = "Hello1" }, 
                                new WriteLine { DisplayName="Test1", Text = "Hello2" }    
                             }
            };
            return sequence;
        }

        private ActivityBuilder GenerateASequenceWithArgument()
        {
            ActivityBuilder ab = new ActivityBuilder();
            ab.Name = "Add";
            ab.Properties.Add(new DynamicActivityProperty { Name = "Operand1", Type = typeof(InArgument<int>) });
            ab.Properties.Add(new DynamicActivityProperty { Name = "Operand2", Type = typeof(InArgument<int>) });
            ab.Implementation = new Sequence
            {
                Activities = 
                {
                    new WriteLine
                    {
                        Text = new VisualBasicValue<string>("Operand1.ToString() + \" + \" + Operand2.ToString()")
                    },
                    new Assign<int>
                    {
                        To = new ArgumentReference<int> { ArgumentName = "Result" },
                        Value = new VisualBasicValue<int>("Operand1 + Operand2")
                    }
                }
            };
            return ab;
        }

        private ModelItem CreateModelItem(ModelItem parent, object obj)
        {
            var context = parent.GetEditingContext();
            return context.Services.GetService<ModelTreeManager>().CreateModelItem(parent, obj);
        }

        private string CreateCompositeWorkFlowTemplateXamlCode(string workflowName)
        {
            return string.Format(@"<Activity x:Class='[ClassName]' sap:VirtualizedContainerService.HintSize='240,240' 
                    mva:VisualBasic.Settings='Assembly references and imported namespaces for internal implementation' xmlns='http://schemas.microsoft.com/netfx/2009/xaml/activities' 
                    xmlns:local='clr-namespace:;assembly={0}' 
                    xmlns:mswac='clr-namespace:Microsoft.Support.Workflow.Authoring.CompositeActivity;assembly=Microsoft.Support.Workflow.Authoring' xmlns:mv='clr-namespace:Microsoft.VisualBasic;assembly=System' xmlns:mva='clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities' xmlns:s='clr-namespace:System;assembly=mscorlib' xmlns:s1='clr-namespace:System;assembly=System' xmlns:s2='clr-namespace:System;assembly=System.Xml' xmlns:s3='clr-namespace:System;assembly=System.Core' xmlns:sad='clr-namespace:System.Activities.Debugger;assembly=System.Activities' xmlns:sap='http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation' xmlns:scg='clr-namespace:System.Collections.Generic;assembly=System' xmlns:scg1='clr-namespace:System.Collections.Generic;assembly=System.ServiceModel' xmlns:scg2='clr-namespace:System.Collections.Generic;assembly=System.Core' xmlns:scg3='clr-namespace:System.Collections.Generic;assembly=mscorlib' xmlns:sd='clr-namespace:System.Data;assembly=System.Data' xmlns:sl='clr-namespace:System.Linq;assembly=System.Core' xmlns:st='clr-namespace:System.Text;assembly=mscorlib' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                      <Sequence sap:VirtualizedContainerService.HintSize='200,200'>
                        <sap:WorkflowViewStateService.ViewState>
                          <scg3:Dictionary x:TypeArguments='x:String, x:Object'>
                            <x:Boolean x:Key='IsExpanded'>True</x:Boolean>
                          </scg3:Dictionary>
                        </sap:WorkflowViewStateService.ViewState>
                        <local:{0} />
                      </Sequence>
                    </Activity>", workflowName);
        }

        private IEnumerable<ModelItem> GetAllActivities(ModelItem startingItem)
        {
            List<ModelItem> foundItems = new List<ModelItem>();
            Queue<ModelItem> modelItems = new Queue<ModelItem>();
            modelItems.Enqueue(startingItem);
            HashSet<ModelItem> alreadyVisited = new HashSet<ModelItem>();

            while (modelItems.Count > 0)
            {
                ModelItem currentModelItem = modelItems.Dequeue();
                if (currentModelItem == null)
                {
                    continue;
                }
                if (currentModelItem.ItemType.IsSubclassOf(typeof(Activity)))
                {
                    foundItems.Add(currentModelItem);
                }

                ModelItemCollection collection = currentModelItem as ModelItemCollection;
                if (collection != null)
                {
                    foreach (ModelItem modelItem in collection)
                    {
                        if (modelItem != null && !alreadyVisited.Contains(modelItem))
                        {
                            alreadyVisited.Add(modelItem);
                            modelItems.Enqueue(modelItem);
                        }
                    }
                }
                else
                {
                    ModelItemDictionary dictionary = currentModelItem as ModelItemDictionary;
                    if (dictionary != null)
                    {
                        foreach (var kvp in dictionary)
                        {
                            ModelItem miKey = kvp.Key;
                            if (miKey != null && !alreadyVisited.Contains(miKey))
                            {
                                alreadyVisited.Add(miKey);
                                modelItems.Enqueue(miKey);
                            }

                            ModelItem miValue = kvp.Value;
                            if (miValue != null && !alreadyVisited.Contains(miValue))
                            {
                                alreadyVisited.Add(miValue);
                                modelItems.Enqueue(miValue);
                            }
                        }
                    }
                }
                if (currentModelItem.ItemType.IsSubclassOf(typeof(Activity)) ||
                    currentModelItem.ItemType.IsAssignableFrom(typeof(ActivityBuilder)))
                {
                    ModelPropertyCollection modelProperties = currentModelItem.Properties;
                    foreach (ModelProperty property in modelProperties)
                    {
                        if (property.PropertyType.IsAssignableFrom(typeof(Type)) || property.PropertyType.IsValueType)
                        {
                            continue;
                        }
                        else
                        {
                            if (property.Value != null && !alreadyVisited.Contains(property.Value))
                            {
                                alreadyVisited.Add(property.Value);
                                modelItems.Enqueue(property.Value);
                            }
                        }
                    }
                }
            }
            return foundItems;
        }

        [TestInitialize]
        public void TestInitial() 
        {
            DispatcherHelp.DoEvents();
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void aaCompositeWorkflow_AddCompositeTest001()
        {
            var xaml = CreateCompositeWorkFlowTemplateXamlCode("test001");
            var newWorkflowDesigner = CreateWorkflowDesigner(xaml);
            var model = GetCompositeModel(newWorkflowDesigner);

            var cw = new Microsoft.Support.Workflow.Authoring.CompositeActivity.CompositeWorkflow();
            cw.ModelService_ItemsAdded(new List<ModelItem>() { model });

            var newModel = GetModelsByName(newWorkflowDesigner, "test001").First();
            Assert.IsInstanceOfType(newModel.GetActivity(), typeof(Sequence));
            Assert.IsTrue(GetViewState(newModel) is NodeKey);
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void aaCompositeWorkflow_AddMultiCompositeTest001()
        {
            var xaml = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            var newWorkflowDesigner = CreateWorkflowDesigner(xaml);
            var model = GetFirstModel(newWorkflowDesigner);

            StoreViewState(model, CreateKey(Test001.GetName(), null));

            var mutiXaml = CreateCompositeWorkFlowTemplateXamlCode("test001");
            var mutiWorkflowDesigner = CreateWorkflowDesigner(mutiXaml);
            var mutiModel = GetCompositeModel(mutiWorkflowDesigner);
            mutiModel = CreateModelItem(model.Root, mutiModel.GetActivity());

            var cw = new Microsoft.Support.Workflow.Authoring.CompositeActivity.CompositeWorkflow();
            cw.ModelService_ItemsAdded(new List<ModelItem>() { mutiModel });
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void aaCompositeWorkflow_AddActivityInComposite()
        {
            var xaml = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            var newWorkflowDesigner = CreateWorkflowDesigner(xaml);
            var model = GetFirstModel(newWorkflowDesigner);

            StoreViewState(model, CreateKey(Test001.GetName(), null));

            var addActivity = CreateModelItem(model, new Sequence() { DisplayName = "addActivity" });

            var cw = new Microsoft.Support.Workflow.Authoring.CompositeActivity.CompositeWorkflow();
            cw.ModelService_ItemsAdded(new List<ModelItem>() { addActivity });

            Assert.IsTrue(GetViewState(addActivity) is ModelKey);
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void aaCompositeWorkflow_AddCompositeInCompositeActivity()
        {
            var xaml = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            var newWorkflowDesigner = CreateWorkflowDesigner(xaml);
            var model = GetFirstModel(newWorkflowDesigner);

            StoreViewState(model, CreateKey(Test001.GetName(), null));

            var compositeXaml = CreateCompositeWorkFlowTemplateXamlCode("test001");
            var compositeWorkflowDesigner = CreateWorkflowDesigner(compositeXaml);
            var compositemodel = GetCompositeModel(compositeWorkflowDesigner);
            var compositeActivity = CreateModelItem(model, compositemodel.GetActivity());

            var cw = new Microsoft.Support.Workflow.Authoring.CompositeActivity.CompositeWorkflow();
            bool isDelete = false;
            using (var messageBox = new ImplementationOfType(typeof(AddInMessageBoxService)))
            {
                messageBox.Register(() => AddInMessageBoxService.Show(Argument<string>.Any, Argument<string>.Any, MessageBoxButton.OK,
                           MessageBoxImage.Warning))
                    .Execute<MessageBoxResult>(() => { isDelete = true; return MessageBoxResult.OK; });
                cw.ModelService_ItemsAdded(new List<ModelItem>() { compositeActivity });
                Assert.IsTrue(isDelete);
            }
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void aaCompositeWorkflow_MoveActivityInComposite()
        {
            var xaml = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            var newWorkflowDesigner = CreateWorkflowDesigner(xaml);
            var model = GetFirstModel(newWorkflowDesigner);
            StoreViewState(model, CreateKey(Test001.GetName(), null));

            var addActivity = CreateModelItem(model, new Sequence() { DisplayName = "addActivity" });
            StoreViewState(addActivity, CreateModelKey());

            var cw = new Microsoft.Support.Workflow.Authoring.CompositeActivity.CompositeWorkflow();
            cw.ModelService_ItemsAdded(new List<ModelItem>() { addActivity });

            Assert.IsTrue(GetViewState(addActivity) is ModelKey);
            var modelKey = GetViewState(addActivity) as ModelKey;
            var nodeKey = GetViewState(model) as NodeKey;
            Assert.AreEqual(nodeKey.Key, modelKey.Node);
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void aaCompositeWorkflow_MoveCompositeInCompositeActivity()
        {
            var xaml = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            var newWorkflowDesigner = CreateWorkflowDesigner(xaml);
            var model = GetFirstModel(newWorkflowDesigner);
            StoreViewState(model, CreateKey(Test001.GetName(), null));

            var addActivity = CreateModelItem(model, new Sequence() { DisplayName = "addActivity" });
            StoreViewState(addActivity, CreateKey(Test001.GetName(), null, true));

            var cw = new Microsoft.Support.Workflow.Authoring.CompositeActivity.CompositeWorkflow();
            cw.ModelService_ItemsAdded(new List<ModelItem>() { addActivity });

            Assert.IsTrue(GetViewState(addActivity) is ModelKey);
            var modelKey = GetViewState(addActivity) as ModelKey;
            var nodeKey = GetViewState(model) as NodeKey;
            Assert.AreEqual(nodeKey.Key, modelKey.Node);
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void aaCompositeWorkflow_AddFlowchat()
        {
            var xaml = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            var newWorkflowDesigner = CreateWorkflowDesigner(xaml);
            var model = GetFirstModel(newWorkflowDesigner);

            var addFlowchat = CreateModelItem(model, new FlowStep() { Action = new WriteLine() });

            var cw = new Microsoft.Support.Workflow.Authoring.CompositeActivity.CompositeWorkflow();
            cw.ModelService_ItemsAdded(new List<ModelItem>() { addFlowchat });
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void aaCompositeWorkflow_AddModelHasKey()
        {
            var xaml = TestUtilities.GenerateASequenceXmalCodeWithoutActivity();
            var newWorkflowDesigner = CreateWorkflowDesigner(xaml);
            var model = GetFirstModel(newWorkflowDesigner);

            StoreViewState(model, CreateKey(Test001.GetName(), null));

            var cw = new Microsoft.Support.Workflow.Authoring.CompositeActivity.CompositeWorkflow();
            cw.ModelService_ItemsAdded(new List<ModelItem>() { model });

            var newModel = GetFirstModel(newWorkflowDesigner);
            Assert.IsInstanceOfType(newModel.GetActivity(), typeof(Sequence));
            Assert.IsTrue(GetViewState(newModel) is NodeKey);
            var key = GetViewState(newModel) as NodeKey;
            Assert.IsTrue(key.IsSuccessfullyApplied);
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void aaCompositeWorkflow_ChangeModel()
        {
            var xaml = CreateCompositeWorkFlowTemplateXamlCode("test001");
            var newWorkflowDesigner = CreateWorkflowDesigner(xaml);
            var root = GetRoot(newWorkflowDesigner);

            var cw = new Microsoft.Support.Workflow.Authoring.CompositeActivity.CompositeWorkflow();
            cw.ModelService_PropertiesChanged(root.Properties);

        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void aaCompositeWorkflow_VerifyUpdateReference()
        {
            var activity = GenerateASequenceWithTwoActivities();
            var newWorkflowDesigner = CreateWorkflowDesigner(activity);
            var models = GetModelsByName(newWorkflowDesigner, "Test1");
            var writeLine = models.Last().GetActivity() as WriteLine;

            foreach (var model in models)
            {
                StoreViewState(model, CreateKey(Test001.GetName(), null));
            }

            var cw = new Microsoft.Support.Workflow.Authoring.CompositeActivity.CompositeWorkflow();
            using (var messageBox = new ImplementationOfType(typeof(AddInMessageBoxService)))
            {
                messageBox.Register(() => AddInMessageBoxService.Show(Argument<string>.Any, Argument<string>.Any, MessageBoxButton.YesNo,
                           MessageBoxImage.Question))
                    .Execute<MessageBoxResult>(() => { return MessageBoxResult.Yes; });
                cw.UpdateReference(models.First());

                models = GetModelsByName(newWorkflowDesigner, "Test1");
                writeLine = models.Last().GetActivity() as WriteLine;
            }
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void aaCompositeWorkflow_VerifyUpdateNoReference()
        {
            var activity = GenerateASequenceWithTwoActivities();
            var newWorkflowDesigner = CreateWorkflowDesigner(activity);
            var models = GetModelsByName(newWorkflowDesigner, "Test1");

            bool isExecute = false;
            var cw = new Microsoft.Support.Workflow.Authoring.CompositeActivity.CompositeWorkflow();
            using (var messageBox = new ImplementationOfType(typeof(AddInMessageBoxService)))
            {
                messageBox.Register(() => AddInMessageBoxService.Show(Argument<string>.Any, Argument<string>.Any, MessageBoxButton.OK,
                           MessageBoxImage.Question))
                    .Execute<MessageBoxResult>(() => { isExecute = true; return MessageBoxResult.OK; });
                cw.UpdateReference(models.First());

                Assert.IsTrue(isExecute);
            }
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void aaCompositeWorkflow_AddCompositeArgument()
        {
            var newWorkflowDesigner = CreateWorkflowDesigner(GenerateASequenceWithArgument());
            var model = GetFirstModel(newWorkflowDesigner);
            bool isAdded = false;
            try
            {
                ArgumentService.CreateArguments(model);
                isAdded = true;
            }
            catch
            {
                isAdded = false;
            }
            Assert.IsTrue(isAdded);
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobo")]
        public void aaCompositeWorkflow_AddCompositeTestWithArgument()
        {
            var xaml = CreateCompositeWorkFlowTemplateXamlCode("TestWorkflowWithArgument");
            Assembly assemblyWithArgument = LoadtestAssembly("TestWorkflowWithArgument");
            var newWorkflowDesigner = CreateWorkflowDesigner(xaml, assemblyWithArgument);
            var model = GetCompositeModel(newWorkflowDesigner, assemblyWithArgument);

            var cw = new Microsoft.Support.Workflow.Authoring.CompositeActivity.CompositeWorkflow();
            cw.ModelService_ItemsAdded(new List<ModelItem>() { model });

            var newModel = GetModelsByName(newWorkflowDesigner, "TestWorkflowWithArgument").First();
            Assert.IsInstanceOfType(newModel.GetActivity(), typeof(Sequence));
            Assert.IsTrue(GetViewState(newModel) is NodeKey);
        }

        [WorkItem(348262)]
        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-jillhu")]
        public void aaCompositeWorkflow_AddUpdateActivityTest()
        {
            var xaml = CreateCompositeWorkFlowTemplateXamlCode("test001");
            var newWorkflowDesigner = CreateWorkflowDesigner(xaml);
            var model = GetCompositeModel(newWorkflowDesigner);

            var cw = new Microsoft.Support.Workflow.Authoring.CompositeActivity.CompositeWorkflow();
            cw.ModelService_ItemsAdded(new List<ModelItem>() { model });

            var newModel = GetModelsByName(newWorkflowDesigner, "test001").First();
            var node = new CompositeNode();
            node.Activity = new Sequence();

            PrivateObject po = new PrivateObject(cw);
            CompositeWorkflow_Accessor cwaco = new CompositeWorkflow_Accessor(po);
            cwaco.UpdateActivity(node);
        }

        [TestCleanup]
        public void TestCleanup() { System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); }
    }
}
