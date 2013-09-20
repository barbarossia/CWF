//-----------------------------------------------------------------------
// <copyright file="GetActivitiesByActivityLibraryNameAndVersionRequestDC.cs" company="Microsoft">
// Copyright
// Store Library And Activities DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Get Activities by activity library name and version request class
    /// </summary>
    [DataContract]
    public class GetActivitiesByActivityLibraryNameAndVersionRequestDC : WorkflowRequestHeader
    {
        private string name;
        private string versionNumber;

        /// <summary>
        /// <para>The unique name for this row </para>
        /// <para>CreateOrUpdate </para>
        /// <para>&#160;&#160;&#160;&#160;Request - Used in create - Required, Update - Optional </para>
        /// <para>&#160;&#160;&#160;&#160;Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - May or may not be the identifier for the operation </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - May or may not be the identifier for the operation </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// <para>The version of the library</para>
        /// <para>CreateOrUpdate </para>
        /// <para>&#160;&#160;&#160;&#160;Request - Create, Optional - Update, Optional </para>
        /// <para>&#160;&#160;&#160;&#160;Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - Not required, however, if present will return only that version</para>
        /// <para>&#160;&#160;&#160;&#160;Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string VersionNumber
        {
            get { return versionNumber; }
            set { versionNumber = value; }
        }
    }
}
