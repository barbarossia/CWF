using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    /// <summary>
    /// Defines the data column names returned from the database when invoking stored procedures.  
    /// These names may or may not represent an exact match with underlying table columns since 
    /// during a selection there is a possibility of aliasing the column names.  Typically these 
    /// constants are used to read data from a data reader.
    /// </summary>
    public static class DataColumnNames
    {
        public const string ActivityLibraryId = "ActivityLibraryId";
        public const string ActivityCategoryName = "ActivityCategoryName";
        public const string ActivityLibraryName = "ActivityLibraryName";
        public const string AuthGroupId = "AuthGroupId";
        public const string AuthGroupName = "AuthGroupName";
        public const string Description = "Description";
        public const string DependentActivityLibraryId = "DependentActivityLibraryId";
        public const string DeveloperNotes = "DeveloperNotes";
        public const string Guid = "Guid";
        public const string HandleVariableId = "HandleVariableId";
        public const string Id = "Id";
        public const string IsCodeBeside = "IsCodeBeside";
        public const string IsService = "IsService";
        public const string Locked = "Locked";
        public const string LockedBy = "LockedBy";
        public const string MetaTags = "MetaTags";
        public const string Name = "Name";
        public const string Namespace = "Namespace";
        public const string PageViewVariableId = "PageViewVariableId";
        public const string PublishingWorkflowName = "PublishingWorkflowName";
        public const string PublishingWorkflowVersion = "PublishingWorkflowVersion";
        public const string PublishingWorkflowId = "PublishingWorkflowId";
        public const string SelectionWorkflowId = "SelectionWorkflowId";
        public const string ToolBoxtab = "ToolBoxtab";
        public const string Version = "Version";
        public const string WorkflowTemplateName = "WorkflowTemplateName";
        public const string WorkflowTemplateVersion = "WorkflowTemplateVersion";
        public const string WorkflowTemplateId = "WorkflowTemplateId";
        public const string WorkflowTypeName = "WorkFlowTypeName";
        public const string WorkflowsCount = "WorkflowsCount";
        public const string Xaml = "XAML";
    }
}
