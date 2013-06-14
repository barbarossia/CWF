using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{

    /// <summary>
    /// Intended to be run inside a sub-AppDomain. Will load the assembly and pass the information
    /// to the caller in the main application AppDomain via out variables. We would just use a lambda
    /// instead, except lambdas aren't MarshalByRefObjects.
    /// </summary>
    public class AppDomain_AssemblyInformationGetter : MarshalByRefObject
    {
        public void LoadReflectedInfoInSubdomain(string location,
            Dictionary<Tuple<string, Version>, string> alreadyLocated,
            out List<ActivityItem> activityItems,
            ref List<AssemblyName> priorReferences, out bool notSafeForTypeLoad, out bool analysisIncomplete)
        {
            // get assembly and use it to compute activities and references
            var assembly = Assembly.ReflectionOnlyLoadFrom(location);
            var activityAssemblyItem = new ActivityAssemblyItem(assembly);
            var referenced = assembly.GetReferencedAssemblies().ToList();

            analysisIncomplete = false;
            if (assembly.GetType("XamlStaticHelperNamespace._XamlStaticHelper") != null)
            {
                var xamlRefs = GetXamlRefs(location, alreadyLocated, out analysisIncomplete);
                if (analysisIncomplete)
                {
                    // remember previous references
                    referenced.AddRange(priorReferences.Where(r => !referenced.Contains(r))); // code smell: smells like spaghetti code. Need a clean way of remembering previous XAML refs on a failure.
                }
                // Add unless we already have it
                referenced.AddRange(xamlRefs.Where(r => !referenced.Contains(r)));
            }

            // return values back to caller in other AppDomain
            activityItems = activityAssemblyItem.ActivityItems;
            priorReferences = referenced
                // Don't need to import built-in assemblies since everybody everywhere already has them as part of .NET or the authoring tool
                .Where(refName => !Utility.AssemblyIsBuiltIn(refName)).ToList();
            notSafeForTypeLoad = activityAssemblyItem.NotSafeForTypeLoad.Value;
        }

        private List<AssemblyName> GetXamlRefs(string location, Dictionary<Tuple<string, Version>, string> alreadyLocated,
            out bool analysisIncomplete)
        {
            // XAML dependencies don't show up in Assembly.GetReferencedAssemblies.
            // However, XamlBuildTask creates an internal class _XamlStaticHelper in every
            // XAML-compiled assembly which does know which assemblies it needs to load for 
            // the XAML to work. Normally this class is just called to create a SchemaContext
            // during InitializeComponent, but here in the authoring tool we will call it
            // directly via private reflection. We will keep track of every assembly it 
            // attempts to load, and those are the XamlRefs.
            var names = new List<AssemblyName>();

            AppDomain.CurrentDomain.AssemblyResolve += (sender, resolveEventArgs) =>
            {
                // TODO: If the user has located this assembly previously we should let Load() succeed so we can
                // get the next dependency.
                // TODO: make sure refs don't show up twice when XamlStaticHelper retries
                var newName = ActivityAssemblyItem.TreatUnsignedAsUnversioned(new AssemblyName(resolveEventArgs.Name));
                // XamlStaticLoader will retry without version info if the first try fails, but
                // we only want the first one (with full version info) to show up in our
                // dependency list
                string loc;
                if (alreadyLocated.TryGetValue(Tuple.Create(newName.Name, newName.Version), out loc))
                {
                    return Assembly.LoadFrom(loc);
                }

                if (!names.Any(n => n.Name == newName.Name))
                {
                    names.Add(newName);
                }
                return null;
            };

            // Create non-ReflectionOnly assembly so we can run XamlStaticHelperNamespace._XamlStaticHelper.LoadAssemblies
            var asm = Assembly.LoadFrom(location);
            var xamlStaticHelperType = asm.GetType("XamlStaticHelperNamespace._XamlStaticHelper");
            try
            {
                var loadedAssemblies = xamlStaticHelperType.InvokeMember("LoadAssemblies", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, null, new object[0]) as IEnumerable<Assembly>;

                // Any XAML-referenced assemblies which were successfully loaded (e.g. because they were in the same directory), except for
                // the assembly we are analyzing (it is not its own dependency).
                names.AddRange(from loadedAssembly in loadedAssemblies where loadedAssembly != asm select loadedAssembly.GetName());
                analysisIncomplete = false;
                return names.Distinct().ToList();
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException is FileNotFoundException)
                {
                    // TODO: Need to tell Import that analysis is not complete yet, user needs to provide more info and then we can retry for next dependency.
                    analysisIncomplete = true;
                    return names;
                }
                else
                {
                    throw;
                }
            }
        }
    }


}
