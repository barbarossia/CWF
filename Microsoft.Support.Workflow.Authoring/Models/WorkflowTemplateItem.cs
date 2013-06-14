// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowTemplateItem.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Microsoft.Support.Workflow.Authoring.Models
{
    using Properties;

    // Class to encapsulate the Workflow Template Item
    public class WorkflowTemplateItem 
    {
        #region Declarations and Constants
        /// <summary>
        /// Definition of a blank workflow
        /// </summary>
        public static string BlankWorkflowXaml = Resources.EmptyWorkflowTemplate;
        private const string TostringFormat = "{0}";
        #endregion Declarations and Constants

        #region Ctor

        /// <summary>
        /// Constructor that is used to create an appropriate blank workflow
        /// </summary>
        /// <param name="id">Id of the template</param>
        /// <param name="name">Name of the template</param>
        public WorkflowTemplateItem(int id, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            WorkflowTemplateId = id;
            WorkflowTypeName = name;
        }

        #endregion Ctor

        #region Public Properties

        /// <summary>
        /// Stores the ID that is associated with the workflow template which is how we get the XAML for the template
        /// </summary>
        public int WorkflowTemplateId { get; set; }
        public string WorkflowTypeName { get; set; }

        #endregion Public Properties
        
        #region Overridden ToString
        /// <summary>
        /// Overridden human readable version of the class instance
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(TostringFormat, WorkflowTypeName);
        }
        #endregion Overridden ToString
    }
}
