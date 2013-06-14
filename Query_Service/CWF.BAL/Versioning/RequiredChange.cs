namespace CWF.BAL.Versioning
{
    /// <summary>
    /// Return values for individual fields, indicating the change needed before a save can happen.
    /// </summary>
    public enum RequiredChange
    {
        /// <summary>
        /// Everything is good with this field. No changes are required.
        /// </summary>
        NoActionRequired, 

        /// <summary>
        /// The field must be incremented in order for the requested operation to succeed.
        /// </summary>
        MustIncrement, 

        /// <summary>
        /// Must reset to the start value for this version section. For the Major field, this means "1". For all other fields, this means "0".
        /// </summary>
        MustReset,
        
        /// <summary>
        /// Cannot save with the current value. Change to any other value before reattempting the operation.
        /// Intended for string fields, and indicates the value exists already in the database.
        /// </summary>
        MustChange,
    }
}