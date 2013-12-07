using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Presentation.Model;
using System.ComponentModel;

namespace Microsoft.Support.Workflow.Authoring.Tests.MultipleAuthor
{
    public class MockModelProperty : ModelProperty
    {
        public override Type AttachedOwnerType { get { return null; } }
        public override AttributeCollection Attributes { get { return null; } }
        public override ModelItemCollection Collection { get { return null; } }
        public override object ComputedValue { get; set; }
        public override TypeConverter Converter { get { return null; } }
        public override object DefaultValue { get { return null; } }
        public override ModelItemDictionary Dictionary { get { return null; } }
        public override bool IsAttached { get { return true; } }
        public override bool IsBrowsable { get { return true; } }
        public override bool IsCollection { get { return true; } }
        public override bool IsDictionary { get { return true; } }
        public override bool IsReadOnly { get { return true; } }
        public override bool IsSet { get { return true; } }
        public override string Name { get { return "Test"; } }
        public override ModelItem Parent { get { return null; } }
        public override Type PropertyType { get { return null; } }
        public override ModelItem Value { get { return null; } }

        public override void ClearValue()
        {
        }
        public override bool Equals(object obj)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return 0;
        }
        public override ModelItem SetValue(object value)
        {
            return null;
        }
    }
}
