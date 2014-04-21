using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using Microsoft.Support.Workflow.Authoring.Security;
using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

namespace Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor {
    /// <summary>
    /// Interaction logic for TaskActivityDesigner.xaml
    /// </summary>
    public partial class TaskActivityDesigner {
        private const string taskIdPropertyName = "TaskId";
        private const string aliasPropertyName = "Alias";
        private const string statusPropertyName = "Status";
        private const string displayNamePropertyName = "DisplayName";

        public string Alias { get; set; }

        public TaskActivityDesigner() {
            InitializeComponent();

            Loaded += TaskActivityDesigner_Loaded;
        }

        private void TaskActivityDesigner_Loaded(object sender, RoutedEventArgs e) {
            Alias = GetAlias();
            Guid taskId = GetTaskId();
            SetLoading();
            Task.Factory.StartNew(() => {
                List<Principal> users = AuthorizationService.GetAuthorizedPrincipals(Permission.SaveWorkflow, Env.Dev, Env.Test);
                Dispatcher.Invoke(new Action(() => {
                    userList.ItemsSource = users;
                    userList.SelectedItem = users.SingleOrDefault(p => p.SamAccountName == Alias);
                    userList.IsEnabled = Body.IsEnabled;
                    if (userList.SelectedValue == null) {
                        userList.Text = string.Empty;
                    }
                }));
            });
            if (!string.IsNullOrWhiteSpace(Alias)) {
                Task.Factory.StartNew(() => {
                    try {
                        var taskDC = TaskService.GetLastVersionTaskActivityDC(taskId);
                        Dispatcher.Invoke(new Action(() => {
                            SetStatus(taskDC.Status);
                            SetReadOnly();
                        }));
                    }
                    catch (CommunicationException) {
                        Dispatcher.Invoke(new Action(() => {
                            SetStatus(TaskActivityStatus.New);
                        }));
                    }
                });
            }
            else {
                SetStatus(TaskActivityStatus.New);
            }
        }

        private void SetLoading() {
            userList.IsEnabled = false;
            userList.Text = TextResources.Loading;
        }

        private void SetReadOnly() {
            userList.IsEnabled = Body.IsEnabled = false;
        }

        private Guid GetTaskId() {
            return (Guid)ModelItem.Properties[taskIdPropertyName].ComputedValue;
        }

        private string GetAlias() {
            return (string)ModelItem.Properties[aliasPropertyName].ComputedValue;
        }

        private TaskActivityStatus GetStatus() {
            return (TaskActivityStatus)ModelItem.Properties[statusPropertyName].ComputedValue;
        }

        private void SetStatus(TaskActivityStatus status) {
            this.ModelItem.Properties[statusPropertyName].SetValue(status);
        }

        private void userList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count > 0) {
                ModelItem.Properties[aliasPropertyName].SetValue(Alias);

                Principal selectedUser = e.AddedItems[0] as Principal;
                string taskName = (string)ModelItem.Properties[displayNamePropertyName].ComputedValue;
                if (taskName == typeof(TaskActivity).Name || (e.RemovedItems.Count > 0 && taskName == GetTaskName(e.RemovedItems[0] as Principal)))
                    ModelItem.Properties[displayNamePropertyName].SetValue(GetTaskName(selectedUser));
            }
        }

        private string GetTaskName(Principal user) {
            return string.Format("Task for {0}", user.DisplayName);
        }

        private void userList_KeyUp(object sender, KeyEventArgs e) {
            Principal principal = userList.SelectedItem as Principal;
            if (principal != null && principal.DisplayName == userList.Text) {
                ModelItem.Properties[aliasPropertyName].SetValue(Alias);
            }
            else {
                ModelItem.Properties[aliasPropertyName].SetValue(null);
            }
        }
    }
}
