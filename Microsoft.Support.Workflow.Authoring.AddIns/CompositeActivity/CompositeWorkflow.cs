using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

namespace Microsoft.Support.Workflow.Authoring.CompositeActivity
{
    /// <summary>
    /// The class deal with composite workflow function
    /// </summary>
    public sealed class CompositeWorkflow
    {
        #region Constant

        private static readonly string MSG_UpdateOtherActivities = TextResources.UpdateOthersActivitiesMsgFormat;
        private static readonly string Title_UpdateOtherActivities = TextResources.UpdateActivities;
        private static readonly string MSG_UpdateReferenceInSelf = TextResources.CannotAddActivitiesSelfMsgFormat;
        private static readonly string MSG_NoUpdatedActivity = TextResources.NoActivitesUpdatedMsg;
        private const string ActivityNamePattern = "{0}_{1}";

        #endregion

        #region Public functions

        /// <summary>
        /// Response the model items added
        /// </summary>
        public void ModelService_ItemsAdded(IEnumerable<ModelItem> addedItems)
        {
            Contract.Requires(addedItems!=null);

            foreach (var model in addedItems)
            {
                Add(model);

            }
        }

        /// <summary>
        /// Response the model Properties changed
        /// </summary>
        public void ModelService_PropertiesChanged(IEnumerable<ModelProperty> changedProperties)
        {
            Contract.Requires(changedProperties != null);

            foreach (var property in changedProperties)
            {
                if (ShouldBeAdd(property))
                {
                    Add(property.Value);
                }

            }
        }

        #endregion

        #region Private Functions

        #region Add

        private bool ShouldBeAdd(ModelProperty property)
        {
            return ((property.Name == ModelItemService.BodyPropertyName ||
                property.Name == ModelItemService.HandlerPropertyName ||
                property.Name == ModelItemService.ImplementationPropertyName) &&
                property.Value != null);
        }

        private void Add(ModelItem addedModel)
        {
            CompositeNode parentNode = NodeKeyManager.SearchNodeInDescendant(addedModel);
            if (HasKey(addedModel))
            {
                RefreshKey(addedModel, parentNode);
            }
            else
            {
                if (CheckAddModelIsReferenceSelf(addedModel, parentNode))
                {
                    CompositeService.DeleteModelItem(addedModel);
                }
                else if (CompositeService.IsCompositeModel(addedModel))
                {
                    AddNewNode(parentNode, addedModel);
                }
                else if (IsFlowchat(addedModel))
                {
                    AddFlowchatModel(addedModel);
                }
                else if (parentNode != null)
                {
                    NodeKeyManager.ApplyModelKey(addedModel, parentNode.Key);
                }
            }
        }

        private void AddFlowchatModel(ModelItem model)
        {
            ModelItem step = ModelItemService.GetModelFromFlowchat(model);
            if (step != null)
            {
                Add(step);
            }
        }

        private void RefreshKey(ModelItem model, CompositeNode parentNode)
        {
            if (NodeKeyManager.CheckIsModel(model) && parentNode != null)
            {
                NodeKeyManager.RefreshModelKey(model, parentNode.Key);
            }
            else if (NodeKeyManager.CheckIsNode(model))
            {
                NodeKey nodeKey = NodeKeyManager.GetNodeKey(model);
                if (!nodeKey.IsSuccessfullyApplied)
                {
                    NodeKeyManager.ApplyNodeSuccessfully(model);
                }
                else if (parentNode != null && nodeKey.Name == parentNode.AssemblyName.FullName)
                {
                    NodeKeyManager.RefreshModelKey(model, parentNode.Key);
                }
            }
        }

