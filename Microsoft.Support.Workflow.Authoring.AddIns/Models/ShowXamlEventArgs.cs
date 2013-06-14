// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShowXamlEventArgs.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Authoring.AddIns
{
    using System;
    using System.Activities;

    /// <summary>
    /// The show xaml event args.
    /// The Activity property reference to the activity instance whose XAML code will be shown.
    /// </summary>
    public class ShowXamlEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets Activity. Reference to the activity instance whose XAML code will be shown.
        /// </summary>
        public Activity Activity { get; set; }

        #endregion
    }
}