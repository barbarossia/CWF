//-----------------------------------------------------------------------
// <copyright file="ActivityCategoryByNameGetReplyDC.cs" company="Microsoft">
// Copyright
// ActivityCategory By Name Get Reply DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Reply data contract for ActivityCategoryByNameGet
    /// </summary>
    [DataContract]
    public class ActivityCategoryByNameGetReplyDC : ReplyHeader
    {
        
        /// <summary>
        /// PK of the activityCategory
        /// </summary>
        [DataMember]
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// GUID of the ActivityCategory
        /// </summary>
        [DataMember]
        public Guid Guid
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the ActivityCategory
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Description of the ActivityCategory
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Metatags associated with the ActivityCategory. Separate with the ";" character
        /// </summary>
        [DataMember]
        public string MetaTags
        {
            get;
            set;
        }

        /// <summary>
        /// AuthGroupName that ActivityCategory is associated with
        /// </summary>
        [DataMember]
        public string AuthGroupName
        {
            get;
            set;
        }

        /// <summary>
        /// The PK of the AuthGroup
        /// </summary>
        [DataMember]
        public int AuthGroupId
        {
            get;
            set;
        }
    }
}
