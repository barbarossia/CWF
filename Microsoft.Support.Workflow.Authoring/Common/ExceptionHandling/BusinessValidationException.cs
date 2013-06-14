using System;

namespace Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling
{
    /// <summary>
    /// This is the wrapper on the UX side for a FaultException of type ValidationFault.
    /// If a FaultException of ValidationFault occurs from the WCF layer, we will use this type internally to wrap that exception.
    /// </summary>
    [Serializable]
    public class BusinessValidationException : Exception
    {
        /// <summary>
        /// Allows setting the message property on the base type.
        /// </summary>
        /// <param name="message">The passthrough parameter for the Message property on the base type.</param>
        public BusinessValidationException(string message) : base(message) { }
    }
}
