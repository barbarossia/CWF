namespace CWF.BAL.Versioning
{
    /// <summary>
    /// Values for specifying sections of a Version object, for purposes of workflow authoring/saving/etc.
    /// </summary>
    public enum Section
    {
        /// <summary>
        /// The Major field of a Version object
        /// </summary>
        Major,

        /// <summary>
        /// The Minor field of a Version object
        /// </summary>
        Minor,

        /// <summary>
        /// The Build field of a Version object
        /// </summary>
        Build,

        /// <summary>
        /// The Revision field of a Version object
        /// </summary>
        Revision,
    }
}