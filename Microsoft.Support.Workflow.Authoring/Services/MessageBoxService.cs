// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBoxService.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Microsoft.Support.Workflow.Authoring.Services
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using Microsoft.Support.Workflow.Authoring.Common;
    using Views;
    using System.Reflection;
    using Microsoft.Support.Workflow.Authoring.AddIns;
    using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
    using System.ServiceModel;
    using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;
    /// <summary>
    /// The message box service.
    /// </summary>
    public class MessageBoxService
    {
        #region Constants

        private static readonly string NetWorkerIssuerMSG = TextResources.ServerUnavailableMsg;

        private static readonly string NetWorkTimeoutMsg = TextResources.ServerTimedOutMsg;
        /// <summary>
        /// The name of the application to use in MessageBox
        /// </summary>
        private static readonly string APPLICATION_NAME = TextResources.CwfApplicationName;

        /// <summary>
        /// Message String for the Not Implemented message
        /// </summary>
        private static readonly string MsgNotImplemented = TextResources.FeatureNotImplementedMsgFormat;

        /// <summary>
        /// Warning message to user close the main window while the marketplace view is open
        /// </summary>
        private static readonly string MsgMarketplaceOpened = TextResources.MarketplaceOpenedMsg;

        /// <summary>
        /// Warning message to user close the main window while the marketplace is downloading
        /// </summary>
        private static readonly string MsgMarketplaceDownloading = TextResources.MarketplaceDownloadingMsg;

        /// <summary>
        /// Error message when user tries to re-import an assembly
        /// </summary>
        private static readonly string MsgAssemblyAlreadyLoaded = TextResources.AssemblyAlreadyImportedMsg;

        /// <summary>
        /// Error message when .wf file fails deserialization
        /// </summary>
        private static readonly string MsgFileIsCorrupt = TextResources.FileContainsNoWorkflowMsgFormat;

        /// <summary>
        /// Message to user about the Data Dirty state
        /// </summary>
        private static readonly string MSG_DATA_DIRTY = TextResources.DataDirtyMsgFormat;

        /// <summary>
        /// Message to non-admin user if project was locked.
        /// </summary>
        private static readonly string MSG_CANNOT_SAVE_LOCKED_ACTIVITY = TextResources.CannotSaveLockedActivityMsg;

        /// <summary>
        /// Message to non-admin user while he opening a locked activity.
        /// </summary>
        private static readonly string MSG_NONADMIN_OPEN_LOCKED_ACTIVITY_FORMAT = TextResources.OpenInReadonlyMsgFormat;

        /// <summary>
        /// Message to admin when the workflow lock has changed by someone else
        /// </summary>
        private static readonly string MSG_ADMIN_LOCK_CHANGED_FORMAT = TextResources.UnlockConfirmationMsgFormat;

        /// <summary>
        /// Message to author when the workflow lock has changed by someone else
        /// </summary>
        private static readonly string MSG_NONADMIN_LOCK_CHANGED_FORMAT = TextResources.CannotUnlockMsgFormat;

        /// <summary>
        /// Message to author if he want to keep locked when the workflow closed
        /// </summary>
        private static readonly string MSG_KEEP_LOCKED = TextResources.KeepLockMsgFormat;

        /// <summary>
        /// Message to user about the unlock confirmation
        /// </summary>
        private static readonly string MSG_UNLOCK_CONFIRMATION = TextResources.SaveAndUnlockConfirmationMsgFormat;

        /// <summary>
        /// Message to ADMIN if a newer version exists.
        /// </summary>
        private static readonly string MSG_CREATE_NEW_ACTIVITY = TextResources.MarketplaceCreateNewerVersionMsg;

        /// <summary>
        /// Message to non-admin user if a newer version exists.
        /// </summary>
        private static readonly string MSG_DOWNLOAD_ACTIVITY = TextResources.MarketplaceDownloadNewerVersionMsg;

        /// <summary>
        /// Message to save the workflow which is already in server
        /// </summary>
        private static readonly string MSG_CANNOT_SAVE_DUPLICATED_ACTIVITY = TextResources.CannotSaveDuplicatedWorkflowMsgFormat;

        private static readonly string COMPILE_WORKFLOW_SUCCESSFULLY = TextResources.CompileWorkflowSuccessfullyMsgFormat;

        private static readonly string DeleteWorkflowConfirmationMsg = TextResources.DeleteWorkflowConfirmationMsg;

        private static readonly string ConfirmationMsg = TextResources.Confirmation;

        private static readonly string NeedToSaveWorkflowMsg = TextResources.SaveWorkflowFirstMsg;

        #endregion Constants

        public static void ShouldSaveWorkflow()
        {
            Show(NeedToSaveWorkflowMsg, TextResources.Information, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static bool ShouldDeleteWorkflow()
        {
            return Show(DeleteWorkflowConfirmationMsg, ConfirmationMsg, MessageBoxButton.YesNo, MessageBoxImage.None) == MessageBoxResult.Yes;
        }
        /// <summary>
        /// Use multiple vesion assembly in a workflow
        /// </summary>
        /// <param name="message"></param>
        public static void CompileSuccessed(string name)
        {
            Show(string.Format(COMPILE_WORKFLOW_SUCCESSFULLY, name), TextResources.Information, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Cannot check assembly because another version of its dependencies is checked
        /// </summary>
        public static void CannotCheckAssemblyForAnotherVersionSelected(string assemblyName, string versionToCheck, string checkedVersion)
        {
            Show(string.Format(TextResources.AssemblyVersionConflictMsgFormat,
                assemblyName, versionToCheck, checkedVersion), TextResources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Cannot check assembly because multiple versions used
        /// </summary>
        public static void CannotCheckAssemblyItself()
        {
            Show(TextResources.CannotImportSelfMsg,
                TextResources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Cannot uncheck assembly because it is referenced
        /// </summary>
        /// <param name="references"></param>
        public static void CannotUncheckAssemblyForReferenced(AssemblyName name, AssemblyName[] references)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format(TextResources.UncheckingAssemblyReferencedMsgFormat, name.Name));
            foreach (AssemblyName parent in references)
            {
                sb.AppendLine(string.Format(TextResources.AssemblyLineInfoFormat, parent.Name, parent.Version));
            }
            Show(sb.ToString(), TextResources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        #region Methods


        /// <summary>
        /// Ask the user if they want to save changes
        /// </summary>
        /// <returns>If true then the user doesn't care about losing changes</returns>
        public static SavingResult? ShowClosingConfirmation(string activityName)
        {
            return MessageBoxService.ShowSavingConfirmation(string.Format(MSG_DATA_DIRTY, activityName), APPLICATION_NAME, true);
        }

        /// <summary>
        /// Ask the user if they want to save changes
        /// </summary>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public static SavingResult? ShowUnlockConfirmation(string activityName)
        {
            return MessageBoxService.ShowSavingConfirmation(string.Format(MSG_UNLOCK_CONFIRMATION, activityName), APPLICATION_NAME, false);
        }

        /// <summary>
        /// Ask the user when open the local workflow, if they want to save changes
        /// </summary>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public static SavingResult? ShowLocalSavingConfirmation(string activityName)
        {
            return MessageBoxService.ShowLocalSavingConfirmation(string.Format(MSG_DATA_DIRTY, activityName), APPLICATION_NAME);
        }

        /// <summary>
        /// Ask the user when open the local workflow, if they want to save changes
        /// </summary>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public static SavingResult? ShowKeepLockedConfirmation(string activityName)
        {
            return MessageBoxService.ShowKeepLockedConfirmation(string.Format(MSG_KEEP_LOCKED, activityName), APPLICATION_NAME);
        }

        /// <summary>
        /// ask user if they want to close marketplace
        /// </summary>
        /// <returns></returns>
        public static bool ShoudExitWithMarketplaceOpened()
        {
            return Show(MsgMarketplaceOpened, APPLICATION_NAME, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        /// <summary>
        /// ask user if they want to cancel download.
        /// </summary>
        /// <returns></returns>
        public static bool ShoudExitWithMarketplaceDownloading()
        {
            return Show(MsgMarketplaceDownloading, APPLICATION_NAME, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        public static void NotifyUploadResult(string result, bool isSucceed)
        {
            if (isSucceed)
                Show(result, TextResources.UploadAssembly, MessageBoxButton.OK, MessageBoxImage.Information);
            else
                Show(result, TextResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }


        public static MessageBoxResult ShowError(string messageBoxText)
        {
            return Show(messageBoxText, APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowException(Exception ex, string errorMsg)
        {
            string details = (ex is UserFacingException)
                            ? ex.InnerException.IfNotNull(inner => inner.ToString())
                            : ex.ToString();

            string msg = string.Empty;
            if (ex is UserFacingException)
                msg = ex.Message;
            else if (ex is CommunicationException)
                msg = NetWorkerIssuerMSG;
            else if (ex is TimeoutException)
                msg = NetWorkTimeoutMsg;
            if (string.IsNullOrEmpty(msg))
                msg = !string.IsNullOrEmpty(errorMsg) ? errorMsg : ex.Message;
            ErrorMessageDialog.Show(msg,
                details,
                Utility.FuncGetCurrentActiveWindow(Application.Current)
                );
        }

        public static MessageBoxResult ShowInfo(string messageBoxText)
        {
            return Show(messageBoxText, APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return ShowFunc(messageBoxText, caption, button, icon, MessageBoxResult.OK);
        }

        public static MessageBoxResult ShowClickable(string messageBoxText, string caption, string url)
        {
            return ShowClickableFunc(messageBoxText, caption, url);
        }

        /// <summary>
        /// This encapsulates the method that shows the clickable dialog. Necessary to suppress the message boxes during test automation
        /// </summary>
        public static Func<string, string, bool, bool, bool, SavingResult?> ShowSavingConfirmationFunc = ((msg, caption, canKeepLocked, shouldUnlock, unlockVisibility) =>
        {
            return SavingConfirmation.ShowAsDialog(msg, caption, canKeepLocked, shouldUnlock, unlockVisibility);
        });

        public static SavingResult? ShowSavingConfirmation(string messageBoxText, string caption, bool canKeepLocked)
        {
            return ShowSavingConfirmationFunc(messageBoxText, caption, canKeepLocked, true, true);
        }

        public static MessageBoxResult LockChangedWhenAdminUnlocking(string lockedBy)
        {
            return Show(string.Format(MSG_ADMIN_LOCK_CHANGED_FORMAT, lockedBy),
                APPLICATION_NAME,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
        }

        public static void LockChangedWhenAuthorUnlocking(string lockedBy)
        {
            Show(string.Format(MSG_NONADMIN_LOCK_CHANGED_FORMAT, lockedBy),
                APPLICATION_NAME,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public static SavingResult? ShowLocalSavingConfirmation(string messageBoxText, string caption)
        {
            return ShowSavingConfirmationFunc(messageBoxText, caption, false, false, false);
        }

        public static SavingResult? ShowKeepLockedConfirmation(string messageBoxText, string caption)
        {
            return ShowSavingConfirmationFunc(messageBoxText, caption, false, true, false);
        }

        /// <summary>
        /// Open a locked activity by a non-admin user.
        /// </summary>
        /// <param name="lockedBy"></param>
        public static void OpenLockedActivityByNonAdmin(string lockedBy)
        {
            Show(string.Format(MSG_NONADMIN_OPEN_LOCKED_ACTIVITY_FORMAT, lockedBy),
                  APPLICATION_NAME,
                  MessageBoxButton.OK,
                  MessageBoxImage.Information);
        }

        /// <summary>
        /// Overwrite activity or not.
        /// </summary>
        /// <returns></returns>
        public static MessageBoxResult CannotSaveLockedActivity()
        {
            return Show(MSG_CANNOT_SAVE_LOCKED_ACTIVITY, APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// When save a project which has a same name in marketplace.
        /// </summary>
        /// <returns></returns>
        public static void CannotSaveDuplicatedNameWorkflow(string workflowName)
        {
            Show(string.Format(MSG_CANNOT_SAVE_DUPLICATED_ACTIVITY, workflowName), APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Overwrite activity or not.
        /// </summary>
        /// <returns></returns>
        public static MessageBoxResult CreateNewActivityOnSaving()
        {
            return Show(MSG_CREATE_NEW_ACTIVITY, APPLICATION_NAME, MessageBoxButton.YesNo, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Download the latest activity or not.
        /// </summary>
        /// <returns></returns>
        public static MessageBoxResult DownloadNewActivityOnSaving()
        {
            return Show(MSG_DOWNLOAD_ACTIVITY, APPLICATION_NAME, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        /// <summary>
        /// This encapsulates the method that shows the clickable dialog. Necessary to suppress the message boxes during test automation
        /// </summary>
        public static Func<string, string, MessageBoxResult> ShowOpenActivityConfirmationFunc = ((msg, caption) =>
        {
            return OpenActivityConfirmation.ShowAsDialog(msg, caption);
        });

        public static MessageBoxResult ShowOpenActivityConfirmation(string messageBoxText, string caption)
        {
            return ShowOpenActivityConfirmationFunc(messageBoxText, caption);
        }

        public static Func<string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult, MessageBoxResult> ShowFunc = ((msg, caption, button, icon, defaultResult) => MessageBox.Show(msg, caption, button, icon, defaultResult));

        /// <summary>
        /// This encapsulates the method that shows the clickable dialog. Necessary to suppress the message boxes during test automation
        /// </summary>
        public static Func<string, string, string, MessageBoxResult> ShowClickableFunc = ((msg, caption, url) =>
        {
            ClickableMessage.ShowAsDialog(msg, caption, url);
            return MessageBoxResult.OK;
        });

        #endregion

        internal static void AssemblyAlreadyLoaded()
        {
            Show(MsgAssemblyAlreadyLoaded, APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

    }
}
