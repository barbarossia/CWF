//-----------------------------------------------------------------------
// <copyright file="ApplicationGetBase.cs" company="Microsoft">
// Copyright
// Application Get Base DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Common class for ApplicationGet
    /// </summary>
    [DataContract]
    public class ApplicationGetBase
    {
       
        /// <summary>
        /// PK of the application (returned only)
        /// </summary>
        [DataMember]
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Guid of the application
        /// </summary>
        [DataMember]
        public Guid Guid
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the application
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Description of the application
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }
    }
}
