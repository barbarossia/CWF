using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.Tests
{
    /// <summary>
    /// Expected WorkFlow Properties
    /// </summary>
    public class WorkFlowProperties
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Version { get; set; }
        public string ActivityType { get; set; }
        public string BusyCaption { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
        public string DeveloperNote { get; set; }
        public string LocalFileFullName { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public string Tags { get; set; }
        public Env Environment { get; set; }
    }
}
