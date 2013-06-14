using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Practices.Prism.ViewModel;

namespace Microsoft.Support.Workflow.Authoring.AddIns.ViewModels
{
    public class ToolboxViewModel : NotificationObject
    {
        private ObservableCollection<ActivityAssemblyItem> items;
        private bool isTask;
        public ObservableCollection<ActivityAssemblyItem> ActivityAssemblyItems
        {
            get { return this.items; }
            set
            {
                this.items = value;
                RaisePropertyChanged(() => ActivityAssemblyItems);
            }
        }
        public bool IsTask
        {
            get { return this.isTask; }
            set
            {
                this.isTask = value;
                RaisePropertyChanged(() => IsTask);
            }
        }

        public ToolboxViewModel(bool isTask)
        {
            ActivityAssemblyItems = new ObservableCollection<ActivityAssemblyItem>(AddInCaching.ActivityAssemblyItems);
            IsTask = isTask;
        }
    }
}
