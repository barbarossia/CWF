// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBoxService.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Microsoft.Support.Workflow.Authoring.AddIns.Utilities
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using Microsoft.Support.Workflow.Authoring.AddIns;
    using Views;
    using System.Reflection;
    using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

    /// <summary>
    /// The message box service.
    /// </summary>
    public class AddInMessageBoxService
    {
        #region Constants

        private static readonly string MSG_PRINT_CONFIRMATION = TextResources.PrintConfirmationMsgFormat;
        private static readonly string MSG_PRINT_ERROR = TextResources.PrintFailureMsgFormat;
        private static readonly string MSG_CANNOT_ASSIGN = TextResources.AlreadyAssignedMsg;
        private static readonly string MSG_CANNOT_GETLASTVERSION = TextResources.CannotGetLatestVersionMsg;

        #endregion Constants

        public static void DirectoryServiceFailure() {
            Show(TextResources.ADFailureMsg, TextResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Confirmation for printing activities.
        /// </summary>
        /// <param name="pageCount"></param>
        /// <param name="paperName"></param>
        /// <param name="printerName"></param>
        /// <returns></returns>
        public static bool PrintConfirmation(int pageCount, string paperName, string printerName)
        {
            return Show(string.Format(MSG_PRINT_CONFIRMATION, pageCount, paperName, printerName), TextResources.Confirmation, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Cancel print and back to main window
        /// </summary>
        /// <returns></returns>
        public static bool PrintReselectConfirmation()
        {
            return Show(TextResources.CancelWithoutSavingConfirmationMsg, TextResources.Confirmation, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Confirmation for select nothing.
        /// </summary>
        public static void PrintNoneSelectMessage()
        {
            Show(TextResources.ChooseActivityMsg, TextResources.Confirmation, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Confirmation cannot print workflow that nested level over 25 
        /// </summary>
        /// <param name="name"></param>
        public static void PrintOverflowWorkflow(string name)
        {
            Show(string.Format(TextResources.NestedIssueMsg, name), TextResources.Confirmation, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Confirmation for select nothing.
        /// </summary>
        public static void PrintNoneActivityMessage()
        {
            Show(TextResources.NothingToPrintMsg, TextResources.Confirmation, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Display error message when printing failed.
        /// </summary>
        /// <param name="message"></param>
        public static void PrintFailed(string message)
        {
            Show(string.Format(MSG_PRINT_ERROR, message), TextResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display error message when user want to assign activity which has been assigned.
        /// </summary>
        public static void CannotAssignTaskActivity()
        {
            Show(MSG_CANNOT_ASSIGN, TextResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display error message when usre want to get last version activity.
        /// </summary
        public static void CannotGetLastVersion()
        {
            Show(MSG_CANNOT_GETLASTVERSION, TextResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display warning when user want to merge.
        /// </summary
        public static MessageBoxResult CannotMergeNextTime()
        {
            return Show(TextResources.UnassignConfirmationMsg, TextResources.Information, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Display message where no task to be merge.
        /// </summary>
        public static void CannotMergeAll()
        {
            Show(TextResources.NoTaskToMergeMsg, TextResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display message where no task to be merge.
        /// </summary>
        public static void CannotMergeSpecialTask()
        {
            Show(TextResources.NoTaskCanMergeMsg, TextResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display message that task activity cannot assign in another task.
        /// </summary>
        public static void CannotAssign()
        {
            Show(TextResources.AddTaskInAnotherMsg, TextResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display message that task activity cannot be assigned.
        /// </summary>
        public static void CannotAssignUseSpecialActivity()
        {
            Show(TextResources.MakeTaskFailureMsg, TextResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display message that there has no task.
        /// </summary>
        public static void CannotUnAssign()
        {
            Show(TextResources.NoTaskToUnassignMsg, TextResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display message that there has no task.
        /// </summary>
        public static void CannotUnAssignSpecialTask()
        {
            Show(TextResources.NoTaskCanUnassignMsg, TextResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// Display message that task cannot download.
        /// </summary>
        public static void CannotDownloadTask()
        {
            Show(TextResources.NetworkIssueOnTaskMsg, TextResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Prompt Only Merge Part Tasks
        /// </summary>
        public static void PromptOnlyMergePartTasks()
        {
            Show(TextResources.PartOfTasksAreMergedMsg, TextResources.Information, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Merge all tasks completed
        /// </summary>
        public static void MergeCompleted()
        {
            Show(TextResources.MergeCompletedMsg, TextResources.Information, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void NoMoreOccurrences() {
            Show(TextResources.NoMoreOccurrencesMsg, TextResources.Information, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #region Methods

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return ShowFunc(messageBoxText, caption, button, icon, MessageBoxResult.OK);

        }

        public static Func<string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult, MessageBoxResult> ShowFunc = ((msg, caption, button, icon, defaultResult) => MessageBox.Show(msg, caption, button, icon, defaultResult));

        #endregion




    }
}
