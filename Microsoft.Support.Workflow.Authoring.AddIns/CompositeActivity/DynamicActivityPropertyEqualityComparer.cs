using System.Activities;
using System.Collections.Generic;

namespace Microsoft.Support.Workflow.Authoring.AddIns.CompositeActivity
{
    /// <summary>
    /// Compare two DynamicActivityProperty by Name
    /// </summary>
    public class DynamicActivityPropertyEqualityComparer : IEqualityComparer<DynamicActivityProperty>
    {
        public bool Equals(DynamicActivityProperty x, DynamicActivityProperty y)
        {
            return (x.Name == y.Name);
        }

        public int GetHashCode(DynamicActivityProperty arg)
        {
            return arg.Name.GetHashCode();
        }
    }
}
