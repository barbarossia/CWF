using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Models
{
    public class ActivityFocuceEventArgs : EventArgs
    {
        public WorkflowOutlineNode Node { get; private set; }
        public ActivityFocuceEventArgs(WorkflowOutlineNode node)
        {
            this.Node = node;
        }
    }

    public delegate void ActivityFocuceEventHandler(object sender, ActivityFocuceEventArgs e);
}
