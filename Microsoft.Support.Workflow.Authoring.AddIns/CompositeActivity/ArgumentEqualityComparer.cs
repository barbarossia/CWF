using System.Activities;
using System.Collections.Generic;

namespace Microsoft.Support.Workflow.Authoring.CompositeActivity
{
    /// <summary>
    /// Compare two Arguments by Name
    /// </summary>
    public class ArgumentEqualityComparer : IEqualityComparer<KeyValuePair<string, Argument>>
    {
        public bool Equals(KeyValuePair<string, Argument> x, KeyValuePair<string, Argument> y)
        {
            return (x.Key == y.Key);
        }

        public int GetHashCode(KeyValuePair<string, Argument> arg)
        {
            return arg.Key.GetHashCode();
        }
    }
}
