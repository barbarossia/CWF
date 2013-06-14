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

    /// <summary>
    /// The message box service.
    /// </summary>
    public class MessageBoxService
    {
        #region Constants

        /// <summary>
        /// The name of the application to use in MessageBox
        /// </summary>
        private const string APPLICATION_NAME = "Common Workflow Foundry";

        /// <summary>
        /// Message String for the Not Implemented message
        /// </summary>
        private const string MsgNotImplemented = "Feature \"{0}\" has not yet been implemented";

        /// <summary>
        /// Upload succeeded message
        /// </summary>
        private const string MsgUploadSucceeded = "The upload the of the assembly named \"{0}\" was successful.";

        /// <summary>
        /// Message to user about the Data Dirty state
        /// </summary>
        private const string MsgDataDirty = "Do you want to close without saving your changes?";

        /// <summary>
        /// Warning message to user close the main window while the marketplace view is open
        /// </summary>
        private const string MsgMarketplaceOpened = "Do you want to close while the Marketplace View is open?";

        /// <summary>
        /// Warning message to user close the main window while the marketplace is downloading
        /// </summary>
        private const string MsgMarketplaceDownloading = "Do you want to close while the Marketplace is processing download?";

        /// <summary>
        /// Error message when user tries to re-import an assembly
        /// </summary>
        private const string MsgAssemblyAlreadyLoaded = "This library has already been imported.";

        /// <summary>
        /// Error message when .wf file fails deserialization
        /// </summary>
        private const string MsgFileIsCorrupt = "The file '{0}' does not contain a valid workflow";

        /// <summary>
        /// Message to user about the Data Dirty state
        /// </summary>
        private const string MSG_DATA_DIRTY = "Do you want to save changes to {0} before closing?";

        /// <summary>
        /// Message to non-admin user if project was locked.
        /// </summary>
        private const string MSG_CANNOT_SAVE_LOCKED_ACTIVITY = "This project was locked by an administrator.  You are not allowed to save your changes to the server.";

        /// <summary>
        /// Message to admin while he opening a locked activity.
        /// </summary>
        private const string MSG_OPEN_ACTIVITY_FORMAT = "Will you open the workflow {0} for editing or readonly?";


        /// <summary>
        /// Message to non-admin user while he opening a locked activity.
        /// </summary>
        private const string MSG_NONADMIN_OPEN_LOCKED_ACTIVITY_FORMAT = "This workflow was locked by {0}. You will open it in read-only mode.";

        /// <summary>
        /// Message to admin when the workflow lock has changed by someone else
        /// </summary>
        private const string MSG_ADMIN_LOCK_CHANGED_FORMAT = "The workflow was locked by {0}. Would you like to unlock it?";

        /// <summary>
        /// Message to author when the workflow lock has changed by someone else
        /// </summary>
        private const string MSG_NONADMIN_LOCK_CHANGED_FORMAT = "The workflow was locked by administrator {0}. You cannot unlock it now.";

        /// <summary>
        /// Message to author if he want to keep locked when the workflow closed
        /// </summary>
        private const string MSG_KEEP_LOCKED = "{0} is locked by yourself, do you want to keep the lock?";

        /// <summary>
        /// Message to user about the unlock confirmation
        /// </summary>
        private const string MSG_UNLOCK_CONFIRMATION = "Do you want to save changes to {0} before unlock? The workflow will be read-only after unlocked.";

        /// <summary>
        /// Message to ADMIN if a newer version exists.
        /// </summary>
        private const string MSG_CREATE_NEW_ACTIVITY = "A newer version of this project exists in the Marketplace.  Would you like to create a newer version?";

        /// <summary>
        /// Message to non-admin user if a newer version exists.
        /// </summary>
        private const string MSG_DOWNLOAD_ACTIVITY = "A new version of this project exists in the Marketplace, you are not allowed to save your changes to the server. It is suggested that you save your work locally. Do you want to download the newer version?";

        /// <summary>
        /// Message to save the workflow which is already in server
        /// </summary>
        private const string MSG_CANNOT_SAVE_DUPLICATED_ACTIVITY = "The name \"{0}\" already exists in server, please use another one.";

        private const string MSG_PRINT_CONFIRMATION = "You are going to print thses activities on [ {0} ] pages of [ {1} ] paper with [ {2} ]. Are you sure?";

        private const string MSG_PRINT_ERROR = "Print failed due to {0}.";

        private const string COMPILE_WORKFLOW_SUCCESSFULLY = "Compiled the workflow {0} successfully.";

        #endregion Constants

        
        /// <summary>
        /// Use multiple vesion assembly in a workflow
        /// </summary>
        /// <param name="message"></param>
        public static void CompileSuccessed(string name)
        {
            Show(string.Format(COMPILE_WORKFLOW_SUCCESSFULLY, name), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Cannot check assembly because another version of its dependencies is checked
        /// </summary>
        public static void CannotCheckAssemblyForAnotherVersionSelected(string assemblyName, string versionToCheck, string checkedVersion)
        {
            Show(string.Format("The assembly you checked needs \"{0}\" version {1}. You cannot import it unless \"{0}\" version {2} is unchecked.",
                assemblyName, versionToCheck, checkedVersion), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Cannot check assembly because multiple versions used
        /// </summary>
        public static void CannotCheckAssemblyForItselfSelected(string assemblyName)
        {
            Show(string.Format("The same name {0} of the assembly you checked cannot import to itself because multiple versions used.",
                assemblyName), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Cannot uncheck assembly because it is referenced
        /// </summary>
        /// <param name="references"></param>
        public static void CannotUncheckAssemblyForReferenced(AssemblyName name, AssemblyName[] references)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("You cannot uncheck \"{0}\" because following assemblies referenced it:", name.Name));
            foreach (AssemblyName parent in references)
            {
                sb.AppendLine(string.Format("\"{0}\" version {1}", parent.Name, parent.Version));
            }
            Show(sb.ToString(), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        #region Methods
        /// <summary>
        /// Ask the user if they want to save changes
        /// </summary>
        /// <returns>If true then the user doesn't care about losing changes</returns>
        public static SavingResult? ShowClosingComfirmation(string activityName)
        {
            return MessageBoxService.ShowSavingComfirmation(string.Format(MSG_DATA_DIRTY, activityName), APPLICATION_NAME, true);
        }

        /// <summary>
        /// Ask the user if they want to save changes
        /// </summary>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public static SavingResult? ShowUnlockComfirmation(string activityName)
        {
            return MessageBoxService.ShowSavingComfirmation(string.Format(MSG_UNLOCK_CONFIRMATION, activityName), APPLICATION_NAME, false);
        }

        /// <summary>
        /// Ask the user when open the local workflow, if they want to save changes
        /// </summary>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public static SavingResult? ShowLocalSavingComfirmation(string activityName)
        {
            return MessageBoxService.ShowLocalSavingComfirmation(string.Format(MSG_DATA_DIRTY, activityName), APPLICATION_NAME);
        }

        /// <summary>
        /// Ask the user when open the local workflow, if they want to save changes
        /// </summary>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public static SavingResult? ShowKeepLockedComfirmation(string activityName)
        {
            return MessageBoxService.ShowKeepLockedComfirmation(string.Format(MSG_KEEP_LOCKED, activityName), APPLICATION_NAME);
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
                Show(result, "Upload Assembly", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                Show(result, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        public static MessageBoxResult ShowError(string messageBoxText)
        {
            return Show(messageBoxText, APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Error);
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
        public static Func<string, string, bool, bool, bool, SavingResult?> ShowSavingComfirmationFunc = ((msg, caption, canKeepLocked, shouldUnlock, unlockVisibility) =>
        {
            return SavingComfirmation.ShowAsDialog(msg, caption, canKeepLocked, shouldUnlock, unlockVisibility);
        });

        public static SavingResult? ShowSavingComfirmation(string messageBoxText, string caption, bool canKeepLocked)
        {
            return ShowSavingComfirmationFunc(messageBoxText, caption, canKeepLocked, true, true);
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

        public static SavingResult? ShowLocalSavingComfirmation(string messageBoxText, string caption)
        {
            return ShowSavingComfirmationFunc(messageBoxText, caption, false, false, false);
        }

        public static SavingResult? ShowKeepLockedComfirmation(string messageBoxText, string caption)
        {
            return ShowSavingComfirmationFunc(messageBoxText, caption, false, true, false);
        }

        /// <summary>
        /// Open a locked activity by an admin.
        /// </summary>
        /// <param name="workflowName"></param>
        public static MessageBoxResult OpenActivity(string workflowName)
        {
            return MessageBoxService.ShowOpenActivityConfirmation(
                string.Format(MSG_OPEN_ACTIVITY_FORMAT, workflowName), APPLICATION_NAME);
        }

        /// <summary>
        /// Open a locked activity by a non-admin user.
        /// </summary>
        /// <param name="lockedBy"></param>
        public static MessageBoxResult OpenLockedActivityByNonAdmin(string lockedBy)
        {
            return Show(string.Format(MSG_NONADMIN_OPEN_LOCKED_ACTIVITY_FORMAT, lockedBy),
                  APPLICATION_NAME,
                  MessageBoxButton.OKCancel,
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
