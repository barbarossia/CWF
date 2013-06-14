//-----------------------------------------------------------------------
// <copyright file="GetLibraryAndActivitiesDC.cs" company="Microsoft">
// Copyright
// Get Library And Activities DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Request/Reply data contract for GetLibraryAndActivities
    /// </summary>
    [DataContract]
    public class GetLibraryAndActivitiesDC : ReplyHeader
    {
        private ActivityLibraryDC activityLibrary = null;
        private List<StoreActivitiesDC> storeActivitiesList = null;

        /// <summary>
        /// ActivityLibrary object
        /// </summary>
        [DataMember]
        public ActivityLibraryDC ActivityLibrary
        {
            get { return activityLibrary; }
            set { activityLibrary = value; }
        }

        /// <summary>
        /// List of StoreActivities records
        /// </summary>
        [DataMember]
        public List<StoreActivitiesDC> StoreActivitiesList
        {
            get { return storeActivitiesList; }
            set { storeActivitiesList = value; }
        }
    }
}