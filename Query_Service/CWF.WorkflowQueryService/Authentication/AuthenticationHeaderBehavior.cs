// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationHeaderBehavior.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CWF.WorkflowQueryService.Authentication
{
    using System;
    using System.ServiceModel.Configuration;
    using System.ServiceModel.Description;
   
    /// <summary>
    /// Custom behavior for the service.
    /// </summary>
    public class AuthenticationHeaderBehavior : BehaviorExtensionElement, IEndpointBehavior
    {

        /// <summary>
        /// Gets the type of behavior.
        /// </summary>
        public override Type BehaviorType
        {
            get
            {
                return typeof(AuthenticationHeaderBehavior);
            }
        }
        
        /// <summary>
        /// Creates a new custom behavior to be used by the extension.
        /// </summary>
        /// <returns>Newly created behavior</returns>
        protected override object CreateBehavior()
        {
            return new AuthenticationHeaderBehavior();
        }

        /// <summary>
        /// Implement to pass data at runtime to bindings to support custom behavior. Not Used.
        /// </summary>
        /// <param name="endpoint">The endpoint to modify</param>
        /// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Implements a modification or extension of the client across an endpoint. Not used.
        /// </summary>
        /// <param name="endpoint">The endpoint that exposes the contract.</param>
        /// <param name="clientRuntime">The client runtime to be customized.</param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
        }

        /// <summary>
        /// Implements a modification or extension of the service across an endpoint. Not Used.
        /// </summary>
        /// <param name="endpoint">The endpoint that exposes the contract.</param>
        /// <param name="endpointDispatcher">The endpoint dispatcher to be modified or extended.</param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(
                new AuthenticationHeaderInspector());
        }

        /// <summary>
        /// Implement to confirm that the endpoint meets some intended criteria.
        /// </summary>
        /// <param name="endpoint">The endpoint to validate</param>
        public void Validate(ServiceEndpoint endpoint)
        {
            
        }
    }
}
