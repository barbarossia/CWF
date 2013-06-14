//-----------------------------------------------------------------------
// <copyright file="ActivityLibrariesCheckExistReply.cs" company="Microsoft">
// Copyright
// ActivityLibrary DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Response contract for the check assemblies in data store function
    /// </summary>
    [DataContract]
    public class GetMissingActivityLibrariesReply : StatusReplyDC
    {
        /// <summary>
        /// List of activities that are missing in the data store
        /// </summary>
        [DataMember]
        public List<ActivityLibraryDC> MissingActivityLibraries { get; set; }
    }
}
