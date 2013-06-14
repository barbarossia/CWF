namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    using System.Diagnostics;
    using Practices.Prism.Commands;
    using Practices.Prism.ViewModel;
    
    /// <summary>
    /// Displays a message dialog that contains a Url that can be clicked to open the user's default browser.
    /// </summary>
    public class ClickableMessageViewModel : NotificationObject
    {
        /// <summary>
        /// Title of the message box
        /// </summary>
        private string title;
        private string url;
        private string message;

        /// <summary>
        /// Title of the message dialog
        /// </summary>
        public string Title { 
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
        /// Url of the message dialog
        /// </summary>
        public string Url { 
            get
            {
                return url;
            }
            set
            {
                url = value;
                RaisePropertyChanged(() => Url);
            }
        }

        /// <summary>
        /// Message text of the dialog.
        /// </summary>
        public string Message { 
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
        /// Command to handle the action to open the url in the default browser.
        /// </summary>
        public DelegateCommand UrlClickedCommand { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ClickableMessageViewModel()
        {
            UrlClickedCommand = new DelegateCommand(UrlClickedCommandExecute);
        }


        /// <summary>
        /// Constructor with basic parameters
        /// </summary>
        /// <param name="dialogTitle">Title of the dialog</param>
        /// <param name="dialogMessage">Message text of the dialog</param>
        /// <param name="dialogUrl">Url that will be displayed as a clickable link</param>
        public ClickableMessageViewModel(string dialogTitle, string dialogMessage, string dialogUrl) : this()
        {
            title = dialogTitle;
            message = dialogMessage;
            url = dialogUrl;           
        }

        /// <summary>
        /// Starts a new process for the default browser to navigate to the link of the dialog.
        /// </summary>
        private void UrlClickedCommandExecute()
        {
            if (!string.IsNullOrEmpty(Url))
            {
                Process.Start(Url);           
            }
        }
    }
}
