//-----------------------------------------------------------------------
// <copyright file="StoreDependenciesRootActiveLibrary.cs" company="Microsoft">
// Copyright
// Store Dependencies Root ActiveLibrary
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Common Root record of Activity library
    /// </summary>
    [Serializable]
    public class StoreDependenciesRootActiveLibrary
    {
        private string activityLibraryName;
        private string activityLibraryVersionNumber;
        private int activityLibraryId;

        /// <summary>
        /// <para>Active Library Name</para>
        /// <para>CreateOrUpdate </para>
        /// <para>&#160;&#160;&#160;&#160;Request - Required </para>
        /// <para>&#160;&#160;&#160;&#160;Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - Required </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a</para>
        /// <para>&#160;&#160;&#160;&#160;Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string ActivityLibraryName
        {
            get { return activityLibraryName; }
            set { activityLibraryName = value; }
        }

        /// <summary>
        /// <para>Active Library Version number</para>
        /// <para>CreateOrUpdate </para>
        /// <para>&#160;&#160;&#160;&#160;Request - Required </para>
        /// <para>&#160;&#160;&#160;&#160;Reply - n/a </para>
        /// <para>Delete  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - Required </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a</para>
        /// <para>&#160;&#160;&#160;&#160;Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public string ActivityLibraryVersionNumber
        {
            get { return activityLibraryVersionNumber; }
            set { activityLibraryVersionNumber = value; }
        }

        /// <summary>
        /// <para>Active Library PK</para>
        /// <para>CreateOrUpdate </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a </para>
        /// <para>&#160;&#160;&#160;&#160;Reply - returned </para>
        /// <para>Delete  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - na/ </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a</para>
        /// <para>&#160;&#160;&#160;&#160;Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public int ActivityLibraryId
        {
            get { return activityLibraryId; }
            set { activityLibraryId = value; }
        }
    }
}
