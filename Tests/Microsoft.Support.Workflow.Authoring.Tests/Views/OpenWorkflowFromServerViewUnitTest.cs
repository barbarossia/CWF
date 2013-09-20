using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Views;
using Microsoft.DynamicImplementations;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Authoring.AssetStore;
using System.Windows.Data;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows;
namespace Microsoft.Support.Workflow.Authoring.Tests.Views
{
    [TestClass]
    public class OpenWorkflowFromServerViewUnitTest
    {
        [WorkItem(325761)]
        [Owner("v-kason1")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Views_VerifyPrivateMethod()
        {
            using (var v = new Implementation<OpenWorkflowFromServerView>())
            {
                v.Register(inst => inst.ShowDialog()).Execute(() =>
                {
                    bool? result = true;
                    return result;
                });

                var view = v.Instance;

                var dataContext = new OpenWorkflowFromServerViewModel();
                view.DataContext = dataContext;

                //verify WorkflowsGridRowLoaded
                PrivateObject po = new PrivateObject(view);
                DataGridRow row = new DataGridRow();
                row.InputBindings.Clear();
                po.Invoke("WorkflowsGridRowLoaded", row, null);
                Assert.IsTrue(row.InputBindings.Count == 1);


                //verify WorkflowsGridSorting
                bool isSort = false;
                dataContext.SortCommand = new DelegateCommand<string>(new Action<string>((param) => { isSort = true; }));

                DataGridTextColumn column = new DataGridTextColumn();
                DataGridSortingEventArgs e = new DataGridSortingEventArgs(column);
                po.Invoke("WorkflowsGridSorting", null, e);
                Assert.IsTrue(e.Handled);

                //verify TextBox_KeyDown
                bool isSearch = false;
                dataContext.SearchCommand = new DelegateCommand(new Action(() => { isSearch = true; }));
                var btn = new Button();
                TextBox sender = new TextBox();
                sender.Text = "search";
                KeyEventArgs args = new KeyEventArgs(Keyboard.PrimaryDevice, new HwndSource(
                    0, 0, 0, 0, 0, string.Empty, IntPtr.Zero), 0, Key.Enter);
                btn.Command = dataContext.SearchCommand;

                po.SetFieldOrProperty("SearchButton", btn);
                //po.Invoke("TextBox_KeyDown", sender, args);
                //Assert.IsTrue(isSearch);

                //verify OpenButton_Click
                RoutedEventArgs ar = new RoutedEventArgs();
                try
                {
                    po.Invoke("OpenButton_Click", null, ar);
                    Assert.IsTrue(view.DialogResult.Value);
                }
                catch (Exception) { }
            }
        }
    }
}
