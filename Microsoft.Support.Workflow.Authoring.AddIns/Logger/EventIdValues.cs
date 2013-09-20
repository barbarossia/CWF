using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Logger
{
    /// <summary>
    /// Range of Event Id
    /// The Event ID range: 3000 ~ 3999
    /// </summary>
    public static class EventIdValues
    {
        //// Unhandled Exception 
        public const int UNHANDELED_EXCEPTION = 3000;

        //// Handled Exception  
        public const int HANDELED_EXCEPTION = 3999;
    }
}
