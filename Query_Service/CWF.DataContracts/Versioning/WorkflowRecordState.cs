namespace CWF.DataContracts.Versioning
{
    /// <summary>
    /// Defines the publishing state of a workflow to be saved.
    /// </summary>
    public enum WorkflowRecordState
    {
        /// <summary>
        /// The workflow is to be saved as Private.
        /// </summary>
        Private = 1,  // Values are explicitly set so that comparisons are clearer.

        /// <summary>
        /// The workflow is to be saved as Public.
        /// </summary>
        Public = 2,

        /// <summary>
        /// The workflow is to be saved as Retired.
        /// </summary>
        Retired = 3
    }
}