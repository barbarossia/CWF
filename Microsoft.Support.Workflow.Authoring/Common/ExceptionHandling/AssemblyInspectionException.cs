// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInspectionException.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2012.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling
{
    using System;

    /// <summary>
    /// Throw this exception for any errors that occur when analyzing assemblies
    /// </summary>
    [Serializable]
    public class AssemblyInspectionException : UserFacingException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="message"></param>
        public AssemblyInspectionException(string message) : base(message)
        {
                
        }

        /// <summary>
        /// Constructor with parameters for stack trace checking.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public AssemblyInspectionException(string message, Exception innerException) :
            base(message,innerException)
        {
                    
        }
    }
}