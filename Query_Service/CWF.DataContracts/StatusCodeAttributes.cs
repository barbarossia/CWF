//-----------------------------------------------------------------------
// <copyright file="StatusCodeAttributes.cs" company="Microsoft">
// Copyright
// Status Code Attributes DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Common base for StatusCode record
    /// </summary>
    [DataContract]
    public class StatusCodeAttributes
    {
        private int code;
        private string name = string.Empty;
        private string description = string.Empty;
        private bool showInProduction = false;
        private bool lockForChanges = false;
        private bool isDeleted = false;
        private bool isEligibleForCleanUp = false;

        /// <summary>
        /// Status code
        /// </summary>
        [DataMember]
        public int Code
        {
            get { return code; }
            set { code = value; }
        }

        /// <summary>
        /// Status code name
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Status code description
        /// </summary>
        [DataMember]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Show in production boolean
        /// </summary>
        [DataMember]
        public bool ShowInProduction
        {
            get { return showInProduction; }
            set { showInProduction = value; }
        }

        /// <summary>
        /// Lock for changes boolean
        /// </summary>
        [DataMember]
        public bool LockForChanges
        {
            get { return lockForChanges; }
            set { lockForChanges = value; }
        }

        /// <summary>
        /// Is deleted boolean
        /// </summary>
        [DataMember]
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; }
        }

        /// <summary>
        /// Is eligible for cleanup boolean
        /// </summary>
        [DataMember]
        public bool IsEligibleForCleanUp
        {
            get { return isEligibleForCleanUp; }
            set { isEligibleForCleanUp = value; }
        }
    }
}
