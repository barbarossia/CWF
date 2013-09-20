using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.PrintCustomization;
using System.Drawing.Printing;
using Microsoft.DynamicImplementations;
using System.Printing;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Microsoft.Support.Workflow.Authoring.Tests.PrintCustomization
{
    [TestClass]
    public class PrintHelperUnitTest
    {
        [TestMethod()]
        [WorkItem(357085)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void PrintHelper_ConstructorTest()
        {
            PrintHelper printHelper = new PrintHelper();
            PrivateObject privateObject = new PrivateObject(printHelper);
            var paperSizes = privateObject.GetField("paperSizes") as List<PaperSize>;
            
            Assert.IsTrue(paperSizes.Count > 0);
            Assert.IsTrue(printHelper.Printers.Count > 0);
            
        }

        [TestMethod()]
        [WorkItem(357088)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void PrintHelper_GetPageMediaSizeTest()
        {
            PrintHelper printHelper = new PrintHelper();

            var printer = printHelper.Printers.Single(p => p.Key.FullName == "Microsoft XPS Document Writer");
            var pageMediaSize = printHelper.GetPageMediaSize(printer.Key);
            Assert.AreEqual(printer.Key.UserPrintTicket.PageMediaSize.Height, pageMediaSize.Height);
            Assert.AreEqual(printer.Key.UserPrintTicket.PageMediaSize.Width, pageMediaSize.Width);
            Assert.AreEqual(printer.Key.UserPrintTicket.PageMediaSize.PageMediaSizeName, pageMediaSize.PageMediaSizeName);
        }

        [TestMethod()]
        [WorkItem(357504)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void PrintHelper_GetPaperSizeTest()
        {
            PrintHelper printHelper = new PrintHelper();
            var printer = printHelper.Printers.Single(p => p.Key.FullName == "Microsoft XPS Document Writer");
            var pagerSize = printHelper.GetPaperSize(printer.Key);

            PrivateObject privateObject = new PrivateObject(printHelper);
            var paperSizes = privateObject.GetField("paperSizes") as List<PaperSize>;
            Assert.IsTrue(paperSizes.Contains(pagerSize));
        }

        [TestMethod()]
        [WorkItem(357521)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void PrintHelper_PrintTest()
        {
            
            using(var printDialog = new Implementation<PrintDialog>())
            {
                var flag = false;
                printDialog.Register(p => p.PrintDocument(Argument<DocumentPaginator>.Any, Argument<string>.Any)).Execute(() => { flag = true; });
                PrintHelper printHelper = new PrintHelper();
                PrintHelper.CreatePrintDialogFunc = () =>
                {
                  return printDialog.Instance;
                };
                var printer = printHelper.Printers.Single(p => p.Key.FullName == "Microsoft XPS Document Writer");
                List<Rectangle> rects = new List<Rectangle>();
                rects.Add(new Rectangle() { Width = 100, Height = 100 });
                rects.Add(new Rectangle() { Width = 200, Height = 200 });
                rects.Add(new Rectangle() { Width = 300, Height = 300 });
                rects.Add(new Rectangle() { Width = 400, Height = 400 });
                printHelper.Print(printer.Key, rects);
                Assert.IsTrue(flag);
            }
            
        }

        [TestMethod()]
        [WorkItem(357525)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void PrintHelper_OpenPrintOptionsTest()
        {
            PrintHelper printHelper = new PrintHelper();
         
           
   
            PrintHelper.DocumentPropertiesFunc = (IntPtr hwnd, IntPtr hPrinter, string pDeviceName, IntPtr pDevModeOutput, IntPtr pDevModeInput, int fMode) =>
            {
                return 1;
            };
            Window mainWindow = new Window();
            var printer = printHelper.Printers.Single(p => p.Key.FullName == "Microsoft XPS Document Writer");
            
            var flag = printHelper.OpenPrintOptions(mainWindow, printer.Key);
            Assert.IsTrue(flag);

            PrintHelper.DocumentPropertiesFunc = (IntPtr hwnd, IntPtr hPrinter, string pDeviceName, IntPtr pDevModeOutput, IntPtr pDevModeInput, int fMode) =>
            {
                return 2; 
            };
            flag = printHelper.OpenPrintOptions(mainWindow, printer.Key);
            Assert.IsFalse(flag);

            TestUtilities.Assert_ShouldThrow<ArgumentNullException>(() =>
            {
                printHelper.OpenPrintOptions(null, printer.Key);
            });
           
        }

        [TestMethod()]
        [WorkItem(357721)]
        [Owner("v-toy")]
        [TestCategory("Unit")]
        public void PrintHelper_GetPageOrientationTest()
        {
            PrintHelper printHelper = new PrintHelper();
            var printer = printHelper.Printers.Single(p => p.Key.FullName == "Microsoft XPS Document Writer");

            var pageOrientation = printHelper.GetPageOrientation(printer.Key);

            var pageOrientationExpected = printer.Key.UserPrintTicket.PageOrientation.HasValue ?
                printer.Key.UserPrintTicket.PageOrientation.Value : PageOrientation.Portrait;

            Assert.AreEqual(pageOrientationExpected, pageOrientation);
        }
    }
}
