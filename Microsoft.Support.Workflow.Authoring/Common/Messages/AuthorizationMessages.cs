// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationMessages.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Common.Messages
{
    using System;
    using Resources;

    /// <summary>
    /// Default messages for authorization operations.
    /// </summary>
     internal static class AuthorizationMessages
     {

         /// <summary>
         /// User is running the tool offline, there is no way to validate the credentials of the account.
         /// </summary>
         public readonly static string Offline = string.Format("We are unable to validate your permissions for access to the Common Workflow Foundry. " +
             Environment.NewLine + "You can request assistance by sending an email to {0} or try again at a later time.", 
             AppSettings.AuthorizationContactEmail);

         /// <summary>
         /// User is not authorized to use the tool.
         /// </summary>
         public readonly static string Unauthorized =
             string.Format("Your network account is not authorized to run the Common Workflow Foundry. " + Environment.NewLine +
                           "You can request access by sending an email to {0}.", 
                           AppSettings.AuthorizationContactEmail);
     }
}
