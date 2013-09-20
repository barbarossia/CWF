namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;
    using System.Collections.Generic;

    [DataContract]
    public class TaskActivityDC : WorkflowRequestReplayHeader
    {
        /// <summary>
        /// The identity for the row entry
        /// </summary>
        [DataMember]
        public int Id { get; set; }


        /// <summary>
        /// The TaskActivity Id in activity table
        /// </summary>
        [DataMember]
        public int ActivityId { get; set; }

        /// <summary>
        /// The TaskActivity
        /// </summary>
        [DataMember]
        public StoreActivitiesDC Activity { get; set; }

        /// <summary>
        /// The identity for a TaskActivity
        /// </summary>
        [DataMember]
        public Guid Guid { get; set; }

        /// <summary>
        /// The alias who owner the TaskActivity
        /// </summary>
        [DataMember]
        public string AssignedTo { get; set; }

        /// <summary>
        /// Task Activity Status 
        /// Initilized, Editing,CheckedIn
        /// </summary>
        [DataMember]
        public TaskActivityStatus Status { get; set; }

    }
}
