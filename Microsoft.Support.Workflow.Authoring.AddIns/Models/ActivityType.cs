// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityType.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.AddIns.Models
{
    /// <summary>
    /// The type of a Activity or Workflow. In actually, Workflow class inherent Activity class. 
    /// </summary>
    public enum ActivityType
    {
        /// <summary>
        /// The activity. Object is Activity.
        /// </summary>
        Activity,

        /// <summary>
        /// The workflow. Object is Workflow.
        /// </summary>
        Workflow,

        /// <summary>
        /// The template. Object, usually a Workflow, is a Template
        /// </summary>
        Template
    }
}