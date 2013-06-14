using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Support.Workflow.Authoring.Models;

namespace Microsoft.Support.Workflow.Authoring.Services
{
    public class ProxyDomain : MarshalByRefObject
    {
        private readonly HashSet<AssemblyName> referencedAssemblies;

        public ProxyDomain()
        {
            referencedAssemblies = new HashSet<AssemblyName>();
            //Event that fires when an assembly cannot be resolved
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
        }
        public Assembly GetAssembly(string AssemblyPath)
        {
            try
            {
                //return Assembly.LoadFrom(AssemblyPath);
                return Assembly.ReflectionOnlyLoadFrom(AssemblyPath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("", ex);
            }
        }

        public Assembly GetAssembly(AssemblyName asm)
        {
            try
            {
                return Assembly.Load(asm);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("", ex);
            }
        }

        Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs eventArgs)
        {
            var referenceName = ActivityAssemblyItem.TreatUnsignedAsUnversioned(new AssemblyName(eventArgs.Name));
            referencedAssemblies.Add(referenceName);
            return null;
        }
    }
}
