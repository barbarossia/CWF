// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportMessages.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

namespace Microsoft.Support.Workflow.Authoring.Common.Messages
{
    /// <summary>
    /// Messages for the import functionality.
    /// </summary>
    internal static class ImportMessages
    {
        /// <summary>
        /// The assembly parameter is null
        /// </summary>
        public readonly static string AssemblyNull = TextResources.AssemblyNullMsg;
        /// <summary>
        /// The name of the assembly is null.
        /// </summary>
        public readonly static string AssemblyNameNull = TextResources.AssemblyNameNullMsg;
        /// <summary>
        /// The location (file path) of the parameter is null.
        /// </summary>
        public readonly static string AssemblyLocationNull = TextResources.AssemblyLocationNullMsg;
        /// <summary>
        /// The location (file path) of the parameter is null.
        /// </summary>
        public readonly static string FileNotFound = TextResources.FileNotFoundMsg;
        /// <summary>
        /// The assembly is not a compatible .NET assembly
        /// </summary>
        public readonly static string NotADotNetAssembly = TextResources.NonDotNetAssemblyMsgFormat;
        /// <summary>
        /// The assembly could not be analyzed
        /// </summary>
        public readonly static string AssemblyNotAnalyzed = TextResources.AssemblyCannotAnalyzeMsg;
        /// <summary>
        /// The asembly is not signed, and for security purposes is not supported.
        /// </summary>
        public static readonly string AssemblyUnsigned = TextResources.AssemblyUnsignedMsg;
        /// <summary>
        /// No assembly was specified in the list of assemblies to import.
        /// </summary>
        public static readonly string AssembliesToImportNull = TextResources.AssemblyListNullMsg;

        /// <summary>
        /// The assembly is already in the local cache.
        /// </summary>
        public static readonly string AssemblyAlreadyImported = TextResources.AssemblyAlreadyImportedMsg;

        /// <summary>
        /// The assembly has already been inspected and exists in the list of assemblies to import
        /// </summary>
        public static readonly string AssemblyAlreadyInListToImport =
            TextResources.AssemblyInImportListMsg;

        /// <summary>
        /// The category is null
        /// </summary>
        public static readonly string CategoryNameNull =
           TextResources.CategoryNameNullMsg;
        /// <summary>
        /// The category name is too long 
        /// </summary>
        public static readonly string CategoryNameOutOfRange =
            TextResources.CategoryNameOutOfRangeMsg;

        /// <summary>
        /// The category name is too long 
        /// </summary>
        public static readonly string CategoryWithInvalidName =
            TextResources.InvalidCategoryNameMsg;

        

    }
}
