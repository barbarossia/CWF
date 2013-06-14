//-----------------------------------------------------------------------
// <copyright file="PublishingRequest.cs" company="Microsoft">
// Copyright
// Publishing Request
// </copyright>
//-----------------------------------------------------------------------
namespace CWF.DataContracts
{
    #region References
    using System;   
    using System.Runtime.Serialization;
    using CWF.DataContracts;    
    #endregion References

    #region Publishing Request

    /// <summary>
    /// Publishing Request
    /// </summary>
    [DataContract]
    public class PublishingRequest : RequestHeader
    {
        #region Public properties
        /// <summary>
        /// Name of the workfloow to publish
        /// </summary>
        [DataMember]
        public string WorkflowName { get; set; }
        /// <summary>
        /// Version of the workflow to publish
        /// </summary>
        [DataMember]
        public string WorkflowVersion { get; set; }
        #endregion Public properties
    }

	#endregion Publishing Request
}