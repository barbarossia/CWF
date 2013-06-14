// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentFileItem.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Models
{
    using System.Xml.Linq;
    using Practices.Prism.ViewModel;    

    /// <summary>
    /// The content file item. Content file is XML file that contains content items for build Activity.
    /// </summary>
    public class ContentFileItem : NotificationObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets Content.
        /// </summary>
        public XDocument Content { get; set; }

        /// <summary>
        /// Gets or sets FileName.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets FileShortName.
        /// </summary>
        public string FileShortName { get; set; }

        #endregion
    }
}