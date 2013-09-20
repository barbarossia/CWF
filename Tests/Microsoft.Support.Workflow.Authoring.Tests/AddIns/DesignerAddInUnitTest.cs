using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.AddIns;
using System.Activities.Statements;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.DynamicImplementations;
using System.Activities.Presentation;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Activities.Presentation.View;
using System.Windows;
using System.Windows.Media;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.Behaviors;

namespace Microsoft.Support.Workflow.Authoring.Tests.AddIns
{
    [TestClass]
    public class DesignerAddInUnitTest
    {
        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        [Ignore]
        public void aaaDesignerAddIn_aaInitWorkflowTest()
        {
            string name = "MyWf";
            string xaml = (new Sequence()).ToXaml();
            DesignerAddIn addin = new DesignerAddIn();
            addin.InitWorkflow(name, xaml, false);

            Assert.AreEqual(addin.DependencyAssemblies.Count(), 0);
            Assert.IsNotNull(addin.CompileProject);
            Assert.IsNotNull(addin.CompiledXaml);
            Assert.IsNotNull(addin.Xaml);
            Assert.IsFalse(addin.IsWorkflowService());
            Assert.IsTrue(addin.IsWorkflowValid());
            Assert.IsNotNull(addin.PrintState);
            Assert.IsNull(addin.InitializeLifetimeService());

            //Assert.IsNotNull(addin.ToolboxView);
           // Assert.IsNotNull(addin.ProjectExplorerView);
            //Assert.IsNotNull(addin.WorkflowEditorView);
            Assert.IsNotNull(addin.WorkflowPropertyView);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void aaDesignerAddIn_TestPrint()
        {
            string name = "MyWf";
            string xaml = (new Sequence()).ToXaml();
            DesignerAddIn addin = new DesignerAddIn();
            addin.InitWorkflow(name, xaml, false);

            PrivateObject po = new PrivateObject(addin);
            WorkflowEditorViewModel vm = po.GetFieldOrProperty("WorkflowEditorVM") as WorkflowEditorViewModel;
            addin.Print();
            Assert.AreEqual(vm.ShouldBePrint, PrintAction.PrintUserSelection);
            addin.PrintAll();
            Assert.AreEqual(vm.ShouldBePrint, PrintAction.PrintAll);
        }


        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void aaDesignerAddIn_RaiseEventTest()
        {
            DesignerAddIn addin = new DesignerAddIn();
            addin.DesignerChanged += new EventHandler(addin_DesignerChanged);
            addin.PrintStateChanged += new EventHandler(addin_PrintStateChanged);
            Assert.IsFalse(isPrintStateChangedRaised);
            Assert.IsFalse(isDesignerChangedRaised);
            addin.WorkflowDesignerChanged(null, null);
            addin.OnPrintStateChanged(null, null);
            Assert.IsTrue(isPrintStateChangedRaised);
            Assert.IsTrue(isDesignerChangedRaised);
        }

        bool isPrintStateChangedRaised;
        void addin_PrintStateChanged(object sender, EventArgs e)
        {
            isPrintStateChangedRaised = true;
        }

        bool isDesignerChangedRaised;
        private void addin_DesignerChanged(object sender, EventArgs e)
        {
            isDesignerChangedRaised = true;
        }

        [TestMethod]
        [Owner("v-kason")]
        [TestCategory("UnitTest")]
        public void aaDesignerAddIn_TestSetWorkflowNameAndReadOnly()
        {
            string name = "MyWf";
            string newName = "NewWf";
            string xaml = (new Sequence()).ToXaml();
            DesignerAddIn addin = new DesignerAddIn();
            addin.InitWorkflow(name, xaml, false);

            addin.SetWorkflowName(newName);

            PrivateObject po = new PrivateObject(addin);
            WorkflowEditorViewModel vm = po.GetFieldOrProperty("WorkflowEditorVM") as WorkflowEditorViewModel;
            Assert.IsNotNull(vm);
            Assert.AreEqual(vm.Name, newName);
            Assert.AreEqual(vm.FullName, newName);

            addin.SetReadOnly(true);
            Assert.IsTrue(vm.IsReadOnly);

        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void aaDesignerAddIn_TestUndoRedo()
        {
            string name = "MyWf";
            string newName = "NewWf";
            string xaml = (new Sequence()).ToXaml();
            DesignerAddIn addin = new DesignerAddIn();
            addin.InitWorkflow(name, xaml, false);
            addin.SetWorkflowName(newName);

            Assert.IsTrue(addin.CanUndo());
            addin.Undo();
            Assert.IsTrue(addin.CanRedo());
            addin.Redo();
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void aaDesignerAddIn_TestCutCopyPaste()
        {
            using (var helper = new ImplementationOfType(typeof(CutCopyPasteHelper)))
            {
                helper.Register(() => CutCopyPasteHelper.CanCut(Argument<EditingContext>.Any)).Return(true);
                bool isCut = false;
                helper.Register(() => CutCopyPasteHelper.DoCut(Argument<EditingContext>.Any)).Execute(() => { isCut = true; });

                helper.Register(() => CutCopyPasteHelper.CanCopy(Argument<EditingContext>.Any)).Return(true);
                bool isCopy = false;
                helper.Register(() => CutCopyPasteHelper.DoCopy(Argument<EditingContext>.Any)).Execute(() => { isCopy = true; });

                helper.Register(() => CutCopyPasteHelper.CanPaste(Argument<EditingContext>.Any)).Return(true);
                bool isPaste = false;
                helper.Register(() => CutCopyPasteHelper.DoPaste(Argument<EditingContext>.Any)).Execute(() => { isPaste = true; });

                string name = "MyWf";
                string xaml = (new Sequence()).ToXaml();
                DesignerAddIn addin = new DesignerAddIn();
                addin.InitWorkflow(name, xaml, false);

                Assert.IsTrue(addin.CanCut());
                addin.Cut();

                Assert.IsTrue(addin.CanCopy());
                addin.Copy();

                Assert.IsTrue(addin.CanPaste());
                addin.Paste();

                Assert.IsTrue(isCut);
                Assert.IsTrue(isCopy);
                Assert.IsTrue(isPaste);
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void aaDesignerAddIn_TestSaveWorkflow()
        {
            string name = "MyWf";
            string xaml = (new Sequence()).ToXaml();

            using (var addInMock = new Implementation<DesignerAddIn>())
            {
                addInMock.Instance.InitWorkflow(name, xaml, false);
                addInMock.Instance.GetRoutedCommand();
                PrivateObject po = new PrivateObject(addInMock.Instance);
                WorkflowEditorViewModel vm = po.GetFieldOrProperty("WorkflowEditorVM") as WorkflowEditorViewModel;

                using (var command = new Implementation<RoutedCommand>())
                {
                    bool isExecute = false;
                    DesignerView view = vm.WorkflowDesigner.Context.Services.GetService<DesignerView>();
                    command.Register(inst => inst.Execute(Argument<object>.Any, view))
                        .Execute(() => { isExecute = true; });

                    addInMock.Register(inst => inst.GetRoutedCommand()).Return(command.Instance);

                    var addin = addInMock.Instance;
                    using (var helper = new ImplementationOfType(typeof(VisualTreeHelper)))
                    {
                        helper.Register(() => VisualTreeHelper.GetDescendantBounds(Argument<Visual>.Any)).Return(new Rect(1, 1, 1, 1));
                        using (var bitmap = new ImplementationOfType(typeof(BitmapFrame)))
                        {
                            bool isCreated = false;
                            bitmap.Register(() => BitmapFrame.Create(Argument<BitmapSource>.Any))
                                .Execute(() =>
                                {
                                    isCreated = true;
                                    return BitmapFrame.Create(new Uri("http://www.test.com"));
                                });

                            using (var file = new ImplementationOfType(typeof(FileService)))
                            {
                                bool isSaved = false;
                                file.Register(() => FileService.SaveImageToDisk(Argument<string>.Any, Argument<BitmapSource>.Any)).Execute(() =>
                                {
                                    isSaved = true;
                                    return isSaved;
                                });

                                //save to Bitmap no targetfile
                                using (var addInDialog = new ImplementationOfType(typeof(AddInDialogService)))
                                {
                                    addInDialog.Register(() => AddInDialogService.ShowSaveDialogAndReturnResult(Argument<string>.Any, Argument<string>.Any))
                                        .Return("filename");
                                    addin.SaveWorkflowToBitmap();
                                    Assert.IsTrue(isSaved);
                                }

                                //save to Bitmap with targetfile
                                isSaved = false;
                                addin.SaveWorkflowToBitmap("filename");
                                Assert.IsTrue(isSaved);

                                using (var designer = new Implementation<WorkflowDesigner>())
                                {
                                    isSaved = false;
                                    designer.Register(inst => inst.Save(Argument<string>.Any)).Execute(() => { isSaved = true; });
                                    addin.WorkflowEditorVM.WorkflowDesigner = designer.Instance;
                                    addin.Save("file");

                                    Assert.IsTrue(isSaved);
                                }
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void aaDesignerAddIn_TestImportAssemblies()
        {
            string name = "MyWf";
            string xaml = (new Sequence()).ToXaml();
            DesignerAddIn addin = new DesignerAddIn();
            addin.InitWorkflow(name, xaml, false);

            List<ActivityAssemblyItem> items = new List<ActivityAssemblyItem>();
            //test failed import
            using (var addinCache = new ImplementationOfType(typeof(AddInCaching)))
            {
                addinCache.Register(() => AddInCaching.ImportAssemblies(items)).Return(false);
                Assert.IsFalse(addin.ImportAssemblies(items));

                addinCache.Register(() => AddInCaching.ImportAssemblies(items)).Return(true);
                Assert.IsTrue(addin.ImportAssemblies(items));
            }
            addin.RefreshDesignerFromXamlCode();
        }
    }
}
