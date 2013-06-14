// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompileMessages.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2012.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.AddIns.Models
{
    using System;

    /// <summary>
    /// Resource file with messages for compile exceptions
    /// </summary>
    public static class CompileMessages
    {
        public static string ProjectNameNull = "Project name cannot be null.";
        public static string ProjectVersionNull = "Project version cannot be null.";
        public static string ProjectXamlNull = "Project xaml cannot be null.";
        public static string OutputDirectoryNull = "Path of output directory cannot be null";
        public static string ProjectModelServiceNull = "Model service parameter cannot be null.";
        public static string ProjectVersionInvalid = "Specified project version is not valid.";
        public static string BuildCodeNull = "Build result cannot be null";
        public static string FileNameNull = "Output file name cannot be null";

        public static string ProjectFileNameTooLong =
            "The output assembly name is too long. The compile operation has been stopped.";
        public static string ProjectNull = "Project cannot be null.";

        public static string CreatingOutputDirectory = "Creating build directory";
        public static string ErrorCreatingOutputDirectory = "Unable to create output directory.";

        public static string ValidatingFilePath = "Validating file path for build operation";

        public static string ErrorInFilePath =
            "Unable to get a valid path for the output files of the compile operation";

        public static string CreatingFiles = "Creating project files";

        public static string ErrorCreatingFiles = "Unable to create files for build operation";

        public static string Compiling = "Compiling project";

        public static string ErrorCompiling =
            "There was an error compiling the workflow. Unfortunately we can't retrieve the detail programmatically. See Output directory for details.";

        public static string UnsignedAssemblies = "Project '{0}' relies upon unsigned assemblies. " + Environment.NewLine + "Unsigned assemblies are not supported in compiled workflows. Request a signed version of from the authors instead." + Environment.NewLine + "{1}";

        public static string MultipleVersionsUsed =
            "Compiling a workflow with activities from different versions of the same assembly is not supported.\n\nFor instance, this workflow contains references to both '{0}' from assembly '{1}' and '{2}' from assembly '{3}'.\n\nRewrite your workflow to use only one version of assembly '{4}' and re-compile.";

        public static string PreviousSelfVersion =
            "Compiling a workflow with activities from a previous compiled version of itself is not supported.\n\nRemove '{0}' from assembly '{1}' from the workflow and recompile.";

        
    }
}
