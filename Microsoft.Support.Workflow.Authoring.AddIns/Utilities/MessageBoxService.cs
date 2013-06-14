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

    /// <summary>
    /// The message box service.
    /// </summary>
    public class AddInMessageBoxService
    {
        #region Constants

        private const string MSG_PRINT_CONFIRMATION = "You are going to print thses activities on [ {0} ] pages of [ {1} ] paper with [ {2} ]. Are you sure?";
        private const string MSG_PRINT_ERROR = "Print failed due to {0}.";
        private const string MSG_CANNOT_ASSIGN = "This Activity or it's parents have been assigned.";
        private const string MSG_CANNOT_GETLASTVERSION = "This Activity cannot get last version.";

        #endregion Constants

        /// <summary>
        /// Confirmation for printing activities.
        /// </summary>
        /// <param name="pageCount"></param>
        /// <param name="paperName"></param>
        /// <param name="printerName"></param>
        /// <returns></returns>
        public static bool PrintConfirmation(int pageCount, string paperName, string printerName)
        {
            return Show(string.Format(MSG_PRINT_CONFIRMATION, pageCount, paperName, printerName), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Cancel print and back to main window
        /// </summary>
        /// <returns></returns>
        public static bool PrintReselectConfirmation()
        {
            return Show("Are you sure to discard your changes?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Confirmation for select nothing.
        /// </summary>
        public static void PrintNoneSelectMessage()
        {
            Show("You have not selected any activity, please choose again.", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Confirmation cannot print workflow that nested level over 25 
        /// </summary>
        /// <param name="name"></param>
        public static void PrintOverflowWorkflow(string name)
        {
            Show(string.Format("Sorry, can not print this over nested workflow {0}. Nested level should be less than 25.", name), "Confirmation", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Confirmation for select nothing.
        /// </summary>
        public static void PrintNoneActivityMessage()
        {
            Show("There is no activity to be printed!", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Display error message when printing failed.
        /// </summary>
        /// <param name="message"></param>
        public static void PrintFailed(string message)
        {
            Show(string.Format(MSG_PRINT_ERROR, message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display error message when user want to assign activity which has been assigned.
        /// </summary>
        public static void CannotAssignTaskActivity()
        {
            Show(MSG_CANNOT_ASSIGN, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display error message when usre want to get last version activity.
        /// </summary
        public static void CannotGetLastVersion()
        {
            Show(MSG_CANNOT_GETLASTVERSION, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display warning when user want to merge.
        /// </summary
        public static MessageBoxResult CannotMergeNextTime()
        {
            return Show("If you unassign, you will not be able to merge this task later. Are you sure?", "Information", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Display message where no task to be merge.
        /// </summary>
        public static void CannotMergeAll()
        {
            Show("There is no task to be merged", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display message where no task to be merge.
        /// </summary>
        public static void CannotMergeSpecialTask()
        {
            Show("There is no task to meet the conditions to be merged.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display message that task activity cannot assign in another task.
        /// </summary>
        public static void CannotAssign()
        {
            Show("Cannot add a task in another task.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display message that task activity cannot be assigned.
        /// </summary>
        public static void CannotAssignUseSpecialActivity()
        {
            Show("Cannot make selected activity as a task.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display message that there has no task.
        /// </summary>
        public static void CannotUnAssign()
        {
            Show("There has no task to be unassigned.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Display message that there has no task.
        /// </summary>
        public static void CannotUnAssignSpecialTask()
        {
            Show("There has no task to meet the conditions to be unassigned.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// Display message that task cannot download.
        /// </summary>
        public static void CannotDownloadTask()
        {
            Show("Network issues have interrupted your downloads from servrt.  Please contact your network administrator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Prompt Only Merge Part Tasks
        /// </summary>
        public static void PromptOnlyMergePartTasks()
        {
            Show("Only tasks in 'Checked In' state can be merged, so this operation did not affect others.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Merge all tasks completed
        /// </summary>
        public static void MergeCompleted()
        {
            Show("Merge Completed.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
