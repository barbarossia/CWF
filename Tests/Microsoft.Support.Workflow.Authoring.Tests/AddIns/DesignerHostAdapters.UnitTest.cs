using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using System.AddIn.Pipeline;
using System.Windows;
using Microsoft.Support.Workflow.Authoring.Behaviors;

namespace Microsoft.Support.Workflow.Authoring.Tests.AddIns
{
    /// <summary>
    /// Summary description for DesignerHostAdapters
    /// </summary>
    [TestClass]
    public class DesignerHostAdaptersUnitTest
    {
        private string name = "MyWorkflow";
        private string xaml = TestUtilities.GetEmptyWorkFlowTemplateXamlCode();
        private List<ActivityAssemblyItem> imports = new List<ActivityAssemblyItem>
                {
                    TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1,
                    TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib2,
                    TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib3
                };

        private Implementation<DesignerAddIn> addin;
        private ImplementationOfType loader;
        private DesignerHostAdapters host;
        private Implementation<AddinPreloader> inst;

        private void Init(Action action)
        {
            using (addin = new Implementation<DesignerAddIn>())
            {
                using (inst = new Implementation<AddinPreloader>())
                {
                    using (loader = new ImplementationOfType(typeof(AddinPreloader)))
                    {
                        bool isInit = false;
                        inst.Register(l => l.Get()).Return(addin.Instance);
                        loader.Register(() => AddinPreloader.Current).Return(inst.Instance);
                        addin.Register(a => a.InitWorkflow(Argument<string>.Any, Argument<string>.Any, Argument<bool>.Any)).Execute(() =>
                        {
                            isInit = true;
                        });
                        addin.Register(a => a.ImportAssemblies(imports, true)).Return(true);
                        addin.Register(a => a.RefreshTasks()).Execute(() => { isInit = true; });

                        host = new DesignerHostAdapters(name, xaml, imports, false);
                        action();
                        Assert.IsTrue(isInit);
                    }
                }
            }
        }

