using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Practices.Prism.ViewModel;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    public class AboutViewModel : NotificationObject
    {

        public string Version 
        { 
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
    }
}
