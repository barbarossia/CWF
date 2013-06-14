using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DataContracts;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor {
    public class TaskItem : NotificationObject {
        private ModelItem taskModelItem;
        private DelegateCommand locateCommand;

        public string Name {
            get {
                return (string)taskModelItem.Properties["DisplayName"].ComputedValue;
            }
            set {
                taskModelItem.Properties["DisplayName"].SetValue(value);
            }
        }
        public TaskActivityStatus Status {
            get {
                return (TaskActivityStatus)taskModelItem.Properties["Status"].ComputedValue;
            }
        }
        public string AssignedTo {
            get {
                return (string)taskModelItem.Properties["Alias"].ComputedValue;
            }
        }
        public DelegateCommand LocateCommand {
            get {
                if (locateCommand == null)
                    locateCommand = new DelegateCommand(() => {
                        taskModelItem.Focus();
                    });
                return locateCommand;
            }
        }

        public TaskItem(ModelItem taskModelItem) {
            this.taskModelItem = taskModelItem;
        }
    }
}
