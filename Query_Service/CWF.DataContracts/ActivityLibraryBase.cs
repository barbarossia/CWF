//-----------------------------------------------------------------------
// <copyright file="ActivityLibraryBase.cs" company="Microsoft">
// Copyright
// ActivityLibrary Base
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// ActivityLibraries base columns.
    /// </summary>
    [DataContract]
    public class ActivityLibraryBase
    {
        /// <summary>
        /// PK  for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Guid  for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public Guid Guid { get; set; }

        /// <summary>
        /// Name for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// (PK) Auth group for the the ActivityLibrary row (returned only)
        /// </summary>
        [DataMember]
        public int AuthGroupId { get; set; }

        /// <summary>
        /// Auth Group Name for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string AuthGroupName { get; set; }

        /// <summary>
        /// Activity category for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public Guid Category { get; set; }

        /// <summary>
        /// Category Name for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string CategoryName { get; set; }

        /// <summary>
        /// PK Category ID  for the the ActivityLibrary row (returned only)
        /// </summary>
        [DataMember]
        public int CategoryId { get; set; }

        /// <summary>
        /// Dll code for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public byte[] Executable { get; set; }

        /// <summary>
        /// Has Activities boolean for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public bool HasActivities { get; set; }

        /// <summary>
        /// Description for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Imported by alias for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string ImportedBy { get; set; }

        /// <summary>
        /// Version number for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string VersionNumber { get; set; }

        /// <summary>
        /// Status name  for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public int Status { get; set; }

        /// <summary>
        /// Meta tags  for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string MetaTags { get; set; }

        /// <summary>
        /// Inserted by aliasMeta tags  for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string InsertedByUserAlias { get; set; }

        /// <summary>
        /// Updated by user aliasMeta tags  for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string UpdatedByUserAlias { get; set; }

        /// <summary>
        /// Gets or sets the friendly name of the activity library
        /// </summary>
        [DataMember]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the release notes of the activity library
        /// </summary>
        [DataMember]
        public string ReleaseNotes { get; set; }
    }
}
