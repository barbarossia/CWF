using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Support.Workflow.Authoring.Views;
using System.Windows;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.AddIns;
using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    public class SavingConfirmationViewModel : NotificationObject
    {
        /// <summary>
        /// Title of the message box
        /// </summary>
        private string title;
        private string message;
        private bool canKeepLocked;
        private bool shouldUnlock = true;
        private Visibility unlockVisibility = Visibility.Visible;
        private SavingConfirmation view;
        private string saveButtonContent = TextResources.Save;
        private string dontSaveButtonContent = TextResources.DontSave;

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
        /// Indicates unlock option can be unchecked or not
        /// </summary>
        public bool CanKeepLocked
        {
            get
            {
                return canKeepLocked;
            }
            private set
            {
                canKeepLocked = value;
                RaisePropertyChanged(() => CanKeepLocked);
            }
        }

        /// <summary>
        /// Indicates the workflow should be unlocked or not
        /// </summary>
        public bool ShouldUnlock
        {
            get
            {
                return shouldUnlock;
            }
            set
            {
                shouldUnlock = value;
                RaisePropertyChanged(() => ShouldUnlock);
            }
        }

        public Visibility UnlockVisibility
        {
            get
            {
                return unlockVisibility;
            }
            set
            {
                unlockVisibility = value;
                RaisePropertyChanged(() => UnlockVisibility);
            }
        }

        public string SaveButtonContent
        {
            get
            {
                return saveButtonContent;
            }
            set
            {
                saveButtonContent = value;
                RaisePropertyChanged(() => SaveButtonContent);
            }
        }

        public string DontSaveButtonContent
        {
            get
            {
                return dontSaveButtonContent;
            }
            set
            {
                dontSaveButtonContent = value;
                RaisePropertyChanged(() => DontSaveButtonContent);
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
        public SavingConfirmationViewModel(SavingConfirmation view, string dialogTitle, string dialogMessage, bool canKeepLocked, bool shouldUnlock, bool isUnlockVisible)
            : this()
        {
            this.view = view;
            title = dialogTitle;
            message = dialogMessage;
            CanKeepLocked = canKeepLocked;
            ShouldUnlock = shouldUnlock;
            UnlockVisibility = isUnlockVisible ? Visibility.Visible : Visibility.Collapsed;
            if (shouldUnlock && !isUnlockVisible)
            {
                SaveButtonContent = TextResources.Yes;
                DontSaveButtonContent = TextResources.No;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SavingConfirmationViewModel()
        {
            SaveClickedCommand = new DelegateCommand(SaveClickedCommandExecute);
            DontSaveClickedCommand = new DelegateCommand(DontSaveClickedCommandExecute);
        }

        /// <summary>
        /// save
        /// </summary>
        private void SaveClickedCommandExecute()
        {
            if (!CanKeepLocked && shouldUnlock && UnlockVisibility == Visibility.Collapsed)
            {
                view.Result = SavingResult.DoNothing;
            }
            else
            {
                view.Result = SavingResult.Save;
            }
            if (ShouldUnlock && CanKeepLocked)
            {
                view.Result |= SavingResult.Unlock;
            }
            view.Close();
        }

        /// <summary>
        /// do not save
        /// </summary>
        private void DontSaveClickedCommandExecute()
        {
            if (!CanKeepLocked && shouldUnlock && UnlockVisibility == Visibility.Collapsed)
            {
                view.Result = SavingResult.Unlock;
            }
            else
            {
                view.Result = SavingResult.DoNothing;
            }
            if (ShouldUnlock)
            {
                view.Result |= SavingResult.Unlock;
            }
            view.Close();
        }

    }
}
