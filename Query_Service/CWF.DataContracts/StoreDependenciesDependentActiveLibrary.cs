//-----------------------------------------------------------------------
// <copyright file="StoreDependenciesDependentActiveLibrary.cs" company="Microsoft">
// Copyright
// StoreDependencies Dependent ActiveLibrary DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// StoreDependentcies parent library and dependent library pair
    /// </summary>
    [Serializable]
    public class StoreDependenciesDependentActiveLibrary
    {
        private string activityLibraryDependentName;
        private string activityLibraryDependentVersionNumber;
        private int activityLibraryParentId;
        private int activityLibraryDependentId;

        /// <summary>
        /// <para>Depent Library Name</para>
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
        public string ActivityLibraryDependentName
        {
            get { return activityLibraryDependentName; }
            set { activityLibraryDependentName = value; }
        }

        /// <summary>
        /// <para>Depent Library version number</para>
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
        public string ActivityLibraryDependentVersionNumber
        {
            get { return activityLibraryDependentVersionNumber; }
            set { activityLibraryDependentVersionNumber = value; }
        }

        /// <summary>
        /// <para>Depent Library PK</para>
        /// <para>CreateOrUpdate </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a </para>
        /// <para>&#160;&#160;&#160;&#160;Reply - Returned </para>
        /// <para>Delete  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a</para>
        /// <para>&#160;&#160;&#160;&#160;Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public int ActivityLibraryParentId
        {
            get { return activityLibraryParentId; }
            set { activityLibraryParentId = value; }
        }

        /// <summary>
        /// <para>Depent Library PK</para>
        /// <para>CreateOrUpdate </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a </para>
        /// <para>&#160;&#160;&#160;&#160;Reply - Returned </para>
        /// <para>Delete  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a </para>
        /// <para>&#160;&#160;&#160;&#160;Reply- n/a </para>
        /// <para>Get  </para>
        /// <para>&#160;&#160;&#160;&#160;Request - n/a</para>
        /// <para>&#160;&#160;&#160;&#160;Reply- will be included as a return column </para>
        /// </summary>
        [DataMember]
        public int ActivityLibraryDependentId
        {
            get { return activityLibraryDependentId; }
            set { activityLibraryDependentId = value; }
        }
    }
}
