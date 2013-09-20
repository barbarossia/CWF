//-----------------------------------------------------------------------
// <copyright file="StoreLibraryAndActivitiesRequestDC.cs" company="Microsoft">
// Copyright
// Store Library And Activities DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Common data contract for StoreLibraryAndActivities
    /// </summary>
    [DataContract]
    public class StoreLibraryAndActivitiesRequestDC : RequestHeader
    {
        public StoreLibraryAndActivitiesRequestDC()
        {
            StoreActivityLibraryDependenciesGroupsRequestDC = null;
            StoreActivitiesList = null;
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
        public List<StoreActivitiesDC> StoreActivitiesList { get; set; }

        /// <summary>
        /// List of TaskActivity
        /// </summary>
        [DataMember]
        public List<StoreLibraryAndTaskActivityRequestDC> TaskActivitiesList { get; set; }

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

        public override void AddAuthGroupOnRequest(string[] inAuthGroupName)
        {
            base.AddAuthGroupOnRequest(inAuthGroupName);
            this.ActivityLibrary.AddAuthGroupOnRequest(inAuthGroupName);
            this.StoreActivitiesList.ForEach(s => s.AddAuthGroupOnRequest(inAuthGroupName));
            if (this.TaskActivitiesList != null)
                this.TaskActivitiesList.ForEach(t => t.AddAuthGroupOnRequest(inAuthGroupName));
        }
    }
}
