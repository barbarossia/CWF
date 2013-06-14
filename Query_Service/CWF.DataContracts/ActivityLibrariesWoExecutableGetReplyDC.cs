//-----------------------------------------------------------------------
// <copyright file="ActivityLibrariesWoExecutableGetReplyDC.cs" company="Microsoft">
// Copyright
// Activity Libraries W oExecutable Get Reply DC
// </copyright>
//-----------------------------------------------------------------------
namespace CWF.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Reply data contract for ActivityLibrariesWoExecutableGet.
    /// </summary>
    [DataContract]
    public class ActivityLibrariesWoExecutableGetReplyDC : ReplyHeader
    {
        /// <summary>
        /// List of base ActivityLibraries columns
        /// </summary>
        [DataMember]
        public IList<ActivityLibrariesWoExecutableGetBase> ActivityLibrariesWoExecutable { get; set; }
    }

    [DataContract]
    public class ActivityLibrariesWoExecutableGetBase
    {
        /// <summary>
        /// PK of the ActivityLibrary row (only for update)
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Guid for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public Guid Guid { get; set; }

        /// <summary>
        /// Name for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// AuthGroup Name for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string Authgroupname { get; set; }

        /// <summary>
        /// PK for the AuthGroup (returned only)
        /// </summary>
        [DataMember]
        public int Authgroupid { get; set; }

        /// <summary>
        /// Activity Category for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public Guid Category { get; set; }

        /// <summary>
        /// Has activities boolean for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public bool HasActivities { get; set; }

        /// <summary>
        /// Description for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// imported by alias for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string ImportedBy { get; set; }

        /// <summary>
        /// Version Number for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string VersionNumber { get; set; }

        /// <summary>
        /// Status  for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Meta tags for the the ActivityLibrary row
        /// </summary>
        [DataMember]
        public string MetaTags { get; set; }

        /// <summary>
        /// Friendly name assigned to the assembly
        /// </summary>
        [DataMember]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Release notes of the assembly
        /// </summary>
        [DataMember]
        public string ReleaseNotes { get; set; }
    }
}
