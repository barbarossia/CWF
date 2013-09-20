using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using System.Activities.Presentation;
using System.Activities.Presentation.Validation;
using System.Activities.Validation;
using System.Threading;

namespace Microsoft.Support.Workflow.Authoring.Tests.AddIns
{
    [TestClass]
    public class ErrorServiceUnitTest
    {
        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void ErrorService_TestShowXamlLoadErrors()
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            WorkflowEditorViewModel vm = new WorkflowEditorViewModel(cancellationToken);
            vm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(vm_PropertyChanged);

            WorkflowEditorViewModel.ErrorService service = new WorkflowEditorViewModel.ErrorService(vm);
            vm.Errors = service;

            List<XamlLoadErrorInfo> infos = new List<XamlLoadErrorInfo>() 
            {
                new XamlLoadErrorInfo("error message",1,1)
            };

            service.ShowXamlLoadErrors(infos);
            Assert.IsTrue(isRaised);

            isRaised = false;
            List<ValidationErrorInfo> infoss = new List<ValidationErrorInfo>() 
            {
                new ValidationErrorInfo(new ValidationError("error message",true)),
                new ValidationErrorInfo(new ValidationError("error message",false))
            };
            service.ShowValidationErrors(infoss);
            Assert.IsTrue(isRaised);
            Assert.IsTrue(service.HasXamlLoadErrors);
            Assert.AreEqual(service.ErrorList.Count(),2);
        }

        bool isRaised = false;
        private void vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Errors")
            {
                isRaised = true;
            }
        }
    }
}
