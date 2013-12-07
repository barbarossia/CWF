using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Microsoft.Support.Workflow
{
    [Serializable]
    [DataContract]
    public enum VersionFlag
    {
        /// <summary>
        /// Request only the item(s) with the highest version number
        /// </summary>
        [EnumMember]
        LastestVersion,
        /// <summary>
        /// Request the item(s) with the highest
        /// build & revision number for the
        /// Version.Major + Version.Minor value(s)
        /// </summary>
        [EnumMember]
        LatestBuild,
        /// <summary>
        /// If the service returns a list, get all of 
        /// versions of the object.  If the service
        /// only returns one workflow, get the exact
        /// match on the version number provided.
        /// </summary>
        [EnumMember]
        AllBuilds,
        /// <summary>
        /// Get only the object that matches both the 
        /// full version number.
        /// </summary>
        [EnumMember]
        SpecificVersionAndBuild

    }
}
