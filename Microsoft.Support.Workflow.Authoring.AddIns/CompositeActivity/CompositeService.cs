using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Hosting;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.View;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Activities;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

namespace Microsoft.Support.Workflow.Authoring.CompositeActivity
{
    /// <summary>
    /// Helper class to composite workflow
    /// </summary>
    public static class CompositeService
    {
        #region Public Functions
        /// <summary>
        /// Setup WorkflowDesigner when add a composite activity
        /// </summary>
        public static WorkflowDesigner CreateWorkflowDesigner(Activity activity, string name)
        {
            ActivityBuilder workflow = CreateWorkflow(activity, name);
            WorkflowDesigner newWorkflowDesigner = new WorkflowDesigner();
            SetupWorkflowDesignerRefernce(newWorkflowDesigner);
            newWorkflowDesigner.Load(workflow); // initialize workflow based on Text property
            return newWorkflowDesigner;
        }

        public static ModelItem CreateActivity(string xaml)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(xaml));

            WorkflowDesigner newWorkflowDesigner = CreateWorkflowDesigner(xaml);
            return newWorkflowDesigner.GetRoot();
        }

        public static Activity GetBody(ModelItem root)
        {
            ActivityBuilder builder = root.GetCurrentValue() as ActivityBuilder;
            return builder.Implementation;
        }

        public static bool IsCompositeModel(ModelItem model)
        {
            return !(AssemblyService.AssemblyIsBuiltIn(model.GetAssemblyName()) ||
                model.Properties.Any(p =>
                {
                    return p.Name != ModelItemService.DisplayNamePropertyName &&
                        p.Name != ModelItemService.IdPropertyName && 
                        !ArgumentService.IsArgument(p.PropertyType);
                }));
        }

        public static Activity GetRootActivity(Activity root)
        {
            try
            {
                return WorkflowInspectionServices.GetActivities(root).FirstOrDefault(r => !ArgumentService.IsArgument(r));
            }
            catch (System.Activities.InvalidWorkflowException)
            {
                //which activity has arguments
                //we need not to return these activities, so do nothing with these
            }
            return null;
        }

        public static IEnumerable<Activity> GetActivities(Activity parent)
        {
            List<Activity> result = new List<Activity>();
            try
            {
                result = GetAllActivitiesInTree(parent).ToList();
            }
            catch (System.Activities.InvalidWorkflowException)
            {
                //which activity has arguments
                //we need not to return these activities, so do nothing with these
            }

            return result;
        }

        /// <summary>
        /// Configure workflowDesigner referenced Assemblies
        /// </summary>
        public static void SetupWorkflowDesignerRefernce(WorkflowDesigner newWorkflowDesigner, IEnumerable<AssemblyName> referenceAssembly = null)
        {
            Contract.Requires(newWorkflowDesigner != null);

            var safeImports = referenceAssembly != null ? referenceAssembly :
                    from c in AddInCaching.ActivityAssemblyItems
                    where c.NotSafeForTypeLoad == false 
                    select c.AssemblyName;

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
                    (safeImports).ToList()
            });
        }

        public static void AddReferenceAssemblies(WorkflowDesigner workflowDesigner, IEnumerable<AssemblyName> referenceAssemblies)
        {
            Contract.Requires(workflowDesigner != null);
            Contract.Requires(referenceAssemblies != null);

            AssemblyContextControlItem assemblyContext = workflowDesigner.Context.Items.GetValue<AssemblyContextControlItem>();
            foreach(var asm in referenceAssemblies)
            {
                if (!IsContainAssembleName(assemblyContext.ReferencedAssemblyNames, asm))
                {
                    assemblyContext.ReferencedAssemblyNames.Add(asm);
                }
            }
            workflowDesigner.Context.Items.SetValue(assemblyContext);

        }

        public static IEnumerable<ModelItem> GetParents(ModelItem modelItem)
        {
            if (modelItem != null && modelItem.Parent != null)
            {
                yield return modelItem.Parent;
            }

            foreach (var parent in modelItem.Parents)
            {
                foreach (var decendent in GetParents(parent))
                {
                    yield return decendent;
                }
            }
        }

        private static bool IsContainAssembleName(IList<AssemblyName> referencedAssemblyNames, AssemblyName asm)
        {
             return referencedAssemblyNames.Any(r => r.Equal(asm));
        }

        /// <summary>
        /// Get subordinate model items
        /// </summary>
        public static IEnumerable<ModelItem> GetSubModelItems(ModelItem root)
        {
            Contract.Requires(root != null);

            List<ModelItem> result = ModelItemService.Find(root, delegate(Type modelItemType)
            {
                return typeof(Activity).IsAssignableFrom(modelItemType);
            }).ToList();
            result.Remove(root);
            return result;
        }

        /// <summary>
        /// Create model item by activity
        /// </summary>
        public static ModelItem CreateModelItem(Activity activity)
        {
            Contract.Requires(activity != null);

            WorkflowDesigner wd = CreateWorkflowDesigner(activity, null);
            return GetSubModelItems(wd).First();
        }

        /// <summary>
        /// Get subordinate model items
        /// </summary>
        public static IEnumerable<ModelItem> GetSubModelItems(WorkflowDesigner wd)
        {
            Contract.Requires(wd != null);

            return GetSubModelItems(wd.GetModelService(), null);
        }

        /// <summary>
        /// Get key which store in modelitem's view state
        /// </summary>
        public static object GetKeyObeject(ModelItem model, string key)
        {
            Contract.Requires(model != null);
            Contract.Requires(key != null);

            return GetViewState(model, key);
        }

        /// <summary>
        /// Delete modelitem
        /// </summary>
        public static void DeleteModelItem(ModelItem model)
        {
            Contract.Requires(model != null);

            foreach (ModelProperty modelProperty in model.Sources.ToList())
            {
                modelProperty.ClearValue();
            }

            var collectionParents = from parent in model.Parents
                                    where parent is ModelItemCollection
                                    select (ModelItemCollection)parent;
            foreach (ModelItemCollection collectionParent in collectionParents.ToList())
            {
                collectionParent.Remove(model);
            }
        }

        /// <summary>
        /// Replace modelitem by activity
        /// </summary>
        /// <param name="oldModelItem"></param>
        public static ModelItem UpdateModelItem(ModelItem oldModelItem, object newItem)
        {
            Contract.Requires(oldModelItem != null);
            Contract.Requires(newItem != null);

            ModelItem newModelItem = WrapAsModelItem(oldModelItem.GetEditingContext(), newItem, oldModelItem.Parent);

            try
            {
                foreach (ModelProperty modelProperty in oldModelItem.Sources.ToList())
                {
                    modelProperty.SetValue(newModelItem);
                }

                var collectionParents = from parent in oldModelItem.Parents
                                        where parent is ModelItemCollection
                                        select (ModelItemCollection)parent;
                foreach (ModelItemCollection collectionParent in collectionParents.ToList())
                {
                    int index = collectionParent.IndexOf(oldModelItem);
                    collectionParent.Insert(index, newModelItem);
                    collectionParent.Remove(oldModelItem);                   
                }
            }
            catch
            {
                throw new UserFacingException("Cannot make selected activity as a task.");
            }

            return newModelItem;
        } 

        public static IEnumerable<ModelItem> UpdateModelItem(IDictionary<ModelItem, Activity> updates)
        {
            Contract.Requires(updates != null);

            return updates.Select(u => UpdateModelItem(u.Key, u.Value)).ToList();
        }

        /// <summary>
        /// Save key object in modelitem's view state
        /// </summary>
        public static void StoreViewState(ModelItem model, string key, object value)
        {
            Contract.Requires(model != null);
            Contract.Requires(key != null);
            Contract.Requires(value != null);

            ViewStateService vss = model.GetEditingContext().Services.GetService<ViewStateService>();
            vss.StoreViewState(model, key, value);
        }

        /// <summary>
        /// Clean key in modelitem's view state
        /// </summary>
        public static void ClearViewState(ModelItem model, string key)
        {
            Contract.Requires(model != null);
            Contract.Requires(key != null);

            ViewStateService vss = model.GetEditingContext().Services.GetService<ViewStateService>();
            vss.RemoveViewState(model, key);
        }

        /// <summary>
        /// Search modelitem which view state has key
        /// </summary>
        public static ModelItem SearchStoreKeyModel(ModelItem model)
        {

            if (model == null)
            {
                return null;
            }
            else if (model.ItemType.IsSubclassOf(typeof(ActivityBuilder)))
            {
                return null;
            }
            else if (model.ItemType.IsSubclassOf(typeof(Activity)))
            {
                return model;
            }
            else
            {
                return SearchStoreKeyModel(model.Parent);
            }
        }
 
        #endregion

        #region Private Functions

        private static object GetViewState(ModelItem model, string key)
        {
            ViewStateService vss = model.GetEditingContext().Services.GetService<ViewStateService>();
            return vss.RetrieveViewState(model, key);
        }

        private static IEnumerable<ModelItem> GetSubModelItems(ModelService ms, ModelItem model)
        {
            List<ModelItem> result = new List<ModelItem>();
            if (ms != null)
            {
                ModelItem root = model == null ? ms.Root : model;
                result.AddRange(ms.Find(root, typeof(Activity)));
            }
            return result;
        }

        private static ActivityBuilder CreateWorkflow(Activity activity, string name)
        {
            return new ActivityBuilder()
            {
                Name = string.IsNullOrEmpty(name) ? Guid.NewGuid().ToString() : name,
                Implementation = activity,
            };
        }

        private static ModelItem WrapAsModelItem(EditingContext context, object obj, ModelItem parant)
        {
            return context.Services.GetService<ModelTreeManager>().CreateModelItem(parant, obj);
        }

        private static WorkflowDesigner CreateWorkflowDesigner(string xaml)
        {
            WorkflowDesigner newWorkflowDesigner = new WorkflowDesigner();
            SetupWorkflowDesignerRefernce(newWorkflowDesigner);
            newWorkflowDesigner.Text = xaml;
            newWorkflowDesigner.Load(); // initialize workflow based on Text property
            return newWorkflowDesigner;
        }

        /// <summary>
        /// Transitive closure of WorkflowInspectionServices.GetActivities()
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private static IEnumerable<Activity> GetAllActivitiesInTree(Activity parent)
        {
            if (parent != null)
            {
                yield return parent;

                foreach (var child in WorkflowInspectionServices.GetActivities(parent))
                {
                    // the child is also a descendant so we don't need to yield it explicitly
                    foreach (var descendant in GetAllActivitiesInTree(child)) // there might be a better way to join IEnum streams than re-yielding
                    {
                        yield return descendant;
                    }
                }
            }
        }

        #endregion
    }
}
