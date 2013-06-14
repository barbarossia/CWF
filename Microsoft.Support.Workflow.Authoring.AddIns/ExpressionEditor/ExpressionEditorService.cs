using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Presentation.View;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Hosting;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Support.Workflow.Authoring.ExpressionEditor
{
    /// <summary>
    /// Represents an expression editor instance.
    /// </summary>
    public class ExpressionEditorService : IExpressionEditorService
    {
        public TreeNode IntellisenseNode { get; set; }
        public string EditorKeyWord { get; set; }

        private readonly Dictionary<string, ExpressionEditorInstance> editorInstances = new Dictionary<string, ExpressionEditorInstance>();
        private object _intellisenseSyncLock = new object();

        /// <summary>
        /// Constructor
        /// </summary>
        public void CloseExpressionEditors()
        {
            foreach (var childEditor in editorInstances.Values)
            {
                childEditor.LostAggregateFocus -= LostFocus;
                childEditor.Close();
            }

            editorInstances.Clear();
        }

        /// <summary>
        /// Get an instance of IExpressionEditorInstance
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="importedNamespaces"></param>
        /// <param name="variables"></param>
        /// <param name="text"></param>
        /// <param name="initialSize"></param>
        /// <returns></returns>
        public IExpressionEditorInstance CreateExpressionEditor(AssemblyContextControlItem assemblies, ImportedNamespaceContextItem importedNamespaces, List<System.Activities.Presentation.Model.ModelItem> variables, string text, System.Windows.Size initialSize)
        {
            return CreateExpressionEditorPrivate(assemblies, importedNamespaces, variables, text, null, initialSize);
        }

        /// <summary>
        /// Get an instance of IExpressionEditorInstance
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="importedNamespaces"></param>
        /// <param name="variables"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public IExpressionEditorInstance CreateExpressionEditor(AssemblyContextControlItem assemblies, ImportedNamespaceContextItem importedNamespaces, List<System.Activities.Presentation.Model.ModelItem> variables, string text)
        {
            return CreateExpressionEditorPrivate(assemblies, importedNamespaces, variables, text, null, Size.Empty);
        }

        /// <summary>
        /// Get an instance of IExpressionEditorInstance
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="importedNamespaces"></param>
        /// <param name="variables"></param>
        /// <param name="text"></param>
        /// <param name="expressionType"></param>
        /// <param name="initialSize"></param>
        /// <returns></returns>
        public IExpressionEditorInstance CreateExpressionEditor(AssemblyContextControlItem assemblies, ImportedNamespaceContextItem importedNamespaces, List<System.Activities.Presentation.Model.ModelItem> variables, string text, Type expressionType, System.Windows.Size initialSize)
        {
            return CreateExpressionEditorPrivate(assemblies, importedNamespaces, variables, text, expressionType,
                                                 initialSize);
        }

        /// <summary>
        /// Get an instance of IExpressionEditorInstance
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="importedNamespaces"></param>
        /// <param name="variables"></param>
        /// <param name="text"></param>
        /// <param name="expressionType"></param>
        /// <returns></returns>
        public IExpressionEditorInstance CreateExpressionEditor(AssemblyContextControlItem assemblies, ImportedNamespaceContextItem importedNamespaces, List<System.Activities.Presentation.Model.ModelItem> variables, string text, Type expressionType)
        {
            return CreateExpressionEditorPrivate(assemblies, importedNamespaces, variables, text, expressionType, Size.Empty);
        }

        /// <summary>
        /// Update context
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="importedNamespaces"></param>
        public void UpdateContext(AssemblyContextControlItem assemblies, ImportedNamespaceContextItem importedNamespaces)
        {
        }

        private IExpressionEditorInstance CreateExpressionEditorPrivate(AssemblyContextControlItem assemblies, ImportedNamespaceContextItem importedNamespaces, List<ModelItem> modelItems, string text, Type expressionType, Size initialSize)
        {
            var instance = new ExpressionEditorInstance();
            instance.Guid = Guid.NewGuid();
            instance.Text = text;
            instance.HighlightWords = EditorKeyWord;
            instance.ExpressionType = expressionType;
            instance.IntellisenseNodeList = CreateUpdatedIntellisense(modelItems);
            instance.LostAggregateFocus += LostFocus;

            editorInstances.Add(instance.Guid.ToString(), instance);
            return instance;
        }

        private TreeNode CreateUpdatedIntellisense(List<ModelItem> modelItems)
        {
            TreeNode result = new TreeNode();
            result.Nodes.AddRange(IntellisenseNode.Nodes);
            lock (_intellisenseSyncLock)
            {
                foreach (ModelItem item in modelItems)
                {
                    var modelProperty = item.Properties["Name"];
                    if (modelProperty == null)
                    {
                        continue;
                    }

                    var name = modelProperty.ComputedValue.ToString();
                    var res = from node in result.Nodes
                              where node.Name == name
                              select node;

                    if (res.FirstOrDefault() == null)
                    {
                        Type systemType = null;
                        var sysTypeProp = item.Properties["Type"];
                        if (sysTypeProp != null)
                        {
                            systemType = sysTypeProp.ComputedValue as Type;

                            // resolve TreeNodeType and Description format from systemType
                            TreeNodeType nodeType;
                            string descriptionFormat;
                            ExpressionEditorHelper.ResolveType(systemType, out nodeType, out descriptionFormat);

                            // construct TreeNode from this type
                            TreeNode itemNode = new TreeNode(null, name, nodeType, systemType);
                            CloneTreeNode(itemNode, ExpressionEditorHelper.SearchNodes(result, systemType.FullName));
                            itemNode.Description = string.Format(descriptionFormat, systemType.Name);
                            result.Nodes.Add(itemNode);
                        }
                    }
                }
            }
            return result;
        }

        private void CloneTreeNode(TreeNode target, TreeNode source)
        {
            if (source != null)
            {
                foreach (TreeNode node in source.Nodes)
                {
                    TreeNode copy = new TreeNode(target, node.Name, node.ItemType, node.SystemType)
                    {
                        Description = node.Description,
                        MethodInfoes = node.MethodInfoes
                    };
                    target.Nodes.Add(copy);
                    CloneTreeNode(copy, node);
                }
            }
        }

        private void LostFocus(object sender, EventArgs e)
        {
            var editorTextBox = sender as TextBox;
            if (editorTextBox != null)
            {
                DesignerView.CommitCommand.Execute(editorTextBox.Text);
            }
        }
    }
}
