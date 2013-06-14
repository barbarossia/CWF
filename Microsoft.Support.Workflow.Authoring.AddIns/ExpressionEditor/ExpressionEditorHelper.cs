using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using System.IO;
using Microsoft.Support.Workflow.Authoring.AddIns;
using System.Activities;
using System.ServiceModel.Activities;

namespace Microsoft.Support.Workflow.Authoring.ExpressionEditor
{
    /// <summary>
    /// Helper to reflect assemblies
    /// </summary>
    public static class ExpressionEditorHelper
    {
        private const string appKeyIncludeImports = "IncludeImportLibrariesInIntellisense";
        private static readonly bool shouldIncludeImports = bool.Parse(ConfigurationManager.AppSettings[appKeyIncludeImports] == null ? "true" : ConfigurationManager.AppSettings[appKeyIncludeImports]);
        private static readonly BindingFlags[] bindingFlags = new BindingFlags[] {
            BindingFlags.Public | BindingFlags.Static,
            BindingFlags.Public | BindingFlags.Instance,
        };
        private static readonly string[] keywords = new string[] {
            "AddressOf",
            "Aggregate",
            "Boolean",
            "Byte",
            "Char",
            "Date",
            "Decimal",
            "Double",
            "From",
            "Function",
            "Integer",
            "Long",
            "Mid",
            "New",
            "Not",
            "Nothing",
            "Object",
            "SByte",
            "Short",
            "Single",
            "String",
            "Sub",
            "TypeOf",
            "UInteger",
            "ULong",
            "UShort",
        };

        private static object intellisenseNodeLock = new object();
        private static TreeNode intellisenseNodeList = null;
        private static List<ActivityAssemblyItem> reflectedAssemblies = null;

        /// <summary>
        /// Clean the intellisense node
        /// </summary>
        public static void ClearIntellisenseList()
        {
            intellisenseNodeList = null;
        }

        /// <summary>
        /// Create the root node of all assemblies
        /// </summary>
        /// <returns></returns>
        public static TreeNode CreateIntellisenseList()
        {
            lock (intellisenseNodeLock)
            {
                if (intellisenseNodeList == null)
                {
                    intellisenseNodeList = new TreeNode();
                    IEnumerable<AssemblyName> refAsmNames = GetReferencesFunc() ?? new AssemblyName[0];
                    if (shouldIncludeImports)
                    {
                        reflectedAssemblies = AddInCaching.ActivityAssemblyItems.ToList();
                        refAsmNames = refAsmNames.Union(AddInCaching.ActivityAssemblyItems.Select(a => a.AssemblyName)).Distinct();
                    }
                    AddToList(refAsmNames);

                    foreach (string keyword in keywords)
                    {
                        TreeNode childNodes = new TreeNode(
                            intellisenseNodeList,
                            keyword,
                            TreeNodeType.Primitive);
                        childNodes.Description = string.Empty;
                        intellisenseNodeList.AddNode(childNodes);
                    }
                    SortNodes(intellisenseNodeList);
                }
                else if (shouldIncludeImports)
                {
                    // refresh intellisense node from cached assemblies
                    var assembliesToLoad = AddInCaching.ActivityAssemblyItems
                        .Except(reflectedAssemblies, new ActivityAssemblyItemComparer()).ToList();
                    if (assembliesToLoad.Any())
                    {
                        reflectedAssemblies = AddInCaching.ActivityAssemblyItems.ToList();
                        AddToList(assembliesToLoad.Select(a => a.AssemblyName));
                        SortNodes(intellisenseNodeList);
                    }
                }
                return intellisenseNodeList;
            }
        }

        public static Func<IEnumerable<AssemblyName>> GetReferencesFunc = (() =>
        {
            return new[] { 
                typeof(int).Assembly.GetName(), // mscorlib
                typeof(Uri).Assembly.GetName(), // System
                typeof(Activity).Assembly.GetName(), // System.Activities
                typeof(System.ServiceModel.BasicHttpBinding).Assembly.GetName(), // System.ServiceModel 
                typeof(CorrelationHandle).Assembly.GetName(), // System.ServiceModel.Activities
            };
        });

