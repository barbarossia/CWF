// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogService.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.AddIns.Utilities
{

    using System;
    using System.Windows;
    using Win32;

    public static class AddInDialogService
    {
        /// <summary>
        /// Extracted fucntionality from SaveFileDialog.
        /// This Func is rewritable so we can suppress dialogs during automation and gain access to the viewModel object.       
        /// </summary>
        public static Func<SaveFileDialog> CreateSaveFileDialog = () => new SaveFileDialog();

        /// <summary>
        /// Show save file dialog.
        /// </summary>
        /// <param name="defaultFileName">The default file name for the dialog</param>
        /// <param name="filter">
        /// The filter for save dialog.
        /// </param>
        /// <returns>
        /// The selected file name.
        /// </returns>
        public static string ShowSaveDialogAndReturnResult(string defaultFileName, string filter)
        {
            string fileName = string.Empty;
            var saveFileDialog = CreateSaveFileDialog();
            saveFileDialog.FileName = defaultFileName;
            saveFileDialog.Filter = filter;
            bool result = saveFileDialog.ShowDialog() ?? false;

            if (result)
            {
                fileName = saveFileDialog.FileName;
            }

            return fileName;
        }

    }
}
