using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Microsoft.Support.Workflow.Authoring.CompositeActivity
{
    /// <summary>
    /// The structure of composite workflow node
    /// </summary>
    public class CompositeNode
    {
        private Activity originalActivity;

        /// <summary>
        /// Composite Activity object
        /// </summary>
        public Activity Activity
        {
            get
            {
                return originalActivity;
            }
            set
            {
                originalActivity = value;
                originalActivity.DisplayName = Name;
            }
        }

        /// <summary>
        /// Composite Actitvity assembley info
        /// </summary>
        public AssemblyName AssemblyName { get; set; }

        /// <summary>
        /// Child Composite Node
        /// </summary>
        public List<CompositeNode> Children { get; set; }

        public NodeKey Key { get; set; }

        public ModelItem OldItem { get; set; }

        public ModelItem NewItem { get; set; }

        public CompositeNode Parent { get; set; }

        public string Name { get; set; }

        public CompositeNode()
        {
            Children = new List<CompositeNode>();
        }

        public CompositeNode(string name, AssemblyName assemblyName, CompositeNode parent, ModelItem oldItem)
            : this()
        {
            Contract.Requires(name != null);
            Contract.Requires(assemblyName != null);

            Name = name;
            AssemblyName = assemblyName;
            Parent = parent;
            OldItem = oldItem;
            CreateKey();
        }

        private void CreateKey()
        {
            Key = NodeKeyManager.CreateKey(AssemblyName, Parent);
        }

    }
}
