using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Activity
{
    public interface ICwfsActivity 
    {
        bool IsPublished { get; set; }
        bool IsUserInteractionActivity { get; set; }

    }
}
