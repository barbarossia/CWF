using System;

namespace Microsoft.Support.Workflow.Authoring.CompositeActivity
{
    /// <summary>
    /// The structure of NodeKey
    /// </summary>
    public class NodeKey
    {
        public Guid Key { get; set; }
        /// <summary>
        /// AssemlbyName.FullName
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parent node's key
        /// </summary>
        public Guid Parent { get; set; }

        public bool IsSuccessfullyApplied { get; set; }

    }

}