        private static void AddToList(IEnumerable<AssemblyName> refAsmNames)
        {
            var typeList = refAsmNames.SelectMany(assemblyName =>
            {
                var assembly = Assembly.Load(assemblyName);
                var result = from x in assembly.GetTypes()
                             where x.IsPublic && x.IsVisible &&
                                 ((x.Namespace != null) &&
                                 (!x.Namespace.Contains(((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(
                                    Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false))
                                    .Company)))
                             select x;
                return result.ToList();
            });

            var actualTypes = new HashSet<Type>(typeList);
            foreach (var asmType in actualTypes)
            {
                var childAsm = asmType;
                AddNode(intellisenseNodeList, childAsm.Namespace);
                AddTypeNode(intellisenseNodeList, childAsm);
            }
        }

        private static void AddNode(TreeNode targetNodes, string namePath)
        {
            var targetPath = namePath.Split('.');
            bool validPath = false;

            TreeNode existsNodes = null;

            var validNode = targetNodes.Nodes.Where(x => x.Name.Equals(targetPath[0], StringComparison.OrdinalIgnoreCase));

            if (validNode.Any())
            {
                existsNodes = validNode.FirstOrDefault();
                validPath = true;
            }

            if (!validPath)
            {
                TreeNode childNodes = new TreeNode(
                    targetNodes,
                    targetPath[0],
                    TreeNodeType.Namespace);
                childNodes.Description = string.Format("Namespace {0}", childNodes.FriendlyName);
                targetNodes.AddNode(childNodes);

                var nextPath = namePath.Substring(targetPath[0].Length, namePath.Length - targetPath[0].Length);
                if (nextPath.StartsWith("."))
                {
                    nextPath = nextPath.Substring(1, nextPath.Length - 1);
                }
                if (!string.IsNullOrEmpty(nextPath))
                {
                    AddNode(childNodes, nextPath);
                }
            }
            else
            {
                var nextPath = namePath.Substring(targetPath[0].Length, namePath.Length - targetPath[0].Length);

                if (nextPath.StartsWith("."))
                {
                    nextPath = nextPath.Substring(1, nextPath.Length - 1);
                }

                if (!string.IsNullOrEmpty(nextPath))
                {
                    AddNode(existsNodes, nextPath);
                }
            }
        }

        private static void AddTypeNode(TreeNode targetNodes, Type target)
        {
            if (target.IsAbstract || !target.IsVisible)
            {
                return;
            }

            // resolve TreeNodeType and Description format from this type
            TreeNodeType nodeType;
            string descriptionFormat;
            ResolveType(target, out nodeType, out descriptionFormat);

            // construct TreeNode from this type
            TreeNode parentNode = SearchNodes(targetNodes, target.Namespace);
            TreeNode newNodes = new TreeNode(
                parentNode,
                target.Name,
                nodeType,
                target);
            newNodes.Description = string.Format(descriptionFormat, newNodes.FriendlyName);
            parentNode.AddNode(newNodes);

            // add sub members to this TreeNode
            AddMethodNode(newNodes, target);
            AddPropertyNode(newNodes, target);
            AddFieldNode(newNodes, target);
            AddEventNode(newNodes, target);
            AddNestedTypeNode(newNodes, target);
        }

        /// <summary>
        /// Resolves TreeNodeType and Description format from input Type
        /// </summary>
        /// <param name="target"></param>
        /// <param name="nodeType"></param>
        /// <param name="descriptionFormat"></param>
        public static void ResolveType(Type target, out TreeNodeType nodeType, out string descriptionFormat)
        {
            nodeType = default(TreeNodeType);
            descriptionFormat = "{0}";

            if (target.IsClass)
            {
                nodeType = TreeNodeType.Class;
                descriptionFormat = "Class {0}";
            }
            else if (target.IsEnum)
            {
                nodeType = TreeNodeType.Enum;
                descriptionFormat = "Enum {0}";
            }
            else if (target.IsInterface)
            {
                nodeType = TreeNodeType.Interface;
                descriptionFormat = "Interface {0}";
            }
            else if (target.IsPrimitive)
            {
                nodeType = TreeNodeType.Primitive;
            }
            else if (target.IsValueType)
            {
                nodeType = TreeNodeType.ValueType;
            }
        }

        private static void AddMethodNode(TreeNode targetNodes, Type target)
        {
            Parallel.ForEach(
                bindingFlags.SelectMany(f => target.GetMethods(f)).GroupBy(t => t.Name),
                targetmember =>
                {
                    var memberNodes = new TreeNode(
                        targetNodes,
                        targetmember.Key,
                        TreeNodeType.Method,
                        targetmember.First().ReturnType);
                    memberNodes.MethodInfoes = CreateMethodDescription(targetmember.Select(group => group)).ToArray();
                    targetNodes.AddNode(memberNodes);
                });
        }

        private static List<string> CreateMethodDescription(IEnumerable<MethodInfo> targets)
        {
            List<string> descs = new List<string>();
            foreach (var target in targets)
                descs.Add(CreateMethodDescription(target));
            if (descs.Count > 1)
                descs = descs.Distinct().ToList();
            
            return descs;
        }

        private static void AddPropertyNode(TreeNode targetNodes, Type target)
        {
            Parallel.ForEach(
                bindingFlags.SelectMany(f => target.GetProperties(f)),
                targetmember =>
                {
                    var memberNodes = new TreeNode(
                        targetNodes,
                        targetmember.Name,
                        TreeNodeType.Property,
                        targetmember.PropertyType);
                    memberNodes.Description = CreatePropertyDescription(targetmember);
                    targetNodes.AddNode(memberNodes);
                });
        }

        private static void AddFieldNode(TreeNode targetNodes, Type target)
        {
            Parallel.ForEach(
                bindingFlags.SelectMany(f => target.GetFields(f)),
                targetmember =>
                {
                    var memberNodes = new TreeNode(
                        targetNodes,
                        targetmember.Name,
                        TreeNodeType.Field,
                        targetmember.FieldType);
                    memberNodes.Description = CreateFieldDescription(targetmember);
                    targetNodes.AddNode(memberNodes);
                });
        }

        private static void AddEventNode(TreeNode targetNodes, Type target)
        {
            Parallel.ForEach(
                bindingFlags.SelectMany(f => target.GetEvents(f)),
                targetmember =>
                {
                    var memberNodes = new TreeNode(
                        targetNodes,
                        targetmember.Name,
                        TreeNodeType.Event);
                    memberNodes.Description = CreateEventDescription(targetmember);
                    targetNodes.AddNode(memberNodes);
                });
        }

        private static void AddNestedTypeNode(TreeNode targetNodes, Type target)
        {
            Parallel.ForEach(
                bindingFlags.SelectMany(f => target.GetNestedTypes(f)),
                targetmember =>
                {
                    var memberNodes = new TreeNode(
                        targetNodes,
                        targetmember.Name,
                        TreeNodeType.Method);
                    targetNodes.AddNode(memberNodes);
                });
        }

        private static string CreateMethodDescription(MethodInfo target)
        {
            var desc = new StringBuilder();

            if (target.IsPublic)
            {
                desc.Append("Public ");
            }

            if (target.IsFamily)
            {
                desc.Append("Protected ");
            }

            if (target.IsAssembly)
            {
                desc.Append("Friend ");
            }

            if (target.IsPrivate)
            {
                desc.Append("Private ");
            }

            if (target.IsAbstract)
            {
                desc.Append("MustOverride ");
            }

            if (target.IsVirtual && !target.IsFinal)
            {
                desc.Append("Overridable ");
            }

            if (target.IsStatic)
            {
                desc.Append("Shared ");
            }

            desc.Append(target.ReturnType != typeof(void) ? "Function " : "Sub ");
            desc.Append(target.Name);
            desc.Append(CreateGenericParameter(target));

            desc.Append("(");
            int paramIndex = 0;
            foreach (var param in target.GetParameters())
            {
                if (paramIndex > 0)
                {
                    desc.Append(", ");
                }

                if (param.IsOptional)
                {
                    desc.Append("Optional ");
                }
                desc.Append(param.IsOut ? "ByRef " : "ByVal ");

                desc.Append(param.Name + " As " + param.ParameterType.Name);
                desc.Append(CreateGenericParameter(param.ParameterType));

                if (param.DefaultValue != DBNull.Value)
                {
                    if (param.DefaultValue == null)
                    {
                        desc.Append(" = Nothing");
                    }
                    else
                    {
                        desc.Append(" = " + param.DefaultValue);
                    }
                }
                paramIndex++;
            }

            desc.Append(") ");
            desc.Append("As " + target.ReturnType.Name);
            desc.Append(CreateGenericParameter(target.ReturnType));
            return desc.ToString();
        }

        private static string CreatePropertyDescription(PropertyInfo target)
        {
            var desc = new StringBuilder();

            if (target.CanRead && !target.CanWrite)
            {
                desc.Append("ReadOnly ");
            }
            if (target.CanWrite && !target.CanRead)
            {
                desc.Append("WriteOnly ");
            }

            desc.Append("Property " + target.Name + " As " + target.PropertyType.Name);
            desc.Append(CreateGenericParameter(target.PropertyType));
            return desc.ToString();
        }

        private static string CreateFieldDescription(FieldInfo target)
        {
            var desc = new StringBuilder();

            if (target.IsPublic)
            {
                desc.Append("Public ");
            }

            if (target.IsPrivate)
            {
                desc.Append("Private ");
            }

            if (target.IsStatic)
            {
                desc.Append("Shared ");
            }

            desc.Append(target.Name);
            desc.Append("() ");
            desc.Append("As " + target.FieldType.Name);
            desc.Append(CreateGenericParameter(target.FieldType));

            return desc.ToString();
        }

        private static string CreateEventDescription(EventInfo target)
        {
            var desc = new StringBuilder();

            desc.Append(target.Name);
            desc.Append("As " + target.EventHandlerType.Name);
            if (target.EventHandlerType.IsGenericType)
            {
                desc.Append(CreateGenericParameter(target.EventHandlerType));
            }
            return desc.ToString();
        }

        private static string CreateGenericParameter(MethodInfo target)
        {
            var result = new StringBuilder();
            if (target.IsGenericMethod)
            {
                result.Append("(Of ");
                int genIndex = 0;
                foreach (var genParam in target.GetGenericArguments())
                {
                    if (genIndex > 0)
                    {
                        result.Append(", ");
                    }
                    result.Append(genParam.Name);
                    genIndex++;
                }
                result.Append(")");
            }
            return result.ToString();
        }

        private static string CreateGenericParameter(Type target)
        {
            var result = new StringBuilder();
            if (target.IsGenericType)
            {
                result.Append("(Of ");
                int genIndex = 0;
                foreach (var genParam in target.GetGenericArguments())
                {
                    if (genIndex > 0)
                    {
                        result.Append(", ");
                    }
                    result.Append(genParam.Name);
                    genIndex++;
                }
                result.Append(")");
            }
            return result.ToString();
        }

        /// <summary>
        /// Search the node which matches with input path
        /// </summary>
        /// <param name="targetNode"></param>
        /// <param name="namePath"></param>
        /// <returns></returns>
        public static TreeNode SearchNodes(TreeNode targetNode, string namePath)
        {
            var targetPath = namePath.Split('.');
            bool validPath = false;
            TreeNode existsNodes = null;

            var validNodes = from node in targetNode.Nodes
                             where node.Name.Equals(targetPath[0], StringComparison.OrdinalIgnoreCase)
                             select node;

            if (validNodes.Any())
            {
                existsNodes = validNodes.FirstOrDefault();
                validPath = true;
            }

            if (!validPath)
            {
                return null;
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

            return SearchNodes(existsNodes, nextPath);
        }

        private static void SortNodes(TreeNode targetNodes)
        {
            targetNodes.Nodes.Sort();
            foreach (var childNode in targetNodes.Nodes)
            {
                SortNodes(childNode);
            }
        }

        private class ActivityAssemblyItemComparer : IEqualityComparer<ActivityAssemblyItem>
        {
            public bool Equals(ActivityAssemblyItem x, ActivityAssemblyItem y)
            {
                return x.AssemblyName.FullName == y.AssemblyName.FullName;
            }

            public int GetHashCode(ActivityAssemblyItem obj)
            {
                return obj.AssemblyName.FullName.GetHashCode();
            }
        }
    }
}
