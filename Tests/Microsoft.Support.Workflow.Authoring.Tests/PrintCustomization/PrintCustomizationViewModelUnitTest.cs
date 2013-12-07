using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.PrintCustomization;
using System.Windows.Controls;
using System.Activities.Presentation;
using System.Windows;
using System.Activities.Presentation.Model;
using System.Activities.Statements;
using Microsoft.Support.Workflow.Authoring.CompositeActivity;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Threading;
using System.Printing;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using System.Windows.Threading;

namespace Microsoft.Support.Workflow.Authoring.Tests.PrintCustomization
{
    [TestClass]
    public class PrintCustomizationViewModelUnitTest
    {

        [TestMethod()]
        [WorkItem(358126)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void aaaaPrintCustomization_InitializeDraggingTest()
        {
            Canvas panel = new Canvas();
            List<ActivityDesigner> designers = new List<ActivityDesigner>();
            var sequence1 = new Sequence();

            var workflowDesigner = new WorkflowDesigner();
            workflowDesigner.Load(sequence1);
            var service = workflowDesigner.GetModelService();
            var activityDesigner = service.Root.View as ActivityDesigner;
            designers.Add(activityDesigner);

            var sequence2 = new Sequence();
            var workflowDesigner2 = new WorkflowDesigner();
            workflowDesigner2.Load(sequence2);
            designers.Add(workflowDesigner.GetModelService().Root.View as ActivityDesigner);

            Window window = new Window();
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Content = panel;
            window.Content = scrollViewer;

            using (var addInMessageBoxService = new ImplementationOfType(typeof(AddInMessageBoxService)))
            {
                using (var printHelper = new Implementation<PrintHelper>())
                {
                    bool isPrint = false;
                    addInMessageBoxService.Register(() =>
                        AddInMessageBoxService.PrintConfirmation(Argument<int>.Any,
                        Argument<string>.Any,
                        Argument<string>.Any)).
                        Return(true);
                    addInMessageBoxService.Register(() => AddInMessageBoxService.PrintReselectConfirmation()).Return(true);
                    printHelper.Register(p => p.Print(Argument<PrintQueue>.Any, Argument<List<Rectangle>>.Any)).Execute(() =>
                    {
                        isPrint = true;
                    });
                    printHelper.Register(p => p.OpenPrintOptions(Argument<Window>.Any, Argument<PrintQueue>.Any)).Return(true);

                    PrintCustomizationViewModel.CreatePrintHelperFunc = () => { return printHelper.Instance; };
                    PrintCustomizationViewModel printCustomizationViewModel = new PrintCustomizationViewModel();
                    Assert.IsTrue(printCustomizationViewModel.PrintHelper != null);

                    window.Loaded += ((s, e) =>
                    {
                        printCustomizationViewModel.InitializeDragging(panel, designers);

                        var print = printCustomizationViewModel.PrintHelper.Printers.FirstOrDefault(p => p.Value.IsDefaultPrinter).Key;
                        Assert.AreEqual(print.Name, printCustomizationViewModel.CurrentPrinter.Name);

                        var viewModes = printCustomizationViewModel.ViewModes;
                        Assert.IsTrue(viewModes.Any(v => v.Value == PrintViewMode.ActualSize));
                        Assert.IsTrue(viewModes.Any(v => v.Value == PrintViewMode.FitToWindow));

                        printCustomizationViewModel.IsSettingEnabled = true;
                        Assert.IsTrue(printCustomizationViewModel.IsSettingEnabled);

                        printCustomizationViewModel.ViewMode = PrintViewMode.ActualSize;
                        Assert.IsTrue(printCustomizationViewModel.ViewMode == PrintViewMode.ActualSize);

                        printCustomizationViewModel.IncludeXaml = true;
                        Assert.IsTrue(printCustomizationViewModel.IncludeXaml);

                        printCustomizationViewModel.OpenPrinterOption.CanExecute();
                        printCustomizationViewModel.OpenPrinterOption.Execute();

                        printHelper.Register(p => p.GetPageOrientation(Argument<PrintQueue>.Any)).Return(PageOrientation.Landscape);
                        int loadedCount = 0;
                        PrivateObject obj = new PrivateObject(printCustomizationViewModel);
                        int elementCount = (int)obj.GetField("elementCount");
                        foreach (FrameworkElement ele in panel.Children)
                        {
                            ele.Loaded += (eleSender, eleArgs) =>
                            {
                                loadedCount++;
                                if (loadedCount == elementCount)
                                {
                                    printCustomizationViewModel.OpenPrinterOption.CanExecute();
                                    printCustomizationViewModel.OpenPrinterOption.Execute();

                                    printCustomizationViewModel.Print.CanExecute();
                                    printCustomizationViewModel.Print.Execute();
                                    Assert.IsTrue(isPrint);

                                    printCustomizationViewModel.View = window;
                                    printCustomizationViewModel.Back.CanExecute();
                                    printCustomizationViewModel.Back.Execute();
                                }
                            };
                        }

                        printCustomizationViewModel.ViewMode = PrintViewMode.FitToWindow;
                        window.Close();
                    });
                    window.ShowDialog();
                }
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown();
        }
    }
}
