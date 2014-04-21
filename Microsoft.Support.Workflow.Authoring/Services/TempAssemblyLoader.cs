using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.Services
{
    public class TempAssemblyLoader : MarshalByRefObject
    {
         /// <summary>
        /// Default constructor.
        /// </summary>
        public TempAssemblyLoader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
        }

        /// <summary>
        /// When a reference cannot be resolved, we need to add it to the list of references to check.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        private Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return AssemblyService.Resolve(args.Name, AddInCaching.ActivityAssemblyItems);
        }
    }
}
