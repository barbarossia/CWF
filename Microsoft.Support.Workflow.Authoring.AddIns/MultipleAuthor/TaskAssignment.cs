using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using CWF.DataContracts;

namespace Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor
{
    [Serializable]
    public class TaskAssignment
    {
        public Guid TaskId { get; set; }
        public string AssignTo { get; set; }
        public string Xaml { get; set; }
        public TaskActivityStatus TaskStatus { get; set; }

        public string Version
        {
            get { return "1.0.0.0"; }
        }
        public string Name
        {
            get { return TaskId.ToString("N"); }
        }
        public string GetFriendlyName(string parentName)
        {
            return string.Format("{0}_{1}", parentName, Name.Substring(0, 8));
        }

        public TaskAssignment()
        {
            TaskStatus = TaskActivityStatus.New;
        }
    }
}
