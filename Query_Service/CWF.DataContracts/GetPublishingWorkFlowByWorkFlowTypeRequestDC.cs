//-----------------------------------------------------------------------
// <copyright file="GetPublishingWorkFlowByWorkFlowTypeRequestDC.cs" company="Microsoft">
// Copyright
// StatusReply DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Request object for Get Publishing WorkFlow By WorkFlowType call
    /// </summary>
    [DataContract]
    public class GetPublishingWorkFlowByWorkFlowTypeRequestDC : RequestHeader
    {
        /// <summary>
        /// Work flow types defined as (Checked in MEL validation logic)
        ///     Not a workflow
        ///     OAS Page
        ///     Publishing Workflow
        ///     Guided Solution
        ///     Template
        ///     OASP Wizard
        ///     OAS Control
        ///     Business
        ///     OASP StepPage
        ///     OAS Application
        ///     Change Control
        ///     Custom Activity
        /// </summary>
        [DataMember]
        public string WorkflowType
        {
            get;
            set;
        }
    }
}
