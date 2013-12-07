//-----------------------------------------------------------------------
// <copyright file="WCFconsoleOutputMessageInspector.cs" company="Microsoft">
// Copyright
// WCF pipeline POC MessageInspector
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DCValidation
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    /// <summary>
    /// Message inspector pipeline class/method
    /// </summary>
    public class WCFconsoleOutputMessageInspector : IDispatchMessageInspector
    {
        /// <summary>
        /// After receive request pipeline
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <param name="instanceContext"></param>
        /// <returns>object object</returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
            request = buffer.CreateMessage();
            Console.WriteLine("Received:\n{0}", buffer.CreateMessage().ToString());
            return null;
        }

        /// <summary>
        /// Before send request pipeline
        /// </summary>
        /// <param name="reply">ref Message object</param>
        /// <param name="correlationState">correlationState object</param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            MessageBuffer buffer = reply.CreateBufferedCopy(Int32.MaxValue);
            reply = buffer.CreateMessage();
            Console.WriteLine("Sending:\n{0}", buffer.CreateMessage().ToString());
        }
    }
}
