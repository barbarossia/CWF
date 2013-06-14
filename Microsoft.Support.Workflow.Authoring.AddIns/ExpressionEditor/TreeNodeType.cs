using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.ExpressionEditor
{
    /// <summary>
    /// Types of TreeNode
    /// </summary>
    public enum TreeNodeType
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
}
