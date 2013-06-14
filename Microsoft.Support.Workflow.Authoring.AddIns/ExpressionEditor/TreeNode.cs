using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.ExpressionEditor
{
    /// <summary>
    /// The structure of intellisense node 
    /// </summary>
    [Serializable]
    public class TreeNode : IComparable<TreeNode>
    {
        private const char genricChar = '`';
        private List<string> keywords = new List<string>();

        public List<TreeNode> Nodes { get; private set; }
        public TreeNode Parent { get; private set; }
        public string Name { get; private set; }
        public string FriendlyName { get; private set; }
        public TreeNodeType ItemType { get; private set; }
        public Type SystemType { get; private set; }
        public string Description
        {
            get
            {
                if (ItemType == TreeNodeType.Method)
                {
                    if (MethodInfoes.Length == 1)
                    {
                        description = MethodInfoes.Single();
                    }
                    else // have override method signatures
                    {
                        description = GetMethodDescriptionAt(0);
                    }
                }
                return description;
            }
            set
            {
                description = value;
            }
        }

        private string description;

        private string[] methodInfoes;
        public string[] MethodInfoes
        {
            get
            {
                return methodInfoes;
            }
            set
            {
                methodInfoes = value;
            }
        }

        public bool HasOverrideMethods
        {
            get
            {
                return methodInfoes != null && methodInfoes.Length > 1;
            }
        }

        public string GetMethodDescriptionAt(int index)
        {
            index = index % MethodInfoes.Length;
            if (index < 0)
            {
                index = index + MethodInfoes.Length;
            }

            return string.Format("({0} of {1}) {2}",
                            index + 1,
                            MethodInfoes.Length,
                            MethodInfoes[index]);
        }

        /// <summary>
        /// Consturctor
        /// </summary>
        public TreeNode()
            : this(null, null, default(TreeNodeType))
        {
        }

        /// <summary>
        /// Consturctor
        /// </summary>
        public TreeNode(TreeNode parent, string name, TreeNodeType nodeType, Type type = null)
        {
            Nodes = new List<TreeNode>();
            Parent = parent;
            Name = name;
            ItemType = nodeType;
            SystemType = type;

            if (!string.IsNullOrEmpty(Name) && Name.Contains(genricChar))
                FriendlyName = Name.Substring(0, Name.LastIndexOf(genricChar));
            else
                FriendlyName = Name;

            if (type != null && type.IsGenericType)
                Name = string.Format("{0}({1})", FriendlyName,
                    string.Join(", ", type.GetGenericArguments().Select(a => a.Name)));

            if (!string.IsNullOrEmpty(FriendlyName))
                for (int i = 0; i < FriendlyName.Length; i++)
                {
                    if (i == 0 || char.IsUpper(FriendlyName[i]))
                        keywords.Add(FriendlyName.Substring(i));
                }
        }

        /// <summary>
        /// Find the node to match the input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsMatch(string input)
        {
            return keywords.Any(k => k.StartsWith(input, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Add a node into the sub nodes list
        /// </summary>
        /// <param name="target"></param>
        public void AddNode(TreeNode target)
        {
            lock (this)
            {
                Nodes.Add(target);
            }
        }

        /// <summary>
        /// Get full path of a node
        /// </summary>
        /// <returns></returns>
        public string GetFullPath()
        {
            List<string> nodes = new List<string>() { Name };
            TreeNode parent = Parent;
            while (parent != null && !string.IsNullOrEmpty(parent.Name))
            {
                nodes.Insert(0, parent.Name);
                parent = parent.Parent;
            }

            return string.Join(".", nodes.ToArray());
        }

        /// <summary>
        /// Compare to another node
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(TreeNode other)
        {
            if (other == null)
                return 1;
            else
                return Name.CompareTo(other.Name);
        }

        /// <summary>
        /// Inherits from System.Object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
