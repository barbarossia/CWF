//-------------------------------------------------------------------------------------------
// <copyright file="ActivityLibraryDependenciesListHeadCreateOrUpdateRequestDC.cs" company="Microsoft">
// Copyright
// StoreActivity Libraries Dependencies DC
// </copyright>
//-------------------------------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Used to Creates or update an mtblActivityLibraryDependencies entry
    /// </summary>
    [DataContract]
    public class ActivityLibraryDependenciesListHeadCreateOrUpdateRequestDC : RequestHeader
    {
        private string name;
        private string version;
        //private string insertedByUserAlias;
        //private string updatedByUserAlias;

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

        ///// <summary>
        ///// Inserting alias name
        ///// </summary>
        //[DataMember]
        //public string InsertedByUserAlias
        //{
        //    get { return insertedByUserAlias; }
        //    set { insertedByUserAlias = value; }
        //}

        ///// <summary>
        ///// Updating alias name
        ///// </summary>
        //[DataMember]
        //public string UpdatedByUserAlias
        //{
        //    get { return updatedByUserAlias; }
        //    set { updatedByUserAlias = value; }
        //}
    }
}
