using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Activities;

namespace TestInput_Library1
{
    /// <summary>
    /// Used for tests.
    /// </summary>
    public static class PluggableWorkflowRunner
    {
        // By default, PluggableWorkflowRunner does nothing and returns "false" for unhandled.
        // This is so that if someone happens to import TestInput_Library1 into a real application
        // it won't actually do anything when they hit "Preview workflow".
        public static bool MarkAsHandled { get; set; }
        public static Action<Activity> Handler { get; set; }

        // Notice: MEF doesn't care what the activity is named, it only cares about the signature and Export tag
        [Export("RunWorkflow")]
        public static bool Exercise(Activity a)
        {
            if(Handler != null)
                Handler(a);
            return MarkAsHandled;
        }
    }
}
