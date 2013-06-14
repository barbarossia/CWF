using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Services;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.CompositeActivity;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Utilities
{
    public class DependencyAnalysisService
    {
        public static CompileProject GetCompileProject(WorkflowEditorViewModel focusedWorkflowItem)
        {
            Contract.Requires(focusedWorkflowItem != null);
            Contract.Requires(focusedWorkflowItem.WorkflowDesigner != null);

            return GetCompileProject(focusedWorkflowItem.WorkflowDesigner);
        }

        private static CompileProject GetCompileProject(WorkflowDesigner desinger)
        {
            MultipleAuthorService.UnassignAllTask(desinger);

            CompileProject compileProject = new CompileProject();            
            HashSet<Type> referencedTypes = GetReferencedTypes(desinger);
            HashSet<Assembly> referencedAssemblies = GetReferencedAssemblies(desinger, referencedTypes);
            
            compileProject.ReferencedAssemblies = referencedAssemblies;
            compileProject.ReferencedTypes = referencedTypes;
            compileProject.ProjectXaml = desinger.CompilableXaml();

            return compileProject;
        }

        private static NameVersionEqualityComparer referenceComparer = new NameVersionEqualityComparer();
        /// <summary>
        /// Attempt to warn the user if he is relying on unsigned assemblies in his workflow (an unsupported scenario).
        /// There are cases we won't catch though that the C# compiler will, involving generics like 
        /// Variable of List of List of TypeFromUnsignedAssembly
        /// </summary>
        /// <param name="project">Project for the compile operation</param>
        public static HashSet<Assembly> GetReferencedAssemblies(WorkflowDesigner workflowDesigner, IEnumerable<Type> referencedTypes)
        {
            HashSet<Assembly> referencedAssemblies = new HashSet<Assembly>();

            //Get the assemblies referenced in the workflow.
            //We are shrinking the enumeration of types, as different types can come from one assembly
            IEnumerable<Assembly> assemblies = referencedTypes
                                                    .Where(baseType => null != baseType)
                                                    .Select(baseType => baseType.Assembly);

            if (assemblies.Any())
            {
                referencedAssemblies.UnionWith(assemblies);
            }

            // XamlBuildTask is unpredictable about which assemblies it needs referenced, but currently loaded assemblies in the AppDomain
            // should be sufficient (since the XAML-ified object is actually present in memory in this AppDomain). However, any assembly
            // referenced here will become a XAML dependency of the compiled workflow so we want to keep the set as small as we can.
            // Therefore, we take the intersection of (loaded assemblies) X (reference closure of assembliesUsedDirectly).
            // Use Caching if possible to include XAML dependencies, but merge with reflected dependencies to pick up 
            // built-in dependencies (which aren't stored in the DB or in ActivityAssemblyItems).
            AddReferencesInApplicationDomain(referencedAssemblies);

            return referencedAssemblies;
        }

        public static HashSet<Type> GetReferencedTypes(WorkflowDesigner workflowDesigner)
        {
            HashSet<Type> referencedTypes = new HashSet<Type>();
            ModelService modelService = workflowDesigner.GetModelService();
            //Get types used in the activities of the workflow
            referencedTypes.UnionWith(modelService.Find(modelService.Root, typeof(object)).Select(item => item.GetCurrentValue().GetType()));

            //Add types referenced in the root (e.g. parameters) to the collection         
            ActivityBuilder builder = modelService.Root.GetCurrentValue() as ActivityBuilder;

            if (builder != null)
            {
                referencedTypes.UnionWith(builder.Properties.Select(property => property.Type));
            }

            //Add base types and types of the parameters of every type in the list
            referencedTypes.UnionWith(GetBaseAndArgumentTypes(referencedTypes));

            return referencedTypes;
        }

        /// <summary>
        /// Add the base type and the type of arguments of a referenced type in the project 
        /// </summary>
        /// <param name="project">Project for the compile operation</param>
        /// <returns>List of discovered types</returns>
        private static IEnumerable<Type> GetBaseAndArgumentTypes(HashSet<Type> referencedTypes)
        {
            HashSet<Type> references = new HashSet<Type>();
            //Parallel.ForEach(referencedTypes, item => references.UnionWith(CheckGenericBaseAndArgumentTypes(item)));
            referencedTypes.ToList().ForEach(item => references.UnionWith(CheckGenericBaseAndArgumentTypes(item)));
            return references;
        }

        /// <summary>
        /// Recursively search for the base types and argument types of a referenced type. 
        /// </summary>
        /// <param name="item">Type that will be examined to get its referenced types</param>
        private static IEnumerable<Type> CheckGenericBaseAndArgumentTypes(Type item)
        {
            var types = new List<Type> { item };
            if (item.IsGenericType)
            {
                types.AddRange(item.GetGenericArguments().SelectMany(CheckGenericBaseAndArgumentTypes));
            }
            if (item.BaseType != null)
            {
                types.AddRange(CheckGenericBaseAndArgumentTypes(item.BaseType));
            }
            return types;
        }

        /// <summary>
        /// XAML and reflected dependencies of an assembly, recursively, if already loaded.
        /// </summary>
        /// <param name="project">Project for the compile operation</param>
        private static void AddReferencesInApplicationDomain(HashSet<Assembly> referencedAssemblies)
        {
            // Can't do simple recursion because some built-in assemblies have recursive references, e.g. System and 
            // System.Runtime.Serialization. So we optimize with a HashSet to ensure termination of the algorithm.
            var unprocessed = new HashSet<Assembly>(referencedAssemblies);

            while (unprocessed.Any())
            {
                Assembly assembly = unprocessed.First();

                //Look for the assembly in the local cache
                var activityAssemblyItem = AddInCaching.ActivityAssemblyItems.FirstOrDefault(cacheItem => cacheItem.Matches(assembly.GetName()));

                // Get the references of the assembly based on the cache and the current application domain.
                // optimization: if it's not loaded (domainReferenced == null), we don't need to load it/reference it
                var domainAssemblies =
                    from reference in (activityAssemblyItem != null ?
                        activityAssemblyItem.ReferencedAssemblies.Union(assembly.GetReferencedAssemblies(), referenceComparer) : assembly.GetReferencedAssemblies())
                    let domainReferenced = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(item => referenceComparer.Equals(item.GetName(), reference))
                    where domainReferenced != null
                    select domainReferenced;

                referencedAssemblies.Add(assembly);
                unprocessed.Remove(assembly);

                foreach (var domainAssembly in domainAssemblies)
                {
                    if (!referencedAssemblies.Contains(domainAssembly))
                    {
                        unprocessed.Add(domainAssembly);
                    }
                }
            }
        }
    }
}
