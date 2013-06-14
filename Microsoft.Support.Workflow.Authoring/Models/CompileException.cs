// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompileException.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2012.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Models
{
    using System;

    /// <summary>
    /// Custom exception for compile operations.
    /// </summary>
    [Serializable]
    public class CompileException : Exception 
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="message"></param>
        public CompileException(string message) : base(message)
        {
                
        }

        /// <summary>
        /// Constructor with parameters for stack trace checking.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public CompileException(string message, Exception innerException) :
            base(message,innerException)
        {
                    
        }
    }
}
