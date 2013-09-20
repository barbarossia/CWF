using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.DynamicImplementations;
using Microsoft.Win32;
using Microsoft.Support.Workflow.Authoring.Services;

namespace Microsoft.Support.Workflow.Authoring.Tests.Services
{
    [TestClass]
    public class DialogServiceUnitTest
    {
        [WorkItem(323736)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        public void DialogService_TestShowOpenFileDialogAndReturnResult()
        {
            Assert.IsInstanceOfType(DialogService.CreateOpenFileDialog(), typeof(OpenFileDialog));
            using (var dialog = new Implementation<OpenFileDialog>())
            {
                dialog.Register(i => i.ShowDialog()).Return(true);
                dialog.Register(i => i.FileName).Return("fileName");
                DialogService.CreateOpenFileDialog = () => dialog.Instance;

                Assert.AreEqual("fileName", DialogService.ShowOpenFileDialogAndReturnResult("All|*.*", null));
                Assert.AreEqual("All|*.*", dialog.Instance.Filter);
                Assert.AreEqual("Open", dialog.Instance.Title);
            }
        }

        [WorkItem(323736)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        public void DialogService_TestShowSaveDialogAndReturnResult()
        { 
            Assert.IsInstanceOfType(DialogService.CreateSaveFileDialog(), typeof(SaveFileDialog));
            using (var dialog = new Implementation<SaveFileDialog>())
            {
                dialog.Register(i => i.ShowDialog()).Return(true);
                dialog.Register(i => i.FileName).Return("fileName");
                DialogService.CreateSaveFileDialog = () => dialog.Instance;

                Assert.AreEqual("fileName", DialogService.ShowSaveDialogAndReturnResult("default", "All|*.*"));
                Assert.AreEqual("All|*.*", dialog.Instance.Filter);

                dialog.Register(i => i.ShowDialog()).Return(null);
                Assert.AreEqual(string.Empty, DialogService.Save());
                Assert.AreEqual("All files (*.*)|*.*", dialog.Instance.Filter);
            }
        }
    }
}
