using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.Common
{
    public class EnvFilter : NotificationObject
    {
        private Env env;
        public Env Env
        {
            get { return this.env; }
            set
            {
                this.env = value;
                RaisePropertyChanged(() => this.Env);
            }
        }

        private bool isFilted;
        public bool IsFilted
        {
            get { return this.isFilted; }
            set
            {
                this.isFilted = value;
                RaisePropertyChanged(() => this.IsFilted);
            }
        }
    }
}
