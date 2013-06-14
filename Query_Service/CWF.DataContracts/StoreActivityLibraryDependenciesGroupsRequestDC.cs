//-------------------------------------------------------------------------------------------
// <copyright file="StoreActivityLibraryDependenciesGroupsRequestDC.cs" company="Microsoft">
// Copyright
// StoreActivity Libraries Dependencies DC
// </copyright>
//-------------------------------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Request DC for creating a dependencies group in one call. Structure is as follows:
    /// 
    /// CWF.DataContracts.StoreActivityLibraryDependenciesGroupsRequestDC request
    ///             DependencyName
    ///             DependencyVersion
    ///             List<CWF.DataContracts.StoreActivityLibraryDependenciesGroupsRequestDC> dependentLibraryList 
    ///                 CWF.DataContracts.StoreActivityLibraryDependenciesGroupsRequestDC
    ///                     DependentName-1
    ///                     DependentVersion-1
    ///                 CWF.DataContracts.StoreActivityLibraryDependenciesGroupsRequestDC
    ///                      DependentName-2
    ///                     DependentVersion-2

    /// </summary>
    [DataContract]
    public class StoreActivityLibraryDependenciesGroupsRequestDC : RequestHeader
    {
        private string name;
        private string version;
        private string status;
        private List<StoreActivityLibraryDependenciesGroupsRequestDC> list;

        /// <summary>
        /// Library name
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Library version number
        /// </summary>
        [DataMember]
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// Status for internal use - relates to "Public", "Private", "Retired"
        /// </summary>
        [DataMember]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// list which contains all dependencies as a list for this root
        /// </summary>
        [DataMember]
        public List<StoreActivityLibraryDependenciesGroupsRequestDC> List
        {
            get { return list; }
            set { list = value; }
        }
    }
}
