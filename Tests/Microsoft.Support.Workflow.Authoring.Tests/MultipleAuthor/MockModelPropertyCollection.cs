using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Presentation.Model;

namespace Microsoft.Support.Workflow.Authoring.Tests.MultipleAuthor
{
    public class MockModelPropertyCollection : ModelPropertyCollection
    {
        ModelItem parent;
        Dictionary<string, ModelProperty> collection = new Dictionary<string, ModelProperty>();
        public MockModelPropertyCollection(ModelItem parent)
        { 
            this.parent = parent; 
        } 

        public override IEnumerator<ModelProperty> GetEnumerator()
        {
            return null;
        }

        public ModelProperty Find(string name)
        {
            return collection[name];
        }

        protected override ModelProperty Find(System.Windows.DependencyProperty value, bool throwOnError)
        {
            return null;
        }

        protected override ModelProperty Find(string name, bool throwOnError)
        {
            return collection[name];
        }

        public void CreateModelPropery(string name, ModelProperty property)
        {
            collection.Add(name, property);
        }

        public ModelProperty this[string index]
        {
            get
            {
                return Find(index);
            }
        }

    }
}
