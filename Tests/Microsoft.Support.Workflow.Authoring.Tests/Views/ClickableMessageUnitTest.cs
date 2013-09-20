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


namespace Microsoft.Support.Workflow.Authoring.Tests.Views
{
    [TestClass]
    public class ClickableMessageUnitTest
    {
        [WorkItem(325769)]
        [Owner("v-kason1")]
        [TestCategory("Unit-Dif")]
        [TestMethod]
        public void Views_VerifyShowAsDialog()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
           {

               using (var window = new Implementation<ClickableMessage>())
               {
                   bool isShow = false;
                   window.Register(inst => inst.ShowDialog()).Execute(() =>
                   {
                       bool? result = true;
                       isShow = true;
                       return result;
                   });

                   using (var mesage = new ImplementationOfType(typeof(ClickableMessage)))
                   {
                       ClickableMessage.GetClickableMessage();
                       mesage.Register(() => ClickableMessage.GetClickableMessage()).Return(window.Instance);

                       ClickableMessage.ShowAsDialog("message", "caption", "url");
                       Assert.IsTrue(isShow);
                   }
               }
           });
        }
    }
}
