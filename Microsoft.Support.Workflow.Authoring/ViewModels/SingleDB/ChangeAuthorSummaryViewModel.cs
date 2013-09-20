using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    public class ChangeAuthorSummaryViewModel : NotificationObject
    {
        private string projectName;
        private string originalAuthor;
        private string currentAuthor;

        public string ProjectName
        {
            get { return this.projectName; }
            set
            {
                this.projectName = value;
                RaisePropertyChanged(() => this.ProjectName);
            }
        }

        public string OriginalAuthor
        {
            get { return this.originalAuthor; }
            set
            {
                this.originalAuthor = value;
                RaisePropertyChanged(() => this.OriginalAuthor);
            }
        }

        public string CurrentAuthor
        {
            get { return this.currentAuthor; }
            set
            {
                this.currentAuthor = value;
                RaisePropertyChanged(() => this.CurrentAuthor);
            }
        }

        public ChangeAuthorSummaryViewModel(string name, string oriAuthor, string curAuthor)
        {
            this.ProjectName = name;
            this.OriginalAuthor = oriAuthor;
            this.CurrentAuthor = curAuthor;
        }
    }
}
