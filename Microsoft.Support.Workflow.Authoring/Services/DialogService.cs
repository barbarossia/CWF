// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogService.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Services
{

    using System;
    using System.Windows;
    using Win32;
    using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

    public static class DialogService
    {
        public static bool? ShowDialog(object viewModel)
        {
            return ShowDialogFunc(viewModel);
        }

        /// <summary>
        /// Extracted fucntionality from ShowDialog.
        /// This Func is rewritable so we can suppress dialogs during automation and gain access to the viewModel object.       
        /// </summary>
        public static Func<object, bool?> ShowDialogFunc = viewModel =>
        {
            Type viewType = ViewViewModelMappings.GetViewTypeFromViewModelType(viewModel.GetType());

            // Create new view and set the datacontext to the corresponding viewmodel
            var dialog = (Window)Activator.CreateInstance(viewType);
            dialog.Owner = Application.Current.MainWindow;
            dialog.DataContext = viewModel;

            // Show dialog
            return dialog.ShowDialog();
        };

        /// <summary>
        /// Extracted fucntionality from OpenFileDialog.
        /// This Func is rewritable so we can suppress dialogs during automation and gain access to the viewModel object.       
        /// </summary>
        public static Func<OpenFileDialog> CreateOpenFileDialog = () => new OpenFileDialog();

        /// <summary>
        /// Extracted fucntionality from SaveFileDialog.
        /// This Func is rewritable so we can suppress dialogs during automation and gain access to the viewModel object.       
        /// </summary>
        public static Func<SaveFileDialog> CreateSaveFileDialog = () => new SaveFileDialog();

        /// <summary>
        /// Show open file dialog with title
        /// </summary>
        /// <param name="filter">filter for opening file</param>
        /// <param name="dialogTitle">title of dialog</param>
        /// <returns>full name of opened file</returns>
        public static string ShowOpenFileDialogAndReturnResult(string filter, string dialogTitle)
        {
            string fileName = string.Empty;
            var openFileDialog = CreateOpenFileDialogAndReturnResult(filter, dialogTitle, true);
            
            bool result = openFileDialog.ShowDialog().GetValueOrDefault();

            if (result)
            {
                fileName = openFileDialog.FileName;
            }

            return fileName;
        }

        /// <summary>
        /// Show open file dialog with title
        /// </summary>
        /// <param name="filter">filter for opening file</param>
        /// <param name="dialogTitle">title of dialog</param>
        /// <returns>full name of opened file</returns>
        public static string[] ShowOpenFileDialogAndReturnMultiResult(string filter, string dialogTitle)
        {
            string[] fileNames = null;

            var openFileDialog = CreateOpenFileDialogAndReturnResult(filter, dialogTitle, true);

            bool result = openFileDialog.ShowDialog().GetValueOrDefault();

            if (result)
            {
                fileNames = openFileDialog.FileNames;
            }

            return fileNames;
        }

        private static OpenFileDialog CreateOpenFileDialogAndReturnResult(string filter, string dialogTitle, bool isMultiselect)
        {
            var openFileDialog = CreateOpenFileDialog();
            openFileDialog.Filter = filter;
            openFileDialog.Multiselect = isMultiselect;

            if (string.IsNullOrEmpty(dialogTitle))
            {
                dialogTitle = TextResources.Open;
            }

            openFileDialog.Title = dialogTitle;

            return openFileDialog;
        }

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

        /// <summary>
        /// Show save file dialog with 'All files' filter.
        /// </summary>
        /// <returns>
        /// The selected file name.
        /// </returns>
        public static string Save()
        {
            return ShowSaveDialogAndReturnResult(string.Empty, TextResources.AllFilesFilter);
        }

        
    }
}
