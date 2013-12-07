using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.Tests.MultipleAuthor
{
    public class MockModelItem : ModelItem
    {
        private string name;
        private object currentObject;
        private ModelPropertyCollection props;
        public MockModelItem(object obj)
        {
            SetValue(obj);
        }
        public override AttributeCollection Attributes 
        { 
            get 
            { 
                return null; 
            } 
        }
        public override ModelProperty Content
        {
            get
            {
                return null;
            }
        }
        public override Type ItemType
        {
            get
            {
                return null;
            }
        }
        public override string Name
        {
            get
            {
                return name; 
            }
            set
            {
                name = value;
            }
        }
        public override ModelItem Parent
        {
            get
            {
                return null;
            }
        }
        public override IEnumerable<ModelItem> Parents
        {
            get
            {
                return null;
            }
        }
        public override ModelPropertyCollection Properties
        {
            get
            {
                return props;
            }
        }
        public override ModelItem Root
        {
            get
            {
                return null;
            }
        }
        public override ModelProperty Source
        {
            get
            {
                return null;
            }
        }
        public override IEnumerable<ModelProperty> Sources
        {
            get
            {
                return null;
            }
        }
        public override DependencyObject View
        {
            get
            {
                return null;
            }
        }
        public override event PropertyChangedEventHandler PropertyChanged;
        public override ModelEditingScope BeginEdit()
        {
            return null;
        }
        public override ModelEditingScope BeginEdit(string description)
        {
            return null;
        }
        public override object GetCurrentValue()
        {
            return currentObject;
        }
        public override string ToString()
        {
            return string.Empty;
        }
        public void SetValue(object obj)
        {
            currentObject = obj;
        }
        public void SetProperty(ModelPropertyCollection prop)
        {
            props = prop;
        }
    }
}
