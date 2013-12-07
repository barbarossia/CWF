using System.Runtime.Serialization;
using CWF.BAL.Versioning;

namespace CWF.WorkflowQueryService.Versioning
{
    /// <summary>
    /// This is the fault we will return to the calling application, describing why their workflow cannot 
    /// be saved, and the changes required.
    /// </summary>
    [DataContract]
    public class VersionFault
    {
        /// <summary>
        /// The rule that produced the fault. The operation that was attempted violated one or more sections
        /// of this rule (for instance, needed to change the name, or one of the verison fields were invalid).
        /// </summary>
        [DataMember]
        public Rule Rule { get; set; }

        /// <summary>
        /// This is a generated message built during the rule checking operation. It is intended to be shown to the user
        /// in order to facilitate correcting the problems found with the version.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        ///// <summary>
        ///// An array of fields indicating the changes required before the operation will pass. This is intended to allow the caller
        ///// to automatically make the required changes, if possible.
        ///// </summary>
        //[DataMember]
        //public VersionRequiredChanges[] RequiredChanges { get; set; }
    }
}