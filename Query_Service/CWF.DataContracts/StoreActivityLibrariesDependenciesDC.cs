//-----------------------------------------------------------------------
// <copyright file="StoreActivityLibrariesDependenciesDC.cs" company="Microsoft">
// Copyright
// StoreActivity Libraries Dependencies DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Request/Reply data contract for StoreactivityLibrariesDependencies
    /// </summary>
    [DataContract]
    public class StoreActivityLibrariesDependenciesDC : RequestReplyCommonHeader
    {
        private StoreDependenciesRootActiveLibrary storeDependenciesRootActiveLibrary;
        private List<StoreDependenciesDependentActiveLibrary> storeDependenciesDependentActiveLibraryList;

        /// <summary>
        /// <para>Root Activity Library</para>
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
        public StoreDependenciesRootActiveLibrary StoreDependenciesRootActiveLibrary
        {
            get { return storeDependenciesRootActiveLibrary; }
            set { storeDependenciesRootActiveLibrary = value; }
        }

        /// <summary>
        /// <para>Root Activity Library</para>
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
        public List<StoreDependenciesDependentActiveLibrary> StoreDependenciesDependentActiveLibraryList
        {
            get { return storeDependenciesDependentActiveLibraryList; }
            set { storeDependenciesDependentActiveLibraryList = value; }
        }
    }
}
