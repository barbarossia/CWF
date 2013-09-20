using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Data {
    [Flags]
    public enum Permission : long {
        None = 0,
        OpenWorkflow = 0x0000000000000001,
        SaveWorkflow = 0x0000000000000002,
        OverrideLock = 0x0000000000000004,
        DeleteWorkflow = 0x0000000000000008,
        CopyWorkflow = 0x0000000000000010,
        MoveWorkflow = 0x0000000000000020,
        ChangeWorkflowAuthor = 0x0000000000000040,
        CreateTask = 0x0000000000000080,
        ManageWorkflowType = 0x0000000000000100,
        ManageEnvAdmin = 0x0000000000000200,
        ManageRoles = 0x0000000000000400,
        ViewMarketplace = 0x0000000000000800,
        UploadAssemblyToMarketplace = 0x0000000000001000,
        UploadProjectToMarketplace = 0x0000000000002000,
        CompileWorkflow = 0x0000000000004000,
        PublishWorkflow = 0x0000000000008000,
    }
}
