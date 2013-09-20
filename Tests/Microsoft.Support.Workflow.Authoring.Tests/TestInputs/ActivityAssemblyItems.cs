using System.Collections.Generic;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Microsoft.Support.Workflow.Authoring.Tests.TestInputs
{
    public static partial class TestInputs{
        
        public static class ActivityAssemblyItems
        {
            public static ActivityAssemblyItem TestInput_NoActivityLibrary
            {
                get {
                    return new ActivityAssemblyItem(Assemblies.TestInput_NoActivityLibrary);
                }
            }

            public static ActivityAssemblyItem TestInput_Lib1
            {
                get { return new ActivityAssemblyItem(Assemblies.TestInput_Lib1); }
            }
            public static ActivityAssemblyItem TestInput_Lib2
            {
                get
                {
                    var item = new ActivityAssemblyItem(Assemblies.TestInput_Lib2);
                    item.ReferencedAssemblies.Add(new AssemblyName(Assemblies.TestInput_Lib1.FullName));
                    return item;
                }
            }
            public static ActivityAssemblyItem TestInput_Lib3
            {
                get
                {
                    var item = new ActivityAssemblyItem(Assemblies.TestInput_Lib3);
                    item.ReferencedAssemblies.Add(new AssemblyName(Assemblies.TestInput_Lib2.FullName));
                    return item;
                }
            }
        }
    }
}
