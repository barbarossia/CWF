using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.DynamicImplementations;
using Microsoft.Win32;

namespace Microsoft.Support.Workflow.Authoring.Tests.AddIns
{
    [TestClass]
    public class AddInDialogServiceUnitTest
    {
       
        [TestMethod]
        [Owner("v-kason")]
        [TestCategory("UnitTest")]
        public void AddInDialogService_TestShowSaveDialogAndReturnResult()
        {
            Assert.IsInstanceOfType(AddInDialogService.CreateSaveFileDialog(), typeof(SaveFileDialog));
            using (var dialog = new Implementation<SaveFileDialog>())
            {
                dialog.Register(i => i.ShowDialog()).Return(true);
                dialog.Register(i => i.FileName).Return("fileName");
                AddInDialogService.CreateSaveFileDialog = () => dialog.Instance;

                Assert.AreEqual("fileName", AddInDialogService.ShowSaveDialogAndReturnResult("default", "All|*.*"));
                Assert.AreEqual("All|*.*", dialog.Instance.Filter);

                dialog.Register(i => i.ShowDialog()).Return(null);
                Assert.AreEqual("All|*.*", dialog.Instance.Filter);
            }
        }
    }
}
