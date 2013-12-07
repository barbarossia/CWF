//-----------------------------------------------------------------------
// <copyright file="WCFconsoleOutputBehavior.cs" company="Microsoft">
// Copyright
// WCF pipeline POC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DCValidation
{
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    /// <summary>
    /// WCF pipeline POC
    /// </summary>
    public class WCFconsoleOutputBehavior : IEndpointBehavior
    {
        /// <summary>
        /// Adds binding parameters
        /// </summary>
        /// <param name="endpoint">ServiceEndpoint object</param>
        /// <param name="bindingParameters">BindingParameterCollection object</param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Applies client behaviour
        /// </summary>
        /// <param name="endpoint">ServiceEndpoint object</param>
        /// <param name="clientRuntime">ClientRuntime object</param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            throw new Exception("Behavior not supported on the consumer side!");
        }

        /// <summary>
        /// Applies the dispatch behaviour
        /// </summary>
        /// <param name="endpoint">ServiceEndpoint object</param>
        /// <param name="endpointDispatcher">EndpointDispatcher object</param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            WCFconsoleOutputMessageInspector inspector = new WCFconsoleOutputMessageInspector();
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
        }

        /// <summary>
        /// Validates the passed objects
        /// </summary>
        /// <param name="endpoint">ServiceEndpoint object</param>
        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
