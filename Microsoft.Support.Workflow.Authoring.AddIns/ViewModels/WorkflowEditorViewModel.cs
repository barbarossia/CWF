using CWF.DataContracts;
using Microsoft.CSharp;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.Behaviors;
using Microsoft.Support.Workflow.Authoring.CompositeActivity;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;
using Microsoft.VisualBasic.Activities;
using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Hosting;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.Validation;
using System.Activities.Presentation.View;
using System.Activities.Validation;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xaml;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Support.Workflow.Authoring.AddIns.ViewModels
{
    public delegate void ActivityParameterChangedEventHandler(object sender, EventArgs e);

    [Serializable]
    public class WorkflowEditorViewModel : NotificationObject
    {
        #region constants
        /// <summary>
        /// Default to use for the workflow type that is not a service
        /// </summary>
        private const string WorkflowTypeDefaultActivity = "Workflow";

        /// <summary>
        /// Default to use for the workflow type that is a service
        /// </summary>
        private const string WorkflowTypeDefaultWfservice = "WorkflowService";

        /// <summary>
        /// Perfix at workflow name
        /// </summary>
        private const string WorkflowNamePerfix = "Workflow{0}";

        /// <summary>
        /// Connectors of workflow name
        /// </summary>
        private const string Dash = "-";

        /// <summary>
        /// The property name of root
        /// </summary>
        private const string Name_NameProperty = "Name";

        /// <summary>
        /// The configuration property name of root
        /// </summary>
        private const string Name_ConfigurationPropery = "ConfigurationName";

        /// <summary>
        /// The default workflow name
        /// </summary>
        private const string DefaultWorkflowName = "Untitled";

        /// <summary>
        /// White space
        /// </summary>
        private const string WhiteSpace = " ";

        /// <summary>
        /// A xml namespace
        /// </summary>
        private const string XmlNamespace = "localnamespace";

        /// <summary>
        /// The error message for validation activity
        /// </summary>
        private const string MSG_Validation_Error = "Not a workflow";

        /// <summary>
        /// The error message of set arguments for workflow;
        /// </summary>
        private const string MSG_SetArguments_Error = "The property type is not Assignable from In, InOut or Out Argument types, This should be impossible.";

        #endregion

        #region private fileds
        [NonSerialized]
        private CancellationTokenSource cancellationToken ;
        /// <summary>
        /// workflow designer
        /// </summary>
        [NonSerialized]
        private WorkflowDesigner workflowDesigner;

        [NonSerialized]
        private TreeView collection;
        /// <summary>
        /// The type that was used to create this workflow
        /// </summary>
        private bool isReadonly;
        private ErrorService errors;
        /// <summary>
        /// The data dirty flag, defaults to true
        /// </summary>
        private bool isDataDirty = true;
        /// <summary>
        /// The xaml code.
        /// </summary>
        private string xamlCode;
        [NonSerialized]
        private CompositeWorkflow compositeWorkflow;
        private string displayName;       // The display name of object.
        private string fullName;          // The full name of object.
        private string name;
        private bool isTask;
        private ObservableCollection<TaskItem> taskItems;

        public ProjectExplorerViewModel ProjectExploreVM { get; set; }

        #endregion

        #region events

        /// <summary>
        /// notify host the designer changed
        /// </summary>
        public event EventHandler DesignerChanged;
        public event EventHandler PrintStateChanged;
        public GetTaskLastVersionEventHandler GetTaskLastVersionChanged;
        public bool IsTask {
            get {
                return isTask;
            }
            set {
                isTask = value;
                RaisePropertyChanged(() => IsTask);
            }
        }

        private ActivityParameterChangedEventHandler ActivityParameterChanged;

        #endregion

        public ObservableCollection<TaskItem> TaskItems {
            get {
                return taskItems;
            }
            set {
                taskItems = value;
                RaisePropertyChanged(() => TaskItems);
            }
        }

        /// <summary>
        /// Gets or sets XamlCode.
        /// </summary>
        public string XamlCode
        {
            get
            {
                return xamlCode;
            }
            set
            {
                xamlCode = value;
                RaisePropertyChanged(() => XamlCode);
                RaisePropertyChanged(() => IsValid);
            }
        }

        /// <summary>
        /// Public property that is used to tell if the data is dirty in the current model
        /// </summary>
        public bool IsDataDirty
        {
            get
            {
                return isDataDirty;
            }
            set
            {
                isDataDirty = value;
                RaisePropertyChanged(() => IsDataDirty);
            }
        }

        public ErrorService Errors
        {
            get { return errors; }
            set
            {
                errors = value;
                RaisePropertyChanged(() => Errors);
                RaisePropertyChanged(() => this.IsValid);
            }
        }

        /// <summary>
        /// Public property to tell if the current workflow is valid
        /// </summary>
        [ReadOnly(true)]
        public bool IsValid
        {
            get
            {
                return !Errors.ErrorList.Any();
            }
        }

        /// <summary>
        /// Public property to tell if a WorkflowItem is a WorkflowService
        /// </summary>
        [ReadOnly(true)]
        public bool IsService
        {
            get
            {
                return IsWorkflowService();
            }
        }

        public event ActivityParameterChangedEventHandler WorkflowDesignerChanged
        {
            add { ActivityParameterChanged += value; }
            remove { ActivityParameterChanged -= value; }
        }

        /// <summary>
        /// Control the Composite Workflow
        /// private field need to add "NonSerialized" attribute
        /// </summary>
        public CompositeWorkflow CompositeWorkflow
        {
            get
            {
                if (compositeWorkflow == null)
                {
                    compositeWorkflow = new CompositeWorkflow();
                }

                return compositeWorkflow;
            }
        }

        /// <summary>
        /// Gets or sets WorkflowDesigner.
        /// </summary>
        public WorkflowDesigner WorkflowDesigner
        {
            get
            {
                return workflowDesigner;
            }
            set
            {
                workflowDesigner = value;
                ConfigureWorkflowDesigner(workflowDesigner);
                WorkflowDesigner.ModelChanged += WorkflowDesigner_ModelChanged;
                IsDataDirty = true;
                // Get the ModelService and then sync to the ModelChanged event
                // service will be non-null if the xaml is WorkflowService or ActivityBuilder.
                ModelService service = WorkflowDesigner.Context.Services.GetService(typeof(ModelService)) as ModelService;
                if (service != null)
                {
                    RefreshTasksTable();
                    // Set defaults for any In/InOut and Out Arguments in Activities
                    service.ModelChanged += SetDefaultArguments;
                    service.ModelChanged += ModelService_ModelChanged;
                }
                
                Task createIntellisenseTask = Task.Factory.StartNew(() =>
                {
                    // Create ExpressionEditorService 
                    ExpressionEditorService expressionEditorService = new ExpressionEditorService();
                    expressionEditorService.IntellisenseNode = ExpressionEditorHelper.CreateIntellisenseList();
                    //Publish the instance of the EditorService.
                    if (!WorkflowDesigner.Context.Services.Contains(typeof(IExpressionEditorService)))
                    {
                        WorkflowDesigner.Context.Services.Publish<IExpressionEditorService>(expressionEditorService);
                    }
                },cancellationToken.Token);
                RaisePropertyChanged(() => WorkflowDesigner);
                if (ActivityParameterChanged != null)
                    ActivityParameterChanged(this, null);
            }
        }

        /// <summary>
        /// Gets or sets FullName.
        /// For ActivtyItem, full name is the FullName of its Activity type.
        /// For AssemblyName, full name is the FullName of its Assembly (or AssemblyName.FullName)
        /// For WorkflowItem, full name is its class name with namespace.
        /// </summary>
        public string FullName
        {
            get { return fullName; }
            set
            {
                fullName = value;
                RaisePropertyChanged(() => FullName);
            }
        }

        /// <summary>
        /// Gets or sets Name.
        /// For ActivityItem and WorkflowItem, name is its Activity/Workflow class name.
        /// For AssemblyItem, name is its Assembly's name (Assembly.GetName().Name)
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged(() => Name);
                OnSetName();
            }
        }

        /// <summary>
        /// Public property to tell if the current workflow is read-only
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return isReadonly;
            }
            set
            {
                ReadOnlyState state = WorkflowDesigner.Context.Items.GetValue<ReadOnlyState>();
                state.IsReadOnly = value;
                WorkflowDesigner.Context.Items.SetValue(state);
                isReadonly = value;
                RaisePropertyChanged(() => IsReadOnly);
            }
        }

        public WorkflowEditorViewModel() : this(null)
        {
        }

        public WorkflowEditorViewModel(CancellationTokenSource token)
        {
            this.cancellationToken = token;
        }

        public void Init(string name, string projectXamlCode, bool isTask)
        {
            collection = new TreeView();

            this.name = name;
            // Set the XAML code
            xamlCode = projectXamlCode;
            // Set the task flag
            this.IsTask = isTask;
            // Make sure we have a Designer
            RefreshDesignerFromXamlCode();
        }

        private void OnSetName()
        {
            // Make sure we sync the FullName
            FullName = Name;
            // Update the required portion of the XAML when the Name changes
            UpdateXamlCodeFromName();
        }

        private PrintAction shouldBePrint;
        public PrintAction ShouldBePrint
        {
            get
            {
                return shouldBePrint;
            }
            set
            {
                shouldBePrint = value;
                RaisePropertyChanged(() => ShouldBePrint);
                RaisePrintStateChanged();
            }
        }

        /// <summary>
        /// Public access method to create a new Workflow Designer
        /// </summary>
        public void RefreshDesignerFromXamlCode()
        {
            // Reload the designer since Xaml has changed\
            WorkflowDesigner = new WorkflowDesigner();
            IsReadOnly = isReadonly;
        }

        public void ClearUndo() {
            List<UndoUnit> undoList = typeof(UndoEngine).GetField("undoBuffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(WorkflowDesigner.Context.Services.GetService<UndoEngine>()) as List<UndoUnit>;
            undoList.Clear();
        }

        #region private method
        /// <summary>
        /// The new ModelCHanged event delegate
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">ModelChangedEventArgs</param>
        private void WorkflowDesigner_ModelChanged(object sender, EventArgs e)
        {
            IsDataDirty = true;
            // Make sure we get the updated XAML
            RefreshXamlCodeFromDesigner();
            RefreshTasksTable();
        }

        private void RefreshTasksTable() {
            TaskItems = new ObservableCollection<TaskItem>(MultipleAuthorService.GetTaskItems(WorkflowDesigner));
        }

        /// <summary>
        /// Sets the defaults on InArgument, OutArgument and InOutArgument Activity arguments based on a DefaultValue
        /// </summary>
        /// <param name="sender">The </param>
        /// <param name="e"></param>
        private void SetDefaultArguments(object sender, ModelChangedEventArgs e)
        {
            IEnumerable<ModelItem> enumerator = e.ItemsAdded;
            if (enumerator == null)
            {
                return;
            }

            foreach (ModelItem mi in enumerator)
            {
                if (mi == null)
                {
                    return;
                }
                foreach (var item in mi.Properties)
                {
                    if (item == null)
                    {
                        return;
                    }
                    Type propertyType = item.PropertyType;

                    Type[] typeParameters = propertyType.GetGenericArguments();

                    if (typeParameters.Length > 0)
                    {
                        Type genericType = typeParameters[0];
                        if (item.DefaultValue != null)
                        {
                            object genericArgumentInstance = null;

                            if (typeof(InArgument).IsAssignableFrom(propertyType))
                            {
                                var inArgument = typeof(InArgument<>);
                                var vbValue = typeof(VisualBasicValue<>);
                                genericArgumentInstance = GetGenericArgumentInstance(item, inArgument, genericType, vbValue);
                            }
                            else if (typeof(InOutArgument).IsAssignableFrom(propertyType))
                            {
                                var inOutArgument = typeof(InOutArgument<>);
                                var vbRefernec = typeof(VisualBasicReference<>);
                                genericArgumentInstance = GetGenericArgumentInstance(item, inOutArgument, genericType, vbRefernec);
                            }
                            else if (typeof(OutArgument).IsAssignableFrom(propertyType))
                            {
                                var outArgument = typeof(OutArgument<>);
                                var vbRefernece1 = typeof(VisualBasicReference<>);
                                genericArgumentInstance = GetGenericArgumentInstance(item, outArgument, genericType, vbRefernece1);
                            }
                            else
                            {
                                throw new DevFacingException(MSG_SetArguments_Error);
                            }

                            item.SetValue(genericArgumentInstance);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Setup steps that we do each time WorkflowDesigner is refreshed. Right now we just configure what assemblies to show in the
        /// type lists for Variable/Argument types.
        /// </summary>
        /// <param name="workflowDesigner"></param>
        private void ConfigureWorkflowDesigner(WorkflowDesigner workflowDesigner)
        {
            Errors = new ErrorService(this);
            ErrorService errors = Errors;
            workflowDesigner.Context.Services.Publish<IXamlLoadErrorService>(errors);
            workflowDesigner.Context.Services.Publish<IValidationErrorService>(errors);

            workflowDesigner.Text = xamlCode; // setup for workflowDesigner.Load()
            workflowDesigner.Load(); // initialize workflow based on Text property

            if (errors.ErrorList.Any())
            {
                // There were XAML errors loading, don't bother adding a validationErrorInfo
                // since it's kind of redunant, but do set the WorkflowDesigner to readonly mode.
                // If the user fixes the Xaml errors we'll get a new WorkflowDesigner which will
                // have an empty errors.ErrorList.
                if (!workflowDesigner.IsInErrorState())
                {
                    // If we are still going to show a diagram, make it translucent to make it clear it's read-only
                    workflowDesigner.Context.Items.GetValue<ReadOnlyState>().IsReadOnly = true;
                    workflowDesigner.View.Opacity = 0.60; // code smell: setting View property from WorkflowItem ctor. But it's hard to get IsReadOnly from XAML.
                }
            }
            else
            {
                // Do initial validation synchronously to avoid UI glitches 
                // (buttons enabled-until-validation-realizes-workflow-isn't-valid)
                try
                {
                    var rootActivity = AsDynamicActivity();
                    if (rootActivity == null)
                    {
                        // It loaded but it's not a workflow and we can't validate it in detail.
                        // All we know is that it's not a valid workflow. E.g. it could be a XAML object like
                        // <x:String xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">32</x:String>
                        errors.ShowValidationErrors(new[] { new ValidationErrorInfo(MSG_Validation_Error) });
                    }
                    else
                    {
                        ValidationResults myValidationResults = ActivityValidationServices.Validate(rootActivity);
                        errors.ShowValidationErrors((from err in myValidationResults.Errors select new ValidationErrorInfo(err)).ToList());
                    }
                }
                catch (XamlException e)  // Yes, ActivityValidationServices.Validate can throw  No, it's not documented on MSDN.
                {
                    // Bad XAML (e.g. setting nonexistent properties) is not a valid workflow
                    errors.ShowXamlLoadErrors(new[] { new XamlLoadErrorInfo(e.Message, e.LineNumber, e.LinePosition) });
                }
            }
        }

        /// <summary>
        /// Creates a generic type instance and it's value 
        /// </summary>
        /// <param name="item">The Model item to use for getting the DefaultValue</param>
        /// <param name="argument">Type to use for the generic type In/Out/InOut Argument</param>
        /// <param name="genericType">Type to use for the generic type</param>
        /// <param name="vbValue">The type to use for the Value</param>
        /// <returns>Object instance of the Argument with the Default Value</returns>
        private object GetGenericArgumentInstance(ModelProperty item, Type argument, Type genericType, Type vbValue)
        {
            Type genericInArgument = argument.MakeGenericType(genericType);
            Type genericVbValue = vbValue.MakeGenericType(genericType);
            object genericVbValueInstance = Activator.CreateInstance(genericVbValue, item.DefaultValue.ToString());
            return Activator.CreateInstance(genericInArgument, genericVbValueInstance);
        }

        /// <summary>
        /// Used to Properly determine if this workflow is a WorkflowService 
        /// </summary>
        /// <returns>True if a WorkfowService, false if not</returns>
        private bool IsWorkflowService()
        {
            bool isWorkflowService = false;
            var modelService = WorkflowDesigner.Context.Services.GetService<ModelService>();

            if (modelService != null)
            {
                if (modelService.Root.GetCurrentValue() is WorkflowService)
                {
                    isWorkflowService = true;
                }
            }
            return isWorkflowService;
        }

        /// <summary>
        /// The workflow as an Activity, or null if conversion fails
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "XmlReader.Dispose() will dispose of StringReader too. System.Xaml uses this same pattern in XamlServices.Parse.")]
        public Activity AsDynamicActivity()
        {
            // If the Workflow is an ActivityBuilder workflow and not a WorkflowService then we need to get ALL of the XAML 
            // that makes up the Workflow
            if (false == IsWorkflowService())
            {
                // This generates Reliability warning CA2000 but we will suppress it.
                try
                {
                    var root = WorkflowDesigner.Context.Services.GetService<ModelService>()
                        .IfNotNull(modelService => modelService.Root)
                        .IfNotNull(r => r.GetCurrentValue());
                    if (root == null)
                    {
                        return null; // not an activity
                    }
                    // transform ActivityBuilder rep to DynamicActivity via XamlNodeQueue
                    using (var objectReader = new XamlObjectReader(root))
                    {
                        var queue = new XamlNodeQueue(new XamlSchemaContext());
                        using (var writexClassXaml = ActivityXamlServices.CreateBuilderWriter(queue.Writer))
                        {
                            XamlServices.Transform(objectReader, writexClassXaml);
                            using (var readXaml = ActivityXamlServices.CreateReader(queue.Reader))
                            {
                                return XamlServices.Load(readXaml) as Activity; // probably a DynamicActivity
                            }
                        }
                    }
                }
                catch (XamlException)
                {
                    return null;
                }
                catch (XmlException) // Yes, XamlServices.Load and XamlXmlReader's ctor can throw  No, it's not documented on MSDN.
                {
                    return null; // invalid XAML (e.g. unclosed tags) is not a valid workflow
                }
            }
            else
            {
                // In the case of a WorkflowService we only need to get the root Activity to do Validation
                return Helper.GetActivityFromDesigner(WorkflowDesigner);
            }
        }

        /// <summary>
        /// Update The Attributes of the Activity or WorkflowService XAML/XAMLX as required.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2122", Justification = "Method is private and has been reviewed. Users "
            + "will not be able to use this method to call IsValidIdentifier().")]
        private void UpdateXamlCodeFromName()
        {
            // Change the name of the generated activity class. Our compiler generates C# projects to compile workflows,
            // so if it's an invalid C# identifier we use a unique synthetic name, otherwise use the FullName 
            // of the ModelItem (for example Microsoft.Support.Activities.MyActivity1)
            var modelService = WorkflowDesigner.Context.Services.GetService<ModelService>();

            using (CSharpCodeProvider csharpCodeProvider = new CSharpCodeProvider())
            {
                if (modelService != null)
                {
                    if (IsService)
                    {
                        var builderName = modelService.Root.Properties[Name_NameProperty];
                        var builderConfigurationName = modelService.Root.Properties[Name_ConfigurationPropery];
                        var trimmedName = (Name ?? DefaultWorkflowName).Replace(WhiteSpace, string.Empty);
                        var newName = XName.Get(trimmedName, XmlNamespace);
                        if (!object.Equals(builderName.Value.IfNotNull(v => v.GetCurrentValue()), newName))
                        {
                            builderName.SetValue(newName);
                            builderConfigurationName.SetValue(Name);
                            // The new XAML from the name change should be copied to WorkflowItem.XamlCode
                            RefreshXamlCodeFromDesigner();
                        }
                    }
                    else
                    {
                        string newName = (
                           csharpCodeProvider.IsValidIdentifier(Name) // CSharpCodeProvider is lightweight, no need to cache
                           ? FullName
                           : string.Format(WorkflowNamePerfix, Guid.NewGuid().ToString().Replace(Dash, string.Empty)));

                        var builderName = modelService.Root.Properties[Name_NameProperty];
                        if (!object.Equals(builderName.Value.IfNotNull(v => v.GetCurrentValue()), newName))
                        {
                            builderName.SetValue(newName);

                            // The new XAML from the name change should be copied to WorkflowItem.XamlCode
                            RefreshXamlCodeFromDesigner();
                        }
                    }
                }
            }
        }

        private void RaisePrintStateChanged()
        {
            if (this.PrintStateChanged != null)
                PrintStateChanged(this, EventArgs.Empty);
        }

        public void RaiseDesignerChanged()
        {
            if (this.DesignerChanged != null && !this.IsReadOnly)
                DesignerChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Get the latest XAML code from workflow designer
        /// </summary>
        private void RefreshXamlCodeFromDesigner()
        {
            if (WorkflowDesigner != null)
            {
                XamlCode = WorkflowDesigner.LooseXaml();
                XamlIndexTreeHelper.Refresh(XamlCode);
                RaiseDesignerChanged();
            }
        }

        /// <summary>
        /// Response the model items changed
        /// </summary>
        private void ModelService_ModelChanged(object sender, ModelChangedEventArgs e)
        {
            if (e.ItemsAdded != null)
            {
                CompositeWorkflow.ModelService_ItemsAdded(e.ItemsAdded);
            }
            else if (e.PropertiesChanged != null)
            {
                CompositeWorkflow.ModelService_PropertiesChanged(e.PropertiesChanged);
            }
        }

        public void DownloadTaskDependency(IEnumerable<TaskActivityDC> taskActivityDCs)
        {
            this.GetTaskLastVersionChanged(
                this,
                new GetTaskEventArgs(taskActivityDCs.Select(
                    t => new { Name = t.Activity.ActivityLibraryName, Version = t.Activity.ActivityLibraryVersion })
                    .ToDictionary(d => d.Name, d => d.Version)));
        }

        public void FinishTaskAssigned()
        {
            MultipleAuthorService.FinishTaskAssigned(this.WorkflowDesigner);
        }

        #endregion

        /// <summary>
        /// Helper class to manage errors
        /// </summary>
        [Serializable]
        public class ErrorService : IXamlLoadErrorService, IValidationErrorService
        {
            private const string ErrorMessageFormat = "Line {0} Pos {1}: {2}";
            ObservableCollection<string> xamlErrors = new ObservableCollection<string>();
            ObservableCollection<string> validationErrors = new ObservableCollection<string>();
            WorkflowEditorViewModel owner;

            public ErrorService(WorkflowEditorViewModel owner) // should never be called by anyone except WorkflowItem
            {
                this.owner = owner;
                xamlErrors.CollectionChanged += xamlErrors_CollectionChanged;
                validationErrors.CollectionChanged += validationErrors_CollectionChanged;
            }

            private void validationErrors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                owner.RaisePropertyChanged(() => owner.Errors); 
            }

            private void xamlErrors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                owner.RaisePropertyChanged(() => owner.Errors); 
            }

            public bool HasXamlLoadErrors
            {
                get
                {
                    return xamlErrors.Any();
                }
            }

            /// <summary>
            /// Errors to show to user
            /// </summary>
            public IEnumerable<string> ErrorList
            {
                get
                {
                    IEnumerable<string> list = xamlErrors.Concat(validationErrors);
                    
                    return list;
                }
            }

            public void ShowXamlLoadErrors(IList<XamlLoadErrorInfo> errors) 
            {
                xamlErrors.Clear();
                (from err in errors select string.Format(ErrorMessageFormat, err.LineNumber, err.LinePosition, err.Message))
                     .ToList()
                     .ForEach(m => xamlErrors.Add(m));
                                 
            }

            public void ShowValidationErrors(IList<ValidationErrorInfo> errors) 
            {
                var tasks = errors.Where(e => e.Message == typeof(TaskActivityAssignException).Name);
                if (tasks.Any())
                {
                    tasks.ToList().ForEach(t => errors.Remove(t));
                    owner.RaisePropertyChanged(typeof(TaskActivityAssignException).Name);
                }

                validationErrors.Clear();
                (from err in errors where !err.IsWarning select err.Message)
                    .ToList()
                    .ForEach(m => validationErrors.Add(m));
            }
        }
    }
}
