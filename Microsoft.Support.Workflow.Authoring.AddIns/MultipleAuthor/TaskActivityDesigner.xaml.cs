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

namespace Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor
{
    /// <summary>
    /// Interaction logic for TaskActivityDesigner.xaml
    /// </summary>
    public partial class TaskActivityDesigner
    {
        public TaskActivityDesigner()
        {
            InitializeComponent();
            Loaded += TaskActivityDesigner_Loaded;

            group.TextChanged += group_TextChanged;
        }

        private void TaskActivityDesigner_Loaded(object sender, RoutedEventArgs e)
        {
            Guid taskId = GetTaskId();
            if (!string.IsNullOrWhiteSpace(GetAlias()))
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var taskDC = TaskService.GetLastVersionTaskActivityDC(taskId);
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            SetStatus(taskDC.Status);
                            SetReadOnly();
                        }));
                    }
                    catch (CommunicationException)
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            SetStatus(TaskActivityStatus.New);
                        }));
                    }
                });
            }
            else
            {
                SetStatus(TaskActivityStatus.New);
            }
        }

        private void group_TextChanged(object sender, TextChangedEventArgs e)
        {
            group.TextChanged -= group_TextChanged;
            group_LostFocus(sender, null);
        }

        private void group_LostFocus(object sender, RoutedEventArgs e)
        {
            SetLoading();
            string groupName = ((TextBox)sender).Text;
            IEnumerable<Principal> users = new List<Principal>();
            if (!string.IsNullOrWhiteSpace(groupName))
            {
                Task.Factory.StartNew(() =>
                {
                    users = PrincipalService.ListGroupsUsers(groupName);
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this.userList.ItemsSource = users;
                        this.userList.IsEnabled = this.group.IsEnabled;
                        if (userList.SelectedValue == null)
                        {
                            userList.Text = string.Empty;
                        }
                    }));
                });
            }
        }

        private void SetLoading()
        {
            userList.IsEnabled = false;
            userList.Text = "Loading...";
        }

        private void SetReadOnly()
        {
            this.group.IsEnabled = false;
            this.userList.IsEnabled = false;
            this.Body.IsEnabled = false;
        }

        private Guid GetTaskId()
        {
            return (Guid)this.ModelItem.Properties["TaskId"].ComputedValue;
        }

        private string GetAlias()
        {
            return (string)this.ModelItem.Properties["Alias"].ComputedValue;
        }

        private TaskActivityStatus GetStatus()
        {
            return (TaskActivityStatus)this.ModelItem.Properties["Status"].ComputedValue;
        }

        private void SetStatus(TaskActivityStatus status)
        {
            this.ModelItem.Properties["Status"].SetValue(status);
        }

        private void userList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count > 0) {
                Principal selectedUser = e.AddedItems[0] as Principal;
                string taskName = (string)this.ModelItem.Properties["DisplayName"].ComputedValue;
                if (taskName == typeof(TaskActivity).Name || (e.RemovedItems.Count > 0 && taskName == GetTaskName(e.RemovedItems[0] as Principal)))
                    this.ModelItem.Properties["DisplayName"].SetValue(GetTaskName(selectedUser));
            }
        }

        private string GetTaskName(Principal user) {
            return string.Format("Task for {0}", user.DisplayName);
        }
    }
}
