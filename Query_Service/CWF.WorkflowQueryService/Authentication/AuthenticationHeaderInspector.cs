// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowQueryServiceContext.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



namespace CWF.WorkflowQueryService.Authentication
{
    using System;
    using System.Configuration;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using Resources;

    /// <summary>
    /// Header inspector class. Validate the user that is accessing the service.
    /// </summary>
    public class AuthenticationHeaderInspector : IDispatchMessageInspector
    {
        /// <summary>
        /// Validate the user after receiving a request, and before sending any answer.
        /// </summary>
        /// <param name="request">Request object</param>
        /// <param name="channel">Channel object</param>
        /// <param name="instanceContext">Context for the request</param>
        /// <returns></returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var securityContext = ServiceSecurityContext.Current;

            //Check if the security context is valid, if null means the user didn't provide credentials.
            if (securityContext != null)
            {
                //Check if the user is connected anonymously
                if (securityContext.IsAnonymous)
                {
                    Logging.Log(LoggingValues.InvalidCredentials, System.Diagnostics.EventLogEntryType.Error, null, AuthMessages.AnonymousAccess);
                    throw new UnauthorizedAccessException(AuthMessages.AnonymousAccess);
                }

                //Get config key from the conf file, to obtain the name of the admin  group we validate.
                if ((ConfigurationManager.AppSettings.Keys.Count > 0) && (!string.IsNullOrEmpty(AppSettings.AuthorGroupName)))
                {
                    //Check if the principal for the request is member of the admin group
                    var principal = new WindowsPrincipal(securityContext.WindowsIdentity);
                    if (!principal.IsInRole(AppSettings.AuthorGroupName))
                    {
                        string message = string.Format(
                            AuthMessages.InvalidCredentials,
                            securityContext.WindowsIdentity.Name,
                            AppSettings.AuthorGroupName);
                        var ex = new UnauthorizedAccessException(message);
                        Logging.Log(LoggingValues.InvalidCredentials, System.Diagnostics.EventLogEntryType.Error, null, ex);
                        throw ex;
                    }
                }
                else
                {
                    Logging.Log(LoggingValues.InvalidCredentials, System.Diagnostics.EventLogEntryType.Error, null, AuthMessages.ConfigurationMissing);
                    throw new UnauthorizedAccessException(AuthMessages.ConfigurationMissing);
                }
            }
            else
            {
                Logging.Log(LoggingValues.InvalidCredentials, System.Diagnostics.EventLogEntryType.Error, null, AuthMessages.NullCredentials);
                throw new UnauthorizedAccessException(AuthMessages.NullCredentials);
            }

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }
    }
}