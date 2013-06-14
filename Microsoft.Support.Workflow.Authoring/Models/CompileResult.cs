namespace Microsoft.Support.Workflow.Authoring.Models
{
    using System;
    using Build.Execution;

    /// <summary>
    /// General result of a compile operation.
    /// </summary>
    public class CompileResult
    {
        /// <summary>
        /// Code that indicates the success or failure of the compile operation.
        /// </summary>
        public BuildResultCode BuildResultCode { get; private set; }

        /// <summary>
        /// Full file path of the output assembly.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Exception produced during the compile operation
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="code">Result code of the operation</param>
        /// <param name="fileName">Full file path of the produced output assembly</param>
        /// <param name="compileException">Exception generated in the operation.</param>
        public CompileResult(BuildResultCode code, string fileName, Exception compileException)
        {
            BuildResultCode = code;
            FileName = fileName;
            Exception = compileException;
        }
    }
}
