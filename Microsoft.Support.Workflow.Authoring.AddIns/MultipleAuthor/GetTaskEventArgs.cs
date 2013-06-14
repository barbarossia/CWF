using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor
{
    [Serializable]
    public class GetTaskEventArgs : EventArgs
    {
        public string ActivityLibraryName { get; set; }
        public string Version { get; set; }
        public IDictionary<string, string> ActivityLibraries { get; set; }
        public bool IsCollection { get; set; }

        public GetTaskEventArgs(string activityLibraryName, string version)
        {
            this.ActivityLibraryName = activityLibraryName;
            this.Version = version;
            this.IsCollection = false;
        }

        public GetTaskEventArgs(IDictionary<string, string> activityLibraries)
        {
            this.ActivityLibraries = activityLibraries;
            this.IsCollection = true;
        }
    }

    [Serializable]
    [ComVisible(true)]
    public delegate void GetTaskLastVersionEventHandler(object sender, GetTaskEventArgs e);
}
