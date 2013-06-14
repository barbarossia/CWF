namespace CWF.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Common data contract for StoreLibraryAndActivities
    /// </summary>
    [DataContract]
    public class StoreLibraryAndTaskActivityRequestDC : RequestHeader
    {
        public StoreLibraryAndTaskActivityRequestDC()
        {
            StoreActivityLibraryDependenciesGroupsRequestDC = null;
            TaskActivitiesList = null;
            ActivityLibrary = null;
        }

        /// <summary>
        /// Root activity library
        /// </summary>
        [DataMember]
        public ActivityLibraryDC ActivityLibrary { get; set; }

        /// <summary>
        /// List of store activities related to root library
        /// </summary>
        [DataMember]
        public List<TaskActivityDC> TaskActivitiesList { get; set; }

        /// <summary>
        /// List of store activities related to root library
        /// </summary>
        [DataMember]
        public StoreActivityLibraryDependenciesGroupsRequestDC StoreActivityLibraryDependenciesGroupsRequestDC { get; set; }

        /// <summary>
        /// Indicates if the version rules need to be checked for the activity library and its activities.
        /// </summary>
        [DataMember]
        public bool EnforceVersionRules { get; set; }
    }
}
