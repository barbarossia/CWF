//-----------------------------------------------------------------------
// <copyright file="MessageInspector.cs" company="Microsoft">
// Copyright
// Comments required for potential BAL candidate
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DCValidation
{
    //// using System;
    //// using System.Collections.Generic;
    //// using System.Configuration;
    //// using System.Linq;
    //// using System.ServiceModel;
    //// using System.ServiceModel.Channels;
    //// using System.ServiceModel.Configuration;
    //// using System.ServiceModel.Description;
    //// using System.ServiceModel.Dispatcher;
    //// using System.Text;
    //// using System.Xml;
    //// using System.Xml.Schema;

    //// public class MessageValidationInspector : IDispatchMessageInspector, IClientMessageInspector 
    //// {
    ////    XmlSchemaSet schemas;
    ////    public MessageValidationInspector(XmlSchemaSet schemas)
    ////    {
    ////        this.schemas = schemas;
    ////    }

    ////    /// <summary>
    ////    /// Reads input message and creates a copy to pass to the WCF method
    ////    /// </summary>
    ////    /// <param name="message"></param>
    ////    void ValidateMessage(ref System.ServiceModel.Channels.Message message)
    ////    {
    ////        XmlDocument bodyDoc = new XmlDocument();
    ////        bodyDoc.Load(message.GetReaderAtBodyContents());
    ////        XmlReaderSettings settings = new XmlReaderSettings();
    ////        settings.Schemas.Add(schemas);
    ////        settings.ValidationType = ValidationType.Schema;
    ////        XmlReader r = XmlReader.Create(new XmlNodeReader(bodyDoc), settings);
    ////        while (r.Read());
    ////        //// Create new message
    ////        Message newMsg = Message.CreateMessage(message.Version, null, new XmlNodeReader(bodyDoc.DocumentElement));
    ////        foreach (string propertyKey in message.Properties.Keys)
    ////            newMsg.Properties.Add(propertyKey, message.Properties[propertyKey]);
    ////        //// Close the original message and return new message
    ////        message.Close();
    ////        message = newMsg;
    ////    }

    ////    //// overide
    ////    object IDispatchMessageInspector.AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, 
    ////                                                            System.ServiceModel.IClientChannel channel, 
    ////                                                            System.ServiceModel.InstanceContext instanceContext) 
    ////    {
    ////        //String s1 = instanceContext.GetServiceInstance().GetType().Name;
    ////        //var action = OperationContext.Current.IncomingMessageHeaders.Action; 
    ////        //var operationName = action.Substring(action.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1);
    ////        //try
    ////        //{
    ////        ////    ValidateMessage(ref request);
    ////        //}
    ////        //catch (FaultException fex)
    ////        //{
    ////        ////    throw new FaultException<string>(fex.Message);
    ////        //}
    ////        MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
    ////        request = buffer.CreateMessage();
    ////        Message originalMessage = buffer.CreateMessage();
    ////        foreach (MessageHeader h in originalMessage.Headers)
    ////        {
    ////            Console.WriteLine("\n{0}\n", h);
    ////        }
    ////        return null;

    ////    }

    ////    /// <summary>
    ////    /// Validates the call by calling a specific CheckDCVlaidation(action) method.
    ////    /// </summary>
    ////    /// <param name="call"></param>
    ////    /// <param name="action"></param>
    ////    public void ValidateDcByAction(string call, string action)
    ////    {
    ////        switch (call)
    ////        {
    ////            case "ActivityLibrary":
    ////                ActivityLibrary.CheckDCValidation(action);
    ////                break;
    ////            case "StoreActivity":
    ////                StoreActivity.CheckDCValidation(action);
    ////                break;
    ////        }
    ////    }
    //// }
}