        private bool CheckAddModelIsReferenceSelf(ModelItem addedModel, CompositeNode parent)
        {
            if (CheckAddModelIsReferenceSelfCore(addedModel, parent))
            {
                string message = string.Format(MSG_UpdateReferenceInSelf, parent.AssemblyName.Name);
                AddInMessageBoxService.Show(message,
                                        Title_UpdateOtherActivities,
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                return true;
            }
            return false;
        }

        private bool CheckAddModelIsReferenceSelfCore(ModelItem addedModel, CompositeNode parent)
        {
            if (parent == null)
            {
                return false;
            }
            else
            {
                List<CompositeNode> checkParentList = new List<CompositeNode>();
                CompositeNode checkParent = parent;
                while (checkParent != null)
                {
                    checkParentList.Add(checkParent);
                    checkParent = checkParent.Parent;
                }
                return CheckNodeListHasEqualModel(checkParentList, addedModel);
            }
        }

        private bool CheckNodeListHasEqualModel(List<CompositeNode> nodes, ModelItem addedModel)
        {
            return nodes.Any(p => p.AssemblyName.Equal(addedModel));
        }

        private CompositeNode AddNewNode(CompositeNode parent, ModelItem modelItem)
        {
            CompositeNode addNode = CreateNewNode(parent, modelItem, null);

            if (addNode != null)
            {
                RecursiveUpdateActivities(addNode);
                UpdateTopModel(addNode);
            }
            ArgumentService.CreateArguments(modelItem);
            return addNode;
        }

        #endregion

        #region Update

        public void UpdateReference(ModelItem source)
        {
            if (HasKey(source))
            {
                ModelItem updateSource = FindUpdateSource(source);
                if (updateSource != null)
                {
                    string message = string.Format(MSG_UpdateOtherActivities, GetSourceTypeName(updateSource));
                    MessageBoxResult result = AddInMessageBoxService.Show(message,
                           Title_UpdateOtherActivities,
                           MessageBoxButton.YesNo,
                           MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        NodeKeyManager.UpdateReference(updateSource);
                    }
                }
            }
            else
            {
                MessageBoxResult result = AddInMessageBoxService.Show(MSG_NoUpdatedActivity,
                                                   Title_UpdateOtherActivities,
                                                   MessageBoxButton.OK,
                                                   MessageBoxImage.Question);
            }
        }

        private bool HasKey(ModelItem source)
        {
            if (NodeKeyManager.CheckIsNode(source) ||
                NodeKeyManager.CheckIsModel(source))
            {
                return true;
            }
            return false;
        }

        private ModelItem FindUpdateSource(ModelItem model)
        {
            ModelItem updateSource = null;
            if (NodeKeyManager.CheckIsNode(model))
            {
                updateSource = model;
            }
            else
            {
                var node = NodeKeyManager.SearchNodeInDescendant(model);
                node.IfNotNull(n => updateSource = n.OldItem);
            }
            return updateSource;
        }

        private string GetSourceTypeName(ModelItem model)
        {
            NodeKey key = NodeKeyManager.GetNodeKey(model);
            return new AssemblyName(key.Name).Name;
        }

        #endregion

        private CompositeNode CreateNewNode(CompositeNode parent, ModelItem item, Activity root)
        {
            CompositeNode node = null;
            AssemblyName assembly = null;
            if (item != null)
            {
                assembly = item.GetAssemblyName();

                root = CompositeService.GetRootActivity(item.GetActivity());
                if (root == null)
                {
                    return null;
                }
            }
            else
            {
                assembly = root.GetAssemblyName();
            }
            if (!AssemblyService.AssemblyIsBuiltIn(assembly))
            {
                node = new CompositeNode(GetDisplayName(item, assembly), assembly, parent, item);
                node.Activity = root;

                var subs = WorkflowInspectionServices.GetActivities(node.Activity);
                foreach (var sub in subs)
                {
                    var child = CreateNewNode(node, null, sub);
                    child.IfNotNull(c => node.Children.Add(c));
                }
            }
            return node;
        }

        private void RecursiveUpdateActivities(CompositeNode node)
        {
            if (!node.Children.Any())
            {
                node.NewItem = CompositeService.CreateModelItem(node.Activity);
                NodeKeyManager.ApplyKey(node.NewItem, node.Key);
            }
            else
            {
                foreach (var child in node.Children)
                {
                    RecursiveUpdateActivities(child);
                }
                UpdateActivity(node);
                NodeKeyManager.ApplyNodeKey(node.NewItem, node.Key);
            }
        }

        private string GetDisplayName(ModelItem model, AssemblyName assemblyName)
        {
            string name = assemblyName.Name;
            if (model != null)
            {
                NodeKeyManager manager = new NodeKeyManager(model.Root);
                List<NodeKey> keys = manager.RetrieveAllKeys();
                int count = keys.Where(v => v.Name == assemblyName.FullName).Count();
                if (count > 0)
                {
                    name = string.Format(ActivityNamePattern, assemblyName.Name, count);
                }
            }
            return name;
        }

        private void UpdateActivity(CompositeNode node)
        {
            if (node.NewItem == null)
            {
                node.NewItem = CompositeService.CreateModelItem(node.Activity);
            }

            var items = CompositeService.GetSubModelItems(node.NewItem);
            foreach (var child in node.Children)
            {
                ModelItem childModel = items.FirstOrDefault(m => child.AssemblyName.Equal(m));
                if (childModel != null)
                {
                    CompositeService.UpdateModelItem(childModel, child.Activity);
                }
            }
            if (node.NewItem != null)
            {
                node.Activity = node.NewItem.GetActivity();
            }

            foreach (var item in items)
            {
                NodeKeyManager.ApplyModelKey(item, node.Key);
            }
        }

        private bool IsFlowchat(ModelItem model)
        {
            return (model.ItemType.IsSubclassOf(typeof(FlowStep)) ||
                model.ItemType.IsSubclassOf(typeof(FlowNode)));
        }

        private void UpdateTopModel(CompositeNode node)
        {
            if (node.Parent == null)
            {
                node.NewItem = CompositeService.UpdateModelItem(node.OldItem, node.Activity);
            }
            else
            {
                UpdateParentModel(node.Parent, node);
            }
        }

        private void UpdateParentModel(CompositeNode parent, CompositeNode child)
        {
            var items = CompositeService.GetSubModelItems(parent.OldItem);
            ModelItem childModel = items.FirstOrDefault(m => child.AssemblyName.Equal(m));
            if (childModel != null)
            {
                CompositeService.UpdateModelItem(childModel, child.Activity);
            }
        }

        #endregion
    }
}
