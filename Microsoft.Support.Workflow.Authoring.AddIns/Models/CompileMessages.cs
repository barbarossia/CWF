// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompileMessages.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2012.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.AddIns.Models
{
    using System;
    using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

    /// <summary>
    /// Resource file with messages for compile exceptions
    /// </summary>
    public static class CompileMessages {
        public static string ProjectNameNull = TextResources.CompileProjectNameNullMsg;
        public static string ProjectVersionNull = TextResources.CompileProjectVersionNullMsg;
        public static string ProjectXamlNull = TextResources.CompileProjectXamlNullMsg;
        public static string OutputDirectoryNull = TextResources.CompileOutputDirectoryNullMsg;
        public static string ProjectModelServiceNull = TextResources.CompileProjectModelServiceNullMsg;
        public static string ProjectVersionInvalid = TextResources.CompileProjectVersionInvalidMsg;
        public static string BuildCodeNull = TextResources.CompileBuildCodeNullMsg;
        public static string FileNameNull = TextResources.CompileFileNameNullMsg;

        public static string ProjectFileNameTooLong = TextResources.CompileProjectFileNameTooLongMsg;
        public static string ProjectNull = TextResources.CompileProjectNullMsg;

        public static string CreatingOutputDirectory = TextResources.CompileCreatingOutputDirectoryMsg;
        public static string ErrorCreatingOutputDirectory = TextResources.CompileErrorCreatingOutputDirectoryMsg;

        public static string ValidatingFilePath = TextResources.CompileValidatingFilePathMsg;

        public static string ErrorInFilePath = TextResources.CompileErrorInFilePathMsg;

        public static string CreatingFiles = TextResources.CompileCreatingFilesMsg;

        public static string ErrorCreatingFiles = TextResources.CompileErrorCreatingFilesMsg;

        public static string Compiling = TextResources.CompileCompilingMsg;

        public static string ErrorCompiling = TextResources.CompileErrorCompilingMsg;

        public static string UnsignedAssemblies = TextResources.CompileUnsignedAssembliesMsgFormat;

        public static string MultipleVersionsUsed = TextResources.CompileMultipleVersionsUsedMsgFormat;

        public static string PreviousSelfVersion = TextResources.CompilePreviousSelfVersionMsgFormat;
    }
}
