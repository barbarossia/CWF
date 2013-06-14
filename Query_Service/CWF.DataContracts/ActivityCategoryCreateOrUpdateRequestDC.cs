//-----------------------------------------------------------------------
// <copyright file="ActivityCategoryCreateOrUpdateRequestDC.cs" company="Microsoft">
// Copyright
// ActivityCategory Create Or Update Request DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Request data contract for ActivityCategoryCreateOrUpdate.
    /// </summary>
    [DataContract]
    public class ActivityCategoryCreateOrUpdateRequestDC : RequestHeader
    {
        private int inId ;
        private Guid inGuid;
        private string inName;
        private string inDescription;
        private string inMetaTags;
        private string inAuthGroupName;

        /// <summary>
        /// PK of the ActivityCategory (only for update)
        /// </summary>
        [DataMember]
        public int InId
        {
            get { return inId; }
            set { inId = value; }
        }

        /// <summary>
        /// Guid of the activityCategory
        /// </summary>
        [DataMember]
        public Guid InGuid
        {
            get { return inGuid; }
            set { inGuid = value; }
        }

        /// <summary>
        /// Name of the ActivityCategory
        /// </summary>
        [DataMember]
        public string InName
        {
            get { return inName; }
            set { inName = value; }
        }

        /// <summary>
        /// Description of the ActivityCategory
        /// </summary>
        [DataMember]
        public string InDescription
        {
            get { return inDescription; }
            set { inDescription = value; }
        }

        /// <summary>
        /// Metatage of the ActivityCategory
        /// </summary>
        [DataMember]
        public string InMetaTags
        {
            get { return inMetaTags; }
            set { inMetaTags = value; }
        }

        /// <summary>
        /// AuthGroup Name of the ActivityCategory
        /// </summary>
        [DataMember]
        public string InAuthGroupName
        {
            get { return inAuthGroupName; }
            set { inAuthGroupName = value; }
        }
    }
}
