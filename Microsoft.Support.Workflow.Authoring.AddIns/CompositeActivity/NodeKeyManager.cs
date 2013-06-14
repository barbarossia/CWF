using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace Microsoft.Support.Workflow.Authoring.CompositeActivity
{
    /// <summary>
    /// Helper class to Nodekey
    /// </summary>
    public class NodeKeyManager
    {
        private const string Key = "Key";

        /// <summary>
        /// Constuction
        /// </summary>
        public NodeKeyManager(ModelItem root)
        {
            Contract.Requires(root != null);

            this.tree = CompositeService.GetSubModelItems(root).ToList();
        }

        /// <summary>
        /// Retrieve all nodekey in model tree
        /// </summary>
        public List<NodeKey> RetrieveAllKeys()
        {
            List<NodeKey> nodes = new List<NodeKey>();
            foreach (var model in tree)
            {
                var key = CompositeService.GetKeyObeject(model, Key);
                if (key != null && key is NodeKey)
                {
                    nodes.Add(key as NodeKey);
                }
            }
            return nodes;
        }

        /// <summary>
        /// Add model key object in model view state
        /// </summary>
        public static void ApplyModelKey(ModelItem model, NodeKey key)
        {
            Contract.Requires(model != null);
            Contract.Requires(key != null);

            StoreKey(model, CreateModelKey(key));
        }

        /// <summary>
        /// Add node key object in model view state
        /// </summary>
        public static void ApplyNodeKey(ModelItem model, NodeKey key)
        {
            Contract.Requires(model != null);
            Contract.Requires(key != null);

            StoreKey(model, key);
        }

        /// <summary>
        /// Add node and model key object in model view state
        /// </summary>
        public static void ApplyKey(ModelItem model, NodeKey key)
        {
            Contract.Requires(model != null);
            Contract.Requires(key != null);

            var items = CompositeService.GetSubModelItems(model);
            foreach (var item in items)
            {
                ApplyModelKey(item, key);
            }
            ApplyNodeKey(model, key);
        }

        /// <summary>
        /// Refresh model key object in model view state
        /// </summary>
        public static void RefreshModelKey(ModelItem model, NodeKey key)
        {
            Contract.Requires(model != null);
            Contract.Requires(key != null);

            CompositeService.ClearViewState(model, Key);
            var items = CompositeService.GetSubModelItems(model);
            foreach (var item in items)
            {
                ApplyModelKey(item, key);
            }
            ApplyModelKey(model, key);
        }

        /// <summary>
        /// Update node key property IsSuccessfullyApplied to true
        /// </summary>
        public static void ApplyNodeSuccessfully(ModelItem model)
        {
            Contract.Requires(model != null);

            var key = GetNodeKey(model);
            key.IsSuccessfullyApplied = true;
            StoreKey(model, key);
        }

        /// <summary>
        /// Create node key by assembly
        /// </summary>
        public static NodeKey CreateKey(AssemblyName assemblyName, CompositeNode parent)
        {
            Contract.Requires(assemblyName != null);

            return new NodeKey()
            {
                Key = Guid.NewGuid(),
                Name = assemblyName.FullName,
                Parent = parent != null ? parent.Key.Key : Guid.Empty,
                IsSuccessfullyApplied = false,
            };
        }

        /// <summary>
        /// Get node key in model view state
        /// </summary>
        public static NodeKey GetNodeKey(ModelItem model)
        {
            Contract.Requires(model != null);

            object key = CompositeService.GetKeyObeject(model, Key);
            if (key != null && key is NodeKey)
            {
                return (NodeKey)key;
            }
            return null;

        }

        /// <summary>
        /// Check key object in model view state is nodekey
        /// </summary>
        public static bool CheckIsNode(ModelItem model)
        {
            Contract.Requires(model != null);

            object key = CompositeService.GetKeyObeject(model, Key);
            if (key != null)
            {
                return key is NodeKey;
            }
            return false;
        }

        /// <summary>
        /// Search composite node in model or model descendant
        /// </summary>
        public static CompositeNode SearchNodeInDescendant(ModelItem model)
        {
            Contract.Requires(model != null);

            foreach (var parent in model.Parents)
            {
                var descendant = SearchModelInDescendant(parent);
                if (descendant != null)
                {
                    var key = GetNodeKey(descendant);
                    return new CompositeNode()
                    {
                        AssemblyName = new AssemblyName(key.Name),
                        Key = key,
                        OldItem = descendant,
                        Parent = SearchNodeInDescendant(descendant.Parent),
                    };
                }
            }
            return null;
        }

        /// <summary>
        /// Check key object in model view state is modelkey
        /// </summary>
        public static bool CheckIsModel(ModelItem model)
        {
            Contract.Requires(model != null);

            object key = CompositeService.GetKeyObeject(model, Key);
            if (key != null)
            {
                return key is ModelKey;
            }
            return false;
        }

        /// <summary>
        /// Update model has same reference
        /// </summary>
        public static void UpdateReference(ModelItem source)
        {
            Contract.Requires(source != null);

            var sourceKey = GetNodeKey(source);
            var assemblyName = sourceKey.Name;

            var needToUpdate = new List<ModelItem>();
            var root = source.Root;
            var tree = CompositeService.GetSubModelItems(root);
            foreach (var model in tree)
            {
                var key = GetNodeKey(model);
                if (key != null && key.Name == assemblyName && model != source)
                {
                    needToUpdate.Add(model);
                }
            }

            foreach (var target in needToUpdate)
            {
                var copyActivity = source.GetActivity().Clone();
                CompositeService.UpdateModelItem(target, copyActivity);
            }
        }

        private static ModelKey CreateModelKey(NodeKey key)
        {
            return new ModelKey()
            {
                Node = key.Key,
                Key = Guid.NewGuid(),
            };
        }

        private static void StoreKey(ModelItem model, object keyValue)
        {
            CompositeService.StoreViewState(model, Key, keyValue);
        }

        private static ModelItem SearchModelInDescendant(ModelItem model)
        {
            if (model != null)
            {
                var acitivity = CompositeService.SearchStoreKeyModel(model);
                if (acitivity != null)
                {
                    var key = GetNodeKey(acitivity);
                    if (key != null)
                    {
                        return acitivity;
                    }
                    else
                    {
                        return SearchModelInDescendant(acitivity.Parent);
                    }
                }
            }
            return null;
        }

        private List<ModelItem> tree;
    }
}
