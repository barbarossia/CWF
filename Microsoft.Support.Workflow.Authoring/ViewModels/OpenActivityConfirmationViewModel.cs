using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Support.Workflow.Authoring.Views;
using Microsoft.Practices.Prism.Commands;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    public class OpenActivityConfirmationViewModel : NotificationObject
    {
        /// <summary>
        /// Title of the message box
        /// </summary>
        private string title;
        private string message;
        private OpenActivityConfirmation view;

        /// <summary>
        /// Title of the message dialog
        /// </summary>
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        /// <summary>
        /// Message text of the dialog.
        /// </summary>
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        /// <summary>
        /// Command to handle save the action
        /// </summary>
        public DelegateCommand SaveClickedCommand { get; set; }

        /// <summary>
        /// Command to handle the not save action
        /// </summary>
        public DelegateCommand DontSaveClickedCommand { get; set; }

        /// <summary>
        /// Constructor with basic parameters
        /// </summary>
        /// <param name="dialogTitle">Title of the dialog</param>
        /// <param name="dialogMessage">Message text of the dialog</param>
        /// <param name="dialogUrl">Url that will be displayed as a clickable link</param>
        public OpenActivityConfirmationViewModel(OpenActivityConfirmation view, string dialogTitle, string dialogMessage)
            : this()
        {
            this.view = view;
            title = dialogTitle;
            message = dialogMessage;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public OpenActivityConfirmationViewModel()
        {
            SaveClickedCommand = new DelegateCommand(SaveClickedCommandExecute);
            DontSaveClickedCommand = new DelegateCommand(DontSaveClickedCommandExecute);
        }

        /// <summary>
        /// Save
        /// </summary>
        private void SaveClickedCommandExecute()
        {
            view.Result = MessageBoxResult.Yes;
            view.Close();
        }

        /// <summary>
        /// not save
        /// </summary>
        private void DontSaveClickedCommandExecute()
        {
            view.Result = MessageBoxResult.No;
            view.Close();
        }
    }
}