        [TestMethod]
        [Owner("v-kason")]
        [TestCategory("UnitTest")]
        public void DesignerHostAdapters_TestProperty()
        {
            using (addin = new Implementation<DesignerAddIn>())
            {
                bool isSave = false;
                addin.Register(inst => inst.SaveWorkflowToBitmap()).Execute(() => { isSave = true; });
                addin.Register(inst => inst.SaveWorkflowToBitmap(Argument<string>.Any)).Execute(() => { isSave = true; });
                addin.Register(inst => inst.CompiledXaml).Return("xaml");
                addin.Register(inst => inst.CompileProject).Return(new CompileProject());
                addin.Register(inst => inst.WorkflowEditorView).Return(null);
                addin.Register(inst => inst.ProjectExplorerView).Return(null);
                addin.Register(inst => inst.ToolboxView).Return(null);
                addin.Register(inst => inst.WorkflowPropertyView).Return(null);
                addin.Register(inst => inst.PrintState).Return(PrintAction.PrintAll);
                addin.Register(inst => inst.IsWorkflowService()).Return(false);

                using (var adapter = new ImplementationOfType(typeof(FrameworkElementAdapters)))
                {
                    FrameworkElement e = new FrameworkElement();
                    adapter.Register(() => FrameworkElementAdapters.ContractToViewAdapter(null)).Return(e);

                    DesignerHostAdapters host = new DesignerHostAdapters();
                    PrivateObject po = new PrivateObject(host);
                    po.SetFieldOrProperty("proxy", addin.Instance);

                    host.SaveWorkflowToBitmap();
                    host.SaveWorkflowToBitmap("file");
                    Assert.AreEqual("xaml", host.CompiledXaml);
                    Assert.IsNotNull(host.CompileProject);
                    Assert.IsTrue(isSave);
                    Assert.AreEqual(host.View, e);
                    Assert.AreEqual(host.PropertyInspectorView, e);
                    Assert.AreEqual(host.ProjectExplorerView, e);
                    Assert.AreEqual(host.ToolboxView, e);
                    Assert.AreEqual(host.PrintState, PrintAction.PrintAll);
                    Assert.IsFalse(host.IsWorkflowService);
                    Assert.IsNull(host.InitializeLifetimeService());
                }
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [Owner("v-kason")]
        public void DesignerHostAdapters_RaiseEventTest()
        {
            DesignerHostAdapters host = new DesignerHostAdapters();

            bool isDesignerChanged = false;
            host.DesignerChanged += (s, e) => { isDesignerChanged = true; };

            bool isPrintStateChanged = false;
            host.PrintStateChanged += (s, e) => { isPrintStateChanged = true; };

            bool isDesignerReloaded = false;
            host.DesignerReloaded += (s, e) => { isDesignerReloaded = true; };

            PrivateObject po = new PrivateObject(host);
            object o = new object();

            EventArgs ev = new EventArgs();
            po.Invoke("OnPrintStateChanged", o, ev);
            po.Invoke("OnDesignerChanged", o, ev);
            po.Invoke("OnDesignerReloaded", o, ev);
            Assert.IsTrue(isDesignerChanged);
            Assert.IsTrue(isDesignerReloaded);
            Assert.IsTrue(isPrintStateChanged);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestSetReadOnly()
        {
            bool isExcute = false;
            Init(() =>
                {
                    addin.Register(a => a.SetReadOnly(Argument<bool>.Any)).Execute(() =>
                    {
                        isExcute = true;
                    });

                    host.SetReadOnly(true);
                });

            Assert.IsTrue(isExcute);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestUnloadAddIn()
        {
            bool isExcute = false;
            Init(() =>
            {
                inst.Register(l => l.Unload(addin.Instance)).Execute(() =>
                {
                    isExcute = true;
                });

                host.UnloadAddIn();
            });

            Assert.IsTrue(isExcute);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestReloadAddIn()
        {
            bool isExcute = false;
            Init(() =>
            {
                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                addin.Register(a => a.Xaml).Return(xaml);
                addin.Register(a => a.DependencyAssemblies).Return(dependecy);
                inst.Register(l => l.Unload(addin.Instance)).Execute(() =>
                {
                    isExcute = true;
                });

                addin.Register(a => a.InitWorkflow(Argument<string>.Any, Argument<string>.Any, Argument<bool>.Any)).Execute(() =>
                {
                    isExcute = true;
                });
                addin.Register(a => a.ImportAssemblies(dependecy, true)).Return(true);

                host.ReloadAddIn();
            });

            Assert.IsTrue(isExcute);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestImportAssemblies()
        {
            bool isExcute = false;
            Init(() =>
            {
                List<ActivityAssemblyItem> dependecy = new List<ActivityAssemblyItem>();
                addin.Register(a => a.ImportAssemblies(imports, true)).Return(false);
                addin.Register(a => a.Xaml).Return(xaml);
                addin.Register(a => a.DependencyAssemblies).Return(dependecy);
                inst.Register(l => l.Unload(addin.Instance)).Execute(() =>
                {
                    isExcute = true;
                });

                host.ImportAssemblies(imports);
            });

            Assert.IsTrue(isExcute);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestUndo()
        {
            bool isExcute = false;
            Init(() =>
            {
                addin.Register(a => a.Undo()).Execute(() =>
                {
                    isExcute = true;
                });

                host.Undo();
            });

            Assert.IsTrue(isExcute);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestCanUndo()
        {
            Init(() =>
            {
                addin.Register(a => a.CanUndo()).Return(true);
                Assert.IsTrue(host.CanUndo());
            });
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestRedo()
        {
            bool isExcute = false;
            Init(() =>
            {
                addin.Register(a => a.Redo()).Execute(() =>
                {
                    isExcute = true;
                });

                host.Redo();
            });
            Assert.IsTrue(isExcute);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestCanRedo()
        {
            Init(() =>
            {
                addin.Register(a => a.CanRedo()).Return(true);
                Assert.IsTrue(host.CanRedo());
            });
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestCut()
        {
            bool isExcute = false;
            Init(() =>
            {
                addin.Register(a => a.Cut()).Execute(() =>
                {
                    isExcute = true;
                });

                host.Cut();
            });
            Assert.IsTrue(isExcute);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestCanCut()
        {
            Init(() =>
            {
                addin.Register(a => a.CanCut()).Return(true);
                Assert.IsTrue(host.CanCut());
            });
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestCopy()
        {
            bool isExcute = false;
            Init(() =>
            {
                addin.Register(a => a.Copy()).Execute(() =>
                {
                    isExcute = true;
                });

                host.Copy();
            });
            Assert.IsTrue(isExcute);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestCanCopy()
        {
            Init(() =>
            {
                addin.Register(a => a.CanCopy()).Return(true);
                Assert.IsTrue(host.CanCopy());
            });
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestPaste()
        {
            bool isExcute = false;
            Init(() =>
            {
                addin.Register(a => a.Paste()).Execute(() =>
                {
                    isExcute = true;
                });

                host.Paste();
            });
            Assert.IsTrue(isExcute);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestCanPaste()
        {
            Init(() =>
            {
                addin.Register(a => a.CanPaste()).Return(true);
                Assert.IsTrue(host.CanPaste());
            });
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestPrint()
        {
            bool isExcute = false;
            Init(() =>
            {
                addin.Register(a => a.Print()).Execute(() =>
                {
                    isExcute = true;
                });

                host.Print();
            });
            Assert.IsTrue(isExcute);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestPrintAll()
        {
            bool isExcute = false;
            Init(() =>
            {
                addin.Register(a => a.PrintAll()).Execute(() =>
                {
                    isExcute = true;
                });

                host.PrintAll();
            });
            Assert.IsTrue(isExcute);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestSetWorkflowName()
        {
            bool isExcute = false;
            Init(() =>
            {
                addin.Register(a => a.SetWorkflowName(Argument<string>.Any)).Execute(() =>
                {
                    isExcute = true;
                });

                host.SetWorkflowName("Test");
            });
            Assert.IsTrue(isExcute);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestSave()
        {
            bool isExcute = false;
            Init(() =>
            {
                addin.Register(a => a.Save(Argument<string>.Any)).Execute(() =>
                {
                    isExcute = true;
                });

                host.Save("Test");
            });
            Assert.IsTrue(isExcute);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestRefresh()
        {
            bool isExcute = false;
            Init(() =>
            {
                addin.Register(a => a.RefreshDesignerFromXamlCode()).Execute(() =>
                {
                    isExcute = true;
                });

                host.RefreshDesignerFromXamlCode();
            });
            Assert.IsTrue(isExcute);
        }

        [TestMethod]
        [Owner("v-bobo")]
        [TestCategory("Unit-Dif")]
        public void DesignerHostAdapters_TestIsValid()
        {
            Init(() =>
            {
                addin.Register(a => a.IsWorkflowValid()).Return(true);
                Assert.IsTrue(host.IsValid());
            });

        }
    }
}
