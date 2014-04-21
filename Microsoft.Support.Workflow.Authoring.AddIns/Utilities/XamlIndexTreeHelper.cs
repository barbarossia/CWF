using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Utilities
{
    public static class XamlIndexTreeHelper
    {
        public static List<XamlIndexNode> Nodes { get; private set; }

        public static void CreateIndexTree(string xaml)
        {
            if (Nodes == null)
            {
                Create(xaml);
            }
        }

        public static void Refresh(string xaml)
        {
            Create(xaml);
        }

        private static void Create(string xaml)
        {
            if (string.IsNullOrWhiteSpace(xaml))
                return;

            List<XamlIndexNode> result = new List<XamlIndexNode>();
            XamlIndexNode current = null;
            XamlIndexNode parent = null;
            Stack<XamlIndexNode> stack = new Stack<XamlIndexNode>();
            int offset = 0;
            string node = string.Empty;
            char x;

            var root = new XamlIndexNode() { Name = "root" };
            stack.Push(root);
            current = root;

            for (int i = 0; i < xaml.Length; i++)
            {
                x = xaml[i];
                switch (x)
                {
                    case '<':
                        offset = i;
                        break;
                    case '>':
                        node = xaml.Substring(offset, i - offset + 1);
                        if (current != null && node.Contains("/" + current.Name))
                        {
                            current.Length = i - current.Offset + 1;
                            current.Name = current.Name.GetTypeName();
                            result.Add(current);
                            stack.Pop();
                        }
                        else if (node.Contains("/>"))
                        {
                            parent = current;
                            XamlIndexNode newNode = new XamlIndexNode()
                            {
                                Offset = offset,
                                Length = i - offset + 1,
                                Parent = parent.Offset,
                                Name = node.GetName().GetTypeName(),
                                DisplayName = node.GetDisplayName(),
                            };

                            result.Add(newNode);
                        }
                        else
                        {
                            parent = stack.Peek();

                            current = new XamlIndexNode()
                            {
                                Offset = offset,
                                Parent = parent.Offset,
                                Name = node.GetName(),
                                DisplayName = node.GetDisplayName(),
                            };

                            stack.Push(current);
                        }

                        current = stack.Peek();
                        node = string.Empty;
                        break;
                }
            }

            Nodes = result;
        }

        public static XamlIndexNode Search(WorkflowOutlineNode activity)
        {
            if(Nodes == null)
                throw new ArgumentNullException();

            return Search(activity, Nodes);
        }

        private static string GetWorkflowOutlineTypeName(WorkflowOutlineNode node)
        {
            return node.ActivityType.Name;
        }

        private static XamlIndexNode Search(WorkflowOutlineNode activity, List<XamlIndexNode> nodes)
        {
            if (activity == null)
            {
                throw new ApplicationException("selection is null.");
            }

            List<XamlIndexNode> candidate = Match(GetWorkflowOutlineTypeName(activity), activity.NodeName);
            int count = candidate.Count;
            if (count == 1)
            {
                return candidate.Single();
            }
            else if (count > 1)
            {
                XamlIndexNode parent = Search(activity.Parent, candidate);
                return FilterInChildren(parent, candidate, activity);
            }
            else
            {
                throw new ApplicationException(string.Format(TextResources.NoMatchOfSelectionMsgFormat, activity.NodeName));
            }
        }

        private static XamlIndexNode FilterInChildren(XamlIndexNode parent, List<XamlIndexNode> children, WorkflowOutlineNode activity)
        {
            var descendant = RetrieveChildrenOrDescendant(parent, children);
            return FilterInChildren(descendant, activity);
        }

        private static XamlIndexNode FilterInChildren(List<XamlIndexNode> children, WorkflowOutlineNode activity)
        {
            if (children.Count == 1)
                return children[0];
            else
            {
                int index = GetChildrenIndex(activity);
                return children.Where(c => c.Name == GetWorkflowOutlineTypeName(activity)).ToList()[index];
            }
        }

        private static int GetChildrenIndex(WorkflowOutlineNode activity)
        {
            List<WorkflowOutlineNode> sameTypeActivies;
            if (activity.PropertyNameOfNodeName == "DisplayName" &&
                activity.NodeName != GetWorkflowOutlineTypeName(activity))
            {
                sameTypeActivies = activity.Parent.Children.Where(c => c.NodeName == activity.NodeName).ToList();
            }
            else
            {
                sameTypeActivies = activity.Parent.Children.Where(c => GetWorkflowOutlineTypeName(c) == GetWorkflowOutlineTypeName(activity)).ToList();
            }

            return sameTypeActivies.IndexOf(activity);
        }

        private static List<XamlIndexNode> RetrieveChildrenOrDescendant(XamlIndexNode parent, List<XamlIndexNode> children)
        {
            List<XamlIndexNode> result = new List<XamlIndexNode>();
            var query = from c in children
                        where c.Parent == parent.Offset
                        select c;
            if (query.Any())
            {
                result = query.ToList();
            }
            else
            {
                var descendants = from c in Nodes
                                  where c.Parent == parent.Offset
                                  select c;

                foreach (var descendant in descendants)
                {
                    List<XamlIndexNode> childrenOfDescendant = RetrieveChildrenOrDescendant(descendant, children);
                    result.AddRange(childrenOfDescendant);
                }
            }

            return result;
        }

        private static List<XamlIndexNode> Match(string type, string displayName)
        {
            string typeName = type.RemoveGenericsName();
            if (typeName == displayName)
                return Match(typeName);

            var query = from x in Nodes
                        where x.Name == typeName && x.DisplayName == displayName
                        select x;
            if (!query.Any())
                return Match(typeName);

            return query.ToList();
        }

        private static List<XamlIndexNode> Match(string type)
        {
            return (from x in Nodes
                    where x.Name == type
                    select x).ToList();
        }

    }
}
