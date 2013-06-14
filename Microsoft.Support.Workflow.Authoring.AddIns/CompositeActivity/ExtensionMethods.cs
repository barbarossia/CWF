using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Diagnostics.Contracts;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

namespace Microsoft.Support.Workflow.Authoring.CompositeActivity
{
    /// <summary>
    ///  Extension methods of the composite workflow
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Clone new activity from an activity
        /// </summary>
        public static Activity Clone(this Activity activity)
        {
            Contract.Requires(activity != null);

            string xaml = XamlService.SerializeToXaml(activity);
            return XamlService.DeserializeString(xaml) as Activity;
        }

        /// <summary>
        /// Does this AssemblyName represent this ModelItem AssemblyName?
        /// </summary>
        public static bool Equal(this AssemblyName left, ModelItem right)
        {
            if (left == null || right == null)
                return false;
            else
            {
                var righName = right.ItemType.Assembly.GetName();
                return (left.Name == righName.Name &&
                    left.Version == righName.Version);
            }
        }

        /// <summary>
        /// Does this AssemblyName represent this ModelItem AssemblyName?
        /// </summary>
        public static bool Equal(this AssemblyName left, AssemblyName right)
        {
            if (left == null || right == null)
                return false;
            else
            {
                return (left.Name == right.Name &&
                    left.Version == right.Version);
            }
        }

        /// <summary>
        /// Get Activity from the modelitem
        /// </summary>
        public static Activity GetActivity(this ModelItem model)
        {
            Contract.Requires(model != null);

            return model.GetCurrentValue() as Activity;
        }

        /// <summary>
        /// Get Task Activity from the modelitem
        /// </summary>
        public static TaskActivity GetTaskActivity(this ModelItem model)
        {
            Contract.Requires(model != null);

            return model.GetCurrentValue() as TaskActivity;
        }

        /// <summary>
        /// Get varliable from the modelitem
        /// </summary>
        public static Variable GetVarliable(this ModelItem model)
        {
            Contract.Requires(model != null);

            return model.GetCurrentValue() as Variable;
        }

        /// <summary>
        /// Get AssemblName from the modelitem
        /// </summary>
        public static AssemblyName GetAssemblyName(this ModelItem model)
        {
            Contract.Requires(model != null);

            return model.ItemType.Assembly.GetName();
        }

        /// <summary>
        /// Get AssemblName from the Activity
        /// </summary>
        public static AssemblyName GetAssemblyName(this Activity activity)
        {
            Contract.Requires(activity != null);

            return activity.GetType().Assembly.GetName();
        }

        /// <summary>
        /// Get ModelService from the ModelItem
        /// </summary>
        public static ModelService GetModelService(this ModelItem root)
        {
            Contract.Requires(root != null);

            return root.GetEditingContext().Services.GetService<ModelService>();
        }

        /// <summary>
        /// Get ModelService from WorkflowDesigner
        /// </summary>
        public static ModelService GetModelService(this WorkflowDesigner wd)
        {
            Contract.Requires(wd != null);

            return wd.Context.Services.GetService(typeof(ModelService)) as ModelService;
        }

        /// <summary>
        /// Get ModelService from WorkflowDesigner
        /// </summary>
        public static ModelItem GetRoot(this WorkflowDesigner wd)
        {
            Contract.Requires(wd != null);

            var ms = wd.GetModelService();
            if (ms != null)
            {
                return wd.GetModelService().Root;
            }

            return null;
        }
    }
}
