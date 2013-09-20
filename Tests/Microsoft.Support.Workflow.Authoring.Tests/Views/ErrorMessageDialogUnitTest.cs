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
using System.Activities.Statements;
using System.Windows.Media;
using System.Windows;


namespace Microsoft.Support.Workflow.Authoring.Tests.Views
{
    [TestClass]
    public class ErrorMessageDialogUnitTest
    {
        [WorkItem(325747)]
        [Owner("v-kason1")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Views_VerifyErrorMessageDialog()
        {
            var window = new Window();
            using (var dialog = new Implementation<ErrorMessageDialog>())
            {
                bool isshow = false;
                dialog.Register(inst => inst.ShowDialog()).Execute(() =>
                {
                    bool? result = true;
                    isshow = true;
                    return result;
                });

                using (var view = new ImplementationOfType(typeof(ErrorMessageDialog)))
                {
                    ErrorMessageDialog.GetErrorMessageDialog("", "", null);
                    view.Register(() => ErrorMessageDialog.GetErrorMessageDialog(Argument<string>.Any, Argument<string>.Any, Argument<Window>.Any)).Return(dialog.Instance);

                    ErrorMessageDialog.Show("","", null);
                    Assert.IsTrue(isshow);
                }
            }
        }
    }
}
