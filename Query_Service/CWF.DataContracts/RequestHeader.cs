//-----------------------------------------------------------------------
// <copyright file="RequestHeader.cs" company="Microsoft">
// Copyright
// RequestHeader class
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Common RequestHeader
    /// </summary>
    [DataContract]
    public class RequestHeader
    {
        private string incaller;
        private string incallerVersion;
        private string inInsertedByUserAlias;
        private string inUpdatedByUserAlias;

        /// <summary>
        /// Caller alias for error log tracking
        /// </summary>
        [DataMember]
        public string Incaller
        {
            get { return incaller; }
            set { incaller = value; }
        }

        /// <summary>
        /// Caller version for error log tracking
        /// </summary>
        [DataMember] 
        public string IncallerVersion
        {
            get { return incallerVersion; }
            set { incallerVersion = value; }
        }

        /// <summary>
        /// Caller alias for error log tracking
        /// </summary>
        [DataMember]
        public string InInsertedByUserAlias
        {
            get { return inInsertedByUserAlias; }
            set { inInsertedByUserAlias = value; }
        }

        /// <summary>
        /// Caller version for error log tracking
        /// </summary>
        [DataMember]
        public string InUpdatedByUserAlias
        {
            get { return inUpdatedByUserAlias; }
            set { inUpdatedByUserAlias = value; }
        }
    }
}
