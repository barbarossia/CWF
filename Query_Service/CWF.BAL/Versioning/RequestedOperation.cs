namespace CWF.BAL.Versioning
{
    /// <summary>
    /// The action requested by the user - save, update, etc.
    /// </summary>
    public enum RequestedOperation
    {
        /// <summary>
        /// The user requested an update to an existing record.
        /// </summary>
        Update,

        /// <summary>
        /// The user is inserting a new record, with no previous versions in the database.
        /// </summary>
        AddNew,

        /// <summary>
        /// The user is deleting a record.
        /// </summary>
        Delete,

        /// <summary>
        /// The user is compiling a workflow.
        /// </summary>
        Compile,
    }
}