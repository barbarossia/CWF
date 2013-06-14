using System.Runtime.Serialization;

namespace CWF.BAL.Versioning
{
    /// <summary>
    /// Describes a rule for versioning. Describes the state that a workflow is in, and the changes that need to be made (if any) before save,
    /// such as change the name or increment a version section.
    /// </summary>
    [DataContract]
    public sealed class Rule
    {
        private Rule() { }

        /// <summary>
        /// Setup for the VersionRule object. Since we consider this object to be an immutable type, we restrict the 
        /// constructor to this methof and require these setup values to be passed.
        /// </summary>
        /// <param name="newActionType">See property "ActionType".</param>
        /// <param name="newIsPublic">See property "IsPublic".</param>
        /// <param name="newIsRetired">See property "IsRetired".</param>
        /// <param name="newNameRequiredAction">See property "NameRequiredAction".</param>
        /// <param name="newMajorRequiredAction">See property "MajorRequiredAction".</param>
        /// <param name="newMinorRequiredAction">See property "MinorRequiredAction".</param>
        /// <param name="newBuildRequiredAction">See property "BuildRequiredAction".</param>
        /// <param name="newRevisionRequiredAction">See property "RevisionRequiredAction".</param>
        public Rule(RequestedOperation newActionType,
                    bool newIsPublic,
                    bool newIsRetired,
                    RequiredChange newNameRequiredAction,
                    RequiredChange newMajorRequiredAction,
                    RequiredChange newMinorRequiredAction,
                    RequiredChange newBuildRequiredAction,
                    RequiredChange newRevisionRequiredAction)
        {
            ActionType = newActionType;
            IsPublic = newIsPublic;
            IsRetired = newIsRetired;
            NameRequiredChange = newNameRequiredAction;
            MajorRequiredChange = newMajorRequiredAction;
            MinorRequiredChange = newMinorRequiredAction;
            BuildRequiredChange = newBuildRequiredAction;
            RevisionRequiredChange = newRevisionRequiredAction;
        }

        /// <summary>
        /// Input condition -- the action that is being attempted (Save, Update, etc.).
        /// </summary>
        [DataMember]
        public RequestedOperation ActionType { get; private set; }

        /// <summary>
        /// Input condition -- Is the workflow being saved to a Public state?
        /// </summary>
        [DataMember]
        public bool IsPublic { get; private set; }

        /// <summary>
        /// Input condition -- Is the workflow being saved to a Retired state?
        /// </summary>
        [DataMember]
        public bool IsRetired { get; private set; }

        /// <summary>
        /// Output -- what do we need to do to make the Name field valid?
        /// </summary>
        [DataMember]
        public RequiredChange NameRequiredChange { get; private set; }

        /// <summary>
        /// Output -- what do we need to do to make the Major section of the Version field valid?
        /// </summary>
        [DataMember]
        public RequiredChange MajorRequiredChange { get; private set; }

        /// <summary>
        /// Output -- what do we need to do to make the Minor section of the Version field valid?
        /// </summary>
        [DataMember]
        public RequiredChange MinorRequiredChange { get; private set; }

        /// <summary>
        /// Output -- what do we need to do to make the Build section of the Version field valid?
        /// </summary>
        [DataMember]
        public RequiredChange BuildRequiredChange { get; private set; }

        /// <summary>
        /// Output -- what do we need to do to make the Revision section of the Version field valid?
        /// </summary>
        [DataMember]
        public RequiredChange RevisionRequiredChange { get; private set; }
    }
}