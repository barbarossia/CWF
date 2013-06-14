using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Presentation.Model;
using System.Activities;
using System.Activities.Statements;
using System.Diagnostics.Contracts;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Utilities
{
    public static class ModelItemService
    {
        public const string ActionPropertyName = "Action";
        public const string NamePropertyName = "Name";
        public const string HandlerPropertyName = "Handler";
        public const string DisplayNamePropertyName = "DisplayName";
        public const string NodesPropertyName = "Nodes";
        public const string BodyPropertyName = "Body";
        public const string ImplementationPropertyName = "Implementation";
        public const string IdPropertyName = "Id";

        public static ModelItem GetModelFromFlowchat(ModelItem model)
        {
            if (model.Properties[ActionPropertyName] != null)
            {
                return model.Properties[ActionPropertyName].Value;
            }

            return null;
        }

        public static IEnumerable<ModelItem> Find(ModelItem startingItem, Predicate<Type> matcher)
        {
            Contract.Requires(startingItem != null);
            Contract.Requires(matcher != null);

            return ModelItemService.FindCore(startingItem, matcher);
        }

        private static IEnumerable<ModelItem> FindCore(ModelItem startingItem, Predicate<Type> matcher)
        {
            List<ModelItem> foundItems = new List<ModelItem>();
            Queue<ModelItem> modelItems = new Queue<ModelItem>();
            modelItems.Enqueue(startingItem);
            HashSet<ModelItem> alreadyVisited = new HashSet<ModelItem>();

            while (modelItems.Count > 0)
            {
                ModelItem currentModelItem = modelItems.Dequeue();
                if (currentModelItem == null)
                {
                    continue;
                }
                if (matcher(currentModelItem.ItemType))
                {
                    foundItems.Add(currentModelItem);
                }

                ModelItemCollection collection = currentModelItem as ModelItemCollection;
                if (collection != null)
                {
                    foreach (ModelItem modelItem in collection)
                    {
                        if (modelItem != null && !alreadyVisited.Contains(modelItem))
                        {
                            alreadyVisited.Add(modelItem);
                            modelItems.Enqueue(modelItem);
                        }
                    }
                }
                else
                {
                    ModelItemDictionary dictionary = currentModelItem as ModelItemDictionary;
                    if (dictionary != null)
                    {
                        foreach (var kvp in dictionary)
                        {
                            ModelItem miKey = kvp.Key;
                            if (miKey != null && !alreadyVisited.Contains(miKey))
                            {
                                alreadyVisited.Add(miKey);
                                modelItems.Enqueue(miKey);
                            }

                            ModelItem miValue = kvp.Value;
                            if (miValue != null && !alreadyVisited.Contains(miValue))
                            {
                                alreadyVisited.Add(miValue);
                                modelItems.Enqueue(miValue);
                            }
                        }
                    }
                }
                if (currentModelItem.ItemType.IsSubclassOf(typeof(Activity)) ||
                    currentModelItem.ItemType.IsAssignableFrom(typeof(ActivityBuilder)) ||
                    typeof(Flowchart).IsAssignableFrom(currentModelItem.ItemType) ||
                    typeof(FlowNode).IsAssignableFrom(currentModelItem.ItemType) ||
                    typeof(ActivityDelegate).IsAssignableFrom(currentModelItem.ItemType))
                {
                    ModelPropertyCollection modelProperties = currentModelItem.Properties;
                    foreach (ModelProperty property in modelProperties)
                    {
                        if (property.PropertyType.IsAssignableFrom(typeof(Type)) || property.PropertyType.IsValueType)
                        {
                            continue;
                        }
                        else
                        {
                            if (property.Value != null && !alreadyVisited.Contains(property.Value))
                            {
                                alreadyVisited.Add(property.Value);
                                modelItems.Enqueue(property.Value);
                            }
                        }
                    }
                }
            }

            return foundItems;
        }
    }
}
