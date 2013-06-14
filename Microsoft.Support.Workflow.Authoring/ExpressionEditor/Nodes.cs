using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.ExpressionEditor
{
    public class TreeNodes
    {
        public enum NodeTypes
        {
            Namespace = 0,
            Interface = 1,
            Class = 2,
            Method = 3,
            Property = 4,
            Field = 5,
            Enum = 6,
            ValueType = 7,
            Event = 8,
            Primitive = 9
        }

        private List<TreeNodes> nodes = new List<TreeNodes>();

        public string Name { get; set; }
        public string AddStrings { get; set; }
        public NodeTypes ItemType { get; set; }
        public Type SystemType { get; set; }
        public string Description { get; set; }
        public TreeNodes Parent { get; set; }

        private object _syncLock = new object();

        public List<TreeNodes> Nodes
        {
            get { return nodes; }
        }
    
        public void AddNode(TreeNodes target)
        {
            
            lock(_syncLock)
            {
                nodes.Add(target);
            }
        }

        public string GetFullPath()
        {
            string result = this.Name;
            if (Parent != null)
            {
                string parentString = Parent.GetFullPath();
                if (!string.IsNullOrEmpty(parentString))
                {
                    result = parentString + "." + result;
                }
            }

            return result;
        }

        public TreeNodes SearchNodes(string namePath)
        {
            return SearchNodesInPrivate(this, namePath);
        }
        
        private TreeNodes SearchNodesInPrivate(TreeNodes targetNodes, string namePath)
        {
            var targetPath = namePath.Split('.');
            var validPath = false;
            TreeNodes existsNodes = null;

            var validNode = from x in targetNodes.Nodes
                            where x.Name.ToLower() == targetPath[0].ToLower()
                            select x;

            if(validNode.Any())
            {
                existsNodes = validNode.FirstOrDefault();
                validPath = true;
            }

            if (!validPath)
            {
                return targetNodes;
            }

            var nextPath = namePath.Substring(targetPath[0].Length, namePath.Length - targetPath[0].Length);
            if (nextPath.StartsWith("."))
            {
                nextPath = nextPath.Substring(1, nextPath.Length - 1);
            }

            if (string.IsNullOrEmpty(nextPath))
            {
                return existsNodes;
            }

            return SearchNodesInPrivate(existsNodes, nextPath);
        }
    }
}
