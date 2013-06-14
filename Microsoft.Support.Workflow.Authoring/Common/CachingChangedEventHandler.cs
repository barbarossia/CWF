using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Support.Workflow.Authoring.Common
{
    public class CachingChangedEventArgs : EventArgs
    {
        public IEnumerable<AssemblyName> Assemblies { get; set; }

        public CachingChangedEventArgs(IEnumerable<AssemblyName> assemblies)
        {
            this.Assemblies = assemblies;
        }
    }
    public delegate void CachingChangedEventHandler(object sender, CachingChangedEventArgs e);
}
